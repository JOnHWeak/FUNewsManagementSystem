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
        public static async Task<SystemAccount> GetSystemAccountByIdAsync(short accountID)
        {
            using var db = new FunewsManagementContext();
            var account = await db.SystemAccounts.FirstOrDefaultAsync(c => c.AccountId == accountID);
            if (account == null)
            {
                throw new Exception($"SystemAccount with ID {accountID} not found.");
            }
            return account;
        }

        public static async Task<SystemAccount> GetSystemAccountByEmailAsync(string email)
        {
            using var db = new FunewsManagementContext();
            var account = await db.SystemAccounts.FirstOrDefaultAsync(c => c.AccountEmail == email);
            if (account == null)
            {
                throw new Exception($"SystemAccount with Email {email} not found.");
            }
            return account;
        }

        public static async Task<List<SystemAccount>> GetAllSystemAccountsAsync()
        {
            using var db = new FunewsManagementContext();
            return await db.SystemAccounts.ToListAsync();
        }

        public static async Task UpdateAccountAsync(SystemAccount updatedAccount)
        {
            using var db = new FunewsManagementContext();
            var existingAccount = await db.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == updatedAccount.AccountId);
            if (existingAccount == null)
            {
                throw new Exception($"SystemAccount with ID {updatedAccount.AccountId} not found.");
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

            // Kiểm tra xem ID đã tồn tại chưa
            var existingAccount = await db.SystemAccounts.AnyAsync(a => a.AccountId == account.AccountId);
            if (existingAccount)
            {
                throw new Exception($"SystemAccount with ID {account.AccountId} already exists.");
            }

            object value = await db.SystemAccounts.AddAsync(account);
            await db.SaveChangesAsync();
        }


        public static async Task DeleteSystemAccountAsync(short accountId)
        {
            using var db = new FunewsManagementContext();
            var account = await db.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
            {
                throw new Exception($"SystemAccount with ID {accountId} not found.");
            }

            db.SystemAccounts.Remove(account);
            await db.SaveChangesAsync();
        }

        public static async Task<SystemAccount> GetAccountProfileAsync(short accountId)
        {
            using var db = new FunewsManagementContext();
            return await db.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
        }
    }
}
