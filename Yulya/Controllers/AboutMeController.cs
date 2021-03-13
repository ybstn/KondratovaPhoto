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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Yulya.Controllers
{
    public class AboutMeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public AboutMeController(IWebHostEnvironment env)
        {
            _env = env;
        }
        XElement root;
        int indx = 0;
        // GET: /<controller>/
        public IActionResult Index()
        {
            var webRoot = _env.WebRootPath;
            List<AboutMeContent> _AboutMeContent = new List<AboutMeContent>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            ViewData["AboutMeMainText"] = root.Element("AboutMe").Element("mainheader").Value;

            _AboutMeContent =
                (from node in root.Element("AboutMe").Elements("block")
                 select new AboutMeContent
                 {
                     header = node.Element("header").Value,
                     text = node.Element("text").Value.Split('/').ToList(),
                     price = node.Element("price").Value,
                     img = "/" + node.Element("image").Value,
                     index = indx++
                 }).ToList();
            ViewData["AboutMeContent"] = _AboutMeContent;                
            LoadMenuButtons();
            return View("AboutMe");
        }

        private void LoadMenuButtons()
        {
            var webRoot = _env.WebRootPath;
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
        }
    }
}
