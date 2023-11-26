using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;

namespace BankAPI.Services;

public class BankTransactionService
{
    private readonly BankContext _context;

    public BankTransactionService(BankContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccountDTOOut>> GetAllById(int id)
    {
        return await _context.Accounts.Where(x => x.ClientId == id).Select(a => new AccountDTOOut{
            Id = a.Id,
            AccountName = a.AccountTypeNavigation.Name,
            ClientName = a.Client != null ? a.Client.Name : "",
            Balance = a.Balance,
            RegDate = a.RegDate
        }).ToListAsync();
    }

    public async Task Update(int id, decimal newBalance)
    {
        var existingAccount = await GetById(id);
        Console.WriteLine(existingAccount);
        if(existingAccount is not null)
        {
            existingAccount.Balance = newBalance;

            await _context.SaveChangesAsync();
        }      

    }

    public async Task Delete(int id)
    {
        var accountToDelete = await GetById(id);

        if(accountToDelete is not null)
        {
            _context.Accounts.Remove(accountToDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Client?> Owner(ClientDTO client)
    {
        return await _context.Clients.SingleOrDefaultAsync(x => x.Email == client.Email && x.Pwd == client.Pwd);
    }

    public async Task<Client?> ClientExists(int id)
    {
        return await _context.Clients.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Account?> GetById(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<AccountDTOOut> GetByIds(int id, int clientId)
    {
        return await _context.Accounts.Where(a => a.Id == id && a.ClientId == clientId).Select(a => new AccountDTOOut {
            Id = a.Id,
            AccountName = a.AccountTypeNavigation.Name,
            ClientName = a.Client != null ? a.Client.Name : "",
            Balance = a.Balance,
            RegDate = a.RegDate
        }).SingleOrDefaultAsync();
    }
}