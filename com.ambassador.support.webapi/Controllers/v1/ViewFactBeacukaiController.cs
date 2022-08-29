using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.ambassador.support.lib.Services;
using com.ambassador.support.webapi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.ambassador.support.webapi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/viewfactbeacukais")]
    [Authorize]
    public class ViewFactBeacukaiController : Controller
    {
        private static readonly string ApiVersion = "1.0";
        private readonly IViewFactBeacukaiService Service;

        public ViewFactBeacukaiController(IViewFactBeacukaiService service)
        {
            this.Service = service;
        }

        [HttpGet("master")]
        public IActionResult Get(int size = 25, string keyword = null)
        {
            try
            {
                var data = Service.Get(size, keyword);
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data,
                    info = new
                    {
                        size,
                        count = data.Count
                    }
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }



    }
}
