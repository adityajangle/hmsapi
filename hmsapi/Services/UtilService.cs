using System;
using System.Collections.Immutable;
using System.Reflection;
using hmsapi.Managers;
using hmsapi.Models;
using Newtonsoft.Json;

namespace hmsapi.Services
{
    public class UtilService
    {
        public static string DtMSql(DateTime d) => d.ToString("yyyy-MM-dd");

        public static string DttMSql(DateTime d) => d.ToString("yyyy-MM-dd HH:mm:ss");

        public static string DoMSql(DateOnly d) => d.ToString("yyyy-MM-dd");

        public static Dictionary<string, object?> ObjectToDictionary(object obj)
        {
            // Using reflection to get the properties and values of the object
            PropertyInfo[] properties = obj.GetType().GetProperties();

            // Create a dictionary to store property names and values
            Dictionary<string, object?> dictionary = new Dictionary<string, object?>();

            foreach (PropertyInfo property in properties)
            {
                // Get the name and value of each property
                string propertyName = property.Name;
                object? propertyValue = property.GetValue(obj);

                // Add to the dictionary
                dictionary.Add(propertyName, propertyValue);
            }

            return dictionary;
        }

        public static string ProcResponse(DaoResponse data, HttpContext context, SessionManager _smg)
        {
            JLogger.WRT(new DaoJLogger()
            {
                FileName = $"REQ_{context.Session.Id}",
                Message = $"Response:{JsonConvert.SerializeObject(data)} \nResponse END \n"
            });
            return EnvironmentServices.IsProduction() ? JSecurityServices.AES_Encrypt(JsonConvert.SerializeObject(data), _smg.AesKey, _smg.AesIv) : JsonConvert.SerializeObject(data);
        }

        public readonly static ImmutableArray<string> fMonths = ImmutableArray.Create("April", "May", "June", "July", "August", "September", "October", "November", "December", "January", "February", "March");
        public readonly static ImmutableArray<string> fQuarter = ImmutableArray.Create("Quarter - 1", "Quarter - 2", "Quarter - 3", "Quarter - 4");
        public readonly static ImmutableArray<string> fHalfYear = ImmutableArray.Create("First Half(Apr-Sep)", "Second Half(Oct-Mar)");

        public readonly static ImmutableArray<string> cMonths = ImmutableArray.Create("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December");
        public readonly static ImmutableArray<string> cQuarter = ImmutableArray.Create("Quarter - 1", "Quarter - 2", "Quarter - 3", "Quarter - 4");
        public readonly static ImmutableArray<string> cHalfYear = ImmutableArray.Create("First Half(Jan-Jun)", "Second Half(Jul-Dec)");

        public static string GetReadableFileSize(long fileSizeInBytes)
        {
            // Define file size units
            string[] sizeUnits = { "B", "KB", "MB", "GB", "TB" };

            int unitIndex = 0;
            double size = fileSizeInBytes;

            // Convert bytes to the appropriate size unit
            while (size >= 1024 && unitIndex < sizeUnits.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return $"{size:0.##} {sizeUnits[unitIndex]}";
        }




    }
}

