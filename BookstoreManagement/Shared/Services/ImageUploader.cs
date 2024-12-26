using Supabase;
using Supabase.Storage.Interfaces;

namespace BookstoreManagement.Shared.Services;


public class ImageUploader
{
    private readonly Client supabaseClient;
    private readonly IStorageFileApi<Supabase.Storage.FileObject> bucket;

    public ImageUploader(Supabase.Client supabaseClient)
    {
        this.supabaseClient = supabaseClient;
        this.bucket = supabaseClient.Storage.From("images");
    }

    public async Task<string> ReplaceImageAsync(string oldImage, string newImagePath)
    {
        await bucket.Remove(oldImage);

        var newImageName = System.Guid.NewGuid().ToString();
        await bucket.Upload(newImagePath, newImageName);
        return newImageName;
    }

    public string GetPublicUrl(string maybeUrl)
    {
        if (maybeUrl == null)
        {
            throw new ArgumentNullException(nameof(maybeUrl));
        }
        if (maybeUrl.StartsWith("http"))
        {
            return maybeUrl;
        }
        return bucket.GetPublicUrl(maybeUrl);
    }
}