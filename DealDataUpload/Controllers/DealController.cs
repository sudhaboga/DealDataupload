using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealDataUpload.Models;
using DealDataUpload.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DealDataUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DealController : ControllerBase
    {
        protected IDealService _dealService;
        private readonly ILogger<DealController> _logger;
        public DealController(ILogger<DealController> logger, IDealService dealService)
        {
            _logger = logger;
            _dealService = dealService;
        }

        // GET: api/<DealController>
        [HttpGet]
        public IEnumerable<Deal> Get()
        {
            return _dealService.GetAll();
        }

        [HttpGet("GetAllVehicles")]
        public IEnumerable<Deal> GetAllVehicles()
        {
            return _dealService.GetAll();
        }

        [HttpGet("GetMostPopularVehicle")]
        public string[] GetMostPopularVehicle()
        {
            return _dealService.GetMostPopularVehicle();
        }

        [HttpPost]
        public async Task<FileUploadModel> PostAsync([FromForm] List<IFormFile> files)
        {
            FileUploadModel result = new FileUploadModel();
            List<CustomError> errorsList = new List<CustomError>();            
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    if (Path.GetExtension(formFile.FileName).ToLower() != ".csv")
                    {
                        CustomError error = new CustomError();

                        error.message = string.Format("Filename: {0} Error: {1}", formFile.FileName, "Invalid file type. supported file types are: csv");
                        errorsList.Add(error);
                    }
                    else
                    {
                        var errors = await _dealService.UploadDealsAsync(formFile);
                        if (errors != null && errors.Count > 0)
                        {
                            errors.First().message = string.Format("Filename: {0} Error(s): {1}", formFile.FileName, errors.First().message);
                            errorsList.AddRange(errors);
                        }
                    }
                }
            }
            result.deals = GetAllVehicles().ToList();
            result.errors = errorsList;
            return result;
        }

      
    }
}
