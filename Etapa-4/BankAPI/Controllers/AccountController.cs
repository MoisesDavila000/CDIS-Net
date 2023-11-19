using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly AccountTypeService _accountTypeService;
    private readonly ClientService _clientService;

    public AccountController(AccountService accountService, AccountTypeService accountTypeService, ClientService clientService)
    {
        _accountService = accountService;
        _accountTypeService = accountTypeService;
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IEnumerable<AccountDTOOut>> Get()
    {
        return await _accountService.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDTOOut>> GetById(int id)
    {
        var account = await _accountService.GetDTOById(id);

        if(account is null)
            return AccountNotFound(id);
        return account;
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(AccountDTOIn account)
    {
        string validationResult = await ValidateAccount(account);

        if(!validationResult.Equals("Valid"))
            return BadRequest(new {message = validationResult});
        
        var newAccount = await _accountService.Create(account);

        return CreatedAtAction(nameof(GetById), new {id =newAccount.Id}, newAccount);
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AccountDTOIn account)
    {
        if(id != account.Id)
            return BadRequest(new {message = $"El ID({id}) de la URL no councide con el ID({account.Id}) del cuerpo."});
        
        var accountToUpdate = await _accountService.GetById(id);

        if(accountToUpdate is not null)
        {
            string validationResult = await ValidateAccount(account);

            if(!validationResult.Equals("Valid"))
                return BadRequest(new {message = validationResult});
            
            await _accountService.Update(account);
            return NoContent();
        }
        else
        {
            return AccountNotFound(id);
        }
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var accountToDelete = await _accountService.GetById(id);

        if(accountToDelete is not null)
        {
            await _accountService.Delete(id);
            return Ok();
        }
        else
        {
            return AccountNotFound(id);
        }
    }

    public NotFoundObjectResult AccountNotFound(int id)
    {
        return NotFound(new {message = $"La cuenta con ID = {id} no existe."});
    }

    public async Task<string> ValidateAccount(AccountDTOIn account)
    {
        string result = "Valid";

        var accountType = await _accountTypeService.GetById(account.AccountType);

        if(accountType is null)
            result = $"El tipo de cuenta {account.AccountType} no existe.";

        var clientID = account.ClientId.GetValueOrDefault();

        var client = await _clientService.GetById(clientID);

        if(client is null)
            result = $"El cliente {clientID} no existe";
        return result;
    }


}
