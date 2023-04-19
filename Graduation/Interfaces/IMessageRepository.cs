using Graduation.DTOs;
using Graduation.Entities;
using Graduation.Helpers;

namespace Graduation.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        string GetFileExtension(IFormFile file);
        Task<string> SaveFileAsync(IFormFile file);
        Task<IEnumerable<GalleryDto>> GetPhotoGallery(string currentUserName, string recipientUserName);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<ThreadDto>> GetMessageThread(string currentUserName, string recipientUserName);
        //Task<bool> SaveAllAsync();
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);

    }
}
