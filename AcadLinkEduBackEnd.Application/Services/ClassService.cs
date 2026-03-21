using AcadLinkEduBackEnd.Application.Dtos;
using AcadLinkEduBackEnd.Domain.DTO;
using AcadLinkEduBackEnd.Domain.Entities;
using AcadLinkEduBackEnd.Infrastructure;
using Supabase;
using Supabase.Postgrest;

namespace AcadLinkEduBackEnd.Application.Services;

public class ClassService
{
    private readonly Supabase.Client _supabase;

    public ClassService(SupabaseService supabaseService)
    {
        _supabase = supabaseService.Client;
    }

    public async Task<List<ClassTrackingDto>> GetClassTrackingAsync(int classId)
    {
        // enrollments for class
        var enrollResp = await _supabase.From<Domain.Entities.Enrollment>().Where(e => e.ClassId == classId).Get();
        var classEnrollments = enrollResp.Models;

        // activities for class
        var activitiesResp = await _supabase.From<Domain.Entities.Activity>().Where(a => a.ClassId == classId).Get();
        var classActivities = activitiesResp.Models;

        // users
        var usersResp = await _supabase.From<Domain.Entities.User>().Get();
        var users = usersResp.Models;

        // submissions
        var subsResp = await _supabase.From<Domain.Entities.Submission>().Get();
        var submissions = subsResp.Models;

        var result = new List<ClassTrackingDto>();

        foreach (var e in classEnrollments)
        {
            var student = users.FirstOrDefault(u => u.Id == e.StudentId);
            var studentSubmissions = submissions.Where(s => s.StudentId == e.StudentId).ToList();

            var activityStatuses = classActivities.Select(a => {
                var submission = studentSubmissions.FirstOrDefault(s => s.ActivityId == a.Id);
                return new ClassTrackingActivityDto
                {
                    ActivityId = a.Id,
                    ActivityTitle = a.Title,
                    Status = submission != null ? "submitted" : "pending",
                    SubmittedAt = submission?.SubmittedAt
                };
            }).ToList();

            result.Add(new ClassTrackingDto
            {
                StudentId = e.StudentId,
                StudentName = student?.Name,
                StudentEmail = student?.Email,
                Activities = activityStatuses
            });
        }

        return result;
    }
    // Get all classes with optional student stats/enrollment
    public async Task<List<ClassDto>> GetAllAsync(int? studentId = null)
    {
        var classesResp = await _supabase.From<Class>().Get();
        var classes = classesResp.Models;

        var usersResp = await _supabase.From<User>().Get();
        var users = usersResp.Models;

        var activitiesResp = await _supabase.From<Activity>().Get();
        var activities = activitiesResp.Models;

        var enrollmentsResp = await _supabase.From<Enrollment>().Get();
        var enrollments = enrollmentsResp.Models;

        var submissionsResp = await _supabase.From<Submission>().Get();
        var submissions = submissionsResp.Models;

        var result = new List<ClassDto>();

        foreach (var c in classes)
        {
            var teacher = users.FirstOrDefault(u => u.Id == c.TeacherId);
            var classMissions = activities.Where(a => a.ClassId == c.Id).ToList();

            ClassStats? stats = null;
            if (studentId.HasValue)
            {
                var studentSubmissions = submissions.Where(s => s.StudentId == studentId.Value).ToList();
                var submittedIds = studentSubmissions.Select(s => s.ActivityId).ToList();
                var submittedCount = classMissions.Count(a => submittedIds.Contains((int)a.Id));
                stats = new ClassStats
                {
                    Total = classMissions.Count,
                    Submitted = submittedCount,
                    Pending = classMissions.Count - submittedCount
                };
            }

            result.Add(new ClassDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                TeacherId = c.TeacherId,
                TeacherName = teacher?.Name ?? "Unknown Authority",
                InviteCode = c.InviteCode,
                IsEnrolled = studentId.HasValue && enrollments.Any(e => e.StudentId == studentId.Value && e.ClassId == c.Id),
                Stats = stats
            });
        }

