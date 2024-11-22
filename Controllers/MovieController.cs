using EntertaimentLib_API.Services;
using EntertaimentLib_API.Models;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_API_UNET.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private FirebaseService _context;

        public MovieController(FirebaseService context)=> _context = context;


        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            return Ok(await _context.GetAllAsy<Movie>("movies"));
        }



        [HttpGet ("{Id}")]
        public async Task<IActionResult> GetMovie(string Id)
        {
            var movie = await _context.GetMovieByIdAsync("movies", Id);

            if(movie == null) 
            {
                NotFound($"Movie with ID {Id} not found.");
            }

            return Ok(movie);
        }



        [HttpPost]
        public async Task<IActionResult> PostMovie([FromBody] Movie newMovie)
        {
            if (newMovie == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                //await _context.PostAsy(newMovie);   //Posting Default Firebase Id
                
                await _context.PostMovieWithCustomIdAsync(newMovie);
                return Ok("Movie details have been added successfully");
            }
            
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateMovie(string Id, [FromBody] Movie updatedMovie)
        {
            if (updatedMovie.Id != null)
                return BadRequest("Not need to put here your id");
                
            try
            {
                string path = $"movies/{Id}"; // Construct the path using the movie ID
                bool result = await _context.PutAsy(path, updatedMovie);

                if (result)
                    return Ok("Details have been updated successfully"); // Successfully updated
                else
                    return NotFound("Unfortunatly details are not found"); // Movie not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteMovie(string Id, Movie movie)
        {
            if (movie.Id != null)
                return BadRequest("Not need to put here your id");

            try
            {
                string path = $"movies/{Id}";
                bool result = await _context.DeleteAsy<Movie>(path);
                if (result)
                    return Ok("Details have been deleted successfully"); // Successfully deleted
                else
                    return NotFound("Unfortunatly details are not found");  // Movie not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
