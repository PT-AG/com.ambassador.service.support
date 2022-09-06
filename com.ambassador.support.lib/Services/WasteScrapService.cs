using com.ambassador.support.lib.Helpers;
using com.ambassador.support.lib.Interfaces;
using com.ambassador.support.lib.ViewModel;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace com.ambassador.support.lib.Services
{
    public class WasteScrapService : IWasteScrapService
    {
        SupportDbContext context;
        public WasteScrapService(SupportDbContext _context)
        {
            this.context = _context;
        }
        public IQueryable<WasteScrapViewModel> getQuery(DateTime? dateFrom, DateTime? dateTo)
        {
            var d1 = dateFrom.Value.ToString("yyyy-MM-dd");
            var d2 = dateTo.Value.ToString("yyyy-MM-dd");
         
            List<WasteScrapViewModel> reportData = new List<WasteScrapViewModel>();

            try
            {
                string connectionString = APIEndpoint.ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "declare @StartDate datetime = '" + d1 + "' declare @EndDate datetime = '" + d2 + "' " +
                        "select c.Code,b.ScrapClassificationName,b.Quantity,b.UomUnit from GarmentScrapTransactions a join GarmentScrapTransactionItems b " +
                        "on a.[Identity] = b.ScrapTransactionId join GarmentScrapClassifications c on b.ScrapClassificationId = c.[Identity] " +
                        "where a.CreatedDate between @StartDate and @EndDate", conn))

                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow data in dSet.Tables[0].Rows)
                        {
                            WasteScrapViewModel view = new WasteScrapViewModel
                            {
                                BeacukaiNo = "-", //data["BeacukaiNo"].ToString(),
                                BeacukaiDate = "-",//data["BCDate"].ToString(),
                                ProductCode = data["Code"].ToString(),
                                ProductName = data["ScrapClassificationName"].ToString(),
                                UomUnit = data["UomUnit"].ToString(),
                                Quantity = (decimal)data["Quantity"],
                                Amount = 0//(decimal)data["Amount"]                             
                            };
                            reportData.Add(view);
                        }
                    }
                    conn.Close();
                }
            }
            catch (SqlException ex)
            { 
            }
            return reportData.AsQueryable();
        }
        public Tuple<List<WasteScrapViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order)
        {
            var Query = getQuery(dateFrom, dateTo);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.BeacukaiDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<WasteScrapViewModel> pageable = new Pageable<WasteScrapViewModel>(Query, page - 1, size);
            List<WasteScrapViewModel> Data = pageable.Data.ToList<WasteScrapViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var Query = getQuery(dateFrom, dateTo);
            Query = Query.OrderBy(b => b.BeacukaiDate);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai", DataType = typeof(double) });
            
            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", "", 0, 0); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                int i = 0;
                foreach (var item in Query)
                {
                    i++;
                    result.Rows.Add(i.ToString(),item.BeacukaiNo,formattedDate(item.BeacukaiDate),item.ProductCode,item.ProductName,item.UomUnit,item.Quantity,item.Amount);
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);

        }

        string formattedDate(string num)
        {
            DateTime date = DateTime.Parse(num);

            string datee = date.ToString("dd MMMM yyyy");
            
            return datee;
        }
    }
}
