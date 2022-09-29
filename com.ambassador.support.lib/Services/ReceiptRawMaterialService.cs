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
    public class ReceiptRawMaterialService : IReceiptRawMaterialService
    {
        PurchasingDBContext context;
        public ReceiptRawMaterialService(PurchasingDBContext _context)
        {
            this.context = _context;
        }
        public IQueryable<ReceiptRawMaterialViewModel> getQuery(DateTime? dateFrom, DateTime? dateTo)
        {
            var d1 = dateFrom.Value.ToString("yyyy-MM-dd");
            var d2 = dateTo.Value.ToString("yyyy-MM-dd");
         
            List<ReceiptRawMaterialViewModel> reportData = new List<ReceiptRawMaterialViewModel>();

            try
            {
                string connectionString = APIEndpoint.PurchasingConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "declare @StartDate datetime = '" + d1 + "' declare @EndDate datetime = '" + d2 + "' " +
                        "select e.CustomsType,e.BeacukaiNo,convert(date,dateadd(hour,7,e.BeacukaiDate)) as BCDate,f.URNNo,convert(date,dateadd(hour,7,f.ReceiptDate)) as URNDate,g.ProductCode,g.ProductName," +
                        "g.SmallQuantity,g.SmallUomUnit,a.DOCurrencyCode,cast((g.PricePerDealUnit * g.SmallQuantity * g.DOCurrencyRate) as decimal(18,2)) as Amount,a.SupplierName,a.Country " +
                        "from GarmentDeliveryOrders a join GarmentDeliveryOrderItems b on a.id=b.GarmentDOId join GarmentDeliveryOrderDetails c on b.id=c.GarmentDOItemId " +
                        "join GarmentBeacukaiItems d on d.GarmentDOId=a.id join GarmentBeacukais e on e.id=d.BeacukaiId " +
                        "join GarmentUnitReceiptNotes f on a.id=f.DOId join GarmentUnitReceiptNoteItems g on f.id=g.URNId " +
                        "where e.BeacukaiDate between @StartDate and @EndDate", conn))

                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow data in dSet.Tables[0].Rows)
                        {
                            ReceiptRawMaterialViewModel view = new ReceiptRawMaterialViewModel
                            {
                                CustomsType = data["CustomsType"].ToString(),
                                BeacukaiNo = data["BeacukaiNo"].ToString(),
                                BeacukaiDate = data["BCDate"].ToString(),
                                SerialNo = "-",
                                URNNo = data["URNNo"].ToString(),
                                URNDate = data["URNDate"].ToString(),
                                ProductCode = data["ProductCode"].ToString(),
                                ProductName = data["ProductName"].ToString(),
                                SmallUomUnit = data["SmallUomUnit"].ToString(),
                                SmallQuantity = (decimal)data["SmallQuantity"],
                                DOCurrencyCode = data["DOCurrencyCode"].ToString(),
                                Amount = (decimal)data["Amount"],
                                StorageName = "GUDANG AG2",
                                SupplierName = data["SupplierName"].ToString(),
                                Country = data["Country"].ToString()
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
        public Tuple<List<ReceiptRawMaterialViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order)
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

            Pageable<ReceiptRawMaterialViewModel> pageable = new Pageable<ReceiptRawMaterialViewModel>(Query, page - 1, size);
            List<ReceiptRawMaterialViewModel> Data = pageable.Data.ToList<ReceiptRawMaterialViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var Query = getQuery(dateFrom, dateTo);
            Query = Query.OrderBy(b => b.BeacukaiDate);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis Dokumen", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bea Cukai", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor Seri Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bukti Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl Bukti Penerimaan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Terima", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nilai Barang", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Gudang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penerima Sub Kontrak", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Negara Asal Barang", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", 0, "", 0, "", "", ""); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                int i = 0;
                foreach (var item in Query)
                {
                    i++;
                    result.Rows.Add(i.ToString(),item.CustomsType,item.BeacukaiNo,formattedDate(item.BeacukaiDate),item.SerialNo,item.URNNo,formattedDate(item.URNDate),item.ProductCode,
                                    item.ProductName,item.SmallUomUnit,item.SmallQuantity,item.DOCurrencyCode,item.Amount,item.StorageName,item.SupplierName,item.Country);
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
