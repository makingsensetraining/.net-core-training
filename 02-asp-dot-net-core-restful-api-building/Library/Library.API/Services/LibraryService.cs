using Library.API.Entities;
using Library.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly LibraryContext _context;

        public LibraryService(LibraryContext context)
        {
            _context = context;
        }

        public bool AddAuthor(Author author)
        {
            _context.Authors.Add(author);

            return Save();
        }

        public bool AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthor(authorId);
            if (author != null)
            {
                author.Books.Add(book);
            }
            return Save();
        }

        public bool AuthorExists(Guid authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public bool DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
            return Save();
        }

        public bool DeleteBook(Book book)
        {
            _context.Books.Remove(book);
            return Save();
        }

        public Author GetAuthor(Guid authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public PagedList<Author> GetAuthors(AuthorResourceParameters authorResourceParameters)
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

            return PagedList<Author>.Create(authorsBeforePaging, authorResourceParameters.PageNumber, authorResourceParameters.PageSize);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public bool UpdateAuthor(Author author)
        {
            return Save();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return _context.Books
              .Where(b => b.AuthorId == authorId && b.Id == bookId).FirstOrDefault();
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            return _context.Books
                        .Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();
        }

        public bool UpdateBookForAuthor(Book book)
        {
            return Save();
        }

        private bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
