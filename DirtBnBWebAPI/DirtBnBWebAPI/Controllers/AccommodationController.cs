using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;

namespace DirtBnBWebAPI.Controllers
{
    public class AccommodationController : ApiController
    {
        [Route("api/accommodations/retrieve")]
        [HttpPost]
        public HttpResponseMessage GetAccommodationsWithColumns([FromBody]SelectSQLString sqlString)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            if (sqlString != null)
            {
                var accommodations = accommodationPersistenceService.GetAccommodationsSelectiveColumns(sqlString.includeAmenities, sqlString.includeBedSize, sqlString.includePricePerNight);
                HttpResponseMessage response;
                if (accommodations == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, "No accommodations found for any user.");
                    return response;
                }
                response = Request.CreateResponse(HttpStatusCode.OK, accommodations);
                return response;
            }
            var selectedAccommodations = accommodationPersistenceService.GetAccommodations();
            HttpResponseMessage selectedResponse;
            if (selectedAccommodations == null)
            {
                selectedResponse = Request.CreateResponse(HttpStatusCode.NotFound, "No accommodations found for any user.");
                return selectedResponse;
            }
            selectedResponse = Request.CreateResponse(HttpStatusCode.OK, selectedAccommodations);
            return selectedResponse;
        }


        [Route("api/accommodations/retrieve/{id}")]
        [HttpPost]
        public HttpResponseMessage GetAccommodationWith(long id, [FromBody] SelectSQLString sqlString)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            if (sqlString != null)
            {
                var accommodations = accommodationPersistenceService.GetAccommodationSelectiveColumns(sqlString.includeAmenities, sqlString.includeBedSize, sqlString.includePricePerNight, id);
                HttpResponseMessage response;
                if (accommodations == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, "No accommodations found with id: " + id);
                    return response;
                }
                response = Request.CreateResponse(HttpStatusCode.OK, accommodations);
                return response;
            }
            var selectedAccommodations = accommodationPersistenceService.GetAccommodation(id);
            HttpResponseMessage selectedResponse;
            if (selectedAccommodations == null)
            {
                selectedResponse = Request.CreateResponse(HttpStatusCode.NotFound, "No accommodations found with id: " + id);
                return selectedResponse;
            }
            selectedResponse = Request.CreateResponse(HttpStatusCode.OK, selectedAccommodations);
            return selectedResponse;
        }

        [Route("api/accommodations/host/{id}")]
        [HttpPost]
        public HttpResponseMessage GetAccommodationPerHost(long id, [FromBody] SelectSQLString sqlString)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            if (sqlString != null)
            {
                var accommodations = accommodationPersistenceService.GetAccommodationsSelectiveColumnsHost(sqlString.includeAmenities, sqlString.includeBedSize, sqlString.includePricePerNight, id);
                HttpResponseMessage response;
                if (accommodations == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, "No accommodations found for cities from that host with id: " + id);
                    return response;
                }
                response = Request.CreateResponse(HttpStatusCode.OK, accommodations);
                return response;
            }
            var selectedAccommodations = accommodationPersistenceService.GetAccommodationsHost(id);
            HttpResponseMessage selectedResponse;
            if (selectedAccommodations == null)
            {
                selectedResponse = Request.CreateResponse(HttpStatusCode.NotFound, "No accommodations found for cities from that host with id: " + id);
                return selectedResponse;
            }
            selectedResponse = Request.CreateResponse(HttpStatusCode.OK, selectedAccommodations);
            return selectedResponse;
        }

        [Route("api/accommodations/cityaverages")]
        [HttpGet]
        public HttpResponseMessage GetAccommodationAverages()
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            var accommodations = accommodationPersistenceService.GetAccommodationsAveragePriceOfAllCities();
            HttpResponseMessage response;
            if (accommodations == null || accommodations.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No accommodation averages found for cities.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, accommodations);
            return response;
        }

        [Route("api/accommodations/cityaverage")]
        [HttpPost]
        public HttpResponseMessage GetAccommodationAveragesOfCity([FromBody]Accommodation accommodation)
        {
            HttpResponseMessage response;
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            if (accommodation == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Request body is empty to get city average.");
                return response;
            }
            var accommodationsAvg = accommodationPersistenceService.GetAccommodationsAveragePriceOfCity(accommodation.city, accommodation.province);

            if (accommodationsAvg == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No accommodation average found from city: " + accommodation.city);
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, accommodationsAvg);
            return response;
        }

        [Route("api/accommodations/")]
        [HttpPost]
        public HttpResponseMessage CreateAccommodation([FromBody]Accommodation accommodation)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            HttpResponseMessage response;

            if (accommodation.parking == null
                || accommodation.wifi == null
                || accommodation.tv == null
                || accommodation.airConditioning == null
                || accommodation.generalAppliances == null
                || string.IsNullOrEmpty(accommodation.bedSize)
                || accommodation.pricePerNight.Equals(0)
                || string.IsNullOrEmpty(accommodation.houseNumber)
                || accommodation.hostUserID == null
                || string.IsNullOrEmpty(accommodation.postalCode)
                || string.IsNullOrEmpty(accommodation.city)
                || string.IsNullOrEmpty(accommodation.street)
                || string.IsNullOrEmpty(accommodation.province))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "All fields are mandatory. Please try again.");
                return response;
            }

            var id = accommodationPersistenceService.SaveAccommodation(accommodation);
            if (id < 0)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Unable to create accommodation, please check if host exists.");
                return response;
            }
            accommodation.accommodationID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, accommodation);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("accommodations/{0}", id));
            return response;
        }

        [Route("api/accommodations/{id}")]
        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage UpdateAccommodation(long id, [FromBody]Accommodation accommodation)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            bool accommodationExists = false;
            HttpResponseMessage response;

            accommodationExists = accommodationPersistenceService.UpdateAccommodation(id, accommodation);

            if (accommodationExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, accommodation);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Accommodation not found.");
                return response;
            }
        }

        [Route("api/accommodations/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteHost(long id)
        {
            AccommodationPersistenceService accommodationPersistenceService = new AccommodationPersistenceService();
            bool userExists = false;
            userExists = accommodationPersistenceService.DeleteAccommodation(id);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, "Accommodation deleted with id: " + id);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Accommodation not found with id :" + id);
                return response;
            }
        }
    }
}