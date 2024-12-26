using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.IO;

namespace BookstoreManagement.Shared.Services;


public class ImageUploader
{
    private static readonly string BUCKET_NAME = "pornpornporn";
    private static readonly string S3_REGION = "ap-southeast-1"; // Replace with your region
    private static readonly string S3_ENDPOINT = "https://s3.amazonaws.com"; // Default S3 endpoint

    private readonly TransferUtility transferUtility;
    private readonly AmazonS3Client amazonS3Client;

    public ImageUploader(TransferUtility transferUtility, AmazonS3Client amazonS3Client)
    {
        this.transferUtility = transferUtility;
        this.amazonS3Client = amazonS3Client;
    }

    // This method replaces an existing image by first deleting the old image (based on its S3 key) and then uploading the new image.
    public async Task<string> ReplaceImageAsync(string existingImageLink, string newImagePath)
    {
        // Extract the key from the existing image link (URL)
        string existingImageKey = ExtractKeyFromLink(existingImageLink);

        // Delete the existing image
        await DeleteImage(existingImageKey);

        // Upload the new image and return the full URL
        return await UploadImage(newImagePath);
    }

    // Helper function to extract the S3 object key from the link (URL)
    private string ExtractKeyFromLink(string link)
    {
        // Assuming the link is in the format: https://s3.amazonaws.com/{BUCKET_NAME}/{object-key}
        Uri uri = new Uri(link);
        string key = uri.AbsolutePath.TrimStart('/'); // Extract the key by removing the leading slash
        return key;
    }

    // Upload a new image to S3 and return the new image URL
    private async Task<string> UploadImage(string newImagePath)
    {
        // Ensure the file exists
        if (!File.Exists(newImagePath))
        {
            throw new FileNotFoundException("The image file could not be found.", newImagePath);
        }

        var newImageName = Guid.NewGuid().ToString();  // Generate a unique name for the new image

        // Open the file as a FileStream
        using var fileStream = new FileStream(newImagePath, FileMode.Open, FileAccess.Read);

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = fileStream,  // New image file stream to upload
            BucketName = BUCKET_NAME,
            Key = newImageName,
        };

        // Perform the upload
        await transferUtility.UploadAsync(uploadRequest);

        // Construct the full S3 URL for the uploaded image
        string imageUrl = $"{S3_ENDPOINT}/{BUCKET_NAME}/{newImageName}";
        return imageUrl;
    }

    // Delete an existing image from S3 using the given path (key)
    private async Task DeleteImage(string path)
    {
        try
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = BUCKET_NAME,
                Key = path
            };

            await transferUtility.S3Client.DeleteObjectAsync(deleteObjectRequest);
        }
        catch (AmazonS3Exception)
        {

        }
        catch (Exception)
        {

        }
    }
}