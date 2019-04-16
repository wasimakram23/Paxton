using Paxton.Net2.OemClientLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PaxtonReportAPI.Models;
using PaxtonReportAPI.CustomFilters;
using PaxtonReportAPI.IRepository;

namespace PaxtonReportAPI.Controllers
{
    [BasicAuthenticationFilter]
    public class EventController : ApiController
    {
        IPaxtonRepository _paxtonRepository;
        public EventController(IPaxtonRepository paxtonRepository)
        {
            _paxtonRepository = paxtonRepository;
        }

        // GET: api/Event
        [Route("api/Event")]
        public HttpResponseMessage GetEvent()
        {
            HttpResponseMessage response;
            Result result = _paxtonRepository.getEvent();
            if (!result.status)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, result.message);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK, result.List);
                return response;
            }
        }

        // GET: api/Event/{startDate}/{endDate}
        [Route("api/Event/Eventlog")]
        public HttpResponseMessage GetEvent(string startDate,string endDate)
        {
            HttpResponseMessage response;
            Result result = _paxtonRepository.getEvent(startDate, endDate);
            if (result.status)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, result.List);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, result.message);
                return response;
            }
        }

        //GET: api/Event/Attendes
        [Route("api/Event/Attendes")]
        public HttpResponseMessage GetAttendes(string date="")
        {
            HttpResponseMessage response;
            Result result = _paxtonRepository.getAttendes(date);
            if (result.status)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, result.List);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, result.message);
                return response;
            }
        }

    }
}
