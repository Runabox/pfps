using Microsoft.AspNetCore.Mvc;
using Pfps.API.Data;

namespace Pfps.API.Controllers
{
    public class PfpsControllerBase : ControllerBase
    {
        public User PfpsUser { get; set; }
    }
}