using FirstInFirstAid.Helpers;
using FirstInFirstAid.Models.Helpers;
using log4net;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace FirstInFirstAid.Controllers
{
    [Authorize]
    public class BackupController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private BackupManager backupHelper;

        //System's database backup folder. Usually resides in SQL Server installation directory
        private string backupFolder;

        public BackupController()
        {
            backupHelper = new BackupManager();
            backupFolder = backupHelper.GetBackupFolder();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var dir = new DirectoryInfo(backupFolder);
            FileInfo[] files = dir.GetFiles("*.bak");
            List<BackupFile> items = new List<BackupFile>();
            foreach (var file in files)
            {
                BackupFile backupFile = new BackupFile
                {
                    FileName = file.Name,
                    CreatedTime = file.CreationTime                   
                };

                items.Add(backupFile);
            }
            ViewBag.Message = TempData["Message"]?.ToString();
            return View(items);
        }

        [HttpPost]
        public ActionResult CreateBackup()
        {
            backupHelper.CreateBackup();
            return RedirectToAction("Index");
        }

    
        public ActionResult Download(string backupName)
        {
           
            string filepath = backupFolder + "\\" + backupName;
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = backupName,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }

        [HttpPost]
        public ActionResult Restore(HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {                
                string uploadsDirPath = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(uploadsDirPath))
                {
                    Directory.CreateDirectory(uploadsDirPath);
                }
                string filePath = uploadsDirPath + Path.GetFileName(postedFile.FileName);
                postedFile.SaveAs(filePath);
                backupHelper.Restore(filePath);
                backupHelper.CleanUploadsDir(uploadsDirPath);
                ViewBag.Message = "File uploaded successfully.";
            }
            TempData["Message"] = "Database restored successfully. Please restart the system.";
            return RedirectToAction("Index");
        }
    }
}