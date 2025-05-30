using BusinessObjects;
using DataAccessObjects.DAO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        public async Task<IEnumerable<SystemAccount>> GetAllAccountsAsync() =>
            await SystemAccountDAO.GetAllSystemAccountsAsync();

        public async Task<SystemAccount> GetAccountByIdAsync(short accountId) =>
            await SystemAccountDAO.GetSystemAccountByIdAsync(accountId);

        public async Task<SystemAccount> GetAccountByEmailAsync(string email) =>
            await SystemAccountDAO.GetSystemAccountByEmailAsync(email);

        public async Task UpdateAccountAsync(SystemAccount account) =>
            await SystemAccountDAO.UpdateAccountAsync(account);

        public async Task CreateAccountAsync(SystemAccount account) =>
            await SystemAccountDAO.CreateSystemAccountAsync(account);

        public async Task DeleteAccountAsync(short accountId) =>
            await SystemAccountDAO.DeleteSystemAccountAsync(accountId);

        public async Task<SystemAccount> GetAccountProfileAsync(short accountId) =>
            await SystemAccountDAO.GetSystemAccountByIdAsync(accountId);
    }
}
