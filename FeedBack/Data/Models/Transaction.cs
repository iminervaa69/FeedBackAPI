using System;
using System.Collections.Generic;

namespace FeedBack.Data.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime PurchasedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
}
