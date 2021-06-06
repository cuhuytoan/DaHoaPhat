using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Website.Areas.Admin.Pages.Shared.Components;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.Article
{
    public partial class Index : IDisposable
    {
        #region Inject

        [Inject]
        private IMapper Mapper { get; set; }

        [Inject]
        private ILoggerManager Logger { get; set; }

        [Inject]
        private UserManager<IdentityUser> UserManager { get; set; }

        #endregion Inject

        #region Parameter

        [Parameter]
        public string keyword { get; set; }

        [Parameter]
        public int? articleCategoryId { get; set; }

        [Parameter]
        public int? p { get; set; }

        #endregion Parameter

        #region Model

        private List<SpArticleSearchResult> lstArticle;
        public int currentPage { get; set; }
        public int totalCount { get; set; }
        public int pageSize { get; set; } = 30;
        public int totalPages => (int)Math.Ceiling(decimal.Divide(totalCount, pageSize));
        public ArticleSearchFilter modelFilter { get; set; }
        public int? articleCategorySelected { get; set; }
        public int? articleStatusSelected { get; set; }
        public int? setArticleStatusSelected { get; set; }
        private List<ArticleCategory> lstArticleCategory { get; set; }
        private List<ArticleStatus> lstArticleStatus { get; set; }
        private string subTitle { get; set; } = "bài viết đã cập nhật";
        public string outMessage = "";
    
        [CascadingParameter]
        protected GlobalModel globalModel { get; set; }

        protected ConfirmBase DeleteConfirmation { get; set; }
        private List<int> listArticleSelected { get; set; } = new List<int>();
        private bool _forceRerender;
        private bool isCheck { get; set; }

        #endregion Model

        #region LifeCycle

        protected override void OnInitialized()
        {
            //Add for change location and seach not reload page
            NavigationManager.LocationChanged += HandleLocationChanged;
        }

        protected override async Task OnInitializedAsync()
        {
            //var authState = await authenticationStateTask;
            //user = authState.User;
            await InitControl();
            await InitData();
        }

        protected override bool ShouldRender()
        {
            if (_forceRerender)
            {
                _forceRerender = false;
                return true;
            }
            return base.ShouldRender();
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= HandleLocationChanged;
            GC.SuppressFinalize(this);
        }

        #endregion LifeCycle

        #region Init

        protected async Task InitControl()
        {
            //Binding Category
            if(globalModel.user.IsInRole("Phụ trách chuyên mục"))
            {
                var lstArticleCate = await Repository.ArticleCategory.GetArticleCategoryByUserId(globalModel.userId);
                if (lstArticleCate != null)
                {
                    lstArticleCategory = lstArticleCate.Select(x => new ArticleCategory { Id = x.Id, Name = x.Name }).ToList();
                }
                //Binding Status
                var lstStatus = await Repository.Article.GetLstArticleStatusByUserId(globalModel.userId);
                if (lstStatus != null)
                {
                    lstArticleStatus = lstStatus.Select(x => new ArticleStatus { Id = x.Id, Name = x.Name }).ToList();
                }
            }
        
            else
            {
                var lstArticleCate = await Repository.ArticleCategory.GetArticleCategoryById(null);
                if (lstArticleCate != null)
                {
                    lstArticleCategory = lstArticleCate.Select(x => new ArticleCategory { Id = x.Id, Name = x.Name }).ToList();
                }
                //Binding Status
                var lstStatus = await Repository.Article.GetLstArticleStatus();
                if (lstStatus != null)
                {
                    lstArticleStatus = lstStatus.Select(x => new ArticleStatus { Id = x.Id, Name = x.Name }).ToList();
                }
            }
            
           
        }

        protected async Task InitData()
        {
            Logger.LogDebug("Init");
            var modelFilter = new ArticleSearchFilter();
            modelFilter.Keyword = keyword;
            modelFilter.ArticleCategoryId = articleCategorySelected;
            modelFilter.CurrentPage = p ?? 1;
            modelFilter.PageSize = 30;
            modelFilter.ArticleStatusId = articleStatusSelected;
            modelFilter.FromDate = DateTime.Now.AddYears(-10);
            modelFilter.ToDate = DateTime.Now;
            if (globalModel.user.IsInRole("Cộng tác viên") )
            {
                modelFilter.CreateBy = globalModel.userId;
            }
            if (globalModel.user.IsInRole("Phụ trách chuyên mục"))
            {
                modelFilter.AssignBy = globalModel.userId;
            }
            var result = await Repository.Article.ArticleSearchWithPaging(modelFilter);

            lstArticle = result.Items;
            totalCount = result.TotalSize;

            //Init Selected
            listArticleSelected.Clear();
            StateHasChanged();
        }

        #endregion Init

        #region Event

        protected async Task OnPostDemand(int postType)
        {
            if (listArticleSelected.Count == 0)
            {
                toastService.ShowToast(ToastLevel.Warning, "Chưa chọn bài viết thực thi", "Thông báo");
                return;
            }
            else
            {
                try
                {
                    foreach (var item in listArticleSelected)
                    {
                        await Repository.Article.ArticleUpdateStatusType(globalModel.userId,item, postType);
                    }
                    
                    toastService.ShowToast(ToastLevel.Success, "Cập nhật thành công", "Thành công!");
                   
                }
                catch(Exception ex)
                {
                    toastService.ShowToast(ToastLevel.Warning, $"Có lỗi trong quá trình thực thi {ex.ToString()}", "Lỗi!");
                }
                _forceRerender = true;
                StateHasChanged();
                await InitData();
            }
        }

        protected void DeleteArticle(int? articleId)
        {
            
            if (articleId == null) // Delete Demand
            {
                if (listArticleSelected.Count == 0)
                {
                    toastService.ShowToast(ToastLevel.Warning, "Chưa chọn bài viết để xóa", "Thông báo");
                    return;
                }
            }
            else
            {
                listArticleSelected.Clear();
                listArticleSelected.Add((int)articleId);
                if (!Repository.Permission.CanDeleteArticle(globalModel.user, globalModel.userId, (int)articleId, ref outMessage))
                {
                    toastService.ShowError(outMessage, "Thông báo");
                    return;
                }    
            
            }

            DeleteConfirmation.Show();
        }

        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                try
                {
                    foreach (var item in listArticleSelected)
                    {
                        await Repository.Article.ArticleDelete(item);
                    }
                    toastService.ShowToast(ToastLevel.Success, "Xóa bài viết thành công", "Thành công");
                }
                catch
                {
                    toastService.ShowToast(ToastLevel.Warning, "Có lỗi trong quá trình thực thi", "Lỗi!");
                }
                StateHasChanged();
                await InitData();
            }
        }

        protected void OnCheckBoxChange(bool headerChecked, int ArticleId, object isChecked)
        {
            if (headerChecked)
            {
                if ((bool)isChecked)
                {
                    listArticleSelected.AddRange(lstArticle.Select(x => x.Id));
                    isCheck = true;
                }
                else
                {
                    isCheck = false;
                    listArticleSelected.Clear();
                }
            }
            else
            {
                if ((bool)isChecked)
                {
                    if (!listArticleSelected.Contains(ArticleId))
                    {
                        listArticleSelected.Add(ArticleId);
                    }
                }
                else
                {
                    if (listArticleSelected.Contains(ArticleId))
                    {
                        listArticleSelected.Remove(ArticleId);
                    }
                }
            }
            StateHasChanged();
        }

        protected async void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            GetQueryStringValues();
            StateHasChanged();
            await InitData();
        }

        protected void GetQueryStringValues()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryStrings = QueryHelpers.ParseQuery(uri.Query);
            if (queryStrings.TryGetValue("keyword", out var _keyword))
            {
                this.keyword = _keyword;
            }
            if (queryStrings.TryGetValue("articleCategoryId", out var _articleCategorySelected))
            {
                if (Int32.TryParse(_articleCategorySelected, out int res))
                {
                    this.articleCategorySelected = res;
                }
            }
            if (queryStrings.TryGetValue("articleStatusId", out var _articleStatusId))
            {
                if (Int32.TryParse(_articleStatusId, out int res))
                {
                    this.articleStatusSelected = res;
                    subTitle = lstArticleStatus.Where(x => x.Id == articleStatusSelected).First()?.Name;
                    StateHasChanged();
                }
            }
            if (queryStrings.TryGetValue("p", out var _p))
            {
                this.currentPage = Convert.ToInt32(_p);
                this.p = Convert.ToInt32(_p);
            }
        }
        private void OnChangeArticleStatus(int artStatusId)
        {
            subTitle = lstArticleStatus.Where(x => x.Id == artStatusId).First()?.Name;
            StateHasChanged();
        }
        #endregion Event
    }
}