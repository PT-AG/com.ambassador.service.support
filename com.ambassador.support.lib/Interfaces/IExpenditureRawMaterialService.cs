using com.ambassador.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.ambassador.support.lib.Interfaces
{
    public interface IExpenditureRawMaterialService
    {
        Tuple<List<ExpenditureRawMaterialViewModel>, int> GetReport(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcel(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset);
    }
}
