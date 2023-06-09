﻿using Graduation.DTOs;
using Graduation.Entities;

namespace Graduation.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        //Task<IEnumerable<AppUser>> GetMembersAsync();
        Task<MemberDto> GetMemberAsync(string username);
    }
}
