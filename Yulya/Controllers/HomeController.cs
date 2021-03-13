using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Yulya.Models;

namespace Yulya.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;  
        //}
        private readonly IWebHostEnvironment _env;
        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }
        XElement root;
        int indx = 0;
        public IActionResult Index()
        {
            
            var webRoot = _env.WebRootPath;

            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));


            List<MainPage> MainPageContent = new List<MainPage>();
            MainPageContent =
                    (from node in root.Element("MainPage").Elements("MainPageButton")
                     select new MainPage
                     {
                         name = node.Attribute("name").Value,
                         url =  node.Attribute("url").Value,
                         img = node.Attribute("ImgUrl").Value,
                         index = indx++
                     }).ToList();
            ViewData["MainButtons"] = MainPageContent;


            List<Menu> MenuContent = new List<Menu>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            MenuContent =
                (from node in root.Element("menu").Elements("menuitem")
                 select new Menu
                 {
                     name = node.Value,
                     link = node.Attribute("link").Value
                 }).ToList();
            ViewData["MenuButtons"] = MenuContent;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
