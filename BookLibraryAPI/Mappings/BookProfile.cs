using AutoMapper;
using BookLibraryAPI.Models;

namespace BookLibraryAPI.Mappings
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>();
        }
    }
}
