using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Yulya.Models;

namespace Yulya.Controllers
{
   
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public AdminController(IWebHostEnvironment env)
        {
            _env = env;
        }
        protected bool ResizeAbort()
        {
            return false;
        }
        XElement root;
        int indx = 0;
        string scrollposition = "0";
        [Authorize]
        public IActionResult Index()
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
            ViewData["AdminMenuButtons"] = MenuContent;
            NaviButtons();
            
            return View("MenuStruct");
        }

        [Authorize]
        public IActionResult Menu()
        {
            var webRoot = _env.WebRootPath;
            List<Menu> MenuContent = new List<Menu>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            MenuContent =
                (from node in root.Element("menu").Elements("menuitem")
                 select new Menu
                 {
                     name = node.Value,
                     link = node.Attribute("link").Value,
                     index=indx++
                     
                 }).ToList();
            ViewData["AdminMenuButtons"] = MenuContent;
           
            NaviButtons();
            return View("MenuStruct");
        }
        [HttpPost]
        public IActionResult Menu(string[] menuitems, string[] menulinks)
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int i = 0;
            foreach (var dd in menuitems)
            {
                 var ff = dd;
                if (ff == null)
                {
                    ff = "";
            }
                root.Element("menu").Elements("menuitem").ElementAt(i).Value = ff;
                i++;
            }
            i = 0;
            foreach (var dd in menulinks)
            {
                var ff = dd;
                if (ff == null)
                {
                    ff = "";
            }
                root.Element("menu").Elements("menuitem").ElementAt(i).Attribute("link").Value = ff;
                i++;
            }
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
           


            return RedirectToAction("Menu");
        }
        [Authorize]
        public IActionResult MainPage()
        {
            ViewData["scroll_position"] = scrollposition;
            var webRoot = _env.WebRootPath;
            List<MainPage> MainPageContent = new List<MainPage>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int MenuElemsCount = root.Element("MainPage").Elements("MainPageButton").Count();
            string[] AlbumImagesThumbs = new string[MenuElemsCount];

            //подгружаем тумбнейлы изображений и записываем пути к ним в массив
            for (int d = 0; d < MenuElemsCount; d++)
            {
                var AlbumImageUrl = root.Element("MainPage").Elements("MainPageButton").ElementAt(d).Attribute("ImgUrl").Value;
                AlbumImagesThumbs[d] = AlbumImageUrl.Insert((AlbumImageUrl.LastIndexOf('/') + 1), "thumb/");
            }
            MainPageContent =
                (from node in root.Element("MainPage").Elements("MainPageButton")
                     select new MainPage
                     {
                         name = node.Attribute("name").Value,
                         url = node.Attribute("url").Value,
                         img = node.Attribute("ImgUrl").Value,
                         imgThumb = AlbumImagesThumbs[indx],
                         id = node.Attribute("id").Value,
                         index = indx++
                     }).ToList();
            ViewData["AdminMainButtons"] = MainPageContent;
            NaviButtons();
           
            return View("MainPageStruct");

        }
        [HttpPost]
        public async Task<IActionResult> MainPage(string[] mainPageButtonImage, string[] mainPageButtonName, string[] mainPageButtonUrl, string[] OnPageItemIDs, IFormFile uploadedFile, string id, string actionName, string scroll_position)
        {
            scrollposition = scroll_position;
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int i = 0;
            foreach (var nameElem in mainPageButtonName)
            {
                var tnameElem = nameElem;
                if (nameElem == null)
                {
                    tnameElem = "";
                }
                root.Element("MainPage").Elements("MainPageButton").ElementAt(i).Attribute("name").Value = tnameElem;
                if (mainPageButtonUrl[i] == null)
                {
                    mainPageButtonUrl[i] = "";
                }
                root.Element("MainPage").Elements("MainPageButton").ElementAt(i).Attribute("url").Value = mainPageButtonUrl[i];
                if (mainPageButtonImage[i] == null)
                {
                    mainPageButtonImage[i] = "";
                }
                root.Element("MainPage").Elements("MainPageButton").ElementAt(i).Attribute("ImgUrl").Value = mainPageButtonImage[i];
                root.Element("MainPage").Elements("MainPageButton").ElementAt(i).Attribute("id").Value = OnPageItemIDs[i]; 
                i++;
            }
            if (uploadedFile != null)
            {
                string Dirpath = "Images/MainPage/";
                string path = Dirpath + uploadedFile.FileName;
                string ThumbPath = System.IO.Path.Combine(webRoot, (Dirpath + "thumb/" + uploadedFile.FileName));
                int j = 0;
                string FileForLoadName = "";
                string constPath = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + "copy";
                if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == true)
                {
                    while (true)
                    {
                        j++;
                        FileForLoadName = constPath + j.ToString() + Path.GetExtension(uploadedFile.FileName);
                        path = Dirpath + FileForLoadName;
                        if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == false)
                        { break; }
                    }
                    path = Dirpath + FileForLoadName;
                    ThumbPath = System.IO.Path.Combine(webRoot, (Dirpath + "thumb/" + FileForLoadName));

                }
                using (var fileStream = new FileStream(System.IO.Path.Combine(webRoot, path), FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                    ThumbGenerator(ThumbPath, fileStream, 200);
                }
                root.Element("MainPage").Elements("MainPageButton").Where(x => x.Attribute("id").Value == id).First().Attribute("ImgUrl").Value = path;
            }
          
        
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
           
            if (actionName == "add")
            {
                AddMainPageButton();
                SiteMapGenerator();
            }
            if (actionName == "delete")
            {
                DeleteMainPageButton(id);
                SiteMapGenerator();
            }
            //return RedirectToAction("MainPage");
            return MainPage();
        }
        private void DeleteMainPageButton(string id)
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int elemZeroCheck = root.Element("MainPage").Elements("MainPageButton").Count();
            if (elemZeroCheck>1)
            {
                string ButtImgPath = "Images/MainPage/";
                ButtImgPath = System.IO.Path.Combine(webRoot, ButtImgPath);
                string ButtImgThumbPath = ButtImgPath + "thumb/";
                root.Element("MainPage").Elements("MainPageButton").Where(x => x.Attribute("id").Value == id).First().Remove();
                root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
                AlbumImageCleaner(ButtImgPath, ButtImgThumbPath, root.Element("MainPage").Elements("MainPageButton"));
            }
        }
        private void AddMainPageButton ()
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int galleryCount = root.Element("MainPage").Elements("MainPageButton").Count();
            int[] existGalleryIDs = new int[galleryCount];
            int newGalleryID = 0;
            int counter = 0;
            //Просмотр id существующих элементов в XML и запись их в массив
            foreach (XElement elem in root.Element("MainPage").Elements("MainPageButton"))
            {
                existGalleryIDs[counter] = int.Parse(root.Element("MainPage").Elements("MainPageButton").ElementAt(counter).Attribute("id").Value);

                counter++;
            }
            //поиск в массиве максимального id и увеличение его на единицу
            newGalleryID = existGalleryIDs.Max() + 1;
            string Dirpath = "Images/MainPage/";
            XElement newNode = new XElement("MainPageButton", new XAttribute("id", newGalleryID), new XAttribute("name",""), new XAttribute("url", ""), new XAttribute("ImgUrl", Dirpath));
            root.Element("MainPage").Add(newNode);
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
        }
        [Authorize]
        public IActionResult Portfolio()
        {
            ViewData["scroll_position"] = scrollposition;
            var webRoot = _env.WebRootPath;
            List<Portfolio> PortfolioContent = new List<Portfolio>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int PortfolioElemsCount = root.Element("Portfolio").Elements("gallery").Count();
            string[][] PortfolioGallerys = new string[PortfolioElemsCount][];
            string[] AlbumImagesThumbs = new string[PortfolioElemsCount];
            for (int d = 0; d < PortfolioElemsCount; d++)
            { 
                var IamgePath = root.Element("Portfolio").Elements("gallery").ElementAt(d).Attribute("ImgFolder").Value;
                string _path = IamgePath.Insert((IamgePath.LastIndexOf('/') + 1), "thumb/");

                string[] GalleryFiles = Directory.GetFiles(System.IO.Path.Combine(webRoot, _path));
                GalleryFiles = FolderShitCleaner(GalleryFiles);
                for (int t = 0; t < GalleryFiles.Length; t++)
                {

                    GalleryFiles[t] = _path + GalleryFiles[t];

                }
                PortfolioGallerys[d] = GalleryFiles;
                var AlbumImageUrl = root.Element("Portfolio").Elements("gallery").ElementAt(d).Attribute("ImgUrl").Value;
                AlbumImagesThumbs[d] = AlbumImageUrl.Insert((AlbumImageUrl.LastIndexOf('/') + 1), "thumb/");
            }
           
            
        PortfolioContent =
                (from node in root.Element("Portfolio").Elements("gallery")
                
                 select new Portfolio
                 {
                     name = node.Attribute("name").Value,
                     description= node.Attribute("description").Value,
                     url = node.Attribute("url").Value,
                     imgfolder = node.Attribute("ImgFolder").Value,
                     img = node.Attribute("ImgUrl").Value,
                     imgThumb = AlbumImagesThumbs[indx],
                     gallery = PortfolioGallerys[indx],
                     id = node.Attribute("id").Value,
                     index = indx++
                 }).ToList();
          
            ViewData["AdminPortfolioContent"] = PortfolioContent;
            NaviButtons();
            return View("PortfolioStruct"); 
        }
       //общий метод для проектов и портфолио
        [HttpPost]
        public async Task<IActionResult> Portfolio(string[] PortolioItemID, string[] PortolioItemFolder, string[] PortolioItemImage, string[] PortolioItemName, string[] PortolioItemDescription, string[] PortolioItemUrl, IFormFile uploadedFile, string DelItemID, string id, string actionName, string pageName, IFormFile[] uploadedFiles, string scroll_position)
        {
            scrollposition = scroll_position;
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));

            int i = 0;
            foreach (var nameElem in PortolioItemName)
            {
                var tnameElem = nameElem;
                if (nameElem == null)
                {
                    tnameElem = "";
                }
                root.Element(pageName).Elements("gallery").ElementAt(i).Attribute("name").Value = tnameElem;
                if (PortolioItemDescription[i] == null)
                {
                    PortolioItemDescription[i] = "";
                }
                root.Element(pageName).Elements("gallery").ElementAt(i).Attribute("description").Value = PortolioItemDescription[i];
                if (PortolioItemUrl[i] == null)
                {
                    PortolioItemUrl[i] = "";
                }
                root.Element(pageName).Elements("gallery").ElementAt(i).Attribute("url").Value = PortolioItemUrl[i];
                if (PortolioItemImage[i] == null)
                {
                    PortolioItemImage[i] = "";
                }
                root.Element(pageName).Elements("gallery").ElementAt(i).Attribute("ImgUrl").Value = PortolioItemImage[i];
                if (PortolioItemFolder[i] == null)
                {
                    PortolioItemFolder[i] = "";
                }
                root.Element(pageName).Elements("gallery").ElementAt(i).Attribute("ImgFolder").Value = PortolioItemFolder[i];
                root.Element(pageName).Elements("gallery").ElementAt(i).Attribute("id").Value = PortolioItemID[i];
                i++;
            }

            if (uploadedFiles.Count() > 0)
            {
                string Dirpath = "Images/"+ pageName + "/" + id + "/";
                string FullDirpath = System.IO.Path.Combine(webRoot, Dirpath);
                foreach (var frmFile in uploadedFiles)
                {
                    if (frmFile.Length > 0)
                    {
                        string Filepath = FullDirpath + frmFile.FileName;
                        string ThumbPath = FullDirpath + "thumb/" + frmFile.FileName;
                        using (var fileStream = new FileStream(Filepath, FileMode.Create))
                        {
                            await frmFile.CopyToAsync(fileStream);

                            ThumbGenerator(ThumbPath, fileStream, 100);
                        }
                        Stream resourceImage = frmFile.OpenReadStream();
                    }
                }
            }
            if (uploadedFile != null)
            {
                string Dirpath = "Images/"+ pageName + "/ButtonsImages/";
                string path = Dirpath + uploadedFile.FileName;
                string ThumbPath = System.IO.Path.Combine(webRoot, (Dirpath + "thumb/" + uploadedFile.FileName));
                int j = 0;
                string FileForLoadName = "";
                string constPath = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + "copy";
                if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == true)
                {
                    while (true)
                    {
                        j++;
                        FileForLoadName = constPath + j.ToString() + Path.GetExtension(uploadedFile.FileName);
                        path = Dirpath + FileForLoadName;
                        if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == false)
                        { break; }
                    }
                    path = Dirpath + FileForLoadName;
                    ThumbPath = System.IO.Path.Combine(webRoot, (Dirpath + "thumb/" + FileForLoadName));
                }
                using (var fileStream = new FileStream(System.IO.Path.Combine(webRoot, path), FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                    ThumbGenerator(ThumbPath, fileStream, 200);
                }
                root.Element(pageName).Elements("gallery").Where(x => x.Attribute("id").Value == id).First().Attribute("ImgUrl").Value = path;
            }
            if (actionName == "add")
            {
                AddPortfolioItem(pageName);
                SiteMapGenerator();
            }
            if (actionName == "delete")
            {
                DeleteXMLGalleryElement(pageName, id);
                SiteMapGenerator();
            }
            if (actionName == "deleteItem")
            {
                string Dirpath = "Images/" + pageName + "/" + id + "/";
                int DelItemID_int = int.Parse(DelItemID);
                DeleteXMLGalleryElementImage(Dirpath, DelItemID_int);
            }
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
          
            return RedirectToAction(pageName);
        }

        //удаляем элемнт XML с галереями. Получает название элемента в XML.
        private void DeleteXMLGalleryElement(string PageActionName, string id)
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int elemZeroCheck = root.Element(PageActionName).Elements("gallery").Count();
            if (elemZeroCheck > 1)
            {
                //объявление путей до файлов галереи портфолио для удаления
                string Dirpath = root.Element(PageActionName).Elements("gallery").Where(x => x.Attribute("id").Value == id).First().Attribute("ImgFolder").Value;
                string ThumbPath = Dirpath + "thumb/";
                string FullDirpath = System.IO.Path.Combine(webRoot, Dirpath);
                string FullThumbPath = System.IO.Path.Combine(webRoot, ThumbPath);

                //удаление файлов галереи для выбранной строки и их тумбнейлов
                if (Directory.Exists(FullDirpath))
                {
                    System.IO.DirectoryInfo dirInfo = new DirectoryInfo(FullDirpath);
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        file.Delete();
                    }

                    if (Directory.Exists(FullThumbPath))
                    {
                        System.IO.DirectoryInfo ThumbDirInfo = new DirectoryInfo(FullThumbPath);
                        foreach (FileInfo file in ThumbDirInfo.GetFiles())
                        {
                            file.Delete();
                        }
                        Directory.Delete(FullThumbPath);
                    }
                    Directory.Delete(FullDirpath);
                }

                //Удаление строки из XML
                root.Element(PageActionName).Elements("gallery").Where(x => x.Attribute("id").Value == id).First().Remove();
                root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));

                //объявление путей до обложек альбомов портфолио для передачи в метод PortfolioAlbumImageCleaner
                //для удаления. Значение пути берётся из 0-го элемента
                string ButtImgPath = System.IO.Path.Combine(webRoot, "Images/" + PageActionName + "/ButtonsImages/");
                string ButtImgThumbPath = ButtImgPath + "thumb/";
                //Передача в метод данных файлов обложек альбомов Потрфолио для их удаления
                AlbumImageCleaner(ButtImgPath, ButtImgThumbPath, root.Element(PageActionName).Elements("gallery"));
            }
           
        }
        //удаляем все файлы из папки обложек альбомов, названия которых не содержатся в XML в разделе Портфолио
        private void AlbumImageCleaner(string FullDirPath, string FullThumbPath, IEnumerable<XElement> XmlElements)
        {
            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(FullDirPath);
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                //флаг удаления файла
                bool CanDelete = false;

                //Сравниваем соответствующий атрибут каждого элемента раздела портфолио с названием файла, 
                //если такое название фала есть - завершаем цикл с установкой значения флага удаления false
                //если нет устанавливаем значение флага true
                foreach (XElement elem in XmlElements)
                {
                    string FromXmlFilePath = elem.Attribute("ImgUrl").Value;
                    if (file.Name == FromXmlFilePath.Substring(FromXmlFilePath.LastIndexOf('/') + 1))
                    {
                        CanDelete = false;
                        break;
                    }
                    else { CanDelete = true; }

                }
                //удаляем файл и его тумбнейл
                if (CanDelete)
                {
                    FileInfo FileForDeleteInfo = new FileInfo(FullThumbPath + file.Name);
                    FileForDeleteInfo.Delete();
                    file.Delete();
                }

            }
        }
        //удаляем фотографии из галереи. Получает путь к папке с галерей 
        private void DeleteXMLGalleryElementImage(string _GalleryDeleteImgPath, int id)
        {
            var webRoot = _env.WebRootPath;
            string Filepath = Path.Combine(webRoot, _GalleryDeleteImgPath);
            string[] filesInPath = Directory.GetFiles(Filepath);
            filesInPath= FolderShitCleaner(filesInPath);
            string FullFilepath = Filepath+filesInPath[id];
           
            string FullThumbFilepath = FullFilepath.Insert(FullFilepath.LastIndexOf('/')+1,"thumb/");

            if (System.IO.File.Exists(FullFilepath))
            {
                System.IO.File.Delete(FullFilepath);
            }
            if (System.IO.File.Exists(FullThumbFilepath))
            {
                System.IO.File.Delete(FullThumbFilepath);
            }
        }
        //создаём элемент XML с галереями. Получаем название элемента в XML оно же является названием папки страницы
        private void AddPortfolioItem(string AddXMLElementPath)
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int galleryCount = root.Element(AddXMLElementPath).Elements("gallery").Count();
            int[] existGalleryIDs = new int[galleryCount];
            int newGalleryID = 0;
            int counter = 0;
            //Просмотр id существующих элементов в XML и запись их в массив
            foreach (XElement elem in root.Element(AddXMLElementPath).Elements("gallery"))
            {
                existGalleryIDs[counter] = int.Parse(root.Element(AddXMLElementPath).Elements("gallery").ElementAt(counter).Attribute("id").Value);

                counter++;
            }
            //поиск в массиве максимального id и увеличение его на единицу
            newGalleryID = existGalleryIDs.Max() + 1;
            //связываем  id и папку с фалами галереи
            //обязательно добавляем в новый элемент не пустое значение пути до обложки,
            //в дальнейших методах происходит чтение пути из 0го элемента
            string CoverImagePath = "Images/" + AddXMLElementPath + "/ButtonsImages/";
            string Dirpath = "Images/"+ AddXMLElementPath + "/" + newGalleryID + "/"; 
             string FullDirpath = System.IO.Path.Combine(webRoot, Dirpath);
            //создаём папку галереи и папку её тумбнейлов 
            if (!Directory.Exists(FullDirpath))
            {
                Directory.CreateDirectory(FullDirpath);
                Directory.CreateDirectory(FullDirpath + "thumb/");
            }
            //создаём новый элемент галереи и добавляем в xml
            XElement newNode = new XElement("gallery", new XAttribute("id", newGalleryID), new XAttribute("name", ""), new XAttribute("description", ""), new XAttribute("url", ""), new XAttribute("ImgUrl", CoverImagePath), new XAttribute("ImgFolder", Dirpath));

            root.Element(AddXMLElementPath).Add(newNode);
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
        }
        // генератор тумбнейлов. На входе путь к файлу, файл картинка и ширина картинки)
        private void ThumbGenerator(string ThumbPath, FileStream InputImg, int ThumbWidth)
        {
            Image CurrImg = Image.FromStream(InputImg);
            int X = CurrImg.Width;
            int Y = CurrImg.Height;
            int width = (int)((X * ThumbWidth) / Y);
            Image Thumb = CurrImg.GetThumbnailImage(width, ThumbWidth, () => false, IntPtr.Zero);
            Thumb.Save(ThumbPath);
        }
        // очистка входной папки от файлов ".DS_Store" и упорядочивание файлов с числовым именем по возрастанию 
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

        //Действие страницы проектов
        [Authorize]
        public IActionResult Projects()
        {
            ViewData["scroll_position"] = scrollposition;
            var webRoot = _env.WebRootPath;
            List<Projects> PortfolioContent = new List<Projects>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int PortfolioElemsCount = root.Element("Projects").Elements("gallery").Count();
            string[][] PortfolioGallerys = new string[PortfolioElemsCount][];
            string[] AlbumImagesThumbs = new string[PortfolioElemsCount];

            //подгружаем тумбнейлы изображений (галерея и обложки) и записываем пути к ним в массивы
            for (int d = 0; d < PortfolioElemsCount; d++)
            {
                var IamgePath = root.Element("Projects").Elements("gallery").ElementAt(d).Attribute("ImgFolder").Value;
                string _path = IamgePath.Insert((IamgePath.LastIndexOf('/') + 1), "thumb/");

                string[] GalleryFiles = Directory.GetFiles(System.IO.Path.Combine(webRoot, _path));
                //
                GalleryFiles = FolderShitCleaner(GalleryFiles);
                for (int t = 0; t < GalleryFiles.Length; t++)
                {

                    GalleryFiles[t] = _path + GalleryFiles[t];

                }
                PortfolioGallerys[d] = GalleryFiles;
                var AlbumImageUrl = root.Element("Projects").Elements("gallery").ElementAt(d).Attribute("ImgUrl").Value;
                AlbumImagesThumbs[d] = AlbumImageUrl.Insert((AlbumImageUrl.LastIndexOf('/') + 1), "thumb/");
            }

            //заполняем класс для отображения данных в представлении
            PortfolioContent =
                    (from node in root.Element("Projects").Elements("gallery")

                     select new Projects
                     {
                         name = node.Attribute("name").Value,
                         description = node.Attribute("description").Value,
                         url = node.Attribute("url").Value,
                         imgfolder = node.Attribute("ImgFolder").Value,
                         img = node.Attribute("ImgUrl").Value,
                         imgThumb = AlbumImagesThumbs[indx],
                         gallery = PortfolioGallerys[indx],
                         id = node.Attribute("id").Value,
                         index = indx++
                     }).ToList();

            ViewData["AdminProjectsContent"] = PortfolioContent;
            NaviButtons();
            return View("ProjectsStruct");
        }
        [Authorize]
        public IActionResult InformationForClients()
        {
            ViewData["scroll_position"] = scrollposition;
            var webRoot = _env.WebRootPath;
            List<ClientsInfoContent> _clientsInfoContent = new List<ClientsInfoContent>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));

            int PortfolioElemsCount = root.Element("InformationForClients").Elements("block").Count();
            string[] AlbumImagesThumbs = new string[PortfolioElemsCount];

            //подгружаем тумбнейлы изображений и записываем пути к ним в массив
            for (int d = 0; d < PortfolioElemsCount; d++)
            {
                var AlbumImageUrl = root.Element("InformationForClients").Elements("block").ElementAt(d).Element("image").Value;
                AlbumImagesThumbs[d] = AlbumImageUrl.Insert((AlbumImageUrl.LastIndexOf('/') + 1), "thumb/");
            }

            ViewData["AdminClientsInfoMainHeader"] = root.Element("InformationForClients").Element("mainheader").Value;
            _clientsInfoContent =
                (from node in root.Element("InformationForClients").Elements("block")
                 select new ClientsInfoContent
                 {
                     header = node.Element("header").Value,
                     text = node.Element("text").Value.Split('/').ToList(),
                     textString = node.Element("text").Value,
                     price = node.Element("price").Value,
                     img = node.Element("image").Value,
                     id = node.Attribute("id").Value,
                     imgThumb = AlbumImagesThumbs[indx],
                     index = indx++
                 }).ToList();
            ViewData["AdminInformationForClientsContent"] = _clientsInfoContent;
            NaviButtons();
            return View("InformationForClientsStruct");
        }
        [HttpPost]
        public async Task<IActionResult> InformationForClients(string PageHeader, string[] CustomHeaders, string[] CustomTexts, string[] CustomPrices, string[] CustomImage, string[] OnPageItemIDs, IFormFile uploadedFile, string id, string actionName, string scroll_position)
        {
            scrollposition = scroll_position;
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int i = 0;
            if (PageHeader == null)
            {
                PageHeader = "";
            }
            root.Element("InformationForClients").Element("mainheader").Value = PageHeader;
            foreach (var nameElem in CustomHeaders)
            {
                var tnameElem = nameElem;
                if (nameElem == null)
                {
                    tnameElem = "";
                }
                root.Element("InformationForClients").Elements("block").ElementAt(i).Element("header").Value = tnameElem;
                if (CustomTexts[i] == null)
                {
                    CustomTexts[i] = "";
                }
                root.Element("InformationForClients").Elements("block").ElementAt(i).Element("text").Value = CustomTexts[i];
                if (CustomPrices[i] == null)
                {
                    CustomPrices[i] = "";
                }
                root.Element("InformationForClients").Elements("block").ElementAt(i).Element("price").Value = CustomPrices[i];
                if (CustomImage[i] == null)
                {
                    CustomImage[i] = "";
                }
                root.Element("InformationForClients").Elements("block").ElementAt(i).Element("image").Value = CustomImage[i];
                root.Element("InformationForClients").Elements("block").ElementAt(i).Attribute("id").Value = OnPageItemIDs[i];
                i++;
            }
            if (uploadedFile != null)
            {
                string FirstElemPath = root.Element("InformationForClients").Elements("block").First().Element("image").Value;
              
                FirstElemPath = FirstElemPath.Substring(0, FirstElemPath.LastIndexOf('/') + 1);
                string path = FirstElemPath + uploadedFile.FileName;
                string ThumbPath = System.IO.Path.Combine(webRoot, (FirstElemPath + "thumb/" + uploadedFile.FileName));
                int j = 0;
                string FileForLoadName = "";
                string constPath = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + "copy";
                if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == true)
                {
                    while (true)
                    {
                        j++;
                        FileForLoadName = constPath + j.ToString() + Path.GetExtension(uploadedFile.FileName);
                        path = FirstElemPath + FileForLoadName;
                        if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == false)
                        { break; }
                    }
                    path = FirstElemPath + FileForLoadName;
                    ThumbPath = System.IO.Path.Combine(webRoot, (FirstElemPath + "thumb/" + FileForLoadName));
                }

                using (var fileStream = new FileStream(System.IO.Path.Combine(webRoot, path), FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);

                    ThumbGenerator(ThumbPath, fileStream, 200);
                }
                root.Element("InformationForClients").Elements("block").Where(x => x.Attribute("id").Value == id).First().Element("image").Value = path;
                

               
            }
            if (actionName == "add")
            {
                AddInfoBlock("InformationForClients");
                SiteMapGenerator();
            }
            if (actionName == "delete")
            {
                DeleteInfoBlock("InformationForClients", id);
                SiteMapGenerator();
            }

            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            return InformationForClients();
        }
        //создаём элемент XML для инфоблоков(для клиентов/обо мне). Получаем название элемента в XML оно же является названием папки страницы
        private void AddInfoBlock(string AddXMLElementPath)
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int galleryCount = root.Element(AddXMLElementPath).Elements("block").Count();
            int[] existGalleryIDs = new int[galleryCount];
            int newGalleryID = 0;
            int counter = 0;
            //Просмотр id существующих элементов в XML и запись их в массив
            foreach (XElement elem in root.Element(AddXMLElementPath).Elements("block"))
            {
                existGalleryIDs[counter] = int.Parse(root.Element(AddXMLElementPath).Elements("block").ElementAt(counter).Attribute("id").Value);

                counter++;
            }
            //поиск в массиве максимального id и увеличение его на единицу
            newGalleryID = existGalleryIDs.Max() + 1;
            //сразу записываем путь к файлам в создаваемый инфоблок
            string Dirpath = "Images/" + AddXMLElementPath + "/" ;
            //создаём новый элемент галереи и добавляем в xml
            XElement newNode = new XElement("block", new XAttribute("id", newGalleryID), new XElement("header"), new XElement("text"), new XElement("price"), new XElement("image", Dirpath));
            root.Element(AddXMLElementPath).Add(newNode);
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
        }
        //удаляем инфоблок(для клиентов/обо мне). Получает название элемента в XML.
        private void DeleteInfoBlock(string PageActionName, string id)
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int elemZeroCheck = root.Element(PageActionName).Elements("block").Count();
            if (elemZeroCheck > 1)
            {
                //объявление путей до обложек инфоблоков для передачи в метод PortfolioAlbumImageCleaner
                //для удаления. Значение пути берётся из 0-го элемента
                string ButtImgPath = root.Element(PageActionName).Elements("block").ElementAt(0).Element("image").Value;
                ButtImgPath = ButtImgPath.Substring(0, (ButtImgPath.LastIndexOf('/') + 1));
                ButtImgPath = System.IO.Path.Combine(webRoot, ButtImgPath);
                string ButtImgThumbPath = ButtImgPath + "thumb/";

                //Удаление строки из XML
                root.Element(PageActionName).Elements("block").Where(x => x.Attribute("id").Value == id).First().Remove();
                root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));

                //Передача в метод данных файлов обложек альбомов Потрфолио для их удаления
                InfoBlockImageCleaner(ButtImgPath, ButtImgThumbPath, root.Element(PageActionName).Elements("block"));
            }

        }
        //удаляем все файлы из папки обложек альбомов, названия которых не содержатся в XML 
        private void InfoBlockImageCleaner(string FullDirPath, string FullThumbPath, IEnumerable<XElement> XmlElements)
        {
            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(FullDirPath);
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                //флаг удаления файла
                bool CanDelete = false;

                //Сравниваем соответствующий элемент каждого инфоблока с названием файла, 
                //если такое название фала есть - завершаем цикл с установкой значения флага удаления false
                //если нет устанавливаем значение флага true
                foreach (XElement elem in XmlElements)
                {
                    string FromXmlFilePath = elem.Element("image").Value;
                    if (file.Name == FromXmlFilePath.Substring(FromXmlFilePath.LastIndexOf('/') + 1))
                    {
                        CanDelete = false;
                        break;
                    }
                    else { CanDelete = true; }

                }
                //удаляем файл и его тумбнейлы
                if (CanDelete)
                {
                    string BigThumbPath = FullDirPath + "thumbBig/";
                    FileInfo BigFileThumb = new FileInfo(BigThumbPath + file.Name);
                    if (BigFileThumb.Exists)
                    {
                        BigFileThumb.Delete();
                    }
                    FileInfo FileThumb = new FileInfo(FullThumbPath + file.Name);
                    if (FileThumb.Exists)
                    {
                        FileThumb.Delete();
                    }

                    file.Delete();
                }

            }
        }
        [Authorize]
        public IActionResult AboutMe()
        {
            ViewData["scroll_position"] = scrollposition;
            var webRoot = _env.WebRootPath;
            List<AboutMeContent> _AboutMeContent = new List<AboutMeContent>();
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));

            int PortfolioElemsCount = root.Element("AboutMe").Elements("block").Count();
            string[] AlbumImagesThumbs = new string[PortfolioElemsCount];

            //подгружаем тумбнейлы изображений и записываем пути к ним в массив
            for (int d = 0; d < PortfolioElemsCount; d++)
            {
                var AlbumImageUrl = root.Element("AboutMe").Elements("block").ElementAt(d).Element("image").Value;
                AlbumImagesThumbs[d] = AlbumImageUrl.Insert((AlbumImageUrl.LastIndexOf('/') + 1), "thumb/");
            }

            ViewData["AdminClientsInfoMainHeader"] = root.Element("AboutMe").Element("mainheader").Value;
            _AboutMeContent =
                (from node in root.Element("AboutMe").Elements("block")
                 select new AboutMeContent
                 {
                     header = node.Element("header").Value,
                     text = node.Element("text").Value.Split('/').ToList(),
                     textString = node.Element("text").Value,
                     price = node.Element("price").Value,
                     img = node.Element("image").Value,
                     imgThumb = AlbumImagesThumbs[indx],
                     id = node.Attribute("id").Value,
                     index = indx++
                 }).ToList();
            ViewData["AdminAboutMeContent"] = _AboutMeContent;
            NaviButtons();
            return View("AboutMeStruct");
        }
        [HttpPost]
        public async Task<IActionResult> AboutMe(string PageHeader, string[] CustomHeaders, string[] CustomTexts, string[] CustomPrices, string[] CustomImage, string[] OnPageItemIDs, IFormFile uploadedFile, string id, string actionName, string scroll_position)
        {
            scrollposition = scroll_position;
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int i = 0;
            if (PageHeader == null)
            {
                PageHeader = "";
            }
            root.Element("AboutMe").Element("mainheader").Value = PageHeader;
            foreach (var nameElem in CustomHeaders)
            {
                var tnameElem = nameElem;
                if (nameElem == null)
                {
                    tnameElem = "";
                }
                root.Element("AboutMe").Elements("block").ElementAt(i).Element("header").Value = tnameElem;
                if (CustomTexts[i] == null)
                {
                    CustomTexts[i] = "";
                }
                root.Element("AboutMe").Elements("block").ElementAt(i).Element("text").Value = CustomTexts[i];
                if (CustomPrices[i] == null)
                {
                    CustomPrices[i] = "";
                }
                root.Element("AboutMe").Elements("block").ElementAt(i).Element("price").Value = CustomPrices[i];
                if (CustomImage[i] == null)
                {
                    CustomImage[i] = "";
                }
                root.Element("AboutMe").Elements("block").ElementAt(i).Element("image").Value = CustomImage[i];
                root.Element("AboutMe").Elements("block").ElementAt(i).Attribute("id").Value = OnPageItemIDs[i];
                i++;
            }
            if (uploadedFile != null)
            {
                string FirstElemPath = root.Element("AboutMe").Elements("block").First().Element("image").Value;

                FirstElemPath = FirstElemPath.Substring(0, FirstElemPath.LastIndexOf('/') + 1);
                string path = FirstElemPath + uploadedFile.FileName;
                string ThumbPath = System.IO.Path.Combine(webRoot, (FirstElemPath + "thumb/" + uploadedFile.FileName));
                int j = 0;
                string FileForLoadName = "";
                string constPath = Path.GetFileNameWithoutExtension(uploadedFile.FileName) + "copy";
                if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == true)
                {
                    while (true)
                    {
                        j++;
                        FileForLoadName = constPath + j.ToString() + Path.GetExtension(uploadedFile.FileName);
                        path = FirstElemPath + FileForLoadName;
                        if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == false)
                        { break; }
                    }
                    path = FirstElemPath + FileForLoadName;
                    ThumbPath = System.IO.Path.Combine(webRoot, (FirstElemPath + "thumb/" + FileForLoadName));

                }

                using (var fileStream = new FileStream(System.IO.Path.Combine(webRoot, path), FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);

                    ThumbGenerator(ThumbPath, fileStream, 200);
                }
                root.Element("AboutMe").Elements("block").Where(x => x.Attribute("id").Value == id).First().Element("image").Value = path;
            }
            
            if (actionName == "add")
            {
                AddInfoBlock("AboutMe");
                SiteMapGenerator();
            }
            if (actionName == "delete")
            {
                DeleteInfoBlock("AboutMe", id);
                SiteMapGenerator();
            }
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            return AboutMe();
        }
        [Authorize]
        public IActionResult Contacts()
        {
            ViewData["scroll_position"] = scrollposition;
            var webRoot = _env.WebRootPath;
            List<Contacts> _ContactLinks = new List<Contacts>();

            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            ViewData["AdminContactsMainText"] = root.Element("Contacts").Element("mainheader").Value;
            ViewData["AdminContactsSubHeader"] = root.Element("Contacts").Element("subheader").Value;
            ViewData["AdminContactsImage"] = root.Element("Contacts").Element("image").Value;
            var ContactsMainImageUrl = root.Element("Contacts").Element("image").Value;
            ViewData["AdminContactsImageThumb"] = ContactsMainImageUrl.Insert((ContactsMainImageUrl.LastIndexOf('/') + 1), "thumb/");
            int RowsCount = root.Element("Contacts").Elements("row").Count();
            string[][] RowLinks = new string[RowsCount][];
            string[][] RowNames = new string[RowsCount][];
            string[][] RowImages = new string[RowsCount][];
            string[][] RowIds = new string[RowsCount][];
            int CurrentRow = 0;
            foreach (XElement RawElem in root.Element("Contacts").Elements("row"))
            {
                int ElemsCount = RawElem.Elements("link").Count();
                string[] Links = new string[ElemsCount];
                string[] Names = new string[ElemsCount];
                string[] Images = new string[ElemsCount];
                string[] iDs = new string[ElemsCount];
                int CurrentLinkNumber = 0;
                foreach (XElement RawLinksElem in RawElem.Elements("link"))
                {
                    Links[CurrentLinkNumber] = RawLinksElem.Attribute("link").Value;
                    Names[CurrentLinkNumber] = RawLinksElem.Value;
                    Images[CurrentLinkNumber] = RawLinksElem.Attribute("image").Value;
                    iDs[CurrentLinkNumber] = RawLinksElem.Attribute("id").Value;
                    CurrentLinkNumber++;
                }
                RowLinks[CurrentRow] = Links;
                RowNames[CurrentRow] = Names;
                RowImages[CurrentRow] = Images;
                RowIds[CurrentRow] = iDs;
                CurrentRow++;
            }

            _ContactLinks =
                (from node in root.Element("Contacts").Elements("row")
                 select new Contacts
                 {
                     link = RowLinks[indx],
                     linkName = RowNames[indx],
                     linkImage = RowImages[indx],
                     Ids = RowIds[indx],
                     index = indx++
                 }).ToList();
            ViewData["ContactLinks"] = _ContactLinks;

            NaviButtons();
            return View("ContactsStruct");
        }
        [HttpPost]
        public async Task<IActionResult> Contacts(string PageHeader, string PageSubHeader, IFormFile uploadedMainFile, string[] ContactsTexts, string[] ContactsLinks, string[] ContactsImages, string[] RowsAfterMove, string[] OnPageItemIDs, IFormFile uploadedFile, string id, string actionName, string scroll_position)
        {
            scrollposition = scroll_position;
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            
            if (PageHeader == null)
            {
                PageHeader = "";
            }
            root.Element("Contacts").Element("mainheader").Value = PageHeader;
            if (PageSubHeader == null)
            {
                PageSubHeader = "";
            }
            root.Element("Contacts").Element("subheader").Value = PageSubHeader;
            int TotalLinknumber = 0;
            int rownumber = 0;
            foreach (var rowElem in RowsAfterMove)
            {
                //перестройка xml под модель страницы, переменная RowsAfterMove - количество рядов и ссылок в каждом ряде
                int OldXmlElemCount = root.Element("Contacts").Elements("row").ElementAt(rownumber).Elements("link").Count();
                int NewXmlElemCount = int.Parse(RowsAfterMove[rownumber]);
                //если число элементов входного массива не равно числу элементов XML
                if (NewXmlElemCount != OldXmlElemCount)
                {
                    //удаляем все элементы XML из элемента row
                    root.Element("Contacts").Elements("row").ElementAt(rownumber).Elements("link").Remove();
                    root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
                    //добавляем пустые элементы по количеству элементов в массиве
                  
                    for (var i=0;i< NewXmlElemCount; i++)
                    {
                        string Dirpath = "Images/Contacts/";
                        XElement newNode = new XElement("link", new XAttribute("id",""), new XAttribute("image", Dirpath), new XAttribute("link", ""));
                        newNode.Value = " ";
                        root.Element("Contacts").Elements("row").ElementAt(rownumber).Add(newNode);
                        root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
                    }
                }
                //заполняем пустые или не пустые элементы
                int linkNumber = 0;
                foreach (var linkElem in root.Element("Contacts").Elements("row").ElementAt(rownumber).Elements("link"))
                {
                    if (ContactsLinks[TotalLinknumber] == null)
                    {
                        ContactsLinks[TotalLinknumber] = "";
                    }
        
                    root.Element("Contacts").Elements("row").ElementAt(rownumber).Elements("link").ElementAt(linkNumber).Attribute("link").Value = ContactsLinks[TotalLinknumber];
                    if (ContactsTexts[TotalLinknumber] == null)
                    {
                        ContactsTexts[TotalLinknumber] = "";
                    }
                    root.Element("Contacts").Elements("row").ElementAt(rownumber).Elements("link").ElementAt(linkNumber).Value = ContactsTexts[TotalLinknumber];
              
                    if (ContactsImages[TotalLinknumber] == null)
                    {
                        ContactsImages[TotalLinknumber] = "";
                    }
                    root.Element("Contacts").Elements("row").ElementAt(rownumber).Elements("link").ElementAt(linkNumber).Attribute("image").Value = ContactsImages[TotalLinknumber];
                    root.Element("Contacts").Elements("row").ElementAt(rownumber).Elements("link").ElementAt(linkNumber).Attribute("id").Value = OnPageItemIDs[TotalLinknumber];
                    linkNumber++;
                    TotalLinknumber++;
                }
                rownumber++;
            }
            if (uploadedMainFile != null)
            {
                string FirstElemPath = root.Element("Contacts").Element("image").Value;

                FirstElemPath = FirstElemPath.Substring(0, FirstElemPath.LastIndexOf('/') + 1);
                string path = FirstElemPath + uploadedMainFile.FileName;
                string ThumbPath = System.IO.Path.Combine(webRoot, (FirstElemPath + "thumb/" + uploadedMainFile.FileName));
                int j = 0;
                string FileForLoadName = "";
                string constPath = Path.GetFileNameWithoutExtension(uploadedMainFile.FileName) + "copy";
                if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == true)
                {
                    while (true)
                    {
                        j++;
                        FileForLoadName = constPath + j.ToString() + Path.GetExtension(uploadedMainFile.FileName);
                        path = FirstElemPath + FileForLoadName;
                        if (System.IO.File.Exists(System.IO.Path.Combine(webRoot, path)) == false)
                        { break; }
                    }
                    path = FirstElemPath + FileForLoadName;
                    ThumbPath = System.IO.Path.Combine(webRoot, (FirstElemPath + "thumb/" + FileForLoadName));
                }
                using (var fileStream = new FileStream(System.IO.Path.Combine(webRoot, path), FileMode.Create))
                {
                    await uploadedMainFile.CopyToAsync(fileStream);
                    ThumbGenerator(ThumbPath, fileStream, 200);
                }
                root.Element("Contacts").Element("image").Value = path;
            }
            if (uploadedFile != null)
            {
                int RawId = 0;
                string LinkId = "";
                if (id != null)
                {
                    string[] IDstringArray = id.Split('?');
                    RawId = int.Parse(IDstringArray[0]);
                    LinkId = IDstringArray[1];
                }
                string FirstElemPath = root.Element("Contacts").Elements("row").First().Element("link").Attribute("image").Value;
                FirstElemPath = FirstElemPath.Substring(0, FirstElemPath.LastIndexOf('/') + 1);
                string path = FirstElemPath + uploadedFile.FileName;
                using (var fileStream = new FileStream(System.IO.Path.Combine(webRoot, path), FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                root.Element("Contacts").Elements("row").ElementAt(RawId).Elements("link").Where(x => x.Attribute("id").Value == LinkId).First().Attribute("image").Value = path;
            }
            if (actionName == "addRow")
            {
                AddContactsRow();
            }
            if (actionName == "deleteRow")
            {
                DeleteContactsRow(id);
            }
            if (actionName == "addLink")
            {
                AddContactsLink(id);
            }
            if (actionName == "deleteLink")
            {
                DeleteContactsLink(id);
            }
            if (actionName == "deleteLinkImg")
            {
                DeleteContactsLinkImage(id);
            }
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            return Contacts();
        }
        private void AddContactsRow()
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            XElement newNode = new XElement("row", "");
            root.Element("Contacts").Add(newNode);
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
        }
        private void DeleteContactsRow(string id)
        {
            int _id = int.Parse(id);
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int elemZeroCheck = root.Element("Contacts").Elements("row").Count();
            if (elemZeroCheck > 1)
            {
                //Удаление строки из XML
                root.Element("Contacts").Elements("row").ElementAt(_id).Remove();
                root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));

                //объявление путей до обложек инфоблоков для удаления в методе InfoBlockImageCleaner
                string ButtImgPath = System.IO.Path.Combine(webRoot, "Images/Contacts/");
                
                ContactsFolderImgCleaner(ButtImgPath);
            }
        }
        private void AddContactsLink(string id)
        {
            int _id = int.Parse(id);
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            int newGalleryID = 0;
            int RowsCount = root.Element("Contacts").Elements("row").Count();
            int[] rowsMaxIds = new int[RowsCount];
            int[] existGalleryIDs;
            int RowCounter = 0;
            foreach (XElement rawElem in root.Element("Contacts").Elements("row"))
            {
                int galleryCount = rawElem.Elements("link").Count();
                if (galleryCount != 0)
                {
                    existGalleryIDs = new int[galleryCount];


                    //Просмотр id существующих элементов в XML и запись их в массив
                    int counter = 0;
                    foreach (XElement elem in rawElem.Elements("link"))
                    {
                        existGalleryIDs[counter] = int.Parse(elem.Attribute("id").Value);
                        counter++;
                    }
                    var tt = existGalleryIDs.Max();
                    rowsMaxIds[RowCounter] = existGalleryIDs.Max();
                }
                RowCounter++;
            }
            //поиск в массиве максимального id и увеличение его на единицу
            newGalleryID = rowsMaxIds.Max() + 1;
            //сразу записываем путь к файлам 
            string Dirpath = "Images/Contacts/";
            //создаём новый элемент галереи и добавляем в xml
            XElement newNode = new XElement("link", new XAttribute("id", newGalleryID), new XAttribute("image", Dirpath), new XAttribute("link", ""));
            newNode.Value = " ";
            root.Element("Contacts").Elements("row").ElementAt(_id).Add(newNode);
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
        }
        private void DeleteContactsLink(string id)
        {
            int RawId = 0;
            string LinkId = "";
            if (id != null)
            {
                string[] IDstringArray = id.Split('?');
                RawId = int.Parse(IDstringArray[0]);
                LinkId = IDstringArray[1];
            }
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            root.Element("Contacts").Elements("row").ElementAt(RawId).Elements("link").Where(x => x.Attribute("id").Value == LinkId).First().Remove();
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));

            //объявление путей до обложек инфоблоков для удаления в методе InfoBlockImageCleaner
            string ButtImgPath = System.IO.Path.Combine(webRoot, "Images/Contacts/");
            ContactsFolderImgCleaner(ButtImgPath);
        }
       
        private void DeleteContactsLinkImage(string id)
        {
            int RawId = 0;
            string LinkId = "";
            if (id != null)
            {
                string[] IDstringArray = id.Split('?');
                RawId = int.Parse(IDstringArray[0]);
                LinkId = IDstringArray[1];
            }
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            root.Element("Contacts").Elements("row").ElementAt(RawId).Elements("link").Where(x => x.Attribute("id").Value == LinkId).First().Attribute("image").Value="Images/Contacts/";
            root.Save(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));

            //объявление путей до обложек инфоблоков для удаления в методе InfoBlockImageCleaner
            string ButtImgPath = System.IO.Path.Combine(webRoot, "Images/Contacts/");
            ContactsFolderImgCleaner(ButtImgPath);
        }
        private void ContactsFolderImgCleaner(string FullDirPath )
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            IEnumerable<XElement> XmlElements = root.Element("Contacts").Elements("row");

            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(FullDirPath);
            //Основной цикл, просматриваем каждый файл в заданной папке
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                //флаг удаления файла
                bool CanDelete = false;

                //прверяем файл обложки до цикла, если совподает название, пропускаем цикл и ничего не удаляем
                string XmlMainImgPath = root.Element("Contacts").Element("image").Value;
                string XmlMainImgName = XmlMainImgPath.Substring(XmlMainImgPath.LastIndexOf('/') + 1);
                if (file.Name != XmlMainImgName)
                {
                    //Сравниваем соответствующий элемент каждого инфоблока с названием файла, 
                    //если такое название фала есть - завершаем цикл с установкой значения флага удаления false
                    //если нет устанавливаем значение флага true
                    int LinksCount = XmlElements.Count();
                    for (int i = 0; i < LinksCount; i++)
                    {

                        foreach (XElement LinkElem in XmlElements.ElementAt(i).Elements("link"))
                        {
                            string FromXmlFilePath = LinkElem.Attribute("image").Value;
                            string XmlFileName = FromXmlFilePath.Substring(FromXmlFilePath.LastIndexOf('/') + 1);
                            if (file.Name == XmlFileName)
                            {
                                CanDelete = false;
                                i = LinksCount;
                                break;
                            }
                            else
                            {
                                CanDelete = true;
                            }
                        }
                    }
                }
                else
                {
                   
                }
              
                //удаляем файл и его тумбнейл
                if (CanDelete)
                {
                    string FullThumbPath = FullDirPath + "thumb/";

                    FileInfo FileThumb = new FileInfo(FullThumbPath + file.Name);
                    if (FileThumb.Exists)
                    {
                        FileThumb.Delete();
                    }
                        file.Delete();
                }

            }
        }

        private void SiteMapGenerator()
        {
            var webRoot = _env.WebRootPath;
            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));
            IEnumerable<XElement> ProjectsPages = root.Element("Projects").Elements("gallery");
            IEnumerable<XElement> PortfolioPages = root.Element("Portfolio").Elements("gallery");
            XNamespace xmlNS = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement rootSM = new XElement(xmlNS+"urlset");
            string mainPageURL = "https://kondratovaphoto.ru/";
            foreach(XElement el in root.Element("menu").Elements())
            {
                string ElLink = el.Attribute("link").Value;
                XElement MainPage = new XElement(xmlNS + "url", new XElement(xmlNS + "loc", (mainPageURL + ElLink)));
                
                rootSM.Add(MainPage);
            }
            foreach (XElement el in ProjectsPages)
            {
                string ElLink = el.Attribute("url").Value;
                XElement ProjectsPage = new XElement(xmlNS + "url", new XElement(xmlNS + "loc", (mainPageURL+ "Projects/?id=" + ElLink)));
                rootSM.Add(ProjectsPage);
            }
            foreach (XElement el in PortfolioPages)
            {
                string ElLink = el.Attribute("url").Value;
                XElement PortfolioPage = new XElement(xmlNS + "url", new XElement(xmlNS + "loc", (mainPageURL + "Portfolio/?id=" + ElLink)));
                rootSM.Add(PortfolioPage);
            }
            XDocument NewSiteMap = new XDocument(rootSM);
            NewSiteMap.Save(System.IO.Path.Combine(webRoot, "Sitemap.xml"));

        }
        private void NaviButtons()
        {
            var webRoot = _env.WebRootPath;

            root = XElement.Load(System.IO.Path.Combine(webRoot, "xml/kondratovaphoto.xml"));


            List<AdminMenuStruct> SiteStruct = new List<AdminMenuStruct>();
            SiteStruct =
                (from node in root.Elements()
                 select new AdminMenuStruct
                 {
                     MenuLink = node.Name.ToString(),
                     MenuName = node.Attribute("name").Value
                 }).ToList();
            ViewData["AdminSiteStruct"] = SiteStruct;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("LoginPage");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Admin admin)
        {
            //if (admin.Login == "q" && admin.Passw == "qaz")
            if (admin.Login == "admin" && admin.Passw == "admin")
            {
                await Authenticate(admin.Login);
                return RedirectToAction("Index");
            }
            return View("LoginPage");

        }
        private async Task Authenticate(string UserName)
        {
            //создаём один Claim
            var claims = new List<Claim>
            { new Claim(ClaimsIdentity.DefaultNameClaimType,UserName)};
            //создаём ClaimIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            //установка куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
       public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
