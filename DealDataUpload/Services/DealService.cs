using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DealDataUpload.Models;
using DealDataUpload.Utilities;

namespace DealDataUpload.Services
{
    public class DealService : IDealService
    {
        private readonly IDBService _inMemoryDBService;

        private readonly ICSVReader _csvReader;
        public List<Deal> GetAll()
        {
            return _inMemoryDBService.FetchAllDeals();
        }
        public DealService(ICSVReader csvReader, IDBService dbService)
        {
            _csvReader = csvReader;
            _inMemoryDBService = dbService;
        }

       
        public async Task<List<CustomError>> UploadDealsAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                //dealService.clear();
                await file.CopyToAsync(memoryStream);
                FileUploadModel uploadresult = _csvReader.Read(memoryStream);
                if (uploadresult.errors.Count == 0)
                {
                    List<Deal> totDeals = uploadresult.deals;
                    _inMemoryDBService.AddMultipleDeals(totDeals);
                }
                else
                {
                    return uploadresult.errors;
                }
            }
            return null;
        }

        public string[] GetMostPopularVehicle()
        {
            return _inMemoryDBService.FetchMostPopularVehicles();
        }
    }
}
