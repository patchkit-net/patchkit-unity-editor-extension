using System.Linq;
using JetBrains.Annotations;
using PatchKit.Api.Models.Main;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.UI
{
public class LinkAppScreen : Screen
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
        get { return "Link App"; }
    }

    public override Vector2? Size
    {
        get { return new Vector2(400f, 400f); }
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
        GUILayout.Label(
            string.Format(
                "Link your PatchKit application for {0}.",
                _platform.ToDisplayString()),
            EditorStyles.boldLabel);

        EditorGUILayout.Space();

        _scrollViewVector = EditorGUILayout.BeginScrollView(
            _scrollViewVector,
            EditorStyles.helpBox);

        foreach (App app in Apps)
        {
            DrawApp(app);
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        if (GUILayout.Button("Create new app", GUILayout.ExpandWidth(true)))
        {
            Dispatch(() => CreateNew());
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(true)))
        {
            Dispatch(() => Cancel());
        }
    }

    private void DrawApp(App app)
    {
        Assert.IsNotNull(app.Name);
        Assert.IsNotNull(app.Platform);

        bool isLinked = app.Secret == _linkedAppSecret;

        using (Style.ColorifyBackground(
            isLinked ? Color.green : new Color(0.8f, 0.8f, 0.8f)))
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.textArea);

            EditorGUILayout.BeginVertical();
            {
                if (app.Name.Length > 30)
                {
                    string shortName = app.Name.Substring(0, 30);
                    shortName += "...";
                    GUILayout.Label(
                        new GUIContent(shortName, app.Name),
                        EditorStyles.boldLabel);
                }
                else
                {
                    GUILayout.Label(app.Name, EditorStyles.largeLabel);
                }

                GUILayout.Label(app.Secret, EditorStyles.miniBoldLabel);

                if (isLinked)
                {
                    GUILayout.Label(
                        "Currently selected app",
                        EditorStyles.miniLabel);
                }
                else
                {
                    using (Style.ColorifyBackground(Color.cyan))
                    {
                        if (GUILayout.Button(
                            "Select",
                            GUILayout.ExpandWidth(true)))
                        {
                            Dispatch(() => Link(app));
                        }
                    }
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }


        Assert.IsNotNull(GUI.skin);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    #endregion

    #region Data

    [SerializeField]
    private AppPlatform _platform;

    [SerializeField]
    private Vector2 _scrollViewVector;

    [SerializeField]
    private string _linkedAppSecret;

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

    private App[] _apps;

    [NotNull]
    private App[] Apps
    {
        get
        {
            return _apps ??
                (_apps = Core.Api.GetApps()
                    .Where(x => x.Platform == _platform.ToApiString())
                    .ToArray());
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