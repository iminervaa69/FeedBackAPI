using System;
using System.Collections.Generic;

namespace FeedBack.Data.Models;

public partial class ProductCategory
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int SuperCategoryId { get; set; }

    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual SuperCategory SuperCategory { get; set; } = null!;
}
