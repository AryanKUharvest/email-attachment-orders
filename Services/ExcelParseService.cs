using System.Data;
using System.Text;
using ExcelDataReader;
using email_attachment_orders.Dto;

namespace email_attachment_orders.Services
{
    public class ExcelParseService
    {
        private static readonly string[] RequiredHeaders =
        {
            "S No",
            "PO Number",
            "Outlet Name",
            "Items Number",
            "Item Name",
            "HSN Code",
            "Specification",
            "UOM",
            "Pack Size",
            "Price",
            "Tax",
            "Delivery Date",
            "Qty"
        };

        public ExcelParseService()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public List<ExcelOrderDto> ParseExcel(byte[] excelBytes)
        {
            var orders = new List<ExcelOrderDto>();

            using var stream = new MemoryStream(excelBytes);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });

            if (dataSet.Tables.Count == 0)
                return orders;

            DataTable table = dataSet.Tables[0];

            ValidateHeaders(table);

            foreach (DataRow row in table.Rows)
            {
                var order = new ExcelOrderDto
                {
                    SNo = ToInt(row["S No"]),
                    PoNumber = ToString(row["PO Number"]),
                    OutletName = ToString(row["Outlet Name"]),
                    ItemNumber = ToInt(row["Items Number"]),
                    ItemName = ToString(row["Item Name"]),
                    HsnCode = ToString(row["HSN Code"]),
                    Specification = ToString(row["Specification"]),
                    Uom = ToString(row["UOM"]),
                    PackSize = ToInt(row["Pack Size"]),
                    Price = ToDecimal(row["Price"]),
                    Tax = ToDecimal(row["Tax"]),
                    DeliveryDate = ToDateTime(row["Delivery Date"]),
                    Qty = ToInt(row["Qty"])
                };

                orders.Add(order);
            }

            return orders;
        }

        private void ValidateHeaders(DataTable table)
        {
            var actualHeaders = table.Columns
                .Cast<DataColumn>()
                .Select(c => c.ColumnName.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingHeaders = RequiredHeaders
                .Where(header => !actualHeaders.Contains(header))
                .ToList();

            if (missingHeaders.Any())
            {
                throw new Exception(
                    "Invalid Excel format. Missing headers: " +
                    string.Join(", ", missingHeaders)
                );
            }
        }

        private string ToString(object? value)
        {
            return value?.ToString()?.Trim() ?? string.Empty;
        }

        private int ToInt(object? value)
        {
            int.TryParse(value?.ToString(), out int result);
            return result;
        }

        private decimal ToDecimal(object? value)
        {
            decimal.TryParse(value?.ToString(), out decimal result);
            return result;
        }

        private DateTime ToDateTime(object? value)
        {
            DateTime.TryParse(value?.ToString(), out DateTime result);
            return result;
        }
    }
}