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
    public class RoomController : ControllerBase
    {
        private HotelManagingContext Context { get; set;}
        public RoomController(HotelManagingContext context)
        {
            Context = context;
        }

        [Route("AddNewRoom/{hotelName}")]
        [HttpPost]
        public async Task<ActionResult> AddRoom(string hotelName, [FromBody] Room room)
        {
            if(DataHandler.ValueOutOfRange(room.Capacity,1,20)){
                return BadRequest("Bad format for room capacity");
            }

            Hotel hotel = await Context.Hotels.Where(p=>p.Name == hotelName).FirstOrDefaultAsync();
            if(hotel == null){
                return BadRequest("There is no hotel with that name.");
            }
            Room possibleRoom = hotel.Rooms.Where(p=>p.RoomNumber == room.RoomNumber).FirstOrDefault();
            if(possibleRoom!=null){
                return BadRequest(new {message = "A room with this room number already exists."});                
            }

            try
            {
                room.Hotel = hotel;
                Context.Rooms.Add(room);
                await Context.SaveChangesAsync();
                return Ok($"Room was added. {room.ID}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }            
        }

        [Route("GetRooms/{hotelName}")]
        [HttpGet]
        public async Task<ActionResult> GetRooms(string hotelName)
        {            
            Hotel hotel = await Context.Hotels.Where(p=>p.Name == hotelName).FirstOrDefaultAsync();
            if(hotel == null){
                return BadRequest("There is no hotel with that name.");
            }           

            var ret = hotel.Rooms.Where(p=>p.Hotel.ID == hotel.ID).Select(p=>new
            {
                id = p.ID,
                capacity = p.Capacity,
                number = p.RoomNumber
            }).OrderBy(p=>p.number).ToList();
            if(ret == null){
                return BadRequest("There are no rooms in this hotel!");
            }
            return Ok(ret);                        
        }

        [Route("RemoveRoom/{hotelName}/{roomNumber}")]
        [HttpDelete]
        public async Task<ActionResult> RemoveRoom(string hotelName, int roomNumber)
        {
            if(roomNumber<0){
                return BadRequest("A room number cannot be less than 0");
            }
            Hotel hotel = await Context.Hotels.Where(p=>p.Name == hotelName).FirstOrDefaultAsync();
            if(hotel == null){
                return BadRequest("There is no hotel with that name.");
            }

            Room room = hotel.Rooms.Where(p=>p.RoomNumber == roomNumber).FirstOrDefault();
            if(room == null){
                return BadRequest("There is no room with that room number.");
            }

            try
            {
                int roomId = room.ID;
                foreach(RoomBooking booking in room.RoomBookings){
                    Context.Remove(booking);
                }
                Context.Rooms.Remove(room);
                await Context.SaveChangesAsync();
                return Ok($"Room was removed. {roomId}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }            
        }
        
    }
}