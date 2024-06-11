using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcTaskManager.Identity;
using MvcTaskManager.Models;

namespace MvcTaskManager.Controllers
{
    public class ClientLocationsController : Controller
    {
        private ApplicationDbContext db;
        private object message;

        public ClientLocationsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("api/clientlocations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Get()
        {
            List<ClientLocation> clientLocations = db.ClientLocations.ToList();
            return Ok(clientLocations);
        }

        [HttpGet]
        [Route("api/clientlocations/searchbyclientlocationid/{ClientLocationID}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetByClientLocationID(int ClientLocationID)
        {
            ClientLocation clientLocation = db.ClientLocations.Where(temp => temp.ClientLocationID == ClientLocationID).FirstOrDefault();
            if (clientLocation != null)
            {
                return Ok(clientLocation);
            }
            else
                return NoContent();
        }

        [HttpPost]
        [Route("api/clientlocations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Post([FromBody] ClientLocation clientLocation)
        {

            // Check if a location with the same name already exists    
            var checkClientLocationIfExist = db.ClientLocations.Where(temp => temp.ClientLocationName == clientLocation.ClientLocationName).FirstOrDefault();
            if (checkClientLocationIfExist != null)
            {
                return BadRequest(new { message = "Client Location name already exists." });

            }
         

                db.ClientLocations.Add(clientLocation);
                db.SaveChanges();


            // Retrieve the newly added record based on ID

            ClientLocation existingClientLocation = db.ClientLocations.Where(temp => temp.ClientLocationID == clientLocation.ClientLocationID).FirstOrDefault();
            return Ok(existingClientLocation);
        }

        [HttpPut]
        [Route("api/clientlocations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ClientLocation Put([FromBody] ClientLocation project)
        {
            ClientLocation existingClientLocation = db.ClientLocations.Where(temp => temp.ClientLocationID == project.ClientLocationID).FirstOrDefault();
            if (existingClientLocation != null)
            {
                existingClientLocation.ClientLocationName = project.ClientLocationName;
                db.SaveChanges();
                return existingClientLocation;
            }
            else
            {
                return null;
            }
        }

        [HttpDelete]
        [Route("api/clientlocations")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public int Delete(int ClientLocationID)
        {
            ClientLocation existingClientLocation = db.ClientLocations.Where(temp => temp.ClientLocationID == ClientLocationID).FirstOrDefault();

            // Check if the client location exists
            if (existingClientLocation != null)
            {
                db.ClientLocations.Remove(existingClientLocation);
                db.SaveChanges();
                return ClientLocationID;
            }
            else
            {
                return -1;
            }
        }



    }
}


