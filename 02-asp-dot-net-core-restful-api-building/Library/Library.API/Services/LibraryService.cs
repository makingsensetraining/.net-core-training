using Library.API.Entities;
using Library.API.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly LibraryContext _context;

        public LibraryService(LibraryContext context)
        {
            _context = context;
        }

        public async Task AddAuthorAsync(Author author)
        {
            await _context.Authors.AddAsync(author);

            await SaveAsync();
        }

        public async Task AddBookForAuthorAsync(Guid authorId, Book book)
        {
            var author = await GetAuthorAsync(authorId);
            if (author != null)
            {
                author.Books.Add(book);
            }
            await SaveAsync();
        }

        public async Task<bool> AuthorExistsAsync(Guid authorId)
        {
            return await _context.Authors.AnyAsync(a => a.Id == authorId);
        }

        public async Task DeleteAuthorAsync(Author author)
        {
            _context.Authors.Remove(author);
            await SaveAsync();
        }

        public async Task DeleteBookAsync(Book book)
        {
            _context.Books.Remove(book);
            await SaveAsync();
        }

        public async Task<Author> GetAuthorAsync(Guid authorId)
        {
            return await _context.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public async Task<PagedList<Author>> GetAuthorsAsync(AuthorResourceParameters authorResourceParameters)
        {
            var authorsBeforePaging = _context.Authors
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .AsQueryable();

            if(authorResourceParameters.Genre != null)
            {
                var genre = authorResourceParameters.Genre.Trim().ToLowerInvariant();

                authorsBeforePaging = authorsBeforePaging
                    .Where(a => a.Genre.ToLowerInvariant() == genre);
            }

            if (authorResourceParameters.SearchQuery != null)
            {
                var searchQuery = authorResourceParameters.SearchQuery.Trim().ToLowerInvariant();

                authorsBeforePaging = authorsBeforePaging
                    .Where(a => a.Genre.ToLowerInvariant().Contains(searchQuery)
                    || a.FirstName.ToLowerInvariant().Contains(searchQuery)
                    || a.LastName.ToLowerInvariant().Contains(searchQuery));
            }            

            return await PagedList<Author>.CreateAsync(authorsBeforePaging, 
                authorResourceParameters.PageNumber, authorResourceParameters.PageSize);
        }


        public async Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds)
        {
            return await _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToListAsync();
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            await SaveAsync();
        }

        public async Task<Book> GetBookForAuthorAsync(Guid authorId, Guid bookId)
        {
            return await _context.Books
              .Where(b => b.AuthorId == authorId && b.Id == bookId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksForAuthorAsync(Guid authorId)
        {
            return await _context.Books
                        .Where(b => b.AuthorId == authorId)
                        .OrderBy(b => b.Title)
                        .ToListAsync();
        }

        public async Task UpdateBookForAuthorAsync(Book book)
        {
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
