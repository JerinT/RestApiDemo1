using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly List<Movie> _movies = new();
        public Task<bool> CreateAsync(Movie movie, Guid? userid = default, CancellationToken token = default)
        {
            _movies.Add(movie);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteByIdAsync(Guid id, Guid? userid = default, CancellationToken token = default)
        {
            var removedIndex = _movies.RemoveAll(x => x.Id == id);
            var movieWasRemoved = removedIndex > 0;
            return Task.FromResult(movieWasRemoved);
        }

        public Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            return Task.FromResult(_movies.AsEnumerable());
        }

        public Task<Movie?> GetByIdAsync(Guid id, Guid? userid = default, CancellationToken token = default)
        {
            var movie = _movies.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(movie);
        }

        public Task<Movie?> GetBySlugAsync(string slug, Guid? userid = default, CancellationToken token = default)
        {
            var movie = _movies.FirstOrDefault(x => x.Slug == slug);
            return Task.FromResult(movie);
        }

        public Task<int> GetCountAsync(string? title, int? yearofrelease, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Movie movie, Guid? userid = default, CancellationToken token = default)
        {
            var movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
            if (movieIndex == -1)
            {
                return Task.FromResult(false);
            }
            _movies[movieIndex] = movie;
            return Task.FromResult(true);

        }
    }
}
