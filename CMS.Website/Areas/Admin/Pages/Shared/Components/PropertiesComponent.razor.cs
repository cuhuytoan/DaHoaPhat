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


namespace CMS.Website.Areas.Admin.Pages.Shared.Components
{
    public partial class PropertiesComponent
    {
        #region Inject

        [Inject]
        private IMapper Mapper { get; set; }

        [Inject]
        private ILoggerManager Logger { get; set; }

        [Inject]
        private UserManager<IdentityUser> UserManager { get; set; }

        #endregion Inject

        [Parameter]
        public int ProductId { get; set; }

        private List<ProductPropertiesCategoryDTO> lstProductPropertiesCategory { get; set; } = new();
        #region LifeCycle
        protected override async Task OnInitializedAsync()
        {
            await InitControl();
            await InitData();         
        }
        #endregion

        #region Init
        protected async Task InitControl()
        {

        }
        protected async Task InitData()
        {
          
        }
        #endregion
    }
}
