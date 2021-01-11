using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DealDataUpload.Models;

namespace DealDataUpload.Utilities
{
   public  interface ICSVReader
    {
       FileUploadModel Read(MemoryStream memoryStream);
    }
}
