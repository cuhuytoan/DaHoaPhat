using AutoMapper;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;

namespace CMS.Website.AutoMap
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Account
            CreateMap<AspNetUsers, AspNetUsersDTO>().ReverseMap();
            CreateMap<AspNetUserProfiles, AspNetUserProfilesDTO>().ReverseMap();
            CreateMap<AspNetUserRoles, AspNetUserRolesDTO>().ReverseMap();
            CreateMap<AspNetUserInfo, AspNetUserInfoDTO>().ReverseMap();
            //Article
            CreateMap<Article, ArticleDTO>().ReverseMap();
            CreateMap<ArticleCategory, ArticleCategoryDTO>().ReverseMap();

            CreateMap<ArticleSearchDTO, SpArticleSearchResult>().ReverseMap();


            CreateMap<ArticleGetByBlockIdDTO, SpArticleGetByBlockIdResult>().ReverseMap();
            CreateMap<ArticleGetTopByCategoryIdDTO, SpArticleGetTopByCategoryIdResult>().ReverseMap();
            CreateMap<ArticleGetByBlockIdDTO, SpArticleSearchResult>().ReverseMap();
            CreateMap<ArticleGetByBlockIdDTO, SpArticleGetTopByCategoryIdResult>().ReverseMap();
            CreateMap<ArticleGetByBlockIdDTO, SpArticleGetNewByCategoryIdResult>().ReverseMap();
            CreateMap<ArticleSearchDTO, SpArticleGetNewByCategoryIdResult>().ReverseMap();
            CreateMap<Article, SpArticleSearchResult>().ReverseMap();
            CreateMap<ArticleGetByBlockIdDTO, SpArticleGetByCategoryIdResult>().ReverseMap();
            CreateMap<ArticleDTO, SpArticleSearchResult>().ReverseMap();
            //ArticleComment 
            CreateMap<ArticleCommentDTO, ArticleComment>().ReverseMap();
            CreateMap<ArticleSearchDTO, SpArticleCommentSearchResult>().ReverseMap();
           

            //Setting
            CreateMap<SettingDTO, Setting>().ReverseMap();

        }
    }
}
