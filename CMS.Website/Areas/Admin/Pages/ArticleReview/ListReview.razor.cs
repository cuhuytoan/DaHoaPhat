using AutoMapper;
using Blazored.Toast.Services;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.ArticleReview
{
    public partial class ListReview : IDisposable
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
        public int? articleInReviewStatusId { get; set; }
        [Parameter]
        public int? p { get; set; }
        public string UserInReviewId { get; set; }
        #endregion

        #region Model
        private List<SpArticleInReviewSearchResult> lstArticleReview;
        public int currentPage { get; set; }
        public int totalCount { get; set; }
        public int pageSize { get; set; } = 30;
        public int totalPages => (int)Math.Ceiling(decimal.Divide(totalCount, pageSize));
        public ArticleInReviewSearchFilter modelFilter { get; set; }


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
            if (user.IsInRole("Hội đồng phản biện"))
            {
                this.UserInReviewId = UserManager.GetUserId(user);
            }
            var modelFilter = new ArticleInReviewSearchFilter();
            modelFilter.Keyword = keyword;
            modelFilter.ArticleInReviewStatusId = articleInReviewStatusId;
            modelFilter.UserInReviewId = UserInReviewId;
            modelFilter.CurrentPage = p ?? 1;
            modelFilter.PageSize = 30;

            var result = await Repository.ArticleReview.ArticleInReviewSearch(modelFilter);

            lstArticleReview = result.Items;
            totalCount = result.TotalSize;

            //Init Selected 

            StateHasChanged();
        }

        #endregion


        #region Event

        protected async Task OnPostDemand(int postType)
        {

        }


        protected async Task DeleteArticleReview(int? articleReviewId)
        {
            listArticleSelected.Clear();
            listArticleSelected.Add((int)articleReviewId);

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
                        var result = await Repository.ArticleReview.ArticleReviewDelete(item);

                    }
                    toastService.ShowToast(ToastLevel.Success, "Xóa bài viết thành công", "Thành công");
                }
                catch (Exception ex)
                {
                    toastService.ShowToast(ToastLevel.Warning, "Có lỗi trong quá trình thực thi", "Lỗi!");
                }
                StateHasChanged();
                await InitData();
            }
        }


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
            if (queryStrings.TryGetValue("articleInReviewStatusId", out var _articleInReviewStatusId))
            {

                if (Int32.TryParse(_articleInReviewStatusId, out int _id))
                {
                    this.articleInReviewStatusId = _id;
                }

            }
            if (user.IsInRole("Hội đồng phản biện"))
            {
                this.UserInReviewId = UserManager.GetUserId(user);
            }
        }
        #endregion
    }
}
