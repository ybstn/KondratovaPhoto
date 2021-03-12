﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Yulya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Yulya.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public ProjectsController(IWebHostEnvironment env)
        {
            _env = env;
        }
        XElement root;
        int indx = 0;

        public IActionResult Index(string id)
        {
            if (id == null || id == "")
            {
                var webRoot = _env.WebRootPath;
                List<Projects> PortfolioContent = new List<Projects>();
                root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
                PortfolioContent =
                    (from node in root.Element("Projects").Elements("gallery")
                     select new Projects
                     {
                         name = node.Attribute("name").Value,
                         description = node.Attribute("description").Value,
                         url = "Projects/?id=" + node.Attribute("url").Value,
                         img = node.Attribute("ImgUrl").Value,
                         index = indx++
                     }).ToList();
                ViewData["ProjectsContent"] = PortfolioContent;

                LoadMenuButtons();
                return View("Projects");
            }
            else
            {
                return Categories(id);
            }

        }


        public IActionResult Categories(string id)
        {
            var webRoot = _env.WebRootPath;
            List<ProjectsGalleryContent> _portfolioGalleryContent = new List<ProjectsGalleryContent>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));

            IEnumerable<XElement> ServiceDescrip =
            from node in root.Element("Projects").Elements("gallery")
            where node.Attribute("url").Value == id
            select node;

            string _path = ServiceDescrip.First().Attribute("ImgFolder").Value;
            string[] GalleryFiles = Directory.GetFiles(System.IO.Path.Combine(webRoot, _path));
            GalleryFiles = FolderShitCleaner(GalleryFiles);
            //Array.Sort(GalleryFiles);

            string firstPhotoPath = '/' + _path + GalleryFiles[0].Substring(GalleryFiles[0].LastIndexOf('/') + 1);
            string lastPhotoPath = '/' + _path + GalleryFiles[GalleryFiles.Length - 1].Substring(GalleryFiles[GalleryFiles.Length - 1].LastIndexOf('/') + 1);
            for (int t = 0; t < GalleryFiles.Length; t++)
            {
                _portfolioGalleryContent.Add(new ProjectsGalleryContent
                {
                    ImgUrl = '/' + _path + GalleryFiles[t],
                    index = t + 1
                });
            }
            ViewData["ProjectsGalleryLastImageIndex"] = GalleryFiles.Length + 1;
            ViewData["ProjectsGalleryFirstPhoto"] = firstPhotoPath;
            ViewData["ProjectsGalleryLastPhoto"] = lastPhotoPath;
            ViewData["ProjectsGalleryContent"] = _portfolioGalleryContent;


            LoadMenuButtons();
            return View("Service");
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
        private string[] FolderShitCleaner(string[] folderContent)
        {

            for (var i = 0; i < folderContent.Length; i++)
            {
                if (folderContent[i].Contains(".DS_Store"))
                {
                    folderContent = Array.FindAll(folderContent, val => val != folderContent[i]);
                    i--;
                }
            }
            for (var i = 0; i < folderContent.Length; i++)
            {
                var iicur = folderContent[i].Substring(folderContent[i].LastIndexOf("/", StringComparison.CurrentCulture) + 1);
                folderContent[i] = iicur;
            }
            folderContent = folderContent.OrderBy(x => x.Length).ToArray();
            return folderContent;
        }
    }
}
