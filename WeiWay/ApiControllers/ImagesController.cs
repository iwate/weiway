using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web.Http.Description;

namespace WeiWay.ApiControllers
{
    public class ImagesController : ApiController
    {
        private byte[] RotateImage(FileStream fs)
        {
            var bmp = new Bitmap(fs);
            var props = bmp.PropertyItems;

            for (int i = 0; i < props.Length; i++)
            {
                var p = props[i];

                if (p.Id == 0x0112)
                {
                    switch (p.Value[0])
                    {
                        case 1:

                            break;
                        case 3:
                            bmp.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
                            break;

                        case 6:
                            bmp.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
                            break;
                        case 8:
                            bmp.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                            break;
                    }
                }
            }
            var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.GetBuffer();
        }
        [HttpPost]
        [ResponseType(typeof(Image))]
        public async Task<HttpResponseMessage> Post()
        {

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartFileStreamProvider(HttpContext.Current.Server.MapPath("~/App_Data"));

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                //CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("");
                CloudBlobClient client = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer images = client.GetContainerReference("images");

                images.CreateIfNotExists(BlobContainerPublicAccessType.Container);

                var files = provider.FileData.Where(f => f.Headers.ContentType != null && f.Headers.ContentType.MediaType.Contains("image")).Select(f => f);

                if (files.Count() == 0)
                {
                    Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Uploaded file is not image.");
                }

                var file = files.First();
                var filepath = file.Headers.ContentDisposition.FileName.Replace("\"", "");
                string uniqueBlobName = string.Format("image_{0}{1}", Guid.NewGuid().ToString(), ".jpg"/*Path.GetExtension(filepath)*/);
                CloudBlockBlob blob = images.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = file.Headers.ContentType.ToString();
                using (var filestream = File.OpenRead(file.LocalFileName))
                {
                    byte[] data = RotateImage(filestream);
                    await blob.UploadFromByteArrayAsync(data, 0 ,data.Length);
                }
                File.Delete(file.LocalFileName);

                return Request.CreateResponse(HttpStatusCode.OK, new WeiWay.Models.Image {
                    Path = blob.Uri.ToString()
                });
                //return Request.CreateErrorResponse(HttpStatusCode.OK, "https://webrolesample.blob.core.windows.net/images/" + uniqueBlobName);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}