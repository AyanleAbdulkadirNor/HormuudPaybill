using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using PInvoke;
using HSAPI.Models;
using static HSAPI.Controllers.billingController;
using System.Dynamic;
using System.Net.Sockets;
using System.Security.Claims;
using Microsoft.Ajax.Utilities;
using Jose;
using System.Runtime.Remoting.Messaging;
using System.IdentityModel.Tokens.Jwt;
using System.Collections;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace HSAPI.Controllers
{
    public class billingController : ApiController
    {
        //CONNCECTIONS
        public static string connection = "Data Source=10.0.5.3;Initial Catalog=HormuudServiceCenter;Integrated Security=false;User ID=SMSgateway_user;Password=developers_2013;Pooling=false;";

        // QueryBillInfo: GET STUDENT INFORMATION
        [HttpPost]
        public HttpResponseMessage query(QueryBillInfoModel Info)
        {
            var authorization = Request.Headers.GetValues("authorization");
            var ItemaAthorization = authorization.ToList();
            var header = ItemaAthorization[0];
            var username = "simad";
            var studentId = "";
            var partnerCOde = "111831";
            var result = new BilInfo();
            var apiPassword = HashSHA512(username + Info.timestamp);
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(partnerCOde + ":" + apiPassword));

            try
            {
                if (svcCredentials == header)
                {
                    //Decode
                    var tokenString = Info.requestBody;
                    // trim 'Bearer ' from the start since its just a prefix for the token string
                    var jwtEncodedString = tokenString;
                    var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                    //var data = token[0]["studentId"];
                    var claims = token.Claims.ToList();
                    var data = claims;
                    if (data.Count > 0)
                    {
                        studentId = data[0].Value;
                    }

                    //Get Fucntion CHECKSTUDENTS
                    result = CHECKSTUDENTS(studentId);

                    if (result.ResultCode == "401")
                    {
                        var dataNotFound = new
                        {
                            message = "NotFound",
                            ResultCode = "401"
                        };
                        return Request.CreateResponse(HttpStatusCode.NotFound, dataNotFound);
                    }

                    var item = new List<Item>();
                    item = result.item;
                    List<BilInfoObj> obj = new List<BilInfoObj>();
                    obj.Add(new BilInfoObj()
                    {
                        studentId = result.studentId,
                        studentName = result.studentName,
                        totalAmount = result.totalAmount,
                        currencyCode = result.currencyCode,
                        item = item,
                    });

                    var json = new

                    {
                        //requestId = Info.requestId,
                        schemaVersion = "1.0",
                        responseHeader = new
                        {
                            timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                            resultCode = result.ResultCode,
                            resultMessage = result.ReplyMessage
                        },

                        studentId = result.studentId,
                        studentName = result.studentName,
                        totalAmount = result.totalAmount,
                        currencyCode = result.currencyCode,
                        bills = item
                    };

                    var postData = JsonConvert.SerializeObject(json);
                    var postJson = _UpdateJSON(postData);
                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(postJson));

                }

                else
                {
                    var data = new
                    {
                        message = "Unauthorized",
                        ResultCode = "401"
                    };
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, data);
                }

            }
            catch (Exception ex)
            {
                var data = new
                {
                    message = "Something went wrong please try again",
                    //ResultCode = "400"
                };
                return Request.CreateResponse(HttpStatusCode.Unauthorized, data);
            }
        }

        //CONVERT content_type TO content-type
        public static string _UpdateJSON(string json)
        {
            json = json.Replace("content_type", "content-type");
            return json;
        }

        //CHECKSTUDENTS : FUNCTION GET DATABASE STUDENT INFORMATION
        public static BilInfo CHECKSTUDENTS(string studentId)
        {
            var result = new BilInfo();
            //var result1 = new Item();

            try
            {
                BilInfo BaypillInfo = new BilInfo();
                BaypillInfo.item = new List<Item>();
                SqlConnection con = new SqlConnection(connection);
                {
                    con.Open();

                    DataSet Myds1 = new DataSet();
                    SqlDataAdapter sqldr1 = new SqlDataAdapter();
                    SqlCommand cmd = new SqlCommand("[Hormuud_Test_Paybill]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add("@studentId", SqlDbType.VarChar, 50).Value = studentId;
                    sqldr1.SelectCommand = cmd;
                    sqldr1.Fill(Myds1);
                    DataTable dt1 = Myds1.Tables[0];
                    int i = 0;

                    if (dt1.Rows.Count > 0)
                    {
                        BaypillInfo.studentId = Convert.ToString(dt1.Rows[i]["studentId"]);
                        BaypillInfo.studentName = Convert.ToString(dt1.Rows[i]["studentName"]);
                        BaypillInfo.totalAmount = Convert.ToDouble(dt1.Rows[i]["totalAmount"]);
                        BaypillInfo.currencyCode = Convert.ToString(dt1.Rows[i]["currencyCode"]);
                        BaypillInfo.item = GetProducts(); // Get Fucntion Item
                        if (Convert.ToDouble(dt1.Rows[i]["totalAmount"]) > 0)
                        {
                            result = BaypillInfo;
                            result.ReplyMessage = "Success";
                            result.ResultCode = "0";
                        }

                        else if (Convert.ToDouble(dt1.Rows[i]["totalAmount"]) == 0)
                        {
                            result = BaypillInfo;
                            result.ReplyMessage = "AlreadyPaidBill";
                            result.ResultCode = "402";
                        }

                    }
                    else
                    {
                        result = BaypillInfo;
                        result.ReplyMessage = "NofFound";
                        result.ResultCode = "401";
                    }

                    con.Close();
                    con.Dispose();


                }

            }
            catch (Exception ex)
            {
                result.ReplyMessage = ex.Message;
                //result.ReplyMessage = "Not Found";
                result.ResultCode = "3";
                return result;
            }
            return result;
        }

        //GetProducts : FUNCTION GET Products
        public static List<Item> GetProducts()
        {
            var result = new List<Item>();
            List<Item> ItemInfo = new List<Item>();
            try
            {
                SqlConnection con = new SqlConnection(connection);
                {
                    con.Open();

                    DataSet Myds1 = new DataSet();
                    SqlDataAdapter sqldr1 = new SqlDataAdapter();
                    SqlCommand cmd = new SqlCommand("[GET_Products]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    //cmd.Parameters.Add("@studentId", SqlDbType.VarChar, 50).Value = studentId;
                    sqldr1.SelectCommand = cmd;
                    sqldr1.Fill(Myds1);
                    DataTable dt1 = Myds1.Tables[0];
                    int i = 0;
                    for (i = 0; i < dt1.Rows.Count; i++)
                    {
                        ItemInfo.Add(new Item()
                        {
                            accountNumber = Convert.ToString(dt1.Rows[i]["accountNumber"]),
                            accountTitle = Convert.ToString(dt1.Rows[i]["accountTitle"]),
                            amount = Convert.ToString(dt1.Rows[i]["Amount"]).Replace("0.0000", "0.00"),
                            description = Convert.ToString(dt1.Rows[i]["description"]),
                            dueDate = Convert.ToString(dt1.Rows[i]["dueDate"]),
                            isPartialPayAllowed = Convert.ToString(dt1.Rows[i]["isPartialPayAllowed"]),
                        });
                    }

                    con.Close();
                    con.Dispose();
                }

            }
            catch (Exception ex)
            {
                //result.ReplyMessage = ex.Message;
                //result.ReplyMessage = "Not Found";
                //result.ResultCode = "3";
                return result;
            }
            return ItemInfo;
        }

        //PayBillNotification: INSERT STUDENT INFORMATION for paid
        [HttpPost]
        public HttpResponseMessage pay(PayBillNotificationModel Info)
        {
            var authorization = Request.Headers.GetValues("authorization");
            var ItemaAthorization = authorization.ToList();
            var header = ItemaAthorization[0];
            var username = "simad";
            var partnerCOde = "111831";
            var apiPassword = HashSHA512(username + Info.timestamp);
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(partnerCOde + ":" + apiPassword));
            var result = new confirmationIdinfo();
            var result1 = new Item();
            try
            {
                if (svcCredentials == header)
                {
                    //Decode
                    var tokenString = Info.requestBody;
                    // trim 'Bearer ' from the start since its just a prefix for the token string
                    var jwtEncodedString = tokenString;
                    JwtSecurityToken token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                    JwtPayload jwtpayload = token.Payload;
                    var jsonobj = jwtpayload.SerializeToJson();
                    JObject jPayBill = new JObject(JObject.Parse(jsonobj));
                    PaybillModel Paybill = jPayBill.ToObject<PaybillModel>(); 
                    var claims = token.Claims.ToList();
                    var data = claims;

                    if (data.Count > 0)
                    {

                        result = CHECKPayBillNotification(Paybill.transacionInfo.tansactionId, Paybill.transacionInfo.totalAmount, Paybill.transacionInfo.currency, Paybill.payerInfo.studentId, Paybill.payerInfo.studentName, Paybill.payerInfo.paidAt, Paybill.bills.ToList());
                    }

                    if (result.ResultCode == "401")
                    {
                        var dataNotFound = new
                        {
                            message = "NotFound",
                            ResultCode = "401"
                        };
                        return Request.CreateResponse(HttpStatusCode.NotFound, dataNotFound);
                    }

                    var json = new

                    {
                        schemaVersion = "1.0",
                        responseHeader = new
                        {
                            timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                            resultCode = result.ResultCode,
                            resultMessage = result.ReplyMessage
                        },
                        confirmationId = result.confirmationId, 
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }

                    else
                    {
                        var resp = new
                        {
                            message = "Unauthorized",
                            ResultCode = "401"
                        };
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, resp);
                    }

                }
            catch (Exception ex)
            {
                var data = new
                {
                    message = "Something went wrong please try again",
                    //id = "400"
                };
                return Request.CreateResponse(HttpStatusCode.Unauthorized, data);
            } 
        }

        //PayBillNotification : FUNCTION INSERT DATABASE STUDENT INFORMATION
        public static confirmationIdinfo CHECKPayBillNotification(string tansactionId, string totalAmount, string currency, string studentId, string studentName, string paidAt, List<Bill> bills)
        {

            var result = new confirmationIdinfo();

            try
            {
                confirmationIdinfo PayBillNotificationInfo = new confirmationIdinfo();
                SqlConnection con = new SqlConnection(connection);
                {
                    con.Open();

                    DataSet Myds1 = new DataSet();
                    SqlDataAdapter sqldr1 = new SqlDataAdapter();
                    foreach (var item in bills)
                    {
                        SqlCommand cmd = new SqlCommand("[Pay_Bill_Notification]", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add("@tansactionId", SqlDbType.VarChar, 50).Value = tansactionId;
                        cmd.Parameters.Add("@totalAmount", SqlDbType.Money).Value = totalAmount;
                        cmd.Parameters.Add("@currency", SqlDbType.VarChar, 50).Value = currency;
                        cmd.Parameters.Add("@studentId", SqlDbType.VarChar, 50).Value = studentId;
                        cmd.Parameters.Add("@studentName", SqlDbType.VarChar, 50).Value = studentName;
                        cmd.Parameters.Add("@paidAt", SqlDbType.VarChar, 50).Value = paidAt;
                        cmd.Parameters.Add("@accountNumber", SqlDbType.VarChar, 50).Value = item.accountNumber;
                        cmd.Parameters.Add("@amount", SqlDbType.Money).Value = item.amount;
                        sqldr1.SelectCommand = cmd;
                        sqldr1.Fill(Myds1);
                    }

                    int i = 0;
                    DataTable dt1 = Myds1.Tables[0];

                    if (dt1.Rows.Count > 0)
                    {
                        PayBillNotificationInfo.confirmationId = Convert.ToString(dt1.Rows[i]["confirmationId"]);
                        result = PayBillNotificationInfo;
                        result.ReplyMessage = "Success";
                        result.ResultCode = "0";
                    }

                    else
                    {
                        PayBillNotificationInfo.confirmationId = Convert.ToString(dt1.Rows[i]["confirmationId"]);
                        result = PayBillNotificationInfo;
                        result.ReplyMessage = "No Found";
                        result.ResultCode = "2";
                    }

                    con.Close();
                    con.Dispose();

                    if (i > 0)
                    {
                        result = PayBillNotificationInfo;
                        result.ReplyMessage = "Not Found";
                        result.ResultCode = "2";
                        return result;
                    }
                    else
                    {
                        result = PayBillNotificationInfo;
                        result.ReplyMessage = "Success";
                        result.ResultCode = "0";
                        return result;

                    }
                }
            }
            catch (Exception ex)
            {
                //result.ReplyMessage = ex.Message;
                result.ReplyMessage = "Sorry something went wrong.";
                result.ResultCode = "3";
                return result;
            }
        }

        //HashSHA512: FUNCTION HashSHA512 PASSWORD
        public static string HashSHA512(string input)
        {
            SHA512Managed crypt = new SHA512Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input));

            foreach (byte append in crypto)
            {
                hash.Append(append.ToString("x2"));
            }
            return hash.ToString();
        }
 
    }
}

