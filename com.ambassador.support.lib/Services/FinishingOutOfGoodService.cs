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
    public class FinishingOutOfGoodService : IFinishingOutOfGoodService
    {
        IProductionDBContext context;
        public FinishingOutOfGoodService(IProductionDBContext _context)
        {
            this.context = _context;
        }
        public IQueryable<FinishingOutOfGoodViewModel> getQuery(DateTime? dateFrom, DateTime? dateTo)
        {
            var d1 = dateFrom.Value.ToString("yyyy-MM-dd");
            var d2 = dateTo.Value.ToString("yyyy-MM-dd");
         
            List<FinishingOutOfGoodViewModel> reportData = new List<FinishingOutOfGoodViewModel>();

            try
            {
                string connectionString = APIEndpoint.ProductionConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "declare @StartDate datetime = '" + d1 + "' declare @EndDate datetime = '" + d2 + "' " +
                        "select a.FinishingOutNo,convert(date,dateadd(hour,7,a.FinishingOutDate)) as FODate,a.ComodityCode,a.ComodityName,b.Quantity,b.UomUnit,c.FinishingInType from GarmentFinishingOuts a  " +
                        "join GarmentFinishingOutItems b on a.[Identity] = b.FinishingOutId join GarmentFinishingIns c on b.FinishingInId=c.[Identity] " +
                        "where a.FinishingTo='GUDANG JADI' and  a.CreatedDate between @StartDate and @EndDate", conn))
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow data in dSet.Tables[0].Rows)
                        {
                            FinishingOutOfGoodViewModel view = new FinishingOutOfGoodViewModel
                            {
                                FinishingOutNo = data["FinishingOutNo"].ToString(),
                                FinishingOutDate = data["FODate"].ToString(),
                                ProductCode = data["ComodityCode"].ToString(),
                                ProductName = data["ComodityName"].ToString(),
                                UomUnit = data["UomUnit"].ToString(),
                                Quantity = data["FinishingInType"].ToString() == "SEWING" ? (double)data["Quantity"] : 0,
                                QuantitySC = data["FinishingInType"].ToString() != "SEWING" ? (double)data["Quantity"] : 0,
                                StorageName = "GUDANG JADI",                                
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
        public Tuple<List<FinishingOutOfGoodViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order)
        {
            var Query = getQuery(dateFrom, dateTo);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.FinishingOutDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<FinishingOutOfGoodViewModel> pageable = new Pageable<FinishingOutOfGoodViewModel>(Query, page - 1, size);
            List<FinishingOutOfGoodViewModel> Data = pageable.Data.ToList<FinishingOutOfGoodViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var Query = getQuery(dateFrom, dateTo);
            Query = Query.OrderBy(b => b.FinishingOutDate);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bukti Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bukti Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Dari Produksi", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Dari Sub Kontrak", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Gudang", DataType = typeof(String) });
           
            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", "", 0, 0, ""); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                int i = 0;
                foreach (var item in Query)
                {
                    i++;
                    result.Rows.Add(i.ToString(),item.FinishingOutNo,formattedDate(item.FinishingOutDate),item.ProductCode,
                                    item.ProductName,item.UomUnit,item.Quantity,item.QuantitySC,item.StorageName);
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
