using API.DTOs;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository userRespository;
        private readonly ILikesRepository likesRepository;

        public LikesController(IUserRepository userRespository, ILikesRepository likesRepository)
        {
            this.userRespository = userRespository;
            this.likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId =int.Parse(User.GetUserId());
            var likedUser=await userRespository.GetUserByNameAsync(username);
            var sourceUser = await likesRepository.GetUserWithLike(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("you can not like your self");
            var userLike = await likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest(" you already liked this user");

            userLike = new Entities.UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if (await userRespository.SaveAllAsync()) return Ok();

            return BadRequest("failed to like a user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
        {
            var users = await likesRepository.GetUserLikes(predicate, int.Parse(User.GetUserId()));
            return Ok(users);
        }

    }
}
