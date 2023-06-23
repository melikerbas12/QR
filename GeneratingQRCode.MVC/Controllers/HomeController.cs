using Microsoft.AspNetCore.Mvc;

using static QRCoder.PayloadGenerator;

namespace GeneratingQRCode.MVC.Controllers;
public class HomeController : Controller
{

    public IActionResult Index()
    {
        return View();
    }
}