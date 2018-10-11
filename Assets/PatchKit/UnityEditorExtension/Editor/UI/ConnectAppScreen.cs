using System.Linq;
using JetBrains.Annotations;
using PatchKit.Api.Models.Main;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace PatchKit.UnityEditorExtension.UI
{
    public class ConnectAppScreen : Screen
    {
        public class LinkedResult
        {
            public readonly App App;

            public LinkedResult(App app)
            {
                App = app;
            }
        }

        #region GUI

        public override string Title
        {
            get { return "Connect App"; }
        }

        public override Vector2? Size
        {
            get { return new Vector2(400f, 600f); }
        }

        public override void UpdateIfActive()
        {
            if (!Config.GetLinkedAccountApiKey().HasValue)
            {
                Push<NotLinkedAccountScreen>().Initialize();
            }
        }

        public override void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(
                    new GUIContent((Texture) EditorGUIUtility.Load("arrowIcon.png"),
                                   "Return"),
                    GUILayout.Width(35),
                    GUILayout.Height(20)))
                {
                    Dispatch(() => Cancel());
                }

                GUILayout.Space(220);
                if (GUILayout.Button(
                    "Create new app",
                    GUILayout.Width(130),
                    GUILayout.Height(20)))
                {
                    Dispatch(() => CreateNew());
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(
                    string.Format(
                        "Connect your PatchKit application for {0}.",
                        _platform.ToDisplayString()),
                    EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            ShouldFilterByPlatform = EditorGUILayout.Toggle(
                "Filter apps by platform",
                ShouldFilterByPlatform);
            EditorGUILayout.Space();

            _scrollViewVector = EditorGUILayout.BeginScrollView(
                _scrollViewVector,
                EditorStyles.helpBox);


            int appIndex = 0;
            foreach (App app in Apps)
            {
                DrawApp(app, appIndex % 2);
                appIndex++;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawApp(App app, int i)
        {
            Assert.IsNotNull(app.Name);
            Assert.IsNotNull(app.Platform);

            bool isLinked = app.Secret == _linkedAppSecret;

            Color backgroundColor = (i == 0)
                ? new Color(1f, 1f, 1f)
                : new Color(0.9f, 0.9f, 0.9f);


            using (Style.ColorifyBackground(
                isLinked ? new Color(0.502f, 0.839f, 0.031f) : backgroundColor))
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.textArea);

                EditorGUILayout.BeginVertical();
                {
                    GUIStyle style = new GUIStyle();
                    style = EditorStyles.largeLabel;
                    style.fontStyle = FontStyle.Bold;

                    if (app.Name.Length > 30)
                    {
                        string shortName = app.Name.Substring(0, 30);
                        shortName += "...";
                        GUILayout.Label(
                            new GUIContent(shortName, app.Name),
                            style);
                    }
                    else
                    {
                        GUILayout.Label(app.Name, style);
                    }

                    GUILayout.Label("Platform: " + app.Platform);

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(
                            "Secret: " + app.Secret,
                            EditorStyles.miniBoldLabel);

                        if (isLinked)
                        {
                            GUILayout.Label(
                                "Currently\nselected",
                                EditorStyles.miniLabel);
                        }
                        else
                        {
                            if (app.Platform == _platform.ToApiString())
                            {
                                using (Style.ColorifyBackground(
                                    Style.greenPastel))
                                {
                                    if (GUILayout.Button(
                                        "Select",
                                        GUILayout.Width(80)))
                                    {
                                        Dispatch(() => Link(app));
                                    }
                                }
                            }
                            else
                            {
                                using (Style.ColorifyBackground(
                                    Style.redPastel))
                                {
                                    if (GUILayout.Button(
                                        "Select",
                                        GUILayout.Width(80)))
                                    {
                                        EditorUtility.DisplayDialog(
                                            "Warning",
                                            "You tried to choose an application with wrong platform.\n\n" +
                                            " Connect your PatchKit application for " +
                                            _platform.ToString() +
                                            " platform",
                                            "Ok");
                                    }
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }


            Assert.IsNotNull(GUI.skin);
        }

        #endregion

        #region Data

        [SerializeField] private AppPlatform _platform;

        [SerializeField] private Vector2 _scrollViewVector;

        [SerializeField] private string _linkedAppSecret;

        #endregion

        #region Logic

        public void Initialize(AppPlatform platform)
        {
            _platform = platform;

            AppSecret? appSecret = Config.GetLinkedAppSecret(platform);

            if (appSecret.HasValue)
            {
                _linkedAppSecret = appSecret.Value.Value;
            }
            else
            {
                _linkedAppSecret = null;
            }
        }

        public override void OnActivatedFromTop(object result)
        {
            if (result is CreateAppScreen.CreatedResult)
            {
                App app = ((CreateAppScreen.CreatedResult) result).App;

                Config.LinkApp(new AppSecret(app.Secret), _platform);

                Pop(new LinkedResult(app));
            }
        }

        private bool _shouldFilterByPlatform = true;
        private bool _isFilterToogleChanged = false;

        public bool ShouldFilterByPlatform
        {
            get { return _shouldFilterByPlatform; }

            private set
            {
                if (value != _shouldFilterByPlatform)
                {
                    _shouldFilterByPlatform = value;
                    _isFilterToogleChanged = true;
                }
            }
        }

        private App[] _apps;

        [NotNull]
        private IEnumerable<App> Apps
        {
            get
            {
                if (_apps == null)
                {
                    _apps = Core.Api.GetApps();
                }
              
                return _apps.Where(
                    x => (!_shouldFilterByPlatform &&  x.Platform != "other")||
                        x.Platform == _platform.ToApiString());
            }
        }

        private void Link(App app)
        {
            Assert.AreEqual(_platform.ToApiString(), app.Platform);
            Assert.IsTrue(Apps.Contains(app));

            Config.LinkApp(new AppSecret(app.Secret), _platform);

            Pop(new LinkedResult(app));
        }

        private void CreateNew()
        {
            Push<CreateAppScreen>().Initialize(_platform);
        }

        private void Cancel()
        {
            Pop(null);
        }

        #endregion
    }
}