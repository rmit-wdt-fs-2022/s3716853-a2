﻿using System.Text;
using MCBACommon.Models;
using MCBACommon.Models.Dto;

namespace MCBACommon.Utilities.Extensions;
public static class TransactionExtension
{
    // Cannot override ToString with extension methods
    // Chose to use an Extension method instead of implementing an overrided ToString
    // in the model classes as to keep them purely as data classes
    public static string AsString(this Transaction transaction)
    {
        return new StringBuilder().AppendArray(new string[]
        {
            $"      TransactionId={transaction.TransactionID}",
            $"      TransactionType={transaction.TransactionType}",
            $"      OriginAccountNumber={transaction.OriginAccountNumber}",
            $"      DestinationAccountNumber={transaction.DestinationAccountNumber}",
            $"      Amount={transaction.Amount}",
            $"      Comment={transaction.Comment}",
            $"      TransactionTimeUtc={transaction.TransactionTimeUtc.ToLocalTime()}",
        }).ToString();
    }

    public static Transaction ToTransaction(this TransactionDto dto, string accountNumber)
    {
        return new Transaction()
        {
            TransactionID = default, // The model has the id as non-nullable, however the database will ignore this when adding as it handles creating transactionIds
            TransactionType = dto.TransactionType,
            OriginAccountNumber = accountNumber,
            Amount = dto.Amount,
            Comment = dto.Comment,
            TransactionTimeUtc = dto.TransactionTimeUtc
        };
    }
}
