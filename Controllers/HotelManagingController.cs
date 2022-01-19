using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Models;

namespace WebProjekat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelManagingController : ControllerBase
    {
        private HotelManagingContext Context { get; set;}
        public HotelManagingController(HotelManagingContext context)
        {
            Context = context;
        }

        [Route("AddNewHotel/{userId}")]
        [HttpPost]
        public async Task<ActionResult> AddNewHotel(int userId, [FromBody] Hotel hotel)
        {
            if(DataHandler.StringOutOfRange(hotel.Name,0,50)){
                return BadRequest(DataHandler.StringToJson("Bad name format!"));
            }
            if(DataHandler.StringOutOfRange(hotel.Address, 0,50)){
                return BadRequest(DataHandler.StringToJson("Bad address format!"));
            }

            if(DataHandler.StringOutOfRange(hotel.City,0,50)){
                return BadRequest(DataHandler.StringToJson("Bad city format!"));
            }

            if(DataHandler.ValueOutOfRange(hotel.Rating,0f,10f)){
                return BadRequest(DataHandler.StringToJson("Rating out of range!"));
            }

            if(Context.Hotels.Count()>0){
                Hotel hot = Context.Hotels.Where(p => p.City == hotel.City && p.Address == hotel.Address).FirstOrDefault();
                if(hot!=null){
                    return BadRequest(DataHandler.StringToJson("This hotel was already entered!"));
                }
            }

            User user = Context.Users.Where(p=>p.ID == userId).FirstOrDefault();
            if(user == null){
                return BadRequest(DataHandler.StringToJson("This user wasn't found"));
            }

            try
            {
                Context.Hotels.Add(hotel);
                await Context.SaveChangesAsync();

                Employed e = new Employed
                {
                    Hotel = hotel,
                    User = user
                };
                Context.Employes.Add(e);
                await Context.SaveChangesAsync();                
                return Ok(DataHandler.StringToJson($"New hotel added."));
            }
            catch(Exception e)
            {
                return BadRequest(DataHandler.StringToJson(e.Message));
            }           
        }

        [Route("RemoveHotel/{hotelName}")]
        [HttpDelete]
        public async Task<ActionResult> RemoveHotel(string hotelName)
        {
            Console.WriteLine(hotelName);
            if(DataHandler.StringOutOfRange(hotelName,0,50)){
                return BadRequest(DataHandler.StringToJson("Bad name format!"));
            }            

            if(Context.Hotels.Count()>0){
                Hotel hot = Context.Hotels.Where(p=>p.Name == hotelName).FirstOrDefault(); 
                
                if(hot==null){
                    return BadRequest(DataHandler.StringToJson("There is no such hotel."));
                }
                try
                {
                    int idHot = hot.ID;                    
                    if(hot.Rooms.Count>0){
                        foreach(Room room in hot.Rooms){
                            Context.Rooms.Remove(room);
                        }
                    }
                    foreach(Employed employed in Context.Employes.Where(p=>p.Hotel.ID == hot.ID).ToList()){
                        Context.Employes.Remove(employed);
                    }
                    Context.Hotels.Remove(Context.Hotels.Where(p=>p.ID == hot.ID).FirstOrDefault());
                    await Context.SaveChangesAsync();
                    return Ok();
                }
                catch(Exception e)
                {
                    return BadRequest(DataHandler.StringToJson(e.Message));
                }
            }
            return BadRequest(DataHandler.StringToJson("No hotel to remove!"));            
        }      


        [Route("GetHotel")]
        [HttpGet]
        public ActionResult GetHotels()
        {
           return Ok(Context.Hotels);
        }


        [Route("GetReferencesOfHotel")]
        [HttpGet]
        public ActionResult GetRef(string hotelName)
        {
            Hotel hot = Context.Hotels.Where(p=>p.Name == hotelName).FirstOrDefault(); 
            var ret = Context.Employes.Where(p=>p.Hotel.ID == hot.ID).ToList();
           return Ok(ret);
        }
    }
}
