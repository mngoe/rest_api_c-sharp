using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenImis.Security.Security;
using OpenImis.ModulesV2;
using OpenImis.ModulesV2.ChequeModule;
using Newtonsoft.Json;

namespace OpenImis.RestApi.Controllers.V2 { 
    
    [ApiVersion("2")]
    [AllowAnonymous]
    [Route("api/")]
    [ApiController]

    public class ChequeController : Controller
    {
        public ChequeController()
        {
        }
        //[HasRights(Rights.DiagnosesDownload)]
        [HttpGet]   //To allow the sending of data that could be used by the consumers
        [Route("GetListChequeItems")]


        public IActionResult GetListChequeItems()
        {
            ClassChequeRequest getRequest = new ClassChequeRequest();
            var response = getRequest.SerializeDr();
            return Json(response);
        }

    }
}

/**************** Generated token to execute your Get Cheque until 07/07/2022 ********************************************************************************************
 * 
 * 
    eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyVVVJRCI6IjRhMWZiYjBlLWFhMzctNGRkYS04OTE0LTk3YzQ3YTViNzY2NSIsImV4cCI6MTY1Nzk2NTIxNiwiaXNzIjoiaHR0cDovL29wZW5pbWlzLm9yZyIsImF1ZCI6Imh0dHA6Ly9vcGVuaW1pcy5vcmcifQ.eSHHgI7O1UA3dhKioeZgvH8N3SDLc8tSdWnu0kfpGeE

*   API Version to provide in this case: 2

*********************************************************************************************************************************************************/
