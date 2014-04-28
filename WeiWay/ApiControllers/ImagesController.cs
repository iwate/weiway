﻿using System;
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

namespace WeiWay.ApiControllers
{
    public class ImagesController : ApiController
    {
        [HttpPost]
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
                CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);
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
                string uniqueBlobName = string.Format("image_{0}{1}", Guid.NewGuid().ToString(), Path.GetExtension(filepath));
                CloudBlockBlob blob = images.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = file.Headers.ContentType.ToString();
                using (var filestream = File.OpenRead(file.LocalFileName))
                {
                    await blob.UploadFromStreamAsync(filestream);
                }
                File.Delete(file.LocalFileName);

                return Request.CreateResponse(HttpStatusCode.OK, blob.Uri.ToString());

            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}