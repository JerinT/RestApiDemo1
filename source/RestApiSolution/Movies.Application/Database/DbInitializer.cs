using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Database
{
    public class DbInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public DbInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task InitializeAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            object value = await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS movies
                (
                    id UUID PRIMARY KEY,
                    slug TEXT NOT NULL,
                    title TEXT NOT NULL,
                    yearofrelease INT NOT NULL
                );
            ");

            await connection.ExecuteAsync(@"
                create unique index if not exists idx_movies_slug
                on movies using btree(slug);
            ");

            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS genres
                (
                    movieid UUID references movies(id),
                    name TEXT NOT NULL                    
                );
            ");

            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS ratings
                (
                    userid UUID,
                    movieid UUID references movies(id),
                    rating INT NOT NULL,
                    primary key(userid,movieid)
                );                
            ");
        }
    }
}
