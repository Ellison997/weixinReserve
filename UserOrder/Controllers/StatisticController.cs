using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserOrder.Models;

namespace UserOrder.Controllers
{
    public class StatisticController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Statistic
        public ActionResult Index()
        {
            List<Statistic> listsMonth = new List<Statistic>();
            //根据月份对数据进行分组
            listsMonth = (from o in db.OrderSet
                     join uoi in db.UserOrderInfoSet on o.Id equals uoi.OrderId
                     where o.OrderDate.Year == DateTime.Now.Year && uoi.State=="已预订"
                     group o by new
                     {
                         o.OrderDate.Month
                     } into g
                     orderby g.Key.Month descending
                     select new Statistic
                     {
                         Month = g.Key.Month,
                         Amount = g.Count(),
                         
                     }).ToList();


            List<Statistic> listsYear = new List<Statistic>();
            listsYear = (from o in db.OrderSet
                         join uoi in db.UserOrderInfoSet on o.Id equals uoi.OrderId
                         where uoi.State=="已预订"
                         group o by new
                         {
                             o.OrderDate.Year
                         } into g
                         orderby g.Key.Year descending
                         select new Statistic
                         {
                             Year = g.Key.Year,
                             Amount = g.Count()

                         }).ToList();
            ViewData["listsYear"] = listsYear;
            Statistic st = listsYear.FirstOrDefault();
            ViewData["yearAmount"] = st.Amount;

            return View(listsMonth);
        }
        //MonthDetails Get
        public ActionResult MonthDetails(int year,int month)
        {
            List<Order> listOrder = (from o in db.OrderSet where o.OrderDate.Month == month && o.OrderDate.Year == year orderby o.Id descending select o).ToList();
            List<Order> Orders = new List<Order>();
            foreach (Order lo in listOrder)
            {
                int count = (from uoi in db.UserOrderInfoSet where uoi.OrderId == lo.Id && uoi.State == "已预订" select uoi).Count();
                lo.Amount = count;
                lo.Describe = lo.Describe.Replace("\n", "<br />");

                Orders.Add(lo);
            }
            
            ViewBag.Year = year;
            String smonth = month.ToString();

            if (month < 10)
            {
                smonth = "0" + month;
            }

            ViewBag.Month = smonth;
            
            
            return View(Orders);
        }
    }
}