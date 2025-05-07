using System;
using System.Collections.Generic;

namespace FeedBack.Data.Models;

public partial class ProductReview
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? TransactionId { get; set; }

    public int Rating { get; set; }

    public string? ReviewText { get; set; }

    public DateTime ReviewedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Transaction? Transaction { get; set; }
}
