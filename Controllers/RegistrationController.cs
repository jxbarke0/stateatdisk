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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var http = new HttpClient())
            {
                //string json = JsonConvert.SerializeObject(newUser);
                var content = new StringContent(newUser.ToString());
                http.DefaultRequestHeaders.Add("x-api-key", "YxKNTgv51J98HUY8dyKEQt+/FbGGRdA+hrhGE6wqnhg=");
                HttpResponseMessage result = await http.PostAsync("https://api.skidata.com/82/v1/user", content);
            };


            return Json("success");
        }
    }
}