using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APIsASManager.CoreV3.Controllers
{
    [Route("v{version:apiVersion}/api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiVersion("3.0")]
    public class LoginController : ControllerBase
    {
        static MyClass.IAuthorizationManager authorizationManager;
        MyClass.ASManager asManager;
        public LoginController(MyClass.IAuthorizationManager pAuthorizationManager, IConfiguration configuration)
        {
            authorizationManager = pAuthorizationManager;
            asManager = new MyClass.ASManager(configuration);
        }
        /// <summary>
        /// login to get token before execute APIs
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns>return token</returns>
        [HttpGet]
        public async Task<ActionResult<Object>> Get(string user, string pwd)
        {
            Models.LoginModel vModel = new Models.LoginModel();
            MyClass.ASManager.LoginResponse vLoginRes = asManager.Login().Result;
            if (vLoginRes.Success == 0)
                return Ok(vLoginRes);
            try
            {
                var x = asManager.USER_ID_LIST().Result;
                var vToken = authorizationManager.Authenticate(user, pwd);
                if (vToken == null)
                    return BadRequest();
                vModel.Token = vToken.ToUpper();
                return Ok(vModel);
            }
            catch (Exception exp)
            {
                return BadRequest(new Models.LoginModel());
            }
        }

    }
}
