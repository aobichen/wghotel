﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WGHotel.Areas.Backend.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using WGHotel.Models;
    public partial class ReportViewModel
    {
        public int ID { get; set; }
        public string Country { get; set; }
        public int CountryID { get; set; }
        public int HotelID { get; set; }
        public string Room { get; set; }
       // public int RoomID { get; set; }
        public int Creator { get; set; }
        public System.DateTime Modified { get; set; }

        public int Modify { get; set; }
        public System.DateTime Created { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString="{0:yyyy/MM/dd}", ApplyFormatInEditMode=true)]
        public System.DateTime CheckInDate { get; set; }

        
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> NumOfPeople { get; set; }
        public string Remark { get; set; }

        public int RoomID { get; set; }

        public Nullable<decimal> FoodCost { get; set; }
        public string Other { get; set; }
        public Nullable<decimal> OtherCost { get; set; }
        public string Food { get; set; }
        public string UserType { get; set; }

        public List<ReportRooms> RoomOfReport { get; set; }

        public void Create(){
            var db = new WGHotelBaseEntities();
            var Model = new Report();
            Model.Created = Created;
            Model.Creator = Creator;
            //Model.CountryID = CountryID;
            var country_id = int.Parse(Country);
            var country = db.Country.Find(country_id);
            Model.CountryID = country.ID;
            Model.Country = country.Name;
            Model.CheckInDate = CheckInDate;
            Model.HotelID = HotelID;
            Model.NumOfPeople = NumOfPeople;
            Model.Price = Price;
            Model.RoomID = RoomID;
            Model.Modified = Modified;
            Model.Modify = Modify;
            Model.Room = Room;
            Model.Food = Food;
            Model.FoodCost = FoodCost;
            Model.Other = Other;
            Model.OtherCost = OtherCost;
            Model.UserType = UserType;

            var ReportRooms = new List<ReportRooms>();

            var RoomString = Room;
            var RoomSpilt = RoomString.Split(',');
            foreach (var ro in RoomSpilt)
            {
                var room = ro.Split('^');
                var id = int.Parse(room[0]);
                var amt = int.Parse(room[1]);
                var name = room[2];
                ReportRooms.Add(new ReportRooms { RoomID = id, Amount = amt, RoomName = name });
            }

            Model.ReportRooms = ReportRooms;

            db.Report.Add(Model);
            db.SaveChanges();
        }


        

        public void Edit()
        {
            var db = new WGHotelBaseEntities();
            var Model = db.Report.Find(ID);
            Model.Modified = Modified;
            Model.Modify = Modify;
            //var Model = new Report();
            //Model.Created = Created;
            //Model.Creator = Creator;
            var country_id = int.Parse(Country);
            var country = db.Country.Find(country_id);
            Model.CountryID = country.ID;
            Model.Country = country.Name;
            Model.CheckInDate = CheckInDate;
            Model.HotelID = HotelID;
            Model.NumOfPeople = NumOfPeople;
            Model.Price = Price;
            Model.RoomID = RoomID;
            Model.Room = Room;
            Model.Food = Food;
            Model.FoodCost = FoodCost;
            Model.Other = Other;
            Model.OtherCost = OtherCost;
            Model.UserType = UserType;
            var ReportRooms = new List<ReportRooms>();
            var ReportRoomsOf = new List<ReportRooms>();
            if (!string.IsNullOrEmpty(Room))
            {
                var RoomString = Room;
                var RoomSpilt = RoomString.Split(',');
                foreach (var ro in RoomSpilt)
                {
                    var room = ro.Split('^');
                    var id = int.Parse(room[0]);
                    var amt = int.Parse(room[1]);
                    var name = room[2];
                    ReportRoomsOf.Add(new ReportRooms { ID =ID, RoomID = id, Amount = amt, RoomName = name });
                    if (!db.ReportRooms.Any(o => o.ReportID == ID && o.RoomID == id && o.Amount == amt))
                    {
                        ReportRooms.Add(new ReportRooms { RoomID = id, Amount = amt, RoomName = name });
                    }
                }

                var rrIDs = ReportRoomsOf.Select(o=>o.RoomID).ToList();
                var remove = db.ReportRooms.Where(o => !rrIDs.Contains(o.RoomID)).ToList();
                db.ReportRooms.RemoveRange(remove);
                Model.ReportRooms = ReportRooms;
            }
            //db.Report.Add(Model);
            db.SaveChanges();
        }
    }
}