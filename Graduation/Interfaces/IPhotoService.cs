using CloudinaryDotNet.Actions;

namespace Graduation.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
        //-----------
        Task<RawUploadResult> AddFileAsync(IFormFile file); // this is for uploading mp3 files......
        Task<VideoUploadResult> AddOggFileAsync(IFormFile file);
    }
}
