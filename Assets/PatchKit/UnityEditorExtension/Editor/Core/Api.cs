using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PatchKit.Api;
using PatchKit.Api.Models.Main;
using PatchKit.UnityEditorExtension.Connection;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.Core
{
public static class Api
{
    private class CachedApps
    {
        public CachedApps(ApiKey apiKey)
        {
            ApiKey = apiKey;
        }

        [NotNull]
        public readonly List<App> List = new List<App>();

        public readonly ApiKey ApiKey;
    }

    [NotNull]
    private static MainApiConnection ApiConnection
    {
        get
        {
            return new MainApiConnection(
                Config.FindOrCreateInstance().ApiConnectionSettings)
            {
                HttpClient = new UnityHttpClient()
            };
        }
    }

    private static ApiKey GetApiKey()
    {
        ApiKey? savedApiKey = Config.FindOrCreateInstance().GetSavedApiKey();

        Assert.IsTrue(savedApiKey.HasValue);

        return savedApiKey.Value;
    }

    private static CachedApps _cachedApps;

    [NotNull]
    public static App[] GetApps()
    {
        ApiKey apiKey = GetApiKey();

        App[] result = ApiConnection.ListsUserApplications(apiKey.Value);
        Assert.IsNotNull(result);

        _cachedApps = new CachedApps(apiKey);
        _cachedApps.List.AddRange(result);

        return result;
    }

    [NotNull]
    public static App[] GetAppsCached()
    {
        ApiKey apiKey = GetApiKey();

        if (_cachedApps != null && _cachedApps.ApiKey.Equals(apiKey))
        {
            return _cachedApps.List.ToArray();
        }

        return GetApps();
    }

    public static App GetAppInfo(AppSecret secret)
    {
        if (!secret.IsValid)
        {
            throw new InvalidArgumentException("secret");
        }

        ApiKey apiKey = GetApiKey();

        App app = ApiConnection.GetApplicationInfo(secret.Value);

        if (_cachedApps == null || !_cachedApps.ApiKey.Equals(apiKey))
        {
            _cachedApps = new CachedApps(apiKey);
        }

        _cachedApps.List.RemoveAll(x => x.Secret == app.Secret);
        _cachedApps.List.Add(app);

        return app;
    }

    public static App GetAppInfoCached(AppSecret secret)
    {
        if (!secret.IsValid)
        {
            throw new InvalidArgumentException("secret");
        }

        ApiKey apiKey = GetApiKey();

        if (_cachedApps != null && _cachedApps.ApiKey.Equals(apiKey))
        {
            int i = _cachedApps.List.FindIndex(a => a.Secret == secret.Value);

            if (i != -1)
            {
                return _cachedApps.List[i];
            }
        }

        return GetAppInfo(secret);
    }

    public static App CreateNewApp([NotNull] string name, AppPlatform platform)
    {
        if (name == null)
        {
            throw new ArgumentNullException("name");
        }

        ApiKey apiKey = GetApiKey();

        App app = ApiConnection.PostUserApplication(
            apiKey.Value,
            name,
            platform.ToApiString());

        if (_cachedApps == null || !_cachedApps.ApiKey.Equals(apiKey))
        {
            _cachedApps = new CachedApps(apiKey);
        }

        _cachedApps.List.RemoveAll(x => x.Secret == app.Secret);
        _cachedApps.List.Add(app);

        return app;
    }
}
}