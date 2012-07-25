using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.Models;

namespace FT.Controllers
{
    public class GalleryController : Controller
    {
        public static ftEntities db;

        public GalleryController()
        {
            db = new ftEntities();
        }
        //
        // GET: /Gallery/

        public ActionResult Index()
        {
            var galleries = from g in db.galleries
                            select g;
            List<gallery> galleryList = galleries.ToList();
            return View(galleryList);
        }

        //
        // GET: /Gallery/Details/5

        public ActionResult Details(int galleryId)
        {
            var gItems = from g in db.gallery_gallery_items
                            where g.gallery_Id == galleryId
                            select g;
            List<gallery_gallery_items> gList = gItems.ToList();
            return View(gList);
        }

        //
        // GET: /Gallery/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Gallery/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Gallery/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Gallery/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Gallery/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Gallery/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
