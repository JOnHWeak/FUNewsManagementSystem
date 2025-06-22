using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObjects.DAO
{
    public class SystemAccountDAO
    {
        public static async Task<SystemAccount?> GetSystemAccountByIdAsync(short accountID)
        {
            using var db = new FunewsManagementContext();
            return await db.SystemAccounts.FirstOrDefaultAsync(c => c.AccountId == accountID);
        }

        public static async Task<SystemAccount?> GetSystemAccountByEmailAsync(string email)
        {
            using var db = new FunewsManagementContext();
            return await db.SystemAccounts.FirstOrDefaultAsync(c => c.AccountEmail == email);
        }

        public static async Task<List<SystemAccount>> GetAllSystemAccountsAsync()
        {
            using var db = new FunewsManagementContext();
            return await db.SystemAccounts.ToListAsync();
        }

        public static async Task<List<SystemAccount>> GetSystemAccountsByRoleAsync(int role)
        {
            using var db = new FunewsManagementContext();
            return await db.SystemAccounts
                          .Where(a => a.AccountRole == role)
                          .ToListAsync();
        }

        public static async Task UpdateAccountAsync(SystemAccount updatedAccount)
        {
            using var db = new FunewsManagementContext();
            var existingAccount = await db.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == updatedAccount.AccountId);
            if (existingAccount == null)
            {
                throw new KeyNotFoundException($"SystemAccount with ID {updatedAccount.AccountId} not found.");
            }

            existingAccount.AccountName = updatedAccount.AccountName;
            existingAccount.AccountEmail = updatedAccount.AccountEmail;
            existingAccount.AccountPassword = updatedAccount.AccountPassword;
            existingAccount.AccountRole = updatedAccount.AccountRole;

            await db.SaveChangesAsync();
        }

        public static async Task CreateSystemAccountAsync(SystemAccount account)
        {
            using var db = new FunewsManagementContext();

            // Check if email already exists (better than ID check since ID is auto-generated)
            var existingAccount = await db.SystemAccounts.AnyAsync(a => a.AccountEmail == account.AccountEmail);
            if (existingAccount)
            {
                throw new InvalidOperationException($"SystemAccount with Email {account.AccountEmail} already exists.");
            }

            await db.SystemAccounts.AddAsync(account);
            await db.SaveChangesAsync();
        }

        public static async Task DeleteSystemAccountAsync(short accountId)
        {
            using var db = new FunewsManagementContext();
            var account = await db.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
            {
                throw new KeyNotFoundException($"SystemAccount with ID {accountId} not found.");
            }

            db.SystemAccounts.Remove(account);
            await db.SaveChangesAsync();
        }

        public static async Task<SystemAccount?> GetAccountProfileAsync(short accountId)
        {
            using var db = new FunewsManagementContext();
            return await db.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        // ⭐ NEW: Check if account has created any news articles
        public static async Task<bool> HasCreatedNewsArticlesAsync(short accountId)
        {
            using var db = new FunewsManagementContext();
            return await db.NewsArticles.AnyAsync(na => na.CreatedById == accountId);
        }

        // ⭐ NEW: Get account with news articles count
        public static async Task<SystemAccount?> GetAccountWithNewsCountAsync(short accountId)
        {
            using var db = new FunewsManagementContext();
            return await db.SystemAccounts
                          .Include(a => a.NewsArticles)
                          .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }
    }
}
