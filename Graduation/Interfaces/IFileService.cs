using CloudinaryDotNet.Actions;

namespace Graduation.Interfaces
{
    public interface IFileService
    {
        Task<VideoUploadResult> AddOggFileAsync(IFormFile file);
        Task<RawUploadResult> AddFileAsync(IFormFile file);
    }
}
