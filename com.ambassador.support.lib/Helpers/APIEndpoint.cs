using System;
using System.Collections.Generic;
using System.Text;

namespace com.ambassador.support.lib.Helpers
{
    public static class APIEndpoint
    {
        public static string Core { get; set; }
        public static string Inventory { get; set; }

		public static string ConnectionString { get; set; }
        public static string LocalConnectionString { get; set; }
        public static string Production { get; set; }
    }
}
