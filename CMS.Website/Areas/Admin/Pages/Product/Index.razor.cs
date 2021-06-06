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

namespace CMS.Website.Areas.Admin.Pages.Product
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
        public int? productCategoryId { get; set; }

        [Parameter]
        public int? p { get; set; }

        #endregion Parameter

        #region Model

        private List<SpProductSearchResult> lstProduct;
        public int currentPage { get; set; }
        public int totalCount { get; set; }
        public int pageSize { get; set; } = 30;
        public int totalPages => (int)Math.Ceiling(decimal.Divide(totalCount, pageSize));
        public ProductSearchFilter modelFilter { get; set; }
        public int? productCategorySelected { get; set; }
        public int? productStatusSelected { get; set; }
        public int? setProductStatusSelected { get; set; }
        private List<ProductCategory> lstProductCategory { get; set; }
        private List<ProductStatus> lstProductStatus { get; set; }
        private string subTitle { get; set; } = "bài viết đã cập nhật";
        public string outMessage = "";

        [CascadingParameter]
        protected GlobalModel globalModel { get; set; }

        protected ConfirmBase DeleteConfirmation { get; set; }
        protected PropertiesDynamic PropertiesDynamicEdit { get; set; }
        private List<int> listProductSelected { get; set; } = new List<int>();
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
            if (globalModel.user.IsInRole("Phụ trách chuyên mục"))
            {
                var lstProductCate = await Repository.ProductCategory.GetProductCategoryByUserId(globalModel.userId);
                if (lstProductCate != null)
                {
                    lstProductCategory = lstProductCate.Select(x => new ProductCategory { Id = x.Id, Name = x.Name }).ToList();
                }
                //Binding Status
                var lstStatus = await Repository.Product.GetLstProductStatusByUserId(globalModel.userId);
                if (lstStatus != null)
                {
                    lstProductStatus = lstStatus.Select(x => new ProductStatus { Id = x.Id, Name = x.Name }).ToList();
                }
            }

            else
            {
                var lstProductCate = await Repository.ProductCategory.GetProductCategoryById(null);
                if (lstProductCate != null)
                {
                    lstProductCategory = lstProductCate.Select(x => new ProductCategory { Id = x.Id, Name = x.Name }).ToList();
                }
                //Binding Status
                var lstStatus = await Repository.Product.GetLstProductStatus();
                if (lstStatus != null)
                {
                    lstProductStatus = lstStatus.Select(x => new ProductStatus { Id = x.Id, Name = x.Name }).ToList();
                }
            }


        }

        protected async Task InitData()
        {
            Logger.LogDebug("Init");
            var modelFilter = new ProductSearchFilter();
            modelFilter.Keyword = keyword;
            modelFilter.ProductCategoryId = productCategorySelected;
            modelFilter.CurrentPage = p ?? 1;
            modelFilter.PageSize = 30;
            modelFilter.ProductStatusId = productStatusSelected;
            modelFilter.FromDate = DateTime.Now.AddYears(-10);
            modelFilter.ToDate = DateTime.Now;
            if (globalModel.user.IsInRole("Cộng tác viên"))
            {
                modelFilter.CreateBy = globalModel.userId;
            }
            if (globalModel.user.IsInRole("Phụ trách chuyên mục"))
            {
                modelFilter.AssignBy = globalModel.userId;
            }
            var result = await Repository.Product.ProductSearchWithPaging(modelFilter);

            lstProduct = result.Items;
            totalCount = result.TotalSize;

            //Init Selected
            listProductSelected.Clear();
            StateHasChanged();
        }

        #endregion Init

        #region Event

        protected async Task OnPostDemand(int postType)
        {
            if (listProductSelected.Count == 0)
            {
                toastService.ShowToast(ToastLevel.Warning, "Chưa chọn bài viết thực thi", "Thông báo");
                return;
            }
            else
            {
                try
                {
                    foreach (var item in listProductSelected)
                    {
                        await Repository.Product.ProductUpdateStatusType(globalModel.userId, item, postType);
                    }

                    toastService.ShowToast(ToastLevel.Success, "Cập nhật thành công", "Thành công!");

                }
                catch (Exception ex)
                {
                    toastService.ShowToast(ToastLevel.Warning, $"Có lỗi trong quá trình thực thi {ex.ToString()}", "Lỗi!");
                }
                _forceRerender = true;
                StateHasChanged();
                await InitData();
            }
        }

        protected void DeleteProduct(int? productId)
        {

            if (productId == null) // Delete Demand
            {
                if (listProductSelected.Count == 0)
                {
                    toastService.ShowToast(ToastLevel.Warning, "Chưa chọn bài viết để xóa", "Thông báo");
                    return;
                }
            }
            else
            {
                listProductSelected.Clear();
                listProductSelected.Add((int)productId);
                if (!Repository.Permission.CanDeleteProduct(globalModel.user, globalModel.userId, (int)productId, ref outMessage))
                {
                    toastService.ShowError(outMessage, "Thông báo");
                    return;
                }

            }

            DeleteConfirmation.Show();
        }
        protected void PropertiesProdShow(int productId)
        {
            PropertiesDynamicEdit.ProductId = productId;
            PropertiesDynamicEdit.Show();
        }
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                try
                {
                    foreach (var item in listProductSelected)
                    {
                        await Repository.Product.ProductDelete(item);
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

        protected void OnCheckBoxChange(bool headerChecked, int ProductId, object isChecked)
        {
            if (headerChecked)
            {
                if ((bool)isChecked)
                {
                    listProductSelected.AddRange(lstProduct.Select(x => x.Id));
                    isCheck = true;
                }
                else
                {
                    isCheck = false;
                    listProductSelected.Clear();
                }
            }
            else
            {
                if ((bool)isChecked)
                {
                    if (!listProductSelected.Contains(ProductId))
                    {
                        listProductSelected.Add(ProductId);
                    }
                }
                else
                {
                    if (listProductSelected.Contains(ProductId))
                    {
                        listProductSelected.Remove(ProductId);
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
            if (queryStrings.TryGetValue("productCategoryId", out var _productCategorySelected))
            {
                if (Int32.TryParse(_productCategorySelected, out int res))
                {
                    this.productCategorySelected = res;
                }
            }
            if (queryStrings.TryGetValue("productStatusId", out var _productStatusId))
            {
                if (Int32.TryParse(_productStatusId, out int res))
                {
                    this.productStatusSelected = res;
                    subTitle = lstProductStatus.Where(x => x.Id == productStatusSelected).First()?.Name;
                    StateHasChanged();
                }
            }
            if (queryStrings.TryGetValue("p", out var _p))
            {
                this.currentPage = Convert.ToInt32(_p);
                this.p = Convert.ToInt32(_p);
            }
        }
        private void OnChangeProductStatus(int artStatusId)
        {
            subTitle = lstProductStatus.Where(x => x.Id == artStatusId).First()?.Name;
            StateHasChanged();
        }
        #endregion Event
    }
}
