using com.ambassador.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace com.ambassador.support.lib.Interfaces
{
    public interface IReceiptRawMaterialService
    {
        Task<Tuple<List<ReceiptRawMaterialViewModel>, int>> GetReport(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order);
        Task<MemoryStream> GenerateExcel(DateTime? dateFrom, DateTime? dateTo);
    }
}
