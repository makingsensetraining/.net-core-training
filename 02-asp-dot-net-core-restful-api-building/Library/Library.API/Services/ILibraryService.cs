using Library.API.Entities;
using Library.API.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.API.Services
{
    /// <summary>
    /// Defines the contract for the service responsible to send the requests to get and set Library data to persistent repository.
    /// </summary>
    public interface ILibraryService
    {
        /// <summary>
        /// Get a paged list of authors as an asynchronous operation.
        /// </summary>
        /// <param name="authorResourceParameters">A set of parameters to set page size, page number, filters and searchs terms.</param>
        /// <returns>An list of authors that meet the input parameters.</returns>
        Task<PagedList<Author>> GetAuthorsAsync(AuthorResourceParameters authorResourceParameters);

        /// <summary>
        /// Get an author by id.
        /// </summary>
        /// <param name="authorId">A Guid that represent the Id for the author.</param>
        /// <returns>An asynchronous operation with the Author.</returns>
        Task<Author> GetAuthorAsync(Guid authorId);

        /// <summary>
        /// Get a list of authors by ids.
        /// </summary>
        /// <param name="authorIds">A list of Guid that represents the Id of each author.</param>
        /// <returns>An asynchronous operation with the list of authors.</returns>
        Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds);

        /// <summary>
        /// Send a request to create an author.
        /// </summary>
        /// <param name="author">The new author.</param>
        /// <returns>An asynchronous operation with a flag that indicates whether the author was created.</returns>
        Task<bool> AddAuthorAsync(Author author);

        /// <summary>
        /// Send a request to delete an author.
        /// </summary>
        /// <param name="author">The author to delete.</param>
        /// <returns>An asynchronous operation with a flag that indicates whether the author was removed.</returns>
        Task<bool> DeleteAuthorAsync(Author author);

        /// <summary>
        /// Send a request to update an author.
        /// </summary>
        /// <param name="author">The author to update.</param>
        /// <returns>An asynchronous operation with a flag that indicates whether the author was updated.</returns>
        Task<bool> UpdateAuthorAsync(Author author);

        /// <summary>
        /// Send a request to check if a user exists.
        /// </summary>
        /// <param name="authorId">The Guid that represents the Id of the author being validated.</param>
        /// <returns>An asynchronous operation with a flag that indicates whether the author exists.</returns>
        Task<bool> AuthorExistsAsync(Guid authorId);

        /// <summary>
        /// Get the list of books that belogs to an author.
        /// </summary>
        /// <param name="authorId">The Guid that represents the Id of the author of the book.</param>
        /// <returns>An asynchronous operation with the list of books that belogs to the author with the provided Id.</returns>
        Task<IEnumerable<Book>> GetBooksForAuthorAsync(Guid authorId);

        /// <summary>
        /// Get a book by id that belongs to an specific author.
        /// </summary>
        /// <param name="authorId">The Guid that represents the Id of the author of the book.</param>
        /// <param name="bookId">The Guid that represents the Id of the book.</param>
        /// <returns>An asynchronous operation with the book.</returns>
        Task<Book> GetBookForAuthorAsync(Guid authorId, Guid bookId);

        /// <summary>
        /// Send a request to create a book for an author.
        /// </summary>
        /// <param name="authorId">The Guid that represents the Id of the author of the book.</param>
        /// <param name="book">The new book.</param>
        /// <returns>An asynchronous operation with a flag that indicates whether the book was created.</returns>
        Task<bool> AddBookForAuthorAsync(Guid authorId, Book book);

        /// <summary>
        /// Send a request to update a book.
        /// </summary>
        /// <param name="book">The book to update.</param>
        /// <returns>An asynchronous operation with a flag that indicates whether the book was updated.</returns>
        Task<bool> UpdateBookForAuthorAsync(Book book);

        /// <summary>
        /// Send a request to delete a book.
        /// </summary>
        /// <param name="book">The book to delete.</param>
        /// <returns>An asynchronous operation with a flag that indicates whether the book was removed.</returns>
        Task<bool> DeleteBookAsync(Book book);
        
    }
}
