﻿using AutoMapper.QueryableExtensions;
using AutoMapper;
using Graduation.Entities;
using Graduation.Interfaces;
using Microsoft.EntityFrameworkCore;
using Graduation.Helpers;
using Graduation.DTOs;
using Microsoft.Extensions.Options;
using Graduation.Services;
using CloudinaryDotNet.Actions;

namespace Graduation.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;

        public MessageRepository(DataContext context, IMapper mapper, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _mapper = mapper;
            _context = context;
            _cloudinarySettings = cloudinarySettings;
            
        }

        //*****************************************************
        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var fileService = new FileService(_cloudinarySettings);

            var extension = GetFileExtension(file);
            if (extension.Result.ToString().ToLower() == ".jpg"
                || extension.Result.ToString().ToLower() == ".png"
                 || extension.Result.ToString().ToLower() == ".gif")
            {
                var photoService = new PhotoService(_cloudinarySettings);
                ImageUploadResult uploadResult0 = await photoService.AddPhotoAsync(file);
                return uploadResult0?.SecureUrl?.AbsoluteUri;
            }
            else if(extension.Result.ToString().ToLower() == ".ogg")
            {
                VideoUploadResult uploadResult1 = await fileService.AddOggFileAsync(file);
                return uploadResult1?.SecureUrl?.AbsoluteUri;
            }

            RawUploadResult uploadResult2 = await fileService.AddFileAsync(file);
            return uploadResult2?.SecureUrl?.AbsoluteUri;

        }
        //----------------------------------------------------
        //public async Task<string> SaveFileAsync(IFormFile file)
        //{
        //    var fileService = new FileService(_cloudinarySettings);
        //    var uploadResult = await fileService.AddOggFileAsync(file);
        //    return uploadResult?.SecureUrl?.AbsoluteUri;
        //}


        //******************************************************






        public async Task<IEnumerable<GalleryDto>> GetPhotoGallery(string currentUserName, string recipientUserName)
        {
            var messages = await _context.Messages
               .Include(u => u.Sender).ThenInclude(p => p.Photos)
               .Include(u => u.Recipient).ThenInclude(p => p.Photos)
               .Where(
                   m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
                   m.SenderUsername == recipientUserName ||
                   m.RecipientUsername == recipientUserName && m.SenderDeleted == false &&
                   m.SenderUsername == currentUserName
               )
               .OrderBy(m => m.MessageSent)
               .ToListAsync();
            var unreadMessages = messages.Where(m => m.DateRead == null
                && m.RecipientUsername == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<GalleryDto>>(messages);
            

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(
                    m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
                    m.SenderUsername == recipientUserName ||
                    m.RecipientUsername == recipientUserName && m.SenderDeleted == false &&
                    m.SenderUsername == currentUserName
                )
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null
                && m.RecipientUsername == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username
                    && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username
                    && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username
                    && u.RecipientDeleted == false && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>
                .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

     

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<string> GetFileExtension(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            return extension;
        }

       
    }
}