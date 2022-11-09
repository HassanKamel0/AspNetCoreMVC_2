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
            CreateMap<Data.ApplicationUser, Models.UserViewModel>()
                .ForMember(u=>u.HasPassword,op=>op.MapFrom(u=>u.PasswordHash!=null));
        }
    }
}
