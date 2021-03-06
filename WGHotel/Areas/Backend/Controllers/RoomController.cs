﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WGHotel.Areas.Backend.Models;
using WGHotel.Controllers;
using WGHotel.Models;
using PagedList;

namespace WGHotel.Areas.Backend.Controllers
{
    [Authorize]
    public class RoomController : BaseController
    {
        public class PagedClientViewModel
        {
            public int Page { get; set; }
            public int id { get; set; }
        }

        private bool IsCanEdit { get; set; }

        public RoomController() {
            var EditDate = new RoomCanEditDate();
            var Begin = DateTime.Parse(EditDate.Begin);
            //var End = DateTime.Parse(EditDate.End).AddDays(1);
            var Now = DateTime.Now;
            if (Now < Begin)
            {
                IsCanEdit = true;
            }
            else
            {
                IsCanEdit = false;
            }

            

           // ViewBag.IsCanEdit = IsAdminUser ? true : IsCanEdit;
        }
        // GET: Backend/Room
        public ActionResult Index(PagedClientViewModel Page = null)
        {
            var id = Request.QueryString["id"];
            var IsAdminUser = (User.IsInRole("Admin") || User.IsInRole("System"))?true:false;
            ViewBag.IsCanEdit = IsAdminUser ? true : IsCanEdit;
             if (User.IsInRole("Admin") || User.IsInRole("System"))
            {
                var strPage = Request.QueryString["Page"] == null ? string.Empty : Request.QueryString["Page"];
                var page = string.IsNullOrEmpty(strPage) ? 0 : int.Parse(Request.QueryString["Page"]);
                var result = (from room in _dbzh.Room
                             join hotel in _dbzh.Hotel
                             on room.HOTELID equals hotel.ID
                             
                             select new RoomList
                             {
                                 ID = room.ID,
                                 Name = room.Name,
                                 RoomType = room.RoomType,
                                 HOTELID = hotel.ID,
                                 HotelName = hotel.Name,
                                 Quantiy = room.Quantiy.Value,
                                 Sell = room.Sell.Value,
                                 BedType = room.BedType
                             }).ToList();
                var currentPage = page < 1 ? 1 : page;
                var PageSize = 15;

                var PageModel = result.ToPagedList(currentPage, PageSize);
                return View(PageModel);
            }

            var Userid = CurrentUser == null ? 0 : CurrentUser.Id;
            var Hotel = Userid > 0 ? _dbzh.Hotel.Where(o => o.UserId == Userid).FirstOrDefault() : null;
            //var model = _dbzh.Room.Where(o=>o.HOTELID == id).ToList();
            if (Page == null || Hotel == null)
            {
                return RedirectToAction("","Hotel");
            }
            var hotelId = (Page != null && Page.id != 0) ? Page.id : Hotel.ID;
            ViewBag.HotelID = hotelId;
            var model = (from room in _dbzh.Room
                         join hotel in _dbzh.Hotel
                         on room.HOTELID equals hotel.ID
                         where hotel.ID == hotelId
                         select new RoomList
                         {
                             ID = room.ID,
                             Name = room.Name,
                             RoomType = room.RoomType,
                             HOTELID = hotel.ID,
                             HotelName = hotel.Name,
                             Quantiy = room.Quantiy.Value,
                             Sell = room.Sell.Value,
                             BedType = room.BedType
                         }).ToList();
            var currentPage1 = Page.Page < 1 ? 1 : Page.Page;
            var PageSize1 = 15;

            var PageModel1 = model.ToPagedList(currentPage1, PageSize1);

            return View(PageModel1);
        }


        public ActionResult Create(int id)
        {
            var IsAdminUser = (User.IsInRole("Admin") || User.IsInRole("System")) ? true : false;
            ViewBag.IsCanEdit = IsAdminUser ? true : IsCanEdit;

            var RoomModel = new RoomViewModel();
            RoomModel.HOTELID = id;
            //ViewBag.HotelID = id;
            ViewBag.RoomTypes = RoomModel.RoomTypeSelectList;
            ViewBag.BedTypes = new BedModel().SelectList();
            
            ViewBag.RoomFacility = RoomModel.FacilityList;

            var key = Guid.NewGuid().GetHashCode().ToString("x");
            RoomModel.ImgKey = key;
            ViewBag.ImgKey = key;
            Session[key] = new List<ImageViewModel>();

            RoomModel.Sell = 2000;
            RoomModel.Quantiy = 1;

            return View(RoomModel);
        }

