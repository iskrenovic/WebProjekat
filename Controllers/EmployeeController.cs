using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;

namespace WebProjekat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private HotelManagingContext Context { get; set;}
        public EmployeeController(HotelManagingContext context)
        {
            Context = context;
        }


        [Route("AssignEmployee/{username}/{name}")]
        [HttpPost]
        public async Task<ActionResult> AddNewEmployee(string username,string name)
        {           
           if(DataHandler.StringOutOfRange(username,4,16)){
                return BadRequest(DataHandler.StringToJson("Bad username format!"));
            }

            if(DataHandler.StringOutOfRange(name,0,50)){
                return BadRequest(DataHandler.StringToJson("Bad name format!"));
            }          


            if(Context.Hotels.Count()>0){
                Hotel hot = await Context.Hotels.Where(p => p.Name == name).FirstOrDefaultAsync();
                if(hot==null){
                    return BadRequest(DataHandler.StringToJson("There is no such hotel."));
                }

                User user = await Context.Users.Where(p=>p.Username == username).FirstOrDefaultAsync();
                if(user==null)
                    return BadRequest(DataHandler.StringToJson("User not found!"));
                
                try
                {                    
                    Employed e = new Employed
                    {
                        Hotel = hot,
                        User = user
                    };
                    Context.Employes.Add(e);
                    await Context.SaveChangesAsync();
                    return Ok(DataHandler.StringToJson($"User {user.Ime} {user.Prezime} was employed to the hotel {hot.Name} in {hot.City} as {user.UserType.ToString()} on id: {e.ID}"));
                }
                catch(Exception e)
                {
                    return BadRequest(DataHandler.StringToJson(e.Message + hot.Employees));
                }
            }
            return BadRequest(DataHandler.StringToJson("No hotel added!"));
        }

        [Route("GetWorkplace/{userId}")]
        [HttpGet]
        public async Task<ActionResult> GetWorkplace(int userId)
        {

            var ret = await Context.Employes.Where(p=>p.User.ID == userId).Select(p => new
            {
                id = p.Hotel.ID,
                name = p.Hotel.Name
            }).ToListAsync();

            if(ret == null)
                return BadRequest(DataHandler.StringToJson("This user isn't employed anywhere yet."));

            return Ok(ret);           
        }


        [Route("GetAllEmployed")]
        [HttpGet]
        public ActionResult GetAllEmployed(){
            return Ok(Context.Employes.Select(p=> new
            {
                HotelName = p.Hotel.Name,
                Username = p.User.Username,
                Role = p.User.UserType.ToString()
            }).ToList());
        }


        [Route("FireEmployed")]
        [HttpDelete]
        public async Task<ActionResult> FireEmployed(string userName, string hotelName){            

            Employed employed = await Context.Employes.Where(p=>p.User.Username == userName && p.Hotel.Name == hotelName).FirstOrDefaultAsync();
            if(employed == null){
                return BadRequest(DataHandler.StringToJson("This user was never employed."));
            }

            try{
                Context.Employes.Remove(employed);
                await Context.SaveChangesAsync();
                return Ok();
            }
            catch(Exception e){
                return BadRequest(DataHandler.StringToJson(e.Message));
            }            
        }


    }
}