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

namespace CMS.Website.Areas.Admin.Pages.Article
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

        public int? articleId { get; set; }
        public ArticleDTO article { get; set; } = new ArticleDTO();
        public int ArticleStatusId { get; set; } = 0;
        private List<ArticleCategory> lstArticleCategory { get; set; } = new List<ArticleCategory>();

        public string PreviewImage { get; set; }
        public List<int> SelectedCateValue { get; set; } = new List<int>();
        public List<string> SelectedCateName { get; set; } = new List<string>();
        private List<string> imageDataUrls = new List<string>();
        public int postType { get; set; }
        public bool chkTopArticleCategory { get; set; } = false;
        public bool chkTopArticleCategoryParent { get; set; } = false;
        public IReadOnlyList<IBrowserFile> MainImages { get; set; }
        string outMessage  = "";
        private bool isCropMainImage { get; set; }        

        // setup upload endpoints
        public string SaveUrl => ToAbsoluteUrl("api/upload/save");

        public string RemoveUrl => ToAbsoluteUrl("api/upload/remove");

        ////List FileAttach Add new
        private List<ArticleAttachFile> lstAttachFile { get; set; } = new List<ArticleAttachFile>();

        ////List FileAttach binding
        private List<ArticleAttachFile> lstAttachFileBinding { get; set; } = new List<ArticleAttachFile>();

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
            if(!Repository.Permission.CanAddNewArticle(globalModel.user,globalModel.userId,ref outMessage))
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
            var lstArticleCate = await Repository.ArticleCategory.GetArticleCategoryById(null);
            if (lstArticleCate != null)
            {
                lstArticleCategory = lstArticleCate.Select(x => new ArticleCategory { Id = x.Id, Name = x.Name }).ToList();
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
                    //Get Lst ArticleCategory
                    var lstArtCate = await Repository.ArticleCategory.GetLstArticleCatebyArticleId((int)article.Id);
                    SelectedCateValue = lstArtCate.Select(x => x.ArticleCategoryId).ToList();
                }
                //L
                lstAttachFileBinding = await Repository.Article.ArticleAttachGetLstByArticleId((int)articleId);
            }
        }

        #endregion Init

        #region Event

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

        private void OnArticleSelected()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            SelectedCateName = lstArticleCategory.Where(p => SelectedCateValue.Contains(p.Id)).Select(p => p.Name).ToList();
            article.ArticleCategoryIds = String.Join(",", SelectedCateValue.ToArray());
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            article.Image = null;
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

        private async Task PostArticle()
        {    
            if (postType == 0)
            {
                ArticleStatusId = 0;
            }
            else if (postType == 1)
            {
                ArticleStatusId = 1;
            }
            else if (postType == -999)
            {
                NavigationManager.NavigateTo("/Admin/Article");
            }
            //Create new
            if (article.Id == null || article.Id == 0)
            {
                //Check permission
                if(!Repository.Permission.CanAddNewArticle(globalModel.user,globalModel.userId,ref outMessage))
                {
                    if (!Repository.Permission.CanAddNewArticle(globalModel.user, globalModel.userId, ref outMessage))
                    {
                        NavigationManager.NavigateTo("/Admin/Error403");
                    }              
                }
                article.EndDate = article.StartDate?.AddYears(100);
                article.Id = await Repository.Article.ArticleInsert(Mapper.Map<CMS.Data.ModelEntity.Article>(article), globalModel.userId, ArticleStatusId, SelectedCateValue);
            }
            //Update
            if (article.Id != null && article.Id > 0)
            {
                //Check permission
                if (!Repository.Permission.CanEditArticle(globalModel.user, globalModel.userId, (int)article.Id, ref outMessage))
                {
                    NavigationManager.NavigateTo("/Admin/Error403");                  
                }
                try
                {   //Save Main Image
                    if (imageDataUrls != null & imageDataUrls.Count > 0)
                    {
                        article.Image = await SaveMainImage((int)article.Id, imageDataUrls);
                    }
                    //change Content
                    if(article.Content !=null && CheckContentHasBase64(article.Content))
                    {
                        article.Content = UploadImgBase64Content(article.Url, $"data/article/upload/{globalModel.userId}/{DateTime.Now:yyyy-MM-dd}", article.Content);
                    }    
                    //Save Upload File
                    if (lstAttachFile.Count > 0)
                    {
                        lstAttachFile.ForEach(x =>
                        {
                            x.ArticleId = article.Id;
                            x.CreateDate = DateTime.Now;
                            x.LastEditDate = DateTime.Now;
                            x.CreateBy = globalModel.userId;
                            x.LastEditBy = globalModel.userId;
                        });
                        var uploadResult = await Repository.Article.ArticleAttachInsert(lstAttachFile);
                        if (!uploadResult)
                        {
                            Logger.LogError("Upload File Error");
                        }
                    }

                    await Repository.Article.ArticleUpdate(Mapper.Map<CMS.Data.ModelEntity.Article>(article), globalModel.userId, ArticleStatusId, SelectedCateValue);

                    if (chkTopArticleCategory == true)
                    {
                        Repository.Article.ArticleTopCategorySave(article.Id.Value);
                    }
                    if (chkTopArticleCategoryParent == true)
                    {
                        Repository.Article.ArticleTopParentCategorySave(article.Id.Value);
                    }
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
                            await hubConnection.SendAsync("SendNotification", p.Id, "Bài viết mới gửi", $"Bài viết {article.Name} đã được {globalModel.user.Identity.Name} gửi tới tòa soạn chờ sơ duyệt", $"/Admin/Article/Preview?articleId={article.Id}", article.Image);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình cập nhật {ex}", "Lỗi");
                }
            }
            NavigationManager.NavigateTo("/Admin/Article");
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

        private async Task InsertImageEditor(InputFileChangeEventArgs e)
        {
            var imageFiles = e.GetMultipleFiles();
            MainImages = imageFiles;
            var format = "image/png";
            foreach (var item in imageFiles)
            {
                var resizedImageFile = await item.RequestImageFileAsync(format, 500, 500);
                var buffer = new byte[resizedImageFile.Size];
                await resizedImageFile.OpenReadStream().ReadAsync(buffer);
                var imageDataUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                article.Content = article.Content + Environment.NewLine + $"<img src=\"{imageDataUrl}\"/>" + Environment.NewLine;
            }
        }

        //Save MainImage
        protected async Task<string> SaveMainImage(int ArticleId, List<string> imageDataUrls)
        {
            string fileName = "noimages.png";
            foreach (var file in imageDataUrls)
            {
                var imageDataByteArray = Convert.FromBase64String(CMS.Common.Utils.GetBase64Image(file));

                var urlArticle = await Repository.Article.CreateArticleURL(ArticleId);
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                fileName = String.Format("{0}-{1}.{2}", urlArticle, timestamp, "webp");
                var physicalPath = Path.Combine(_env.WebRootPath, "data/article/mainimages/original");
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
                try
                {
                    Utils.EditSize(Path.Combine(_env.WebRootPath, "data/article/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/article/mainimages/small", fileName), 500, 500);
                    Utils.EditSize(Path.Combine(_env.WebRootPath, "data/article/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/article/mainimages/thumb", fileName), 120, 120);
                }
                catch
                {
                }
            }
            return fileName;
        }

        public string ToAbsoluteUrl(string url)
        {
            return $"{NavigationManager.BaseUri}{url}";
        }

        public void OnSuccess(UploadSuccessEventArgs args)
        {
            foreach (var file in args.Files)
            {
                ArticleAttachFile item = new ArticleAttachFile();
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

        private async Task DeleteAttachFile(int articleAttachFileId)
        {
            await Repository.Article.ArticleAttachDelete(articleAttachFileId);
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
                    if(isCropMainImage)
                    {
                        article.Image = null;
                        imageDataUrls.Clear();
                        imageDataUrls.Add(imageCropperModal.ImgData);
                    }
                    else
                    {
                        article.Content = article.Content + Environment.NewLine + $"<img src=\"{imageCropperModal.ImgData}\"/>" + Environment.NewLine;
                    }
                    
                    StateHasChanged();
                }
            }
        }


        bool CheckContentHasBase64(string content)
        {
         
            var regex = new Regex(@"<img src=""(?<data>.*)""");
            var match = regex.Matches(content).ToList();
            if(match.Count >0)
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
                if(file.Groups["data"].Value.StartsWith("data:image"))
                {
                    var imageDataByteArray = Convert.FromBase64String(CMS.Common.Utils.GetBase64Image(file.Groups["data"].Value));
                    var urlArticle = imgName;
                    var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    string fileName = String.Format("{0}-{1}.{2}", urlArticle, timestamp, "webp");
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
        #endregion Event
    }
}