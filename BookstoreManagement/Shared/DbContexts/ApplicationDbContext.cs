using BookstoreManagement.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.Shared.DbContexts;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Import> Imports { get; set; }

    public virtual DbSet<ImportItem> ImportItems { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoicesItem> InvoicesItems { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemPrice> ItemPrices { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.HasIndex(e => e.Email, "idx_customers_email");

            entity.HasIndex(e => e.PhoneNumber, "idx_customers_phone_number");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.HasIndex(e => e.Email, "idx_employees_email");

            entity.HasIndex(e => e.IsManager, "idx_employees_is_manager");

            entity.HasIndex(e => e.PhoneNumber, "idx_employees_phone_number");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsManager)
                .HasDefaultValue(false)
                .HasColumnName("is_manager");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.ProfilePicture).HasColumnName("profile_picture");
            entity.Property(e => e.Salary)
                .HasColumnType("money")
                .HasColumnName("salary");
        });

        modelBuilder.Entity<Import>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("imports_pkey");

            entity.ToTable("imports");

            entity.HasIndex(e => e.CreatedAt, "idx_imports_created_at");

            entity.HasIndex(e => e.ProviderId, "idx_imports_provider_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.TotalCost)
                .HasColumnType("money")
                .HasColumnName("total_cost");

            entity.HasOne(d => d.Provider).WithMany(p => p.Imports)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("imports_provider_id_fkey");
        });

        modelBuilder.Entity<ImportItem>(entity =>
        {
            entity.HasKey(e => new { e.ImportId, e.ItemId }).HasName("import_items_pkey");

            entity.ToTable("import_items");

            entity.HasIndex(e => e.ImportId, "idx_import_items_import_id");

            entity.HasIndex(e => e.ItemId, "idx_import_items_item_id");

            entity.Property(e => e.ImportId).HasColumnName("import_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Import).WithMany(p => p.ImportItems)
                .HasForeignKey(d => d.ImportId)
                .HasConstraintName("import_items_import_id_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.ImportItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("import_items_item_id_fkey");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invoices_pkey");

            entity.ToTable("invoices");

            entity.HasIndex(e => e.CreatedAt, "idx_invoices_created_at");

            entity.HasIndex(e => e.CustomerId, "idx_invoices_customer_id");

            entity.HasIndex(e => e.EmployeeId, "idx_invoices_employee_id");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Total)
                .HasColumnType("money")
                .HasColumnName("total");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("invoices_customer_id_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("invoices_employee_id_fkey");
        });

        modelBuilder.Entity<InvoicesItem>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.ItemId }).HasName("invoices_items_pkey");

            entity.ToTable("invoices_items");

            entity.HasIndex(e => e.InvoiceId, "idx_invoices_items_invoice_id");

            entity.HasIndex(e => e.ItemId, "idx_invoices_items_item_id");

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoicesItems)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("invoices_items_invoice_id_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.InvoicesItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("invoices_items_item_id_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("items_pkey");

            entity.ToTable("items");

            entity.HasIndex(e => e.BasePrice, "idx_items_base_price");

            entity.HasIndex(e => e.Name, "idx_items_name");

            entity.HasIndex(e => e.ProviderId, "idx_items_provider_id");

            entity.HasIndex(e => e.Quantity, "idx_items_quantity");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrice).HasColumnName("base_price");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Provider).WithMany(p => p.Items)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("items_provider_id_fkey");

            entity.HasMany(d => d.Tags).WithMany(p => p.Items)
                .UsingEntity<Dictionary<string, object>>(
                    "ItemsTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("items_tags_tag_id_fkey"),
                    l => l.HasOne<Item>().WithMany()
                        .HasForeignKey("ItemId")
                        .HasConstraintName("items_tags_item_id_fkey"),
                    j =>
                    {
                        j.HasKey("ItemId", "TagId").HasName("items_tags_pkey");
                        j.ToTable("items_tags");
                        j.HasIndex(new[] { "ItemId" }, "idx_items_tags_item_id");
                        j.HasIndex(new[] { "TagId" }, "idx_items_tags_tag_id");
                        j.IndexerProperty<int>("ItemId").HasColumnName("item_id");
                        j.IndexerProperty<int>("TagId").HasColumnName("tag_id");
                    });
        });

        modelBuilder.Entity<ItemPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_prices_pkey");

            entity.ToTable("item_prices");

            entity.HasIndex(e => e.BeginDate, "idx_item_prices_begin_date");

            entity.HasIndex(e => e.ItemId, "idx_item_prices_item_id");

            entity.HasIndex(e => e.PriceType, "idx_item_prices_price_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BeginDate).HasColumnName("begin_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Ordering).HasColumnName("ordering");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.PriceType).HasColumnName("price_type");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemPrices)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("item_prices_item_id_fkey");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notes_pkey");

            entity.ToTable("notes");

            entity.HasIndex(e => e.EmployeeId, "idx_notes_employee_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Employee).WithMany(p => p.Notes)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("notes_employee_id_fkey");
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("providers_pkey");

            entity.ToTable("providers");

            entity.HasIndex(e => e.Address, "idx_providers_address");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tags_pkey");

            entity.ToTable("tags");

            entity.HasIndex(e => e.Name, "idx_tags_name");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
