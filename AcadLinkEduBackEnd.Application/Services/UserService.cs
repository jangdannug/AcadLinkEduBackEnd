using AcadLinkEduBackEnd.Domain.Entities;
using AcadLinkEduBackEnd.Infrastructure;
using Supabase;
using Supabase.Postgrest;

namespace AcadLinkEduBackEnd.Application.Services
{
    public class UserService
    {
        private readonly Supabase.Client _supabase;

        public UserService(SupabaseService supabaseService)
        {
            _supabase = supabaseService.Client;
        }


        public async Task<User> LoginAsync(string email)
        {
            // Find user by email
            var response = await _supabase.From<User>().Where(u => u.Email == email).Get();
            var user = response.Models.FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException("User not found");

            return user;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _supabase.From<User>().Get();
            return response.Models;
        }

        public async Task<User> RegisterAsync(string email, string name, string role)
        {
            // Check if email already exists (mock API throws if already registered)
            var existing = await _supabase.From<User>().Where(u => u.Email == email).Get();
            if (existing.Models.Any())
                throw new InvalidOperationException("Email already registered");

            var newUser = new User
            {
                Email = email,
                Name = name,
                Role = role,
                IsVerified = false
            };

            var response = await _supabase.From<User>().Insert(newUser);
            return response.Models.First();
        }

        public async Task<User> VerifyUserAsync(int userId)
        {

            var user = (await _supabase
                     .From<User>()
                     .Where(u => u.Id == userId)
                     .Get())
                     .Models
                     .FirstOrDefault(); // safe

            if (user == null)
            {
                // Clear exception, frontend can display this message
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            // Mark user as verified
            user.IsVerified = true;

            await _supabase
                    .From<User>()
                    .Where(u => u.Id == userId)
                    .Set(u => u.IsVerified, true)
                    .Update();


            return user;
        }

        public async Task<User> RevokeUserAsync(int userId)
        {

            var user = (await _supabase
                     .From<User>()
                     .Where(u => u.Id == userId)
                     .Get())
                     .Models
                     .FirstOrDefault(); // safe

            if (user == null)
            {
                // Clear exception, frontend can display this message
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            // Mark user as verified
            user.IsVerified = false;

            await _supabase
                    .From<User>()
                    .Where(u => u.Id == userId)
                    .Set(u => u.IsVerified, false)
                    .Update();


            return user;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            // Check existence first
            var existing = await _supabase.From<User>().Where(u => u.Id == userId).Get();
            if (!existing.Models.Any())
                return false;

            // Attempt to delete the user. Await the delete operation (some clients return a response).
            await _supabase.From<User>().Where(u => u.Id == userId).Delete();

            // Confirm deletion
            var check = await _supabase.From<User>().Where(u => u.Id == userId).Get();
            return !check.Models.Any();
        }

        public async Task<bool> BatchVerifyUsersAsync(IEnumerable<int> userIds)
        {
            if (userIds == null) throw new ArgumentNullException(nameof(userIds));

            var ids = userIds.ToArray();
            if (!ids.Any()) return false;

            // Update each user individually to mark as verified
            foreach (var id in ids)
            {
                await _supabase.From<User>().Where(u => u.Id == id).Update(new User { IsVerified = true });
            }

            return true;
        }
    }
}