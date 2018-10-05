
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class PdfContent : ActionResult
    {
        public MemoryStream MemoryStream { get; set; }
        public string FileName { get; set; }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var response = context.HttpContext.Response;
            response.ContentType = "pdf/application";
            response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
            response.OutputStream.Write(MemoryStream.GetBuffer(), 0, MemoryStream.GetBuffer().Length);
            //string filename = "Facture-" + numFacture + "-" + DateTime.Now.Month + ".pdf";
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Pdf/" + FileName);
            System.IO.File.WriteAllBytes(filePath, MemoryStream.GetBuffer());
        }
    }
}