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
    public class FinishedGoodService
    {
		SupportDbContext context;
		public FinishedGoodService(SupportDbContext _context)
		{
			this.context = _context;
		}
		public IQueryable<FinishedGoodViewModel> GetFinishedGoodReport(DateTime? dateFrom, DateTime? dateTo, int offset)
		{
			DateTime Datefrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
			DateTime Dateto = dateTo == null ? new DateTime(1970, 1, 1) : (DateTime)dateTo;
			string DateFrom = Datefrom.ToString("yyyy-MM-dd");
			string DateTo = Dateto.ToString("yyyy-MM-dd");
			List<FinishedGoodViewModel> wipData = new List<FinishedGoodViewModel>();
			try
			{
				string connectionString = APIEndpoint.ConnectionString;
				//"// select  ComodityId,0 as SaldoQtyFin, SUM(Qty) as QtyFin,0 as FinishingTransfer,0 as AdjFin, 0 as Retur ,0 as QtyExport, 0 as QtySample, 0 as QtyOther ,0 as sewingretur, 0 as  QtyFinSub,	0 as cuttingSubkon ,0 as finoutSubkon 	from (select ReferenceFinishingInDetailId ,ProcessDate,ComodityId,UnitCode,UnitCodeTo,Qty from FactFinishingDetailNotPembelian where   ProcessDate between  @DateFrom and @DateTo   and FinishingOutTo = 'GUDANG JADI'  and UnitCode= UnitCodeTo ) data group by ComodityId	" +
				//	"//union all " 
				using (SqlConnection conn =
					new SqlConnection(connectionString))
				{
					conn.Open();
					using (SqlCommand cmd = new SqlCommand(
					"declare @DateFrom datetime = '" + DateFrom + "' " +
					"declare @DateTo datetime = '" + DateTo + "' " +
					" select ComodityId,  (case when ProcessDate < @DateFrom then -Qty else 0 end) as SaldoQtyFin,0 as QtyFin,0 as FinishingTransfer,(case when ProcessDate >= @DateFrom then Qty else 0 end) as AdjFin, 0 as Retur ,0 as QtyExport, 0 as QtySample, 0 as QtyOther,0 as sewingretur ,0 as QtyFinSub,0 as cuttingSubkon,0 as finoutSubkon into #adjust from FactAdjustment where   ProcessDate <= @DateTo and AdjustType = 'FINISHING'   " +
					" select * into #returexpend from ( select ComodityId, sum(case when ProcessDate < @DateFrom then Qty else 0 end) as SaldoQtyFin, 0 as QtyFin, 0 as FinishingTransfer, 0 as AdjFin, sum(case when ProcessDate >= @DateFrom then Qty else 0 end) as Retur, 0 as QtyExport, 0 as QtySample, 0 as QtyOther, 0 as sewingretur, 0 as QtyFinSub, 0 as cuttingSubkon, 0 as finoutSubkon from factreturExpend where ProcessDate <= @DateTo group by ComodityId) as data where retur > 0 or SaldoQtyFin >0  " +
                        " select * into #factexpend from ( select ComodityId,sum(case when ProcessDate < @DateFrom  then (-1*Qty) else 0 end) as SaldoQtyFin,0 as QtyFin,0 as FinishingTransfer,0 as AdjFin,0 as Retur,sum(case when ProcessDate >= @DateFrom  and ExpenditureType = 'E001' then Qty else 0 end) as QtyExport, sum(case when ProcessDate >= @DateFrom  and ExpenditureType = 'E003' then Qty else 0 end) as QtySample,sum (case when ProcessDate >= @DateFrom  and ExpenditureType = 'E002' then Qty else 0 end) as QtyOther  ,0 as sewingretur ,0 as QtyFinSub,0 as cuttingSubkon,0 as finoutSubkon   from ( select ComodityId, Qty, ProcessDate, ExpenditureType from FactExpenditureGoods where ProcessDate <= @DateTo and ExpenditureType IN ('E001','E003')  )data group by ComodityId ) as data where saldoqtyfin<>0   or QtyExport <>0 or QtySample <>0 or QtyOther <> 0 " +
						" select * into #finishingbarangjadi from ( select ComodityId,sum(case when ProcessDate < @DateFrom then Qty else 0 end) as SaldoQtyFin, sum(case when ProcessDate >= @DateFrom then Qty else 0 end)   as QtyFin,0 as FinishingTransfer,0 as AdjFin, 0 as Retur ,0 as QtyExport, 0 as QtySample, 0 as QtyOther ,0 as sewingretur, 0 as  QtyFinSub,	0 as cuttingSubkon ,0 as finoutSubkon 	from ( select ProcessDate, ComodityId, UnitCode, UnitCodeTo, Qty from FactFinishingDetailBarangJadi where ProcessDate <= @DateTo     and FinishingOutTo = 'GUDANG JADI') data group by comodityid ) as data where saldoqtyfin > 0  or qtyfin >0  " +
						" select * into #factsewing from ( select ComodityId, 0 as SaldoQtyFin, 0 as QtyFin, 0 as FinishingTransfer, 0 as AdjFin, 0 as Retur, 0 as QtyExport, 0 as QtySample, 0 as QtyOther, (case when ProcessDate >= @DateFrom and SewingOutTo = 'CUTTING' and UnitCode = UnitCodeTo then Qty else 0 end) as sewingretur,0 as QtyFinSub ,0 as cuttingSubkon, 0 as finoutSubkon from factsewing where ProcessDate <= @DateTo ) as data where sewingretur >0 " +
						" select * into #factfinishingsubkon from ( select ComodityId,  (case when ProcessDate < @DateFrom then Qty else 0 end)  as SaldoQtyFin, 0 as QtyFin ,0 as FinishingTransfer,0 as AdjFin, 0 as Retur ,0 as QtyExport, 0 as QtySample, 0 as QtyOther ,0 as sewingretur,0 as QtyFinSub ,0 as cuttingSubkon, (case when ProcessDate >= @DateFrom then Qty else 0 end) as finoutSubkon from FactFinishingOutSubkon where   ProcessDate <= @DateTo  ) as data where finoutSubkon>0 or saldoqtyfin<>0  " +
						" select ComodityId,sum (case when ProcessDate < @DateFrom then Qty else 0 end) as SaldoQtyFin,0 as QtyFin,0 as FinishingTransfer,0 as AdjFin,sum(case when ProcessDate >= @DateFrom then Qty else 0 end) as Retur,0 as QtyExport, 0 as QtySample, 0 as QtyOther ,0 as sewingretur ,0 as QtyFinSub,0 as cuttingSubkon,0 as finoutSubkon into #returQtyFin from  factreturExpend where   ProcessDate <= @DateTo  group by ComodityId   " +
						" select * into #factnotsewingIn from ( select  ComodityId,  0 as SaldoQtyFin,0 as QtyFin, isnull(( select case when ProcessDate >= @DateFrom and UnitCode <> UnitCodeTo  and SewingOutTo = 'FINISHING' then Qty else 0 end),0) as FinishingTransfer,0 as AdjFin, 0 as Retur ,0 as QtyExport, 0 as QtySample, 0 as QtyOther ,0 as sewingretur,0 as QtyFinSub,0 as cuttingSubkon,0 as finoutSubkon from[FactSewingNotInSewingIn] where ProcessDate <= @DateTo ) as data2 where FinishingTransfer > 0   " +
                         " select case when saldoawal < 0 then 0 else saldoawal end SaldoAwal, KodeBarang, NamaBarang, Pemasukan, Pengeluaran, UnitQtyName, Penyesuaian, StockOpname, case when SaldoBuku < 0 then 0 else SaldoBuku end SaldoBuku from (select comodityCode KodeBarang,comodityname NamaBarang,sum(saldoqtyFin ) SaldoAwal ,sum(finoutSubkon +retur +QtyFin) Pemasukan,sum(QtyExport+QtySample+QtyOther+AdjFin) Pengeluaran,'PCS' UnitQtyName,0 as Penyesuaian,0 as StockOpname, sum(saldoqtyFin )+sum(finoutSubkon +retur +QtyFin)-sum(FinishingTransfer+QtyExport+QtySample+QtyOther+AdjFin)  SaldoBuku from (  " +
					" select * from #adjust  " +
					" union all  " +
					" select * from #returexpend  " +
					" union all  " +
					" select * from #factexpend  " +
					" union all  " +
					" select * from  #finishingbarangjadi " +
					" union all  " +
					" select * from #factsewing  " +
					" union all  " +
					" select* from #factfinishingsubkon " +
					 
					" union all  " +
					" select * from #factnotsewingIn) as data join( select comodityId, comodityName, comodityCode from comodity ) comodity on data.comodityid = comodity.comodityId    " +
					" group by comodityname,comodityCode) as data where saldoawal <> 0 or pemasukan <>0 or pengeluaran <>0 or Penyesuaian <>0 or StockOpname <>0 or SaldoBuku <>0  drop table #adjust drop table #returexpend drop table #factexpend drop table #finishingbarangjadi  drop table #factsewing     " +
					" drop table #factfinishingsubkon drop table #returQtyFin drop table #factnotsewingIn "
					, conn))
					{
						SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
						DataSet dSet = new DataSet();
						dataAdapter.Fill(dSet);
						foreach (DataRow data in dSet.Tables[0].Rows)
						{
							var a = data["Pemasukan"];
							var b = data["Pengeluaran"];
							var c = data["SaldoAwal"];
							var g = data["SaldoBuku"];
							var j= data["UnitQtyName"];
							var o = data["Penyesuaian"];
							var u = data["StockOpname"];

							FinishedGoodViewModel view = new FinishedGoodViewModel
							{
								KodeBarang = data["KodeBarang"].ToString(),
								NamaBarang=data["NamaBarang"].ToString(),
								Pemasukan = String.Format("{0:n}", data["Pemasukan"]),
								Pengeluaran = String.Format("{0:n}", data["Pengeluaran"]),
								SaldoAwal = String.Format("{0:n}", data["SaldoAwal"]),
								SaldoBuku = String.Format("{0:n}", data["SaldoBuku"]),
								UnitQtyName = data["UnitQtyName"].ToString(),
								Penyesuaian = String.Format("{0:n}", data["Penyesuaian"]),
								StockOpname = String.Format("{0:n}", data["StockOpname"]),
								Selisih = String.Format("{0:n}", 0)
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

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetFinishedGoodReport(dateFrom, dateTo, offset);
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
