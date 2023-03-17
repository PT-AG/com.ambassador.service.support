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
    public class WIPInSubconService : IWIPInSubconService
    {
        IPurchasingDBContext context;
        public WIPInSubconService(IPurchasingDBContext _context)
        {
            this.context = _context;
        }
        public IQueryable<WIPInSubconViewModel> getQuery(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset)
        {
            DateTimeOffset d1 = dateFrom.Value.AddHours(offset);
            DateTimeOffset d2 = dateTo.Value.AddHours(offset);
            var customCategory = "Fasilitas";
            //string DateFrom = d1.ToString("yyyy-MM-dd");
            //string DateTo = d2.ToString("yyyy-MM-dd");

            List<WIPInSubconViewModel> reportData = new List<WIPInSubconViewModel>();
            try
            {
                string connectionString = APIEndpoint.PurchasingConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "declare @StartDate datetimeoffset = '" + d1 + "' declare @EndDate datetimeoffset = '" + d2 + "' " +
                        "SELECT a.UENNo,convert(date,dateadd(hour,7,a.ExpenditureDate)) as 'Tanggal Keluar',b.ProductCode,b.ProductName,b.UomUnit,b.Quantity,c.SupplierReceiptName FROM GarmentUnitExpenditureNotes a " +
                        "join GarmentUnitExpenditureNoteItems b on a.Id = b.UENId " +
                        "join GarmentUnitDeliveryOrders c on a.UnitDOId = c.Id " +
                        "join GarmentUnitDeliveryOrderItems d on c.Id = d.UnitDOId " +
                        "join GarmentUnitReceiptNoteItems e on d.URNItemId = e.Id " +
                        "where a.ExpenditureType = 'SUBCON' and DATEADD(HOUR,7,a.ExpenditureDate) between @StartDate and @EndDate and e.CustomsCategory='" + customCategory +"' " +
                        "and a.IsDeleted=0 and b.IsDeleted=0 and c.IsDeleted=0 and d.IsDeleted=0 and e.IsDeleted=0 ", conn))

                    {
                        
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow data in dSet.Tables[0].Rows)
                        {
                            WIPInSubconViewModel view = new WIPInSubconViewModel
                            {
                                UENNo = data["UENNo"].ToString(),
                                ExpenditureDate = data["Tanggal Keluar"].ToString(),
                                ProductCode = data["ProductCode"].ToString(),
                                ProductName = data["ProductName"].ToString(),
                                UomUnit = data["UomUnit"].ToString(),
                                QuantitySubcon = (double)data["Quantity"],
                                SupplierName = data["SupplierReceiptName"].ToString(),
                            };

                            reportData.Add(view);
                        }
                    }
                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }

            return reportData.AsQueryable();
        }
        public Tuple<List<WIPInSubconViewModel>, int> GetReport(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string Order, int offset)
        {
            var Query = getQuery(dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.ExpenditureDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }


            Pageable<WIPInSubconViewModel> pageable = new Pageable<WIPInSubconViewModel>(Query, page - 1, size);
            List<WIPInSubconViewModel> Data = pageable.Data.ToList<WIPInSubconViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset)
        {
            var Query = getQuery(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.ExpenditureDate);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Jml Digunakan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "DiSubKontrakan", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerima SubKontrak", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", "", 0, ""); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                int i = 0;
                foreach (var item in Query)
                {
                    i++;
                    result.Rows.Add(i.ToString(), item.UENNo, formattedDate(item.ExpenditureDate), item.ProductCode, item.ProductName, item.UomUnit, item.QuantitySubcon, "-");
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
