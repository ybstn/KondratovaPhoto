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
    public class ContactsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public ContactsController(IWebHostEnvironment env)
        {
            _env = env;
        }
        XElement root;
        int indx = 0;
        // GET: /<controller>/
        public IActionResult Index()
        {
            var webRoot = _env.WebRootPath;
            List<Contacts> _ContactLinks = new List<Contacts>();

            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            ViewData["ContactsMainText"] = root.Element("Contacts").Element("mainheader").Value;
            ViewData["ContactsSubHeader"] = root.Element("Contacts").Element("subheader").Value;
            ViewData["ContactsImage"] = root.Element("Contacts").Element("image").Value;
         

            int RowsCount = root.Element("Contacts").Elements("row").Count();
            string[][] RowLinks = new string[RowsCount][];
            string[][] RowNames = new string[RowsCount][];
            string[][] RowImages = new string[RowsCount][];
            int CurrentRow = 0;
            foreach (XElement RawElem in root.Element("Contacts").Elements("row"))
            {
                int ElemsCount = RawElem.Elements("link").Count();
                string[] Links = new string[ElemsCount];
                string[] Names = new string[ElemsCount];
                string[] Images = new string[ElemsCount];
                int CurrentLinkNumber= 0;
                foreach (XElement RawLinksElem in RawElem.Elements("link"))
                {
                    Links[CurrentLinkNumber] = RawLinksElem.Attribute("link").Value;
                    Names[CurrentLinkNumber] = RawLinksElem.Value;
                    Images[CurrentLinkNumber] = RawLinksElem.Attribute("image").Value;
                    CurrentLinkNumber++;
                }
                RowLinks[CurrentRow] = Links;
                RowNames[CurrentRow] = Names;
                RowImages[CurrentRow] = Images;
                CurrentRow++;
            }

            _ContactLinks =
                (from node in root.Element("Contacts").Elements("row")
                 select new Contacts
                 {
                     link = RowLinks[indx],
                     linkName = RowNames[indx],
                     linkImage = RowImages[indx],
                     index = indx++
                 }).ToList();
            ViewData["ContactLinks"] = _ContactLinks;
            LoadMenuButtons();
            return View("Contacts");
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
