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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public MessagesController(IMapper mapper, IPhotoService photoService, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _photoService = photoService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage([FromForm] CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");
            

            var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound("there is not user name with that name");

            var message = new Message();


            string fileUrl = null;


            if (createMessageDto.File != null)
            {
                fileUrl = await _unitOfWork.MessageRepository.SaveFileAsync(createMessageDto.File);

                if (fileUrl == null)
                {
                    return BadRequest("Failed to upload the file");
                }
               
                var extension = _unitOfWork.MessageRepository.GetFileExtension(createMessageDto.File);

                if (extension.ToString().ToLower() == ".jpg"
                    || extension.ToString().ToLower() == ".png"
                     || extension.ToString().ToLower() == ".gif")
                {
                    message.PhotoUrl = fileUrl;
                }
                else
                {
                    message.FileUrl = fileUrl;
                }

            }
            message.Sender = sender;
            message.Recipient = recipient;
            message.SenderUsername = sender.UserName;
            message.RecipientUsername = recipient.UserName;
            message.Content = createMessageDto.Content;


            _unitOfWork.MessageRepository.AddMessage(message);


            if (message.FileUrl == null && message.PhotoUrl != null)
            {
                var v = _mapper.Map<MessageDto>(message);
                v.FileUrl = message.PhotoUrl;
                if (await _unitOfWork.Complete())
                    return Ok(v);

            }

            if (await _unitOfWork.Complete())
            {
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet("gallery/{username}")]
        public async Task<ActionResult<IEnumerable<string>>> GetGallery(string username)
        {
            var currentUsername = User.GetUsername();

            var reciever = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username.ToLower());
            if (reciever == null)
                return NotFound("there is not user name with that name");

            var result= await _unitOfWork.MessageRepository.GetPhotoGallery(currentUsername, username.ToLower());

            List<string> y = new List<string>();
            foreach (var item in result)
            {
                if (item.PhotoUrl is not null)
                {
                    y.Add(item.PhotoUrl);
                }
            }

            return Ok(y);

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<ThreadDto>>> GetMessageThread(string username)
        {

            var currentUsername = User.GetUsername();
            var reciver=await _unitOfWork.UserRepository.GetUserByUsernameAsync(username.ToLower());
            if (reciver == null)
                return NotFound("there is not user name with that name");
            return Ok(await _unitOfWork.MessageRepository.GetMessageThread("khaled", username.ToLower()));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if (message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();

            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _unitOfWork.MessageRepository.DeleteMessage(message);
            }

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the message");

        }

    }
}