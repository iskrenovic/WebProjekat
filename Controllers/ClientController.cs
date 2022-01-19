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
    public class ClientController : ControllerBase
    {
        private HotelManagingContext Context { get; set;}
        public ClientController(HotelManagingContext context)
        {
            Context = context;
        }    

        [Route("AddNewClient/{hotel}")]
        [HttpPost]        
        public async Task<ActionResult> AddClient(string hotelName, [FromBody] Client client)
        {
            Console.WriteLine(client);
            if(DataHandler.StringOutOfRange(client.Ime,3,20))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for client name"));                
            } 

            if(DataHandler.StringOutOfRange(client.Prezime,3,20))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for client surname"));                
            }

            if(DataHandler.ValueOutOfRange((int)client.DocumentType,0,1))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for document type"));                
            } 

            if(DataHandler.StringOutOfRange(client.DocumentIdNumber,5,13))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for document number"));                
            }
            

            Client c = await Context.Clients.Where(p=> p.DocumentType == client.DocumentType && p.DocumentIdNumber == client.DocumentIdNumber).FirstOrDefaultAsync();
            if(c!=null)
            {
                return Ok(DataHandler.StringToJson("This user is already in the database and ready to be used."));
            }

            try
            {
                Context.Clients.Add(client);
                await Context.SaveChangesAsync();                
                return Ok(DataHandler.StringToJson($"Client added. {client.ID}"));  
            }
            catch(Exception e)
            {
                return BadRequest(DataHandler.StringToJson(e.Message));
            }
        }


        [Route("GetClientInfo")]
        [HttpGet]        
        public async Task<ActionResult> GetClientInfo(int documentType, string documentIdNumber)
        {
            if(DataHandler.ValueOutOfRange(documentType,0,1))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for document type"));                
            } 

            if(DataHandler.StringOutOfRange(documentIdNumber,5,13))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for document number"));                
            }                 

            Client c = await Context.Clients.Where(p=> (int) p.DocumentType == documentType && p.DocumentIdNumber == documentIdNumber).FirstOrDefaultAsync();
            if(c==null)
            {
                return BadRequest(DataHandler.StringToJson("This client cannot be found."));
            }

            return Ok(c);                           
        }


        [Route("GetClientsForHotel/{hotelName}")]
        [HttpGet]        
        public async Task<ActionResult> GetClientsForHotel(string hotelName)
        {
            Hotel hotel = await Context.Hotels.Where(p=>p.Name == hotelName).FirstOrDefaultAsync();
            if(hotel==null)
                return BadRequest(DataHandler.StringToJson("No hotel was found"));
            

            var result = await Context.RoomBookings.Where(p=>p.Room.Hotel.ID == hotel.ID).Select(p=> 
                new
                {                                  
                    Name = p.Client.Ime,
                    Surname = p.Client.Prezime,
                    IdType = p.Client.DocumentType,
                    IdNumber = p.Client.DocumentIdNumber
                }).Distinct().ToListAsync();                  
            
            return Ok(result);
        }


        [Route("DeleteClient/{documentType}/{documentIdNumber}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteClient(int documentType, string documentIdNumber)
        {
            if(DataHandler.ValueOutOfRange(documentType,0,1))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for document type"));                
            } 

            if(DataHandler.StringOutOfRange(documentIdNumber,5,13))
            {
                return BadRequest(DataHandler.StringToJson("Bad format for document number"));                
            }

            Client c = await Context.Clients.Where(p=> (int) p.DocumentType == documentType && p.DocumentIdNumber == documentIdNumber).FirstOrDefaultAsync();
            if(c==null)
            {
                return BadRequest(DataHandler.StringToJson("This client cannot be found."));
            }

            try
            {
                int clientId = c.ID;
                Context.Clients.Remove(c);
                await Context.SaveChangesAsync();                
                return Ok($"Client deleted. {clientId}");  
            }
            catch(Exception e)
            {
                return BadRequest(DataHandler.StringToJson(e.Message));
            }                           
        }
    }
}