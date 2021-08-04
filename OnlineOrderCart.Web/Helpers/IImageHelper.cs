using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public interface IImageHelper
    {
        Task<byte[]> UploadImageArrayAsync(IFormFile imageFile);
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
        string UploadImage(byte[] pictureArray, string folder);
    }
}
