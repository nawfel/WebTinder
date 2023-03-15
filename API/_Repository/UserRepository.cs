using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
               .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = context.Users
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .AsNoTracking();
            return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber,userParams.PageSize);
                
        }

        public async Task<AppUser> GetUserById(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByNameAsync(string name)
        {
            return await context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == name);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.Users.
                Include(p => p.Photos).
                ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            context.Entry(user).State = EntityState.Modified;
        }
    }
}
