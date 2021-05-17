using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CMS.Website.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UploadController : Controller
    {
        public IWebHostEnvironment HostingEnvironment { get; set; }

        public UploadController(IWebHostEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Save(IFormFile file) // must match SaveField which defaults to "files"
        {
            if (file != null)
            {
                try
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    var fileName = Path.GetFileNameWithoutExtension(fileContent.FileName.ToString().Trim('"')) + Path.GetExtension(fileContent.FileName.ToString().Trim('"'));
                    var pathOriginal = Path.Combine(HostingEnvironment.WebRootPath + $"/data/article/upload/", fileName);

                    //save original
                    using (var fileStream = new FileStream(pathOriginal, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                }
                catch (Exception ex)
                {
                    // Implement error handling here, this example merely indicates an upload failure.
                    Response.StatusCode = 500;
                    await Response.WriteAsync($"some error message {ex.ToString()}"); // custom error message
                }
            }
            // Return an empty string message in this case
            return new EmptyResult();
        }
        [HttpPost]
        public ActionResult Remove(string fileToRemove) // must match RemoveField which defaults to "files"
        {
            if (fileToRemove != null)
            {
                try
                {
                    var fileName = Path.GetFileName(fileToRemove);
                    var pathOriginal = Path.Combine(HostingEnvironment.WebRootPath + $"/data/article/upload/", fileName);
                    if (System.IO.File.Exists(pathOriginal))
                    {
                        // Implement security mechanisms here - prevent path traversals,
                        // check for allowed extensions, types, permissions, etc.
                        // this sample always deletes the file from the root and is not sufficient for a real application.

                        System.IO.File.Delete(pathOriginal);
                    }
                }
                catch
                {
                    // Implement error handling here, this example merely indicates an upload failure.
                    Response.StatusCode = 500;
                    Response.WriteAsync("some error message"); // custom error message
                }
            }

            // Return an empty string message in this case
            return new EmptyResult();
        }
        [HttpPost]
        public async Task<IActionResult> SaveArticlePublish(IFormFile file) // must match SaveField which defaults to "files"
        {
            if (file != null)
            {
                try
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    var fileName = Path.GetFileNameWithoutExtension(fileContent.FileName.ToString().Trim('"')) + Path.GetExtension(fileContent.FileName.ToString().Trim('"'));
                    var pathOriginal = Path.Combine(HostingEnvironment.WebRootPath + $"/data/articlepublish/upload/", fileName);

                    //save original
                    using (var fileStream = new FileStream(pathOriginal, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                }
                catch (Exception ex)
                {
                    // Implement error handling here, this example merely indicates an upload failure.
                    Response.StatusCode = 500;
                    await Response.WriteAsync($"some error message {ex.ToString()}"); // custom error message
                }
            }
            // Return an empty string message in this case
            return new EmptyResult();
        }
        [HttpPost]
        public ActionResult SaveArticlePublish(string fileToRemove) // must match RemoveField which defaults to "files"
        {
            if (fileToRemove != null)
            {
                try
                {
                    var fileName = Path.GetFileName(fileToRemove);
                    var pathOriginal = Path.Combine(HostingEnvironment.WebRootPath + $"/data/articlepublish/upload/", fileName);
                    if (System.IO.File.Exists(pathOriginal))
                    {
                        // Implement security mechanisms here - prevent path traversals,
                        // check for allowed extensions, types, permissions, etc.
                        // this sample always deletes the file from the root and is not sufficient for a real application.

                        System.IO.File.Delete(pathOriginal);
                    }
                }
                catch
                {
                    // Implement error handling here, this example merely indicates an upload failure.
                    Response.StatusCode = 500;
                    Response.WriteAsync("some error message"); // custom error message
                }
            }

            // Return an empty string message in this case
            return new EmptyResult();
        }
    }
}
