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
                Config.instance().connectionSettings
            );
        }

        private bool _lst = true;

        public List<App> GetApps()
        {
            if (_lst)
            {
                try
                {
                    return _api.ListsUserApplications(_apiKey.Key).ToList();
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                    _lst = false;
                    return null;
                }
            }

            return null;
        }
    }
}