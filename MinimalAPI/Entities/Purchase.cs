// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace MinimalAPI.Api.Entities;

public partial class Purchase
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int? Amount { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int PurchaseType { get; set; }
}