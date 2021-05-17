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
    public interface IMagazinePaperRepository : IRepositoryBase<MagazinePaper>
    {
        //Post Article Review
        Task<int> MagazinePaperPost(PostMagazinePaperDTO model);
        //Update Article Review Article
        Task<int> MagazinePaperArticleAddOrRemove(string action, string userId, MagazinePaperArticle model);
        //Get List MagazinePaper
        Task<VirtualizeResponse<SpMagazinePaperSearchResult>> MagazinePaperSearch(MagazinePaperSearchFilter model);
        //Get MagazinePaper By Id
        Task<MagazinePaper> MagazinePaperGetById(int MagazinePaperId);
        //Get List MagazinePaperArticle by MagazinePaperId
        Task<List<SpArticleSearchResult>> MagazinePaperArticleGetLstById(int MagazinePaperId);
        //Delete
        Task<bool> MagazinePaperDelete(int MagazinePaperId);
        //Update Status MagazinePaper
        Task<bool> MagazinePaperUpdateStatus(int MagazinePaperid, int statusId);
        //Create Url
        Task<string> CreateMagazinePaperURL(int MagazinePaperId);

        Task<VirtualizeResponse<SpArticleInMagazinePaperSearchResult>> ArticleInMagazinePaperSearch(ArticleInMagazinePaperSearchFilter model);
    }
    public class MagazinePaperRepository : RepositoryBase<MagazinePaper>, IMagazinePaperRepository
    {
        public MagazinePaperRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {

        }

        public async Task<int> MagazinePaperArticleAddOrRemove(string action, string userId, MagazinePaperArticle model)
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

        public async Task<List<SpArticleSearchResult>> MagazinePaperArticleGetLstById(int MagazinePaperId)
        {
            List<SpArticleSearchResult> lstOutput = new List<SpArticleSearchResult>();

            var lstArticle = await CmsContext.MagazinePaperArticle.Where(p => p.MagazinePaperId == MagazinePaperId).AsNoTracking().ToListAsync();
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

        public async Task<bool> MagazinePaperDelete(int MagazinePaperId)
        {
            var item = await CmsContext.MagazinePaper.FirstOrDefaultAsync(p => p.Id == MagazinePaperId);
            if (item != null)
            {
                CmsContext.MagazinePaper.Remove(item);
                await CmsContext.SaveChangesAsync();
                //Delete MagazinePaperarticle
                var itemArt = await CmsContext.MagazinePaperArticle.Where(p => p.MagazinePaperId == MagazinePaperId).AsNoTracking().ToListAsync();
                if (itemArt != null)
                {
                    CmsContext.MagazinePaperArticle.RemoveRange(itemArt);
                    await CmsContext.SaveChangesAsync();
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public async Task<MagazinePaper> MagazinePaperGetById(int MagazinePaperId)
        {
            var output = new MagazinePaper();
            var result = await CmsContext.MagazinePaper.Where(p => p.Id == MagazinePaperId).FirstOrDefaultAsync();
            if (result != null)
            {
                output = result;
            }
            return output;
        }

        public async Task<int> MagazinePaperPost(PostMagazinePaperDTO model)
        {
            if (model.MagazinePaper.Id == null) // Add
            {
                var item = new MagazinePaper();
                item.Name = model.MagazinePaper.Name;
                item.Description = model.MagazinePaper.Description;
                item.StartDate = model.MagazinePaper.StartDate;
                item.EndDate = model.MagazinePaper.EndDate;
                item.CreateBy = model.MagazinePaper.CreateBy;
                item.CreateDate = DateTime.Now;
                item.LastEditDate = DateTime.Now;
                item.LastEditBy = model.MagazinePaper.LastEditBy;
                item.MagazinePaperStatusId = 1;
                CmsContext.Entry(item).State = EntityState.Added;
                await CmsContext.SaveChangesAsync();

                //Add ListArticle
                if (model.LstArticleId != null)
                {
                    List<MagazinePaperArticle> lstMagazinePaperArticle = new List<MagazinePaperArticle>();
                    foreach (var p in model.LstArticleId)
                    {
                        MagazinePaperArticle itemMagazinePaperArt = new MagazinePaperArticle();
                        itemMagazinePaperArt.MagazinePaperId = item.Id;
                        itemMagazinePaperArt.ArticleId = p;
                        itemMagazinePaperArt.CreateBy = model.MagazinePaper.CreateBy;
                        itemMagazinePaperArt.CreateDate = DateTime.Now;
                        itemMagazinePaperArt.LastEditDate = DateTime.Now;
                        itemMagazinePaperArt.LastEditBy = model.MagazinePaper.LastEditBy;
                        lstMagazinePaperArticle.Add(itemMagazinePaperArt);
                    };
                    await CmsContext.MagazinePaperArticle.AddRangeAsync(lstMagazinePaperArticle);
                    await CmsContext.SaveChangesAsync();
                }
                return item.Id;
            }
            else// Update Only
            {
                var MagazinePaperItems = await CmsContext.MagazinePaper.FirstOrDefaultAsync(p => p.Id == model.MagazinePaper.Id);
                if (MagazinePaperItems != null)
                {
                    MagazinePaperItems.Name = model.MagazinePaper.Name;
                    MagazinePaperItems.Description = model.MagazinePaper.Description;
                    MagazinePaperItems.StartDate = model.MagazinePaper.StartDate;
                    MagazinePaperItems.EndDate = model.MagazinePaper.EndDate;
                    MagazinePaperItems.LastEditDate = DateTime.Now;
                    MagazinePaperItems.LastEditBy = model.MagazinePaper.LastEditBy;
                    CmsContext.Entry(MagazinePaperItems).State = EntityState.Modified;
                    await CmsContext.SaveChangesAsync();
                }
                //Delete List Person Review And MagazinePaper Article               
                var lstMagazinePaperArticleDel = await CmsContext.MagazinePaperArticle.Where(p => p.MagazinePaperId == MagazinePaperItems.Id).AsNoTracking().ToListAsync();
                if (lstMagazinePaperArticleDel != null)
                {
                    CmsContext.MagazinePaperArticle.RemoveRange(lstMagazinePaperArticleDel);
                    await CmsContext.SaveChangesAsync();
                }
                //Add ListPerson Review

                //Add ListArticle
                if (model.LstArticleId != null)
                {
                    List<MagazinePaperArticle> lstMagazinePaperArticle = new List<MagazinePaperArticle>();
                    foreach (var p in model.LstArticleId)
                    {
                        MagazinePaperArticle itemMagazinePaperArt = new MagazinePaperArticle();
                        itemMagazinePaperArt.MagazinePaperId = MagazinePaperItems.Id;
                        itemMagazinePaperArt.ArticleId = p;
                        itemMagazinePaperArt.CreateBy = model.MagazinePaper.CreateBy;
                        itemMagazinePaperArt.CreateDate = DateTime.Now;
                        itemMagazinePaperArt.LastEditDate = DateTime.Now;
                        itemMagazinePaperArt.LastEditBy = model.MagazinePaper.LastEditBy;
                        lstMagazinePaperArticle.Add(itemMagazinePaperArt);
                    };
                    await CmsContext.MagazinePaperArticle.AddRangeAsync(lstMagazinePaperArticle);
                    await CmsContext.SaveChangesAsync();
                }
                return (int)model.MagazinePaper.Id;
            }
        }

        public async Task<VirtualizeResponse<SpMagazinePaperSearchResult>> MagazinePaperSearch(MagazinePaperSearchFilter model)
        {
            var output = new VirtualizeResponse<SpMagazinePaperSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();
            var result = await CmsContext.GetProcedures().SpMagazinePaperSearchAsync(
             model.Keyword,
             model.MagazinePaperId,
             model.MagazinePaperStatusId,
             model.PageSize,
             model.CurrentPage,
             itemCounts,
             returnValues
         );
            output.Items = result.ToList();
            output.TotalSize = (int)itemCounts.Value;
            return output;
        }

        public async Task<bool> MagazinePaperUpdateStatus(int MagazinePaperid, int statusId)
        {
            var item = await CmsContext.MagazinePaper.FirstOrDefaultAsync(p => p.Id == MagazinePaperid);
            if (item != null)
            {
                item.MagazinePaperStatusId = statusId;
                await CmsContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> CreateMagazinePaperURL(int MagazinePaperId)
        {
            try
            {
                var currentArticle = CmsContext.MagazinePaper.FirstOrDefault(p => p.Id == MagazinePaperId);
                return FormatURL(currentArticle?.Name) + "-" + MagazinePaperId.ToString();
            }
            catch
            {

            }
            return "nourl";
        }



        public async Task<VirtualizeResponse<SpArticleInMagazinePaperSearchResult>> ArticleInMagazinePaperSearch(ArticleInMagazinePaperSearchFilter model)
        {
            var output = new VirtualizeResponse<SpArticleInMagazinePaperSearchResult>();
            var itemCounts = new OutputParameter<int?>();
            var returnValues = new OutputParameter<int>();
            try
            {
                var result = await CmsContext.GetProcedures().SpArticleInMagazinePaperSearchAsync(
                 model.Keyword,
                 model.MagazinePaperId,
                 model.ArticleInMagazinePaperStatusId,
                 null,
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
    }
}
