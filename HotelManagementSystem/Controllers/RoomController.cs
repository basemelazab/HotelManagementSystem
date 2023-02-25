using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class RoomController : Controller
    {
        private HotelDBEntities db;
        public RoomController()
        {
            db = new HotelDBEntities();
        }
        // GET: Room
        public ActionResult Index()
        {
            RoomViewModel roomViewModel = new RoomViewModel();
            roomViewModel.listOfBookingStatus = (from obj in db.BookingStaus
                                                 select new SelectListItem()
                                                 {
                                                     Text=obj.BookingStatus,
                                                     Value=obj.BookingStatusId.ToString(),

                                                 }).ToList();
            roomViewModel.listOfRoomTypes = (from obj in db.RoomTypes
                                             select new SelectListItem()
                                             {
                                                 Text = obj.RoomTypeName,
                                                 Value = obj.RoomTypeId.ToString(),
                                             }).ToList();

            return View(roomViewModel);
        }
        [HttpPost]
        public ActionResult Index(RoomViewModel roomViewModel)
        {
            string message = string.Empty;
            string imageUniqueName= string.Empty;
            string ActualImageName= string.Empty;
            if (roomViewModel.RoomId==0)
            {        
                 imageUniqueName = Guid.NewGuid().ToString();
                 ActualImageName = imageUniqueName + Path.GetExtension(roomViewModel.Image.FileName);
                 roomViewModel.Image.SaveAs(Server.MapPath("~/RoomImages/" + ActualImageName));
                Room room = new Room()
                {
                    RoomNumber = roomViewModel.RoomNumber,
                    RoomImage = ActualImageName,
                    RoomCapacity = roomViewModel.RoomCapacity,
                    RoomDescription = roomViewModel.RoomDiscription,
                    RoomPrice = roomViewModel.RoomPrice,
                    BookingStatusId = roomViewModel.BookingStatusId,
                    RoomTypeId = roomViewModel.RoomTypeId,
                    IsActive = true,
                };
                db.Rooms.Add(room);
                message="Added";
            }
            else
            {
                Room room = db.Rooms.Single(model => model.RoomId == roomViewModel.RoomId);
                if (roomViewModel.Image !=null)
                {
                    imageUniqueName = Guid.NewGuid().ToString();
                    ActualImageName = imageUniqueName + Path.GetExtension(roomViewModel.Image.FileName);
                    roomViewModel.Image.SaveAs(Server.MapPath("~/RoomImages/" + ActualImageName));
                    room.RoomImage = ActualImageName;
                }
                room.RoomNumber = roomViewModel.RoomNumber;             
                room.RoomCapacity = roomViewModel.RoomCapacity;
                room.RoomDescription = roomViewModel.RoomDiscription;
                room.RoomPrice = roomViewModel.RoomPrice;
                room.BookingStatusId = roomViewModel.BookingStatusId;
                room.RoomTypeId = roomViewModel.RoomTypeId;
                room.IsActive = true;
                message = "Updated";
            }
            db.SaveChanges();
            return Json( new {message ="Dada Successfully "+ message,success=true },JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public PartialViewResult GetAllRooms()
        {
            IEnumerable<RoomDetailsViewModel> listOfRoomDetailsViewModel =
                (from room in db.Rooms
                 join bookingStatus in db.BookingStaus on room.BookingStatusId equals bookingStatus.BookingStatusId
                 join roomType in db.RoomTypes on room.RoomTypeId equals roomType.RoomTypeId
                 //where room.IsActive == true
                 select new RoomDetailsViewModel()
                 {
                     RoomCapacity = room.RoomCapacity,
                     RoomNumber= room.RoomNumber,
                     RoomPrice = room.RoomPrice,
                     RoomDescription = room.RoomDescription,
                     BookingStatus =bookingStatus.BookingStatus,
                     RoomType=roomType.RoomTypeName,
                     RoomImage=room.RoomImage,
                     RoomId=room.RoomId,
                 }
                 ).ToList();

            return PartialView("_RoomDetailsPartial",listOfRoomDetailsViewModel);
        }
        [HttpGet]
        public JsonResult EditRoomDetails( int roomId)
        {
            var result=db.Rooms.Single(model=>model.RoomId==roomId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeleteRoomDetails(int roomId)
        {
           Room room= db.Rooms.Single(model => model.RoomId == roomId);
            room.IsActive = false;
            db.SaveChanges();
            return Json(new {message="Room Successfully Deleted",success=true}, JsonRequestBehavior.AllowGet);
        }
    }
}