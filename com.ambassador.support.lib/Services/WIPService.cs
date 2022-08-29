using com.ambassador.support.lib.Helpers;
using com.ambassador.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace com.ambassador.support.lib.Services
{
    public class WIPService
    {
		SupportDbContext context;
		public WIPService(SupportDbContext _context)
		{
			this.context = _context;
		}
		public IQueryable<WIPViewModel> GetWIPReport(DateTime? date, int offset)
		{
			DateTime Date = date == null ? new DateTime(1970, 1, 1) : (DateTime)date;
			string Dates = Date.ToString("yyyy-MM-dd");
			List<WIPViewModel> wipData = new List<WIPViewModel>();
			try
			{
				string connectionString = APIEndpoint.ConnectionString;
				using (SqlConnection conn =
					new SqlConnection(connectionString))
				{
					conn.Open();
					using (SqlCommand cmd = new SqlCommand(" select * from ( select Kode, komoditi,Satuan,sum(WIP) WIP from (  SELECT   c.ComodityCode Kode, ComodityName komoditi ,'PCS' Satuan,sum(JumlahCutting) WIP from Factcutting j join comodity c on j.ComodityCode=c.ComodityCode where    ProcessDate <='" + Dates + "'  group by ro,c.ComodityCode,ComodityName union all  SELECT   c.ComodityCode Kode, ComodityName komoditi ,'PCS' Satuan,sum(-JumlahFinish) WIP from FactFinishing j join comodity c on j.ComodityCode=c.ComodityCode where   ProcessDate <='" + Dates + "'  group by ro,c.ComodityCode,ComodityName ) as data  where wip<>0 group by Kode,komoditi,Satuan ) as data where WIP>0  union all   select itemcode,ArticleNo,satuan,SUM(WIP) WIP from( SELECT   ItemCode, ArticleNo ,'MTR' Satuan,round(sum(JumlahPrepare),3) WIP from factprepare j where   ProcessDate <='" + Dates + "'  group by ro,ItemCode,ArticleNo  ) d2 where WIP>0.01 group by ItemCode,ArticleNo,satuan ", conn))
					{
						SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
						DataSet dSet = new DataSet();
						dataAdapter.Fill(dSet);
						foreach (DataRow data in dSet.Tables[0].Rows)
						{
							WIPViewModel view = new WIPViewModel
							{
								Kode = data["Kode"].ToString(),
								Comodity = data["komoditi"].ToString(),
								UnitQtyName=data["Satuan"].ToString(),
								WIP = String.Format("{0:n}", (double)data["WIP"])
							};
							wipData.Add(view);
						}
					}
					conn.Close();
				}

			}
			catch (SqlException ex)
			{
				 
			}


			return wipData.AsQueryable();
		}

        public MemoryStream GenerateExcel(DateTime? date, int offset)
        {
            var Query = GetWIPReport(date, offset);
            Query = Query.OrderBy(b => b.Kode).ThenBy(b=>b.Comodity);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(int) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "WIP", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else {
                var index = 1;
                foreach (var item in Query)
                {
                    result.Rows.Add((index), item.Kode, item.Comodity, item.UnitQtyName, item.WIP);
                    index++;
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

    }
}
