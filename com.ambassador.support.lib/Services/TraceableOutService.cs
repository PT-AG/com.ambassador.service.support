using com.ambassador.support.lib.Helpers;
using com.ambassador.support.lib.Models;
using com.ambassador.support.lib.ViewModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace com.ambassador.support.lib.Services
{
    public class TraceableOutService
    {   
        ILocalDbProductionDBContext dBContext;

        private readonly DbSet<ViewFactBeacukai> dbSet;
        public TraceableOutService(ILocalDbProductionDBContext _dBContext, SupportDbContext _context)
        {
            this.dBContext = _dBContext;
        }

        public IQueryable<TraceableOutViewModel> getQueryTraceableOut(string bcno) {

            List<TraceableOutViewModel> reportData = new List<TraceableOutViewModel>();
            //List<TraceableOutDetailViewModel> reportDataDetail = new List<TraceableOutDetailViewModel>();

            try
            {
                string cmdtraceableout = "select a.ExpenditureDate as TGL_KELUAR,b.ExpenditureGoodId as NO_BON,d.ComodityName as NAMA_BARANG,sum(c.Qty) as JUMLAH,a.ExpenditureNo as"+
                                         " INVOICE,a.BuyerName as BUYER,a.BCType as TIPE_BC,a.BCNo as NOMOR_PEB,a.BCDate as TGL_BC,b.RO as NOMOR_RO, q.UnitQtyName as SATUAN from DL_Supports.dbo.BEACUKAI_ADDED " +
                                         " a join Production.dbo.ExpenditureGood b on a.ExpenditureNo = b.Invoice join Production.dbo.ExpenditureGoodDetail c on b.ExpenditureGoodId = c.ExpenditureGoodId"+
                                         " join DL_Inventories.dbo.UNIT_QUANTITY q on c.UnitQty=q.UnitQtyCode"+
                                         " join Production.dbo.Comodity d on c.ComodityId = d.ComodityID where a.BCNo ='" +bcno+"' "+
                                         " group by a.ExpenditureDate,b.ExpenditureGoodId,d.ComodityName, a.ExpenditureNo,a.BuyerName,a.BCType,a.BCNo,a.BCDate,b.RO, q.UnitQtyName";

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("key", bcno));

                var data = dBContext.ExecuteReader2(cmdtraceableout);

                while (data.Read())
                {
                    reportData.Add(new TraceableOutViewModel
                    {
                        
                      ExpenditureDate = data["TGL_KELUAR"].ToString(),
                      ExpenditureGoodId = data["NO_BON"].ToString(),
                      ComodityName = data["NAMA_BARANG"].ToString(),
                      Qty = (double)data["JUMLAH"],
                      ExpenditureNo = data["INVOICE"].ToString(),
                        BuyerName = data["BUYER"].ToString(),
                        BCType = data["TIPE_BC"].ToString(),
                        BCNo = data["NOMOR_PEB"].ToString(),
                        BCDate = data["TGL_BC"].ToString(),
                        RO = data["NOMOR_RO"].ToString(),
                        UnitQtyName = data["SATUAN"].ToString()
                    });
             
                }
                data.Close();
            }
            catch (Exception e)
            {
                
            }
            
           return reportData.AsQueryable();
           
            //return Tuple.Create(reportData, reportDataDetail);
        }

        public MemoryStream GetTraceableOutExcel(string bcno)
        {
            var index2 = 0;
            var query = getQueryTraceableOut(bcno);
            var satuan = "-";

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "no", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "tanggal keluar", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "no. bon", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "nama barang", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "jumlah barang", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "satuan", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "no invoice", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "buyer", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "jenis", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "nomor", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "tanggal", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(string) });

            DataTable result2 = new DataTable();
            result2.Columns.Add(new DataColumn() { ColumnName = "Nomor22", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Nama Barang 2", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Jumlah Pemakaian", DataType = typeof(Double) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Satuan 2", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "jumlah budget", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Jenis BC", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "No.", DataType = typeof(String) });
            result2.Columns.Add(new DataColumn() { ColumnName = "Tanggal 2", DataType = typeof(String) });

            if (query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", 0, "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
            {

                var index = 0;
                foreach (var item in query)
                {
                    index++;
                    result.Rows.Add(index,formattedDate(item.ExpenditureDate), item.ExpenditureGoodId, item.ComodityName, item.Qty, item.UnitQtyName, item.ExpenditureNo, item.BuyerName, item.BCType, item.BCNo, formattedDate(item.BCDate), item.RO);


                }


            }

            
            foreach (var detail in query)
            {
                var querydetail = getQueryTraceableOutDetail(detail.RO);




                if (querydetail.ToArray().Count() == 0)
                    result2.Rows.Add("", "", "", "", 0, "", "", "", ""); // to allow column name to be generated properly for empty data as template
                else
                {

                  
                    foreach (var item in querydetail)
                    {
                        index2++;
                        result2.Rows.Add(index2, item.DestinationJob, item.ItemCode, item.ItemName, item.SmallestQuantity, item.UnitQtyName, item.BCType, item.BCNo,  formattedDate(item.BCDate));

                    }
                }

            }


            ExcelPackage package = new ExcelPackage();

            

            var sheet = package.Workbook.Worksheets.Add("Data");


            var Tittle = new string[] {"Monitoring Pengeluaran Hasil Produksi"};
            var headers = new string[] { "No", "Tanggal Keluar", "No BON", "Nama Barang", "Jumlah Barang", "Satuan", "No. Invoice", "Buyer", "Dokumen", "Dokumen1", "Dokumen2","RO" };
            var subHeaders = new string[] { "Jenis", "Nomor", "Tanggal" };

            //for (int i = 0; i < headers.Length; i++)
            //{
            //    result.Columns.Add(new DataColumn() { ColumnName = headers[i], DataType = typeof(string) });
            //}



            sheet.Cells["A5"].LoadFromDataTable(result, false, OfficeOpenXml.Table.TableStyles.Light16);

            sheet.Cells["A2"].Value = Tittle[0];
            sheet.Cells["A2:L2"].Merge = true;

            sheet.Cells["I3"].Value = headers[8];
            sheet.Cells["I3:K3"].Merge = true;

            foreach (var i in Enumerable.Range(0, 8))
            {
                var col = (char)('A' + i);
                sheet.Cells[$"{col}3"].Value = headers[i];
                sheet.Cells[$"{col}3:{col}4"].Merge = true;
            }

            foreach (var i in Enumerable.Range(0, 3))
            {
                var col = (char)('I' + i);
                sheet.Cells[$"{col}4"].Value = subHeaders[i];
            }

            foreach (var i in Enumerable.Range(0, 1))
            {
                var col = (char)('L' + i);
                sheet.Cells[$"{col}3"].Value = headers[i+11];
                sheet.Cells[$"{col}3:{col}4"].Merge = true;
            }
            sheet.Cells["A1:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A1:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells["A1:L4"].Style.Font.Bold = true;
            //-----------


            var countdata = query.Count();

            sheet.Cells[$"A{countdata + 11}"].LoadFromDataTable(result2, false, OfficeOpenXml.Table.TableStyles.Light16);

            var Tittle1 = new string[] { "PERINCIAN PEMAKAIAN BAHAN BAKU DAN BAHAN PENOLONG" };
            var headers1 = new string[] { "No", "No. RO", "Kode Barang", "Nama Barang", "Jumlah Pemakaian", "Satuan",  "Dokumen", "Dokumen1", "Dokumen2" };
            var subHeaders1 = new string[] { "Jenis", "Nomor", "Tanggal" };

            sheet.Cells[$"A{countdata + 8}"].Value = Tittle1[0];
            sheet.Cells[$"A{countdata + 8}:I{countdata + 8}"].Merge = true;

            sheet.Cells[$"G{countdata + 9}"].Value = headers1[6];
            sheet.Cells[$"G{countdata + 9}:I{countdata + 9}"].Merge = true;

            foreach (var i in Enumerable.Range(0, 6))
            {
                var col = (char)('A' + i);
                sheet.Cells[$"{col}{countdata + 9}"].Value = headers1[i];
                sheet.Cells[$"{col}{countdata + 9}:{col}{countdata + 10}"].Merge = true;
            }

            foreach (var i in Enumerable.Range(0, 3))
            {
                var col = (char)('G' + i);
                sheet.Cells[$"{col}{countdata + 10}"].Value = subHeaders1[i];
            }

            
            sheet.Cells[$"A{countdata + 8}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[$"A{countdata + 8}:I{countdata + 10}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[$"A{countdata + 8}:I{countdata + 10}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[$"A{countdata + 8}:I{countdata + 10}"].Style.Font.Bold = true;

            var widths = new int[] { 5, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20};
            foreach (var i in Enumerable.Range(0, headers.Length))
            {
                sheet.Column(i + 1).Width = widths[i];
            }

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;


            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }


        public IQueryable<TraceableOutDetailViewModel> getQueryTraceableOutDetail(string bcno)
        {
            

            List<TraceableOutDetailViewModel> reportData2 = new List<TraceableOutDetailViewModel>();

            //try
            //{
                string cmdtraceableoutdetail = "select c.DestinationJob as 'RO_JOB',d.ItemCode as 'KODE_BARANG',i.ItemName as 'NAMA_BARANG', sum(d.SmallestQuantity) as 'JUMLAH_PAKAI',"+
                                               " h.UnitQtyName as SATUAN,g.BCNo as 'NOMOR_BC',g.BCType as 'TIPE_BC',g.BCDate as 'TANGGAL_BC' from DL_Supports.dbo.DETAIL_DELIVERY_ORDER a"+
                                               " join DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS b  on a.DetailDOId = b.DetailDOId join DL_Supports.dbo.DELIVERY_ORDER c  on a.DeliveryOrderNo = c.DeliveryOrderNo"+
                                               " join DL_Supports.dbo.DO_ITEM d on a.ReferenceNo = d.DOItemNo join DL_Supports.dbo.DETAIL_SHIPPING_ORDER e on d.poid = e.POId  join DL_Supports.dbo.SHIPPING_ORDER f"+  
                                               " on e.ShippingOrderId = f.ShippingOrderId join DL_Supports.dbo.BEACUKAI g on f.BCId = g.BCId join DL_Supports.dbo.UNIT_QUANTITY h on d.SmallestUnitQtyCode = h.UnitQtyCode"+
                                               " join DL_Supports.dbo.ITEM_CATEGORY i on d.ItemCode = i.ItemCode  where c.DestinationJob = '"+bcno+"' "+
                                               " group by c.DestinationJob, d.ItemCode, i.ItemName, h.UnitQtyName, g.BCNo, g.BCType, g.BCDate";

                //List<SqlParameter> parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("@key", bcno));

                var data = dBContext.ExecuteReader2(cmdtraceableoutdetail);

                while (data.Read())
                {
                    reportData2.Add(new TraceableOutDetailViewModel
                    {
                        DestinationJob = data["RO_JOB"].ToString(),
                        ItemCode = data["KODE_BARANG"].ToString(),
                        ItemName = data["NAMA_BARANG"].ToString(),
                        SmallestQuantity = (double)data["JUMLAH_PAKAI"],
                        UnitQtyName = data["SATUAN"].ToString(),
                        BCNo = data["NOMOR_BC"].ToString(),
                        BCType = data["TIPE_BC"].ToString(),
                        BCDate = data["TANGGAL_BC"].ToString()
                    });
                }
            data.Close();

            //}
            //catch (Exception e)
            //{

            //}
            return reportData2.AsQueryable();
        }


        //public MemoryStream GetTraceableOutDetailExcel(string bcno)
        //{
        //    var Query = getQueryTraceableOutDetail(bcno);
        //    Query.OrderBy(x => x.BCNo);
        //    DataTable result = new DataTable();
        //    result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Pemakaian", DataType = typeof(Double) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Jenis BC", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "No.", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(Double) });

        //    if (Query.ToArray().Count() == 0)
        //        result.Rows.Add("", "", "", "", 0, "", "", "", ""); // to allow column name to be generated properly for empty data as template
        //    else
        //    {

        //        var index = 0;
        //        foreach (var item in Query)
        //        {
        //            index++;
        //            result.Rows.Add(index, item.DestinationJob, item.ItemCode, item.ItemName, item.SmallestQuantity, item.UnitQtyName, item.BCNo, item.BCType,  item.BCDate);

        //        }
        //    }
        //    return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        //}


        //public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        //{
        //    string Key = "";
        //    string cmd = "";

        //    List<TraceableOutViewModel> data = new List<TraceableOutViewModel>();

        //    List<SqlParameter> parameters = new List<SqlParameter>();
        //    Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
        //    foreach (var f in FilterDictionary)
        //    {
        //        Key = f.Value;

        //    }
            
        //        cmd = "SELECT Distinct BCNo  FROM DL_Supports.dbo.BEACUKAI_ADDED where BCNo LIKE @key ";
            
        //    parameters.Add(new SqlParameter("key", "%" + Keyword + "%"));
        //    var reader = dBContext.ExecuteReader(cmd, parameters);

            
        //    while (reader.Read())
        //    {
        //        data.Add(new TraceableOutViewModel
        //        {
        //            BCNo = reader["BCNo"].ToString(),
        //        });

                

        //    }

        //    Pageable< TraceableOutViewModel> pageable = new Pageable<TraceableOutViewModel>(data, Page - 1, Size);
        //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
        //    List<TraceableOutViewModel> Data = pageable.Data.ToList();
        //    int TotalData = pageable.TotalCount;

        //    List<object> ListData = new List<object>();
          
        //            ListData.AddRange(Data.Select(s => new
        //            {
        //                s.BCNo
        //            }));
                   

        //    return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
        //}

        //public class ReadResponse<TModel>
        //{
        //    public List<TModel> Data { get; set; }
        //    public int TotalData { get; set; }
        //    public Dictionary<string, string> Order { get; set; }

        //    public ReadResponse(List<TModel> Data, int TotalData, Dictionary<string, string> Order)
        //    {
        //        this.Data = Data;
        //        this.TotalData = TotalData;
        //        this.Order = Order;
        //    }
        //}

        string formattedDate(string num)
        {

            return num.Substring(0, num.Length - 12);
        }
    }
}
