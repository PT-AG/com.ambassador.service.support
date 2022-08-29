using com.ambassador.support.lib.Helpers;
using com.ambassador.support.lib.Models;
using com.ambassador.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace com.ambassador.support.lib.Services
{
    public class ScrapService
    {
		SupportDbContext context;
		public ScrapService(SupportDbContext _context)
		{
			this.context = _context;
		}


		public IQueryable<ScrapViewModel> GetScrapReport(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime Datefrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime Dateto = dateTo == null ? new DateTime(1970, 1, 1) : (DateTime)dateTo;
			string DateFrom = Datefrom.ToString("yyyy-MM-dd");
			string DateTo = Dateto.ToString("yyyy-MM-dd");
			List<ScrapViewModel> scrapData = new List<ScrapViewModel>();
			try
			{
				string connectionString = APIEndpoint.ConnectionString;
				using (SqlConnection conn =
					new SqlConnection(connectionString))
				{
					conn.Open();
                    //using (SqlCommand cmd = new SqlCommand("select HASIL.*,convert(float, (HASIL.SaldoAwal + (HASIL.Pemasukan - HASIL.Pengeluaran))) as SaldoBuku from (select DATA.ClassificationID, DATA.ClassificationCode, DATA.ClassificationName, DATA.StockId, DATA.DestinationId, DATA.UnitQtyName,convert(float,((DATA.saldoIn + DATA.koreksisaldoIn) - (DATA.saldoOut + DATA.koreksisaldoOut))) as SaldoAwal,convert(float,(DATA.transaksiIn + DATA.koreksitransaksiIn)) as Pemasukan,convert(float,(DATA.transaksiOut + DATA.koreksitransaksiOut)) as Pengeluaran,convert(float,0) as Penyesuaian,convert(float,0) as StockOpname,convert(float,0) as Selisih from (select SC.ClassificationID, SC.ClassificationCode, SC.ClassificationName, SS.StockId, SS.DestinationId, UQ.UnitQtyName,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'IN' and a.TransactionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId) as saldoIn,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'OUT' and a.TransactionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID) as saldoOut,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'PLUS') - (select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'MINUS') as koreksisaldoIn,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate < '"+DateFrom+"' and b.ClassificationID = SC.ClassificationID and c.DetailType = 'MINUS') as koreksisaldoOut,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'IN' and a.TransactionDate >= '"+DateFrom+"' and a.TransactionDate < dateadd(day, 1, '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId) as transaksiIn,(select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'OUT' and a.TransactionDate >= '"+DateFrom+"' and a.TransactionDate < dateadd(day, 1,  '"+DateTo+"') and b.ClassificationID = SC.ClassificationID) as transaksiOut,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1,'"+DateTo+"') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1,  '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'MINUS') as koreksitransaksiIn,(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1, '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate >= '"+DateFrom+"' and d.CorrectionDate < dateadd(day, 1,  '"+DateTo+"') and b.ClassificationID = SC.ClassificationID and c.DetailType = 'MINUS') as koreksitransaksiOut from ScrapClassification SC inner join ScrapStock SS on SC.ClassificationID = SS.ClassificationId inner join UNITQUANTITY UQ on SS.UnitQty = UQ.UnitQtyCode )DATA)HASIL", conn))
                    using (SqlCommand cmd = new SqlCommand("select HASIL.*,convert(float, (HASIL.SaldoAwal + (HASIL.Pemasukan - HASIL.Pengeluaran))) as SaldoBuku from (select DATA.ClassificationID, DATA.ClassificationCode, DATA.ClassificationName, convert(float,((DATA.SaldoInFabric + DATA.saldoInGarment + DATA.saldoInKain + DATA.SaldoInTcKecil + DATA.koreksisaldoInTcKecil) - (DATA.saldoOutFabric + DATA.saldoOutGarment + DATA.saldoOutKain + DATA.saldoOutTcKecil + Data.koreksitransaksiOutTcKecil))) as SaldoAwal, convert(float,(DATA.transaksiInFabric + DATA.transaksiInGarment + DATA.transaksiInKain + DATA.transaksiInTcKecil + DATA.koreksitransaksiInTcKecil)) as Pemasukan, convert(float,(DATA.transaksiOutFabric + DATA.transaksiOutGarment + DATA.transaksiOutKain + DATA.transaksiOutTcKecil + DATA.koreksitransaksiOutTcKecil)) as Pengeluaran, convert(float,0) as Penyesuaian, convert(float,0) as StockOpname, convert(float,0) as Selisih from (select SC.ClassificationID, SC.ClassificationCode, SC.ClassificationName, (select isnull(SUM(a.Quantity), 0) from AvalFabIn a where a.CreateDate < '" + DateFrom + "' and a.ClassificationId = SC.ClassificationID ) as SaldoInFabric, (select isnull(SUM(a.Quantity), 0) from AvalGarmentIn a where a.CreateDate < '" + DateFrom + "' and a.ClassificationID = SC.ClassificationID) as saldoInGarment, (select isnull(SUM(a.Quantity), 0) from AvalKainIn a where a.CreatedDate < '" + DateFrom + "' and a.ClassificationID = SC.ClassificationID) as saldoInKain, (select isnull(SUM(a.QuantityOut), 0) from AvalFabOut a where a.CreatedDate < '" + DateFrom + "' and a.ClassificationID = SC.ClassificationID) as saldoOutFabric, (select isnull(SUM(a.QuantityOut), 0) from AvalGarmentOut a where a.CreatedDate < '" + DateFrom + "' and a.ClassificationID = SC.ClassificationID) as saldoOutGarment, (select isnull(SUM(a.QtyRevision), 0) from AvalKainOut a where a.CreatedDate < '" + DateFrom + "' and a.ClassificationID = SC.ClassificationID) as saldoOutKain, (select isnull(SUM(a.Quantity), 0) from AvalFabIn a where a.CreateDate >= '" + DateFrom + "' and a.CreateDate < dateadd(day, 1, '" + DateTo + "') and a.ClassificationID = SC.ClassificationID ) as transaksiInFabric, (select isnull(SUM(a.Quantity), 0) from AvalGarmentIn a where a.CreateDate >= '" + DateFrom + "' and a.CreateDate < dateadd(day, 1, '" + DateTo + "') and a.ClassificationID = SC.ClassificationID ) as transaksiInGarment, (select isnull(SUM(a.Quantity), 0) from AvalKainIn a where a.CreatedDate >= '" + DateFrom + "' and a.CreatedDate < dateadd(day, 1, '" + DateTo + "') and a.ClassificationID = SC.ClassificationID ) as transaksiInKain, (select isnull(SUM(a.QuantityOut), 0) from AvalFabOut a where a.CreatedDate >= '" + DateFrom + "' and a.CreatedDate < dateadd(day, 1, '" + DateTo + "') and a.ClassificationID = SC.ClassificationID ) as transaksiOutFabric, (select isnull(SUM(a.QuantityOut), 0) from AvalGarmentOut a where a.CreatedDate >= '" + DateFrom + "' and a.CreatedDate < dateadd(day, 1, '" + DateTo + "') and a.ClassificationID = SC.ClassificationID ) as transaksiOutGarment, (select isnull(SUM(a.QtyRevision), 0) from AvalKainOut a where a.CreatedDate >= '" + DateFrom + "' and a.CreatedDate < dateadd(day, 1, '" + DateTo + "') and a.ClassificationID = SC.ClassificationID ) as transaksiOutKain, (select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'IN' and a.TransactionDate < '" + DateFrom + "' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId) as SaldoInTcKecil, (select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'OUT' and a.TransactionDate < '" + DateFrom + "' and b.ClassificationID = SC.ClassificationID ) as saldoOutTcKecil, (select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate < '" + DateFrom + "' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'PLUS')-(select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate < '" + DateFrom + "' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'MINUS') as koreksisaldoInTcKecil, (select ISNULL(SUM(c.Quantity),0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate < '" + DateFrom + "' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'PLUS')- (select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate < '" + DateFrom + "' and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'MINUS') as koreksisaldoOutTcKecil, (select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate >= '" + DateFrom + "' and d.CorrectionDate < dateadd(day, 1, '" + DateTo + "') and b.ClassificationID = SC.ClassificationID and c.DetailType = 'PLUS')- (select isnull(SUM(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'OUT' and d.CorrectionDate >= '" + DateFrom + "' and d.CorrectionDate < dateadd(day, 1,  '" + DateTo + "') and b.ClassificationID = SC.ClassificationID and c.DetailType = 'MINUS') as koreksitransaksiOutTcKecil, (select ISNULL(Sum(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' and d.CorrectionDate >= '" + DateFrom + "' and d.CorrectionDate < dateadd(day, 1,  '" + DateTo + "') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'PLUS')- (select ISNULL(Sum(c.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID inner join ScrapCorrectionDetail c on b.DetailID = c.DetailTransactionId inner join ScrapCorrection d on c.CorrectionId = d.CorrectionId where a.TransactionType = 'IN' AND D.CorrectionDate >= '" + DateFrom + "' and d.CorrectionDate < dateadd(day, 1,  '" + DateTo + "') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId and c.DetailType = 'MINUS') as koreksitransaksiInTcKecil, (select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'IN' and a.TransactionDate >= '" + DateFrom + "' and a.TransactionDate < dateadd(day, 1, '" + DateTo + "') and b.ClassificationID = SC.ClassificationID and a.Destination = SS.DestinationId) as transaksiInTcKecil, (select isnull(SUM(b.Quantity), 0) from ScrapTransaction a inner join ScrapTransactionDetail b on a.TransactionID = b.TransactionID where a.TransactionType = 'OUT' and a.TransactionDate >= '" + DateFrom + "' and a.TransactionDate < dateadd(day, 1,  '" + DateTo + "') and b.ClassificationID = SC.ClassificationID) as transaksiOutTcKecil from ScrapClassification SC LEFT JOIN ScrapStock SS on SC.ClassificationID = SS.ClassificationId)DATA )HASIL", conn))
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow data in dSet.Tables[0].Rows)
                        {
                            if (data["ClassificationId"].ToString() == "SC190001")
                            {
                                ScrapViewModel scrap = new ScrapViewModel
                                {
                                    ClassificationId = data["ClassificationId"].ToString(),
                                    ClassificationCode = data["ClassificationCode"].ToString(),
                                    ClassificationName = data["ClassificationName"].ToString(),
                                    DestinationId = null,
                                    Pemasukan = String.Format("{0:n}", (double)data["Pemasukan"]),
                                    Pengeluaran = String.Format("{0:n}", (double)data["Pengeluaran"]),
                                    Penyesuaian = String.Format("{0:n}", (double)data["Penyesuaian"]),
                                    SaldoAwal = String.Format("{0:n}", (double)data["SaldoAwal"]),
                                    SaldoBuku = String.Format("{0:n}", (double)data["SaldoBuku"]),
                                    Selisih = String.Format("{0:n}", (double)data["Selisih"]),
                                    StockId = null,
                                    StockOpname = String.Format("{0:n}", (double)data["StockOpname"]),
                                    UnitQtyName = "PCS"
                                };
                                scrapData.Add(scrap);
                            }
                            else if (data["ClassificationId"].ToString() == "SC190002")
                            {
                                ScrapViewModel scrap = new ScrapViewModel
                                {
                                    ClassificationId = data["ClassificationId"].ToString(),
                                    ClassificationCode = data["ClassificationCode"].ToString(),
                                    ClassificationName = data["ClassificationName"].ToString(),
                                    DestinationId = null,
                                    Pemasukan = String.Format("{0:n}", (double)data["Pemasukan"]),
                                    Pengeluaran = String.Format("{0:n}", (double)data["Pengeluaran"]),
                                    Penyesuaian = String.Format("{0:n}", (double)data["Penyesuaian"]),
                                    SaldoAwal = String.Format("{0:n}", (double)data["SaldoAwal"]),
                                    SaldoBuku = String.Format("{0:n}", (double)data["SaldoBuku"]),
                                    Selisih = String.Format("{0:n}", (double)data["Selisih"]),
                                    StockId = null,
                                    StockOpname = String.Format("{0:n}", (double)data["StockOpname"]),
                                    UnitQtyName = "KG"
                                };
                                scrapData.Add(scrap);
                            }
                            else if (data["ClassificationId"].ToString() == "SC190003")
                            {
                                ScrapViewModel scrap = new ScrapViewModel
                                {
                                    ClassificationId = data["ClassificationId"].ToString(),
                                    ClassificationCode = data["ClassificationCode"].ToString(),
                                    ClassificationName = data["ClassificationName"].ToString(),
                                    DestinationId = null,
                                    Pemasukan = String.Format("{0:n}", (double)data["Pemasukan"]),
                                    Pengeluaran = String.Format("{0:n}", (double)data["Pengeluaran"]),
                                    Penyesuaian = String.Format("{0:n}", (double)data["Penyesuaian"]),
                                    SaldoAwal = String.Format("{0:n}", (double)data["SaldoAwal"]),
                                    SaldoBuku = String.Format("{0:n}", (double)data["SaldoBuku"]),
                                    Selisih = String.Format("{0:n}", (double)data["Selisih"]),
                                    StockId = null,
                                    StockOpname = String.Format("{0:n}", (double)data["StockOpname"]),
                                    UnitQtyName = "MT"
                                };
                                scrapData.Add(scrap);
                            }
                            else if (data["ClassificationId"].ToString() == "SC140001")
                            {
                                ScrapViewModel scrap = new ScrapViewModel
                                {
                                    ClassificationId = data["ClassificationId"].ToString(),
                                    ClassificationCode = data["ClassificationCode"].ToString(),
                                    ClassificationName = data["ClassificationName"].ToString(),
                                    DestinationId = null,
                                    Pemasukan = String.Format("{0:n}", (double)data["Pemasukan"]),
                                    Pengeluaran = String.Format("{0:n}", (double)data["Pengeluaran"]),
                                    Penyesuaian = String.Format("{0:n}", (double)data["Penyesuaian"]),
                                    SaldoAwal = String.Format("{0:n}", (double)data["SaldoAwal"]),
                                    SaldoBuku = String.Format("{0:n}", (double)data["SaldoBuku"]),
                                    Selisih = String.Format("{0:n}", (double)data["Selisih"]),
                                    StockId = null,
                                    StockOpname = String.Format("{0:n}", (double)data["StockOpname"]),
                                    UnitQtyName = "KG"
                                };
                                scrapData.Add(scrap);
                            }
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

			return scrapData.AsQueryable();
		}

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetScrapReport(dateFrom, dateTo, offset);
            DataTable Result = new DataTable();
            Result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Saldo Awal", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Pemasukan", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Pengeluaran", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Penyesuaian", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Saldo Buku", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Stock Opname", DataType = typeof(Double) });
            Result.Columns.Add(new DataColumn() { ColumnName = "Selisih", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                Result.Rows.Add("", "", "", 0, 0, 0, 0, 0, 0, 0);
            else
                foreach (var item in Query)
                {
                    Result.Rows.Add(item.ClassificationCode, item.ClassificationName, item.UnitQtyName, item.SaldoAwal, item.Pemasukan, item.Pengeluaran, item.Penyesuaian, item.SaldoBuku, item.StockOpname, item.Selisih);
                }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(Result, "ScrapReject") }, true);
        }
    }
}
 
