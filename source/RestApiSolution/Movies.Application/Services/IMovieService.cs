using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    public interface IMovieService
    {
        Task<bool> CreateAsync(Movie movie, Guid? userid = default, CancellationToken token = default);
        Task<bool> DeleteByIdAsync(Guid id, Guid? userid = default, CancellationToken token = default);
        Task<Movie?> GetByIdAsync(Guid id, Guid? userid = default, CancellationToken token = default);
        Task<Movie?> GetBySlugAsync(string slug, Guid? userid = default, CancellationToken token = default);
        Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default);
        Task<Movie?> UpdateAsync(Movie movie, Guid? userid = default, CancellationToken token = default);
        Task<int> GetCountAsync(string? title, int? yearofrelease, CancellationToken token);
    }
}
