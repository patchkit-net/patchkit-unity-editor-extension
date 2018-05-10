using System;
using System.Linq;
using System.Collections.Generic;
using PatchKit.Api.Models.Main;

namespace PatchKit.Tools.Integration
{
    public class ApiUtils
    {
        private ApiKey _apiKey;
        private Api.MainApiConnection _api;

        public ApiUtils(ApiKey apiKey)
        {
            if (!apiKey.IsValid())
            {
                throw new ArgumentException("apiKey");
            }
            _apiKey = apiKey;
            _api = new Api.MainApiConnection(
                Config.Instance().ConnectionSettings
            );

            if (_api == null)
            {
                throw new Exception("Cannot connect to API.");
            }
        }

        public string Key
        {
            get 
            {
                return _apiKey.Key;
            }
        }

        public Api.MainApiConnection Api
        {
            get
            {
                return _api;
            }
        }

        public List<App> GetApps()
        {
            try
            {
                return _api.ListsUserApplications(_apiKey.Key).ToList();
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogError(e);
                return null;
            }
        }

        private List<App> _apps = null;

        public List<App> GetAppsCached(bool forceReload = false)
        {
            if (forceReload || _apps == null)
            {
                _apps = GetApps();
            }

            return _apps;
        }

        public App GetAppInfo(string secret, bool canUseCache = true)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException(secret);
            }
            
            if (canUseCache && _apps != null)
            {
                if (_apps.Any(app => app.Secret == secret))
                {
                    return _apps.Find(app => app.Secret == secret);
                }
            }

            var newAppInfo = _api.GetApplicationInfo(secret);
            
            if (_apps != null && !_apps.Contains(newAppInfo))
            {
                _apps.Add(newAppInfo);
            }

            return newAppInfo;
        }

        public App CreateNewApp(string name, string platform)
        {
            return _api.PostUserApplication(Key, name, platform);
        }
    }
}