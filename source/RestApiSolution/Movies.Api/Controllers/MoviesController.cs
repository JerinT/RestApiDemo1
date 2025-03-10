using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    
    [ApiController]
    [ApiVersion(1.0)]
    //[ApiVersion(1.0), Deprecated = true]

    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IOutputCacheStore _outputCacheStore;


        public MoviesController(IMovieService movieService, IOutputCacheStore outputCacheStore)
        {
            _movieService = movieService;
            _outputCacheStore = outputCacheStore;
        }

        //[Authorize(AuthConstants.TrustedMemberPolicyName)]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [HttpPost(ApiEndpoints.Movies.Create)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request,
            CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movie = request.MapToMovie();

            var result = await _movieService.CreateAsync(movie, userId, token);
            await _outputCacheStore.EvictByTagAsync("movies", token);

            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
            //return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}",movie);

            //we should only return the movieresponse
        }

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        [OutputCache(PolicyName = "MovieCache")]
        //[ResponseCache(Duration = 30, VaryByQueryKeys = new[] {"title", "yearofrelease", "sortBy", "page", "pageSize"},VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]

        public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var options = request.MapToOptions().WithUser(userId);
            var movies = await _movieService.GetAllAsync(options, token);
            var movieCount = await _movieService.GetCountAsync(options.Title, options.Year, token);
            var moviesResponse = movies.MapToResponse(options.Page,options.PageSize,movieCount);
            return Ok(moviesResponse);
        }

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.Movies.Get)]
        //[OutputCache(PolicyName = "MovieCache")]
        [ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding",Location = ResponseCacheLocation.Any)]

        public async Task<IActionResult> Get([FromRoute]string idOrSlug,
            CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movie = Guid.TryParse(idOrSlug, out var id) ? await _movieService.GetByIdAsync(id, userId, token)
                : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie.MapToResponse());
        }
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
            CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movie = request.MapToMovie(id);
            
            var result = await _movieService.UpdateAsync(movie, userId, token);
            if (result is null)
            {
                return NotFound();
            }
            var response = result.MapToResponse();
            await _outputCacheStore.EvictByTagAsync("movies", token);
            return Ok(response);
        }
        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id,
            CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var result = await _movieService.DeleteByIdAsync(id, userId, token);
            if (!result)
            {
                return NotFound();
            }
            await _outputCacheStore.EvictByTagAsync("movies", token);
            return Ok();
        }
    }
}
