using EntertaimentLib_API.Services;
using EntertaimentLib_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_API_UNET.Controllers
{
    [Route("api/shows")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        private FirebaseService _context;

        public ShowController(FirebaseService context) => _context = context;


        [HttpGet]
        public async Task<IActionResult> GetShows()
        {
            return Ok(await _context.GetAllShowsAsy<Show>("shows"));
        }



        [HttpGet("{Id}")]
        public async Task<IActionResult> GetShow(string Id)
        {
            var show = await _context.GetShowByIdAsync("shows", Id);

            if (show == null)
            {
                NotFound($"Show with ID {Id} not found.");
            }

            return Ok(show);
        }

        [HttpPost]
        public async Task<IActionResult> PostShow([FromBody] Show newShow)
        {
            if (newShow == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                //await _context.PostAsy(newShow);   //Posting Default Firebase Id

                await _context.PostShowWithCustomIdAsync(newShow);
                return Ok("Show details have been added successfully");
            }

        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateShow(string Id, [FromBody] Show updatedShow)
        {
            if (updatedShow.Id != null)
                return BadRequest("Not need to put here your id");

            try
            {
                string path = $"shows/{Id}"; // Construct the path using the Show ID
                bool result = await _context.PutShowAsy(path, updatedShow);

                if (result)
                    return Ok("Details have been updated successfully"); // Successfully updated
                else
                    return NotFound("Unfortunatly details are not found"); // Show not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteShow(string Id)
        {
            try
            {
                string path = $"shows/{Id}";
                bool result = await _context.DeleteShowAsy<Show>(path);
                if (result)
                    return Ok("Details have been deleted successfully"); // Successfully deleted
                else
                    return NotFound("Unfortunatly details are not found");  // Show not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
