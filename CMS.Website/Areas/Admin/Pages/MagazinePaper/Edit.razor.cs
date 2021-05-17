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

namespace CMS.Website.Areas.Admin.Pages.MagazinePaper
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
        public int? magazinePaperId { get; set; }
        public PostMagazinePaperDTO magazinePaperModel { get; set; } = new PostMagazinePaperDTO();
        public MagazinePaperDTO magazinePaper { get; set; } = new MagazinePaperDTO();
        public List<SpArticleSearchResult> lstMagazinePaperArticle { get; set; } = new List<SpArticleSearchResult>();

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
            if (queryStrings.TryGetValue("magazinePaperId", out var _MagazinePaperId))
            {
                this.magazinePaperId = Convert.ToInt32(_MagazinePaperId);
            }

            if (magazinePaperId != null)
            {
                var result = await Repository.MagazinePaper.MagazinePaperGetById((int)magazinePaperId);
                if (result != null)
                {
                    magazinePaperModel.MagazinePaper = Mapper.Map<MagazinePaperDTO>(result);
                    magazinePaperModel.MagazinePaper.Id = (int)magazinePaperId;

                }
                lstArticleSelected = await Repository.MagazinePaper.MagazinePaperArticleGetLstById((int)magazinePaperId);
            }
            //Init List Article
            var modelFilter = new ArticleSearchFilter();
            modelFilter.Keyword = "";
            //modelFilter.ArticleCategoryId = articleCategorySelected;
            modelFilter.CurrentPage = 1;
            modelFilter.PageSize = 100;
            modelFilter.ArticleStatusId = 6; // Bài viết duyệt đăng tạp chí giấy
            modelFilter.FromDate = DateTime.Now.AddYears(-10);
            modelFilter.ToDate = DateTime.Now;
            var resultArticle = await Repository.Article.ArticleSearchWithPaging(modelFilter);
            if (resultArticle != null)
            {
                lstArticle = resultArticle.Items;
            }
        }
        #endregion

        #region Event

        private async Task OnPostMagazinePaper()
        {
            if (lstArticleSelected == null || lstArticleSelected.Count == 0)
            {
                toastService.ShowWarning("Danh sách bài viết số báo không được trống", "Thông báo");
                return;
            }
            magazinePaperModel.LstArticleId = lstArticleSelected.Select(x => x.Id).ToList();

            var result = await Repository.MagazinePaper.MagazinePaperPost(magazinePaperModel);
            if (result > 0)
            {
                toastService.ShowSuccess("Tạo mới số báo thành công", "Thông báo");
                NavigationManager.NavigateTo("/Admin/magazinePaper/Index");
            }
            else
            {
                toastService.ShowWarning("Có lỗi trong quá trình thêm mới", "Thông báo");
            }
        }

        async Task OnAddMagazinePaperArticle(SpArticleSearchResult item)
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
        async Task OnRemmoveMagazinePaperArticle(SpArticleSearchResult item)
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
