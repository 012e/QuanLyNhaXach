using BookstoreManagement.Shared.Models;
using System.IO;
using System;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BookstoreManagement.Shared.DbContexts;
using Microsoft.EntityFrameworkCore;
using BookstoreManagement.PricingUI.Services;
using BookstoreManagement.PricingUI.Dtos;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;

namespace BookstoreManagement.InvoiceUI.Exporters;

public class InvoiceDocument(Invoice invoice, List<PricingResponseDto> pricingSource, byte[] logo) : IDocument
{
    public Invoice Invoice { get; } = invoice;
    public List<PricingResponseDto> Pricings { get; } = pricingSource;
    public byte[] Logo { get; } = logo;

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;


    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item()
                    .Text($"Invoice #{Invoice.Id}")
                    .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                column.Item().Text(text =>
                {
                    text.Span("Issue date: ").SemiBold();
                    text.Span($"{Invoice.CreatedAt:d}");
                });

                column.Item().Text(text =>
                {
                    text.Span("Employee: ").SemiBold();
                    text.Span($"{Invoice.Employee.FirstName} {Invoice.Employee.LastName}");
                });

                column.Item().Text(text =>
                {
                    text.Span("Customer: ").SemiBold();
                    text.Span($"{Invoice.Customer.FirstName} {Invoice.Customer.LastName}");
                });
            });

            row.ConstantItem(100).Height(50).Image(Logo);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(5);

            column.Item().Element(ComposeTable);
            column.Item().AlignRight().Text($"Grand total: {Invoice.Total:C}").FontSize(14).Bold();
        });
    }

    public static async Task<byte[]> ReadImageAsByteArrayAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            throw new ArgumentException("Image path cannot be null or empty", nameof(imagePath));

        if (!File.Exists(imagePath))
            throw new FileNotFoundException("The specified image file does not exist", imagePath);

        try
        {
            return await File.ReadAllBytesAsync(imagePath);
        }
        catch (Exception ex)
        {
            throw new IOException("An error occurred while reading the image file", ex);
        }
    }



    private void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(25);
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("#");
                header.Cell().Element(CellStyle).Text("Product");
                header.Cell().Element(CellStyle).AlignRight().Text("Unit price");
                header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                }
            });

            int i = 1;
            foreach (var item in Invoice.InvoicesItems)
            {
                var pricing = Pricings.First(p => p.Item.Id == item.ItemId) ?? throw new ArgumentNullException(nameof(item.Item));
                table.Cell().Element(CellStyle).Text($"{i}");
                table.Cell().Element(CellStyle).Text(item.Item.Name);
                table.Cell().Element(CellStyle).AlignRight().Text($"{pricing.FinalPrice:C}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{(pricing.FinalPrice * item.Quantity):C}");

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
                i++;
            }
        });
    }
}

public class InvoiceExporter
{
    private readonly ApplicationDbContext db;
    private readonly PricingService pricingService;

    public InvoiceExporter(ApplicationDbContext db, PricingService pricingService)
    {
        this.db = db;
        this.pricingService = pricingService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private static async Task<byte[]> ReadImageAsByteArrayAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            throw new ArgumentException("Image path cannot be null or empty", nameof(imagePath));

        if (!File.Exists(imagePath))
            throw new FileNotFoundException("The specified image file does not exist", imagePath);

        try
        {
            return await File.ReadAllBytesAsync(imagePath);
        }
        catch (Exception ex)
        {
            throw new IOException("An error occurred while reading the image file", ex);
        }
    }


    private string GetProjectDirectory()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        return projectDirectory;

    }

    public async Task ExportPdf(int invoiceId)
    {
        var document = await GenerateDocument(invoiceId);
        document.GeneratePdfAndShow();
    }

    public async Task ExportPdf(int invoiceId, string path)
    {
        InvoiceDocument document = await GenerateDocument(invoiceId);
        document.GeneratePdf(path);
    }

    private async Task<InvoiceDocument> GenerateDocument(int invoiceId)
    {
        var invoice = await db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Employee)
            .Include(i => i.InvoicesItems)
            .ThenInclude(ii => ii.Item)
            .FirstOrDefaultAsync(i => i.Id == invoiceId)
            ?? throw new ArgumentNullException(nameof(invoiceId));
        var pricings = (await pricingService.GetAllPricingAsync()).ToList();
        var logo = await ReadImageAsByteArrayAsync(Path.Join(GetProjectDirectory(), "Shared", "Images", "logo.png"));
        var document = new InvoiceDocument(invoice, pricings, logo);
        return document;
    }
}
