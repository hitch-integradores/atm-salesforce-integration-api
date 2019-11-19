using System;
using System.IO;

namespace HitchAtmApi.Lib
{
    public class Utils
    {
        public static string ATTACHMENTS_FOLDER;

        public static string SavePDFAttachment(string filename, string base64Pdf)
        {
            Byte[] pdfBytes = Convert.FromBase64String(base64Pdf);
            string fileLocation = Path.Combine(ATTACHMENTS_FOLDER, $"{filename}.pdf");

            if (Directory.Exists(ATTACHMENTS_FOLDER) == false)
            {
                Directory.CreateDirectory(ATTACHMENTS_FOLDER);
            }

            using (MemoryStream memoryStream = new MemoryStream(pdfBytes))
            {
                using (FileStream pdfFile = new FileStream(fileLocation, FileMode.Create, FileAccess.Write))
                {
                    memoryStream.WriteTo(pdfFile);
                }
            }

            return fileLocation;
        }
    }
}
