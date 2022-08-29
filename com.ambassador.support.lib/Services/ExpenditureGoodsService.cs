using com.ambassador.support.lib.Helpers;
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
    public class ExpenditureGoodsService
    {
        SupportDbContext context;
        public ExpenditureGoodsService(SupportDbContext _context)
        {
            this.context = _context;
        }

        public IQueryable<ExpenditureGoodViewModel> getQuery(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            DateTime d1 = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime d2 = dateTo == null ? DateTime.Now : (DateTime)dateTo;


            string DateFrom = d1.ToString("yyyy-MM-dd");
            string DateTo = d2.ToString("yyyy-MM-dd");
            List<ExpenditureGoodViewModel> reportData = new List<ExpenditureGoodViewModel>();

            try
            {
                string connectionString = APIEndpoint.LocalConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "declare @StartDate datetime = '" + DateFrom + "' declare @EndDate datetime = '" + DateTo + "' " +
                        "select a.ExpenditureGoodId, a.RO, a.Article, a.UnitCode, a.BuyerContract, e.ExpenditureTypeName, a.Description, c.ComodityName, d.SizeNumber, c.ComodityCode, b.Description as Desb, b.Qty from ExpenditureGood a " +
                        "join ExpenditureGoodDetail b on a.ExpenditureGoodId = b.ExpenditureGoodId " +
                        "join Comodity c on b.ComodityId = c.ComodityID " +
                        "join Sizes d on b.SizeId = d.SizeId join ExpenditureType e on a.ExpenditureType = e.ExpenditureTypeId " +
                        "where a.ProcessDate between @StartDate and @EndDate", conn))

                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow data in dSet.Tables[0].Rows)
                        {
                            ExpenditureGoodViewModel view = new ExpenditureGoodViewModel
                            {
                                ExpenditureGoodId = data["ExpenditureGoodId"].ToString(),
                                RO = data["RO"].ToString(),
                                Article = data["Article"].ToString(),
                                UnitCode = (int)data["UnitCode"] == 1 ? "KONF 2A/EX. K1" : (int)data["UnitCode"] == 2 ? "KONF 2B/EX. K2" : (int)data["UnitCode"] == 3 ? "KONF 1A/EX. K3" : (int)data["UnitCode"] == 4 ? "KONF 2C/EX. K4" : (int)data["UnitCode"] == 5 ? "KONF 1B/EX. K2D" : "GUDANG PUSAT",
                                BuyerContract = data["BuyerContract"].ToString(),
                                ExpenditureTypeName = data["ExpenditureTypeName"].ToString(),
                                Description = data["Description"].ToString(),
                                ComodityName = data["ComodityName"].ToString(),
                                ComodityCode = data["ComodityCode"].ToString(),
                                SizeNumber = data["SizeNumber"].ToString(),
                                Descriptionb = data["Desb"].ToString(),
                                Qty = String.Format("{0:n}", (double)data["Qty"])
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

        public Tuple<List<ExpenditureGoodViewModel>, int> GetReportExGood(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = getQuery(dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.ExpenditureGoodId);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }


            Pageable<ExpenditureGoodViewModel> pageable = new Pageable<ExpenditureGoodViewModel>(Query, page - 1, size);
            List<ExpenditureGoodViewModel> Data = pageable.Data.ToList<ExpenditureGoodViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcelExGood(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = getQuery(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.ExpenditureGoodId);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Unit", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Buyer Contract", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Deskripsi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Komoditi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Komoditi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Ukuran", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Warna", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah", DataType = typeof(Double) });

            if (Query.ToArray().Count() == 0)
            {
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", 0); // to allow column name to be generated properly for empty data as template
            }
            else
            {
                int i = 0;
                foreach (var item in Query)
                {
                    i++;
                    result.Rows.Add(i.ToString(), item.ExpenditureGoodId, item.RO, item.Article, item.UnitCode, "-", item.BuyerContract, item.ExpenditureTypeName, item.Description, item.ComodityCode ,item.ComodityName, item.SizeNumber, item.Descriptionb, item.Qty);
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);

        }
    }
}
