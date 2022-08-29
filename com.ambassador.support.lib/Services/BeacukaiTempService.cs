using com.ambassador.support.lib.Models;
using com.ambassador.support.lib.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ambassador.support.lib.Services
{
    public class BeacukaiTempService : IBeacukaiTempService
    {
        private readonly SupportDbContext context;

        public BeacukaiTempService(SupportDbContext context)
        {
            this.context = context;
        }

        public List<BEACUKAI_TEMPViewModel> Get(int size = 25, string keyword = null)
        {
            string[] bcType = { "BC 262", "BC 23", "BC 40", "BC 27" };

            IQueryable<BEACUKAI_TEMP> Query = context.BeacukaiTemp.Where(s => bcType.Contains(s.JenisBC) && s.TglBCNo.Year >= DateTime.Now.Year - 1 && s.Barang != null);


            var Query2 = Query
                .Select(p => new BEACUKAI_TEMPViewModel
                {

                    BCNo = p.BCNo,
                    BCId = p.BCId, //BonNo = p.BonNo,
                    TglBCNo = p.TglBCNo, //BCDate = p.BCDate,
                    JenisBC = p.JenisBC, //BCType = p.BCType,
                    TglDatang = p.TglDatang,
                    Hari = p.Hari,
                    Netto = Convert.ToDouble(p.Netto),
                    Bruto = Convert.ToDouble(p.Bruto)
                });

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                Query2 = Query2.Where(s => s.BCNo.StartsWith(keyword));
            }

            Query2 = Query2
                .OrderBy(o => o.BCNo)
                .Distinct();

            var Query3 = Query2.GroupBy(x => new { x.BCNo, x.JenisBC, x.TglBCNo }, (key, group) => new BEACUKAI_TEMPViewModel
            {
                BCNo = key.BCNo,
                BCId = group.FirstOrDefault().BCId, //BonNo = p.BonNo,
                TglBCNo = key.TglBCNo, //BCDate = p.BCDate,
                JenisBC = key.JenisBC, //BCType = p.BCType,
                TglDatang = group.FirstOrDefault().TglDatang,
                Hari = group.FirstOrDefault().Hari,
                Netto = Convert.ToDouble(group.Sum(x=>x.Netto)),
                Bruto = Convert.ToDouble(group.FirstOrDefault().Bruto)
            });

            Query3 = Query3
                .Take(size);

            return Query3.ToList();
        }


    }

    public interface IBeacukaiTempService
    {
        List<BEACUKAI_TEMPViewModel> Get(int size = 25, string keyword = null);
    }
}
