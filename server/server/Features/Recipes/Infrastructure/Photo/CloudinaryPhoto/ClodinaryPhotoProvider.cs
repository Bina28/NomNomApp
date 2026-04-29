using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Server.Features.Recipes.Infrastructure.Photo.CloudinaryPhoto;

public class ClodinaryPhotoProvider : IPhotoProvider
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<ClodinaryPhotoProvider> _logger;

    public ClodinaryPhotoProvider(IOptions<CloudinarySettings> config, ILogger<ClodinaryPhotoProvider> logger)
    {
        _logger = logger;
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
            );

        _cloudinary = new Cloudinary(account);

    }
    public async Task<PhotoUploadResult?> UploadImgFromUrl(string imageUrl, CancellationToken ct = default)
    {

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(imageUrl),
            Folder = "NomNom2025"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams, ct);

        if (uploadResult.Error != null)
        {
            _logger.LogError("Failed to upload image to Cloudinary: {Error}", uploadResult.Error.Message);
            throw new PhotoUploadException($"Failed to upload image: {uploadResult.Error.Message}");
        }

        return new PhotoUploadResult(
            uploadResult.PublicId,
            uploadResult.SecureUrl.AbsoluteUri
        );

    }


}




