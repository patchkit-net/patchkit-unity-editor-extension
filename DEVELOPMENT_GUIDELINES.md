# Development Guidelines

## Taking advantage of development environment

To make development and debugging easier, you might want to access or display some of the background stuff. To provide such functionality, you can use scripting define `PATCHKIT_UNITY_EDITOR_EXTENSION_DEV` which is set in this project. It's very comfortable way since scripting defines aren't exported through unity package.

```CSharp
public static PatchKit.Api.ApiConnectionSettings ApiConnectionSettings
{
    get
    {
#if PATCHKIT_UNITY_EDITOR_EXTENSION_DEV
        Config instance = FindOrCreateInstance();
        
        if (instance._useOverrideApiConnectionSettings)
        {
            return instance._overrideApiConnectionSettings;
        }
#endif
        return PatchKit.Api.MainApiConnection.GetDefaultSettings();
    }
}
```