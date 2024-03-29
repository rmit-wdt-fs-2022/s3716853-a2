﻿using MCBACommon.Contexts;
using MCBACommon.Models;
using Microsoft.EntityFrameworkCore;

namespace MCBACommon.Repositories;

public class CustomerRepository : DataRepository<Customer, string>
{
    public CustomerRepository(McbaContext context) : base(context)
    {
    }

    public override async Task<List<Customer>> GetAll()
    {
        return await _context.Customer.
            Include(customer => customer.Login).
            Include(customer => customer.Accounts).ToListAsync();
    }

    public override async Task<Customer?> Get(string id)
    {
        return await _context.Customer.
            Include(customer => customer.Login).
            Include(customer => customer.Accounts).
            FirstOrDefaultAsync(customer => customer.CustomerID == id);
    }

    public override async Task Add(Customer entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public string Add(Customer customer, Login login)
    {
        // Ensuring the newly added user is the one recieving this login
        if (customer.CustomerID == login.CustomerID)
        {
            _context.Customer.Add(customer);
            _context.Login.Add(login);
            _context.SaveChanges();
        }

        return customer.CustomerID;
    }

    public override async Task Update(Customer customer)
    {
        _context.Customer.Update(customer);
        await _context.SaveChangesAsync();
    }

    public override async Task Delete(string id)
    {
        var entity = await Get(id);
        if (entity != null)
        {
            _context.Customer.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}