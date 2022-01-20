﻿using System;
using System.Collections.Generic;
using System.Data;
using MCBABackend.Managers.Interfaces;
using MCBABackend.Models;
using MCBABackend.Utilities.Extensions;
using Microsoft.Data.SqlClient;

namespace MCBABackend.Managers;
public class LoginManager : ILoginManager
{
    private readonly string _connectionString;

    public LoginManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Insert(Login login)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "insert into Login (LoginID, CustomerID, PasswordHash) values (@loginID, @customerID, @passwordHash)";
        command.Parameters.AddWithValue("loginID", login.LoginID);
        command.Parameters.AddWithValue("customerID", login.CustomerID);
        command.Parameters.AddWithValue("passwordHash", login.PasswordHash);

        command.ExecuteNonQuery();

    }

    public Login? RetrieveLogin(string loginId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = connection.CreateCommand();
        command.CommandText = "select * from Login where LoginID = @loginID";
        command.Parameters.AddWithValue("loginID", loginId);

        // Only a single
        Login[] logins = command.GetDataTable().Select().Select(dataRow => new Login()
        {
            CustomerID = dataRow.Field<string>("CustomerID"),
            // LoginID and PasswordHash will never be null on a returned Login
            LoginID = dataRow.Field<string>("LoginID"),
            PasswordHash = dataRow.Field<string>("PasswordHash")
        }).ToArray();
        return logins.Length == 0 ? null : logins[0];
    }
}

