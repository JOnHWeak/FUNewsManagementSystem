using BusinessObjects;
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

        public async Task<SystemAccount> GetAccountByIdAsync(short accountID) =>
            await _systemAccountRepository.GetAccountByIdAsync(accountID);

        public async Task<SystemAccount> GetAccountByEmailAsync(string email) =>
            await _systemAccountRepository.GetAccountByEmailAsync(email);

        public async Task<IEnumerable<SystemAccount>> GetAllAccountsAsync() =>
            await _systemAccountRepository.GetAllAccountsAsync();

        public async Task UpdateAccountAsync(SystemAccount account) =>
            await _systemAccountRepository.UpdateAccountAsync(account);

        public async Task CreateAccountAsync(SystemAccount account) =>
            await _systemAccountRepository.CreateAccountAsync(account);

        public async Task DeleteAccountAsync(short accountId) =>
            await _systemAccountRepository.DeleteAccountAsync(accountId);

        public async Task<SystemAccount> GetAccountProfileAsync(short accountId) =>
            await _systemAccountRepository.GetAccountProfileAsync(accountId);
    }
}
