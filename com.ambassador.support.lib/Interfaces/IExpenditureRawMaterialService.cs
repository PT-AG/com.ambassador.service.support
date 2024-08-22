using com.ambassador.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace com.ambassador.support.lib.Interfaces
{
    public interface IExpenditureRawMaterialService
    {
        Task<Tuple<List<ExpenditureRawMaterialViewModel>, int>> GetReport(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string Order, int offset);
        Task<MemoryStream> GenerateExcel(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset);
    }
}
