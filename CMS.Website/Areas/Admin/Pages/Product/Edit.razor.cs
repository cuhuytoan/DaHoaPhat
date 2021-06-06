using AutoMapper;
using Blazored.Toast.Services;
using CMS.Common;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Website.Areas.Admin.Pages.Shared.Components;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;
using FontFamily = Telerik.Blazor.Components.Editor.FontFamily;

namespace CMS.Website.Areas.Admin.Pages.Product
{
    public partial class Edit : IDisposable
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

        public int? productId { get; set; }
        public ProductDTO product { get; set; } = new ProductDTO();
        public int ProductStatusId { get; set; } = 0;
        private List<ProductCategory> lstProductCategory { get; set; } = new List<ProductCategory>();
        private List<ProductType> lstProductType { get; set; } = new();
        private List<ProductManufacture> lstProductManufactures { get; set; } = new();
        private List<Country> lstCountry { get; set; } = new();
        private List<Unit> lstUnit { get; set; } = new();
        public string PreviewImage { get; set; }
        public int SelectedCateValue { get; set; }
        public List<string> SelectedCateName { get; set; } = new List<string>();
        private List<string> imageDataUrls = new();
        private List<string> lstSubImageData = new();
        private List<ProductPicture> lstProductPicture = new();
        public int postType { get; set; }
        public bool chkTopProductCategory { get; set; } = false;
        public bool chkTopProductCategoryParent { get; set; } = false;
        public IReadOnlyList<IBrowserFile> MainImages { get; set; }
        string outMessage = "";
        private bool isCropMainImage { get; set; }        

        // setup upload endpoints
        public string SaveUrl => ToAbsoluteUrl("api/upload/save");

        public string RemoveUrl => ToAbsoluteUrl("api/upload/remove");

        ////List FileAttach Add new
        private List<ProductAttachFile> lstAttachFile { get; set; } = new List<ProductAttachFile>();

        ////List FileAttach binding
        private List<ProductAttachFile> lstAttachFileBinding { get; set; } = new List<ProductAttachFile>();

        //Modal Crop Image
        protected ImageCropper imageCropperModal { get; set; }

        //Noti Hub
        [CascadingParameter]
        protected HubConnection hubConnection { get; set; }

        [CascadingParameter]
        private GlobalModel globalModel { get; set; }

        #endregion Parameter

        #region LifeCycle

        protected override async Task OnInitializedAsync()
        {
            if (!Repository.Permission.CanAddNewProduct(globalModel.user, globalModel.userId, ref outMessage))
            {
                NavigationManager.NavigateTo("/Admin/Error403");
            }
            await InitControl();
            await InitData();
            StateHasChanged();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion LifeCycle

        #region Init

        protected async Task InitControl()
        {
            //ProductCategory
            var lstProductCate = await Repository.ProductCategory.GetProductCategoryById(null);
            if (lstProductCate != null)
            {
                lstProductCategory = lstProductCate.Select(x => new ProductCategory { Id = x.Id, Name = x.Name }).ToList();
            }
            //ProductType
             lstProductType = await Repository.Product.ProductTypeGetLst();
             if(lstProductType !=null)
            {
                lstProductType = lstProductType.Select(x => new ProductType { Id = x.Id, Name = x.Name }).ToList();
            }
            //Product Manyfactures
            lstProductManufactures = await Repository.Product.ProductManufacturesGetLst();
            if(lstProductManufactures !=null)
            {
                lstProductManufactures = lstProductManufactures.Select(x => new ProductManufacture { Id = x.Id, Name = x.Name }).ToList();
            }
            //Country
            lstCountry = await Repository.MasterData.CountriesGetLst();
            if(lstCountry !=null)
            {
                lstCountry = lstCountry.Select(x => new Country { Id = x.Id, Name = x.Name }).ToList();
            }
            //Lst Unit
            lstUnit = await Repository.MasterData.UnitGetLst();
            if(lstUnit !=null)
            {
                lstUnit = lstUnit.Select(x => new Unit { Id = x.Id, Name = x.Name }).ToList();
            }    
        }   

        protected async Task InitData()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryStrings = QueryHelpers.ParseQuery(uri.Query);
            if (queryStrings.TryGetValue("productId", out var _productId))
            {
                this.productId = Convert.ToInt32(_productId);
            }

            if (productId != null)
            {
                var result = await Repository.Product.ProductGetById((int)productId);
                if (result != null)
                {
                    product = Mapper.Map<ProductDTO>(result);
                    //Get Lst ProductCategory
                    var lstProdCate = await Repository.ProductCategory.GetLstProductCatebyProductId((int)product.Id);
                    SelectedCateValue = lstProdCate.Select(x => x.ProductCategoryId).FirstOrDefault();
                    lstProductPicture = await Repository.ProductPicture.ProductPictureGetLstByProductId((int)product.Id);
                }
                //L
                lstAttachFileBinding = await Repository.Product.ProductAttachGetLstByProductId((int)productId);
            }
        }

