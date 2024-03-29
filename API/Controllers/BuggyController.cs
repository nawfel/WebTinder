﻿using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class BuggyController : BaseApiController
    {
        private readonly DataContext _dataContext;
        public BuggyController(DataContext context)
        {
            _dataContext = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _dataContext.Users.Find(-1);
            if (thing == null) return NotFound();
            return thing;

        }
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {

            var thing = _dataContext.Users.Find(-1);
            var thingToReturn = thing.ToString();
            return thingToReturn;


           
        }
    
    [HttpGet("bad-request")]
    public ActionResult<string> BadRequest()
    {
        return BadRequest("Bad Request");
    }

}
}
