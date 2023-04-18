using AutoMapper;
using Graduation.DTOs;
using Graduation.Entities;
using Graduation.Extensions;
using Graduation.Helpers;
using Graduation.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Graduation.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository,
            IMapper mapper, IPhotoService photoService)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _photoService = photoService;
        }


        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage([FromForm] CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");


            var sender = await _userRepository.GetUserByUsernameAsync("khaled");

            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message();


            string fileUrl = null;


            if (createMessageDto.File != null)
            {
                fileUrl = await _messageRepository.SaveFileAsync(createMessageDto.File);

                if (fileUrl == null)
                {
                    return BadRequest("Failed to upload the file");
                }
                //-------------------------------------------------------------تمام  

                var extension = GetFileExtension(createMessageDto.File);
                if (extension.Result.ToString().ToLower() == ".jpg"
                    || extension.Result.ToString().ToLower() == ".png"
                     || extension.Result.ToString().ToLower() == ".gif")
                {
                    message.PhotoUrl = fileUrl;
                }
                else
                    message.FileUrl = fileUrl;

            }
            message.Sender = sender;
            message.Recipient = recipient;
            message.SenderUsername = sender.UserName;
            message.RecipientUsername = recipient.UserName;
            message.Content = createMessageDto.Content;


            _messageRepository.AddMessage(message);

            
            if (message.FileUrl==null && message.PhotoUrl!=null)
            {
                var v=_mapper.Map<MessageDto>(message);
                v.FileUrl = message.PhotoUrl;
                if(await _messageRepository.SaveAllAsync())
                    return Ok(v);

            }

            if (await _messageRepository.SaveAllAsync())
            {
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }

        private async Task<string> GetFileExtension(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            return extension;
        }


        //[HttpPost]
        //public async Task<ActionResult<MessageDto>> CreateMessage([FromForm]CreateMessageDto createMessageDto)
        //{
        //    var username = User.GetUsername();

        //    if (username == createMessageDto.RecipientUsername.ToLower())
        //        return BadRequest("You cannot send messages to yourself");

        //    var sender = await _userRepository.GetUserByUsernameAsync("khaled");

        //    var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        //    if (recipient == null) return NotFound();

        //    var message = new Message();

        //    string fileUrl = null;


        //    if (createMessageDto.File != null)
        //    {
        //        fileUrl = await _messageRepository.SaveFileAsync(createMessageDto.File);
        //        if (fileUrl == null)
        //        {
        //            return BadRequest("Failed to upload the file");
        //        }

        //        message.FileUrl = fileUrl;                      // Set the uploaded file's URL;

        //    } 
        //     message.Sender = sender;
        //     message.Recipient = recipient;
        //     message.SenderUsername = sender.UserName;
        //     message.RecipientUsername = recipient.UserName;
        //     message.Content = createMessageDto.Content;


        //    _messageRepository.AddMessage(message);

        //    if (await _messageRepository.SaveAllAsync())
        //        return Ok(_mapper.Map<MessageDto>(message));

        //    return BadRequest("Failed to send message");
        //}

        //[HttpPost]
        //public async Task<ActionResult<MessageDto>> CreateMessage([FromForm] CreateMessageDto createMessageDto)
        //{


        //    var username = User.GetUsername();

        //    if (username == createMessageDto.RecipientUsername.ToLower())
        //        return BadRequest("You cannot send messages to yourself");

        //    var sender = await _userRepository.GetUserByUsernameAsync(username);
        //    var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        //    if (recipient == null) return NotFound();



        //    var message = new Message
        //    {
        //        Sender = sender,
        //        Recipient = recipient,
        //        SenderUsername = sender.UserName,
        //        RecipientUsername = recipient.UserName,
        //        Content = createMessageDto.Content,
        //    };

        //    _messageRepository.AddMessage(message);

        //    if (await _messageRepository.SaveAllAsync()) 
        //        return Ok(_mapper.Map<MessageDto>(message));


        //    return BadRequest("Failed to send message");
        //}

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {

            var currentUsername = User.GetUsername();
            //****************************************************
            var x=await _userRepository.GetUserByUsernameAsync(username);
            if (x == null)
                return BadRequest("this username is not correct");
            //*****************************************************
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        //[HttpGet("thread/{username}")]
        //public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        //{

        //    //var currentUsername = User.GetUsername();

        //    return Ok(await _messageRepository.GetMessageThread("khaled", username));
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _messageRepository.GetMessage(id);

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");

        }


       

    }
}