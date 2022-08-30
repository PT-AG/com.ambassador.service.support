using com.ambassador.support.lib.Helpers;
using com.ambassador.support.lib.ViewModel;
using Com.Moonlay.NetCore.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace com.ambassador.support.lib.Services
{
    public class ExpenditureRawMaterialService
    {
        SupportDbContext context;
        public ExpenditureRawMaterialService(SupportDbContext _context)
        {
            this.context = _context;
        }
        public IQueryable<ExpenditureRawMaterialViewModel> getQuery(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int offset)
        {
            DateTimeOffset d1 = dateFrom.Value.AddHours(offset);
            DateTimeOffset d2 = dateTo.Value.AddHours(offset);

            //string DateFrom = d1.ToString("yyyy-MM-dd");
            //string DateTo = d2.ToString("yyyy-MM-dd");

            List<ExpenditureRawMaterialViewModel> reportData = new List<ExpenditureRawMaterialViewModel>();

            try
            {
                string connectionString = APIEndpoint.ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "declare @StartDate datetimeoffset = '" + d1 + "' declare @EndDate datetimeoffset = '" + d2 + "' " +
                        "select l.UENNo,convert(date,dateadd(hour,7,l.ExpenditureDate)) as 'Tanggal Keluar',k.ProductCode,k.ProductName,k.UomUnit,k.Quantity,l.ExpenditureType from GarmentDeliveryOrders a  " +
                        "join GarmentDeliveryOrderItems b on a.id=b.GarmentDOId join GarmentDeliveryOrderDetails c on b.id=c.GarmentDOItemId " +
                        "join GarmentBeacukaiItems d on d.GarmentDOId=a.id join GarmentBeacukais e on e.id=d.BeacukaiId " +
                        "join GarmentUnitReceiptNotes f on a.id=f.DOId join GarmentUnitReceiptNoteItems g on f.id=g.URNId " +
                        "left join GarmentDOItems h on h.URNItemId=g.Id " +
                        "left join GarmentUnitDeliveryOrderItems i on i.DOItemsId=h.Id join GarmentUnitDeliveryOrders j on j.id=i.UnitDOId " +
                        "left join GarmentUnitExpenditureNoteItems k on k.UnitDOItemId=i.Id join GarmentUnitExpenditureNotes l on l.id=k.UENId " +
                        "where l.CreatedUtc between @StartDate and @EndDate", conn))

                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                        DataSet dSet = new DataSet();
                        dataAdapter.Fill(dSet);
                        foreach (DataRow data in dSet.Tables[0].Rows)
                        {
                            ExpenditureRawMaterialViewModel view = new ExpenditureRawMaterialViewModel
                            {
                                //ExpenditureGoodId = data["ExpenditureGoodId"].ToString(),
                                //RO = data["RO"].ToString(),
                                //Article = data["Article"].ToString(),
                                //UnitCode = (int)data["UnitCode"] == 1 ? "KONF 2A/EX. K1" : (int)data["UnitCode"] == 2 ? "KONF 2B/EX. K2" : (int)data["UnitCode"] == 3 ? "KONF 1A/EX. K3" : (int)data["UnitCode"] == 4 ? "KONF 2C/EX. K4" : (int)data["UnitCode"] == 5 ? "KONF 1B/EX. K2D" : "GUDANG PUSAT",
                                //BuyerContract = data["BuyerContract"].ToString(),
                                //ExpenditureTypeName = data["ExpenditureTypeName"].ToString(),
                                //Description = data["Description"].ToString(),
                                //ComodityName = data["ComodityName"].ToString(),
                                //ComodityCode = data["ComodityCode"].ToString(),
                                //SizeNumber = data["SizeNumber"].ToString(),
                                //Descriptionb = data["Desb"].ToString(),
                                //Qty = String.Format("{0:n}", (double)data["Qty"])
                                UENNo = data["UENNo"].ToString(),
                                ExpenditureDate = data["Tanggal Keluar"].ToString(),
                                ProductCode = data["ProductCode"].ToString(),
                                ProductName = data["ProductName"].ToString(),
                                UomUnit = data["UomUnit"].ToString(),
                                //Quantity = (double)data["Quantity"],
                                Quantity = data["ExpenditureType"].ToString() != "SUBCON" ? (double)data["Quantity"] : 0,
                                QuantitySubcon = data["ExpenditureType"].ToString() == "SUBCON" ? (double)data["Quantity"] : 0,
                                ExpenditureType = data["ExpenditureType"].ToString(),
                                SubconTo = "-"

                            };
                            reportData.Add(view);
                        }
                    }
                    conn.Close();
                }
            }
            catch (SqlException ex)
            { 
            }
            return reportData.AsQueryable();
        }
        public Tuple<List<ExpenditureRawMaterialViewModel>, int> GetReport(DateTimeOffset? dateFrom, DateTimeOffset? dateTo, int page, int size, string Order, int offset)
        {
            var Query = getQuery(dateFrom, dateTo, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.ExpenditureDate);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                //Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }


            Pageable<ExpenditureRawMaterialViewModel> pageable = new Pageable<ExpenditureRawMaterialViewModel>(Query, page - 1, size);
            List<ExpenditureRawMaterialViewModel> Data = pageable.Data.ToList<ExpenditureRawMaterialViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
    }
}