        return result;
    }

    public async Task<ClassDto?> GetByIdAsync(int id, int? studentId = null)
    {
        var classResp = await _supabase.From<Class>().Where(c => c.Id == id).Get();
        var c = classResp.Models.FirstOrDefault();
        if (c == null) return null;

        var teacherResp = await _supabase.From<User>().Where(u => u.Id == c.TeacherId).Get();
        var teacher = teacherResp.Models.FirstOrDefault();

        // gather related data only if studentId requested
        ClassStats? stats = null;
        bool isEnrolled = false;

        if (studentId.HasValue)
        {
            var activitiesResp = await _supabase.From<Activity>().Where(a => a.ClassId == c.Id).Get();
            var activities = activitiesResp.Models;

            var submissionsResp = await _supabase.From<Submission>().Where(s => s.StudentId == studentId.Value).Get();
            var submissions = submissionsResp.Models;

            var studentSubmissions = submissions.Where(s => activities.Select(a => a.Id).Contains(s.ActivityId)).ToList();
            var submittedIds = studentSubmissions.Select(s => s.ActivityId).ToList();
            var submittedCount = activities.Count(a => submittedIds.Contains((int)a.Id));

            stats = new ClassStats
            {
                Total = activities.Count,
                Submitted = submittedCount,
                Pending = activities.Count - submittedCount
            };

            var enrollResp = await _supabase.From<Enrollment>().Where(e => e.StudentId == studentId.Value && e.ClassId == c.Id).Get();
            isEnrolled = enrollResp.Models.Any();
        }

        return new ClassDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            TeacherId = c.TeacherId,
            TeacherName = teacher?.Name ?? "Unknown Authority",
            InviteCode = c.InviteCode,
            IsEnrolled = isEnrolled,
            Stats = stats
        };
    }

    public async Task<Class> CreateClassAsync(string name, string description, int teacherId)
    {
        var inviteCode = Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper();

        var newClass = new Class
        {
            Name = name,
            Description = description,
            TeacherId = teacherId,
            InviteCode = inviteCode
        };

        var response = await _supabase.From<Class>().Insert(newClass);
        return response.Models.First();
    }

    public async Task<Enrollment> JoinClassAsync(JoinClassRequest request)
    {
        var classesResp = await _supabase.From<Class>().Where(c => c.InviteCode == request.InviteCode).Get();
        var targetClass = classesResp.Models.FirstOrDefault();
        if (targetClass == null)
            throw new KeyNotFoundException("Invalid invite code");

        var enrollResp = await _supabase.From<Enrollment>()
            .Where(e => e.StudentId == request.StudentId && e.ClassId == targetClass.Id)
            .Get();
        if (enrollResp.Models.Any())
            throw new InvalidOperationException("Already joined");

        var newEnrollment = new Enrollment
        {
            StudentId = (int)request.StudentId,
            ClassId = targetClass.Id
        };

        var insertResp = await _supabase.From<Enrollment>().Insert(newEnrollment);
        return insertResp.Models.First();
    }

    public async Task<Class> CreateAsync(ClassRequest c)
    {
        var data = new Class
        {
            Name = c.Name,
            Description = c.Description,
            TeacherId = c.TeacherId,
            InviteCode = Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper()
        };

        var response = await _supabase.From<Class>().Insert(data);
        return response.Models.First();
    }

    public async Task<Class> UpdateAsync(int id, Class data)
    {
        var resp = await _supabase.From<Class>().Where(x => x.Id == id).Update(data);
        var updated = resp.Models.FirstOrDefault();
        if (updated == null) throw new KeyNotFoundException("Class not found");
        return updated;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _supabase.From<Class>().Where(x => x.Id == id).Get();
        if (!existing.Models.Any()) return false;

        await _supabase.From<Class>().Where(x => x.Id == id).Delete();
        var check = await _supabase.From<Class>().Where(x => x.Id == id).Get();
        return !check.Models.Any();
    }

    public Task<List<ClassDto>> GetForStudentAsync(int studentId)
    {
        return GetAllAsync(studentId);
    }

    public async Task<AnalyticsDto> GetAnalyticsAsync(int teacherId)
    {
        // get classes for teacher
        var classesResp = await _supabase.From<Domain.Entities.Class>().Where(c => c.TeacherId == teacherId).Get();
        var teacherClasses = classesResp.Models;
        var classIds = teacherClasses.Select(c => c.Id).Where(id => id.HasValue).Select(id => id!.Value).ToList();

        // get activities for those classes
        var activitiesResp = await _supabase.From<Domain.Entities.Activity>().Get();
        var activities = activitiesResp.Models.Where(a => a.ClassId.HasValue && classIds.Contains(a.ClassId.Value)).ToList();
        var activityIds = activities.Select(a => a.Id).Where(id => id.HasValue).Select(id => id!.Value).ToList();

        // get submissions for those activities
        var submissionsResp = await _supabase.From<Domain.Entities.Submission>().Get();
        var submissions = submissionsResp.Models.Where(s => activityIds.Contains((int)s.ActivityId)).ToList();

        // get enrollments for those classes
        var enrollResp = await _supabase.From<Domain.Entities.Enrollment>().Get();
        var enrollments = enrollResp.Models.Where(e => e.ClassId.HasValue && classIds.Contains(e.ClassId.Value)).ToList();

        var totalClasses = teacherClasses.Count;
        var totalStudents = enrollments.Count;
        var totalSubmissions = submissions.Count;

        var completionRate = 0;
        if (activities.Count > 0)
        {
            // original JS scaled by 10 per activity, replicating that behavior
            completionRate = (int)Math.Round((double)totalSubmissions / (activities.Count * 10) * 100);
        }

        return new AnalyticsDto
        {
            TotalClasses = totalClasses,
            TotalStudents = totalStudents,
            TotalSubmissions = totalSubmissions,
            CompletionRate = completionRate
        };
    }
}
