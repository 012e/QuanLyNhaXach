using Microsoft.Extensions.Hosting;
using Supabase;

namespace BookstoreManagement.Shared.Services;

public class BucketSetupService : BackgroundService
{
    private readonly Client supabaseClient;

    public BucketSetupService(Supabase.Client supabaseClient)
    {
        this.supabaseClient = supabaseClient;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var buckets = await supabaseClient.Storage.ListBuckets();
        foreach (var bucket in buckets)
        {
            if (bucket.Name == "images")
            {
                await supabaseClient.Storage.UpdateBucket("images", new Supabase.Storage.BucketUpsertOptions { Public = true });
                return;
            }
        }
        await supabaseClient.Storage.CreateBucket("images", new Supabase.Storage.BucketUpsertOptions { Public = true });
    }
}
