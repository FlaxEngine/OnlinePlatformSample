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

        // Reference platform-specific Online impl (see TestOnlineScript which creates them)
        switch (options.Platform.Target)
        {
        case TargetPlatform.Windows:
        case TargetPlatform.Linux:
        case TargetPlatform.Mac:
            options.PrivateDependencies.Add("OnlinePlatformSteam");
            break;
        case TargetPlatform.XboxOne:
        case TargetPlatform.XboxScarlett:
            options.PrivateDependencies.Add("OnlinePlatformXboxLive");
            break;
        case TargetPlatform.Switch:
            options.PrivateDependencies.Add("OnlinePlatformSwitch");
            break;
        case TargetPlatform.PS4:
            options.PrivateDependencies.Add("OnlinePlatformPS4");
            break;
        case TargetPlatform.PS5:
            options.PrivateDependencies.Add("OnlinePlatformPS5");
            break;

        }
    }
}
