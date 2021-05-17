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
    public partial class Edit : IDisposable
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
        public string keyword { get; set; }
        public int? articleReviewId { get; set; }
        public PostArticleReviewDTO articleReviewModel { get; set; } = new PostArticleReviewDTO();
        public ArticleReviewDTO articleReview { get; set; } = new ArticleReviewDTO();
        public List<SpArticleSearchResult> lstArticleReviewArticle { get; set; } = new List<SpArticleSearchResult>();
        public List<ArticleReviewPersonDTO> lstArticleReviewPerson { get; set; } = new List<ArticleReviewPersonDTO>();
        public List<AspNetUsers> lstReViewPerson { get; set; } = new List<AspNetUsers>();
        public List<string> lstAccountSearchResult { get; set; } = new List<string>();
        public string selectedUser { get; set; }

        private List<SpArticleSearchResult> lstArticle;
        private List<SpArticleSearchResult> lstArticleSelected = new List<SpArticleSearchResult>();


        private HubConnection hubConnection;
        //For reload
        bool _forceRerender;
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        ClaimsPrincipal user;

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
            hubConnection.On<string, string>("ReceiveMessage", (userid, message) =>
            {
                if (UserManager.GetUserId(user) == userid)
                {
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Info, $"{message}", "Bạn có thông báo mới");
                    StateHasChanged();
                    InitData();
                }
            });

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



        }
        protected async Task InitData()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryStrings = QueryHelpers.ParseQuery(uri.Query);
            if (queryStrings.TryGetValue("articleReviewId", out var _articleReviewId))
            {
                this.articleReviewId = Convert.ToInt32(_articleReviewId);
            }

            if (articleReviewId != null)
            {
                var result = await Repository.ArticleReview.ArticleReviewGetById((int)articleReviewId);
                if (result != null)
                {
                    articleReviewModel.ArticleReview = Mapper.Map<ArticleReviewDTO>(result);
                    articleReviewModel.ArticleReview.Id = (int)articleReviewId;

                }
                lstArticleReviewPerson = await Repository.ArticleReview.ArticleReviewPersonGetLstById((int)articleReviewId);
                lstArticleSelected = await Repository.ArticleReview.ArticleReviewArticleGetLstById((int)articleReviewId);
            }
            //Init List Article
            var modelFilter = new ArticleSearchFilter();
            modelFilter.Keyword = "";
            //modelFilter.ArticleCategoryId = articleCategorySelected;
            modelFilter.CurrentPage = 1;
            modelFilter.PageSize = 100;
            modelFilter.ArticleStatusId = 2; // Bài viết đã sơ duyệt
            modelFilter.FromDate = DateTime.Now.AddYears(-10);
            modelFilter.ToDate = DateTime.Now;
            var resultArticle = await Repository.Article.ArticleSearchWithPaging(modelFilter);
            if (resultArticle != null)
            {
                lstArticle = resultArticle.Items;
            }

            //Init Person in Review
            var modelFilterAcc = new AccountSearchFilter();
            modelFilterAcc.PageSize = 100;
            modelFilterAcc.CurrentPage = 1;
            modelFilterAcc.RoleId = Guid.Parse("AF5B6A2D-9473-4403-BDBD-F4BEDC7EE519"); //Phản biện
            var resultAcc = await Repository.AspNetUsers.GetLstUsersPaging(modelFilterAcc);
            if (resultAcc != null)
            {
                lstAccountSearchResult = resultAcc.Items.Select(x => x.UserName).ToList();
            }

        }
        #endregion

        #region Event

        private async Task OnPostArticleReview()
        {
            if (lstArticleSelected == null || lstArticleSelected.Count == 0)
            {
                toastService.ShowWarning("Danh sách bài viết phản biện không được trống", "Thông báo");
                return;
            }
            if (lstArticleReviewPerson == null || lstArticleReviewPerson.Count == 0)
            {
                toastService.ShowWarning("Danh sách thành viên phản biện không được trống", "Thông báo");
                return;
            }
            articleReviewModel.LstArticleId = lstArticleSelected.Select(x => x.Id).ToList();
            articleReviewModel.LstReviewPerson = lstArticleReviewPerson.Select(x => x.UserId).ToList();

            var result = await Repository.ArticleReview.ArticleReviewPost(articleReviewModel);
            if (result > 0)
            {
                toastService.ShowSuccess("Tạo mới phản biện thành công", "Thông báo");
                NavigationManager.NavigateTo("/Admin/ArticleReview/Index");
            }
            else
            {
                toastService.ShowWarning("Có lỗi trong quá trình thêm mới", "Thông báo");
            }
        }

        async Task OnAddArticleReviewPerson()
        {
            if (selectedUser == null)
            {
                toastService.ShowWarning("Thành viên phản biện không hợp lệ", "Thông báo");
                return;
            }
            else
            {

                var itemPerson = await UserManager.FindByNameAsync(selectedUser);
                if (itemPerson != null)
                {
                    var profile = await Repository.AspNetUsers.GetAccountInfoByUserId(itemPerson.Id);
                    ArticleReviewPersonDTO item = new ArticleReviewPersonDTO();
                    item.UserId = profile.AspNetUserProfiles.UserId;
                    item.FullName = profile.AspNetUserProfiles.FullName;
                    item.AvatarUrl = profile.AspNetUserProfiles.AvatarUrl;
                    item.Email = profile.AspNetUsers.UserName;
                    if (lstArticleReviewPerson.Contains(item))
                    {
                        toastService.ShowWarning("Thành viên phản biện đã tồn tại trong danh sách", "Thông báo");
                        return;
                    }
                    lstArticleReviewPerson.Add(item);
                    toastService.ShowSuccess("Thêm thành viên vào danh sách phản biện thành công", "Thông báo");
                    StateHasChanged();
                }
            }
        }
        async Task OnRemoveArticleReviewPerson(ArticleReviewPersonDTO item)
        {
            if (item == null)
            {
                return;
            }
            else
            {
                if (!lstArticleReviewPerson.Contains(item))
                {
                    toastService.ShowWarning($"Không tồn tại thành viên", "Thông báo");
                    return;
                }
                lstArticleReviewPerson.Remove(item);
                StateHasChanged();
            }
        }
        async Task OnAddArticleReviewArticle(SpArticleSearchResult item)
        {
            if (item == null)
            {
                return;
            }
            else
            {
                if (lstArticleSelected.Contains(item))
                {
                    toastService.ShowWarning("Bài viết đã tồn tại trong danh sách", "Thông báo");
                    return;
                }
                lstArticleSelected.Add(item);
                StateHasChanged();
            }
        }
        async Task OnRemmoveArticleReviewArticle(SpArticleSearchResult item)
        {
            if (item == null)
            {
                return;
            }
            else
            {
                if (!lstArticleSelected.Contains(item))
                {
                    toastService.ShowWarning("Bài viết không tồn tại", "Thông báo");
                    return;
                }
                lstArticleSelected.Remove(item);
                StateHasChanged();
            }
        }
        async Task OnSearchArticle(string keyword)
        {
            //Init List Article
            var modelFilter = new ArticleSearchFilter();
            modelFilter.Keyword = keyword;
            //modelFilter.ArticleCategoryId = articleCategorySelected;
            modelFilter.CurrentPage = 1;
            modelFilter.PageSize = 100;
            modelFilter.ArticleStatusId = 2; // Bài viết đã sơ duyệt
            modelFilter.FromDate = DateTime.Now.AddYears(-10);
            modelFilter.ToDate = DateTime.Now;
            var resultArticle = await Repository.Article.ArticleSearchWithPaging(modelFilter);
            if (resultArticle != null)
            {
                lstArticle = resultArticle.Items;
            }
            StateHasChanged();
        }
        #endregion
    }

}
