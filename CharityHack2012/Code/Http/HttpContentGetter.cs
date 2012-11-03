using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace CharityHack2012.Code.Http
{
    public class CachingHttpContentGetter : IHttpContentGetter
    {
        private readonly IHttpContentGetter _inner;
        private const string CacheDirectory = "App_Data";

        public CachingHttpContentGetter(IHttpContentGetter inner)
        {
            _inner = inner;
        }

        public string Get(string uri)
        {
            var cacheKey = CreateCacheKey(uri);

            var cacheCheck = ExistsInCache(cacheKey);
            if(cacheCheck.Item1)
            {
                return cacheCheck.Item2;
            }

            var value = _inner.Get(uri);
            AddToCache(cacheKey, value);
            return value;
        }

        private string CreateCacheKey(string uri)
        {
            return uri.Replace("\\", "-").Replace("/", "").Replace("?", "").Replace("&", "").Replace(":", "") + ".html";
        }

        public Tuple<bool, string> ExistsInCache(string cacheKey)
        {
            var fileloc = PathExtensions.GetAppInstallDirectory() + CacheDirectory + "\\" + cacheKey;
            return File.Exists(fileloc)
                       ? new Tuple<bool, string>(true, File.ReadAllText(fileloc))
                       : new Tuple<bool, string>(false, null);
        }

        public void AddToCache(string cacheKey, string value)
        {
            var fileloc = PathExtensions.GetAppInstallDirectory() +  CacheDirectory + "\\" + cacheKey;
            File.WriteAllText(fileloc, value);
        }
    }

    public class HttpContentGetter : IHttpContentGetter
    {
        public string Get(string uri)
        {
            var client = new HttpClient();

            var task = client.GetAsync(uri);
            task.Wait();
            var result = task.Result;
            var readBody = result.Content.ReadAsStringAsync();
            readBody.Wait();

            return readBody.Result;
        }
    }

    public class PathExtensions
    {
        public static string GetAppInstallDirectory()
        {
            var currentApplication = Assembly.GetAssembly(typeof(PathExtensions));
            var thisClassesPhysicalLocation = currentApplication.CodeBase;
            var thisClassesPyhsicalPath = thisClassesPhysicalLocation.Replace(currentApplication.GetName().Name + ".DLL", "");
            thisClassesPyhsicalPath =
                thisClassesPyhsicalPath.Replace("file:///", "")
                                       .Replace("/bin/Debug", "")
                                       .Replace("/bin/Release", "")
                                       .Replace("/bin", "");
            return thisClassesPyhsicalPath;
        }

        public static string ToAbsolutePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            if (!path.Contains("~/") && !path.Contains("~\\"))
            {
                return path;
            }

            var thisClassesPyhsicalPath = GetAppInstallDirectory();
            return path.Replace("~/", thisClassesPyhsicalPath).Replace("~\\", thisClassesPyhsicalPath);
        }
    }
}