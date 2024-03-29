﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MCBACommon.Models;
public class Login
{
    [Key]
    [StringLength(8, MinimumLength = 8)]
    [RegularExpression("^\\d+")] //Only allow digit
    public string LoginID { get; set; }

    [ForeignKey(nameof(Customer))]
    public string CustomerID { get; set; }

    [JsonIgnore]
    public Customer? Customer { get; set; }

    [StringLength(64)]
    public string PasswordHash { get; set; }
}
