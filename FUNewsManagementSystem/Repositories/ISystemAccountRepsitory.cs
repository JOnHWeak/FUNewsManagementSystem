using BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISystemAccountRepository
    {
        Task<IEnumerable<SystemAccount>> GetAllAccountsAsync();
        Task<SystemAccount> GetAccountByIdAsync(short accountId);
        Task<SystemAccount> GetAccountByEmailAsync(string email);
        Task UpdateAccountAsync(SystemAccount account);
        Task CreateAccountAsync(SystemAccount account);
        Task DeleteAccountAsync(short id);
        Task<SystemAccount> GetAccountProfileAsync(short accountId);

    }
}
