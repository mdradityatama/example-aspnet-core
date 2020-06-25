using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExampleUploadFile.WebUI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExampleUploadFile.WebUI.Pages
{
    public class BufferedSingleFileUploadPhysicalModel : PageModel
    {
        private readonly ILogger<BufferedSingleFileUploadPhysicalModel> _logger;
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".txt", ".png" };
        private readonly string _targetFilePath;

        public BufferedSingleFileUploadPhysicalModel(IConfiguration config, ILogger<BufferedSingleFileUploadPhysicalModel> logger)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");

            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");

            // To save physical files to the temporary files folder, use:
            //_targetFilePath = Path.GetTempPath();

            _logger = logger;
        }

        [BindProperty]
        public BufferedSingleFileUploadPhysical FileUpload { get; set; }

        public string Result { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";

                return Page();
            }

            var formFileContent =
                await FileHelpers.ProcessFormFile<BufferedSingleFileUploadPhysical>(
                    FileUpload.FormFile, ModelState, _permittedExtensions,
                    _fileSizeLimit);

            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";

                return Page();
            }

            // For the file name of the uploaded file stored
            // server-side, use Path.GetRandomFileName to generate a safe
            // random file name.
            //var trustedFileNameForFileStorage = Path.GetRandomFileName();
            //var filePath = Path.Combine(
            //    _targetFilePath, trustedFileNameForFileStorage);
            //var filePath = Path.Combine(
            //    "c:\\files", trustedFileNameForFileStorage);

            // **WARNING!**
            // In the following example, the file is saved without
            // scanning the file's contents. In most production
            // scenarios, an anti-virus/anti-malware scanner API
            // is used on the file before making the file available
            // for download or for use by other systems. 
            // For more information, see the topic that accompanies 
            // this sample.

            var trustedFileNameForFileStorage = $"{FileUpload.FormFile.FileName}";
            var filePath = Path.Combine("c:\\files", trustedFileNameForFileStorage);


            using (var fileStream = System.IO.File.Create(filePath))
            {
                //await fileStream.WriteAsync(formFileContent);

                // To work directly with a FormFile, use the following
                // instead:
                await FileUpload.FormFile.CopyToAsync(fileStream);
            }

            return RedirectToPage("./Index");
        }
    }

    public class BufferedSingleFileUploadPhysical
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }

        [Display(Name = "Note")]
        [StringLength(50, MinimumLength = 0)]
        public string Note { get; set; }
    }
}