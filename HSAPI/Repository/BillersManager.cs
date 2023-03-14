using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.IO;
using System.Web.Script.Services;
using HSAPI.Models;
using System.Data.Services.Client;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace HSAPI.Repository
{
    public class BillersManager
    {
        public static string connection = "Data Source=XXXXXX;Initial Catalog=DatabaseName;Integrated Security=false;User ID=SA;Password=123;Pooling=false;";

        //HttpContext context = HttpContext.Current;


        //public IEnumerable<ShowPaybillTransactions> ViewPaybillTransactions()
        //{
        //    List<ShowPaybillTransactions> TFGList = new List<ShowPaybillTransactions>();
        //    SqlConnection Conn = new SqlConnection(connection);
        //    {
        //        SqlCommand cmd = new SqlCommand("Show_Paybill_Transactions", Conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        Conn.Open();
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            ShowPaybillTransactions BaypillInfo = new ShowPaybillTransactions();
        //            BaypillInfo.ID = dr["ID"].ToString();
        //            BaypillInfo.tansactionId = dr["tansactionId"].ToString();
        //            BaypillInfo.totalAmount = dr["totalAmount"].ToString().Replace("0.0000", "0.00");
        //            BaypillInfo.currency = dr["currency"].ToString();
        //            BaypillInfo.studentId = dr["studentId"].ToString();
        //            BaypillInfo.studentName = dr["studentName"].ToString();
        //            BaypillInfo.paidAt = dr["paidAt"].ToString();
        //            BaypillInfo.amount = dr["amount"].ToString().Replace("0.0000", "0.00");
        //            TFGList.Add(BaypillInfo);
        //        }
        //        Conn.Dispose();
        //        Conn.Close();
        //    }
        //    return TFGList;
        //}
          
    }

    }