using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Server.Features.Recipes.Infrastructure.Photo.CloudinaryPhoto;

public class ClodinaryPhotoProvider : IPhotoProvider
{
    private readonly Cloudinary _cloudinary;

    public ClodinaryPhotoProvider(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
            );

        _cloudinary = new Cloudinary(account);

    }
    public async Task<PhotoUploadResult?> UploadImgFromUrl(string imageUrl)
    {

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(imageUrl),
            Folder = "NomNom2025"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new Exception(uploadResult.Error.Message);
        }

        return new PhotoUploadResult(
            uploadResult.PublicId,
            uploadResult.SecureUrl.AbsoluteUri
        );

    }


}




