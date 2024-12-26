using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Hosting;

namespace BookstoreManagement.Shared.Services;

public class BucketSetupService : BackgroundService
{
    private readonly AmazonS3Client amazonS3Client;

    public BucketSetupService(AmazonS3Client amazonS3Client)
    {
        this.amazonS3Client = amazonS3Client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!await AmazonS3Util.DoesS3BucketExistV2Async(amazonS3Client, "images"))
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = "images",
                UseClientRegion = true
            };
            PutBucketResponse putBucketResponse = await amazonS3Client.PutBucketAsync(putBucketRequest, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var putBucketRequest = new PutBucketRequest
        {
            BucketName = "pornpornporn",
            BucketRegion = S3Region.APSoutheast1
        };
        PutBucketResponse putBucketResponse = await amazonS3Client.PutBucketAsync(putBucketRequest, stoppingToken);
    }
}
