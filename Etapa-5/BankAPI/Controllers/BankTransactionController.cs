using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BankTransactionController : ControllerBase
{
    private readonly BankTransactionService _service;

    public BankTransactionController(BankTransactionService service)
    {
        _service = service;
    }

    [HttpGet("getall/{id}")]
    public async Task<IEnumerable<AccountDTOOut >> GetAllById(int id)
    {
        return await _service.GetAllById(id);
    }

    /* Para los retiros por transferencia se necesiatara contar con el correo
    y contraseña del propietario de la cuenta. Se valida si el cliente que realiza
    el retiro existe, si los datos del dueño y su cuenta sean validos.*/
    [HttpPut("retiro/transferencia/{id}")]
    public async Task<IActionResult> RetiroTransferencia(int id, TransferenciaDTO transaction)
    {

        var client = await _service.ClientExists(id);

        if(client is null)
            return BadRequest(new {message = $"El usuario con ID = {id} no es un cliente registrado."});

        var ownerInfo = new ClientDTO{
            Email = transaction.ClientEmail,
            Pwd = transaction.ClientPwd
        };

        var owner = await _service.Owner(ownerInfo);

        if(owner is null)
            return BadRequest(new {message = "Credenciales invalidas."});

        var accountToUpdate = await _service.GetByIds(transaction.AccountId, owner.Id);

        if(accountToUpdate is not null)
        {
            var newBalance = accountToUpdate.Balance - transaction.Balance;
            if(newBalance<0)
                return BadRequest( new {message = "No se puede retirar mas dinero del que la cuenta tiene."});
            
            await _service.Update(transaction.AccountId, newBalance);
            return NoContent();
        }
        else
        {
            return NotFound(new {message = $"El usuario con ID ={owner.Id} no tiene una cuenta con ID = {transaction.AccountId}."});
        }
        
    }

    [HttpPut("retiro/efectivo/{id}")]
    public async Task<IActionResult> RetiroEfectivo(int id, EfectivoDTO transaction)
    {
        var accountToUpdate = await _service.GetByIds(transaction.AccountId, id);
        if(accountToUpdate is not null)
        {
            var newBalance = accountToUpdate.Balance - transaction.Balance;
            if(newBalance<0)
                return BadRequest( new {message = "No se puede retirar mas dinero del que la cuenta tiene."});
            
            await _service.Update(transaction.AccountId, newBalance);
            return NoContent();
        }
        else
        {
            return NotFound(new {message = $"El usuario con ID ={id} no tiene una cuenta con ID = {transaction.AccountId}."});
        }
    }

    [HttpPut("deposito/{id}")]
    public async Task<IActionResult> Deposito(int id, EfectivoDTO transaction)
    {
        var accountToUpdate = await _service.GetByIds(transaction.AccountId, id);
        if(accountToUpdate is not null)
        {
            if(transaction.Balance<0)
                return BadRequest( new {message = "Esta funcion no puede restar dinero a la cuenta."});
            var newBalance = accountToUpdate.Balance + transaction.Balance;
            Console.WriteLine(newBalance);
            await _service.Update(transaction.AccountId, newBalance);
            return NoContent();
        }
        else
        {
            return NotFound(new {message = $"El usuario con ID ={id} no tiene una cuenta con ID = {transaction.AccountId}."});
        }
    }

    [HttpDelete("eliminarCuenta/{userId}/{accountId}")]
    public async Task<IActionResult> EliminarCuenta(int userId, int accountId){

        var accountToDelete = await _service.GetByIds(accountId, userId);
        if(accountToDelete is not null)
        {
            if(accountToDelete.Balance==0)
            {
                await _service.Delete(accountId);
                return Ok();
            }
            else{
                return BadRequest( new {message = "Solo es posible eliminar una cuenta con Balance = 0"});
            }
        }
        else
        {
            return NotFound(new {message = $"El usuario con ID ={userId} no tiene una cuenta con ID = {accountId}."});
        }
    }

}