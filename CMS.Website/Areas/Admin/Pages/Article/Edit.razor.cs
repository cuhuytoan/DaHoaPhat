using AutoMapper;
using Blazored.Toast.Services;
using CMS.Common;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Website.Areas.Admin.Pages.Shared;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;

namespace CMS.Website.Areas.Admin.Pages.Article
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

        public int? articleId { get; set; }
        public ArticleDTO article { get; set; } = new ArticleDTO();
        public int ArticleStatusId { get; set; } = 0;
        List<ArticleCategory> lstArticleCategory { get; set; }
        public string PreviewImage { get; set; }
        public int? SelectedValue { get; set; }
        List<string> imageDataUrls = new List<string>();
        public int postType { get; set; }
        public bool chkTopArticleCategory { get; set; } = false;
        public bool chkTopArticleCategoryParent { get; set; } = false;
        IReadOnlyList<IBrowserFile> MainImages;
        // setup upload endpoints
        public string SaveUrl => ToAbsoluteUrl("api/upload/save");
        public string RemoveUrl => ToAbsoluteUrl("api/upload/remove");
        ////List FileAttach Add new
        List<ArticleAttachFile> lstAttachFile { get; set; } = new List<ArticleAttachFile>();
        ////List FileAttach binding
        List<ArticleAttachFile> lstAttachFileBinding { get; set; } = new List<ArticleAttachFile>();
        //Modal Crop Image
        protected ImageCropper imageCropperModal { get; set; }
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
                    SelectedValue = Convert.ToInt32(article.ArticleCategoryIds);
                }
                //L
                lstAttachFileBinding = await Repository.Article.ArticleAttachGetLstByArticleId((int)articleId);
            }
        }
        #endregion

        #region Event

        async Task OnInputFileChange(InputFileChangeEventArgs e)
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
                article.ArticleCategoryIds = SelectedValue.ToString();
                article.Id = await Repository.Article.ArticleInsert(Mapper.Map<CMS.Data.ModelEntity.Article>(article), UserManager.GetUserId(user), ArticleStatusId);
            }
            //Update 
            if (article.Id != null && article.Id > 0)
            {
                try
                {   //Save Main Image
                    if (MainImages != null)
                    {
                        article.Image = await SaveMainImage((int)article.Id, MainImages);
                    }
                    //Save Upload File
                    if (lstAttachFile.Count > 0)
                    {
                        lstAttachFile.ForEach(x =>
                        {
                            x.ArticleId = article.Id;
                            x.CreateDate = DateTime.Now;
                            x.LastEditDate = DateTime.Now;
                            x.CreateBy = UserManager.GetUserId(user);
                            x.LastEditBy = UserManager.GetUserId(user);
                        });
                        var uploadResult = await Repository.Article.ArticleAttachInsert(lstAttachFile);
                        if (!uploadResult)
                        {
                            Logger.LogError("Upload File Error");
                        }
                    }

                    await Repository.Article.ArticleUpdate(Mapper.Map<CMS.Data.ModelEntity.Article>(article), UserManager.GetUserId(user), ArticleStatusId);

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
                    //Noti for user
                    await hubConnection.SendAsync("SendNotification", UserManager.GetUserId(user), "Gửi bài viết thành công", $"Bạn đã gửi bài viết <b>{article.Name}</b> tới tòa soạn thành công", $"/Admin/Article/Preview?articleId={article.Id}", article.Image);
                    //Noti for sectary
                    var modelfilter = new AccountSearchFilter();
                    modelfilter.RoleId = Guid.Parse("EF289F43-08FD-4C16-9F82-498BC8D1CD85"); // Thư kí
                    modelfilter.PageSize = 100;
                    modelfilter.CurrentPage = 1;
                    modelfilter.Active = true;
                    var lstProfielSec = await Repository.AspNetUsers.GetLstUsersPaging(modelfilter);
                    if (lstProfielSec != null && lstProfielSec.Items.Count > 0)
                    {
                        foreach (var p in lstProfielSec.Items)
                        {
                            await hubConnection.SendAsync("SendNotification", p.Id, "Bài viết mới gửi", $"Bài viết {article.Name} đã được {user.Identity.Name} gửi tới tòa soạn chờ sơ duyệt", $"/Admin/Article/Preview?articleId={article.Id}", article.Image);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình cập nhật {ex.ToString()}", "Lỗi");
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
        async Task InsertImageEditor(InputFileChangeEventArgs e)
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
        protected async Task<string> SaveMainImage(int ArticleId, IReadOnlyList<IBrowserFile> MainImage)
        {
            string fileName = "noimages.png";
            foreach (var file in MainImage)
            {
                if (file == null) return fileName;
                var urlArticle = await Repository.Article.CreateArticleURL(ArticleId);
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                fileName = String.Format("{0}-{1}.{2}", urlArticle, timestamp, file.ContentType.Replace("image/", ""));
                var physicalPath = Path.Combine(_env.WebRootPath, "data/article/mainimages/original", fileName);
                using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.OpenReadStream().CopyToAsync(fileStream);
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
        async Task DeleteAttachFile(int articleAttachFileId)
        {
            await Repository.Article.ArticleAttachDelete(articleAttachFileId);
            //_forceRerender = true;
            //StateHasChanged();
        }

        async Task OnCropImage()
        {
            imageCropperModal.Show();
        }
        protected async Task ConfirmImageCropper(bool isDone)
        {
            if (isDone)
            {
                if(imageCropperModal.ImgData !=null)
                {
                    imageDataUrls.Clear();
                    imageDataUrls.Add(imageCropperModal.ImgData);
                    StateHasChanged();
                }    
            }
        }
        #endregion

    }
}

