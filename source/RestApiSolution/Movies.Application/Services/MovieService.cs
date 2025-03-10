using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IValidator<Movie> _movieValidator;
        private readonly IRatingRepository _ratingRepository;
        private readonly IValidator<GetAllMoviesOptions> _optionsValidator;

        public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator, 
            IRatingRepository ratingRepository, IValidator<GetAllMoviesOptions> optionsValidator)
        {
            _movieRepository = movieRepository;
            _movieValidator = movieValidator;
            _ratingRepository = ratingRepository;
            _optionsValidator = optionsValidator;
        }

        public async Task<bool> CreateAsync(Movie movie, Guid? userid = default, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, token);
           
            return await _movieRepository.CreateAsync(movie, userid, token);
        }

        public Task<bool> DeleteByIdAsync(Guid id, Guid? userid = default, CancellationToken token = default)
        {
           return _movieRepository.DeleteByIdAsync(id, userid, token);
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options,CancellationToken token = default)
        {
            await _optionsValidator.ValidateAndThrowAsync(options, token);
            return await _movieRepository.GetAllAsync(options, token);
        }

        public Task<Movie?> GetByIdAsync(Guid id, Guid? userid = default, CancellationToken token = default)
        {
            return _movieRepository.GetByIdAsync(id, userid, token);
        }

        public Task<Movie?> GetBySlugAsync(string slug, Guid? userid = default, CancellationToken token = default)
        {
            return _movieRepository.GetBySlugAsync(slug, userid, token);
        }

        public async Task<Movie?> UpdateAsync(Movie movie, Guid? userid = default, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie);
            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, token);
            if(!movieExists) return null;
            await _movieRepository.UpdateAsync(movie, userid, token);
            if (!userid.HasValue)
            { 
                var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
                movie.Rating = rating;
                return movie;
            }

            var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userid.Value, token);
            movie.Rating = ratings.Rating;
            movie.UserRating = ratings.UserRating;
            return movie;
        }
        public async Task<int> GetCountAsync(string? title, int? yearofrelease, CancellationToken token)
        {
            return await _movieRepository.GetCountAsync(title, yearofrelease, token);
        }
    }
}
