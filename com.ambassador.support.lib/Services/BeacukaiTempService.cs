using com.ambassador.support.lib.Models;
using com.ambassador.support.lib.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<BEACUKAI_ToDeleteViewModel>> GetDataForDelete(string jenis, string type, string nomor)
        {
            List<BEACUKAI_ToDeleteViewModel> data = new List<BEACUKAI_ToDeleteViewModel>();
            if (jenis == "PIB")
            {
                var Query = type == "Nomor Aju" ?
                    await context.BeacukaiTemp.Where(x => x.NoAju.Contains(nomor) && (x.TglBCNo.Month >= DateTime.Now.Month - 2 && x.TglBCNo.Year >= DateTime.Now.Year - 1))
                    .Select(a => new BEACUKAI_ToDeleteViewModel
                    {
                        NoAju = a.NoAju,
                        TglBCNo = a.TglBCNo,
                        BCNo = a.BCNo,
                        JenisBC = a.JenisBC
                    })
                    .ToListAsync() :
                    await context.BeacukaiTemp.Where(x => x.BCNo.Contains(nomor) && (x.TglBCNo.Month >= DateTime.Now.Month - 2 && x.TglBCNo.Year >= DateTime.Now.Year - 1))
                     .Select(a => new BEACUKAI_ToDeleteViewModel
                     {
                         NoAju = a.NoAju,
                         TglBCNo = a.TglBCNo,
                         BCNo = a.BCNo,
                         JenisBC = a.JenisBC
                     }).ToListAsync();

                var QueryGroup = Query.GroupBy(x => new { x.NoAju, x.TglBCNo, x.BCNo, x.JenisBC, x.CreatedBy }, (key, group) => new BEACUKAI_ToDeleteViewModel
                {
                    NoAju = key.NoAju,
                    TglBCNo = key.TglBCNo,
                    BCNo = key.BCNo,
                    JenisBC = key.JenisBC,
                    CreatedBy = key.CreatedBy
                });

                foreach (var a in QueryGroup)
                {
                    data.Add(a);
                }
            }
            else if (jenis == "PEB")
            {
                var Query = type == "Nomor Aju" ?
                   await context.BEACUKAI_ADDED.Where(x => x.CAR.Contains(nomor) && (x.BCDate.Month >= DateTime.Now.Month - 2 && x.BCDate.Year >= DateTime.Now.Year - 1))
                   .Select(a => new BEACUKAI_ToDeleteViewModel
                   {
                       NoAju = a.CAR,
                       TglBCNo = a.BCDate,
                       BCNo = a.BCNo,
                       JenisBC = a.BCType,
                       CreatedBy = a.CreateUser
                   })
                   .ToListAsync() :
                   await context.BEACUKAI_ADDED.Where(x => x.BCNo.Contains(nomor) && (x.BCDate.Month >= DateTime.Now.Month - 2 && x.BCDate.Year >= DateTime.Now.Year - 1))
                    .Select(a => new BEACUKAI_ToDeleteViewModel
                    {
                        NoAju = a.CAR,
                        TglBCNo = a.BCDate,
                        BCNo = a.BCNo,
                        JenisBC = a.BCType,
                        CreatedBy = a.CreateUser
                    }).ToListAsync();

                var QueryGroup = Query.GroupBy(x => new { x.NoAju, x.TglBCNo, x.BCNo, x.JenisBC, x.CreatedBy }, (key, group) => new BEACUKAI_ToDeleteViewModel
                {
                    NoAju = key.NoAju,
                    TglBCNo = key.TglBCNo,
                    BCNo = key.BCNo,
                    JenisBC = key.JenisBC,
                    CreatedBy = key.CreatedBy
                });


                foreach (var a in QueryGroup)
                {
                    data.Add(a);
                }
            }

            return data;
        }

        public async Task<int> DeleteData(List<BEACUKAI_ToDeleteViewModel> AjuToDelete)
        {
            int Delete = 0;

            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var data in AjuToDelete)
                    {
                        if (data.Jenis == "PIB")
                        {
                            var header = context.BeacukaiTemp.Where(x => x.NoAju == data.NoAju);

                            context.BeacukaiTemp.RemoveRange(header);
                        }
                        else if (data.Jenis == "PEB")
                        {
                            var header = context.BEACUKAI_ADDED.Where(x => x.CAR == data.NoAju);

                            var detail = context.BEACUKAI_ADDED_DETAIL.Where(x => x.CAR == data.NoAju);

                            context.BEACUKAI_ADDED.RemoveRange(header);
                            context.BEACUKAI_ADDED_DETAIL.RemoveRange(detail);
                        }

                    }
                    Delete = await context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Delete;
        }


    }

    public interface IBeacukaiTempService
    {
        Task<int> DeleteData(List<BEACUKAI_ToDeleteViewModel> AjuToDelete);
        List<BEACUKAI_TEMPViewModel> Get(int size = 25, string keyword = null);
        Task<List<BEACUKAI_ToDeleteViewModel>> GetDataForDelete(string jenis, string type, string nomor);
    }
}
