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
    public class MachineMutationService
    {
		SupportDbContext context;
		public MachineMutationService(SupportDbContext _context)
		{
			this.context = _context;
		}


		public IQueryable<FinishedGoodViewModel> GetMachineMutationReport(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime Datefrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime Dateto = dateTo == null ? new DateTime(1970, 1, 1) : (DateTime)dateTo;
			string DateFrom = Datefrom.ToString("yyyy-MM-dd");
			string DateTo = Dateto.ToString("yyyy-MM-dd");
			List<FinishedGoodViewModel> machine = new List<FinishedGoodViewModel>();
			try
			{
				string connectionString = APIEndpoint.ConnectionString;
				using (SqlConnection conn =
					new SqlConnection(connectionString))
				{
					conn.Open();
					using (SqlCommand cmd = new SqlCommand("declare @TglLaporan datetime='"+DateFrom+"'"+
														   "declare @Tgl2 datetime = '" + DateTo + "'" +
															"select B.KodeBarang, B.NamaBarang, B.Sat,convert(float, B.SaldoAwal) as SaldoAwal,convert(float, B.Pemasukan) as Pemasukan,convert(float, B.Pengeluaran) as Pengeluaran,convert(float,B.Penyesuaian) as Penyesuaian,convert(float, (B.SaldoAwal + (B.Pemasukan - B.Pengeluaran))) as SaldoBuku,convert(float, B.StockOpname) as StockOpname, convert(float,B.Selisih) as Selisih, B.ActivateDate, B.CloseDate from " +
															"(select A.KodeBarang, A.NamaBarang, A.Sat,(A.SaldoAwal + (select isnull(SUM(TransactionAmount), 0) from dbo.MachineMutation where MachineID = A.KodeBarang and TransactionType = 'IN' and TransactionDate < @TglLaporan) - (select isnull(SUM(TransactionAmount), 1) from dbo.MachineMutation where MachineID = A.KodeBarang and TransactionType = 'OUT' and TransactionDate < @TglLaporan)) as SaldoAwal,	(select isnull(SUM(TransactionAmount), 0) from dbo.MachineMutation where MachineID = A.KodeBarang and TransactionType = 'IN' and TransactionDate>= @TglLaporan and TransactionDate<DATEADD(DAY,1,@Tgl2)) as Pemasukan, "+
															"(select isnull(SUM(TransactionAmount), 0) from dbo.MachineMutation where MachineID = A.KodeBarang and TransactionType = 'OUT' and TransactionDate>= @TglLaporan and TransactionDate<DATEADD(DAY,1,@Tgl2)) as Pengeluaran, "+
															"A.Penyesuaian, A.StockOpname, A.Selisih, A.ActivateDate, A.CloseDate from view_machinereport A where A.ActivateDate <= @Tgl2)B "+
															"where B.SaldoAwal > 0 or(B.SaldoAwal + (B.Pemasukan - B.Pengeluaran)) > 0 order by B.KodeBarang  ", conn))
					{
						SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
						DataSet dSet = new DataSet();
						dataAdapter.Fill(dSet);
						foreach (DataRow data in dSet.Tables[0].Rows)
						{
							FinishedGoodViewModel view = new FinishedGoodViewModel
							{
								KodeBarang = data["KodeBarang"].ToString(),
								NamaBarang = data["NamaBarang"].ToString(),
								Pemasukan = String.Format("{0:n}", (double)data["Pemasukan"]),
								Pengeluaran = String.Format("{0:n}", (double)data["Pengeluaran"]),
								SaldoAwal = String.Format("{0:n}", (double)data["SaldoAwal"]),
								SaldoBuku = String.Format("{0:n}", (double)data["SaldoBuku"]),
								UnitQtyName = data["Sat"].ToString(),
								Penyesuaian = String.Format("{0:n}", (double)data["Penyesuaian"]),
								StockOpname = String.Format("{0:n}", (double)data["StockOpname"]),
								Selisih = String.Format("{0:n}", (double)data["Selisih"])
							};
							machine.Add(view);
						}
					}
					conn.Close();
				}
			}
			catch (SqlException ex)
			{
				//Log exception
				//Display Error message
			}

			return machine.AsQueryable();
		}
        public Tuple<List<FinishedGoodViewModel>, int> GetMachineMutationReportData(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset)
        {
            var Query = GetMachineMutationReport(dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.KodeBarang);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }


            Pageable<FinishedGoodViewModel> pageable = new Pageable<FinishedGoodViewModel>(Query, page - 1, size);
            List<FinishedGoodViewModel> Data = pageable.Data.ToList<FinishedGoodViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetMachineMutationReport(dateFrom, dateTo, offset);
            Query = Query.OrderBy(b => b.KodeBarang);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sat", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pemasukan", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Penyesuaian", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Saldo Buku", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Stock Opname", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", ""); // to allow column name to be generated properly for empty data as template
            else
                foreach (var item in Query)
                {
                    result.Rows.Add((item.KodeBarang), item.NamaBarang, item.UnitQtyName, item.SaldoAwal, item.Pemasukan, item.Pengeluaran, item.Penyesuaian, item.SaldoBuku, item.StockOpname, item.Selisih);
                }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
    }
}
