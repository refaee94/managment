using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication7.Models;

namespace WebApplication7.Controllers
{
    [Authorize]
    public class inquiriesController : Controller
    {
        private AccountingEntities1 db = new AccountingEntities1();

        // GET: inquiries
        public ActionResult Index()
        {
            var inquiries = db.inquiries.Include(i => i.AspNetUser);
            var id = HttpContext.User.Identity.GetUserId();
            var isAdmin = HttpContext.User.IsInRole("ADMIN");
            if (!isAdmin)
                inquiries = inquiries.Where(i => i.Employee == id);
            ViewBag.isAdmin = isAdmin;
            return View(inquiries.ToList());
        }

        // GET: inquiries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            inquiry inquiry = db.inquiries.Find(id);
            ViewBag.creater = db.AspNetUsers.Where(u => u.Id == inquiry.Employee);
            if (inquiry == null)
            {
                return HttpNotFound();
            }
            return View(inquiry);
        }

        // GET: inquiries/Create
        public ActionResult Create()
        {
            ViewBag.Employee = new SelectList(db.AspNetUsers, "Id", "Email");

            return View();
        }

        // POST: inquiries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Company,Employee,CreationDate")] inquiry inquiry, HttpPostedFileBase files)
        {
            if (ModelState.IsValid)
            {
                if (files != null && files.ContentLength > 0)
                {
                    // extract only the filename
                    var fileName = Path.GetFileName(files.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    files.SaveAs(path);
                    inquiry.Images.Add(new Image { Url = path, fileName = fileName });

                }


                inquiry.Employee = HttpContext.User.Identity.GetUserId();
                inquiry.CreationDate = DateTime.Now.Date;
                db.inquiries.Add(inquiry);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee = new SelectList(db.AspNetUsers, "Id", "Email", inquiry.Employee);
            return View(inquiry);
        }

        // GET: inquiries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            inquiry inquiry = db.inquiries.Find(id);
            if (inquiry == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee = new SelectList(db.AspNetUsers, "Id", "Email", inquiry.Employee);
            return View(inquiry);
        }

        // POST: inquiries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Company,Employee,CreationDate")] inquiry inquiry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inquiry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee = new SelectList(db.AspNetUsers, "Id", "Email", inquiry.Employee);
            return View(inquiry);
        }

        // GET: inquiries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            inquiry inquiry = db.inquiries.Find(id);
            if (inquiry == null)
            {
                return HttpNotFound();
            }
            return View(inquiry);
        }

        // POST: inquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            inquiry inquiry = db.inquiries.Find(id);
            db.inquiries.Remove(inquiry);
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
