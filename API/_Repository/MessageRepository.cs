using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API._Repository
{
    public class MessageRepository : IMessageRespository
    {
        private DataContext context { get; set; }
        private IMapper mapper;
        public MessageRepository(DataContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper ;
        }

      

        public void AddMessage(Message message)
        {
           context.Messages.Add(message);
        }

        public void DelteMessage(Message message)
        {
            context.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages.OrderByDescending(x=>x.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username),
                "Outbox" =>query.Where(u=>u.SenderUsername == messageParams.Username),
                _=>query.Where(u=>u.RecipientUsername== messageParams.Username && u.DateRead==null),
            };
            var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Where(
                m => m.RecipientUsername == currentUsername
                && m.SenderUsername == recipientUsername
                || m.RecipientUsername == recipientUsername && m.SenderUsername == currentUsername
                ).OrderByDescending(m => m.MessageSent)
                .ToListAsync();

            var undreadMessages = messages.Where(m=>m.DateRead==null && m.RecipientUsername==currentUsername).ToList();
           if( undreadMessages.Any() )
            {
                foreach (var msg in undreadMessages)
                {
                    msg.DateRead = DateTime.UtcNow;
                }
                await context.SaveChangesAsync();
            }
           return mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
           return await context.SaveChangesAsync() > 0;
        }
    }
}
