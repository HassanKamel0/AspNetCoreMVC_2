using AutoMapper;

namespace WoodyMovie.AutoMapperProfiles
{
    public class MovieProfile: Profile
    {
        public MovieProfile()
        {
            CreateMap<Models.Movie, Data.ApplicationDbContext>();
            CreateMap<Data.ApplicationDbContext,Models.Movie>();
        }
    }
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Models.UserViewModel, Data.ApplicationUser>();
            CreateMap<Data.ApplicationUser, Models.UserViewModel>();
        }
    }
}
