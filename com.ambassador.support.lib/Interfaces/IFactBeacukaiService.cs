using com.ambassador.support.lib.Models;
using com.ambassador.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace com.ambassador.support.lib.Interfaces
{
    public interface IFactBeacukaiService
    {
        IQueryable<FactBeacukaiViewModel> GetReportINQuery(string type, DateTime? dateFrom, DateTime? dateTo, int offset);
        Tuple<List<FactBeacukaiViewModel>, int> GetReportIN(string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelIN(string type, DateTime? dateFrom, DateTime? dateTo, int offset);
        IQueryable<FactBeacukaiViewModel> GetReportOUTQuery(string type, DateTime? dateFrom, DateTime? dateTo, int offset);
        Tuple<List<FactBeacukaiViewModel>, int> GetReportOUT(string type, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);
        MemoryStream GenerateExcelOUT(string type, DateTime? dateFrom, DateTime? dateTo, int offset);
        IQueryable<BEACUKAI_TEMP> GetBeacukaiQuery(string no, int offset);
        Tuple<List<BEACUKAI_TEMP>, int> GetBeacukai(string no, int page, int size, string Order, int offset);
        IQueryable<ViewFactBeacukai> GetBEACUKAI_TEMPs(string Keyword = null, string Filter = "{}");
        List<PEBforExpenditureViewModel> GetBEACUKAI_ADDEDs(string invoice);
        List<ViewFactBeacukai> GetBEACUKAI_ADDEDbyBCNo(string bcno);
        List<ViewFactBeacukai> GetBEACUKAI_ADDEDbyDate(DateTime? dateFrom, DateTime? dateTo);




    }
}
