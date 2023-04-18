using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Graduation.Interfaces;
using Graduation.Helpers;
using Microsoft.Extensions.Options;

namespace Graduation.Services
{
    public class FileService: IFileService
    {
        private readonly Cloudinary _cloudinary;
        public FileService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }
        public async Task<VideoUploadResult> AddOggFileAsync(IFormFile file)
        {
            var uploadResult = new VideoUploadResult();

            if (file.Length > 0)
            {
                // Check if the file has .ogg extension
                if (Path.GetExtension(file.FileName).ToLower() == ".ogg")
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new VideoUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        // ResourceType = "video", // Set resource type to "video" for .ogg files
                        Folder = "da-net7"
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
                else
                {
                    // You can handle non-ogg file extensions here if needed
                    throw new ArgumentException("Invalid file type. Please upload a .ogg file.");
                }
            }

            return uploadResult;
        }

        //******************************************************
        public async Task<RawUploadResult> AddFileAsync(IFormFile file)
        {
            var uploadResult = new RawUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "da-net7"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }
    }
}
