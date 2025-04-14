using System.Security.Claims;
using ApplicationDatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestCrafter.Models;

namespace QuestCrafter.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/Quests/{questID}")]
    public class LeaderBoardController : Controller
    {
        private readonly AppDbCtx _ctx;
        public LeaderBoardController(AppDbCtx ctx) {_ctx = ctx;}

        [HttpPost("rate")]
        public async Task<IActionResult> Rate(int participantId, int Score)
        {
            var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("NameIdentifier claim is missing"));
            var participant = await _ctx.participantsTable.FindAsync(participantId);
            if (participant == null)
                return NotFound("Participant not found");
            if (!participant.IsCompleted)
                return BadRequest("Quest not completed yet");

            //maing sure rater is also a member of the challenge
            if (! _ctx.participantsTable.Any(p => p.Quest.QuestId == participant.QuestId && p.UserId == UserId))
                // return Forbid("not a member of the Quest!"); can't use forbid like this!
                return StatusCode(403, "not a member of the Quest!");
            
            // if (participant.UserId == UserId)
            //     // return Forbid("You can't rate yourself!"); // can't be used like this either retun forbid(); or :
            //     return StatusCode(403, "You can't rate for yourself!");

            if ( Score < 1 || Score > 100)
                return BadRequest("Score must be bewteen 1 to 100");

            if (_ctx.LeaderTable.Any(l => l.QuestId == participant.QuestId && l.UserId == participant.UserId && l.RaterId == UserId))
                return BadRequest("Already rated this participant");

            var rating = new LeaderBoard
            {
                QuestId = participant.QuestId,
                UserId = participant.UserId,
                RaterId = UserId,
                Score = Score
            };
            _ctx.LeaderTable.Add(rating);
            await _ctx.SaveChangesAsync();
            return Ok(new { Msg = "Rating submitted", LeaderBoardId = rating.LeaderBoardId, rating.Quest?.Title });
        }

        
        [HttpGet("ratings")]
        public async Task<IActionResult> GetQuestRating(int questid)
        {
            var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("NameIdentifier claim is missing"));
            if (! await _ctx.QuestsTable.AnyAsync(q => q.QuestId == questid))
                return NotFound("Quest Not Found");

            // if you want only members of a quest be able to see rating:
            // if (! await _ctx.participantsTable.AnyAsync(p => p.QuestId == questid && p.UserId == UserId))
            //     return Forbid();

            var leaderboard = await _ctx.LeaderTable
                .Where(l => l.QuestId == questid)
                .GroupBy(l => l.UserId)
                .Select(g => new // with select no need to include navigation properites
                {
                    UserId = g.Key,
                    Username = g.First().User.UserName,
                    AvarageScore = g.Average(l => l.Score),
                    RatingsCount = g.Count()
                }).OrderByDescending(l => l.AvarageScore)
                .ToListAsync();
            return Ok(new { Msg = "Leaderboard retrieved", leaderboard = leaderboard });

        }
    }
}