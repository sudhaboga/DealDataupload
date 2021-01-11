using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DealDataUpload.Models;

namespace DealDataUpload.Services
{
   public interface IDealService
    {
        List<Deal> GetAll();

        string[] GetMostPopularVehicle();
        Task<List<CustomError>> UploadDealsAsync(IFormFile file);

       
    }
}
