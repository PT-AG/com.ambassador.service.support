using System;
using System.Collections.Generic;
using System.Text;

namespace com.ambassador.support.lib.ViewModel
{
    public class TraceableINViewModel
    {
        public int count { get; set; }
        public string BCNo { get; set; }
        public string BonNo { get; set; }
        public string BCType { get; set; }
        public string BCDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double ProduksiQty { get; set; }
        public double ReceiptQty { get; set; }
        public double BJQty { get; set; }
        public string SatuanReceipt { get; set; }
        public double EksporQty { get; set; }
        public string PO { get; set; }
        //public string SatuanWIP { get; set; }
        public string ROJob { get; set; }
        public string Invoice { get; set; }
        public string PEBDate { get; set; }
        public string PEB { get; set; }
        public string BUK { get; set; }
        public double QtyBUK { get; set; }
        public string SatuanBUK { get; set; }
        public double SampleQty { get; set; }
        public double SubkonOutQty { get; set; }
        public double Sisa { get; set; }
        public string ExType { get; set; }
        public long No { get; set; }
    }
}
