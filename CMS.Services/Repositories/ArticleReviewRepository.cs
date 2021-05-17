using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{

    public interface IArticleReviewRepository : IRepositoryBase<ArticleReview>
    {
        //Add or update
        Task<int> ArticleReviewPost(PostArticleReviewDTO model);
        //Update Person Review
        Task<int> ArticleReviewPersonAddOrRemove(string action, string userId, ArticleReviewPerson model);
        //Update Article Review Article
        Task<int> ArticleReviewArticleAddOrRemove(string action, string userId, ArticleReviewArticle model);
        //Get List ArticleReview
        Task<VirtualizeResponse<SpArticleReviewSearchResult>> ArticleReviewSearch(ArticleReviewSearchFilter model);
        //Get List ArticleReviewPerson by ArticleReviewId
        Task<List<ArticleReviewPersonDTO>> ArticleReviewPersonGetLstById(int articleReviewId);
        //Get ArticleReview By Id
        Task<ArticleReview> ArticleReviewGetById(int articleReviewId);
        //Get List ArticleReviewArticle by ArticleReviewId
        Task<List<SpArticleSearchResult>> ArticleReviewArticleGetLstById(int articleReviewId);
        //Delete
        Task<bool> ArticleReviewDelete(int articleReviewId);
        //GetList ArticleInReview
        Task<VirtualizeResponse<SpArticleInReviewSearchResult>> ArticleInReviewSearch(ArticleInReviewSearchFilter model);
        //GetList ReviewDetail
        Task<VirtualizeResponse<SpArticleReviewDetailSearchResult>> ArticleReviewDetailSearch(ArticleReviewDetailSearchFilter model);
        //Getlst ArticleReview Status
        Task<List<ArticleReviewStatus>> ArticleReviewStatusGetLst();
        //Post Review Detail
        Task<int> PostReviewDetail(ArticleReviewDetail model);
        //Get Detail  ArticleReviewDetail By id
        Task<ArticleReviewDetail> ArticleReviewDetailGetById(int articleReviewDetailId);
        //Update Status ArticleReview
        Task<bool> ArticleReviewUpdateStatus(int articleReviewid, int statusId);

    }
    public class ArticleReviewRepository : RepositoryBase<ArticleReview>, IArticleReviewRepository
    {

        public ArticleReviewRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {

        }

        public async Task<int> ArticleReviewArticleAddOrRemove(string action, string userId, ArticleReviewArticle model)
        {
            model.LastEditBy = userId;
            model.LastEditDate = DateTime.Now;
            if (action == "A")
            {
                model.CreateBy = userId;
                model.CreateDate = DateTime.Now;
            }
            CmsContext.Entry(model).State = action == "A" ? EntityState.Added : EntityState.Modified;
            await CmsContext.SaveChangesAsync();
            return model.Id;
        }

        public async Task<List<SpArticleSearchResult>> ArticleReviewArticleGetLstById(int articleReviewId)
        {
            List<SpArticleSearchResult> lstOutput = new List<SpArticleSearchResult>();

            var lstArticle = await CmsContext.ArticleReviewArticle.Where(p => p.ArticleReviewId == articleReviewId).AsNoTracking().ToListAsync();
            if (lstArticle != null)
            {
                foreach (var p in lstArticle)
                {
                    SpArticleSearchResult item = new SpArticleSearchResult();
                    var article = await CmsContext.Article.FirstOrDefaultAsync(x => x.Id == p.ArticleId);
                    if (article != null)
                    {
                        item.Id = article.Id;
                        item.Image = article.Image;
                        item.Name = article.Name;
                        item.CreateDate = article.CreateDate;
                        if (Int32.TryParse(item.ArticleCategoryIds, out int _cate))
                        {
                            var cate = await CmsContext.ArticleCategory.FirstOrDefaultAsync(x => x.Id == _cate);
                            item.ArticleCategoryName = cate.Name;
                        }
                    }
                    lstOutput.Add(item);
                }
            }
            return lstOutput;
        }

        public async Task<int> ArticleReviewPersonAddOrRemove(string action, string userId, ArticleReviewPerson model)
        {
            model.LastEditBy = userId;
            model.LastEditDate = DateTime.Now;
            if (action == "A")
            {
                model.CreateBy = userId;
                model.CreateDate = DateTime.Now;
            }
            CmsContext.Entry(model).State = action == "A" ? EntityState.Added : EntityState.Modified;
            await CmsContext.SaveChangesAsync();
            return model.Id;
        }

        public async Task<List<ArticleReviewPersonDTO>> ArticleReviewPersonGetLstById(int articleReviewId)
        {
            List<ArticleReviewPersonDTO> lstOutput = new List<ArticleReviewPersonDTO>();
            var lstPerson = await CmsContext.ArticleReviewPerson.Where(p => p.ArticleReviewId == articleReviewId).AsNoTracking().ToListAsync();
            if (lstPerson != null)
            {
                foreach (var p in lstPerson)
                {
                    ArticleReviewPersonDTO item = new ArticleReviewPersonDTO();
                    var profile = await CmsContext.AspNetUserProfiles.FirstOrDefaultAsync(x => x.UserId == p.UserId);
                    if (profile != null)
                    {
                        item.UserId = profile.UserId;
                        item.FullName = profile.FullName;
                        item.AvatarUrl = profile.AvatarUrl;
                        var userItem = await CmsContext.AspNetUsers.FirstOrDefaultAsync(x => x.Id == profile.UserId);
                        if (userItem != null)
                        {
                            item.Email = userItem.UserName;
                        }

                        lstOutput.Add(item);
                    }
                }
            }
            return lstOutput;
        }



        public async Task<VirtualizeResponse<SpArticleReviewSearchResult>> ArticleReviewSearch(ArticleReviewSearchFilter model)
        {
            var output = new VirtualizeResponse<SpArticleReviewSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();
            var result = await CmsContext.GetProcedures().SpArticleReviewSearchAsync(
             model.Keyword,
             model.ArticleReviewId,
             model.ArticleReviewStatusId,
             model.PageSize,
             model.CurrentPage,
             itemCounts,
             returnValues
         );
            output.Items = result.ToList();
            output.TotalSize = (int)itemCounts.Value;
            return output;
        }

        public async Task<int> ArticleReviewPost(PostArticleReviewDTO model)
        {
            if (model.ArticleReview.Id == null) // Add
            {
                var item = new ArticleReview();
                item.Name = model.ArticleReview.Name;
                item.Description = model.ArticleReview.Description;
                item.StartDate = model.ArticleReview.StartDate;
                item.EndDate = model.ArticleReview.EndDate;
                item.CreateBy = model.ArticleReview.CreateBy;
                item.CreateDate = DateTime.Now;
                item.LastEditDate = DateTime.Now;
                item.LastEditBy = model.ArticleReview.LastEditBy;
                item.ArticleRevStatusId = 1;
                CmsContext.Entry(item).State = EntityState.Added;
                await CmsContext.SaveChangesAsync();
                //Add ListPerson Review
                if (model.LstReviewPerson != null)
                {
                    List<ArticleReviewPerson> lstArticleReviewPerson = new List<ArticleReviewPerson>();
                    foreach (var p in model.LstReviewPerson)
                    {
                        ArticleReviewPerson itemPerson = new ArticleReviewPerson();
                        itemPerson.ArticleReviewId = item.Id;
                        itemPerson.UserId = p;
                        itemPerson.CreateBy = model.ArticleReview.CreateBy;
                        itemPerson.CreateDate = DateTime.Now;
                        itemPerson.LastEditDate = DateTime.Now;
                        itemPerson.LastEditBy = model.ArticleReview.LastEditBy;
                        lstArticleReviewPerson.Add(itemPerson);
                    }
                    await CmsContext.ArticleReviewPerson.AddRangeAsync(lstArticleReviewPerson);
                    await CmsContext.SaveChangesAsync();

                }
                //Add ListArticle
                if (model.LstArticleId != null)
                {
                    List<ArticleReviewArticle> lstArticleReviewArticle = new List<ArticleReviewArticle>();
                    foreach (var p in model.LstArticleId)
                    {
                        ArticleReviewArticle itemArticleReviewArt = new ArticleReviewArticle();
                        itemArticleReviewArt.ArticleReviewId = item.Id;
                        itemArticleReviewArt.ArticleId = p;
                        itemArticleReviewArt.CreateBy = model.ArticleReview.CreateBy;
                        itemArticleReviewArt.CreateDate = DateTime.Now;
                        itemArticleReviewArt.LastEditDate = DateTime.Now;
                        itemArticleReviewArt.LastEditBy = model.ArticleReview.LastEditBy;
                        lstArticleReviewArticle.Add(itemArticleReviewArt);
                    };
                    await CmsContext.ArticleReviewArticle.AddRangeAsync(lstArticleReviewArticle);
                    await CmsContext.SaveChangesAsync();
                }
                return item.Id;
            }
            else// Update Only
            {
                var articleReviewItems = await CmsContext.ArticleReview.FirstOrDefaultAsync(p => p.Id == model.ArticleReview.Id);
                if (articleReviewItems != null)
                {
                    articleReviewItems.Name = model.ArticleReview.Name;
                    articleReviewItems.Description = model.ArticleReview.Description;
                    articleReviewItems.StartDate = model.ArticleReview.StartDate;
                    articleReviewItems.EndDate = model.ArticleReview.EndDate;
                    articleReviewItems.LastEditDate = DateTime.Now;
                    articleReviewItems.LastEditBy = model.ArticleReview.LastEditBy;
                    CmsContext.Entry(articleReviewItems).State = EntityState.Modified;
                    await CmsContext.SaveChangesAsync();
                }
                //Delete List Person Review And ArticleReview Article
                var lstPersonDel = await CmsContext.ArticleReviewPerson.Where(p => p.ArticleReviewId == articleReviewItems.Id).AsNoTracking().ToListAsync();
                if (lstPersonDel != null)
                {
                    CmsContext.ArticleReviewPerson.RemoveRange(lstPersonDel);
                    await CmsContext.SaveChangesAsync();
                }
                var lstArticleReviewArticleDel = await CmsContext.ArticleReviewArticle.Where(p => p.ArticleReviewId == articleReviewItems.Id).AsNoTracking().ToListAsync();
                if (lstArticleReviewArticleDel != null)
                {
                    CmsContext.ArticleReviewArticle.RemoveRange(lstArticleReviewArticleDel);
                    await CmsContext.SaveChangesAsync();
                }
                //Add ListPerson Review
                if (model.LstReviewPerson != null)
                {
                    List<ArticleReviewPerson> lstArticleReviewPerson = new List<ArticleReviewPerson>();
                    foreach (var p in model.LstReviewPerson)
                    {
                        ArticleReviewPerson itemPerson = new ArticleReviewPerson();
                        itemPerson.ArticleReviewId = articleReviewItems.Id;
                        itemPerson.UserId = p;
                        itemPerson.CreateBy = model.ArticleReview.CreateBy;
                        itemPerson.CreateDate = DateTime.Now;
                        itemPerson.LastEditDate = DateTime.Now;
                        itemPerson.LastEditBy = model.ArticleReview.LastEditBy;
                        lstArticleReviewPerson.Add(itemPerson);
                    }
                    await CmsContext.ArticleReviewPerson.AddRangeAsync(lstArticleReviewPerson);
                    await CmsContext.SaveChangesAsync();

                }
                //Add ListArticle
                if (model.LstArticleId != null)
                {
                    List<ArticleReviewArticle> lstArticleReviewArticle = new List<ArticleReviewArticle>();
                    foreach (var p in model.LstArticleId)
                    {
                        ArticleReviewArticle itemArticleReviewArt = new ArticleReviewArticle();
                        itemArticleReviewArt.ArticleReviewId = articleReviewItems.Id;
                        itemArticleReviewArt.ArticleId = p;
                        itemArticleReviewArt.CreateBy = model.ArticleReview.CreateBy;
                        itemArticleReviewArt.CreateDate = DateTime.Now;
                        itemArticleReviewArt.LastEditDate = DateTime.Now;
                        itemArticleReviewArt.LastEditBy = model.ArticleReview.LastEditBy;
                        lstArticleReviewArticle.Add(itemArticleReviewArt);
                    };
                    await CmsContext.ArticleReviewArticle.AddRangeAsync(lstArticleReviewArticle);
                    await CmsContext.SaveChangesAsync();
                }
                return (int)model.ArticleReview.Id;
            }

        }

        public async Task<ArticleReview> ArticleReviewGetById(int articleReviewId)
        {
            var output = new ArticleReview();
            var result = await CmsContext.ArticleReview.Where(p => p.Id == articleReviewId).FirstOrDefaultAsync();
            if (result != null)
            {
                output = result;
            }
            return output;
        }

        public async Task<bool> ArticleReviewDelete(int articleReviewId)
        {
            var item = await CmsContext.ArticleReview.FirstOrDefaultAsync(p => p.Id == articleReviewId);
            if (item != null)
            {
                CmsContext.ArticleReview.Remove(item);
                await CmsContext.SaveChangesAsync();
                //Detete ArticleReviewPerson
                var itemPerson = await CmsContext.ArticleReviewPerson.Where(p => p.ArticleReviewId == articleReviewId).AsNoTracking().ToListAsync();
                if (itemPerson != null)
                {
                    CmsContext.ArticleReviewPerson.RemoveRange(itemPerson);
                    await CmsContext.SaveChangesAsync();
                }
                //Delete ArticleReviewarticle
                var itemArt = await CmsContext.ArticleReviewArticle.Where(p => p.ArticleReviewId == articleReviewId).AsNoTracking().ToListAsync();
                if (itemArt != null)
                {
                    CmsContext.ArticleReviewArticle.RemoveRange(itemArt);
                    await CmsContext.SaveChangesAsync();
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public async Task<VirtualizeResponse<SpArticleInReviewSearchResult>> ArticleInReviewSearch(ArticleInReviewSearchFilter model)
        {
            var output = new VirtualizeResponse<SpArticleInReviewSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();
            try
            {
                var result = await CmsContext.GetProcedures().SpArticleInReviewSearchAsync(
                 model.Keyword,
                 model.ArticleReviewId,
                 model.ArticleInReviewStatusId,
                 model.UserInReviewId,
                 model.PageSize,
                 model.CurrentPage,
                 itemCounts,
                 returnValues
             );
                output.Items = result.ToList();
                output.TotalSize = (int)itemCounts.Value;
            }

            catch (Exception ex)
            {

            }
            return output;
        }

        public async Task<VirtualizeResponse<SpArticleReviewDetailSearchResult>> ArticleReviewDetailSearch(ArticleReviewDetailSearchFilter model)
        {
            var output = new VirtualizeResponse<SpArticleReviewDetailSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();
            var result = await CmsContext.GetProcedures().SpArticleReviewDetailSearchAsync(
             model.Keyword,
             model.ArticleId,
             model.CreateBy,
             model.PageSize,
             model.CurrentPage,
             itemCounts,
             returnValues
         );
            output.Items = result.ToList();
            output.TotalSize = (int)itemCounts.Value;
            return output;
        }

        public async Task<List<ArticleReviewStatus>> ArticleReviewStatusGetLst()
        {

            return await CmsContext.ArticleReviewStatus.ToListAsync();
        }

        public async Task<int> PostReviewDetail(ArticleReviewDetail model)
        {

            CmsContext.Entry(model).State = model.Id == 0 ?
                EntityState.Added : EntityState.Modified;
            await CmsContext.SaveChangesAsync();
            //Kiểm tra điều kiện chuyển trạng thái bài viết sang chờ đăng
            var lstArtReview = await CmsContext.ArticleReviewDetail.Where(p => p.ArticleId == model.ArticleId).AsNoTracking().ToListAsync();
            if (lstArtReview != null)
            {
                bool isUpdate = true;
                foreach (var p in lstArtReview)
                {
                    if (p.ArticleReviewStatusId == null)
                    {
                        isUpdate = false;
                    }
                    if (p.ArticleReviewStatusId == 3 || p.ArticleReviewStatusId == 4)
                    {
                        isUpdate = false;
                    }
                }
                if (isUpdate)
                {
                    var articleItem = await CmsContext.Article.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.ArticleId);
                    if (articleItem != null)
                    {
                        articleItem.ArticleStatusId = 5;
                        articleItem.LastEditDate = DateTime.Now;
                        CmsContext.Entry(articleItem).State = EntityState.Modified;
                        await CmsContext.SaveChangesAsync();
                    }

                }
            }

            return model.Id;

        }

        public async Task<ArticleReviewDetail> ArticleReviewDetailGetById(int articleReviewDetailId)
        {
            return await CmsContext.ArticleReviewDetail.AsNoTracking().FirstOrDefaultAsync(p => p.Id == articleReviewDetailId);
        }

        public async Task<bool> ArticleReviewUpdateStatus(int articleReviewid, int statusId)
        {
            var item = await CmsContext.ArticleReview.FirstOrDefaultAsync(p => p.Id == articleReviewid);
            if (item != null)
            {
                item.ArticleRevStatusId = statusId;
                await CmsContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
