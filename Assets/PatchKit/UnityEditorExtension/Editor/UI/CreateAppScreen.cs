using System.Linq;
using JetBrains.Annotations;
using PatchKit.Api.Models.Main;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PatchKit.UnityEditorExtension.UI
{
public class CreateAppScreen : Screen
{
    public class CreatedResult
    {
        public readonly App App;

        public CreatedResult(App app)
        {
            App = app;
        }
    }

    #region GUI

    public override string Title
    {
        get { return "Create app"; }
    }

    public override Vector2? Size
    {
        get { return new Vector2(400f, 150f); }
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
                "Create new PatchKit app for {0}",
                _platform.ToDisplayString()),
            EditorStyles.boldLabel);

        _name = EditorGUILayout.TextField("Name: ", _name);

        EditorGUILayout.Space();

        if (string.IsNullOrEmpty(_name))
        {
            EditorGUILayout.HelpBox(
                "Please insert your application name",
                MessageType.Info);
        }
        else
        {
            if (NameValidationError != null)
            {
                EditorGUILayout.HelpBox(NameValidationError, MessageType.Error);
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    using (Style.Colorify(Color.green))
                    {
                        if (GUILayout.Button("Create", GUILayout.Width(100)))
                        {
                            Dispatch(() => Create());
                        }
                    }

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                Dispatch(() => Cancel());
            }

            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
    }

    #endregion

    #region Data

    [SerializeField]
    private AppPlatform _platform;

    [SerializeField]
    private string _name;

    #endregion

    #region Logic

    public void Initialize(AppPlatform platform)
    {
        _platform = platform;

        _name = string.Empty;
    }

    public override void OnActivatedFromTop(object result)
    {
    }

    private App[] _apps;

    [NotNull]
    private App[] Apps
    {
        get { return _apps ?? (_apps = Core.Api.GetApps()); }
    }

    private string NameValidationError
    {
        get
        {
            if (Apps.Any(x => x.Name == _name))
            {
                return "This name is already taken.";
            }

            return AppName.GetValidationError(_name);
        }
    }

    private void Create()
    {
        App app = Core.Api.CreateNewApp(new AppName(_name), _platform);

        Pop(new CreatedResult(app));
    }

    private void Cancel()
    {
        Pop(null);
    }

    #endregion
}
}