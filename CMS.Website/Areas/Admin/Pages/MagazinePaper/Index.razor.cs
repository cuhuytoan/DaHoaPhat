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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Website.Areas.Admin.Pages.MagazinePaper
{
    public partial class Index : IDisposable
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
        public int? articleRevStatusId { get; set; }
        [Parameter]
        public int? p { get; set; }
        #endregion

        #region Model
        private List<SpMagazinePaperSearchResult> lstMagazinePaper;
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

            var modelFilter = new MagazinePaperSearchFilter();
            modelFilter.MagazinePaperStatusId = articleRevStatusId;
            modelFilter.Keyword = keyword;
            modelFilter.CurrentPage = p ?? 1;
            modelFilter.PageSize = 30;

            var result = await Repository.MagazinePaper.MagazinePaperSearch(modelFilter);

            lstMagazinePaper = result.Items;
            totalCount = result.TotalSize;

            //Init Selected 

            StateHasChanged();
        }

        #endregion


        #region Event




        protected async Task DeleteMagazinePaper(int? MagazinePaperId)
        {
            listArticleSelected.Clear();
            listArticleSelected.Add((int)MagazinePaperId);

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
                        var result = await Repository.MagazinePaper.MagazinePaperDelete(item);

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
            InitData();
            StateHasChanged();

        }
        protected void GetQueryStringValues()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryStrings = QueryHelpers.ParseQuery(uri.Query);
            if (queryStrings.TryGetValue("keyword", out var _keyword))
            {
                this.keyword = _keyword;
            }
            if (queryStrings.TryGetValue("articleRevStatusId", out var _articleRevStatusId))
            {
                if (Int32.TryParse(_articleRevStatusId, out int _id))
                {
                    this.articleRevStatusId = _id;
                }
            }
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
                        await Repository.MagazinePaper.MagazinePaperUpdateStatus(item, postType);
                    }

                    toastService.ShowToast(ToastLevel.Success, "Cập nhật trạng thái thành công", "Thành công!");
                }
                catch (Exception ex)
                {
                    toastService.ShowToast(ToastLevel.Warning, "Có lỗi trong quá trình thực thi", "Lỗi!");
                }
                InitData();
                StateHasChanged();


            }
        }
        protected void OnCheckBoxChange(bool headerChecked, int ArticleId, object isChecked)
        {
            if (headerChecked)
            {
                if ((bool)isChecked)
                {
                    listArticleSelected.AddRange(lstMagazinePaper.Select(x => x.Id));
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
        #endregion
    }
}
