using System;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// Public static delegates associated with changing Themes.
    /// This can inform any components listening for seasonal or portrait/landscape theme changes. 
    ///
    /// Note: these are "events" in the conceptual sense and not the strict C# sense.
    /// </summary>
    public static class ThemeEvents
    {
        // Event for changing themes (string represents the theme name)
        public static Action<string> ThemeChanged;

        // Event triggered for updating a Theme Camera
        public static Action<Camera> CameraUpdated;

    }
}
