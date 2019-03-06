using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;

namespace DirtBnBWebAPI.Controllers
{
    public class UserController : ApiController
    {
        [Route("api/users")]
        [HttpGet]
        public IEnumerable<string> GetUsers()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("api/users/{id}")]
        [HttpGet]
        public User Get(long id)
        {
            User user = new User();

            user.userID = id;
            user.name = "John";
            user.emailAddress = "john@example.com";
            user.phoneNumber = "123-456-7890";
            user.password = "somepassword";

            return user;
        }

        [Route("api/users/")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]User user)
        {
            UserPersistenceService userPersistenceService = new UserPersistenceService();
            long id;
            id = userPersistenceService.SaveUser(user);
            user.userID = id;
            HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("users/{0}", id));
            return response;
        }

        [Route("api/users/{id}")]
        [HttpPut]
        public void Put(int id, [FromBody]User user)
        {
        }

        [Route("api/users/{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
        }
    }
}