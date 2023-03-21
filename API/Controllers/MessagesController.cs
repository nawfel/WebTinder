using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMessageRespository messageRepository;
        private readonly IMapper mapper;

        public MessagesController(IUserRepository userRepository, IMessageRespository messageRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessageDto(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();
            if (username == createMessageDto.RecipientUsername.ToLower()) //recipientUsername
                return BadRequest("you can not send a message to your self");

            var sender = await userRepository.GetUserByNameAsync(username);
            var recipient = await userRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);
            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            messageRepository.AddMessage(message);
            if (await messageRepository.SaveAllAsync()) 
                return Ok(mapper.Map<MessageDto>(message));
            return BadRequest("failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username= User.GetUserName();
            var messages = await messageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername= User.GetUserName();
            return Ok(await messageRepository.GetMessageThread(currentUsername, username));
        }

    }

}
