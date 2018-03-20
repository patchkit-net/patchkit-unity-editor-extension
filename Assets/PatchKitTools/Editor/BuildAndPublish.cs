using UnityEditor;
using UnityEngine;
using PatchKit.Api;
using PatchKit.Network;
using System.Linq;
using System.IO;

namespace PatchKit.Tools.Integration
{
    public class BuildAndPublish : EditorWindow
    {        
        private ApiKey _apiKey;
        private ToolsWrapper _tools = new ToolsWrapper();

        private System.Action _currentGuiScreen;

        private SubmitKeyMenu _submitKey;
        private SubmitLabelAndChangelog _submitVersionDetails;

        private Api.Models.Main.App[] _appList = null;
        private Api.Models.Main.App? _selectedApp = null;

        private bool _hasAppBeenBuilt = false;

        [MenuItem("File/Build and Publish")]
        public static void ShowWindow()
        {
            var target = EditorWindow.GetWindow(typeof(BuildAndPublish), false, "Build & Publish") as BuildAndPublish;

            target.Initialize();
        }

        private void Initialize()
        {
            _apiKey = ApiKey.LoadCached();

            if (_apiKey == null)
            {
                OpenSubmitKeyDialog();
            }
        }

        private string CachedAppFilename()
        {
            var dataPath = Application.dataPath;
            const string dataFilename = ".selected_app";

            var selectedAppFilename = Path.Combine(dataPath, dataFilename);

            return selectedAppFilename;
        }

        private bool LoadCachedSelectedApp()
        {
            return true;
        }

        private void CacheSelectedApp()
        {
            var filepath = CachedAppFilename();

            

        }

        private void OpenSubmitKeyDialog()
        {
            _submitKey = EditorWindow.GetWindow(typeof(SubmitKeyMenu)) as SubmitKeyMenu;
            _submitKey.OnKeyResolve += OnKeyResolved;
        }

        private void OpenSubmitVersionDetailsDialog()
        {
            _submitVersionDetails = EditorWindow.GetWindow(typeof(SubmitLabelAndChangelog)) as SubmitLabelAndChangelog;
            _submitVersionDetails.OnResolve += OnVersionDetailsResolved;
        }

        private void OnKeyResolved(ApiKey key)
        {
            _apiKey = key;

            _submitKey.OnKeyResolve -= OnKeyResolved;

            _currentGuiScreen = null;

            ApiKey.Cache(_apiKey);

            this.Focus();
        }

        private void OnVersionDetailsResolved(string label, string changelog)
        {
            this.Focus();
            PublishApp(_selectedApp.Value.Secret, label, changelog, ResolveBuildPath());
        }

        private void ResolveKeyGui()
        {
            GUILayout.Label("Please resolve the API key.");
            if (_submitKey == null)
            {
                if (GUILayout.Button("Open dialog"))
                {
                    OpenSubmitKeyDialog();
                }
            }
        }

        private void BuildGUI()
        {
            if (_hasAppBeenBuilt)
            {
                if (_submitVersionDetails == null)
                {
                    OpenSubmitVersionDetailsDialog();
                }
            }
            else
            {
                BuildApp();
            }
        }

        private void PublishGUI()
        {
            GUILayout.Label("Publishing...");

            EditorGUILayout.HelpBox("Do not close the console window!", MessageType.Warning);
        }

        private void BuildApp()
        {
            GUILayout.Label("Building...");
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;

            string errorMessage = null;
            
            // errorMessage = BuildPipeline.BuildPlayer(
            //     EditorBuildSettings.scenes.Select(s => s.path).ToArray(), 
            //     EditorUserBuildSettings.GetBuildLocation(buildTarget), 
            //     buildTarget,
            //     BuildOptions.None);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                // An error occured
                GUILayout.Label("ERROR: " + errorMessage);
                return;
            }

            _hasAppBeenBuilt = true;

            OpenSubmitVersionDetailsDialog();
        }

        private string ResolveBuildPath()
        {
            return Path.GetDirectoryName(EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget));
        }

        private void SelectApp(Api.Models.Main.App app)
        {
            _selectedApp = app;
            _currentGuiScreen = BuildGUI;
        }

        private void PublishApp(string appSecret, string label, string changelog, string buildDir)
        {
            _currentGuiScreen = PublishGUI;

            ToolsWrapper.MakeVersionHeadless(_apiKey.Key, appSecret, label, changelog, buildDir);
        }

        private void SelectAppGUI()
        {
            GUILayout.Label("Your apps: ", EditorStyles.boldLabel);

            ApiUtils apiUtils = new ApiUtils(_apiKey);

            UnityEngine.Debug.Log("Getting app list for api key: " + _apiKey.Key);
            var apps = apiUtils.GetApps(); 

            apps.ForEach(app => {
                GUILayout.Label("Name: " + app.Name);
                GUILayout.Label("Disp. name: " + app.DisplayName);
                if (GUILayout.Button("Select"))
                {
                    SelectApp(app);
                }
            });

            GUILayout.Label("New app: ", EditorStyles.boldLabel);
            GUILayout.Button("Add");
        }

        void OnGUI()
        {
            CacheSelectedApp();
            return;

            if (_currentGuiScreen == null)
            {
                if (_apiKey == null)
                {
                    _currentGuiScreen = ResolveKeyGui;
                }
                else if (!_selectedApp.HasValue)
                {
                    _currentGuiScreen = SelectAppGUI;
                }
            }

            if (_currentGuiScreen != null) _currentGuiScreen();
        }
    }
}