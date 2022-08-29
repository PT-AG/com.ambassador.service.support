using System;
using System.Collections.Generic;
using System.Text;

namespace com.ambassador.support.lib.ViewModel
{
    public class FactMutationItemViewModel
    {
        public string unitCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
		public string SupplierType { get; set; }
		public string UnitQtyName { get; set; }
        public string BeginQty { get; set; }
        public string ReceiptQty { get; set; }
        public string ExpenditureQty { get; set; }
        public string AdjustmentQty { get; set; }
        public string OpnameQty { get; set; }
        public string LastQty { get; set; }
        public string Diff { get; set; }


    }
}
