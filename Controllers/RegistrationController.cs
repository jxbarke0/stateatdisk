using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace skidata_app.Controllers
{
    [Route("/api/[controller]")]
    public class Registration : Controller
    {

        // POST: api/registration
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] object newUser)
        {
            // Leaving in here for future model binding check
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var http = new HttpClient())
            {
                // Changes object to JSON string
                //var content = new StringContent(newUser.ToString());


                // Includes API key in request headers
                http.DefaultRequestHeaders.Add("x-api-key", "YxKNTgv51J98HUY8dyKEQt+/FbGGRdA+hrhGE6wqnhg=");

                // Sends POST to endpoint
                //HttpResponseMessage result = await http.PostAsync("https://api.skidata.com/82/v1/user", content);
                HttpResponseMessage result = await http.PostAsJsonAsync("https://apistage.skidataus.com/user/82/v1/user/", newUser);

                var resbod = result.ReasonPhrase;
                var rescode = result.StatusCode;
                if (result.IsSuccessStatusCode)
                {
                    return Json("success");
                }
                else
                {
                    return Json(result.StatusCode + result.ReasonPhrase);
                }

            };



        }
    }
}