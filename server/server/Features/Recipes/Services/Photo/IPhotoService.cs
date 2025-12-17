namespace server.Features.Recipes.Services.Photo;

public interface IPhotoService
{
    Task<PhotoUploadResult?> UploadImgFromUrl(string imageUrl);
   
    //Task<string> DeletePhoto(string publicId);
}
