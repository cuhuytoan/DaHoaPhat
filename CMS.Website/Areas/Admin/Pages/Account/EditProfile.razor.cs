using AutoMapper;
using Blazored.Toast.Services;
using CMS.Common;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Website.Areas.Admin.Pages.Shared.Components;
using CMS.Website.Logging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private IMapper Mapper { get; set; }

        [Inject]
        private ILoggerManager Logger { get; set; }

        [Inject]
        private UserManager<IdentityUser> UserManager { get; set; }

        #endregion Inject

        #region Parameter

        public AspNetUserProfilesDTO userInfo { get; set; } = new AspNetUserProfilesDTO();

        private List<AspNetRoles> lstRole { get; set; }
        private List<KeyValuePair<bool, string>> lstGender { get; set; }

        private List<string> imageDataUrls = new();
        private IReadOnlyList<IBrowserFile> MainImages;
        public DateTime MaxDate = new DateTime(2021, 12, 31);
        public DateTime MinDate = new DateTime(1950, 1, 1);

        private List<Location> lstLocation { get; set; } = new();
        private List<District> lstDistrict { get; set; } = new();
        private List<Ward> lstWard { get; set; } = new();
        private List<Department> lstDepartment { get; set; } = new();
        private List<Bank> lstBank { get; set; } = new();

        [CascadingParameter]
        private GlobalModel globalModel { get; set; }

        //Modal Crop Image
        protected ImageCropper imageCropperModal { get; set; }

        #endregion Parameter

        #region LifeCycle

        protected override async Task OnInitializedAsync()
        {
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
            //Binding lstGender
            List<KeyValuePair<bool, string>> lstGenderAdd = new List<KeyValuePair<bool, string>>();
            lstGenderAdd.Add(new KeyValuePair<bool, string>(true, "Nam"));
            lstGenderAdd.Add(new KeyValuePair<bool, string>(false, "Nữ"));
            lstGender = lstGenderAdd.ToList();

            lstLocation = await Repository.MasterData.LocationGetLstByCountryId(1);
            lstLocation = lstLocation.Select(x => new Location { Id = x.Id, Name = x.Name }).ToList();

            lstDepartment = await Repository.MasterData.DepartmentsGetLst();
            lstDepartment = lstDepartment.Select(x => new Department { Id = x.Id, Name = x.Name }).ToList();

            lstBank = await Repository.MasterData.BankGetLst();
            lstBank = lstBank.Select(x => new Bank { Id = x.Id, Name = x.Name }).ToList();

            
        }

        protected async Task InitData()
        {
            var result = await Repository.AspNetUsers.AspNetUserProfilesGetByUserId(globalModel.userId);
            if (result != null)
            {
                userInfo = Mapper.Map<AspNetUserProfilesDTO>(result);
            }
            if(userInfo.LocationId >0)
            {
                lstDistrict = userInfo.LocationId == null ? new() : await Repository.MasterData.DistrictsGetLstByLocationId((int)userInfo.LocationId);
                lstDistrict = lstDistrict.Select(x => new District { Id = x.Id, Name = x.Name }).ToList();
            }    
            if(userInfo.DistrictId >0)
            {
                lstWard = userInfo.DistrictId == null ? new() : await Repository.MasterData.WardsGetLstByDistrictId((int)userInfo.DistrictId);
                lstWard = lstWard.Select(x => new Ward { Id = x.Id, Name = x.Name }).ToList();
            }    
            
        }

        #endregion Init

        #region Event

        private async Task PostUserInfo()
        {
            var profileExists = Repository.AspNetUsers.AspNetUserProfilesGetByUserId(globalModel.userId);
            if (profileExists != null)
            {
                try
                {
                    //Save Main Image
                    if (imageDataUrls != null && imageDataUrls.Count >0)
                    {
                        userInfo.AvatarUrl = await SaveMainImage((int)profileExists.Id, imageDataUrls);
                    }

                    await Repository.AspNetUsers.AspNetUserProfilesUpdate(
                      Mapper.Map<AspNetUserProfiles>(userInfo));

                    //ToastMessage
                    toastService.ShowToast(ToastLevel.Success, "Cập nhật user thành công", "Thành công");
                    NavigationManager.NavigateTo("Admin/Article", true);
                }
                catch
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

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
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
        protected async Task<string> SaveMainImage(int UserProfileId, List<string> imageDataUrls)
        {
            string fileName = "noimages.png";
            foreach (var file in imageDataUrls)
            {
                var imageDataByteArray = Convert.FromBase64String(CMS.Common.Utils.GetBase64Image(file));

                var urlArticle = $"Profile_{UserProfileId}";
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                fileName = String.Format("{0}-{1}.{2}", urlArticle, timestamp, CMS.Common.Utils.GetBase64ImageMime(file));
                var physicalPath = Path.Combine(_env.WebRootPath, "data/user/mainimages/original");

                using (MemoryStream ms = new(imageDataByteArray))
                {
                    using Bitmap bm2 = new(ms);
                    bm2.Save(Path.Combine(physicalPath, fileName));
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

        private void OnCropImage()

        {
            imageCropperModal.Show();
        }

        protected void ConfirmImageCropper(bool isDone)
        {
            if (isDone)
            {
                if (imageCropperModal.ImgData != null)
                {
                    userInfo.AvatarUrl = null;
                    imageDataUrls.Clear();
                    imageDataUrls.Add(imageCropperModal.ImgData);
                    StateHasChanged();
                }
            }
        }

        private async void LocationSelected()
        {
            lstDistrict = userInfo.LocationId == null ? new() : await Repository.MasterData.DistrictsGetLstByLocationId((int)userInfo.LocationId);
            lstDistrict = lstDistrict.Select(x => new District { Id = x.Id, Name = x.Name }).ToList();
            StateHasChanged();
        }

        private async void DistrictSelected()
        {
            lstWard = userInfo.DistrictId == null ? new() : await Repository.MasterData.WardsGetLstByDistrictId((int)userInfo.DistrictId);
            lstWard = lstWard.Select(x => new Ward { Id = x.Id, Name = x.Name }).ToList();
            StateHasChanged();
        }

        #endregion Event
    }
}