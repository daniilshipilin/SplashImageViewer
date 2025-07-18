namespace SplashImageViewer.Helpers;

using System;
using System.Runtime.InteropServices;

public static partial class Screensaver
{
    [Flags]
    private enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040, // This flag must be combined with ES_CONTINUOUS. If the machine is configured to allow it, this indicates that the thread requires away mode. When in away mode the computer will appear to sleep as normal. However, the thread will continue to execute even though the computer has partially suspended. As this flag gives the false impression that the computer is in a low power state, you should only use it when absolutely necessary.
        ES_CONTINUOUS = 0x80000000,        // This flag is used to specify that the behaviour of the two previous flags is continuous. Rather than resetting the idle timers once, they are disabled until you specify otherwise. Using this flag means that you do not need to call SetThreadExecutionState repeatedly.
        ES_DISPLAY_REQUIRED = 0x00000002,  // This flag indicates that the display is in use. When passed by itself, the display idle timer is reset to zero once. The timer restarts and the screensaver will be displayed when it next expires.
        ES_SYSTEM_REQUIRED = 0x00000001,   // This flag indicates that the system is active. When passed alone, the system idle timer is reset to zero once. The timer restarts and the machine will sleep when it expires.
    }

    /// <summary>
    /// Disables the screensaver.
    /// </summary>
    public static void Disable() => SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS); // To disable it until we state otherwise, we use the ES_DISPLAY_REQUIRED and ES_CONTINUOUS flags.

    /// <summary>
    /// Re-enables the screensaver.
    /// </summary>
    public static void Reset() => SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS); // Re-enabling the screensaver requires that we clear the ES_DISPLAY_REQUIRED state flag. We can do this by passing the ES_CONTINUOUS flag alone

    [LibraryImport("kernel32.dll")]
    private static partial EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
}
