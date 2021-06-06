using CMS.Data.ModelDTO;
using CMS.Data.ModelEntity;
using CMS.Data.ModelFilter;
using CMS.Services.RepositoriesBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Repositories
{
    public interface IMasterDataRepository : IRepositoryBase<Bank>
    {
        Task<List<Department>> DepartmentsGetLst();

        Task<List<Country>> CountriesGetLst();

        Task<List<Location>> LocationGetLstByCountryId(int countryId);

        Task<List<Ward>> WardsGetLstByDistrictId(int districtId);

        Task<List<District>> DistrictsGetLstByLocationId(int locationId);

        Task<List<Bank>> BankGetLst();

        Task<List<Unit>> UnitGetLst();
       
    }

    public class MasterDataRepository : RepositoryBase<Bank>, IMasterDataRepository
    {
        public MasterDataRepository(CmsContext CmsContext) : base(CmsContext)
        {
        }

        public async Task<List<Department>> DepartmentsGetLst()
        {
            return await CmsContext.Department.AsNoTracking().ToListAsync();
        }

        public async Task<List<Country>> CountriesGetLst()
        {
            return await CmsContext.Country.AsNoTracking().ToListAsync();
        }

        public async Task<List<Location>> LocationGetLstByCountryId(int countryId)
        {
            return await CmsContext.Location.Where(p => p.CountryId == countryId).AsNoTracking().ToListAsync();
        }

        public async Task<List<Ward>> WardsGetLstByDistrictId(int districtId)
        {
            return await CmsContext.Ward.Where(p => p.DistrictId == districtId).AsNoTracking().ToListAsync();
        }

        public async Task<List<District>> DistrictsGetLstByLocationId(int locationId)
        {
            return await CmsContext.District.Where(p => p.LocationId == locationId).AsNoTracking().ToListAsync();
        }

        public async Task<List<Bank>> BankGetLst()
        {
            return await CmsContext.Bank.AsNoTracking().ToListAsync();
        }

        public async Task<List<Unit>> UnitGetLst()
        {
            return await CmsContext.Unit.AsNoTracking().ToListAsync();
        }
    }
}