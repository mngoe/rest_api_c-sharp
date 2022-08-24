using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenImis.Security.Security;
using OpenImis.ModulesV2;
using OpenImis.ModulesV2.ListChequeServicesModule;
using Newtonsoft.Json;

namespace OpenImis.RestApi.Controllers.V2 { 
    
    [ApiVersion("2")]
    [AllowAnonymous]
    [Route("api/")]
    [ApiController]

    public class ListChequeServiceController : Controller
    {
        public ListChequeServiceController()
        {
        }
        //[HasRights(Rights.DiagnosesDownload)]
        [HttpGet]   //To allow the sending of data that could be used by the consumers
        [Route("GetListChequeServiceAllItems")]


        public IActionResult GetListChequeServiceAllItems()
        {
            ClassListChequeServicesRequest getlistRequest = new ClassListChequeServicesRequest();
            var response = getlistRequest.ListChequeServ();
            return Json(response);
        }

    }
}

/**************** Generated token to execute your Get Cheque until 08/08/2022 ********************************************************************************************
 * 
 * 
    "access_token": 
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyVVVJRCI6IjRhMWZiYjBlLWFhMzctNGRkYS04OTE0LTk3YzQ3YTViNzY2NSIsImV4cCI6MTY2MTYwNTg2OCwiaXNzIjoiaHR0cDovL29wZW5pbWlzLm9yZyIsImF1ZCI6Imh0dHA6Ly9vcGVuaW1pcy5vcmcifQ.P62EQxk10REkBc__m36vtXoPRZuQ2_ixrCMZOrgUshI",
  "expires_on": "2022-08-10T10:50:16.8424129+01:00"

*   API Version to provide in this case: 2

*********************************************************************************************************************************************************/
