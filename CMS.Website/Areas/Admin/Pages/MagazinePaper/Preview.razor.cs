using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Website.Areas.Admin.Pages.Shared;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.MagazinePaper
{
    public partial class Preview : IDisposable
    {
        #region Inject   
        [Inject]
        IMapper Mapper { get; set; }
        [Inject]
        ILoggerManager Logger { get; set; }
        [Inject]
        UserManager<IdentityUser> UserManager { get; set; }
        #endregion


        #region Parameter
        [Parameter]
        public string keyword { get; set; }
        [Parameter]
        public int? articleCategoryId { get; set; }
        [Parameter]
        public int? p { get; set; }
        public string UserInReviewId { get; set; }
        #endregion

        #region Model
        public int? MagazinePaperId { get; set; }
        private List<SpArticleInMagazinePaperSearchResult> lstArticle { get; set; } = new List<SpArticleInMagazinePaperSearchResult>();
        private MagazinePaperDTO MagazinePaperItem { get; set; } = new MagazinePaperDTO();
        public int currentPage { get; set; }
        public int totalCount { get; set; }
        public int pageSize { get; set; } = 30;
        public int totalPages => (int)Math.Ceiling(decimal.Divide(totalCount, pageSize));
        public MagazinePaperSearchFilter modelFilter { get; set; }


        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        ClaimsPrincipal user;
        protected ConfirmBase DeleteConfirmation { get; set; }
        List<int> listArticleSelected { get; set; } = new List<int>();
        bool _forceRerender;
        bool isCheck { get; set; }
        #endregion

        #region LifeCycle

        protected override async Task OnParametersSetAsync()
        {

        }
        protected override void OnInitialized()
        {
            //Add for change location and seach not reload page
            NavigationManager.LocationChanged += HandleLocationChanged;
        }

        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateTask;
            user = authState.User;
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

        #endregion


        #region Init
        protected async Task InitControl()
        {

        }
        protected async Task InitData()
        {
            GetQueryStringValues();
            //Init MagazinePaperItem
            var itemArt = await Repository.MagazinePaper.MagazinePaperGetById((int)MagazinePaperId);
            if (itemArt != null)
            {
                MagazinePaperItem = Mapper.Map<MagazinePaperDTO>(itemArt);
            }
            var modelFilter = new ArticleInMagazinePaperSearchFilter();
            modelFilter.MagazinePaperId = MagazinePaperId;
            modelFilter.Keyword = keyword;
            modelFilter.CurrentPage = p ?? 1;
            modelFilter.PageSize = 30;

            var result = await Repository.MagazinePaper.ArticleInMagazinePaperSearch(modelFilter);

            lstArticle = result.Items;
            totalCount = result.TotalSize;

            //Init Selected 

            StateHasChanged();
        }

        #endregion


        #region Event


        protected void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            GetQueryStringValues();
            StateHasChanged();
            InitData();
        }
        protected void GetQueryStringValues()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryStrings = QueryHelpers.ParseQuery(uri.Query);
            if (queryStrings.TryGetValue("keyword", out var _keyword))
            {
                this.keyword = _keyword;
            }
            if (queryStrings.TryGetValue("MagazinePaperId", out var _MagazinePaperId))
            {
                if (Int32.TryParse(_MagazinePaperId, out int _artId))
                {
                    this.MagazinePaperId = _artId;
                }

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
                        await Repository.Article.ArticleUpdateStatusType(item, postType);
                    }
                    toastService.ShowToast(ToastLevel.Success, "Cập nhật trạng thái", "Thành công!");

                }
                catch (Exception ex)
                {
                    toastService.ShowToast(ToastLevel.Warning, "Có lỗi trong quá trình thực thi", "Lỗi!");
                }
                InitData();
                StateHasChanged();

            }
        }
        #endregion
    }
}
