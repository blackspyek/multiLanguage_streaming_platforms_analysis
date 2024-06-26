﻿using AutoMapper;
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
            CreateMap<TitleApiDTO, Title>();
            // Category
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<TitleCategory, TitleCategoryDTO>();

            // Platform
            CreateMap<Platform, PlatformDTO>();
            CreateMap<PlatformDTO, Platform>();
            CreateMap<TitlePlatform, TitlePlatformDTO>();

            // Country
            CreateMap<Country, CountryDTO>();
            CreateMap<CountryDTO, Country>();
            CreateMap<TitleCountry, TitleCountryDTO>();

            // FullTitleInfo
            CreateMap<Title, FullTitleInfoDTO>()
                .ForMember(dest => dest.TitleCategory, opt => opt.MapFrom(src => src.TitleCategory))
                .ForMember(dest => dest.TitlePlatform, opt => opt.MapFrom(src => src.TitlePlatform))
                .ForMember(dest => dest.TitleCountry, opt => opt.MapFrom(src => src.TitleCountry));

        }
    }
}
