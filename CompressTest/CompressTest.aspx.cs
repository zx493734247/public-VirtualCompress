using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VirtualCompress;
using VirtualCompress.Entity;
using System.IO;

namespace CompressTest
{
    public partial class CompressTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            E_CompressConfig VCConfig = new E_CompressConfig();
            VCConfig.CRCLevel = Const.EnumCRCLevel.RealTime;
            VCompress Vcom = new VCompress(VCConfig);
            List<E_SourceFile> fileList = new List<E_SourceFile>();
            string dirPath = HttpContext.Current.Server.MapPath("/CompressFile/");
            var dir = Directory.GetFiles(dirPath);
            foreach (var file in dir)
            {
                FileInfo fi = new FileInfo(file);
                var source = new E_SourceFile();
                source.FileName = fi.Name;
                source.PhyFilePath = fi.FullName;
                source.FileSize = fi.Length;
                fileList.Add(source);
            }
            Vcom.VirDownload(fileList);
            HttpContext.Current.Response.End();
        }
    }
}