using UnityEngine.UIElements;
using System.Threading.Tasks;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// Shows the player's total experience level (the sum of all four character levels) using a custom Radial Counter
    /// </summary>
    // 
    public class LevelMeterView : UIView
    {
        // Vector element representing the radial Progress bar for the player's level
        RadialProgress m_LevelMeterRadialProgressBar;

        // Label showing the player's current numeric level
        Label m_LevelMeterNumberLabel;

        // Label displaying the player's rank based on their total levels (visible on hover)
        Label m_LevelMeterRankLabel;

        // Holds the player's level data, which is bound to the UI
        readonly LevelMeterData m_LevelMeterData;

        bool m_IsRankLabelVisible;
        bool m_IsCooldownActive;

        const int k_DelayInSeconds = 1;

        /// <summary>
        /// Constructor for the LevelMeterView, initializes the level meter with the provided data and registers hover events.
        /// </summary>
        /// <param name="topElement">The root VisualElement that contains the level meter UI.</param>
        /// <param name="levelMeterData">The data source for the player's total levels.</param>
        public LevelMeterView(VisualElement topElement, LevelMeterData levelMeterData) : base(topElement)
        {
            // Listen for character level changes
            m_LevelMeterData = levelMeterData;

            // topElement.RegisterCallback<PointerEnterEvent>(evt => OnPointerEnter());
            topElement.RegisterCallback<PointerLeaveEvent>(evt => OnPointerLeave());

            // Use PointerDownEvent to toggle the visibility of the rank label
            topElement.RegisterCallback<PointerDownEvent>(evt => OnPointerDown());

            // Enable rank label visibility if you drag into Level Meter
            topElement.RegisterCallback<PointerEnterEvent>(evt => OnPointerEnter());
        }

        /// <summary>
        /// Sets up the visual elements and data bindings.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            SetVisualElements();
            SetupDataBindings();

            // Hide by default
            m_LevelMeterRankLabel.style.display = DisplayStyle.Flex;
            m_LevelMeterRankLabel.style.opacity = 0;
        }

        /// <summary>
        /// Queries the top element of the Level Meter to assign the different UI elements
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // References the circular Progress bar
            m_LevelMeterRadialProgressBar = m_TopElement.Q<RadialProgress>("level-meter__radial-progress");

            // References the level number Label
            m_LevelMeterNumberLabel = m_TopElement.Q<Label>("level-meter__number");

            m_LevelMeterRankLabel = m_TopElement.Q<Label>("level-meter__rank");
        }

        /// <summary>
        /// Set up data bindings with runtime data binding system
        /// </summary>
        void SetupDataBindings()
        {
            // Create a data binding for TotalLevels
            var totalLevelBinding = new DataBinding()
            {
                dataSource = m_LevelMeterData, // The source of the data (LevelMeterData)
                dataSourcePath =
                    new PropertyPath(nameof(m_LevelMeterData.TotalLevels)), // The path to the TotalLevels property
                bindingMode = BindingMode.ToTarget // One-way binding (data -> UI)
            };

            // Apply a per-binding converter for RadialCounter (int to float)
            totalLevelBinding.sourceToUiConverters.AddConverter((ref int total) => (float)total);

            // Bind the Progress property of the radial progress bar to TotalLevels
            m_LevelMeterRadialProgressBar.SetBinding("Progress", totalLevelBinding);

            // Bind the numeric text
            m_LevelMeterNumberLabel.SetBinding("text", totalLevelBinding);

            // Bind same data source using a different Converter; this text appears when hovering the mouse over the Level Meter
            var rankLevelBinding = new DataBinding()
            {
                dataSource = m_LevelMeterData, // The source of the data (LevelMeterData)
                dataSourcePath =
                    new PropertyPath(nameof(m_LevelMeterData.TotalLevels)), // The path to the TotalLevels property
                bindingMode = BindingMode.ToTarget // One-way binding (data -> UI)
            };

            rankLevelBinding.sourceToUiConverters.AddConverter(
                (ref int total) => LevelMeterData.GetRankFromLevel(total));

            m_LevelMeterRankLabel.SetBinding("text", rankLevelBinding);
        }


        /// <summary>
        /// Called when the pointer (mouse or touch) is pressed or enters the element.
        /// </summary>
        private void OnPointerEnter()
        {
            if (!m_IsRankLabelVisible)
            {
                ShowRankLabel(true); // Show the label immediately when the pointer enters or taps/clicks
            }
        }

        /// <summary>
        /// Called when the pointer (mouse or touch) leaves the element
        /// </summary>
        private async void OnPointerLeave()
        {
            if (m_IsRankLabelVisible && !m_IsCooldownActive)
            {
                await StartCooldown();
            }
        }

        /// <summary>
        /// Called when the pointer (mouse or touch) is pressed
        /// </summary>
        private void OnPointerDown()
        {
            if (!m_IsRankLabelVisible)
            {
                ShowRankLabel(true); // Show the label immediately
            }
        }

        /// <summary>
        /// Toggle the visibility of the rank label.
        /// </summary>
        private void ShowRankLabel(bool state)
        {
            if (state)
            {
                m_LevelMeterRankLabel.style.display = DisplayStyle.Flex;
                m_LevelMeterRankLabel.style.opacity = 1f; // Show the rank label
                m_IsRankLabelVisible = true;
            }
            else
            {
                m_LevelMeterRankLabel.style.opacity = 0f; // Hide the rank label
                m_IsRankLabelVisible = false;
            }
        }

        /// <summary>
        /// Starts the cooldown period to hide the rank label after n seconds
        /// </summary>
        private async Task StartCooldown()
        {
            m_IsCooldownActive = true;

            await Task.Delay(k_DelayInSeconds * 1000); // Wait for n seconds
            ShowRankLabel(false); // Hide the label after the cooldown

            m_IsCooldownActive = false;
        }
    }
}