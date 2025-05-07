using System;
using System.Collections.Generic;

namespace FeedBack.Data.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