        #endregion Init

        #region Event

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

        private void OnProductSelected()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            //SelectedCateName = lstProductCategory.Where(p => SelectedCateValue.Contains(p.Id)).Select(p => p.Name).ToList();
            //product.ProductCategoryIds = String.Join(",", SelectedCateValue.ToArray());
            product.ProductCategoryIds = SelectedCateValue.ToString();
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            product.Image = null;
            var imageFiles = e.GetMultipleFiles();
            MainImages = imageFiles;
            var format = "image/png";
            foreach (var item in imageFiles)
            {
                var resizedImageFile = await item.RequestImageFileAsync(format, 500, 500);
                var buffer = new byte[resizedImageFile.Size];
                await resizedImageFile.OpenReadStream().ReadAsync(buffer);
                var imageDataUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                //Clear Old Image
                imageDataUrls.Clear();
                imageDataUrls.Add(imageDataUrl);
            }
        }

        private async Task PostProduct()
        {
            if (postType == 1)
            {
                ProductStatusId = 1;
            }
            //            
            SelectedCateValue = Int32.Parse(product.ProductCategoryIds);
            List<int> prodCateId = new();
            prodCateId.Add(SelectedCateValue);
            //Create new
            if (product.Id == null || product.Id == 0)
            {
                //Check permission
                if (!Repository.Permission.CanAddNewProduct(globalModel.user, globalModel.userId, ref outMessage))
                {
                    if (!Repository.Permission.CanAddNewProduct(globalModel.user, globalModel.userId, ref outMessage))
                    {
                        NavigationManager.NavigateTo("/Admin/Error403");
                    }
                }
                product.EndDate = product.StartDate?.AddYears(100);
                int ProdCateID = Convert.ToInt32(product.ProductCategoryIds);
             
                product.Id = await Repository.Product.ProductInsert(Mapper.Map<CMS.Data.ModelEntity.Product>(product), globalModel.userId, ProductStatusId, prodCateId);
            }
            //Update
            if (product.Id != null && product.Id > 0)
            {
                //Check permission
                if (!Repository.Permission.CanEditProduct(globalModel.user, globalModel.userId, (int)product.Id, ref outMessage))
                {
                    NavigationManager.NavigateTo("/Admin/Error403");
                }
                try
                {   //Save Main Image
                    if (imageDataUrls != null && imageDataUrls.Count > 0)
                    {
                        product.Image = await SaveMainImage((int)product.Id, imageDataUrls);
                    }
                    //Save Sub Image
                    if(lstSubImageData !=null && lstSubImageData.Count > 0)
                    {
                        List<ProductPicture> lstProPic = await SaveSubImage((int)product.Id,globalModel.userId, lstSubImageData);
                        var result = await Repository.ProductPicture.ProductPictureInsert(lstProPic, globalModel.userId, (int)product.Id);
                        if(!result)
                        {
                            toastService.ShowError("Có lỗi trong quá trình cập nhật ảnh phụ", "Thông báo");
                        }    
                    }    
                    //change Content
                    if (product.Content !=null && CheckContentHasBase64(product.Content))
                    {
                        product.Content = UploadImgBase64Content(product.Url, $"data/product/upload/{globalModel.userId}/{DateTime.Now:yyyy-MM-dd}", product.Content);
                    }
                    //Save Upload File
                    if (lstAttachFile.Count > 0)
                    {
                        lstAttachFile.ForEach(x =>
                        {
                            x.ProductId = product.Id;
                            x.CreateDate = DateTime.Now;
                            x.LastEditDate = DateTime.Now;
                            x.CreateBy = globalModel.userId;
                            x.LastEditBy = globalModel.userId;
                        });
                        var uploadResult = await Repository.Product.ProductAttachInsert(lstAttachFile);
                        if (!uploadResult)
                        {
                            Logger.LogError("Upload File Error");
                        }
                    }

                    await Repository.Product.ProductUpdate(Mapper.Map<CMS.Data.ModelEntity.Product>(product), globalModel.userId, ProductStatusId, prodCateId);

                   
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Success, "Cập nhật bài viết thành công", "Thành công");

                    //Noti for sectary
                    var modelfilter = new AccountSearchFilter();
                    modelfilter.RoleId = Guid.Parse("6df4162d-38a4-42e9-b3d3-a07a5c29215b"); // phụ trách chuyên mục
                    modelfilter.PageSize = 100;
                    modelfilter.CurrentPage = 1;
                    modelfilter.Active = true;
                    var lstProfielSec = await Repository.AspNetUsers.GetLstUsersPaging(modelfilter);
                    if (lstProfielSec != null && lstProfielSec.Items.Count > 0)
                    {
                        foreach (var p in lstProfielSec.Items)
                        {
                            await hubConnection.SendAsync("SendNotification", p.Id, "Bài viết mới gửi", $"Bài viết {product.Name} đã được {globalModel.user.Identity.Name} gửi tới tòa soạn chờ sơ duyệt", $"/Admin/Product/Preview?productId={product.Id}", product.Image);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình cập nhật {ex}", "Lỗi");
                }
            }
            NavigationManager.NavigateTo("/Admin/Product");
        }

        //Config Editor
        public List<IEditorTool> Tools { get; set; } = new List<IEditorTool>()
       {
            new EditorButtonGroup(new Bold(), new Italic(), new Underline()),
            new EditorButtonGroup(new AlignLeft(), new AlignCenter(), new AlignRight()),
            new UnorderedList(),
            new EditorButtonGroup(new CreateLink(), new Unlink(), new InsertImage()),
            new InsertTable(),
            new EditorButtonGroup(new AddRowBefore(), new AddRowAfter(), new MergeCells(), new SplitCell()),
            new Format(),
            new FontSize(),
            new FontFamily(),
            new CustomTool("ImportImage")
       };

        private async Task InsertSubImage(InputFileChangeEventArgs e)
        {
            var imageFiles = e.GetMultipleFiles();       
            if(imageFiles.Count + lstProductPicture.Count + lstSubImageData.Count > 10)
            {
                toastService.ShowError("Chỉ có thể tối đa 10 ảnh", "Thông báo");
                return;
            }
            foreach (var item in imageFiles)
            {
                var format = item.ContentType;
                var resizedImageFile = await item.RequestImageFileAsync(format, 1900, 1080);
                var buffer = new byte[resizedImageFile.Size];
                await resizedImageFile.OpenReadStream().ReadAsync(buffer);
                lstSubImageData.Add($"data:{format};base64,{Convert.ToBase64String(buffer)}");
                
            }
        }

        //Save MainImage
        protected async Task<string> SaveMainImage(int ProductId, List<string> imageDataUrls)
        {
            string fileName = "noimages.png";
            foreach (var file in imageDataUrls)
            {
                var imageDataByteArray = Convert.FromBase64String(CMS.Common.Utils.GetBase64Image(file));

                var urlProduct = await Repository.Product.CreateProductURL(ProductId);
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                fileName = String.Format("{0}-{1}.{2}", urlProduct, timestamp, "webp");
                var physicalPath = Path.Combine(_env.WebRootPath, "data/product/mainimages/original");
                ImageCodecInfo jpgEncoder = CMS.Common.Utils.GetEncoder(ImageFormat.Jpeg);

                System.Drawing.Imaging.Encoder myEncoder =
                    System.Drawing.Imaging.Encoder.Quality;
      
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                using (MemoryStream ms = new(imageDataByteArray))
                {
                    using Bitmap bm2 = new(ms);
                    bm2.Save(Path.Combine(physicalPath, fileName), jpgEncoder, myEncoderParameters);
                }
                try
                {
                    Utils.EditSize(Path.Combine(_env.WebRootPath, "data/product/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/product/mainimages/small", fileName), 500, 500);
                    Utils.EditSize(Path.Combine(_env.WebRootPath, "data/product/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/product/mainimages/thumb", fileName), 120, 120);
                }
                catch
                {
                }
            }
            return fileName;
        }
        //Save MainImage
        protected async Task<List<ProductPicture>> SaveSubImage(int ProductId, string userId, List<string> imageDataUrls)
        {
            List<ProductPicture> lstProPic = new();
           
            int endSub = 1;
            foreach (var file in imageDataUrls)
            {
                ProductPicture item = new();
                var imageDataByteArray = Convert.FromBase64String(CMS.Common.Utils.GetBase64Image(file));

                var urlProduct = await Repository.Product.CreateProductURL(ProductId);
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                string fileName = String.Format("{0}-{1}-{2}.{3}", urlProduct, endSub, timestamp, "webp");
                var physicalPath = Path.Combine(_env.WebRootPath, "data/productpicture/mainimages/original");
                ImageCodecInfo jpgEncoder = CMS.Common.Utils.GetEncoder(ImageFormat.Jpeg);

                System.Drawing.Imaging.Encoder myEncoder =
                    System.Drawing.Imaging.Encoder.Quality;

                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                using (MemoryStream ms = new(imageDataByteArray))
                {
                    using Bitmap bm2 = new(ms);
                    bm2.Save(Path.Combine(physicalPath, fileName), jpgEncoder, myEncoderParameters);
                }
                try
                {
                    Utils.EditSize(Path.Combine(_env.WebRootPath, "data/productpicture/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/productpicture/mainimages/small", fileName), 500, 500);
                    Utils.EditSize(Path.Combine(_env.WebRootPath, "data/productpicture/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/productpicture/mainimages/thumb", fileName), 120, 120);
                }
                catch
                {
                }
                item.Image = fileName;
                item.ProductId = productId;
                item.Sort = endSub;
                item.CreateBy = userId;
                item.CreateDate = DateTime.Now;
                item.LastEditBy = userId;
                item.LastEditDate = DateTime.Now;
                lstProPic.Add(item);
                //Increase 
                endSub++;
                
            }
            return lstProPic;
        }
        public string ToAbsoluteUrl(string url)
        {
            return $"{NavigationManager.BaseUri}{url}";
        }

        public void OnSuccess(UploadSuccessEventArgs args)
        {
            foreach (var file in args.Files)
            {
                ProductAttachFile item = new ProductAttachFile();
                item.AttachFileName = file.Name;
                item.FileType = file.Extension;
                item.FileSize = file.Size;
                lstAttachFile.Add(item);
            }
        }

        public void OnRemove(UploadEventArgs args)
        {
            foreach (var file in args.Files)
            {
                var itemDel = lstAttachFile.FirstOrDefault(p => p.AttachFileName == file.Name);
                if (itemDel != null)
                {
                    lstAttachFile.Remove(itemDel);
                }
            }
        }

        private async Task DeleteAttachFile(int productAttachFileId)
        {
            await Repository.Product.ProductAttachDelete(productAttachFileId);
            StateHasChanged();
        }

        private void OnCropImage(bool isMainImages)
        {
            isCropMainImage = isMainImages;

            imageCropperModal.Show();
        }

        protected void ConfirmImageCropper(bool isDone)
        {
            if (isDone)
            {
                if (imageCropperModal.ImgData != null)
                {
                    if (isCropMainImage)
                    {
                        product.Image = null;
                        imageDataUrls.Clear();
                        imageDataUrls.Add(imageCropperModal.ImgData);
                    }
                    else
                    {
                        product.Content = product.Content + Environment.NewLine + $"<img src=\"{imageCropperModal.ImgData}\"/>" + Environment.NewLine;
                    }

                    StateHasChanged();
                }
            }
        }


        bool CheckContentHasBase64(string content)
        {

            var regex = new Regex(@"<img src=""(?<data>.*)""");
            var match = regex.Matches(content).ToList();
            if (match.Count > 0)
            {
                return true;
            }
            return false;
        }

        public string UploadImgBase64Content(string imgName, string pathSave, string content)
        {
            var regex = new Regex(@"<img src=""(?<data>.*)""");
            var match = regex.Matches(content).ToList();
            foreach (var file in match)
            {
                if (file.Groups["data"].Value.StartsWith("data:image"))
                {
                    var imageDataByteArray = Convert.FromBase64String(CMS.Common.Utils.GetBase64Image(file.Groups["data"].Value));
                    var urlProduct = imgName;
                    var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    string fileName = String.Format("{0}-{1}.{2}", urlProduct, timestamp, "webp");
                    var physicalPath = Path.Combine(_env.WebRootPath, pathSave);
                    if (!System.IO.Directory.Exists(physicalPath))
                    {
                        System.IO.Directory.CreateDirectory(physicalPath);
                    }
                    ImageCodecInfo jpgEncoder = CMS.Common.Utils.GetEncoder(ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID  
                    // for the Quality parameter category.  
                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object.  
                    // An EncoderParameters object has an array of EncoderParameter  
                    // objects. In this case, there is only one  
                    // EncoderParameter object in the array.  
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    using (MemoryStream ms = new(imageDataByteArray))
                    {
                        using Bitmap bm2 = new(ms);
                        bm2.Save(Path.Combine(physicalPath, fileName), jpgEncoder, myEncoderParameters);
                    }
                    content = content.Replace(file.Groups["data"].Value, $"{pathSave}/{fileName}");
                }

            }
            return content;
        }
        private async Task OnDeleteSubImage(bool isExistsDB,string imgSub ,ProductPicture item)
        {
            if(!isExistsDB) // file base 64
            {
                lstSubImageData.Remove(imgSub);
                StateHasChanged();
            }
            else
            {
                var result =await Repository.ProductPicture.ProductPictureDeleteById(item.Id);
                if(result)
                {
                    lstProductPicture.Remove(item);
                    StateHasChanged();
                }    
            }
        }
        #endregion Event
    }
}
