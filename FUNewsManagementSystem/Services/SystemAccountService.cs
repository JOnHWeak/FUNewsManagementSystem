using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly ISystemAccountRepository _systemAccountRepository;

        public SystemAccountService(ISystemAccountRepository systemAccountRepository)
        {
            _systemAccountRepository = systemAccountRepository;
        }

        public async Task<SystemAccount?> GetAccountByIdAsync(short accountID) =>
            await _systemAccountRepository.GetAccountByIdAsync(accountID);

        public async Task<SystemAccount?> GetAccountByEmailAsync(string email) =>
            await _systemAccountRepository.GetAccountByEmailAsync(email);

        public async Task<IEnumerable<SystemAccount>> GetAllAccountsAsync() =>
            await _systemAccountRepository.GetAllAccountsAsync();

        public async Task<IEnumerable<SystemAccount>> GetAccountsByRoleAsync(int role) =>
            await _systemAccountRepository.GetAccountsByRoleAsync(role);

        public async Task UpdateAccountAsync(SystemAccount account) =>
            await _systemAccountRepository.UpdateAccountAsync(account);

        public async Task CreateAccountAsync(SystemAccount account)
        {
            // Business logic validation
            if (account.AccountRole < 1 || account.AccountRole > 2)
                throw new ArgumentException("Role must be 1 (Staff) or 2 (Lecturer)");

            if (string.IsNullOrEmpty(account.AccountEmail))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrEmpty(account.AccountPassword) || account.AccountPassword.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            await _systemAccountRepository.CreateAccountAsync(account);
        }

        public async Task DeleteAccountAsync(short accountId)
        {
            // Business rule: Cannot delete account that has created news articles
            var hasArticles = await _systemAccountRepository.HasCreatedNewsArticlesAsync(accountId);
            if (hasArticles)
            {
                throw new InvalidOperationException("Cannot delete account that has created news articles");
            }

            await _systemAccountRepository.DeleteAccountAsync(accountId);
        }

        public async Task<SystemAccount?> GetAccountProfileAsync(short accountId) =>
            await _systemAccountRepository.GetAccountProfileAsync(accountId);

        public async Task<bool> HasCreatedNewsArticlesAsync(short accountId) =>
            await _systemAccountRepository.HasCreatedNewsArticlesAsync(accountId);

        // ⭐ NEW: Additional business methods
        public async Task<bool> CanDeleteAccountAsync(short accountId)
        {
            var account = await GetAccountByIdAsync(accountId);
            if (account == null) return false;

            var hasArticles = await HasCreatedNewsArticlesAsync(accountId);
            return !hasArticles;
        }

        public async Task<int> GetNewsArticlesCountAsync(int accountId)
        {
            using var db = new FunewsManagementContext();
            return await db.NewsArticles.CountAsync(na => na.CreatedById == accountId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                var account = await GetAccountByEmailAsync(email);
                return account != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ValidateAccountCredentialsAsync(string email, string password)
        {
            try
            {
                var account = await GetAccountByEmailAsync(email);
                return account != null && account.AccountPassword == password;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
