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

    public async Task<Enrollment> JoinClassAsync(int studentId, string inviteCode)
    {
        var classesResp = await _supabase.From<Class>().Where(c => c.InviteCode == inviteCode).Get();
        var targetClass = classesResp.Models.FirstOrDefault();
        if (targetClass == null)
            throw new KeyNotFoundException("Invalid invite code");

        var enrollResp = await _supabase.From<Enrollment>()
            .Where(e => e.StudentId == studentId && e.ClassId == targetClass.Id)
            .Get();
        if (enrollResp.Models.Any())
            throw new InvalidOperationException("Already joined");

        var newEnrollment = new Enrollment
        {
            StudentId = studentId,
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
}
