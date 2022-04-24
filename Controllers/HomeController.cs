using Amazon.Textract;
using Amazon.Textract.Model;
using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAWSTextract.Models;

namespace WebAWSTextract.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> ExtractFileDataAsync(string filename)
        {
            JResponse resp = new JResponse() { success=0,message="N/A" };
            if (string.IsNullOrEmpty(filename))
            {
                resp.message = "No file found";
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            var path = Server.MapPath("\\Content\\TempFile\\") + filename;
            if (!System.IO.File.Exists(path))
            {
                resp.message = "No file found";
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            var dd =await DetectSampleAsync(path);
            resp.data = dd; resp.success = 1;
            return Json(resp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public void UploadFile()
        {
            string orgFileName = string.Empty;
            string newFileName = string.Empty;
            //string newFileNameGuid = string.Empty;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; 

                if (file.ContentLength > 0 || file != null)
                {
                    orgFileName = file.FileName;
                    newFileName = string.Format("{0}{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(file.FileName));
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    if (!System.IO.Directory.Exists(Server.MapPath("~/Content/TempFile")))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/TempFile"));
                    }
                    var path = System.IO.Path.Combine(Server.MapPath("~/Content/TempFile"), newFileName);
                    file.SaveAs(path);
                }

            }
            //var files = Json(new { file = orgFileName, uploaded = newFileName});
            Response.Write(newFileName);
            Response.End();

        }
        private static async Task<List<JKeyValue>> DetectSampleAsync( string filename)
        {
            List<JKeyValue> retText = new List<JKeyValue>();
       
            using (var textractClient = new AmazonTextractClient("<Your Key>", "<Your key secret>", Amazon.RegionEndpoint.USEast1))
            {
                var bytes = System.IO.File.ReadAllBytes(filename);

              
                var detectResponse = await textractClient.DetectDocumentTextAsync(new DetectDocumentTextRequest
                {
                    Document = new Document
                    {
                        Bytes = new System.IO.MemoryStream(bytes)
                    }
                });

                

                foreach (var block in detectResponse.Blocks)
                {
                 
                    //Console.WriteLine($"Type {block.BlockType}, Text: {block.Text}");
                    JKeyValue jkv = new JKeyValue() { _key = block.BlockType, _value = block.Text };
                    retText.Add(jkv);
                }
            }
            return retText;
        }
    }
}