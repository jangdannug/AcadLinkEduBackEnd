using AcadLinkEduBackEnd.Domain.Entities;
using AcadLinkEduBackEnd.Infrastructure;
using Supabase;
using Supabase.Postgrest;

namespace AcadLinkEduBackEnd.Application.Services;

public class SubmissionService
{
    private readonly Supabase.Client _supabase;

    public SubmissionService(SupabaseService supabaseService)
    {
        _supabase = supabaseService.Client;
    }

    public async Task<List<Submission>> GetSubmissionsAsync(int? studentId = null)
    {
        if (studentId.HasValue)
        {
            var resp = await _supabase.From<Submission>().Where(s => s.StudentId == studentId.Value).Get();
            return resp.Models.ToList();
        }

        var response = await _supabase.From<Submission>().Get();
        return response.Models.ToList();
    }

    public async Task<Submission> CreateSubmissionAsync(int activityId, int studentId, string fileUrl, string fileName)
    {
        var newSubmission = new Submission
        {
            ActivityId = activityId,
            StudentId = studentId,
            FileUrl = fileUrl,
            FileName = fileName,
            SubmittedAt = DateTime.UtcNow,
            Status = "finished"
        };

        var response = await _supabase.From<Submission>().Insert(newSubmission);
        return response.Models.First();
    }
}
