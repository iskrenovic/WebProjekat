using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;

namespace WebProjekat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private HotelManagingContext Context { get; set;}
        public BookingController(HotelManagingContext context)
        {
            Context = context;
        } 

        [Route("BookARoom/{arrivalDate}/{departureDate}/{hotelName}/{roomNumber}/{documentType}/{documentIdNumber}")]
        [HttpPost]        
        public async Task<ActionResult> BookARoom(string arrivalDate, string departureDate, string hotelName, int roomNumber, int documentType, string documentIdNumber)
        {
            
            DateTime aDate, dDate;
            try
            {
                aDate = DataHandler.StringToDate(arrivalDate);
                dDate = DataHandler.StringToDate(departureDate);
            }
            catch{
                return BadRequest(DataHandler.StringToJson("Bad date format"));
            }
            
            if(aDate>dDate){
                DateTime temp = aDate;
                aDate = dDate;
                dDate = temp;
            }
            else if(aDate == dDate){
                return BadRequest(DataHandler.StringToJson("Invalid departure date."));
            }

            if(aDate < DateTime.Now){
                return BadRequest(DataHandler.StringToJson("No bookings can be made for pased dates."));
            }

            if(dDate < DateTime.Now){
                return BadRequest(DataHandler.StringToJson("No bookings can be made for pased dates."));
            }            

            if(roomNumber<0){
                return BadRequest(DataHandler.StringToJson("A room number cannot be less than 0"));
            }

             if(DataHandler.ValueOutOfRange(documentType,0,1))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for document type"));             
            } 

            if(DataHandler.StringOutOfRange(documentIdNumber,5,13))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for document number"));          
            }            

            Client c = await Context.Clients.Where(p=> (int)p.DocumentType == documentType && p.DocumentIdNumber == documentIdNumber).FirstOrDefaultAsync();
            if(c==null)
            {
                return BadRequest(DataHandler.StringToJson("This client cannot be found."));
            }

            Room room = await Context.Rooms.Where(p=>p.Hotel.Name == hotelName && p.RoomNumber == roomNumber).FirstOrDefaultAsync();
            if(room == null){
                return BadRequest(DataHandler.StringToJson("This room cannot be found."));
            }

            RoomBooking reservation = room.RoomBookings.Where(p=>DataHandler.BetweenDates(p.ArrivalDate,p.DepartureDate,aDate,dDate) && p.Room.Hotel.Name == hotelName).FirstOrDefault();
            if(reservation!=null)
            {
                return BadRequest(DataHandler.StringToJson("This room is already taken. Change the date or change the room."));
            }

            try
            {
                RoomBooking roomBooking = new RoomBooking
                {
                    ArrivalDate = aDate,
                    DepartureDate = dDate,
                    Client = c,
                    Room = room 
                };
                Context.RoomBookings.Add(roomBooking);
                await Context.SaveChangesAsync();
               return Ok(DataHandler.StringToJson($"New reservation was made. Reservation id: {roomBooking.ID}"));
            }
            catch(Exception e)
            {
                return BadRequest(DataHandler.StringToJson(e.Message));
            }                           
        }

        [Route("GetMonth/{hotelName}")]
        [HttpGet]        
        public async Task<ActionResult> GetReservationsForRoom(string hotelName)
        {
            DateTime date = DateTime.Now;

            Hotel hotel = await Context.Hotels.Where(p=>p.Name == hotelName).FirstOrDefaultAsync();
            if(hotel==null)
                return BadRequest(DataHandler.StringToJson("No hotel was found"));
            

            var result = await Context.RoomBookings.Where(p=>p.Room.Hotel.ID == hotel.ID && p.ArrivalDate.Day >= date.Day && p.ArrivalDate.Month == date.Month).Select(p=> 
                new
                {
                ArrivalDate = p.ArrivalDate.Day,
                DepartureDate = p.DepartureDate.Day,
                Room = p.Room.RoomNumber,                
                Name = p.Client.Ime,
                Surname = p.Client.Prezime
                }).ToListAsync();
            foreach(Object obj in result){
                Console.WriteLine(obj);
            }        
            
            return Ok(JsonSerializer.Serialize(result));
        }      


        [Route("CancelAReservation/{arrivalDate}/{hotelName}/{roomNumber}")]
        [HttpDelete]        
        public async Task<ActionResult> CancelReservation(string arrivalDate, string hotelName, int roomNumber)
        {
            DateTime aDate;
            try
            {
                aDate = DataHandler.StringToDate(arrivalDate);
            }
            catch{
                return BadRequest(DataHandler.StringToJson("Bad date format"));
            }            
            Console.WriteLine(aDate);

            if(aDate < DateTime.Now){
                return BadRequest(DataHandler.StringToJson("You can't cancel for pased dates."));
            }
            
            if(roomNumber<0){
                return BadRequest(DataHandler.StringToJson("A room number cannot be less than 0"));
            }     

            Room room = await Context.Rooms.Where(p=>p.Hotel.Name == hotelName && p.RoomNumber == roomNumber).FirstOrDefaultAsync();
            if(room == null){
                return BadRequest(DataHandler.StringToJson("This room cannot be found."));
            }

            RoomBooking reservation = room.RoomBookings.Where(p=>p.ArrivalDate == aDate).FirstOrDefault();
            if(reservation==null)
            {
                return BadRequest(DataHandler.StringToJson("Reservation not found."));
            }

            try
            {
                int reservationId = reservation.ID;
                Context.RoomBookings.Remove(reservation);
                await Context.SaveChangesAsync();  
               return Ok(DataHandler.StringToJson($"Reservation cancled. Reservation id: {reservationId}"));
            }
            catch(Exception e)
            {
                return BadRequest(DataHandler.StringToJson(e.Message));
            }                           
        }       
    }

}