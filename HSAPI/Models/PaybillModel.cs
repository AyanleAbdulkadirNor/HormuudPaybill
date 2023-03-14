using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HSAPI.Models
{
    public class PaybillModel
    {
        public Transacioninfo transacionInfo { get; set; }
        public Payerinfo payerInfo { get; set; }
        public Bill[] bills { get; set; }
    }

    public class Transacioninfo
    {
        public string tansactionId { get; set; }
        public string totalAmount { get; set; }
        public string currency { get; set; }
    }

    public class Payerinfo
    {
        public string studentId { get; set; }
        public string studentName { get; set; }
        public string paidAt { get; set; }
    }

    public class Bill
    {
        public string accountNumber { get; set; }
        public string amount { get; set; }
    }
 
    public class PayBillNotificationModel
    
    {
        public string requestId { get; set; }
        public string schemaVersion { get; set; }
        public string timestamp { get; set; }
        public string requestBody { get; set; }
    }
    public class QueryBillInfoModel
    
    {
        public string requestId { get; set; }
        public string schemaVersion { get; set; }
        public string timestamp { get; set; }
        public string requestBody { get; set; }

    }
     public class confirmationIdinfo
    {
        public string confirmationId { get; set; }
        public string ReplyMessage { get; set; }
        public string ResultCode { get; set; }
    }

    public class BilInfo
    {
        public string studentId { get; set; }
        public string studentName { get; set; }
        public double totalAmount { get; set; }
        public string currencyCode { get; set; }
        public string ReplyMessage { get; set; }
        public string ResultCode { get; set; }
        public List<Item> item { get; set; }

    }
    public class BilInfoObj
    {
        public string studentId { get; set; }
        public string studentName { get; set; }
        public double totalAmount { get; set; }
        public string currencyCode { get; set; }
        public string ReplyMessage { get; set; }
        public string ResultCode { get; set; }
        public List<Item> item { get; set; }

    }

    public class Item
    {
        public string accountNumber { get; set; }
        public string accountTitle { get; set; }
        public string amount { get; set; }
        public string description { get; set; }
        public string dueDate { get; set; }
        public string isPartialPayAllowed { get; set; }
    } 
}

 