using EntertaimentLib_API.Models;
using EntertaimentLib_API.Services;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Web_API_UNET.Controllers
{
    [Route("api/")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        private FirebaseService _context;

        public FeedBackController(FirebaseService context)
        {
            _context = context;
        }

        [HttpGet("feedbacks")]
        public async Task<List<FeedBack>> GetFeedbacks()
        {
            List<FeedBack> feeds = await _context.Feeds("feedbacks");
            return feeds;
        }

        [HttpGet("feedbacks/{FeedId}/Comment")]
        public async Task<IActionResult> GetComment(string FeedId)
        {
            var comments = await _context.Comments("feedbacks", FeedId);
            return Ok(comments);   
        }

        [HttpGet("feedbacks/{EntId}")]
        public async Task<List<FeedBack>> GetFeedBacks(string EntId)
        {

            var feedbacks = await _context.FeedBacksGetAsy("feedbacks", EntId);

            return feedbacks;
        }


        [HttpPost("feedbacks/{EntId}/{Email}")]
        public async Task<IActionResult> PostFeed([FromBody] FeedBack feed)
        {
            if(feed.Like != true || feed.Comment != null || feed.Rating != 0)
            {
                return Ok(new { Message = await _context.FeedBackPostAsy("feedbacks", feed)});
            }

            return BadRequest("We are not get anything for Post Operation");
        }

        [HttpPatch("feedbacks/{FeedId}/Like")]
        public async Task<IActionResult> PatchLikeFeed(string FeedId, [FromBody] bool feed)
        {
            if(feed == true || feed == false)
            {
                await _context.FeedBackLikePatchAsy($"feedbacks/{FeedId}", feed);

                return Ok(new { Message = "Feedback updated successfully.", FeedId = FeedId, UpdatedLikeStatus = feed });
            }
            return BadRequest("ERROR");
        }
/*
        [HttpPatch("feedbacks/{FeedId}/Comment")]
        public async Task<IActionResult> PatchCmtFeed(string FeedId, [FromBody] Comment feed)
        {
            var comments = await _context.Comments("feedbacks", FeedId);
            var CmtId = comments.Count; 
            return Ok(new { Message = await _context.FeedBackCmtPatchAsy($"feedbacks/{FeedId}/Comment/{CmtId}", feed) });
        }*/

        [HttpPatch("feedbacks/{FeedId}/Comment/{CmtId}")]
        public async Task<IActionResult> EditComment(string FeedId, string CmtId, [FromBody] Comment updFeed)
        {
            return Ok(new { Message = await _context.CmtEdit($"feedbacks/{FeedId}/Comment/{CmtId}", updFeed) });
        }

        [HttpPatch("feedbacks/{FeedId}/Rating")]
        public async Task<IActionResult> PatchRatingFeed(string FeedId, [FromBody] short feed)
        {
            if (feed >= 0)
            {
                return Ok(new {Message = await _context.FeedBackRatingPatchAsy($"feedbacks/{FeedId}", feed) });
                //return Ok(new { Message = "Feedback updated successfully.", FeedId = FeedId, UpdatedLikeStatus = feed });
            }
            return BadRequest("ERROR");
        }

        [HttpDelete("feedbacks/{FeedId}/Comment/{CmtId}")]
        public async Task<IActionResult> DeleteComment(string FeedId, int CmtId)
        {
            if (FeedId !=null && CmtId >= 0)
            {
                var result = await _context.DelCmt($"feedbacks/{FeedId}/Comment", CmtId);
                return Ok(new { Message = result });
            }
            return BadRequest(new {Message = "FeedId or CmtId is missing." });
        }

    }
}
