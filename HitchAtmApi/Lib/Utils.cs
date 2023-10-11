using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using HitchSapB1Lib;

namespace HitchAtmApi.Lib
{
    public class Utils
    {
        public static string ATTACHMENTS_FOLDER;
        public static Credentials Credentials { get; set; }
        public static string SalesforceInstanceUrl { get; set; }
        public static string SalesforceApiVersion { get; set; }

        public static string SavePDFAttachment(string filename, string base64Pdf)
        {
            Byte[] pdfBytes = Convert.FromBase64String(base64Pdf);
            string fileLocation = Path.Combine(ATTACHMENTS_FOLDER, $"{filename}");

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

        public static string TranslateTaxCode(string Code)
        {
            if (Code == "IVA")
                return "IVA";
            else if (Code == "IVA - EXE")
                return "IVA_EXE";
            else
                return "IVA";
        }

        public static string TranslateCurrency(string Code)
        {
            if (Code == "CLP")
                return "$";
            else
                return Code;
        }

        public static string TranslateCurrencyCode(string Code)
        {
            if (Code == "$")
                return "CLP";
            else if (Code == "UAH")
            {
                return "UF";
            }
            else if (Code == "CLF")
            {
                return "UF";
            }
            else
                return Code;
        }

        public static string RemoveAccent(string txt)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        public static string Slug(string phrase)
        {
            string str = RemoveAccent(phrase).ToLower();
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim();
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-");
            return str;
        }

        public static DateTime? ParseDateTime(string input)
        {
            DateTime temp;
            if (DateTime.TryParse(input, out temp))
            {
                return temp;
            }

            return null;
        }

        public static void CleanAddresses(string CardCode, Company Company)
        {
            CleanAddressesOperation cleanAddresses = new CleanAddressesOperation();
            cleanAddresses.Company = Company;
            cleanAddresses.CardCode = CardCode;
            cleanAddresses.Start();
        }
    }
}
