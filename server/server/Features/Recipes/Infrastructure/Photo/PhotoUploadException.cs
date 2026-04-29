namespace Server.Features.Recipes.Infrastructure.Photo;

public class PhotoUploadException : Exception
{
    public PhotoUploadException(string message) : base(message)
    {
    }
}
