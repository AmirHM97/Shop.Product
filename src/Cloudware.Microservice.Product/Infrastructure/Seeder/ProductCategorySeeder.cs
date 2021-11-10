using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Cloudware.Microservice.Product.Model;
using ExcelDataReader;
using Newtonsoft.Json;

namespace Cloudware.Microservice.Product.Infrastructure.Seeder
{
    public static class ProductCategorySeeder
    {
        public static DataTable GetExcelAsDataTable(string filePath, bool isFirstRowAsColumnNames = false, bool isCheckCount = false)
        {
            try
            {
                var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                //var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                var extension = Path.GetExtension(filePath).ToLower();

                IExcelDataReader excelReader =
                extension == ".xlsx" ? ExcelReaderFactory.CreateOpenXmlReader(stream)
                : ExcelReaderFactory.CreateBinaryReader(stream);
                var config = new ExcelDataSetConfiguration { ConfigureDataTable = s => new ExcelDataTableConfiguration { UseHeaderRow = isFirstRowAsColumnNames } };

                var dataTable = excelReader.AsDataSet(config).Tables[0];
                if (isCheckCount && dataTable.Rows.Count > 5000)
                    throw new Exception("Count > 5000");

                return dataTable;
            }
            catch (Exception e)
            {
                // ErrorHandle.TempLog("1importexcel", JsonConvert.SerializeObject(e));
                return null;
            }
        }
        public static List<ProductCategory> ReadData(DataTable dataTable)
        {
            var categories = new List<ProductCategory>();
            for (var i = 57; i < dataTable.Rows.Count; i++)
            {
                var id = dataTable.Rows[i][0].ToString().Trim();
                var parentId = dataTable.Rows[i][1].ToString();
                var name = dataTable.Rows[i][2].ToString().Trim();
                categories.Add(new ProductCategory { Id = Int64.Parse(id), ParentId = Int64.Parse(parentId), Name = name });
            }
            return categories;
        }
    }
}