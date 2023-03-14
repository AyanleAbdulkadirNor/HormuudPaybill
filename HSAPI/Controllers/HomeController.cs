using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HSAPI.Repository;
using HSAPI.Models;

namespace HSAPI.Controllers
{
    public class HomeController : Controller
    {
        BillersManager tr = new BillersManager();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
  
        //[HttpPost]
        //public ActionResult Index1()
        //{ 
        //    //List<ShowPaybillTransactions> ViewPaybillTransactions = new List<ShowPaybillTransactions>();
        //    //ViewPaybillTransactions = tr.ViewPaybillTransactions().ToList();
        //    ////ViewBag.TblData = ViewPaybillTransactions;
        //    //return View(ViewPaybillTransactions);
        //}
         
    }
}
