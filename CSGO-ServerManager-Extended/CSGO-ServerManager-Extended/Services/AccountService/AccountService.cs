using CSGO_ServerManager_Extended.Models.Constants;
using CSGO_ServerManager_Extended.Repositories.AccountRepository;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Services.AccountService;

public interface IAccountService
{
    Task SavePassword(string password);
    Task<bool> IsPasswordCorrect(string password);
}

public class AccountService : IAccountService
{
    private IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task SavePassword(string password)
    {
        try
        {
            await _accountRepository.SavePassword(password);
        }
        catch (Exception e)
        {
            throw new Exception($"Unable to save password: \"{e.Message}\"");
        }
    }

    /// <summary>
    /// Return true if the provided password equals the password stored in secure storage
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> IsPasswordCorrect(string password)
    {
        try
        {
            if (Equals(password, await _accountRepository.GetPassword()))
                return true;
            else 
                return false;
        }
        catch (Exception e)
        {
            throw new Exception($"Unable to validate password: \"{e.Message}\"");
        }
    }
}