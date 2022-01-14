﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCBABackend.Managers.Interfaces;
using MCBABackend.Models;
using MCBABackend.Utilities;
using MCBABackend.Utilities.Extensions;
using Microsoft.Data.SqlClient;

namespace MCBABackend.Managers;
public static class DatabaseManager
{
    // Public setter to allow dependency injection
    // Private getter because only want services to use DatabaseManager not the rest of the Manager classes
    public static ICustomerManager _customerManager { private get; set; }
    public static ILoginManager _loginManager { private get; set; }
    public static IAccountManager _accountManager { private get; set; }
    public static ITransactionManager _transactionManager { private get; set; }

    public static void InitFromWebApiResponse(List<Customer>? customers)
    {
        customers.ForEach(customer =>
        {
            _customerManager.Insert(customer);
            _loginManager.Insert(customer.Login);
            customer.Accounts.ForEach(account =>
            {
                _accountManager.Insert(account);
                account.Transactions.ForEach(transaction =>
                {
                    _transactionManager.Insert(transaction);
                });
            });
        });
    }

    public static List<Customer> RetrieveCustomers()
    {
        return _customerManager.RetrieveCustomers();
    }

    public static Login? RetrieveLogin(string loginId)
    {
        return _loginManager.RetrieveLogin(loginId);
    }

    public static List<Account> RetrieveUserAccounts(int customerId)
    {
        return _accountManager.RetrieveUserAccounts(customerId);
    }

    public static void Update(Account account)
    {
        _accountManager.Update(account);
    }
    public static void Deposit(Account account, decimal amount, string? comment)
    {
        account.Balance += amount;
        _accountManager.Update(account);
        _transactionManager.Insert(new Transaction()
        {
            TransactionType = (char) TransactionType.Deposit,
            AccountNumber = account.AccountNumber,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.Now.ToUniversalTime()
        });
    }
}
