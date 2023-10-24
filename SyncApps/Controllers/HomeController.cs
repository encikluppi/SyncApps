using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncApps.Data;
using SyncApps.Services;

namespace SyncApps.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private AppService _AppService;

        public HomeController(AppService appService)
        {
            _AppService =  appService;

        }

        [HttpPost]
        public async Task<string> SyncData()
        {

            var token = await _AppService.Login();

            bool success = await _AppService.SyncTask(token);

            if (success)
            {
                return "Successfully Sync Data";
            }

            return "Failed Sync Data";
        }
    }
}
