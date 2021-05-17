using AutoMapper;
using Blazored.Toast.Services;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.Article
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

        public int? articleId { get; set; }
        public ArticleDTO article { get; set; } = new ArticleDTO();
        public int ArticleStatusId { get; set; } = 0;
        public string articleTypeName { get; set; }
        [Required(ErrorMessage = "Nhập bình luận")]
        [MinLength(50, ErrorMessage = "Bình luận tối thiểu 50 kí tự")]
        public string comment { get; set; }
        //List ArticleComment 
        List<SpArticleCommentSearchResult> lstArticleComment { get; set; }
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
            NavigationManager.LocationChanged += HandleLocationChanged;
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
            NavigationManager.LocationChanged -= HandleLocationChanged;
            _ = hubConnection.DisposeAsync();
        }
        #endregion

        #region Init
        protected async Task InitControl()
        {


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
                ArticleCommentSearchFilter model = new ArticleCommentSearchFilter();
                model.Keyword = "";
                model.ArticleId = (int)articleId;
                model.Active = true;
                model.CreateBy = null;
                model.PageSize = 100;
                model.CurrentPage = 1;

                var lstResult = await Repository.ArticleComment.ArticleCommentSearch(model);
                if (lstResult != null)
                {
                    lstArticleComment = lstResult.Items;
                }
                //Update ArticleTypeName
                if (article.ArticleContentTypeId != null)
                {
                    if (article.ArticleContentTypeId == 1)
                    {
                        articleTypeName = "LUẬT CÔNG";
                    }
                    else if (article.ArticleContentTypeId == 2)
                    {
                        articleTypeName = "LUẬT TƯ";
                    }
                }
            }
        }
        #endregion

        #region Event
        async Task OnPostComment()
        {
            ArticleComment item = new ArticleComment();
            item.ArticleId = articleId;
            item.Content = comment;
            item.Name = user.Identity.Name;
            item.CreateBy = UserManager.GetUserId(user);
            item.CreateDate = DateTime.Now;
            item.Email = UserManager.GetUserName(user);
            item.Active = true;


            try
            {
                await Repository.ArticleComment.ArticleCommentPostComment(item);
                await InitData();
                comment = "";
                StateHasChanged();

                toastService.ShowToast(ToastLevel.Success, "Bạn dã gửi bình luận thành công", "Thành công");


            }
            catch (Exception ex)
            {
                toastService.ShowToast(ToastLevel.Error, "Có lỗi trong quá trình gửi bình luận", "Thất bại");
            }

        }

        async Task OnChangeArticleType(int articleContentTypeId)
        {
            var result = await Repository.Article.ArticleChangeArticleType((int)articleId, articleContentTypeId);
            if (result)
            {
                toastService.ShowSuccess("Cập nhật thành công", "Thông báo");
            }
            else
            {
                toastService.ShowError("Có lỗi xảy ra trong quá trình cập nhật", "Thông báo");
            }
            //Update ArticleTypeName

            if (articleContentTypeId == 1)
            {
                articleTypeName = "LUẬT CÔNG";
            }
            else if (articleContentTypeId == 2)
            {
                articleTypeName = "LUẬT TƯ";
            }

            StateHasChanged();
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
            if (queryStrings.TryGetValue("articleId", out var _articleId))
            {
                this.articleId = Convert.ToInt32(_articleId);
            }
        }

        protected async Task OnPostDemand(int postType)
        {

            try
            {
                await Repository.Article.ArticleUpdateStatusType((int)articleId, postType);

                toastService.ShowToast(ToastLevel.Success, "Sơ duyệt thành công", "Thông báo");
                //Noti for user
                await hubConnection.SendAsync("SendNotification", article.CreateBy, "Sơ duyệt thành công", $"Ban biên tập đã sơ duyệt thành công bài viết của bạn", $"/Admin/Article/Preview?articleId={article.Id}", article.Image);
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
