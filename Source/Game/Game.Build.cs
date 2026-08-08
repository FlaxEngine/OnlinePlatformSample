using Flax.Build;
using Flax.Build.NativeCpp;

public class Game : GameModule
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // C#-only scripting
        BuildNativeCode = false;
    }

    /// <inheritdoc />
    public override void Setup(BuildOptions options)
    {
        base.Setup(options);

        options.ScriptingAPI.IgnoreMissingDocumentationWarnings = true;

        // Here you can modify the build options for your game module
        // To reference another module use: options.PublicDependencies.Add("Audio");
        // To add C++ define use: options.PublicDefinitions.Add("COMPILE_WITH_FLAX");
        // To learn more see scripting documentation.

        switch (options.Platform.Target)
        {
        case TargetPlatform.Windows:
            options.PublicDependencies.Add("OnlinePlatformSteam");
            break;
        case TargetPlatform.XboxOne:
        case TargetPlatform.XboxScarlett:
            options.PublicDependencies.Add("OnlinePlatformXboxLive");
            break;
        case TargetPlatform.PS4:
            options.PublicDependencies.Add("OnlinePlatformPS4");
            break;
        case TargetPlatform.Switch:
            options.PublicDependencies.Add("OnlinePlatformSwitch");
            break;
        }
    }
}
