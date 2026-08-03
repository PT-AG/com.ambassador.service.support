using com.ambassador.support.lib.Helpers;
using com.ambassador.support.lib.Interfaces;
using com.ambassador.support.lib.ViewModel;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace com.ambassador.support.lib.Services
{
    public class ReceiptRawMaterialService : IReceiptRawMaterialService
    {
        SupportDbContext context;
        public readonly IServiceProvider serviceProvider;
        public ReceiptRawMaterialService(SupportDbContext _context, IServiceProvider serviceProvider)
        {
            this.context = _context;
            this.serviceProvider = serviceProvider;
        }
        #region oldQuery
        //public async Task<IQueryable<ReceiptRawMaterialViewModel>> getQuery(DateTime? dateFrom, DateTime? dateTo)
        //{
        //    var d1 = dateFrom.Value.ToString("yyyy-MM-dd");
        //    var d2 = dateTo.Value.ToString("yyyy-MM-dd");
        //    var customCategory = "Fasilitas";


        //    List<ReceiptRawMaterialViewModel> reportData = new List<ReceiptRawMaterialViewModel>();

        //    try
        //    {
        //        string connectionString = APIEndpoint.PurchasingConnectionString;
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = new SqlCommand(
        //                "declare @StartDate datetime = '" + d1 + "' declare @EndDate datetime = '" + d2 + "' " +
        //                "select distinct e.CustomsType,e.BeacukaiNo,convert(date,dateadd(hour,7,e.BeacukaiDate)) as BCDate," +
        //                "f.URNNo,convert(date,dateadd(hour,7,f.ReceiptDate)) as URNDate, a.DOCurrencyCode," +
        //                "a.SupplierName,a.Country, " +
        //                "c.ProductSeries,c.HsCode,a.RecordDate, c.DeletedAgent," +
        //                "COALESCE(NULLIF(g.PIBProductCode, ''), g.ProductCode) AS ProductCode," +
        //                "COALESCE(NULLIF(g.PIBProductName, ''), g.ProductName) AS ProductName, " +
        //                "COALESCE(NULLIF(g.PIBUom, ''), g.SmallUomUnit) AS SmallUomUnit," +
        //                "SUM(CASE WHEN g.PIBQuantity IS NOT NULL AND g.PIBQuantity <> 0 THEN g.PIBQuantity" +
        //                " ELSE g.SmallQuantity END ) AS SmallQuantity," +
        //                "SUM(CASE WHEN g.PIBValue IS NOT NULL AND g.PIBValue <> 0 THEN g.PIBValue " +
        //                "ELSE CAST((g.PricePerDealUnit * g.ReceiptQuantity) AS DECIMAL(18,2)) END) AS Amount " +
        //                "from GarmentDeliveryOrders a join GarmentDeliveryOrderItems b on a.id=b.GarmentDOId " +
        //                "join GarmentDeliveryOrderDetails c on b.id=c.GarmentDOItemId " +
        //                "join GarmentBeacukaiItems d on d.GarmentDOId=a.id join GarmentBeacukais e on e.id=d.BeacukaiId " +
        //                "join GarmentUnitReceiptNoteItems g on c.id=g.DODetailId join GarmentUnitReceiptNotes f on g.URNId=f.Id " +
        //                "where e.BeacukaiDate between @StartDate and @EndDate and a.CustomsCategory = '"+ customCategory +
        //                "' and f.URNType='PEMBELIAN' and a.IsDeleted=0 and b.IsDeleted=0 and c.IsDeleted=0 and d.IsDeleted=0 " +
        //                "and e.IsDeleted=0 and f.IsDeleted=0 and g.IsDeleted=0 " +
        //                "group by e.CustomsType,e.BeacukaiNo,e.BeacukaiDate,f.URNNo,f.ReceiptDate,g.ProductCode,g.ProductName,g.SmallUomUnit," +
        //                "a.DOCurrencyCode,a.SupplierName,a.Country,c.ProductSeries,c.HsCode,a.RecordDate,c.DeletedAgent, " +
        //                " g.PIBUom, g.PIBProductName, g.PIBProductCode  " +
        //                "order by BCDate asc", conn))

        //            {
        //                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
        //                DataSet dSet = new DataSet();
        //                dataAdapter.Fill(dSet);
        //                foreach (DataRow data in dSet.Tables[0].Rows)
        //                {
        //                    ReceiptRawMaterialViewModel view = new ReceiptRawMaterialViewModel
        //                    {
        //                        CustomsType = data["CustomsType"].ToString(),
        //                        BeacukaiNo = data["BeacukaiNo"].ToString(),
        //                        BeacukaiDate = data["BCDate"].ToString(),
        //                        SerialNo = data["ProductSeries"].ToString(),
        //                        URNNo = data["URNNo"].ToString(),
        //                        URNDate = data["URNDate"].ToString(),
        //                        ProductCode = data["ProductCode"].ToString(),
        //                        ProductName = data["ProductName"].ToString(),
        //                        SmallUomUnit = data["SmallUomUnit"].ToString(),
        //                        SmallQuantity = (decimal)data["SmallQuantity"],
        //                        DOCurrencyCode = data["DOCurrencyCode"].ToString(),
        //                        Amount = (decimal)data["Amount"],
        //                        StorageName = "GUDANG AG",
        //                        SupplierName = "-",
        //                        Country = data["Country"].ToString(),
        //                        DeletedAgent = data["DeletedAgent"].ToString(),
        //                        HsCode = data["HsCode"].ToString() == "" ? "-" : data["HsCode"].ToString(),
        //                        RecordDate = data["RecordDate"].ToString()
        //                    };

        //                    reportData.Add(view);
        //                }
        //            }
        //            conn.Close();
        //        }
        //    }
        //    catch (SqlException ex)
        //    { 
        //    }

        //    var Codes = await GetProductCode(string.Join(",", reportData.Select(x => x.ProductCode).Distinct().ToList()));

        //    string[] exceptionBCNo = { "629905", "627663" , "038117", "046380", "621904", "758615", "643895" };
        //    foreach(var a in reportData)
        //    {
        //        var trimProduct = a.ProductCode.Trim();
        //        var remark = Codes.FirstOrDefault(x => x.Code.Trim() == trimProduct);
        //        //var Composition = remark == null ? "-" : remark.Composition;
        //        var Composition = remark == null ? "-" : (remark.Composition == null ? remark.Name : remark.Composition);

        //        //var Width = remark == null ? "-" : remark.Width;
        //        //var Const = remark == null ? "-" : remark.Const;
        //        //var Yarn = remark == null ? "-" : remark.Yarn;
        //        //var Name = remark == null ? "-" : remark.Name;

        //        if (!exceptionBCNo.Contains(a.BeacukaiNo))
        //        {
        //            if(a.ProductName== remark.Name && a.ProductCode == remark.Code)
        //            {
        //                a.ProductName = remark != null ? string.Concat(/*a.ProductName, " - ",*/ Composition) : a.ProductName;
        //            }
        //        }
        //        else
        //        {
        //            a.ProductName = string.Concat(/*a.ProductName, " - ",*/ Composition + " - "+ a.DeletedAgent);
        //        }
        //    }

        //    //Order by SerialNo
        //    var groupedData = reportData
        //        .GroupBy(x => new { x.BeacukaiNo, x.BeacukaiDate, x.CustomsType })
        //        .Select(g => new
        //        {
        //            g.Key.BeacukaiNo,
        //            g.Key.BeacukaiDate,
        //            g.Key.CustomsType,
        //            // Custom sorting untuk SerialNo string / angka
        //            Items = g.OrderBy(i =>
        //            {
        //                // Jika bisa parse ke int, urutkan sebagai angka
        //                return int.TryParse(i.SerialNo, out int n) ? n : int.MaxValue;
        //            })
        //    .ThenBy(i => i.SerialNo) // jika bukan angka, urutkan alfabet
        //    .ToList()
        //        })
        //        .ToList();

        //    // Flatten data sesuai urutan group dan SerialNo
        //    var flattenedData = groupedData
        //        .SelectMany(g => g.Items)
        //        .ToList();



        //    return flattenedData.AsQueryable();
        //}

        //private async Task<List<GarmentProductViewModel>> GetProductCode(string codes)
        //{
        //    IHttpClientService httpClient = (IHttpClientService)this.serviceProvider.GetService(typeof(IHttpClientService));

        //    var garmentProductionUri = APIEndpoint.Core + $"master/garmentProducts/byCode?code=" + codes;

        //    var httpResponse = httpClient.GetAsync(garmentProductionUri).Result;
        //    if (httpResponse.IsSuccessStatusCode)
        //    {
        //        var content = httpResponse.Content.ReadAsStringAsync().Result;
        //        Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

        //        List<GarmentProductViewModel> viewModel;
        //        if (result.GetValueOrDefault("data") == null)
        //        {
        //            viewModel = new List<GarmentProductViewModel>();
        //        }
        //        else
        //        {
        //            viewModel = JsonConvert.DeserializeObject<List<GarmentProductViewModel>>(result.GetValueOrDefault("data").ToString());

        //        }
        //        return viewModel;
        //    }
        //    else
        //    {
        //        List<GarmentProductViewModel> viewModel = new List<GarmentProductViewModel>();
        //        return viewModel;
        //    }
        //}
        #endregion
        public async Task<IQueryable<ReceiptRawMaterialViewModel>> getQuery(DateTime? dateFrom, DateTime? dateTo)
        {
            var d1 = dateFrom.Value.Date;
            var d2 = dateTo.Value.Date;

            var query = from a in context.Raw_Material_Income_Report
                        where a.URNDate >= d1 && a.URNDate <= d2
                        select new ReceiptRawMaterialViewModel
                        {
                            Amount = a.Price,

                            BeacukaiDate = a.BeacukaiDate.HasValue
            ? a.BeacukaiDate.Value.ToString("dd-MM-yyyy")
            : null,

                            BeacukaiNo = a.BeacukaiNo,
                            Country = a.Country,
                            CustomsType = a.CustomsType,
                            DOCurrencyCode = a.CurrencyCode,
                            HsCode = a.HsCode,
                            ProductCode = a.ProductCode,
                            ProductName = a.ProductName,

                            RecordDate = a.RecordDate.HasValue
            ? a.RecordDate.Value.ToString("dd-MM-yyyy")
            : null,

                            URNNo = a.URNNo,
                            SmallQuantity = a.SmallQuantity,
                            SmallUomUnit = a.SmallUomUnit,
                            SerialNo = a.SeriBarang.ToString(),

                            URNDate = a.URNDate.HasValue
            ? a.URNDate.Value.ToString("dd-MM-yyyy")
            : null,

                            StorageName = a.StorageName,
                            SupplierName = a.SupplierName
                        };

            return query.AsQueryable();

        }
        public async Task<Tuple<List<ReceiptRawMaterialViewModel>, int>> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order)
        {
            var Query = await getQuery(dateFrom, dateTo);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.BeacukaiNo).ThenBy(a => a.BeacukaiDate).ThenBy(c => c.HsCode).ThenBy(d => d.SerialNo).ThenBy(e => e.RecordDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<ReceiptRawMaterialViewModel> pageable = new Pageable<ReceiptRawMaterialViewModel>(Query, page - 1, size);
            List<ReceiptRawMaterialViewModel> Data = pageable.Data.ToList<ReceiptRawMaterialViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public async Task<MemoryStream> GenerateExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var query = (await getQuery(dateFrom, dateTo)).ToList().OrderBy(b => b.BeacukaiNo).ThenBy(a => a.BeacukaiDate).ThenBy(c => c.HsCode).ThenBy(d => d.SerialNo).ThenBy(e => e.RecordDate);

            var result = new DataTable();

            result.Columns.Add("No", typeof(string));
            result.Columns.Add("Tgl Rekam", typeof(string));
            result.Columns.Add("Jenis Dokumen", typeof(string));
            result.Columns.Add("No Bea Cukai", typeof(string));
            result.Columns.Add("Tgl Bea Cukai", typeof(string));
            result.Columns.Add("Kode HS", typeof(string));
            result.Columns.Add("Nomor Seri Barang", typeof(string));
            result.Columns.Add("No Bukti Penerimaan", typeof(string));
            result.Columns.Add("Tgl Bukti Penerimaan", typeof(string));
            result.Columns.Add("Kode Barang", typeof(string));
            result.Columns.Add("Nama Barang", typeof(string));
            result.Columns.Add("Satuan", typeof(string));
            result.Columns.Add("Jumlah Terima", typeof(double));
            result.Columns.Add("Mata Uang", typeof(string));
            result.Columns.Add("Nilai Barang", typeof(double));
            result.Columns.Add("Gudang", typeof(string));
            result.Columns.Add("Penerima Sub Kontrak", typeof(string));
            result.Columns.Add("Negara Asal Barang", typeof(string));

            if (!query.Any())
            {
                // Agar header tetap tergenerate untuk template kosong
                result.Rows.Add(
                    "", "", "", "", "", "",
                    "", "", "", "", "", "",
                    0D, "", 0D, "", "", ""
                );
            }
            else
            {
                var no = 0;

                foreach (var item in query)
                {
                    no++;

                    result.Rows.Add(
                        no.ToString(),
                        item.RecordDate,
                        item.CustomsType,
                        item.BeacukaiNo,
                        item.BeacukaiDate,
                        item.HsCode,
                        item.SerialNo,
                        item.URNNo,
                        item.URNDate,
                        item.ProductCode,
                        item.ProductName,
                        item.SmallUomUnit,
                        item.SmallQuantity,
                        item.DOCurrencyCode,
                        item.Amount,
                        item.StorageName,
                        item.SupplierName,
                        item.Country
                    );
                }
            }

            var initialStream = Excel.CreateExcel(
                new List<KeyValuePair<DataTable, string>>
                {
            new KeyValuePair<DataTable, string>(
                result,
                "Territory"
            )
                },
                true
            );

            initialStream.Position = 0;

            using (var package = new ExcelPackage(initialStream))
            {
                var worksheet = package.Workbook.Worksheets["Territory"];

                if (worksheet != null &&
                    worksheet.Dimension != null &&
                    query.Any())
                {
                    const int firstDataRow = 2;

                    // ProductCode/Kode Barang = kolom J = 10
                    const int firstMergeColumn = 10;

                    // Country/Negara Asal Barang = kolom R = 18
                    const int lastMergeColumn = 18;

                    MergeIdenticalRows(
                        worksheet,
                        firstDataRow,
                        firstMergeColumn,
                        lastMergeColumn
                    );
                }

                var outputStream = new MemoryStream();

                package.SaveAs(outputStream);

                outputStream.Position = 0;

                return outputStream;
            }
        }

        private static void MergeIdenticalRows(ExcelWorksheet worksheet, int firstDataRow, int firstColumn, int lastColumn)
        {
            var lastDataRow = worksheet.Dimension.End.Row;

            if (lastDataRow < firstDataRow)
            {
                return;
            }

            var groupStartRow = firstDataRow;

            for (var currentRow = firstDataRow + 1;
                 currentRow <= lastDataRow + 1;
                 currentRow++)
            {
                var isSameGroup =
                    currentRow <= lastDataRow &&
                    AreRowsEqual(
                        worksheet,
                        currentRow - 1,
                        currentRow,
                        firstColumn,
                        lastColumn
                    );

                if (isSameGroup)
                {
                    continue;
                }

                var groupEndRow = currentRow - 1;

                if (groupEndRow > groupStartRow)
                {
                    MergeRowGroup(
                        worksheet,
                        groupStartRow,
                        groupEndRow,
                        firstColumn,
                        lastColumn
                    );
                }

                groupStartRow = currentRow;
            }
        }

        private static bool AreRowsEqual(ExcelWorksheet worksheet, int firstRow, int secondRow, int firstColumn, int lastColumn)
        {
            for (var column = firstColumn;
                 column <= lastColumn;
                 column++)
            {
                var firstValue = NormalizeCellValue(
                    worksheet.Cells[firstRow, column].Value
                );

                var secondValue = NormalizeCellValue(
                    worksheet.Cells[secondRow, column].Value
                );

                if (!string.Equals(
                        firstValue,
                        secondValue,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static void MergeRowGroup(ExcelWorksheet worksheet, int startRow, int endRow, int firstColumn, int lastColumn)
        {
            for (var column = firstColumn;
                 column <= lastColumn;
                 column++)
            {
                var range = worksheet.Cells[
                    startRow,
                    column,
                    endRow,
                    column
                ];

                range.Merge = true;

                range.Style.VerticalAlignment =
                    ExcelVerticalAlignment.Center;
            }
        }

        private static string NormalizeCellValue(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is string stringValue)
            {
                return stringValue.Trim();
            }

            return Convert
                .ToString(value, CultureInfo.InvariantCulture)
                ?.Trim() ?? string.Empty;
        }

        string formattedDate(string num)
        {
            DateTime date = DateTime.Parse(num);

            string datee = date.ToString("dd MMMM yyyy");


            return datee;
        }
    }
}
