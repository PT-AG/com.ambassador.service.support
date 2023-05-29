using System;
using System.Collections.Generic;
using System.Text;

namespace com.ambassador.support.lib.ViewModel
{
    public class PEBforExpenditureViewModel
    {
        public string BCId { get; set; }
        public string BCNo { get; set; }
        public string Tipe { get; set; }
        public string BCType { get; set; }
        public DateTime BCDate { get; set; }
        public string BonNo { get; set; }
        public DateTime? BonDate { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitQtyName { get; set; }
        public double Quantity { get; set; }
        public decimal Nominal { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime TglDatang { get; set; }
        public string Vendor { get; set; }
        public string Country { get; set; }
    }
}
