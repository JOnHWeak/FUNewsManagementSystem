using BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISystemAccountRepository
    {
        Task<IEnumerable<SystemAccount>> GetAllAccountsAsync();
        Task<SystemAccount?> GetAccountByIdAsync(short accountId); // Changed to int
        Task<SystemAccount?> GetAccountByEmailAsync(string email);
        Task<IEnumerable<SystemAccount>> GetAccountsByRoleAsync(int role); // New method
        Task UpdateAccountAsync(SystemAccount account);
        Task CreateAccountAsync(SystemAccount account);
        Task DeleteAccountAsync(short accountId); // Changed to int
        Task<SystemAccount?> GetAccountProfileAsync(short accountId); // Changed to int
        Task<bool> HasCreatedNewsArticlesAsync(short accountId); // ⭐ NEW

    }
}
