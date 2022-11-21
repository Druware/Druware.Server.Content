using System;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using AutoMapper;
using Druware.Server.Entities;
using Druware.Server.Models;
using Druware.Server.Results;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Druware.Server.Content;

using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Druware.Server.Content.Controllers
{
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly ApplicationSettings _settings;

        private readonly EntityContext _context;

        public TagController(
            IConfiguration configuration,
            IMapper mapper,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            Content.EntityContext context)
        {
            _configuration = configuration;
            _settings = new ApplicationSettings(_configuration);
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        // get - get a list of all the article, with count and offset built in
        [HttpGet("")]
        public IActionResult GetList([FromQuery] int page = 0, [FromQuery] int count = 1000)
        {
            if (_context.Tags == null) return Ok(Result.Ok("No Data Available")); // think I want to alter this to not need the Ok()

            var total = _context.Tags?.Count() ?? 0;
            var list = _context.Tags?
                .OrderBy(a => a.Name)
                .Skip(page * count)
                .Take(count)
                .ToList();
            if (list == null)
            {
                return Ok(ListResult.Error("No List Returned"));
            }
            ListResult result = ListResult.Ok(total, 0, 1000, list: list);
            return Ok(result);
        }

        [HttpGet("{value}")]
        public IActionResult GetArticle(string value)
        {
            Tag? tag = Tag.ByNameOrId(_context, value);
            return (tag != null) ? Ok(tag) : BadRequest("Not Found");
        }

        // post - trigger a password reset
        [HttpPost("")]
        public async Task<ActionResult<Tag>> Add(
            [FromBody] Tag model)
        {
            /*
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            return (_signInManager.IsSignedIn(currentUser)) ?
                Ok(Result.Ok("")) : Ok(Result.Error(""));
            */

            if (!ModelState.IsValid)
                return Ok(Results.Result.Error("Invalid Model Recieved"));

            if (_context.Tags == null)
                return Ok(Result.Ok("No Data Available")); // think I want to alter this to not need the Ok()

            _context.Tags.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpDelete("{value}")]
        public IActionResult DeleteObject(string value)
        {
            Tag? tag = Tag.ByNameOrId(_context, value);
            if (tag == null) return BadRequest("Not Found");

            // now that we have an entity, we will need to check if it is
            // referenced from other entities, like artictles, but we are not
            // there yet.
            // ... Dru 2022.11.02

            _context.Tags.Remove(tag);
            _context.SaveChanges();

            // Should rework the save to return a success of fail on the delete
            return Ok(Result.Ok("Delete Successful"));


        }
    }
}

