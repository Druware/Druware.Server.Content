using System;
using AutoMapper;
using Druware.Server.Content;
using Druware.Server.Entities;
using Druware.Server.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Druware.Server.Content.Controllers
{
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly ApplicationSettings _settings;

        private readonly EntityContext _context;

        public NewsController(
            IConfiguration configuration,
            IMapper mapper,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            EntityContext context)
        {
            _configuration = configuration;
            _settings = new ApplicationSettings(_configuration);
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        private Article? ArticleByPermalinkOrId(string value)
        {
            Int32 id;
            Article? article = null;

            if (Int32.TryParse(value, out id))
                article = _context.News?.Single(t => t.ArticleId == id);

            if (article == null)
                article = _context.News?.Single(t => t.Permalink == value);

            return article;
        }

        // get - get a list of all the article, with count and offset built in
        [HttpGet("")]
        public IActionResult GetList()
        {
            // not relevant to THIS method
            /*
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            return (_signInManager.IsSignedIn(currentUser)) ?
                Ok(Result.Ok("")) : Ok(Result.Error(""));
            */
            if (_context.News == null) return Ok(Result.Ok("No Data Available")); // think I want to alter this to not need the Ok()

            var total = _context.News?.Count() ?? 0;
            var list = _context.News?
                .OrderBy(a => a.Posted)
                .Include(s => s.ArticleTags)
                .TagWithSource("Getting articles")
                //                .Skip(page * count)
                //              .Take(count)
                .ToList();
            ListResult result = ListResult.Ok(total, 0, 1000, list: list);
            return Ok(result);
        }

        [HttpGet("{value}")]
        public IActionResult Get(string value)
        {
            Article? article = ArticleByPermalinkOrId(value);
            return (article != null) ? Ok(article) : BadRequest("Not Found");
        }

        // post - trigger a password reset
        [HttpPost("")]
        public async Task<ActionResult<Article>> Add(
            [FromBody] Article model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                       .Where(y => y.Count > 0)
                       .ToList();
                var message = "Invalid Model Recieved";
                foreach (var error in errors)
                    message += String.Format("\n\t{0}", error);
                return Ok(Results.Result.Error(message));
            }

            if (_context.News == null)
                return Ok(Result.Ok("No Data Available")); // think I want to alter this to not need the Ok()

            _context.News.Add(model);
            await _context.SaveChangesAsync();

            // fix the tags on the model if present, to by

            //foreach (string t in model.Tags)
            //{
            //    Tag? tag = _context.Tags?.Single(ta => ta.Name == t);
            //       
//
  //          }

            // convert the Tags array to ArticleTags ( and create any new ones that are missing )

            // tag = _context.Tags?.Single(t => t.Name == value);
            // return (tag != null) ? Ok(tag) : BadRequest("Not Found");


            return Ok(model);
        }

        [HttpPut("{value}")]
        public async Task<ActionResult<Article>> Update(
            string value,
            [FromBody] Article model)
        {
            // find the article
            Article? article = ArticleByPermalinkOrId(value);
            if (article == null) return BadRequest("Not Found");

            // validate the model
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                       .Where(y => y.Count > 0)
                       .ToList();
                var message = "Invalid Model Recieved";
                foreach (var error in errors)
                    message += String.Format("\n\t{0}", error);
                return Ok(Results.Result.Error(message));
            }

            // validate the changes on the internal rules.
            // the Id cannot be changed.
            // the postedDate cannot be changed
            // may need to adjust the collections ( tags/comments )

            // set and write the changes

            article.Title = model.Title;

            // _context.Entry<Article>(article).CurrentValues.SetValues(model);
            await _context.SaveChangesAsync();

            // return the updated object
            return Ok(ArticleByPermalinkOrId(value));
        }

        [HttpDelete("{value}")]
        public IActionResult Remove(string value)
        {
            // TODO: Add Security/Permissions check


            Article? article = ArticleByPermalinkOrId(value);
            if (article == null) return BadRequest("Not Found");

            // now that we have an entity, we will need to check if it is
            // referenced from other entities, like artictles, but we are not
            // there yet.
            // ... Dru 2022.11.02

            _context.News.Remove(article);
            _context.SaveChanges();

            // Should rework the save to return a success of fail on the delete
            return Ok(Result.Ok("Delete Successful"));


        }

    }

}

