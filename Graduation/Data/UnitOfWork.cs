using AutoMapper;
using Graduation.Helpers;
using Graduation.Interfaces;
using Microsoft.Extensions.Options;

namespace Graduation.Data
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        public UnitOfWork(DataContext context, IMapper mapper, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _context = context;
            _mapper = mapper;
            _cloudinarySettings = cloudinarySettings;
        }
        public IUserRepository UserRepository => new UserRepository(_context, _mapper);
        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper,_cloudinarySettings);


        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
