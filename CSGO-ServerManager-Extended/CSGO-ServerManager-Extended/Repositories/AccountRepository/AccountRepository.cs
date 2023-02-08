using CSGO_ServerManager_Extended.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Repositories.AccountRepository;

public interface IAccountRepository
{
    Task<string> GetPassword();
    Task SavePassword(string password);
    bool ResetPassword();
}

public class AccountRepository : IAccountRepository
{
    public async Task SavePassword(string password)
    {
        await SecureStorage.SetAsync(AccountConstants.AccountPasswordKey, password);
    }

    public async Task<string> GetPassword()
    {
        return await SecureStorage.GetAsync(AccountConstants.AccountPasswordKey);
    }

    public bool ResetPassword()
    {
        return SecureStorage.Remove(AccountConstants.AccountPasswordKey);
    }
}