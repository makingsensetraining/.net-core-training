using Library.API.Entities;
using Library.API.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public interface ILibraryService
    {
        PagedList<Author> GetAuthors(AuthorResourceParameters authorResourceParameters);
        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        bool AddAuthor(Author author);
        bool DeleteAuthor(Author author);
        bool UpdateAuthor(Author author);
        bool AuthorExists(Guid authorId);
        IEnumerable<Book> GetBooksForAuthor(Guid authorId);
        Book GetBookForAuthor(Guid authorId, Guid bookId);
        bool AddBookForAuthor(Guid authorId, Book book);
        bool UpdateBookForAuthor(Book book);
        bool DeleteBook(Book book);

        Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds);
        Task<bool> AddAuthorAsync(Author author);
    }
}
