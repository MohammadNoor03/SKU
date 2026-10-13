using Microsoft.EntityFrameworkCore;
using SKU.DbContexts;
using SKU.Models;
using System.Security.Cryptography;
using System.Text;

namespace SKU.Services
{
    public class VerificationService
    {
        public static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ResendDelay = TimeSpan.FromSeconds(60);
        private const int MaxAttempts = 5;

        private readonly SKUContext _db;

        public VerificationService(SKUContext db)
        {
            _db = db;
        }

        // Returns the new code, or null if a code was requested less than 60 seconds ago.
        public async Task<string?> IssueAsync(long userId, VerificationPurpose purpose, int length)
        {
            var now = DateTime.UtcNow;

            var lastCreatedAt = await _db.Sec_VerificationCode
                .Where(x => x.UserId == userId && x.Purpose == purpose)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => (DateTime?)x.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastCreatedAt != null && now - lastCreatedAt.Value < ResendDelay)
            {
                return null;
            }

            // Invalidate any previous unused codes of the same purpose
            var oldCodes = await _db.Sec_VerificationCode
                .Where(x => x.UserId == userId && x.Purpose == purpose && !x.IsUsed)
                .ToListAsync();
            foreach (var old in oldCodes)
            {
                old.IsUsed = true;
            }

            var max = (int)Math.Pow(10, length);
            var code = RandomNumberGenerator.GetInt32(max).ToString($"D{length}");

            _db.Sec_VerificationCode.Add(new VerificationCode
            {
                Id = 0,
                UserId = userId,
                Code = code,
                Purpose = purpose,
                CreatedAt = now,
                ExpiresAt = now.Add(CodeLifetime),
                Attempts = 0,
                IsUsed = false
            });
            await _db.SaveChangesAsync();

            return code;
        }

        // True only if the code is correct, not expired, not used, and attempts are not exhausted.
        public async Task<bool> VerifyAsync(long userId, VerificationPurpose purpose, string code)
        {
            var now = DateTime.UtcNow;

            var entry = await _db.Sec_VerificationCode
                .Where(x => x.UserId == userId && x.Purpose == purpose && !x.IsUsed && x.ExpiresAt > now)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (entry == null || entry.Attempts >= MaxAttempts)
            {
                return false;
            }

            var match = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(entry.Code),
                Encoding.UTF8.GetBytes(code));

            if (!match)
            {
                entry.Attempts++;
                await _db.SaveChangesAsync();
                return false;
            }

            entry.IsUsed = true;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}