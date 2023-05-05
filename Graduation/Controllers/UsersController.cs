using AutoMapper;
using Graduation.Data;
using Graduation.DTOs;
using Graduation.Entities;
using Graduation.Extensions;
using Graduation.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Graduation.Controllers
{
    [Authorize]
    public class UsersController:BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        public UsersController(IPhotoService photoService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _photoService = photoService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _unitOfWork.UserRepository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]

        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _unitOfWork.UserRepository.GetMemberAsync(username);
        }

        [HttpPut("update-location")]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpGet("get-location/{username}")]
        public async Task<ActionResult> GetUserLocation(string username)
        {
            var user =await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            
            if (user == null) return NotFound();


            return Ok(user.Location);
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _unitOfWork.Complete())
            {
                return CreatedAtAction(nameof(GetUser),
                    new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("this is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting photo");
        }


    }
}
