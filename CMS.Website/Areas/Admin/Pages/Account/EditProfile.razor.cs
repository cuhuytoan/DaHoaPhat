using AutoMapper;
using Blazored.Toast.Services;
using CMS.Common;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace CMS.Website.Areas.Admin.Pages.Account
{
    public partial class EditProfile : IDisposable
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
        public AspNetUserProfilesDTO userInfo { get; set; } = new AspNetUserProfilesDTO();

        List<AspNetRoles> lstRole { get; set; }
        List<KeyValuePair<bool, string>> lstGender { get; set; }

        IList<string> imageDataUrls = new List<string>();
        IReadOnlyList<IBrowserFile> MainImages;
        public DateTime MaxDate = new DateTime(2021, 12, 31);
        public DateTime MinDate = new DateTime(1950, 1, 1);
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        ClaimsPrincipal user;
        string userId { get; set; }



        #endregion

        #region LifeCycle
        protected override async Task OnParametersSetAsync()
        {

        }

        protected override async Task OnInitializedAsync()
        {
            //get claim principal
            var authState = await authenticationStateTask;
            user = authState.User;
            userId = user.FindFirstValue(ClaimTypes.NameIdentifier);


            await InitControl();
            await InitData();


        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Init
        protected async Task InitControl()
        {
            //Binding lstGender
            List<KeyValuePair<bool, string>> lstGenderAdd = new List<KeyValuePair<bool, string>>();
            lstGenderAdd.Add(new KeyValuePair<bool, string>(true, "Nam"));
            lstGenderAdd.Add(new KeyValuePair<bool, string>(false, "Nữ"));
            lstGender = lstGenderAdd.ToList();
        }
        protected async Task InitData()
        {


            var result = await Repository.AspNetUsers.AspNetUserProfilesGetByUserId(userId);
            if (result != null)
            {
                userInfo = Mapper.Map<AspNetUserProfilesDTO>(result);
            }

        }
        #endregion

        #region Event

        async Task PostUserInfo()
        {
            var profileExists = Repository.AspNetUsers.AspNetUserProfilesGetByUserId(userId);
            if (profileExists != null)
            {
                try
                {
                    //Save Main Image
                    if (MainImages != null)
                    {
                        userInfo.AvatarUrl = await SaveMainImage((int)profileExists.Id, MainImages);
                    }

                    await Repository.AspNetUsers.AspNetUserProfilesUpdate(
                      Mapper.Map<AspNetUserProfiles>(userInfo));

                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Success, "Cập nhật user thành công", "Thành công");
                    NavigationManager.NavigateTo("Admin/Article", true);
                }
                catch (Exception ex)
                {
                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Error, $"Có lỗi trong quá trình cập nhật", "Lỗi");
                }

            }
            else
            {
                //ToastMessage
                toastService.ShowToast(ToastLevel.Error, $"Không tồn tại tài khoản cập nhật", "Lỗi");
            }

        }
        async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            userInfo.AvatarUrl = null;
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
        //Save MainImage
        protected async Task<string> SaveMainImage(int UserProfileId, IReadOnlyList<IBrowserFile> MainImage)
        {
            string fileName = "noimages.png";
            foreach (var file in MainImage)
            {
                if (file == null) return fileName;
                var urlArticle = $"Profile_{UserProfileId}";
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                fileName = String.Format("{0}-{1}.{2}", urlArticle, timestamp, file.ContentType.Replace("image/", ""));
                var physicalPath = Path.Combine(_env.WebRootPath, "data/user/mainimages/original", fileName);
                using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.OpenReadStream().CopyToAsync(fileStream);
                }

                try
                {
                    Utils.EditSize(Path.Combine(_env.WebRootPath, "data/user/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/user/mainimages/small", fileName), 500, 500);
                    Utils.EditSize(Path.Combine(_env.WebRootPath, "data/user/mainimages/original", fileName), Path.Combine(_env.WebRootPath, "data/user/mainimages/thumb", fileName), 120, 120);
                }
                catch
                {
                }
            }
            return fileName;
        }
        #endregion
    }
}
