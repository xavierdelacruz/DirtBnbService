using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;

namespace DirtBnBWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ComplaintController : ApiController
    {
        [Route("api/complaints")]
        [HttpGet]
        public HttpResponseMessage GetComplaints()
        {
            ComplaintPersistenceService ComplaintPersistenceService = new ComplaintPersistenceService();
            var Complaints = ComplaintPersistenceService.GetComplaints();
            HttpResponseMessage response;
            if (Complaints == null || Complaints.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No Complaints found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Complaints);
            return response;
        }

        [Route("api/complaints/{id}")]
        [HttpGet]
        public HttpResponseMessage GetComplaint(long id)
        {
            ComplaintPersistenceService ComplaintPersistenceService = new ComplaintPersistenceService();
            Complaint Complaint = ComplaintPersistenceService.GetComplaint(id);
            HttpResponseMessage response;
            if (Complaint == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Complaint not found.");
                return response;
            }
            return Request.CreateResponse(HttpStatusCode.OK, Complaint);
        }

        [Route("api/complaints/")]
        [HttpPost]
        public HttpResponseMessage CreateComplaint([FromBody]Complaint Complaint)
        {
            ComplaintPersistenceService ComplaintPersistenceService = new ComplaintPersistenceService();
            HttpResponseMessage response;

            if (string.IsNullOrEmpty(Complaint.content))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Content is mandatory. Please try again.");
                return response;
            }

            if (Complaint.content.Length > 1400 || (!String.IsNullOrEmpty(Complaint.resolution) && Complaint.resolution.Length > 1400))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Content and resolution cannot exceed 1400 characters. Please try again.");
                return response;
            }

            long id = ComplaintPersistenceService.SaveComplaint(Complaint);
            if (id == -1)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Ensure referential integrity. Please try again.");
                return response;
            }

            Complaint.complaintID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, Complaint);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("Complaints/{0}", id));
            return response;
        }

        [Route("api/complaints/{id}")]
        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage UpdateComplaint(long id, [FromBody]Complaint complaint)
        {
            ComplaintPersistenceService complaintPersistenceService = new ComplaintPersistenceService();
            bool complaintExists = false;
            complaintExists = complaintPersistenceService.UpdateComplaint(id, complaint);

            HttpResponseMessage response;
            if (complaintExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, complaint);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Complaint not found.");
                return response;
            }
        }
    }
}