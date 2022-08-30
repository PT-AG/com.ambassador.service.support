using System;
using System.Collections.Generic;
using System.Text;

namespace com.ambassador.support.lib.ViewModel
{
    public class ExpenditureRawMaterialViewModel
    {
        public string UENNo { get; set; }
        public string ExpenditureDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UomUnit { get; set; }
        public double Quantity { get; set; }
        public double QuantitySubcon { get; set; }
        public string ExpenditureType { get; set; }
        public string SubconTo { get; set; }

    }
}
