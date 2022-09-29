using Xunit;
using System.Threading.Tasks;
using System.Data.SqlClient;
using com.ambassador.support.lib.Helpers;
using System.Transactions;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System;
using com.ambassador.support.lib.Services;

namespace com.ambassador.support.Test.Service
{
    public class ExpenditureRawMaterialServiceTest
    {
        //private ExpenditureRawMaterialService GetService ()
        //{
        //    ExpenditureRawMaterialService expenditureRawMaterialService = new ExpenditureRawMaterialService();
        //    string connectionString = APIEndpoint.ConnectionString;

        //    return expenditureRawMaterialService;
        //}

        //[Fact]
        //public async Task GetState_Expect_ExpendRaw()
        //{

        //    ExpenditureRawMaterialService service = GetService();

        //    var response = service.GetReport(DateTimeOffset.MinValue, DateTimeOffset.Now, 1, 1, "", 1);

        //    Assert.NotNull(response);

        //    //DateTimeOffset d1 = DateTimeOffset.MinValue;
        //    //DateTimeOffset d2 = DateTimeOffset.Now;
        //    //string connectionString = APIEndpoint.ConnectionString;
        //    //using (TransactionScope ts = new TransactionScope())

        //    //{
        //    //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    //    {
        //    //        conn.Open();
        //    //        using (SqlCommand cmd = new SqlCommand(
        //    //            "declare @StartDate datetimeoffset = '" + d1 + "' declare @EndDate datetimeoffset = '" + d2 + "' " +
        //    //            "select l.UENNo,convert(date,dateadd(hour,7,l.ExpenditureDate)) as 'Tanggal Keluar',k.ProductCode,k.ProductName,k.UomUnit,k.Quantity,l.ExpenditureType from GarmentDeliveryOrders a  " +
        //    //            "join GarmentDeliveryOrderItems b on a.id=b.GarmentDOId join GarmentDeliveryOrderDetails c on b.id=c.GarmentDOItemId " +
        //    //            "join GarmentBeacukaiItems d on d.GarmentDOId=a.id join GarmentBeacukais e on e.id=d.BeacukaiId " +
        //    //            "join GarmentUnitReceiptNotes f on a.id=f.DOId join GarmentUnitReceiptNoteItems g on f.id=g.URNId " +
        //    //            "left join GarmentDOItems h on h.URNItemId=g.Id " +
        //    //            "left join GarmentUnitDeliveryOrderItems i on i.DOItemsId=h.Id join GarmentUnitDeliveryOrders j on j.id=i.UnitDOId " +
        //    //            "left join GarmentUnitExpenditureNoteItems k on k.UnitDOItemId=i.Id join GarmentUnitExpenditureNotes l on l.id=k.UENId " +
        //    //            "where DATEADD(HOUR,7,l.CreatedUtc) between @StartDate and @EndDate", conn))

        //    //        {
        //    //            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
        //    //            DataSet dSet = new DataSet();
        //    //            dataAdapter.Fill(dSet);
        //    //            DataRow[] dr1 = dSet.Tables[0].Select("[ExpenditureType]");
        //    //            DataRow[] dr2 = dSet.Tables[0].Select("[ExpenditureType]");
        //    //            Assert.Equal(dr2.Length, dr2.Length);

        //    //        }
        //    //    }
        //    //}

        //}
    }
}
