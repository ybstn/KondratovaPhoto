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
    public class ClientsInfoController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public ClientsInfoController(IWebHostEnvironment env)
        {
            _env = env;
        }
        XElement root;
        int indx = 0;

        public IActionResult Index()
        {
            var webRoot = _env.WebRootPath;
            List<ClientsInfoContent> _clientsInfoContent = new List<ClientsInfoContent>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            ViewData["ClientsInfoMainHeader"] = root.Element("InformationForClients").Element("mainheader").Value;
            _clientsInfoContent =
                (from node in root.Element("InformationForClients").Elements("block")
                 select new ClientsInfoContent
                 {
                     header = node.Element("header").Value,
                     text =  node.Element("text").Value.Split('/').ToList(),
                     price = node.Element("price").Value,
                     img = "/" + node.Element("image").Value,
                     index = indx++
                 }).ToList();
            ViewData["ClientsInfoContent"] = _clientsInfoContent;

            LoadMenuButtons();
            return View("ClientsInfo");
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
