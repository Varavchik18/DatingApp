using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
           _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
            .Include(u => u.Sender)
            .Include(u => u.Recepient)
            .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch  
            {
                "Inbox" => query.Where(
                    u => u.Recepient.UserName == messageParams.UserName && u.RecepientDeleted == false),
                "OutBox" => query.Where(
                    u=> u.Sender.UserName == messageParams.UserName && u.SenderDeleted == false),
                _ => query.Where(
                    u => u.Recepient.UserName == messageParams.UserName && u.RecepientDeleted == false && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.
                        CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, 
                                                                string recepientUserName)
        {
            var messages = await _context.Messages
                .Include(u=> u.Sender).ThenInclude(p => p.Photos)
                .Include(u=> u.Recepient).ThenInclude(p => p.Photos)
                .Where( m=> m.Recepient.UserName == currentUserName && m.RecepientDeleted == false
                    && m.Sender.UserName == recepientUserName
                    || m.Recepient.UserName == recepientUserName
                    && m.Sender.UserName == currentUserName && m.SenderDeleted == false
                )
                .OrderBy(m =>m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null 
                            && m.Recepient.UserName == currentUserName).ToList();

            if(unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}