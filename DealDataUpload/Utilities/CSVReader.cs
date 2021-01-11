using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DealDataUpload.Models;

namespace DealDataUpload.Utilities
{
    public class CsvReader : ICSVReader
    {
        public FileUploadModel Read(MemoryStream memoryStream)
        {
            var deals = new List<Deal>();
            var enc1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
            FileUploadModel result = new FileUploadModel(){ errors = new List<CustomError>()};
            CustomError errorout = new CustomError();
            string errormessage = string.Empty;
            try
            {
                using (var sr = new StreamReader(memoryStream, enc1252))
                {
                    bool isheaderRead = false;
                    string line;
                    string[] dealDataheader = new string[] { };
                    sr.BaseStream.Position = 0;
                    int linepos = 0;
                    
                    do
                    {
                        line = sr.ReadLine();

                        if (line != null)
                        {


                            if (isheaderRead)
                            {
                                linepos++;
                                var dealData = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                                Deal deal = AssignValues(dealData, dealDataheader, out errormessage);
                                if (string.IsNullOrEmpty(errormessage))
                                    deals.Add(deal);
                                else
                                {
                                   
                                    errormessage = string.Format("{0} at line number {1}", errormessage, linepos);
                                    errorout.message = errormessage;
                                    result.errors.Add(errorout);
                                }
                            }
                            else
                            {
                                dealDataheader = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                                if(dealDataheader == null)
                                {
                                    errorout.message = "Header is required";
                                    result.errors.Add(errorout);
                                    return result;
                                   
                                }
                                if(dealDataheader.Length > 0 && !dealDataheader.Any(m => m.ToLower() == "dealnumber"))
                                {
                                    errorout.message = "DealNumber column is a required field in the header";
                                    result.errors.Add(errorout);
                                    return result;

                                }
                                isheaderRead = true;
                                linepos = 1;
                            }


                        }
                    } while (line != null);
                }
                result.deals = deals;
            }
            catch(Exception ex)
            {
                //log  error here
                errorout.message = "File is corrupt. Please contact support";
                result.errors.Add(errorout);
            }
           
            return result;
        }
       
        private Deal AssignValues(string[] dealData, string[] header, out string error)
        {
            var obj = new Deal();
            int index = 0;
            var ci = new CultureInfo("en-US");
            error = string.Empty;
            string[] formats = new[] { "MM/dd/yyyy", "M/dd/yyyy", "MM-dd-yyyy" }.
                             Union(ci.DateTimeFormat.GetAllDateTimePatterns()).ToArray(); ;
            foreach (string val in header)
            {
                Type type = obj.GetType();

                PropertyInfo prop = type.GetProperty(val);

                if (index < dealData.Length)
                {
                    dealData[index] = dealData[index].TrimStart(' ', '"');
                    dealData[index] = dealData[index].TrimEnd('"');
                    if(string.IsNullOrEmpty(dealData[index]))
                    {
                        error = string.Format("{0} cannot be empty", val);
                        return null;
                    }
                    else
                    {
                        switch (prop.PropertyType.Name)
                        {
                            case "Int32":
                                int dealNumber;
                                if (int.TryParse(dealData[index], out dealNumber))
                                {
                                    prop.SetValue(obj, dealNumber);
                                }
                                else
                                {
                                    error = string.Format("Error occured while reading dealNumber");
                                    return null;
                                }
                                break;
                            case "Decimal":
                                Decimal price;
                                if (decimal.TryParse(dealData[index], out price))
                                {
                                    prop.SetValue(obj, price);
                                }
                                else
                                {
                                    error = string.Format("Error occured while reading price");
                                    return null;
                                }
                                break;
                            case "DateTime":
                                DateTime dateTime;
                                if (DateTime.TryParseExact(dealData[index], formats, CultureInfo.InvariantCulture,
                               DateTimeStyles.None, out dateTime))
                                {
                                    prop.SetValue(obj,
                                               dateTime);
                                }
                                else
                                {
                                    error = string.Format("Error occured while reading date");
                                    return null;
                                }
                                break;
                            default:
                                prop.SetValue(obj, dealData[index]);
                                break;
                        }
                    }
                   
                }
                index++;
            }
            return obj;
        }

        

    }

    
}
