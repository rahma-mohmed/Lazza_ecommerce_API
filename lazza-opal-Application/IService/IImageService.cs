namespace Lazza.opal.Application.Service
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile Img, string folderPath);
        Task DeleteFileAsync(string File);
    }
}
