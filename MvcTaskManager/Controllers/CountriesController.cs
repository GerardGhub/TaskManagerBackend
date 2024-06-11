using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTaskManager.Identity;
using MvcTaskManager.Models;

namespace MvcTaskManager.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CountriesController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("api/countries")]
        public async Task<IActionResult> GetCountries()
        {
            List<Country> countries = await this.db.Countries.OrderBy(temp => temp.CountryName).ToListAsync();
            return Ok(countries);
        }

        [HttpGet]
        [Route("api/countries/searchbycountryid/{CountryID}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetByCountryID(int CountryID)
        {
            Country country = await db.Countries.Where(temp => temp.CountryID == CountryID).FirstOrDefaultAsync();
            if (country != null)
            {
                return Ok(country);
            }
            else
                return NoContent();
        }

        [HttpPost]
        [Route("api/countries")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Country> Post([FromBody] Country country)
        {
            db.Countries.Add(country);
           await db.SaveChangesAsync();

            Country existingCountry =  await db.Countries.Where(temp => temp.CountryID == country.CountryID).FirstOrDefaultAsync();
            return existingCountry;
        }

        [HttpPut]
        [Route("api/countries")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Country> Put([FromBody] Country country)
        {
            Country existingCountry = await db.Countries.Where(temp => temp.CountryID == country.CountryID).FirstOrDefaultAsync();
            if (existingCountry != null)
            {
                existingCountry.CountryName = country.CountryName;
               await db.SaveChangesAsync();
                return existingCountry;
            }
            else
            {
                return null;
            }
        }

        [HttpDelete]
        [Route("api/countries")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<int> Delete(int CountryID)
        {
            Country existingCountry = await db.Countries.Where(temp => temp.CountryID == CountryID).FirstOrDefaultAsync();
            if (existingCountry != null)
            {
                db.Countries.Remove(existingCountry);
               await db.SaveChangesAsync();
                return CountryID;
            }
            else
            {
                return -1;
            }
        }
    }
}


