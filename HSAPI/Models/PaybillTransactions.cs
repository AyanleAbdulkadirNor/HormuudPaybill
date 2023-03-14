using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace HSAPI.Models

{
    public class ShowPaybillTransactions
    {
        public string ID { get; set; }
        public string tansactionId { get; set; }
        public string totalAmount { get; set; }
        public string currency { get; set; }
        public string studentId { get; set; }
        public string studentName { get; set; }
        public string paidAt { get; set; }
        public string amount { get; set; }
        //tansactionId,totalAmount,currency,studentId,studentName,paidAt,amount

    }
}
