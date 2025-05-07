using System;
using System.Collections.Generic;

namespace FeedBack.Data.Models;

public partial class TransactionDetail
{
    public int Id { get; set; }

    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;
}
