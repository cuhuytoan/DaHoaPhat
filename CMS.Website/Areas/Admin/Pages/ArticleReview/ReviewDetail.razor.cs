using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.ArticleReview
{
    public partial class ReviewDetail : IDisposable
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

        public int? articleId { get; set; }
        public ArticleDTO article { get; set; } = new ArticleDTO();
        public int ArticleStatusId { get; set; } = 0;
        public int? SelectedValue { get; set; }
        List<ArticleReviewStatus> lstArticleReviewStatus { get; set; }
        public ArticleReviewDetailDTO articleReviewDetail { get; set; } = new ArticleReviewDetailDTO();
        //List ArticleReviewDetailSearchResult 
        List<SpArticleReviewDetailSearchResult> lstArticleInReview { get; set; }
        ////List FileAttach binding
        List<ArticleAttachFile> lstAttachFileBinding { get; set; } = new List<ArticleAttachFile>();
        //Noti Hub
        private HubConnection hubConnection;
        //For reload
        bool _forceRerender;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        ClaimsPrincipal user;
        public List<string> RemoveAttributes { get; set; } = new List<string>() { "data-id" };
        public List<string> StripTags { get; set; } = new List<string>() { "font" };
        #endregion

        #region LifeCycle
        protected override async Task OnParametersSetAsync()
        {


        }
        protected override void OnInitialized()
        {

        }
        protected override async Task OnInitializedAsync()
        {
            //get claim principal
            var authState = await authenticationStateTask;
            user = authState.User;
            //Init Hub
            hubConnection = new HubConnectionBuilder()
              .WithUrl(NavigationManager.ToAbsoluteUri("/notificationHubs"))
              .Build();

            await hubConnection.StartAsync();
            //
            await InitControl();
            await InitData();
            StateHasChanged();

        }

        public void Dispose()
        {
            //GC.SuppressFinalize(this);
            _ = hubConnection.DisposeAsync();
        }
        #endregion

        #region Init
        protected async Task InitControl()
        {
            var lstArticleStt = await Repository.ArticleReview.ArticleReviewStatusGetLst();
            if (lstArticleStt != null)
            {
                lstArticleReviewStatus = lstArticleStt.Select(x => new ArticleReviewStatus { Id = x.Id, Name = x.Name }).ToList();
            }

        }
        protected async Task InitData()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryStrings = QueryHelpers.ParseQuery(uri.Query);
            if (queryStrings.TryGetValue("articleId", out var _articleId))
            {
                this.articleId = Convert.ToInt32(_articleId);
            }

            if (articleId != null)
            {
                var result = await Repository.Article.ArticleGetById((int)articleId);
                if (result != null)
                {
                    article = Mapper.Map<ArticleDTO>(result);
                }
                lstAttachFileBinding = await Repository.Article.ArticleAttachGetLstByArticleId((int)articleId);
                ArticleReviewDetailSearchFilter model = new ArticleReviewDetailSearchFilter();
                model.Keyword = "";
                model.ArticleId = (int)articleId;
                model.CreateBy = null;
                model.PageSize = 100;
                model.CurrentPage = 1;

                var lstResult = await Repository.ArticleReview.ArticleReviewDetailSearch(model);
                if (lstResult != null)
                {
                    lstArticleInReview = lstResult.Items;
                }
            }
        }
        #endregion

        #region Event
        async Task OnPostReviewDetail()
        {
            try
            {
                if (articleReviewDetail.Id == 0)
                {
                    articleReviewDetail.CreateBy = UserManager.GetUserId(user);
                    articleReviewDetail.CreateDate = DateTime.Now;
                }

                articleReviewDetail.LastEditBy = UserManager.GetUserId(user);
                articleReviewDetail.LastEditDate = DateTime.Now;
                articleReviewDetail.ArticleReviewStatusId = SelectedValue;
                articleReviewDetail.ArticleId = articleId;
                await Repository.ArticleReview.PostReviewDetail(Mapper.Map<ArticleReviewDetail>(articleReviewDetail));
                await InitData();
                articleReviewDetail = new ArticleReviewDetailDTO();
                StateHasChanged();

                toastService.ShowToast(ToastLevel.Success, "Bạn dã gửi phản biện thành công", "Thành công");
            }
            catch (Exception ex)
            {
                toastService.ShowToast(ToastLevel.Error, "Có lỗi trong quá trình gửi phản biện", "Thất bại");
            }

        }
        async Task OnEditReviewDetail(int articleReviewDetailId)
        {
            var item = await Repository.ArticleReview.ArticleReviewDetailGetById(articleReviewDetailId);
            if (item != null)
            {
                articleReviewDetail = Mapper.Map<ArticleReviewDetailDTO>(item);
                InitData();
                StateHasChanged();
            }

        }
        #endregion
    }
}
