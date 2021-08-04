using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public async Task<byte[]> UploadImageArrayAsync(IFormFile imageFile)
        {
            MemoryStream ms = new MemoryStream();
            await imageFile.OpenReadStream().CopyToAsync(ms);
            byte[] bytes = ms.ToArray();
            return bytes;
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder)
        {
            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.png";
            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"wwwroot\\image\\{folder}",
                file);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"~/image/{folder}/{file}";
        }

        public string UploadImage(byte[] pictureArray, string folder)
        {
            MemoryStream stream = new MemoryStream(pictureArray);
            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.png";

            try
            {
                stream.Position = 0;
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{folder}", file);
                File.WriteAllBytes(path, stream.ToArray());
            }
            catch
            {
                return string.Empty;
            }

            return $"~/images/{folder}/{file}";
        }
    }
}
