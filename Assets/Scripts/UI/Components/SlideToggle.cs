// This is an example of a Custom Control that demonstrates a "switch-like" variation of a toggle.
// Manual documentation: https://docs.unity3d.com/Manual/UIE-slide-toggle.html

using UnityEngine;
using UnityEngine.UIElements;

namespace MyUILibrary
{
    /// <summary>
    /// A custom switch-like toggle control derived from BaseField<bool>.
    /// </summary>
    [UxmlElement]
    public partial class SlideToggle : BaseField<bool>
    {
        //  Class names for the toggle's block, elements, and states. Names use BEM standard.

        public static readonly new string ussClassName = "slide-toggle";
        public static readonly new string inputUssClassName = "slide-toggle__input";
        public static readonly string inputKnobUssClassName = "slide-toggle__input-knob";
        public static readonly string inputCheckedUssClassName = "slide-toggle__input--checked";
        public static readonly string stateLabelUssClassName = "slide-toggle__state-label";

        bool m_IsOn;

        /// <summary>
        /// Label to display when the toggle is in the 'On' state.
        /// </summary>
        public string OnLabel { get; set; } = "On"; // Default value

        /// <summary>
        /// Label to display when the toggle is in the 'Off' state.
        /// </summary>
        public string OffLabel { get; set; } = "Off"; // Default value
        
        /// <summary>
        /// Attribute for toggle state.
        /// </summary>
        [UxmlAttribute("IsOn")]
        public bool IsOn
        {
            get => m_IsOn;
            set
            {
                if (m_IsOn == value)
                    return;

                m_IsOn = value;
                UpdateStateLabel();
                SetValueWithoutNotify(m_IsOn);
            }
        }

        VisualElement m_Input; // Input part of the toggle.
        VisualElement m_Knob; // Knob element.
        Label m_StateLabel; // On/off label.

        /// <summary>
        /// Default constructor (calls the other constructor in this class).
        /// </summary>
        public SlideToggle() : this(null)
        {
        }

        /// <summary>
        /// Constructor with label.
        /// </summary>
        /// <param name="label">Text for the toggle's label.</param>
        public SlideToggle(string label) : base(label, null)
        {
            // Style the control overall.
            AddToClassList(ussClassName);

            // Get the BaseField's visual input element and use it as the background of the slide.
            m_Input = this.Q(className: BaseField<bool>.inputUssClassName);
            m_Input.AddToClassList(inputUssClassName);
            Add(m_Input);

            // Create a "knob" child element for the background to represent the actual slide of the toggle.
            m_Knob = new();
            m_Knob.AddToClassList(inputKnobUssClassName);
            m_Input.Add(m_Knob);

            m_StateLabel = new Label();
            m_StateLabel.AddToClassList(stateLabelUssClassName);
            m_Input.Add(m_StateLabel);

            // There are three main ways to activate or deactivate the SlideToggle. All three event handlers use the
            // static function pattern described in the Custom control best practices.

            // ClickEvent fires when a sequence of pointer down and pointer up actions occurs.
            RegisterCallback<ClickEvent>(evt => OnClick(evt));

            // KeydownEvent fires when the field has focus and a user presses a key.
            RegisterCallback<KeyDownEvent>(evt => OnKeydownEvent(evt));

            // NavigationSubmitEvent detects input from keyboards, gamepads, or other devices at runtime.
            RegisterCallback<NavigationSubmitEvent>(evt => OnSubmit(evt));

            // 
            UpdateStateLabel();
        }

        /// <summary>
        /// Handles pointer click event to toggle the switch.
        /// </summary>
        /// <param name="evt"></param>
        static void OnClick(ClickEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;
            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        /// <summary>
        /// Handles submit event (keyboard/gamepad) to toggle the switch.
        /// </summary>
        /// <param name="evt"></param>
        static void OnSubmit(NavigationSubmitEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;

            if (slideToggle == null)
                return;

            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        /// <summary>
        /// Handles key press event to toggle the switch.
        /// </summary>
        /// <param name="evt"></param>
        static void OnKeydownEvent(KeyDownEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;

            if (slideToggle == null)
                return;

            // NavigationSubmitEvent event already covers keydown events at runtime, so this method shouldn't handle
            // them.
            if (slideToggle.panel.contextType == ContextType.Player)
                return;

            // Toggle the value only when the user presses Enter, Return, or Space.
            if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
            {
                slideToggle.ToggleValue();
                evt.StopPropagation();
            }
        }

        /// <summary>
        /// All three callbacks call this method.
        /// </summary>
        void ToggleValue()
        {
            value = !value;
            UpdateStateLabel();
        }

        /// <summary>
        /// Sets the value of the switch without sending change notifications and updates the toggle's
        /// visual state.
        /// 
        /// Triggered by the ChangeEvent invoked from modifying the value in ToggleValue.
        /// </summary>
        /// <param name="newValue">The new state of the switch.</param>
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);

            // This styles the input element to look enabled or disabled.
            m_Input.EnableInClassList(inputCheckedUssClassName, newValue);

            // Update on/off text
            UpdateStateLabel();
        }

        /// <summary>
        /// Update the on/off label.
        /// </summary>
        void UpdateStateLabel()
        {
            if (value) // If the toggle is on
            {
                m_StateLabel.text = OnLabel;
            }
            else
            {
                m_StateLabel.text = OffLabel;
            }
        }
        
        
    }
}