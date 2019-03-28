using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DirtBnBWebAPI.Models;
using DirtBnBWebAPI.PersistenceServices;
using System.Web.Http.Cors;

namespace DirtBnBWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReviewController : ApiController
    {
        [Route("api/reviews")]
        [HttpGet]
        public HttpResponseMessage GetReviews()
        {
            ReviewPersistenceService ReviewPersistenceService = new ReviewPersistenceService();
            var Reviews = ReviewPersistenceService.GetReviews();
            HttpResponseMessage response;
            if (Reviews == null || Reviews.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No Reviews found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Reviews);
            return response;
        }

        [Route("api/reviews/aboveavg")]
        [HttpGet]
        public HttpResponseMessage GetAboveAverageReviewPerAccommodation()
        {
            ReviewPersistenceService ReviewPersistenceService = new ReviewPersistenceService();
            var Reviews = ReviewPersistenceService.GetAboveAverageReviews();
            HttpResponseMessage response;
            if (Reviews == null || Reviews.Count.Equals(0))
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No Reviews found.");
                return response;
            }
            response = Request.CreateResponse(HttpStatusCode.OK, Reviews);
            return response;
        }


        [Route("api/Reviews/{id}")]
        [HttpGet]
        public HttpResponseMessage GetReview(long id)
        {
            ReviewPersistenceService ReviewPersistenceService = new ReviewPersistenceService();
            Review Review = ReviewPersistenceService.GetReview(id);
            HttpResponseMessage response;
            if (Review == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Review not found.");
                return response;
            }
            return Request.CreateResponse(HttpStatusCode.OK, Review);
        }

        [Route("api/Reviews/")]
        [HttpPost]
        public HttpResponseMessage CreateReview([FromBody]Review Review)
        {
            ReviewPersistenceService ReviewPersistenceService = new ReviewPersistenceService();
            HttpResponseMessage response;

            if (Review.rating < 0 || Review.rating > 10)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Rating must in [0, 10]. Please try again.");
                return response;

            }

            if (string.IsNullOrEmpty(Review.content))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Content is mandatory. Please try again.");
                return response;
            }

            if (Review.content.Length > 1400)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Content cannot exceed 1400 characters. Please try again.");
                return response;
            }

            long id = ReviewPersistenceService.SaveReview(Review);
            if (id == -1)
            {

                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Ensure referential integrity. Please try again.");
                return response;
            }

            Review.reviewID = id;
            response = Request.CreateResponse(HttpStatusCode.Created, Review);
            response.Headers.Location = new Uri(Request.RequestUri, string.Format("Reviews/{0}", id));
            return response;
        }

        [Route("api/Reviews/selectReviewWithRating")]
        [HttpPost]
        public HttpResponseMessage selectReview([FromBody]SelectReviewOperator body)
        {
            ReviewPersistenceService ReviewPersistenceService = new ReviewPersistenceService();
            HttpResponseMessage response;

            if ((body.op != ">") && (body.op != "<") && (body.op != "="))
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,
                    "op field of body must be one of `>`, `<`, or `=`. Please try again.");
                return response;
            }

            else
            {
                var Reviews = ReviewPersistenceService.GetSelectReviews(body.op, body.rating);
                if (Reviews == null || Reviews.Count.Equals(0))
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, "No Reviews found.");
                    return response;
                }
                response = Request.CreateResponse(HttpStatusCode.OK, Reviews);
                return response;
            }
        }
    }
}

