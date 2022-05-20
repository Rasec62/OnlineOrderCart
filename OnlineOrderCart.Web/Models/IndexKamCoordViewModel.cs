using OnlineOrderCart.Common.Utilities;

namespace OnlineOrderCart.Web.Models
{
    public class IndexKamCoordViewModel
    {
        public long Id { get; set; }
        public long KcId { get; set; }
        public string NoEmployee { get; set; }
        public string Email { get; set; }
        public string KickName { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public int IsCoordinator { get; set; }
        public string Path { get; set; }
        public bool IsKam { get; set; }
        public string PictureFullPath
        {
            get
            {
                if (Path == null)
                {
                    return $":{CT.UrlBaseApi}{"/images/noimage.png"}";
                }

                //return string.Format(
                //    "http://shoppingcartsystems.ddns.net:8087{0}",
                //    Path.Substring(1));
                return $"{CT.UrlBaseApi}{Path.Substring(1)}";
            }
        }
    }
}
