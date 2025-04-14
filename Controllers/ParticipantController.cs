using System.Security.Claims;
using ApplicationDatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestCrafter.Models;

namespace QuestCrafter.Controllers
{   
    [Authorize]
    [ApiController]
    [Route("api/Quests")]
    public class ParticipantController : Controller
    {
        private readonly AppDbCtx _ctx;
        private readonly IWebHostEnvironment _env;
        public ParticipantController (AppDbCtx ctx, IWebHostEnvironment env) {_ctx = ctx; _env = env; Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, "CompletedMissionPictures"));}


        [Route("{questID}/join")]
        [HttpPost]
        public async Task<IActionResult> JoinQuest(int questid)
        {
            var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("NameIdentifier claim is missing"));
            var quest = await _ctx.QuestsTable.FindAsync(questid);
            if (quest == null) {return NotFound("the Quest doesn't exists!");}
            if (_ctx.participantsTable.Any(p => p.UserId == UserId && p.QuestId == quest.QuestId)) {return BadRequest("Yout Already Joined This Quest!");}

            var Participant = new Participant
            {
                UserId = UserId,
                QuestId = quest.QuestId,
                IsCompleted = false
            };

            _ctx.participantsTable.Add(Participant);
            await _ctx.SaveChangesAsync();
            return Ok(new {msg = "You are now a member of this quest", Participant_ID = Participant.ParticipantId});
        }



        [HttpPost("{participantID}/completed")]
        public async Task<IActionResult> CompletedQuest(int participantid, IFormFileCollection files)
        {
            var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("NameIdentifier claim is missing"));
            var Participant = await _ctx.participantsTable.Include(p => p.Quest).FirstAsync(p => p.ParticipantId == participantid);
            // var Participant = await _ctx.participantsTable.FindAsync(participantid);

            if (Participant == null) {return BadRequest("Participant not found");}
            if (Participant.UserId != UserId) {return Forbid("Not your participation");}
            if (Participant.IsCompleted) {return BadRequest($"Already completed Quest({Participant.QuestId}): {Participant.Quest.Title}");}

            Participant.IsCompleted = true;
            Participant.CompletionDate = DateTime.UtcNow;
            Participant.CompletionPictureUrl = new List<string>();

            var allowedTypes = new[] { "image/jpeg", "image/png"};
            const long MaxSize = 2 * 1024 * 1024;

            if (files != null)
            {
                foreach (var f in files.Take(5))
                {
                    if (f.Length <= MaxSize && allowedTypes.Contains(f.ContentType))
                    {
                        var filepath = Path.Combine(_env.ContentRootPath, "CompletedMissionPictures", $"{Participant.User.UserName}{UserId}{DateTime.UtcNow::yMMdd_hhmmss}.{f.FileName}");
                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            await f.CopyToAsync(stream);
                        }
                        Participant.CompletionPictureUrl.Add(filepath);
                    }
                }
            }
            await _ctx.SaveChangesAsync();
            return Ok(new { Msg = "Quest completed", ParticipantId = participantid, Participant.CompletionPictureUrl });            
        }

    }
}