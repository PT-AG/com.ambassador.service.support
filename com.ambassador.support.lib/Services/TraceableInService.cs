using com.ambassador.support.lib.Helpers;
using com.ambassador.support.lib.ViewModel;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using OfficeOpenXml;
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
    public class TraceableInService
    {
        ILocalDbProductionDBContext dBContext;
        public TraceableInService(ILocalDbProductionDBContext _dBContext)
        {
            this.dBContext = _dBContext;
        }

        public IQueryable<TraceableINViewModel> getQueryTracable2(string filter, string tipe, string tipebc, DateTime? Datefrom, DateTime? DateTo)
        {
            DateTime dateFrom = Datefrom == null ? new DateTime(1970, 1, 1) : (DateTime)Datefrom;
            DateTime dateTo = DateTo == null ? DateTime.Now : (DateTime)DateTo;
            string datefrom = dateFrom.ToString("yyyy-MM-dd");
            string dateto = dateTo.ToString("yyyy-MM-dd");
            int no = 0;
            List<TraceableINViewModel> reportData = new List<TraceableINViewModel>();
                string connectionString = APIEndpoint.LocalConnectionString;
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();
                if (string.IsNullOrWhiteSpace(tipebc))
                {

                    using (SqlCommand cmd = new SqlCommand("select ROW_NUMBER() OVER ( ORDER BY BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt, ROJob, BUK) row_num, BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt,SatMasuk, BUK, QtyBUK, SatKeluar, ROJob, ProduksiQty, subconOut + FinisihingOutQty as BJQty, subconOut, QtyReceipt - QtyBUK as Sisa, ExpenditureType, '' Invoice, '' PEB, '' TAnggalPEB,'' EksporQty, ExpenditureTypeId into #2 " +
                    "from " +
                    "(Select a.BCNo, b.BonNo, a.BCType, a.BCDate,d.ItemCode, (select top 1 ItemName from DL_Supports.dbo.ITEM_CATEGORY where ItemCode = d.ItemCode) as ItemName, isnull(h.Invoice,'-') Invoice ,Sum(i.Qty) As EksporQty, isnull(h.ExpenditureType, '-') ExpenditureType, isnull(j.BCNo,'-') as PEB, isnull(j.BCDate,'1970-01-01') as TAnggalPEB, " +
                    "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = c.DetailShippingOrderId),0) As QtyReceipt," +
                    "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = c.DetailShippingOrderId) as SatMasuk," +
                    "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId) as SatKeluar," +
                    "isnull(g.DestinationJob,'-') as ROJob, " +
                    "isnull(x.ExpenditureTypeId, '-') ExpenditureTypeId, " +
                    "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = d.POId) NoPO," +
                    "isnull((select  ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),'-') BUK," +
                    "isnull((select  SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),0) QtyBUK," +
                    "(SELECT isnull(Sum(Qty),0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = g.DestinationJob and b.FinishingOutTo='GUDANG JADI') FinisihingOutQty," +
                    "(select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = g.DestinationJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut," +
                    "(SELECT isnull(SUM(A.QtyCutting),0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = g.DestinationJob) ProduksiQty " +
                    "from DL_Supports.dbo.BEACUKAI a " +
                    "join DL_Supports.dbo.SHIPPING_ORDER b on a.BCId = b.BCId " +
                    "join DL_Supports.dbo.DETAIL_SHIPPING_ORDER c on b.ShippingOrderId = c.ShippingOrderId " +
                    "join DL_Supports.dbo.DO_ITEM d on c.POId= d.POId " +
                    "left join DL_Supports.dbo.DETAIL_DELIVERY_ORDER e on d.DOItemNo = e.ReferenceNo " +
                    "left join DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS f on e.DetailDOId = f.DetailDOId " +
                    "left join (Select ExpenditureNo, ExpenditureTypeId from DL_Inventories.dbo.EXPENDITURE_GOODS) x on f.ExpenditureNo = x.ExpenditureNo" +
                    "left join DL_Supports.dbo.DELIVERY_ORDER g  on e.DeliveryOrderNo = g.DeliveryOrderNo " +
                    "left join ExpenditureGood h on h.RO = g.DestinationJob " +
                    "left join ExpenditureGoodDetail i on h.ExpenditureGoodId = i.ExpenditureGoodId " +
                    "join DL_Inventories.dbo.DETAIL_RECEIPT_GOODS k on k.DetailShippingOrderId = c.DetailShippingOrderId " +
                    "left join DL_Supports.dbo.BEACUKAI_ADDED j on j.ExpenditureNo = h.Invoice " +
                    "group by a.BCNo,a.BCType, a.BCDate,BonNo," +
                    "g.DestinationJob ,k.DetailReceiptGoodsId, c.DetailShippingOrderId," +
                    "d.ItemCode," +
                    "d.ItemDetailId, d.POId, h.Invoice, f.DetailExpenditureGoodsId, h.ExpenditureType, j.BCNo, j.BCDate, ExpenditureTypeId " +
                    ") data where bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' " +
                    "order by BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt, ROJob, BUK " +
                    "select ROW_NUMBER() OVER ( " +
                    "ORDER BY BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt, ROJob, Invoice " +
                    ") row_num,  '' BCType, '' BCNo, '' BCDate, '' BonNo,'' NoPO, '' ItemCode, '' ItemName, 0 QtyReceipt, '' SatMasuk, '' BUK, 0 QtyBUK, '' SatKeluar, '' ROJob, 0 ProduksiQty, 0 as BJQty, 0 subconOut, 0 as Sisa, '' as ExpenditureType, Invoice, PEB, TAnggalPEB, EksporQty, ExpenditureTypeId into #1 from " +
                    "(Select a.BCNo, b.BonNo, a.BCType, a.BCDate,d.ItemCode, D.ItemDetailId as ItemName, isnull(h.Invoice,'-') Invoice ,Sum(i.Qty) As EksporQty, isnull(h.ExpenditureType, '-') ExpenditureType, isnull(j.BCNo,'-') as PEB, isnull(j.BCDate,'1970-01-01') as TAnggalPEB, " +
                    "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = c.DetailShippingOrderId),0) As QtyReceipt," +
                    "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = c.DetailShippingOrderId) as SatMasuk," +
                    "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId) as SatKeluar," +
                    "isnull(g.DestinationJob,'-') as ROJob, " +
                    "isnull(x.ExpenditureTypeId, '-') ExpenditureTypeId, " +
                    "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = d.POId) NoPO," +
                    "isnull((select  ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),'-') BUK," +
                    "isnull((select  SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),0) QtyBUK," +
                    "(SELECT isnull(Sum(Qty),0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = g.DestinationJob and b.FinishingOutTo='GUDANG JADI') FinisihingOutQty," +
                    "(select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = g.DestinationJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut," +
                    "(SELECT isnull(SUM(A.QtyCutting),0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = g.DestinationJob) ProduksiQty " +
                    "from DL_Supports.dbo.BEACUKAI a " +
                    "join DL_Supports.dbo.SHIPPING_ORDER b on a.BCId = b.BCId " +
                    "join DL_Supports.dbo.DETAIL_SHIPPING_ORDER c on b.ShippingOrderId = c.ShippingOrderId " +
                    "join DL_Supports.dbo.DO_ITEM d on c.POId= d.POId " +
                    "left join DL_Supports.dbo.DETAIL_DELIVERY_ORDER e on d.DOItemNo = e.ReferenceNo " +
                    "left join DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS f on e.DetailDOId = f.DetailDOId " +
                    "left join (Select ExpenditureNo, ExpenditureTypeId from DL_Inventories.dbo.EXPENDITURE_GOODS) x on f.ExpenditureNo = x.ExpenditureNo " +
                    "left join DL_Supports.dbo.DELIVERY_ORDER g  on e.DeliveryOrderNo = g.DeliveryOrderNo " +
                    "left join ExpenditureGood h on h.RO = g.DestinationJob " +
                    "left join ExpenditureGoodDetail i on h.ExpenditureGoodId = i.ExpenditureGoodId " +
                    "join DL_Inventories.dbo.DETAIL_RECEIPT_GOODS k on k.DetailShippingOrderId = c.DetailShippingOrderId " +
                    "left join DL_Supports.dbo.BEACUKAI_ADDED j on j.ExpenditureNo = h.Invoice " +
                    "group by a.BCNo,a.BCType, a.BCDate,BonNo, " +
                    "g.DestinationJob ,k.DetailReceiptGoodsId, c.DetailShippingOrderId," +
                    "d.ItemCode," +
                    "d.ItemDetailId, d.POId, h.Invoice, f.DetailExpenditureGoodsId, h.ExpenditureType, j.BCNo, j.BCDate , ExpenditureTypeId" +
                    ") data where bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' " +
                    "order by BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt, ROJob, Invoice " +
                    "Select b.BCType, b.BCNo, b.BCDate, b.BonNo, b.NoPO, b.ItemCode, b.ItemName, b.QtyReceipt,b.SatMasuk, b.BUK, b.QtyBUK, b.SatKeluar, b.ROJob, b.ProduksiQty, b.BJQty, b.subconOut, b.Sisa, b.ExpenditureType, a.Invoice, a.PEB, a.TAnggalPEB, a.EksporQty from #1 a join #2 b on a.row_num = b.row_num " +
                    "where a.ExpenditureTypeId NOT IN ('EXT002','EXT003','EXT004','EXT005') " +
                    "drop table #1 " +
                    "drop table #2", conn))
                    {
                        cmd.CommandTimeout = (1000 * 60 * 20);
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow data in dSet.Tables[0].Rows)
                        {
                            TraceableINViewModel trace = new TraceableINViewModel
                            {
                                No = (long)no++,
                                BCType = data["BCType"].ToString(),
                                BCNo = data["BCNo"].ToString(),
                                BCDate = data["BCDate"].ToString() == null ? "" : data["BCDate"].ToString(),
                                BonNo = data["BonNo"].ToString(),
                                PO = data["NoPO"].ToString(),
                                ItemCode = data["ItemCode"].ToString(),
                                ItemName = data["ItemName"].ToString(),
                                ReceiptQty = (double)data["QtyReceipt"],
                                SatuanReceipt = data["SatMasuk"].ToString(),
                                BUK = data["BUK"].ToString(),
                                QtyBUK = (double)data["QtyBUK"],
                                SatuanBUK = data["SatKeluar"].ToString(),
                                ROJob = data["ROJob"].ToString(),
                                ProduksiQty = (double)data["ProduksiQty"],
                                //BJQty = (double)data["subconOut"] + (double)data["FinisihingOutQty"],
                                BJQty = (double)data["BJQty"],
                                Invoice = data["Invoice"].ToString(),
                                PEB = data["PEB"].ToString(),
                                PEBDate = data["TAnggalPEB"].ToString() == null ? "" : data["TAnggalPEB"].ToString(),
                                EksporQty = data["ExpenditureType"].ToString() == "E001" ? (double)data["EksporQty"] : 0,
                                SampleQty = data["ExpenditureType"].ToString() == "E003" ? (double)data["EksporQty"] : 0,
                                SubkonOutQty = (double)data["subconOut"],
                                //Sisa = (double)data["QtyReceipt"] - (double)data["QtyBUK"],
                                Sisa = (double)data["Sisa"],
                                ExType = data["ExpenditureType"].ToString()
                            };
                            reportData.Add(trace);
                        }
                    }

                    var total = from a in reportData
                                group a by new { a.QtyBUK } into groupdata
                                select new
                                {
                                    qtybuk = groupdata.FirstOrDefault().QtyBUK,
                                    PO = groupdata.FirstOrDefault().PO
                                };
                    var totalQty = from a in total
                                   group a by new { a.PO } into data
                                   select new
                                   {
                                       PO = data.Key.PO,
                                       qty = data.Sum(x => x.qtybuk)
                                   };
                    foreach (var i in reportData)
                    {
                        var qtytotalbuk = totalQty.Where(x => x.PO == i.PO).Select(x => x.qty).FirstOrDefault();
                        i.Sisa = i.ReceiptQty - qtytotalbuk;
                    }

                    foreach (var i in reportData)
                    {
                        i.QtyBUK = i.ReceiptQty - i.Sisa;
                        //i.Sisa = i.ReceiptQty - i.qtytotalbuk;
                    }

                }
                else
                {
                    //string cmdtraceable = "select ROW_NUMBER() Over(Order By NoPO, BUK,  QtyBUK, QtyReceipt) row_num, '' as BCNo, '' as BonNo, '' as BCType, null as BCDate,ItemCode,  ItemName,'' as Invoice, 0 as EksporQty, '' as ExpenditureType, '' as PEB, null as TAnggalPEB, QtyReceipt, '' as SatMasuk, '' as SatKeluar, '' as ROJob, BUK, QtyBUK, 0 as ProduksiQty, 0 as subconOut, NoPO, 0 as FinisihingOutQty into #PO " +
                    //    "from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, b.Invoice,Sum(c.Qty) As EksporQty, b.ExpenditureType, isnull(a.PEB,'-') as PEB, isnull(a.TglPEB,'1970-01-01') as TAnggalPEB, " +
                    //    "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt," +
                    //    "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk," +
                    //    "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar," +
                    //    "ROJob,DetailExpendId," +
                    //    "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO," +
                    //    "isnull((select  ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),'-') BUK," +
                    //    "isnull((select  SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK," +
                    //    "(SELECT isnull(Sum(Qty),0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI') FinisihingOutQty," +
                    //    "(select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut," +
                    //    "(SELECT isnull(SUM(A.QtyCutting),0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob) ProduksiQty " +
                    //    "from FactControlFlow fcf " +
                    //    "join ExpenditureGood b on fcf.ROJob = b.RO " +
                    //    "join ExpenditureGoodDetail c on c.ExpenditureGoodId = b.ExpenditureGoodId " +
                    //    "left join Shipping.dbo.OmzetKonf a on b.Invoice = a.Invoice_No " +
                    //    "group by BCNo,BCType, BCDate,BonNo," +
                    //    "ROJob ,DetailReceiptId, DetailShippingOrderId," +
                    //    "fcf.ItemCode," +
                    //    "fcf.ItemName, DetailExpendId,POId, b.Invoice, b.ExpenditureType, a.PEB, a.TglPEB) as data where " + tipe + " = '" + filter + "' AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' order by NoPO, BUK,  QtyBUK,data.QtyReceipt " +
                    //    "select ROW_NUMBER() Over(order by Invoice, EksporQty, PEB, BCNo, BonNo, BCType, ROJob, ProduksiQty, FinisihingOutQty) row_num, BCNo,  BonNo, BCType, BCDate, '' as ItemCode, '' as ItemName, Invoice, EksporQty, ExpenditureType, PEB, TAnggalPEB, 0 as QtyReceipt, SatMasuk, SatKeluar, ROJob, '' as BUK, 0 as QtyBUK, ProduksiQty, subconOut, '' as NoPO, FinisihingOutQty into #BCNo " +
                    //    "from(select distinct BCNo,BonNo,BCType, BCDate,fcf.ItemCode, fcf.ItemName, b.Invoice,Sum(c.Qty) As EksporQty, b.ExpenditureType, isnull(a.PEB,'-') as PEB, isnull(a.TglPEB,'1970-01-01') as TAnggalPEB, " +
                    //    "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = fcf.DetailShippingOrderId),0) As QtyReceipt," +
                    //    "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = fcf.DetailShippingOrderId) as SatMasuk," +
                    //    "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = fcf.DetailExpendId) as SatKeluar," +
                    //    "ROJob,DetailExpendId," +
                    //    "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = fcf.POId) NoPO," +
                    //    "isnull((select  ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),'-') BUK," +
                    //    "isnull((select  SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = fcf.DetailExpendId),0) QtyBUK," +
                    //    "(SELECT isnull(Sum(Qty),0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = fcf.ROJob and b.FinishingOutTo='GUDANG JADI') FinisihingOutQty," +
                    //    "(select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = fcf.ROJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut," +
                    //    "(SELECT isnull(SUM(A.QtyCutting),0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo where RO = fcf.ROJob) ProduksiQty " +
                    //    "from FactControlFlow fcf " +
                    //    "join ExpenditureGood b on fcf.ROJob = b.RO " +
                    //    "join ExpenditureGoodDetail c on c.ExpenditureGoodId = b.ExpenditureGoodId " +
                    //    "left join Shipping.dbo.OmzetKonf a on b.Invoice = a.Invoice_No " +
                    //    "group by BCNo,BCType, BCDate,BonNo," +
                    //    "ROJob ,DetailReceiptId, DetailShippingOrderId," +
                    //    "fcf.ItemCode," +
                    //    "fcf.ItemName, DetailExpendId,POId, b.Invoice, b.ExpenditureType, a.PEB, a.TglPEB) as data where " + tipe + " = '" + filter + "' AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' order by Invoice, EksporQty, PEB, BCNo, BonNo, BCType, ROJob, ProduksiQty, FinisihingOutQty " +
                    //    "select * from (select * from #PO union all select * from #BCNo)as data " +
                    //    "drop table #PO " +
                    //    "drop table #BCNo ";
                    string cmdtraceable = "select ROW_NUMBER() OVER ( ORDER BY BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt, ROJob, BUK) row_num, BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt,SatMasuk, BUK, QtyBUK, SatKeluar, ROJob, ProduksiQty, subconOut + FinisihingOutQty as BJQty, subconOut, QtyReceipt - QtyBUK as Sisa, ExpenditureType, '' Invoice, '' PEB, '' TAnggalPEB,'' EksporQty, ExpenditureTypeId into #2 " +
                        "from " +
                        "(Select a.BCNo, b.BonNo, a.BCType, a.BCDate,d.ItemCode, (select top 1 ItemName from DL_Supports.dbo.ITEM_CATEGORY where ItemCode = d.ItemCode) as ItemName, isnull(h.Invoice,'-') Invoice ,Sum(i.Qty) As EksporQty, isnull(h.ExpenditureType, '-') ExpenditureType , isnull(j.BCNo,'-') as PEB, isnull(j.BCDate,'1970-01-01') as TAnggalPEB, " +
                        "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = c.DetailShippingOrderId),0) As QtyReceipt," +
                        "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = c.DetailShippingOrderId) as SatMasuk," +
                        "isnull((select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),'-') as SatKeluar," +
                        "isnull(g.DestinationJob,'-') as ROJob, " +
                        "isnull(x.ExpenditureTypeId, '-') ExpenditureTypeId, " +
                        "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = d.POId) NoPO," +
                        "isnull((select  ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),'-') BUK," +
                        "isnull((select  SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),0) QtyBUK," +
                        "(SELECT isnull(Sum(Qty),0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = g.DestinationJob and b.FinishingOutTo='GUDANG JADI') FinisihingOutQty," +
                        "(select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = g.DestinationJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut," +
                        "(SELECT isnull(SUM(A.QtyCutting),0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo join DL_Supports.dbo.DELIVERY_ORDER c on b.RO = c.DestinationJob join DL_Supports.dbo.DETAIL_DELIVERY_ORDER t on c.DeliveryOrderNo = t.DeliveryOrderNo join DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS e on t.DetailDOId = e.DetailDOId where RO = g.DestinationJob and e.PODescription like '%'+ (select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = d.POId) + '%' and a.ItemCode = d.ItemCode) ProduksiQty " +
                        "from DL_Supports.dbo.BEACUKAI a " +
                        "join DL_Supports.dbo.SHIPPING_ORDER b on a.BCId = b.BCId " +
                        "join DL_Supports.dbo.DETAIL_SHIPPING_ORDER c on b.ShippingOrderId = c.ShippingOrderId " +
                        "join DL_Supports.dbo.DO_ITEM d on c.POId= d.POId " +
                        "left join DL_Supports.dbo.DETAIL_DELIVERY_ORDER e on d.DOItemNo = e.ReferenceNo " +
                        "left join DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS f on e.DetailDOId = f.DetailDOId " +
                        "left join (Select ExpenditureNo, ExpenditureTypeId from DL_Inventories.dbo.EXPENDITURE_GOODS ) x on f.ExpenditureNo = x.ExpenditureNo " +
                        "left join DL_Supports.dbo.DELIVERY_ORDER g  on e.DeliveryOrderNo = g.DeliveryOrderNo " +
                        "left join ExpenditureGood h on h.RO = g.DestinationJob " +
                        "left join ExpenditureGoodDetail i on h.ExpenditureGoodId = i.ExpenditureGoodId " +
                        "join DL_Inventories.dbo.DETAIL_RECEIPT_GOODS k on k.DetailShippingOrderId = c.DetailShippingOrderId " +
                        "left join DL_Supports.dbo.BEACUKAI_ADDED j on j.ExpenditureNo = h.Invoice " +
                        "group by a.BCNo,a.BCType, a.BCDate,BonNo," +
                        "g.DestinationJob ,k.DetailReceiptGoodsId, c.DetailShippingOrderId," +
                        "d.ItemCode," +
                        "d.ItemDetailId, d.POId, h.Invoice, f.DetailExpenditureGoodsId, h.ExpenditureType, j.BCNo, j.BCDate, ExpenditureTypeId " +
                        ") data where " + tipe + " = '" + filter + "' AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' and BCType = '"+ tipebc +"' " +
                        "order by BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt, ROJob, BUK " +
                        "select ROW_NUMBER() OVER ( " +
                        "ORDER BY BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt, ROJob, Invoice " +
                        ") row_num,  '' BCType, '' BCNo, '' BCDate, '' BonNo,'' NoPO, '' ItemCode, '' ItemName, 0 QtyReceipt, '' SatMasuk, '' BUK, 0 QtyBUK, '' SatKeluar, '' ROJob, 0 ProduksiQty, 0 as BJQty, 0 subconOut, 0 as Sisa, '' as ExpenditureType, Invoice, PEB, TAnggalPEB, EksporQty, ExpenditureTypeId into #1 from " +
                        "(Select a.BCNo, b.BonNo, a.BCType, a.BCDate,d.ItemCode, D.ItemDetailId as ItemName, isnull(h.Invoice,'-') Invoice ,Sum(i.Qty) As EksporQty, isnull(h.ExpenditureType, '-') ExpenditureType , isnull(j.BCNo,'-') as PEB, isnull(j.BCDate,'1970-01-01') as TAnggalPEB, " +
                        "isnull((select top 1 Quantity * ConvertValue from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS where DetailShippingOrderId = c.DetailShippingOrderId),0) As QtyReceipt," +
                        "(select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_RECEIPT_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQtyCode = b.UnitQtyCode where a.DetailShippingOrderId = c.DetailShippingOrderId) as SatMasuk," +
                        "isnull((select top 1 b.UnitQtyName from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a join DL_Inventories.dbo.UNIT_QUANTITY b on a.SmallestUnitQuantity = b.UnitQtyCode Where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),'-') as SatKeluar," +
                        "isnull(g.DestinationJob,'-') as ROJob," +
                        "isnull(x.ExpenditureTypeId, '-') ExpenditureTypeId," +
                        "(select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = d.POId) NoPO," +
                        "isnull((select  ExpenditureNo from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),'-') BUK," +
                        "isnull((select  SmallestQuantity from DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS a where a.DetailExpenditureGoodsId = f.DetailExpenditureGoodsId),0) QtyBUK," +
                        "(SELECT isnull(Sum(Qty),0) as a from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId where b.RO = g.DestinationJob and b.FinishingOutTo='GUDANG JADI') FinisihingOutQty," +
                        "(select isnull(sum(a.Qty),0) from FinishingOutDetail a join FinishingOut b on a.FinishingOutId = b.FinishingOutId join FinishingInDetail c on a.ReferenceFinishingInDetailId = c.FinishingInDetailId join FinishingIn d on c.FinishingNo = d.FinishingId where b.RO = g.DestinationJob and b.FinishingOutTo = 'GUDANG JADI' and d.FinishingFrom = 'PEMBELIAN') subconOut," +
                        "(SELECT isnull(SUM(A.QtyCutting),0) FROM CuttingDODetail a join CuttingDO b on a.DeliveryOrderNo = B.DeliveryOrderNo join DL_Supports.dbo.DELIVERY_ORDER c on b.RO = c.DestinationJob join DL_Supports.dbo.DETAIL_DELIVERY_ORDER t on c.DeliveryOrderNo = t.DeliveryOrderNo join DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS e on t.DetailDOId = e.DetailDOId where RO = g.DestinationJob and e.PODescription like '%'+ (select top 1 PlanPO from DL_Supports.dbo.PURCHASE_ORDER f where f.POId = d.POId) + '%' and a.ItemCode = d.ItemCode) ProduksiQty " +
                        "from DL_Supports.dbo.BEACUKAI a " +
                        "join DL_Supports.dbo.SHIPPING_ORDER b on a.BCId = b.BCId " +
                        "join DL_Supports.dbo.DETAIL_SHIPPING_ORDER c on b.ShippingOrderId = c.ShippingOrderId " +
                        "join DL_Supports.dbo.DO_ITEM d on c.POId= d.POId " +
                        "left join DL_Supports.dbo.DETAIL_DELIVERY_ORDER e on d.DOItemNo = e.ReferenceNo " +
                        "left join DL_Inventories.dbo.DETAIL_EXPENDITURE_GOODS f on e.DetailDOId = f.DetailDOId " +
                        "left join (Select ExpenditureNo, ExpenditureTypeId from DL_Inventories.dbo.EXPENDITURE_GOODS ) x on f.ExpenditureNo = x.ExpenditureNo " +
                        "left join DL_Supports.dbo.DELIVERY_ORDER g  on e.DeliveryOrderNo = g.DeliveryOrderNo " +
                        "left join ExpenditureGood h on h.RO = g.DestinationJob " +
                        "left join ExpenditureGoodDetail i on h.ExpenditureGoodId = i.ExpenditureGoodId " +
                        "join DL_Inventories.dbo.DETAIL_RECEIPT_GOODS k on k.DetailShippingOrderId = c.DetailShippingOrderId " +
                        "left join DL_Supports.dbo.BEACUKAI_ADDED j on j.ExpenditureNo = h.Invoice " +
                        "group by a.BCNo,a.BCType, a.BCDate,BonNo, " +
                        "g.DestinationJob ,k.DetailReceiptGoodsId, c.DetailShippingOrderId," +
                        "d.ItemCode, ExpenditureTypeId, " +
                        "d.ItemDetailId, d.POId, h.Invoice, f.DetailExpenditureGoodsId, h.ExpenditureType, j.BCNo, j.BCDate, ExpenditureTypeId" +
                        ") data where " + tipe + " = '" + filter + "' AND bcdate > '" + datefrom + "' And bcdate <= '" + dateTo + "' and ExpenditureType != 'E002' and BCType = '" + tipebc + "' " +
                        "order by BCType, BCNo, BCDate, BonNo, NoPO, ItemCode, ItemName, QtyReceipt, ROJob, Invoice " +
                        "Select b.BCType, b.BCNo, b.BCDate, b.BonNo, b.NoPO, b.ItemCode, b.ItemName, b.QtyReceipt,b.SatMasuk, b.BUK, b.QtyBUK, b.SatKeluar, b.ROJob, b.ProduksiQty, b.BJQty, b.subconOut, b.Sisa, b.ExpenditureType, a.Invoice, a.PEB, a.TAnggalPEB, a.EksporQty from #1 a join #2 b on a.row_num = b.row_num " +
                        "where a.ExpenditureTypeId NOT IN ('EXT002','EXT003','EXT004','EXT005') " +
                        "drop table #1 " +
                        "drop table #2";
                    using (SqlCommand cmd = new SqlCommand(cmdtraceable, conn))
                    {
                        cmd.CommandTimeout = (1000 * 60 * 20);
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);

                        foreach (DataRow data in dSet.Tables[0].Rows)
                        {
                            TraceableINViewModel trace = new TraceableINViewModel
                            {
                                No = (long)no++,
                                BCType = data["BCType"].ToString(),
                                BCNo = data["BCNo"].ToString(),
                                BCDate = data["BCDate"].ToString() == null ? "" : data["BCDate"].ToString(),
                                BonNo = data["BonNo"].ToString(),
                                PO = data["NoPO"].ToString(),
                                ItemCode = data["ItemCode"].ToString(),
                                ItemName = data["ItemName"].ToString(),
                                ReceiptQty = (double)data["QtyReceipt"],
                                SatuanReceipt = data["SatMasuk"].ToString(),
                                BUK = data["BUK"].ToString(),
                                QtyBUK = (double)data["QtyBUK"],
                                SatuanBUK = data["SatKeluar"].ToString(),
                                ROJob = data["ROJob"].ToString(),
                                ProduksiQty = (double)data["ProduksiQty"],
                                //BJQty = (double)data["subconOut"] + (double)data["FinisihingOutQty"],
                                BJQty = (double)data["BJQty"],
                                Invoice = data["Invoice"].ToString(),
                                PEB = data["PEB"].ToString(),
                                PEBDate = data["TAnggalPEB"].ToString() == null ? "" : data["TAnggalPEB"].ToString(),
                                EksporQty = data["ExpenditureType"].ToString() == "E001" ? (double)data["EksporQty"] : 0,
                                SampleQty = data["ExpenditureType"].ToString() == "E003" ? (double)data["EksporQty"] : 0,
                                SubkonOutQty = (double)data["subconOut"],
                                //Sisa = (double)data["QtyReceipt"] - (double)data["QtyBUK"],
                                Sisa = (double)data["Sisa"],
                                ExType = data["ExpenditureType"].ToString()
                            };
                            reportData.Add(trace);
                        }
                    }
                    var total = from a in reportData
                                group a by new { a.QtyBUK } into groupdata
                                select new
                                {
                                    qtybuk = groupdata.FirstOrDefault().QtyBUK,
                                    PO = groupdata.FirstOrDefault().PO
                                };
                    var totalQty = from a in total
                                   group a by new { a.PO } into data
                                   select new
                                   {
                                       PO = data.Key.PO,
                                       qty = data.Sum(x => x.qtybuk)
                                   };
                    foreach (var i in reportData)
                    {
                        var qtytotalbuk = totalQty.Where(x => x.PO == i.PO).Select(x => x.qty).FirstOrDefault();
                        i.Sisa = i.ReceiptQty - qtytotalbuk;
                    }

                    foreach (var i in reportData)
                    {
                        i.QtyBUK = i.ReceiptQty - i.Sisa;
                        //i.Sisa = i.ReceiptQty - i.qtytotalbuk;
                    }
                }
                    conn.Close();
                }
                //var report = (from a in reportData
                //             group a by a.No into datagroup
                //             select new TraceableINViewModel
                //             {
                //                 BCType = String.Join("", datagroup.Select(i=>i.BCType)),
                //                 BCNo = String.Join("", datagroup.Select(i => i.BCNo)),
                //                 BCDate = String.Join("", datagroup.Select(i => i.BCDate)).Trim(),
                //                 BonNo = String.Join("", datagroup.Select(i => i.BonNo)),
                //                 PO = String.Join("", datagroup.Select(i => i.PO)),
                //                 ItemCode = String.Join("", datagroup.Select(i => i.ItemCode)),
                //                 ItemName = String.Join("", datagroup.Select(i => i.ItemName)),
                //                 ReceiptQty = datagroup.Sum(x => x.ReceiptQty),
                //                 SatuanReceipt = String.Join("", datagroup.Select(i => i.SatuanReceipt)),
                //                 BUK = String.Join("", datagroup.Select(i => i.BUK)),
                //                 QtyBUK = datagroup.Sum(x => x.QtyBUK),
                //                 SatuanBUK = String.Join("", datagroup.Select(i => i.SatuanBUK)),
                //                 ROJob = String.Join("", datagroup.Select(i => i.ROJob)),
                //                 ProduksiQty = datagroup.Sum(x => x.ProduksiQty),
                //                 BJQty = datagroup.Sum(x => x.BJQty),
                //                 Invoice = String.Join("", datagroup.Select(i => i.Invoice)),
                //                 PEB = String.Join("", datagroup.Select(i => i.PEB)),
                //                 PEBDate = String.Join("", datagroup.Select(i => i.PEBDate)).Trim(),
                //                 EksporQty = datagroup.Sum(x => x.EksporQty),
                //                 SampleQty = datagroup.Sum(x => x.SampleQty),
                //                 SubkonOutQty = datagroup.Sum(x => x.SubkonOutQty),
                //                 Sisa = datagroup.Sum(x => x.Sisa),
                //                 ExType = String.Join("", datagroup.Select(i => i.ExType))
                //             }).ToList();
                var b = reportData.ToArray();
                var index = 0;
                foreach (TraceableINViewModel a in reportData)
                {
                    TraceableINViewModel dup = Array.Find(b, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == a.BonNo);
                    //TraceableINViewModel dup = Array.Find(b, o => o.Invoice == a.Invoice && o.PO == a.PO && o.PEB == a.PEB && o.BCType == a.BCType);
                    if (dup != null)
                    {
                        if (dup.count == 0)
                        {
                            index++;
                            dup.count = index;
                        }
                    }
                    a.count = dup.count;
                }
            
            return reportData.AsQueryable();
        }
        
        public MemoryStream GetTraceableInExcel(string filter, string tipe, string tipebc, DateTime? Datefrom, DateTime? DateTo)
        {
            var Query = getQueryTracable2(filter, tipe, tipebc, Datefrom, DateTo);
            Query.OrderBy(x => x.BCNo);
            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jenis BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nomor BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal BC", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No Bon", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "ROJob", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "PO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Terima", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Terima", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No BUK", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Keluar", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sisa", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Produksi Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "BJ Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Invoice", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "BC Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tgl BC Keluar", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Ekspor Qty", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sample Qty", DataType = typeof(Double) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", 0, "","", 0, 0, "",0, 0,"", "", "", 0, 0); // to allow column name to be generated properly for empty data as template
            else
            {
                var docNo = Query.ToArray();
                var q = Query.ToList();
                var index = 0;
                foreach (TraceableINViewModel a in q)
                {
                    TraceableINViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo && o.BonNo == o.BonNo);
                    if (dup != null)
                    {
                        if (dup.count == 0)
                        {
                            index++;
                            dup.count = index;
                        }
                    }
                    a.count = dup.count;
                }
                Query = q.AsQueryable();
                //var index = 0;
                foreach (var item in Query)
                {
                    string bcdate = String.IsNullOrWhiteSpace(item.BCDate) ? "-" : Convert.ToDateTime(item.BCDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string pebdate = String.IsNullOrWhiteSpace(item.PEBDate) ? "-" : Convert.ToDateTime(item.PEBDate).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(item.count, item.BCType, item.BCNo, bcdate, item.BonNo, item.ROJob, item.PO, item.ItemCode, item.ItemName, item.ReceiptQty, item.SatuanReceipt, item.BUK, item.QtyBUK, item.Sisa, item.SatuanBUK, item.ProduksiQty, item.BJQty, item.Invoice, item.PEB, pebdate, item.EksporQty, item.SampleQty);

                }
            }
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
            ExcelPackage package = new ExcelPackage();
            bool styling = true;

            foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);
                sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
                //sheet.Cells["C1:D1"].Merge = true;
                //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //sheet.Cells["E1:F1"].Merge = true;
                //sheet.Cells["C1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                Dictionary<string, int> bcnospan = new Dictionary<string, int>();
                Dictionary<string, int> rospan = new Dictionary<string, int>();
                Dictionary<string, int> rojobspan = new Dictionary<string, int>();
                Dictionary<string, int> docspan = new Dictionary<string, int>();
                Dictionary<string, int> rowspan = new Dictionary<string, int>();
                Dictionary<string, int> docspanpo = new Dictionary<string, int>();
                Dictionary<string, int> qtybukspan = new Dictionary<string, int>();
                Dictionary<string, int> sisaspan = new Dictionary<string, int>();
                Dictionary<string, int> prospan = new Dictionary<string, int>();
                Dictionary<string, int> eksporspan = new Dictionary<string, int>();
                var docNo = Query.ToArray();
                int value;
                foreach (var a in Query)
                {
                    //FactBeacukaiViewModel dup = Array.Find(docNo, o => o.BCType == a.BCType && o.BCNo == a.BCNo);
                    if (bcnospan.TryGetValue(a.BCType + a.BCNo + a.BCDate, out value))
                    {
                        bcnospan[a.BCType + a.BCNo + a.BCDate]++;
                    }
                    else
                    {
                        bcnospan[a.BCType + a.BCNo + a.BCDate] = 1;
                    }

                    //FactBeacukaiViewModel dup1 = Array.Find(docNo, o => o.BCType == a.BCType);
                    if (rospan.TryGetValue(a.BCType + a.BCNo + a.BCDate + a.BonNo, out value))
                    {
                        rospan[a.BCType + a.BCNo + a.BCDate + a.BonNo]++;
                    }
                    else
                    {
                        rospan[a.BCType + a.BCNo + a.BCDate + a.BonNo] = 1;
                    }

                    if (rojobspan.TryGetValue(a.BCType + a.BCNo + a.BonNo + a.ROJob + a.PO, out value))
                    {
                        rojobspan[a.BCType + a.BCNo + a.BonNo + a.ROJob + a.PO]++;
                    }
                    else
                    {
                        rojobspan[a.BCType + a.BCNo + a.BonNo + a.ROJob + a.PO] = 1;
                    }
                    if (docspan.TryGetValue(a.BCType + a.BCNo + a.BonNo + a.PO + a.ItemCode + a.ItemName, out value))
                    {
                        docspan[a.BCType + a.BCNo + a.BonNo + a.PO + a.ItemCode + a.ItemName]++;
                    }
                    else
                    {
                        docspan[a.BCType + a.BCNo + a.BonNo + a.PO + a.ItemCode + a.ItemName] = 1;
                    }
                    if (rowspan.TryGetValue(a.BCNo + a.BCType + a.BonNo + a.PO + a.ItemCode + a.ItemName + a.ReceiptQty + a.SatuanReceipt, out value))
                    {
                        rowspan[a.BCNo + a.BCType + a.BonNo + a.PO + a.ItemCode + a.ItemName + a.ReceiptQty + a.SatuanReceipt]++;
                    }
                    else
                    {
                        rowspan[a.BCNo + a.BCType + a.BonNo + a.PO + a.ItemCode + a.ItemName + a.ReceiptQty + a.SatuanReceipt] = 1;
                    }
                    if (docspanpo.TryGetValue(a.BCType + a.BCNo + a.BonNo + a.PO + a.ItemCode + a.ItemName + a.ReceiptQty + a.SatuanReceipt + a.ROJob + a.BUK, out value))
                    {
                        docspanpo[a.BCType + a.BCNo + a.BonNo + a.PO + a.ItemCode + a.ItemName + a.ReceiptQty + a.SatuanReceipt + a.ROJob + a.BUK]++;
                    }
                    else
                    {
                        docspanpo[a.BCType + a.BCNo + a.BonNo + a.PO + a.ItemCode + a.ItemName + a.ReceiptQty + a.SatuanReceipt + a.ROJob + a.BUK] = 1;
                    }
                    if(qtybukspan.TryGetValue(a.BCNo + a.BCType + a.BonNo + a.PO + a.QtyBUK, out value))
                    {
                        qtybukspan[a.BCNo + a.BCType + a.BonNo + a.PO + a.QtyBUK]++;
                    }
                    else
                    {
                        qtybukspan[a.BCNo + a.BCType + a.BonNo + a.PO + a.QtyBUK] = 1;
                    }
                    if (sisaspan.TryGetValue(a.BCNo + a.BCType + a.BonNo + a.PO + a.Sisa, out value))
                    {
                        sisaspan[a.BCNo + a.BCType + a.BonNo + a.PO + a.Sisa]++;
                    }
                    else
                    {
                        sisaspan[a.BCNo + a.BCType + a.BonNo + a.PO + a.Sisa] = 1;
                    }
                    if (prospan.TryGetValue(a.BCNo + a.BCType + a.BonNo + a.ROJob + a.PO + a.BUK + a.ProduksiQty, out value))
                    {
                        prospan[a.BCNo + a.BCType + a.BonNo + a.ROJob + a.PO + a.BUK + a.ProduksiQty]++;
                    }
                    else
                    {
                        prospan[a.BCNo + a.BCType + a.BonNo + a.ROJob + a.PO + a.BUK + a.ProduksiQty] = 1;
                    }
                    if (eksporspan.TryGetValue(a.BCNo + a.BCType + a.BonNo + a.ROJob + a.Invoice + a.PEB + a.EksporQty, out value))
                    {
                        eksporspan[a.BCNo + a.BCType + a.BonNo + a.ROJob + a.Invoice + a.PEB + a.EksporQty]++;
                    }
                    else
                    {
                        eksporspan[a.BCNo + a.BCType + a.BonNo + a.ROJob + a.Invoice + a.PEB + a.EksporQty] = 1;
                    }
                }

                int index = 2;
                foreach (KeyValuePair<string, int> b in bcnospan)
                {
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["B" + index + ":B" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["B" + index + ":B" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += b.Value;
                }

                index = 2;
                foreach (KeyValuePair<string, int> c in rospan)
                {
                    sheet.Cells["E" + index + ":E" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["E" + index + ":E" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["K" + index + ":K" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["O" + index + ":O" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in rojobspan)
                {
                    sheet.Cells["F" + index + ":F" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["F" + index + ":F" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in docspan)
                {
                    sheet.Cells["G" + index + ":G" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["G" + index + ":G" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["H" + index + ":H" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["H" + index + ":H" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["I" + index + ":I" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["I" + index + ":I" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in rowspan)
                {
                    sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["J" + index + ":J" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
 
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in docspanpo)
                {
                    sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["L" + index + ":L" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                  
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in qtybukspan)
                {
                    sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["M" + index + ":M" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    
                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in sisaspan)
                {
                    sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["N" + index + ":N" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }
                index = 2;
                foreach (KeyValuePair<string, int> c in prospan)
                {
                    sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["P" + index + ":P" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Merge = true;
                    sheet.Cells["Q" + index + ":Q" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index += c.Value;
                }
                //index = 2;
                //foreach (KeyValuePair<string, int> c in eksporspan)
                //{
                //    sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["R" + index + ":R" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                //    sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["S" + index + ":S" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                //    sheet.Cells["T" + index + ":T" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["T" + index + ":T" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                //    sheet.Cells["U" + index + ":U" + (index + c.Value - 1)].Merge = true;
                //    sheet.Cells["U" + index + ":U" + (index + c.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                //    index += c.Value;
                //}
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            }
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public ReadResponse<object> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            string Key = "";
            string cmd = "";

            List<TraceableINViewModel> data = new List<TraceableINViewModel>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            foreach (var f in FilterDictionary)
            {
                Key = f.Value;

            }
            if (Key == "ComodityName")
            {
                cmd = "Select Distinct a.ComodityName from Comoditi a join ExpenditureGoodDetail b on a.ComodityID = b.ComodityId where a.ComodityName Like @key";
            }
            else
            {
                cmd = "SELECT Distinct " + Key + "  FROM FactControlFlow where " + Key + " LIKE @key ";
            }
            parameters.Add(new SqlParameter("key", "%" + Keyword + "%"));
            var reader = dBContext.ExecuteReader(cmd, parameters);
            while (reader.Read())
            {
                switch (Key)
                {
                    case "BCNo":
                        data.Add(new TraceableINViewModel
                        {
                            BCNo = reader["BCNo"].ToString(),
                        });
                        break;
                    case "ROJob":
                        data.Add(new TraceableINViewModel
                        {
                            ROJob = reader["ROJob"].ToString(),
                        });
                        break;
                    case "ItemCode":
                        data.Add(new TraceableINViewModel
                        {
                            ItemCode = reader["ItemCode"].ToString(),
                        });
                        break;
                        //case "ComodityName":
                        //    data.Add(new TraceableINViewModel
                        //    {
                        //        ComodityName = reader["ComodityName"].ToString(),
                        //    });
                        //    break;
                }

            }

            Pageable<TraceableINViewModel> pageable = new Pageable<TraceableINViewModel>(data, Page - 1, Size);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            List<TraceableINViewModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            List<object> ListData = new List<object>();
            switch (Key)
            {
                case "BCNo":
                    ListData.AddRange(Data.Select(s => new
                    {
                        s.BCNo
                    }));
                    break;
                case "ROJob":
                    ListData.AddRange(Data.Select(s => new
                    {
                        s.ROJob

                    }));
                    break;
                case "ItemCode":
                    ListData.AddRange(Data.Select(s => new
                    {
                        s.ItemCode
                    }));
                    break;
                    //case "ComodityName":
                    //    ListData.AddRange(Data.Select(s => new
                    //    {
                    //        s.ComodityName
                    //    }));
                    //    break;
            }


            return new ReadResponse<object>(ListData, TotalData, OrderDictionary);
        }

        public class ReadResponse<TModel>
        {
            public List<TModel> Data { get; set; }
            public int TotalData { get; set; }
            public Dictionary<string, string> Order { get; set; }

            public ReadResponse(List<TModel> Data, int TotalData, Dictionary<string, string> Order)
            {
                this.Data = Data;
                this.TotalData = TotalData;
                this.Order = Order;
            }
        }
    }
}
