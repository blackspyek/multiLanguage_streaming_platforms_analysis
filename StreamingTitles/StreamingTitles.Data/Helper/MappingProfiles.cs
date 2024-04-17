using AutoMapper;
using StreamingTitles.Data.DTO;
using StreamingTitles.Data.Model;

namespace StreamingTitles.Data.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Title
            CreateMap<Title, TitleDTO>();
            CreateMap<Title, TitleDataDTO>();
            CreateMap<TitleDTO, Title>();
            // Category
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<TitleCategory, TitleCategoryDTO>();

            // Platform
            CreateMap<Platform, PlatformDTO>();
            CreateMap<PlatformDTO, Platform>();
            CreateMap<TitlePlatform, TitlePlatformDTO>();

        }
    }
}
