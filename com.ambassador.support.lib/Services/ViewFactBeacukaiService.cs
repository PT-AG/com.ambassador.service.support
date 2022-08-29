using com.ambassador.support.lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ambassador.support.lib.Services
{
    public class ViewFactBeacukaiService : IViewFactBeacukaiService
    {
        private readonly SupportDbContext context;

        public ViewFactBeacukaiService(SupportDbContext context)
        {
            this.context = context;
        }

        public List<ViewFactBeacukai> Get(int size = 25, string keyword = null)
        {
            string[] bcType = { "BC 2.6.1", "BC 3.0", "BC 4.1", "BC 2.5" };

            IQueryable<ViewFactBeacukai> Query = context.ViewFactBeacukai.Where(s => bcType.Contains(s.BCType));

            Query = Query
                .Select(p => new ViewFactBeacukai
                {

                    BCNo = p.BCNo
                    
                });

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                Query = Query.Where(s => s.BCNo.StartsWith(keyword));
            }

            Query = Query
                .OrderBy(o => o.BCNo)
                .Distinct();

            Query = Query
                .Take(size);

            return Query.ToList();
        }


    }

    public interface IViewFactBeacukaiService
    {
        List<ViewFactBeacukai> Get(int size = 25, string keyword = null);
    }
}

