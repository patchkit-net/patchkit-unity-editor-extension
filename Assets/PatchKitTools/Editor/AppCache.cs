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
    [Serializable]
    public struct CacheEntry
    {
        public CacheEntry(BuildTarget target, string secret)
        {
            this.Target = target;
            this.Secret = secret;
        }

        [SerializeField]
        public BuildTarget Target;

        [SerializeField]
        public string Secret;

    }

    [Serializable]
    public class AppCache
    {
        public List<CacheEntry> _appsList = new List<CacheEntry>();

        public void UpdateEntry(BuildTarget target, App app)
        {
            _appsList.Add(new CacheEntry(target, app.Secret));
        }

        public void RemoveEntry(App app)
        {
            _appsList = _appsList
                .Where(entry => entry.Secret != app.Secret)
                .ToList();
        }

        public void RemoveEntry(BuildTarget target, App app)
        {
            _appsList = _appsList
                .Where(entry => entry.Target != target && entry.Secret != app.Secret)
                .ToList();
        }

        public void RemoveEntry(BuildTarget target)
        {
            _appsList = _appsList
                .Where(entry => entry.Target != target)
                .ToList();
        }

        public string AppByPlatform(BuildTarget target)
        {
            if (_appsList.Any(e => e.Target == target))
            {
                return _appsList.Find(e => e.Target == target).Secret;
            }

            return null;
        }

        public Dictionary<BuildTarget, string> AppsByPlatform()
        {
            return _appsList
                .ToDictionary(e => e.Target, e => e.Secret);
        }

        public List<string> Apps()
        {
            return _appsList
                .Select(e => e.Secret)
                .ToList();
        }

        public void Clear()
        {
            _appsList.Clear();
        }
    }
}