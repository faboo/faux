using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Windows;
using System.IO;

namespace Project {
    public static class IconCache {
        private static Regex isLocalRegex = new Regex(@".:\\|pack://");
        private static Regex toLocalRegex = new Regex("[\\/:?\"|]");
        private static Dictionary<string, string> knownCache = new Dictionary<string, string>();
        private static object cacheLock = new object();

        private static bool IsLocal(string url) {
            return isLocalRegex.IsMatch(url);
        }

        private static string getIconCachePath() {
            string userData = Application.Current.GetUserAppDataPath();
            string iconCache = Path.Combine(userData, "icon-cache");

            if(!Directory.Exists(userData))
                Directory.CreateDirectory(userData);
            if(!Directory.Exists(iconCache))
                Directory.CreateDirectory(iconCache);

            return iconCache;
        }

        private static string createLocalName(string url) {
            string name = toLocalRegex.Replace(url, "^");
            
            return Path.Combine(getIconCachePath(), name);
        }

        private static bool isCached(string url) {
            bool cached = knownCache.ContainsKey(url);

            if(!cached){
                string localName = createLocalName(url);

                if(System.IO.File.Exists(localName)) {
                    knownCache[url] = localName;
                    cached = true;
                }
            }

            return cached;
        }

        private static string Add(string url) {
            string localName;
            bool onDisk;

            lock(cacheLock) {
                localName = createLocalName(url);

                if(!(onDisk = isCached(url))) {
                    knownCache[url] = localName;
                }
            }

            if(!onDisk){
                WebClient web = new WebClient();
                web.DownloadFile(url, localName);
            }

            return localName;
        }

        public static string Get(string url) {
            string local = url;

            if(!IsLocal(url)) {
                local = Add(url);
            }

            return local;
        }
    }
}
