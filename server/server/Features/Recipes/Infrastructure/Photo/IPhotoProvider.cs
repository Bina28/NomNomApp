using Server.Features.Recipes.Infrastructure.Photo.CloudinaryPhoto;

namespace Server.Features.Recipes.Infrastructure.Photo;

public interface IPhotoProvider
{
    Task<PhotoUploadResult?> UploadImgFromUrl(string imageUrl);
   
    //Task<string> DeletePhoto(string publicId);
}
