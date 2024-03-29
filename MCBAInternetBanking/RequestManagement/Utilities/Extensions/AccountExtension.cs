﻿using System.Text;
using MCBACommon.Models;
using MCBACommon.Models.Dto;

namespace MCBACommon.Utilities.Extensions;
public static class AccountExtension
{
    // Cannot override ToString with extension methods
    // Chose to use an Extension method instead of implementing an overrided ToString
    // in the model classes as to keep them purely as data classes
    public static string AsString(this Account account)
    {
        string transactionStrings = "";
        account.Transactions.ForEach(transaction =>
        {
            transactionStrings += transaction.AsString();
        });
        return new StringBuilder().AppendArray(new string[]
        {
            $"  AccountNumber={account.AccountNumber}",
            $"  AccountType={account.AccountType}",
            $"  CustomerID={account.CustomerID}",
            "   -Transactions-",
            transactionStrings
        }).ToString();
    }

    public static bool HasFreeTransactions(this Account account)
    {
        int usedCount = 0;

        foreach (Transaction accountTransaction in account.Transactions)
        {
            switch (accountTransaction.TransactionType)
            {
                case TransactionType.Withdraw:
                    ++usedCount;
                    break;
                case TransactionType.Transfer:
                    if (accountTransaction.DestinationAccountNumber != null) ++usedCount;
                    break;
                case TransactionType.Deposit:
                case TransactionType.Service:
                case TransactionType.BillPay:
                default:
                    break;
            }
        }
        return usedCount <= Constants.FreeTransactionAmount;
    }

    public static Account ToAccount(this AccountDto dto)
    {
        List<Transaction> transactions = new List<Transaction>();
        dto.Transactions.ForEach(transactionDto =>
        {
            transactions.Add(transactionDto.ToTransaction(dto.AccountNumber));
        });
        return new Account()
        {
            AccountNumber = dto.AccountNumber,
            AccountType = dto.AccountType.ToAccountType(),
            CustomerID = dto.CustomerID,
            Transactions = transactions
        };
    }

    public static decimal Balance(this Account account)
    {
        decimal balance = 0;

        account.Transactions.ForEach(transaction =>
        {
            switch (transaction.TransactionType)
            {
                case TransactionType.BillPay:
                case TransactionType.Service:
                case TransactionType.Withdraw:
                    balance -= transaction.Amount;
                    break;
                case TransactionType.Deposit:
                    balance += transaction.Amount;
                    break;
                case TransactionType.Transfer:
                    if (transaction.DestinationAccountNumber == null || transaction.DestinationAccountNumber == "") // No destinationAccount = they ARE the destination account
                    {
                        balance += transaction.Amount;
                    }
                    else
                    {
                        balance -= transaction.Amount;
                    }
                    break;
                default:
                    // If any new types are added this warning should break the code if this code is never updated
                    throw new ArgumentException(
                        "A transaction has a new TransactionType not handled by Account.Balance()");
            }
        });

        return balance;
    }
}
