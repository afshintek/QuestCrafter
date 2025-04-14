using System.Security.Claims;
using ApplicationDatabaseContext;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Create([FromForm]QuestDTO quest, IFormFileCollection files /*= Request.Form.Files*/) //in the form, if key name doesn't match IFormFile name, key valye won't be accessable via IFormFile(can find it in request.form.file) and also IFormFile will be empty.
        {
            var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("NameIdentifier claim is missing"));
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

        // [HttpGet]
        // public async Task<IActionResult> All()
        // {
        //     var quests = await _ctx.QuestsTable
        //         .Include(q => q.Participants)
        //             .ThenInclude(p => p.User)
        //         .Include(q => q.LeaderBoards)
        //         .Select(q => new
        //         {
        //             QuestId = q.QuestId,
        //             Title = q.Title,
        //             Description = q.Description,
        //             Deadline = q.Deadline,
        //             CreatedAt = q.CreatedAt,
        //             PictureUrls = q.PictureUrl,
        //             CreatorId = q.CreatorId,
        //             CreatorUsername = q.Creator.UserName,
        //             Participants = q.Participants.Select(p => new
        //             {
        //                 ParticipantId = p.ParticipantId,
        //                 UserId = p.UserId,
        //                 Username = p.User.UserName,
        //                 IsCompleted = p.IsCompleted,
        //                 CompletionDate = p.CompletionDate,
        //                 CompletionPictureUrls = p.CompletionPictureUrl,
        //                 AverageScore = _ctx.LeaderTable
        //                     .Where(l => l.QuestId == q.QuestId && l.UserId == p.UserId)
        //                     .Average(l => (double?)l.Score) ?? 0,
        //                 RatingsCount = _ctx.LeaderTable
        //                     .Count(l => l.QuestId == q.QuestId && l.UserId == p.UserId)
        //             }).ToList()
        //         })
        //         .ToListAsync();

        //     return Ok(new { Msg = "All quests retrieved", Quests = quests });
        // }

        [HttpGet]
        public async Task<IActionResult> GetQuests(int? userId, string? status = "all")
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("NameIdentifier claim is missing"));

            var query = _ctx.QuestsTable
                .AsQueryable();

            // Filter by userId (creator or participant)
            if (userId.HasValue)
            {
                query = query.Where(q => q.CreatorId == userId.Value || 
                    q.Participants.Any(p => p.UserId == userId.Value));
            }

            // Filter by status
            if (status?.ToLower() == "active")
                query = query.Where(q => q.Deadline > DateTime.UtcNow);
            else if (status?.ToLower() == "completed")
                query = query.Where(q => q.Deadline <= DateTime.UtcNow);

            // Fetch with projection to avoid loops
            var quests = await query
                .Select(q => new
                {
                    QuestId = q.QuestId,
                    Title = q.Title,
                    Description = q.Description,
                    Deadline = q.Deadline,
                    CreatedAt = q.CreatedAt,
                    PictureUrls = q.PictureUrl,
                    CreatorId = q.CreatorId,
                    CreatorUsername = q.Creator.UserName,
                    Participants = q.Participants.Select(p => new
                    {
                        ParticipantId = p.ParticipantId,
                        UserId = p.UserId,
                        Username = p.User.UserName,
                        IsCompleted = p.IsCompleted,
                        CompletionDate = p.CompletionDate,
                        CompletionPictureUrls = p.CompletionPictureUrl,
                        AverageScore = q.LeaderBoards
                            .Where(l => l.UserId == p.UserId)
                            .Average(l => (double?)l.Score) ?? 0,
                        RatingsCount = q.LeaderBoards
                            .Count(l => l.UserId == p.UserId)
                    }).ToList()
                })
                .ToListAsync();

            return Ok(new { Msg = "Quests retrieved", Quests = quests });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuest(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("NameIdentifier claim is missing"));

            var quest = await _ctx.QuestsTable
                .Where(q => q.QuestId == id)
                .Select(q => new
                {
                    QuestId = q.QuestId,
                    Title = q.Title,
                    Description = q.Description,
                    Deadline = q.Deadline,
                    CreatedAt = q.CreatedAt,
                    PictureUrls = q.PictureUrl,
                    CreatorId = q.CreatorId,
                    CreatorUsername = q.Creator.UserName,
                    Participants = q.Participants.Select(p => new
                    {
                        ParticipantId = p.ParticipantId,
                        UserId = p.UserId,
                        Username = p.User.UserName,
                        IsCompleted = p.IsCompleted,
                        CompletionDate = p.CompletionDate,
                        CompletionPictureUrls = p.CompletionPictureUrl,
                        AverageScore = q.LeaderBoards
                            .Where(l => l.UserId == p.UserId)
                            .Average(l => (double?)l.Score) ?? 0,
                        RatingsCount = q.LeaderBoards
                            .Count(l => l.UserId == p.UserId)
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (quest == null)
                return NotFound("Quest not found");

            // Optional: Ensure user is in quest
            // if (!quest.Participants.Any(p => p.UserId == currentUserId) && quest.CreatorId != currentUserId)
            //     // return Forbid("You must be in the quest or its creator");
            //     return StatusCode(403, "You must be in the quest or its creator");

            return Ok(new { Msg = "Quest retrieved", Quest = quest });
        }
    }
}