using System;
using Microsoft.AspNetCore.Mvc;
using Clockwork.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Clockwork.API.Controllers
{
    [Route("api/[controller]")]
    public class CurrentTimeController : Controller
    {
        // GET api/currenttime
        [HttpGet]
        public IActionResult Get([FromQuery]string timezone)
        {
            var utcTime = DateTime.UtcNow;
            var serverTime = DateTime.Now;
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();

            if (timezone == "Central")
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                serverTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, cstZone);
                timezone = "Central Standard Time";
            }
            if (timezone == "Eastern")
            {
                TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                serverTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, estZone);
                timezone = "Eastern Standard Time";
            }
            if (timezone == "Pacific")
            {
                TimeZoneInfo pfcZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                serverTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, pfcZone);
                timezone = "Pacific Standard Time";
            }
            if (timezone == "Mountain")
            {
                TimeZoneInfo mtnZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
                serverTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, mtnZone);
                timezone = "Mountain Standard Time";
            }

            var returnVal = new CurrentTimeQuery

            {
                UTCTime = utcTime,
                ClientIp = ip,
                Time = serverTime,
                Timezone = timezone
            };

            using (var db = new ClockworkContext())
            {
                db.CurrentTimeQueries.Add(returnVal);
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                foreach (var CurrentTimeQuery in db.CurrentTimeQueries)
                {
                    Console.WriteLine(" - {0}", CurrentTimeQuery.UTCTime);
                }
            }

            return Ok(returnVal);
        }

        [HttpGet("entries")]
        public IActionResult GetTimeEntries()
        {
            var results = new List<CurrentTimeQuery>();

            using (var db = new ClockworkContext())
            {
                var items = db.CurrentTimeQueries.AsQueryable().ToList();
                results.AddRange(items);
                Console.WriteLine("{0} items retrieved from the database", results.Count);
            
            }

            return Ok(results);
        }
    }
}
