using ClosedXML.Excel;
using Cloudware.Utilities.Contract.Abstractions;
using MassTransit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Application.Command
{
    public class ExportProductsToExcelCommandConsumer : IConsumer<ExportProductsToExcelCommand>, IMediatorConsumerType
    {
        public ExportProductsToExcelCommandConsumer()
        {

        }
        public async Task Consume(ConsumeContext<ExportProductsToExcelCommand> context)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("WithdrawalRequest");

            worksheet.Cell(1, 1).Value = "Id";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Price";
            worksheet.Cell(1, 4).Value = "image";

            for (int index = 1; index <= context.Message.Products.Count; index++)
            {
                worksheet.Cell(index + 1, 1).Value = context.Message.Products[index - 1].Id;
                worksheet.Cell(index + 1, 2).Value = context.Message.Products[index - 1].Name;
                worksheet.Cell(index + 1, 3).Value = context.Message.Products[index - 1].Price;
                worksheet.Cell(index + 1, 4).Value = context.Message.Products[index - 1].image;       
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                await context.RespondAsync(new ExportProductsToExcelCommandResponse(content, contentType));
            }
        }

        public class ExportProductsToExcelCommandResponse
        {
            public ExportProductsToExcelCommandResponse(byte[] content, string contentType)
            {
                Content = content;
                ContentType = contentType;
            }

            public byte[] Content { get; set; }
            public string ContentType { get; set; }
        }
    }
}
