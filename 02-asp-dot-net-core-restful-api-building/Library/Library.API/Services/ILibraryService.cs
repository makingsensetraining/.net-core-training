using Library.API.Entities;
using Library.API.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public interface ILibraryService
    {
        Task<PagedList<Author>> GetAuthorsAsync(AuthorResourceParameters authorResourceParameters);
        Task<Author> GetAuthorAsync(Guid authorId);
        Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds);
        Task<bool> AddAuthorAsync(Author author);
        Task<bool> DeleteAuthorAsync(Author author);
        Task<bool> UpdateAuthorAsync(Author author);
        Task<bool> AuthorExistsAsync(Guid authorId);
        Task<IEnumerable<Book>> GetBooksForAuthorAsync(Guid authorId);
        Task<Book> GetBookForAuthorAsync(Guid authorId, Guid bookId);
        Task<bool> AddBookForAuthorAsync(Guid authorId, Book book);
        Task<bool> UpdateBookForAuthorAsync(Book book);
        Task<bool> DeleteBookAsync(Book book);
        
    }
}
