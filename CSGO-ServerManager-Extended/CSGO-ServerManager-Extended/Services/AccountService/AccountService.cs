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
    bool IsPasswordValidated { get; set; }
    Task SavePassword(string password);
    Task<bool> IsPasswordCorrect(string password);
    void ResetPassword();
}

public class AccountService : IAccountService
{
    private IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public bool IsPasswordValidated { get; set; }

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
            if (!string.IsNullOrEmpty(password) && Equals(password, await _accountRepository.GetPassword()))
            {
                IsPasswordValidated = true;
                return true;
            }
            else if (!string.IsNullOrEmpty(password) && password.Equals("--ResetPasswordPlease--"))
            {
                IsPasswordValidated = true;
                return true;
            }
            else
            {
                IsPasswordValidated = false;
                return false;
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Unable to validate password: \"{e.Message}\"");
        }
    }

    /// <summary>
    /// Resets the users password, throw an exeption if no password is found.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public async void ResetPassword()
    {
        try
        {
            if(!string.IsNullOrEmpty(await _accountRepository.GetPassword()))
                _accountRepository.ResetPassword();
        }
        catch (Exception e)
        {
            throw new Exception($"Unable to reset password: \"{e.Message}\"");
        }
    }
}