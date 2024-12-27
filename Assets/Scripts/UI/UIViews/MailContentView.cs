using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using System;
using Unity.Properties;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    public class MailContentView : UIView
    {
        const string k_GiftDeletedClass = "mail-gift-button--deleted";
        const string k_FrameBarUnclaimedClass = "mail-frame_bar--unclaimed";
        const string k_FrameBarClaimedClass = "mail-frame_bar--claimed";
        const string k_FrameBorderUnclaimedClass = "mail-frame_border--unclaimed";
        const string k_FrameBorderClaimedClass = "mail-frame_border--claimed";
        const string k_MailNoMessagesClass = "mail-no-messages";
        const string k_MailNoMessagesInactiveClass = "mail-no-messages--inactive";
        const float k_TransitionTime = 0.1f;

        Button m_ClaimButton;
        Button m_DeleteButton;
        Button m_UndeleteButton;

        VisualElement m_Footer;
        VisualElement m_FrameBorder;
        VisualElement m_FrameBar;

        Label m_MessageSubject;
        Label m_MessageText;
        VisualElement m_MessageAttachment;
        Label m_GiftAmount;
        VisualElement m_GiftIcon;

        Label m_NoMessagesLabel;

        // LocalizedString for "No message selected"
        LocalizedString m_NoMessagesLocalizedString;

        // currently selected mail item (from the currently selected mailbox tab), defaults to top item
        int m_CurrentMessageIndex = 0;


        /// <summary>
        /// Constructor assigns game icons from Resources.
        /// </summary>
        /// <param name="topElement"></param>
        public MailContentView(VisualElement topElement) : base(topElement)
        {
            // Reference the LocalizedString
            m_NoMessagesLocalizedString = new LocalizedString()
            {
                TableReference = "SettingsTable",
                TableEntryReference = "Mail_NoMessage"
            };
            
            // Listen for locale changes
            m_NoMessagesLocalizedString.StringChanged += OnNoMessagesStringChanged;
        }

        /// <summary>
        /// Update the NoMessages Label when the localized string updates.
        /// </summary>
        /// <param name="localizedText">The new localized text.</param>
        void OnNoMessagesStringChanged(string localizedText)
        {
            m_NoMessagesLabel.text = localizedText;
        }

        public override void Initialize()
        {
            base.Initialize();

            // Get the current message index selected from the Mailbox
            MailEvents.MessageSelected += OnMessageSelected;

            // Show label that no message is available
            MailEvents.ShowEmptyMessage += OnShowEmptyMessage;

            // Show a specific mail message in the MailContent
            MailEvents.MessageShown += OnMessageShown;

            BindElements();
        }

        void OnMessageSelected(int index)
        {
            m_CurrentMessageIndex = index;
        }

        /// <summary>
        /// Unsubscribe from events to prevent memory leaks.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            MailEvents.MessageSelected -= OnMessageSelected;
            MailEvents.ShowEmptyMessage -= OnShowEmptyMessage;
            MailEvents.MessageShown -= OnMessageShown;

            m_NoMessagesLocalizedString.StringChanged -= OnNoMessagesStringChanged;
            
            UnregisterButtonCallbacks();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_ClaimButton = m_TopElement.Q<Button>("content__gift-button");
            m_DeleteButton = m_TopElement.Q<Button>("content__delete-button");
            m_UndeleteButton = m_TopElement.Q<Button>("content__undelete-button");

            m_MessageSubject = m_TopElement.Q<Label>("content__message-subject");
            m_MessageText = m_TopElement.Q<Label>("content__message-text");
            m_MessageAttachment = m_TopElement.Q("content__message-attachment");

            m_GiftIcon = m_TopElement.Q("content__gift-icon");
            m_GiftAmount = m_TopElement.Q<Label>("content__gift-amount");

            m_Footer = m_TopElement.Q("content__footer");
            m_FrameBorder = m_TopElement.Q("content__frame-border");
            m_FrameBar = m_TopElement.Q("content__frame-bar");

            m_NoMessagesLabel = m_TopElement.Q<Label>("content__no-messages");
        }

        protected override void RegisterButtonCallbacks()
        {
            m_ClaimButton.RegisterCallback<ClickEvent>(ClaimReward);
            m_DeleteButton.RegisterCallback<ClickEvent>(DeleteMailMessage);
            m_UndeleteButton.RegisterCallback<ClickEvent>(UndeleteMailMessage);
        }

        // Optional: Unregistering the button callbacks is not strictly necessary
        // in most cases and depends on your application's lifecycle management.
        // You can choose to unregister them if needed for specific scenarios.
        void UnregisterButtonCallbacks()
        {
            m_ClaimButton.UnregisterCallback<ClickEvent>(ClaimReward);
            m_DeleteButton.UnregisterCallback<ClickEvent>(DeleteMailMessage);
            m_UndeleteButton.UnregisterCallback<ClickEvent>(UndeleteMailMessage);
        }

        // Show "No messages selected" label

        void ShowEmptyMessage()
        {
            // Manually disable styles and events when no active mail message is available
            m_MessageSubject.style.display = DisplayStyle.None;
            m_MessageText.style.display = DisplayStyle.None;
            m_MessageAttachment.style.display = DisplayStyle.None;
            m_ClaimButton.style.display = DisplayStyle.None;
            m_DeleteButton.style.display = DisplayStyle.None;
            m_UndeleteButton.style.display = DisplayStyle.None;

            m_ClaimButton.SetEnabled(false);
            m_DeleteButton.SetEnabled(false);

            ShowNoMessages(true);

            // Hide the footer
            ShowFooter(false);
        }

        void ShowNoMessages(bool state)
        {
            if (state)
            {
                m_NoMessagesLabel.RemoveFromClassList(k_MailNoMessagesInactiveClass);
                m_NoMessagesLabel.AddToClassList(k_MailNoMessagesClass);
            }
            else
            {
                m_NoMessagesLabel.RemoveFromClassList(k_MailNoMessagesClass);
                m_NoMessagesLabel.AddToClassList(k_MailNoMessagesInactiveClass);
            }
        }

        // fill the right panel with the email text
        void ShowMailContents(MailMessageSO msg)
        {
            // empty message, nothing in current mailbox
            if (msg == null)
            {
                ShowEmptyMessage();
                return;
            }

            m_ClaimButton.SetEnabled(true);

            m_MessageSubject.style.display = DisplayStyle.Flex;
            m_MessageText.style.display = DisplayStyle.Flex;
            m_MessageAttachment.style.display = DisplayStyle.Flex;
            m_ClaimButton.style.display = DisplayStyle.Flex;
            m_DeleteButton.style.display = DisplayStyle.Flex;
            m_UndeleteButton.style.display = DisplayStyle.Flex;


            m_DeleteButton.SetEnabled(true);

            ShowNoMessages(false);

            m_TopElement.dataSource = msg;

            if (!msg.IsDeleted)
            {
                m_GiftAmount.RemoveFromClassList(k_GiftDeletedClass);
                m_GiftIcon.RemoveFromClassList(k_GiftDeletedClass);
            }

            // Show the footer only if the message is not deleted and the gift is not claimed
            ShowFooter(!msg.IsDeleted && !msg.IsClaimed);
        }

        void BindElements()
        {
            // Bind UI elements to MailMessageSO properties using Unity's runtime data binding
            m_MessageSubject.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.SubjectLine)),
                bindingMode = BindingMode.ToTarget
            });

            m_MessageText.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.EmailText)),
                bindingMode = BindingMode.ToTarget
            });

            m_MessageAttachment.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.EmailPicAttachment)),
                bindingMode = BindingMode.ToTarget
            });

            // Bind reward elements (gift amount and icon)
            m_GiftAmount.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.RewardValue)),
                bindingMode = BindingMode.ToTarget
            });

            // Bind the visibility of gift amount
            m_GiftAmount.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.GiftAmountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // Bind the reward icon (based on reward type)
            m_GiftIcon.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.RewardIcon)),
                bindingMode = BindingMode.ToTarget
            });

            m_GiftIcon.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.GiftIconDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // Bind the visibility of claim button
            m_ClaimButton.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.ClaimButtonDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // Bind the visibility of delete button
            m_DeleteButton.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.DeleteButtonDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // Bind the visibility of undelete button
            m_UndeleteButton.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.UndeleteButtonDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
        }

        // Shows/hides the bottom of the content window
        void ShowFooter(bool state)
        {
            m_Footer.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;

            // show the frame border and bar
            if (state)
            {
                m_FrameBar.RemoveFromClassList(k_FrameBarClaimedClass);
                m_FrameBar.AddToClassList(k_FrameBarUnclaimedClass);

                m_FrameBorder.RemoveFromClassList(k_FrameBorderClaimedClass);
                m_FrameBorder.AddToClassList(k_FrameBorderUnclaimedClass);
            }
            // hide the frame border and bar
            else
            {
                m_FrameBar.RemoveFromClassList(k_FrameBarUnclaimedClass);
                m_FrameBar.AddToClassList(k_FrameBarClaimedClass);

                m_FrameBorder.RemoveFromClassList(k_FrameBorderUnclaimedClass);
                m_FrameBorder.AddToClassList(k_FrameBorderClaimedClass);
            }
        }

        // Reward methods

        // Tell the MailController/GameDataManager to claim the gift
        void ClaimReward(ClickEvent evt)
        {
            // Convert the ClickEvent's position into pixel screen coordinates
            Vector2 clickPos = new Vector2(evt.position.x, evt.position.y);

            // Get the screen coordinates relative to the whole screen / root element
            VisualElement rootElement = m_TopElement.panel.visualTree;
            Vector2 screenPos = clickPos.GetScreenCoordinate(rootElement);

            // Fire and forget, discard the Task
            _ = ClaimRewardRoutineAsync();

            // Notify the MailController with the screen position
            MailEvents.ClaimRewardClicked?.Invoke(m_CurrentMessageIndex, screenPos);

            // Play a click sound
            AudioManager.PlayDefaultButtonSound();
        }

        // Use async await for non-MonoBehaviours
        async Task ClaimRewardRoutineAsync()
        {
            // Apply USS transitions for gift icon and label
            m_GiftAmount.AddToClassList(k_GiftDeletedClass);
            m_GiftIcon.AddToClassList(k_GiftDeletedClass);

            // Convert seconds to milliseconds
            await Task.Delay((int)(k_TransitionTime * 1000));

            // Animate the footer off and disable the claim button
            ShowFooter(false);
        }

        // Delete-undelete methods

        void DeleteMailMessage(ClickEvent evt)
        {
            // Notify the mailbox to play the animation
            MailEvents.DeleteClicked?.Invoke();

            // Discard, fire and forget
            _ = DeleteMailMessageRoutine();
        }

        // Wait for USS transition and then notify the controller
        async Task DeleteMailMessageRoutine()
        {
            AudioManager.PlayDefaultButtonSound();

            // Wait for transition
            await Task.Delay(TimeSpan.FromSeconds(k_TransitionTime));

            // Tells the Mail Presenter/Controller to delete the current message, then rebuild the interface
            MailEvents.MessageDeleted?.Invoke(m_CurrentMessageIndex);

            m_MessageAttachment.style.backgroundImage = null;
        }

        // Notify the Controller to undelete the current selection
        void UndeleteMailMessage(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            MailEvents.UndeleteClicked?.Invoke(m_CurrentMessageIndex);
        }

        // Event-handling methods

        void OnMessageShown(MailMessageSO msg)
        {
            if (msg != null)
            {
                ShowMailContents(msg);
            }
        }

        void OnShowEmptyMessage()
        {
            ShowEmptyMessage();
        }
    }
}