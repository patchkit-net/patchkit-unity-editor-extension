using System;
using System.IO;
using UnityEngine;

using App = PatchKit.Api.Models.Main.App;

namespace PatchKit.Tools.Integration
{
    public static class AppCache
    {
        public static string CachedAppFilename()
        {
            var dataPath = Application.dataPath;
            const string dataFilename = ".selected_app";

            var selectedAppFilename = Path.Combine(dataPath, dataFilename);

            return selectedAppFilename;
        }

        public static App? LoadCachedApp(ApiUtils api)
        {
            var filepath = CachedAppFilename();

            if (!File.Exists(filepath))
            {
                return null;
            }

            var cachedAppSecret = File.ReadAllText(filepath);

            if (string.IsNullOrEmpty(cachedAppSecret))
            {
                return null;
            }

            var appInfo = api.GetAppInfo(cachedAppSecret);

            return appInfo;
        }

        public static void CacheApp(App app)
        {
            var filepath = CachedAppFilename();
            File.WriteAllText(filepath, app.Secret);
        }
    }
}