namespace server.Features.Recipes.Photos;

public interface IPhotoService
{
    Task<PhotoUploadResult?> UploadImgFromUrl(string imageUrl);
   
    //Task<string> DeletePhoto(string publicId);
}
