using CMS.Common;
using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Services.RepositoriesBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IProductPropertiesRepository :   IRepositoryBase<ProductProperty>
    {
       
    }
    public class ProductPropertiesRepository : RepositoryBase<ProductProperty>, IProductPropertiesRepository
    {
        public ProductPropertiesRepository(CmsContext CmsDBContext) : base(CmsDBContext)
        {
        }

     
    }
}
