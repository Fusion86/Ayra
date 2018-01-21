using System.Net;

namespace Ayra.Core.Helpers
{
    public static class Utility
    {
        /// <summary>
        /// Get human-readable filesize string
        /// </summary>
        /// <returns></returns>
        /// <param name="size"></param>
        public static string GetSizeString(long size)
        {
            // https://stackoverflow.com/a/11124118/2125072
            long absolute_i = (size < 0 ? -size : size);
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (size >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (size >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (size >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (size >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (size >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = size;
            }
            else
            {
                return size.ToString("0 B"); // Byte
            }
            readable = (readable / 1024);
            return readable.ToString("0.### ") + suffix;
        }

        /// <summary>
        /// Get response status code from HEAD request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpStatusCode GetUrlResponseStatusCode(string url)
        {
            HttpStatusCode result = default;

            WebRequest request = WebRequest.Create(url);
            request.Method = "HEAD";

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response != null)
                {
                    result = response.StatusCode;
                    response.Close();
                }
            }

            return result;
        }
    }
}
