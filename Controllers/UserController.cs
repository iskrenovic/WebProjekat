using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

using Models;

namespace WebProjekat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private HotelManagingContext Context { get; set;}
        public UserController(HotelManagingContext context)
        {
            Context = context;
        }

        [Route("AddOwner")]
        [HttpPost]
        public async Task<ActionResult> AddNewOwner([FromBody] User user)
        {
            if(DataHandler.StringOutOfRange(user.Username,4,16)){
                return BadRequest("Bad username format!");
            }
            if(DataHandler.StringOutOfRange(user.Password, 4,16)){
                return BadRequest("Bad password format!");
            }

            if(Context.Users.Count()>0){
                User us = Context.Users.Where(p => p.Username == user.Username).FirstOrDefault();
                if(us!=null){
                    return BadRequest("This username is taken!");
                }
            }

            user.UserType = UserType.OWNER;

            try
            {
                Context.Users.Add(user);
                await Context.SaveChangesAsync();
                return Ok(new{
                        ID = user.ID,
                        Ime = user.Ime,
                        Prezime = user.Prezime,
                        Hotels = user.WorksAt,
                        Privilege = (int) user.UserType
                    });
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }           
        }
        

        [Route("AddUser/{hotelName}/{userType}")]
        [HttpPost]
        public async Task<ActionResult> AddNewUser(string hotelName, int userType, [FromBody] User user)
        {            
            if(DataHandler.StringOutOfRange(user.Username,4,16)){
                return BadRequest(DataHandler.StringToJson("Bad username format!"));
            }
            if(DataHandler.StringOutOfRange(user.Password, 4,16)){
                return BadRequest(DataHandler.StringToJson("Bad password format!"));
            }

            if(Context.Users.Count()>0){
                User us = Context.Users.Where(p => p.Username == user.Username).FirstOrDefault();
                if(us!=null){
                    return BadRequest(DataHandler.StringToJson("This username is taken!"));
                }
            }
            user.UserType = (UserType) userType;

            Hotel hotel = Context.Hotels.Where(p=>p.Name == hotelName).FirstOrDefault();
            if(hotel == null){
               return BadRequest(DataHandler.StringToJson("This hotel doesn't exist"));
            }

            try
            {
                Context.Users.Add(user);
                await Context.SaveChangesAsync();

                Employed e = new Employed
                {
                    Hotel = hotel,
                    User = user
                };
                Context.Employes.Add(e);
                await Context.SaveChangesAsync();
                return Ok(DataHandler.StringToJson("User added"));
            }
            catch(Exception e)
            {
                return BadRequest(DataHandler.StringToJson(e.Message));
            }          
        }

        [Route("LoginUser/{username}/{password}")]
        [HttpGet]
        public ActionResult GetUserPrivilages(string username, string password) 
        {
            Console.WriteLine("GET CALLED");
            if(DataHandler.StringOutOfRange(username,4,16)){
                return BadRequest(JsonSerializer.Serialize("Bad username format!"));
            }
            if(DataHandler.StringOutOfRange(password,4,16)){
                return BadRequest(JsonSerializer.Serialize("Bad password format!"));
            }
            if(Context.Users.Count()>0){
                User user = Context.Users.Where(p => p.Username == username && p.Password == password).FirstOrDefault();
                if(user!=null){                    
                    return Ok(new{
                        ID = user.ID,
                        Ime = user.Ime,
                        Prezime = user.Prezime,
                        Hotels = user.WorksAt,
                        Privilege = (int) user.UserType
                    });
                }
                return BadRequest(JsonSerializer.Serialize("User not found!"));
            }
            return BadRequest(JsonSerializer.Serialize("No users on the server."));                        
        }   

        [Route("RemoveUser/{username}")]
        [HttpDelete]
        public async Task<ActionResult> RemoveUser(string username)
        {
            if(DataHandler.StringOutOfRange(username,4,16)){
                return BadRequest("Bad username format!");
            }            

            if(Context.Users.Count()>0){
                User us = Context.Users.Where(p => username == p.Username).FirstOrDefault();
                if(us==null){
                    return BadRequest("There is no such user.");
                }
                if(us.UserType == UserType.OWNER){
                    return BadRequest("Cannot delete an OWNER");
                }

                try
                {
                    int idUs = us.ID;
                    foreach(Employed emp in us.WorksAt){
                        Context.Employes.Remove(emp);
                    }
                    Context.Users.Remove(us);
                    await Context.SaveChangesAsync();
                    return Ok($"User removed. ID={idUs}");
                }
                catch(Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            return BadRequest("No users to remove!");            
        }

    }
}