using Com.Moonlay.NetCore.Lib;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Data;
using com.ambassador.support.lib.Helpers;
using com.ambassador.support.lib.Models;
using com.ambassador.support.lib.ViewModel;
using OfficeOpenXml;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace com.ambassador.support.lib.Services
{
    public class HOrderService
    {
        SupportDbContext context;
        public HOrderService(SupportDbContext _context)
        {
            this.context = _context;
        }

        public List<HOrder> ReadModel(int size)
        {
            return this.context.HOrder.Take(size).ToList();
        }


        public IQueryable<HOrder> GetHOrderQuery(string no, int offset)
        {

            var Query = (from a in context.HOrder
                         where a.No == no
                         select a);
            return Query;
        }
        public Tuple<List<HOrder>, int> GetHOrder(string no, int page, int size, string Order, int offset)
        {
            var Query = GetHOrderQuery(no, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            var q = Query.ToList();
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.No);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Query = q.AsQueryable();


            Pageable<HOrder> pageable = new Pageable<HOrder>(Query, page - 1, size);
            List<HOrder> Data = pageable.Data.ToList<HOrder>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }


        public IQueryable<HOrder> GetHOrders(string Keyword = null, string Filter = "{}")
        {
            //string[] bcType = { "BC 262", "BC 23", "BC 40", "BC 27" };
            IQueryable<HOrder> Query = context.HOrder.OrderBy(s=>s.No);
            Query = Query
                .Select(p => new HOrder
                {
                    No = p.No,
                    Kode = p.Kode,
                }).Where(s => s.No == Keyword);

            return Query.Distinct();
        }
    }
}
