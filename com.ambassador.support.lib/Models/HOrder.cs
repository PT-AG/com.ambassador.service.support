using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.ambassador.support.lib.Models
{
    public class HOrder : StandardEntity
    {
        public string No { get; set; }
        public string Konf { get; set; }
        public int? LT { get; set; }
        public string Repeat { get; set; }
        public string Seksi { get; set; }
        public string Codeby { get; set; }
        public string Article { get; set; }
        public DateTimeOffset Tgl { get; set; }
        public string Kode { get; set; }
        public double Qty { get; set; }
        public string Sat { get; set; }
        public double Price { get; set; }
        public string Valas { get; set; }
        public double Kurs { get; set; }
        public double Pcs { get; set; }
        public string SatPcs { get; set; }
        public double Otlk { get; set; }
        public double Otlu { get; set; }
        public DateTimeOffset Ord_Date { get; set; }
        public string Mt_Uang { get; set; }
        public DateTimeOffset Ship { get; set; }
        public string Post { get; set; }
        public string Complete { get; set; }
        public string Salesman { get; set; }
        public double Sh_Cut { get; set; }
        public double Sh_Sew { get; set; }
        public double Sh_Fin { get; set; }
        public double Sh { get; set; }
        public double Ws { get; set; }
        public string Valid { get; set; }
        public double Eff { get; set; }
        public DateTimeOffset Ppic_Date { get; set; }
        public DateTimeOffset Plan_Date { get; set; }
        public DateTimeOffset Ship_Date { get; set; }
        public DateTimeOffset Deadline { get; set; }
        public string Userin { get; set; }
        public DateTimeOffset Tglin { get; set; }
        public string Jamin { get; set; }
        public string Usered { get; set; }
        public DateTimeOffset Tgled { get; set; }
        public string Jamed { get; set; }

    }
}
