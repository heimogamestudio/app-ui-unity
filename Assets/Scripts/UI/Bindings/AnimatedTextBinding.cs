using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// A custom binding that animates a label's text value when the data source changes.
    /// The UxmlObject attribute allows it to appear in the UI Builder.
    /// </summary>
    [UxmlObject]
    public partial class AnimatedTextBinding : CustomBinding, IDataSourceProvider
    {
        // Data source properties
        public object dataSource { get; set; }
        public PropertyPath dataSourcePath { get; set; }

        // Animation state
        uint m_CurrentValue = 0;
        uint m_TargetValue = 0;
        bool m_IsAnimating = false;

        readonly float m_AnimationDuration = 0.5f; // Duration in seconds
        float m_AnimationStartTime = 0f;

        /// <summary>
        /// Constructor. Initializes a new instance of the <see cref="AnimatedTextBinding"/> class.
        /// </summary>
        public AnimatedTextBinding()
        {
            // The updateTrigger is a property that determines when the binding should update its target value.
            // In this example, OnSourceChanged triggers an update when the source data changes 
            updateTrigger = BindingUpdateTrigger.OnSourceChanged;
        }

        /// <summary>
        /// Updates the binding by animating the label's text when the data source value changes.
        /// </summary>
        /// <param name="context">The binding context containing the target element.</param>
        /// <returns>A <see cref="BindingResult"/> indicating the success or failure of the update.</returns>
        protected override BindingResult Update(in BindingContext context)
        {
            // The Label element
            var element = context.targetElement;

            // Attempt to get the value from the data source
            if (!TryGetValue(out uint newValue))
            {
                // Invalid data source
                return new BindingResult(BindingStatus.Failure,
                    "[AnimatedTextBinding] Update: Failed to retrieve value from data source.");
            }

            // Check if the element is a Label
            if (element is not Label label)
            {
                return new BindingResult(BindingStatus.Failure,
                    "[AnimatedTextBinding] Update: Target element is not a Label.");
            }

            // Check if the new value is different from the target value
            if (newValue == m_TargetValue)
            {
                return new BindingResult(BindingStatus.Success);
            }

            // Initialize animation parameters
            m_CurrentValue = m_IsAnimating ? GetCurrentAnimatedValue() : m_TargetValue;
            
            m_TargetValue = newValue;
            m_AnimationStartTime = Time.realtimeSinceStartup;
            m_IsAnimating = true;

            // Start the animation
            AnimateValue(label);

            return new BindingResult(BindingStatus.Success);
        }

        /// <summary>
        /// Animates the label's text value from the current value to the target value.
        /// </summary>
        /// <param name="label">The label to update.</param>
        void AnimateValue(Label label)
        {
            if (!m_IsAnimating)
                return;

            // Calculate the elapsed time
            float elapsedTime = Time.realtimeSinceStartup - m_AnimationStartTime;
            
            // Normalized progress t (0 = start; 1 = completion) is based on elapsed time 
            // relative to animation duration
            float t = Mathf.Clamp01(elapsedTime / m_AnimationDuration);

            // Interpolate between the current value and the target value
            uint interpolatedValue = (uint)Mathf.Lerp(m_CurrentValue, m_TargetValue, t);
            label.text = interpolatedValue.ToString();

            // If the animation is complete, set the target value and flag
            if (t >= 1f)
            {
                m_CurrentValue = m_TargetValue;
                m_IsAnimating = false;
            }
            else
            {
                // Otherwise call AnimateValue every frame; IVisualElementScheduler allows you to
                // schedule tasks based on a specific time or interval
                label.schedule.Execute(() => AnimateValue(label)).StartingIn(0);
            }
        }

        /// <summary>
        /// Gets the current interpolated value during animation.
        /// </summary>
        /// <returns>The interpolated value as a <see cref="uint"/>.</returns>
        uint GetCurrentAnimatedValue()
        {
            float elapsedTime = Time.realtimeSinceStartup - m_AnimationStartTime;

            float t = Mathf.Clamp01(elapsedTime / m_AnimationDuration);

            return (uint)Mathf.Lerp(m_CurrentValue, m_TargetValue, t);
        }

        /// <summary>
        /// Attempts to retrieve the value from the data source.
        /// </summary>
        /// <param name="value">The value retrieved.</param>
        /// <returns><c>true</c> if the value was successfully retrieved; otherwise, <c>false</c>.</returns>
        bool TryGetValue(out uint value)
        {
            value = default;

            if (dataSource == null)
                return false;

            // Use PropertyContainer to get the value
            if (PropertyContainer.TryGetValue(dataSource, dataSourcePath, out object objValue))
            {
                if (objValue is uint uintValue)
                {
                    value = uintValue;
                    return true;
                }
            }
            return false;
        }
    }
}