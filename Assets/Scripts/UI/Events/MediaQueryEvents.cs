using System;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// Public static delegates associated with changing screen resolution and aspect ratio.
    /// This is used to inform any components listening for landscape/portrait switching.
    ///
    /// Note: these are "events" in the conceptual sense and not the strict C# sense.
    /// </summary>
    public class MediaQueryEvents 
    {
        // Raised when the screen dimension changes
        public static Action<Vector2> ResolutionUpdated;

        public static Action<MediaAspectRatio> AspectRatioUpdated;

        public static Action CameraResized;

        public static Action SafeAreaApplied;
    }
}