        [HttpPost]
        public ActionResult Create(RoomViewModel model)
        {
            var IsAdminUser = (User.IsInRole("Admin") || User.IsInRole("System")) ? true : false;
            var RoomIsCanEdit = IsAdminUser ? true : IsCanEdit;
            if (!RoomIsCanEdit){
               return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                model.HOTELID = model.HOTELID;
                model.Create();
                return RedirectToAction("", new {id = model.HOTELID });
            }

            var RoomModel = new RoomViewModel();
            ViewBag.RoomTypes = RoomModel.RoomTypeSelectList;
            ViewBag.BedTypes = new BedModel().SelectList();
            ViewBag.RoomFacility = RoomModel.FacilityList;
            return View();
        }

        public ActionResult Edit(int id)
        {
            var IsAdminUser = (User.IsInRole("Admin") || User.IsInRole("System")) ? true : false;
            ViewBag.IsCanEdit = IsAdminUser ? true : IsCanEdit;

            var modelzh = _dbzh.Room.Find(id);
            var key = Guid.NewGuid().GetHashCode().ToString("x");
            var modelus = _dbus.Room.Where(o => o.ParentId == modelzh.ID).FirstOrDefault();
            var model = new RoomViewModel { 
                Sell = modelzh.Sell.Value,
                BedType = modelzh.BedType,
                 Enabled = modelzh.Enabled.Value,
                 Facilities = modelzh.Facilities,
                 HOTELID = modelzh.HOTELID,
                 HasBreakfast = modelzh.HasBreakfast.Value,
                 NameUs = modelus.Name,
                 NoticeUs = modelus.Notice,
                 NoticeZh = modelzh.Notice,
                 Quantiy = modelzh.Quantiy.Value,
                 RoomType = modelzh.RoomType,
                 NameZh = modelzh.Name,
                FeatureUs = modelus.Feature,
                FeatureZh = modelzh.Feature,
                ID = modelzh.ID,
                ImgKey = key
            };
            var RoomModel = new RoomViewModel();
            ViewBag.RoomTypes = RoomModel.RoomTypeSelectList;
            if (string.IsNullOrEmpty(model.BedType)) {
                ViewBag.BedTypes = new BedModel().SelectList();
            }
            else
            {
                var Beds = model.BedType.Split(',').Select(int.Parse).ToList();
                ViewBag.BedTypes = new BedModel().SelectList(Beds);
              
            }

            ViewBag.RoomFacility = RoomModel.FacilityList;
            var Images = _basedb.ImageStore.Where(o => o.ReferIdZH == model.ID && o.Type == "Room").Select(p =>
                new ImageViewModel
                {
                    Image = p.Image,
                    Extension = p.Extension,
                    Name = p.Name,
                    Type = p.Type
                }).ToList();

            
            ViewBag.ImgKey = key;
            Session[key] = Images;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(RoomViewModel model)
        {
            var IsAdminUser = (User.IsInRole("Admin") || User.IsInRole("System")) ? true : false;
            var RoomIsCanEdit = IsAdminUser ? true : IsCanEdit;
            if (!RoomIsCanEdit)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
               // model.HOTELID = 11;
                model.Edit();
                return RedirectToAction("Edit", new {id=model.ID });
            }
            return View();
        }

        public ActionResult Price(int id)
        {
            var IsAdminUser = (User.IsInRole("Admin") || User.IsInRole("System")) ? true : false;
            var RoomIsCanEdit = IsAdminUser ? true : IsCanEdit;
            ViewBag.IsCanEdit = IsAdminUser ? true : IsCanEdit;
            ViewBag.RoomId = id;
            ViewBag.Name = _dbzh.Room.Find(id).Name;
            var PRDate = new PRDate();
            ViewBag.Begin = PRDate.Begin;
            ViewBag.End = PRDate.End;
            return View();
        }
    }
}