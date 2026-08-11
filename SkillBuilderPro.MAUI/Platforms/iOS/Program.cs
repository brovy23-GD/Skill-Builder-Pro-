// Location: SkillBuilderPro.MAUI/Platforms/iOS/Program.cs
using UIKit;

namespace SkillBuilderPro.MAUI;

public class Program
{
    // This is the native static entry point that the iOS compiler searches for
    static void Main(string[] args)
    {
        // Explicitly links the native window cycle directly to your AppDelegate class
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
