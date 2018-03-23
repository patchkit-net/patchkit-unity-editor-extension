using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

using App = PatchKit.Api.Models.Main.App;

namespace PatchKit.Tools.Integration
{
    public class AppCache
    {
        private Dictionary<BuildTarget, App> _apps;

        private readonly string _cacheFilePath;

        public AppCache(string cacheFilePath)
        {
            _cacheFilePath = cacheFilePath;
            _apps = Load(CacheFilePath());

            if (_apps == null)
            {
                _apps = new Dictionary<BuildTarget, App>();
            }
        }

        public static Dictionary<BuildTarget, App> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var jsonData = File.ReadAllText(filePath);
            var apps = JsonConvert.DeserializeObject<Dictionary<BuildTarget, App>>(jsonData);

            if (apps == null)
            {
                return null;
            }

            return apps;
        }

        public void Save()
        {
            var jsonData = JsonConvert.SerializeObject(_apps);
            File.WriteAllText(CacheFilePath(), jsonData);
        }

        public string CacheFilePath()
        {
            var dataPath = Application.dataPath;
            string appCacheFilename = _cacheFilePath;

            var appCacheFilePath = Path.Combine(dataPath, appCacheFilename);

            return appCacheFilePath;
        }

        public void UpdateEntry(BuildTarget target, App app)
        {
            _apps.Add(target, app);
            
            Save();
        }

        public void RemoveEntry(App app)
        {
            _apps = _apps
                .Where(entry => entry.Value.Secret != app.Secret)
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            Save();
        }

        public void RemoveEntry(BuildTarget target, App app)
        {
            _apps = _apps
                .Where(entry => entry.Key != target && entry.Value.Secret != app.Secret)
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            Save();
        }

        public void RemoveEntry(BuildTarget target)
        {
            _apps.Remove(target);
            Save();
        }

        public App? AppByPlatform(BuildTarget target)
        {
            if (!_apps.ContainsKey(target))
            {
                return null;
            }

            return _apps[target];
        }

        public Dictionary<BuildTarget, App> AppsByPlatform()
        {
            return _apps;
        }

        public List<App> Apps()
        {
            return _apps.Values.ToList();
        }
    }
}