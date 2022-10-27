using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Extensions
{
    public static class FileManager
    {
        public static bool CheckContentType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }

        public static bool CheckLength(this IFormFile file, double length)
        {
            return (file.Length / 1024) > length;
        }

        public async static Task<string> SaveFileAsync(this IFormFile File,string root)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid() + "_" + File.FileName;

            string filepath = Path.Combine(root, fileName);

            using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
            {
                await File.CopyToAsync(fileStream);
            }

            return fileName;
        }
    }
}
