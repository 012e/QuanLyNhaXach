using BookstoreManagement.Models;
using dotenv.net.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.DbContexts;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoicesItem> InvoicesItems { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(EnvReader.GetStringValue("DATABASE_CONNECTION"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.Property(e => e.EmployeeId).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.IsManager)
                .HasDefaultValue(false)
                .HasColumnName("is_manager");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.ProfilePicture).HasColumnName("profile_picture");
            entity.Property(e => e.Salary)
                .HasColumnType("money")
                .HasColumnName("salary");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("invoices_pkey");

            entity.ToTable("invoices");

            entity.Property(e => e.InvoiceId).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Total)
                .HasColumnType("money")
                .HasColumnName("total");

            entity.HasOne(d => d.Employee).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoices_employee_id_fkey");
        });

        modelBuilder.Entity<InvoicesItem>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.ItemId }).HasName("invoices_items_pkey");

            entity.ToTable("invoices_items");

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoicesItems)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoices_items_invoice_id_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.InvoicesItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoices_items_item_id_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("items_pkey");

            entity.ToTable("items");

            entity.Property(e => e.ItemId).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Provider).WithMany(p => p.Items)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("items_provider_id_fkey");

            entity.HasMany(d => d.TagNames).WithMany(p => p.Items)
                .UsingEntity<Dictionary<string, object>>(
                    "ItemsTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagName")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("items_tags_tag_name_fkey"),
                    l => l.HasOne<Item>().WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("items_tags_item_id_fkey"),
                    j =>
                    {
                        j.HasKey("ItemId", "TagName").HasName("items_tags_pkey");
                        j.ToTable("items_tags");
                        j.IndexerProperty<int>("ItemId").HasColumnName("id");
                        j.IndexerProperty<string>("TagName")
                            .HasMaxLength(255)
                            .HasColumnName("tag_name");
                    });
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("providers_pkey");

            entity.ToTable("providers");

            entity.Property(e => e.ProviderId).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("tags_pkey");

            entity.ToTable("tags");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
