using BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ISystemAccountService
    {
        Task<SystemAccount> GetAccountByIdAsync(short accountID);
        Task<SystemAccount> GetAccountByEmailAsync(string email);
        Task<IEnumerable<SystemAccount>> GetAllAccountsAsync();
        Task UpdateAccountAsync(SystemAccount account);
        Task CreateAccountAsync(SystemAccount account);
        Task DeleteAccountAsync(short id);
        Task<SystemAccount> GetAccountProfileAsync(short accountId);
    }
}
