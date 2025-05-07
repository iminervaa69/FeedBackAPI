using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FeedBack.Data.Models;

public partial class FeedBackContext : DbContext
{
    public FeedBackContext()
    {
    }

    public FeedBackContext(DbContextOptions<FeedBackContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> categories { get; set; }

    public virtual DbSet<Customer> customers { get; set; }

    public virtual DbSet<Product> products { get; set; }

    public virtual DbSet<ProductCategory> productcategories { get; set; }

    public virtual DbSet<ProductReview> productreviews { get; set; }

    public virtual DbSet<RefreshToken> refreshtokens { get; set; }

    public virtual DbSet<SuperCategory> supercategories { get; set; }

    public virtual DbSet<Transaction> transactions { get; set; }

    public virtual DbSet<TransactionDetail> transactiondetails { get; set; }

    public virtual DbSet<User> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3213E83FAB0E3984");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3213E83F60FEACB3");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3213E83F23B62016");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateProductUpdatedAt"));

            entity.HasIndex(e => e.ProductCode, "UQ__Products__AE1A8CC441B1DE71").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(100)
                .HasColumnName("product_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3213E83F7AF1397D");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SuperCategoryId).HasColumnName("super_category_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCategories_Categories");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCategories_Products");

            entity.HasOne(d => d.SuperCategory).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.SuperCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCategories_SuperCategories");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductR__3213E83F42E04229");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewText).HasColumnName("review_text");
            entity.Property(e => e.ReviewedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("reviewed_at");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReviews_Products");

            entity.HasOne(d => d.Transaction).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK_ProductReviews_Transaction");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07F2718A95");

            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.RevokedAt).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__UserI__160F4887");
        });

        modelBuilder.Entity<SuperCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SuperCat__3213E83FE0EEF927");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3213E83F74DAA115");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_SoftDeleteProductReviews");
                    tb.HasTrigger("trg_SoftDeleteTransaction");
                    tb.HasTrigger("trg_SoftDeleteTransactionDetails");
                    tb.HasTrigger("trg_UpdateTransactionsUpdatedAt");
                });

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.PurchasedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("purchased_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Customer).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Customers");
        });

        modelBuilder.Entity<TransactionDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3213E83F6C77F6A4");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateTransactionDetailsUpdatedAt"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([quantity]*[unit_price])", true)
                .HasColumnType("decimal(29, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unit_price");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Product).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionDetails_Products");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionDetails_Transactions");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83FC835A2E4");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164021BA736").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572745BC962").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
