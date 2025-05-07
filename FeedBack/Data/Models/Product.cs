using System;
using System.Collections.Generic;

namespace FeedBack.Data.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ProductCode { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
}
