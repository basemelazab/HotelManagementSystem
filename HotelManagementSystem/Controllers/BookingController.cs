using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class BookingController : Controller
    {
        private HotelDBEntities db;
        public BookingController()
        {
            db = new HotelDBEntities();
        }
        public ActionResult Index()
        {
            BookingViewModel bookingViewModel= new BookingViewModel();
            bookingViewModel.listOfRooms=(from objRoom in db.Rooms
                                          where objRoom.BookingStatusId==2
                                          select new SelectListItem()
                                          {
                                              Text =objRoom.RoomNumber,
                                              Value = objRoom.RoomId.ToString()
                                          }
                                          ).ToList();
            bookingViewModel.BookingFrom=DateTime.Now;
            bookingViewModel.BookingTo=DateTime.Now.AddDays(1);
            return View(bookingViewModel);
        }
        [HttpPost]
        public ActionResult Index(BookingViewModel bookingViewModel)
        {
           // string dt = bookingViewModel.BookingTo.ToString("dd-MM-yyyy");
            int NumberOfDayes=Convert.ToInt32((bookingViewModel.BookingTo- bookingViewModel.BookingFrom).TotalDays);
            Room room = db.Rooms.Single(model => model.RoomId == bookingViewModel.AssignRoomId);
            decimal roomPrice = room.RoomPrice;
            decimal totalPrice=roomPrice* NumberOfDayes;
            RoomBooking roomBooking = new RoomBooking()
            {
                CustomerAddress= bookingViewModel.CustomerAddress,
                CustomerName= bookingViewModel.CustomerName,
                BookingFrom=bookingViewModel.BookingFrom,
                BookingTo=bookingViewModel.BookingTo,
                AssignRoomId= bookingViewModel.AssignRoomId,
                NoOfMembers=bookingViewModel.NoOfMembers,
                TotalAmount=totalPrice,
                CustomerPhone=bookingViewModel.CustomerPhone,
            };
            db.RoomBookings.Add(roomBooking);
            db.SaveChanges();

            room.BookingStatusId = 3;
            db.SaveChanges();
            return Json(new {message="Hotal Booking is Successfully Created.",success=true},JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public PartialViewResult GetAllBookingHistory()
        {
            List<RoomBookingViewModel> listOfRoomBookingViewModel= new List<RoomBookingViewModel>();
            listOfRoomBookingViewModel = (from hotalBooking in db.RoomBookings
                                          join room in db.Rooms on hotalBooking.AssignRoomId equals room.RoomId
                                          select new RoomBookingViewModel()
                                          {
                                              BookingFrom=hotalBooking.BookingFrom,
                                              BookingTo=hotalBooking.BookingTo,
                                              CustomerPhone=hotalBooking.CustomerPhone,
                                              CustomerAddress=hotalBooking.CustomerAddress,
                                              CustomerName=hotalBooking.CustomerName,
                                              NumberOfMembers=hotalBooking.NoOfMembers,
                                              BookingId=hotalBooking.BookingId,
                                              RoomNumber=room.RoomNumber,
                                              RoomPrice=room.RoomPrice,
                                              NumberOfDays=System.Data.Entity.DbFunctions.DiffDays(hotalBooking.BookingFrom, hotalBooking.BookingTo).Value
                                          }
                                        ).ToList(); 
            return  PartialView("_BookingHistoryPartial", listOfRoomBookingViewModel);
        }
    }
}
//Convert.ToInt32(( - hotalBooking.BookingFrom).TotalDays),