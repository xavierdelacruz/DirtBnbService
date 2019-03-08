using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;

namespace DirtBnBWebAPI.Controllers
{
    public class CSRController : ApiController
    {
        [Route("api/csrs")]
        [HttpGet]
        public HttpResponseMessage GetCSRS()
        {
            CSRPersistenceService csrPersistenceService = new CSRPersistenceService();
            var csrs = csrPersistenceService.GetCSRS();
            HttpResponseMessage response;
            if (csrs == null || csrs.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No CSRs found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, csrs);
            return response;
        }

        [Route("api/csrs/{id}")]
        [HttpGet]
        public HttpResponseMessage GetCSR(long id, [FromBody] CSR csrRequest)
        {
            CSRPersistenceService csrPersistenceService = new CSRPersistenceService();
            CSR csr = csrPersistenceService.GetCSR(id);
            HttpResponseMessage response;
            if (csr == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "CSR not found.");
                return response;
            }

            if (csrRequest.password != csr.password)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "Incorrect password. Please try logging again.");
                return response;
            }

            return Request.CreateResponse(HttpStatusCode.OK, csr);
        }

        [Route("api/csrs/")]
        [HttpPost]
        public HttpResponseMessage CreateCSR([FromBody]CSR csr)
        {
            CSRPersistenceService csrPersistenceService = new CSRPersistenceService();
            HttpResponseMessage response;

            if (string.IsNullOrEmpty(csr.name)
                || string.IsNullOrEmpty(csr.phoneNumber)
                || string.IsNullOrEmpty(csr.emailAddress)
                || string.IsNullOrEmpty(csr.password))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "All fields are mandatory. Please try again.");
                return response;
            }

            var id = csrPersistenceService.SaveCSR(csr);
            if (id < 0)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "A CSR with the same email address has already been created");
                return response;
            }
            csr.userID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, csr);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("csrs/{0}", id));
            return response;
        }

        [Route("api/csrs/{id}")]
        [HttpPatch]
        [HttpPut]
        public HttpResponseMessage UpdateCSR(long id, [FromBody]CSR csr)
        {
            CSRPersistenceService csrPersistenceService = new CSRPersistenceService();
            bool userExists = false;
            userExists = csrPersistenceService.UpdateCSR(id, csr);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, csr);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "CSR not found.");
                return response;
            }
        }

        [Route("api/csrs/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteCSR(long id)
        {
            CSRPersistenceService csrPersistenceService = new CSRPersistenceService();
            bool userExists = false;
            userExists = csrPersistenceService.DeleteCSR(id);

            HttpResponseMessage response;
            if (userExists)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, "CSR deleted.");
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "CSR not found.");
                return response;
            }
        }
    }
}