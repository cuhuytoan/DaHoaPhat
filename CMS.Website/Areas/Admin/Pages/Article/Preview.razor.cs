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
        public int editCommentId { get; set; }
        //List ArticleComment 
        List<SpArticleCommentStaffSearchResult> lstArticleComment { get; set; }
        ////List FileAttach binding
        List<ArticleAttachFile> lstAttachFileBinding { get; set; } = new List<ArticleAttachFile>();   
        //Noti Hub
        [CascadingParameter]
        protected HubConnection hubConnection { get; set; }
        [CascadingParameter]
        private GlobalModel globalModel { get; set; }
        string outMessage = "";
      
        #endregion

        #region LifeCycle      
        protected override void OnInitialized()
        {
          
            NavigationManager.LocationChanged += HandleLocationChanged;
        }
        protected override async Task OnInitializedAsync()
        {
           
            await InitControl();
            await InitData();
        }

        public void Dispose()
        {           
            NavigationManager.LocationChanged -= HandleLocationChanged;           
        }
        #endregion

        #region Init
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        protected async Task InitControl()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
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
                if (!Repository.Permission.CanViewArticle(globalModel.user, globalModel.userId, (int)articleId, ref outMessage))
                {
                    NavigationManager.NavigateTo("/Admin/Error403");                    
                }
                var result = await Repository.Article.ArticleGetById((int)articleId);
                if (result != null)
                {
                    article = Mapper.Map<ArticleDTO>(result);
                }
                lstAttachFileBinding = await Repository.Article.ArticleAttachGetLstByArticleId((int)articleId);
                ArticleCommentStaffSearchFilter model = new ArticleCommentStaffSearchFilter();
                model.Keyword = "";
                model.ArticleId = (int)articleId;
                model.Active = true;
                model.CreateBy = null;
                model.PageSize = 100;
                model.CurrentPage = 1;

                var lstResult = await Repository.ArticleCommentStaff.ArticleCommentStaffSearch(model);
                if (lstResult != null)
                {
                    lstArticleComment = lstResult.Items;
                }
               
            }
        }
        #endregion

        #region Event
        async Task OnPostComment()
        {
            if (!Repository.Permission.CanCommentArticle(globalModel.user, globalModel.userId,(int)articleId, ref outMessage))
            {
                toastService.ShowToast(ToastLevel.Error, outMessage, "Thông báo");
            }
            ArticleCommentStaff item = new ArticleCommentStaff();
            item.Id = editCommentId;
            item.ArticleId = articleId;
            item.Content = comment;
            item.Name = globalModel.user.Identity.Name;
            item.CreateBy = globalModel.userId;
            item.CreateDate = DateTime.Now;
            item.Email = UserManager.GetUserName(globalModel.user);
            item.Active = true;


            try
            {
                await Repository.ArticleCommentStaff.ArticleCommentStaffPostComment(item);
                await InitData();
                comment = "";
                editCommentId = 0;
                StateHasChanged();

                toastService.ShowToast(ToastLevel.Success, "Bạn dã gửi bình luận thành công", "Thành công");


            }
            catch(Exception ex)
            {
                toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình gửi bình luận {ex}", "Thất bại");
            }

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
            if (queryStrings.TryGetValue("articleId", out var _articleId))
            {
                this.articleId = Convert.ToInt32(_articleId);
            }
        }

        protected async Task OnPostDemand(int postType)
        {
           
            try
            {
                await Repository.Article.ArticleUpdateStatusType(globalModel.userId,(int)articleId, postType);

                toastService.ShowToast(ToastLevel.Success, "Sơ duyệt thành công", "Thông báo");
                //Noti for user
                await hubConnection.SendAsync("SendNotification", article.CreateBy, "Sơ duyệt thành công", $"Ban biên tập đã sơ duyệt thành công bài viết của bạn", $"/Admin/Article/Preview?articleId={article.Id}", article.Image);
            }
            catch 
            {
                toastService.ShowToast(ToastLevel.Warning, "Có lỗi trong quá trình thực thi", "Lỗi!");
            }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            InitData();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            StateHasChanged();


        }

        protected void OnEditComment(int commentId,string content)
        {
            if(Repository.Permission.CanEditCommentArticle(globalModel.user,globalModel.userId,commentId, ref outMessage))
            {
                comment = content;
                editCommentId = commentId;
                StateHasChanged();
            }    
        }

    }
    #endregion
}
