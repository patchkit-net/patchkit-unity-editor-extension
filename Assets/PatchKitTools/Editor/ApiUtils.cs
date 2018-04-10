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
            _apiKey = apiKey;
            _api = new Api.MainApiConnection(
                Config.Instance().ConnectionSettings
            );
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

        public App GetAppInfo(string secret)
        {
            return _api.GetApplicationInfo(secret);
        }

        public App CreateNewApp(string name, string platform)
        {
            return _api.PostUserApplication(Key, name, platform);
        }
    }
}