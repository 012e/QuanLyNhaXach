using BookstoreManagement.Shared.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.ImportUI.Services;

public class ImportService
{
    private readonly ApplicationDbContext db;

    public ImportService(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task ApplyImport(int importId)
    {
        var import = await db
            .Imports
            .Include(i => i.ImportItems)
            .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(i => i.Id == importId)
            ?? throw new ArgumentNullException(nameof(importId));

        import.Applied = true;
        foreach (var importItem in import.ImportItems)
        {
            var item = importItem.Item; 

            var currentQuantity = item.Quantity;
            var newQuantity = currentQuantity + importItem.Quantity;

            item.Quantity = newQuantity;
        }

        db.Update(import);

        await db.SaveChangesAsync();
    }
}
