using System.IO;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Services.Controllers
{
    [RoutePrefix("api/files")]
    [AllowAnonymous]
    [EnableCors("*", "*", "*")]
    public class FilesController : ApiController
    {
        private readonly string username = "User1";
        private readonly string password = "user1";

        [Route("download")]
        [HttpPost]
        public IHttpActionResult DownloadFile(MyFile fileName)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            WebClient request = new WebClient();
            var pathToSaveNewFile = @"C:\Repos\Downloads\";
            request.Credentials = new NetworkCredential(this.username, this.password);
            byte[] fileData = request.DownloadData("ftp://127.0.0.1" + "/" + fileName.FileName);

            FileStream file = File.Create(pathToSaveNewFile + fileName.FileName);
            file.Write(fileData, 0, fileData.Length);
            file.Close();

            return this.Ok();
        }

        [Route("upload")]
        [HttpPost]
        public IHttpActionResult UploadFile()
        {
            //if (!this.ModelState.IsValid)
            //{
            //    return this.BadRequest();
            //}

            var fileText = "hello there";
            FileInfo fileToUpload = new FileInfo(fileText);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("" + fileToUpload.Name);

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(this.username, this.password);
            Stream ftpStream = request.GetRequestStream();
            FileStream file = File.OpenRead(fileText);

            int length = 1024;
            byte[] buffer = new byte[length];
            int bytesRead = 0;

            do
            {
                bytesRead = file.Read(buffer, 0 , length);
                ftpStream.Write(buffer, 0, length);
            } while (bytesRead != 0);

            file.Close();
            ftpStream.Close();
            return this.Ok();
        }
    }

    public class MyFile
    {
        public string FileName { get; set; }
    }
}