using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;

namespace DirtBnBWebAPI.Controllers
{
    public class ReservationController : ApiController
    {
        [Route("api/reservations")]
        [HttpGet]
        public HttpResponseMessage GetReservations()
        {
            ReservationPersistenceService reservationPersistenceService = new ReservationPersistenceService();
            var reservations = reservationPersistenceService.GetReservations();
            HttpResponseMessage response;
            if (reservations == null || reservations.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No reservations found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, reservations);
            return response;
        }

        [Route("api/reservations/{id}")]
        [HttpGet]
        public HttpResponseMessage GetReservation(long id, [FromBody] Reservation reservationRequest)
        {
            ReservationPersistenceService reservationPersistenceService = new ReservationPersistenceService();
            Reservation reservation = reservationPersistenceService.GetReservation(id);
            HttpResponseMessage response;
            if (reservation == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Reservation not found.");
                return response;
            }

            return Request.CreateResponse(HttpStatusCode.OK, reservation);
        }

        [Route("api/reservations/")]
        [HttpPost]
        public HttpResponseMessage CreateReservation([FromBody]Reservation reservation)
        {
            ReservationPersistenceService reservationPersistenceService = new ReservationPersistenceService();
            HttpResponseMessage response;

            var id = reservationPersistenceService.SaveReservation(reservation);
            if (id < 0)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "TODO: debug");
                return response;
            }
            reservation.reservationID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, reservation);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("reservations/{0}", id));
            return response;
        }

        [Route("api/reservations/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteReservation(long id)
        {
            ReservationPersistenceService reservationPersistenceService = new ReservationPersistenceService();
            bool userExists = false;
            userExists = reservationPersistenceService.DeleteReservation(id);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, "Reservation deleted.");
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Reservation not found.");
                return response;
            }
        }
    }
}