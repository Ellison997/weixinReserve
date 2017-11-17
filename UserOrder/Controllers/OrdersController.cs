using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserOrder.Models;
using UserOrder.Controllers;
using System.Configuration;

namespace UserOrder.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        public ActionResult Index()
        {
            List<Order> listOrder = (from o in db.OrderSet orderby o.Id descending select o).Take(5).ToList();
            List<Order> Orders = new List<Order>();
            foreach (Order lo in listOrder) {
                int count = (from uoi in db.UserOrderInfoSet where  uoi.OrderId==lo.Id && uoi.State=="已预订" select uoi).Count();
                lo.Amount = count;
                lo.Describe = lo.Describe.Replace("\n", "<br />");
               
                Orders.Add(lo);
            }
            //判断今日是否有菜单发布
            DateTime Tooday = DateTime.Now.Date;
            DateTime Enday = DateTime.Today.AddDays(86399F / 86400);
            IQueryable<Order> orders = db.OrderSet.Where(o=>o.OrderDate>=Tooday && o.OrderDate<Enday);
            if (orders.Count() >= 1)
            {
                ViewData["release"] = "今日已发布";
            }
            else {
                ViewData["release"] = "未发布";
            }
            return View(Orders);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Order order = db.OrderSet.Find(id);
            order.Describe=order.Describe.Replace("\n", "<br />");
            List<UserOrderInfo> uois = (from uoi in db.UserOrderInfoSet where uoi.OrderId == id select uoi).OrderBy(C => C.State).ToList();
            if (uois == null)
            {
                return HttpNotFound();
            }
            ViewBag._Order = order;
           
            return View(uois);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OrderDate,Describe,Amount")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.OrderSet.Add(order);
                db.SaveChanges();
                SendMessageController smc = new SendMessageController();
                String content = "今日菜单：" + order.Describe;
                String toParty = ConfigurationManager.AppSettings["toParty"];
                smc.SendTest(content, toParty);
                return RedirectToAction("Index");
            }

            
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.OrderSet.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderDate,Describe,Amount")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.OrderSet.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.OrderSet.Find(id);
            db.OrderSet.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
