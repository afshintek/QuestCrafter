using System.Security.Claims;
using ApplicationDatabaseContext;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestCrafter.Models;

namespace QuestCrafter.Controllers
{   
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class QuestController : Controller
    {
        private readonly AppDbCtx _ctx;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public QuestController(AppDbCtx appDbCtx, IMapper mapper, IWebHostEnvironment environment)
        {
            _ctx = appDbCtx; _mapper = mapper; _env = environment;
             Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, "QuestPictures"));
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateQuest([FromForm]QuestDTO quest, IFormFileCollection files /*= Request.Form.Files*/) //in the form, if key name doesn't match IFormFile name, key valye won't be accessable via IFormFile(can find it in request.form.file) and also IFormFile will be empty.
        {
            var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (!ModelState.IsValid) {return BadRequest(ModelState);}
            
            var q = _mapper.Map<Quest>(quest);
            q.CreatedAt = DateTime.UtcNow;
            q.CreatorId = UserId;
            q.PictureUrl = new List<string>();
            
            var allowedTypes = new[] { "image/jpeg", "image/png"};
            const long MaxSize = 2 * 1024 * 1024;


            if (files != null)
            {
                foreach(var f in files.Take(5))
                {
                    if (f.Length <= MaxSize && allowedTypes.Contains(f.ContentType))
                    {
                        var filepath = Path.Combine(_env.ContentRootPath, "QuestPictures", $"{DateTime.UtcNow:yMMdd_hhmmss}.{f.FileName}");
                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            await f.CopyToAsync(stream);
                        }
                        q.PictureUrl.Add(filepath);
                    }
                }
            }

            _ctx.QuestsTable.Add(q);
            await _ctx.SaveChangesAsync();

            return Ok(new {msg = "created quest", QuestID = q.QuestId, q.PictureUrl});
        }

    }
}