using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class MailboxView : UIView
    {
        // Sprites
        Sprite m_NewMailIcon;
        Sprite m_OldMailIcon;
        GameIconsSO m_GameIcons;


        VisualTreeAsset m_MailMessageAsset;
        VisualElement m_MailboxContainer;

        // Currently selected mail item (from the currently selected mailbox tab), defaults to top item
        int m_CurrentMessageIndex = 0;

        const string k_MailMessageClass = "mail-message";
        const string k_MailMessageSelectedClass = "mail-message-selected";
        const string k_MailMessageDeletedClass = "mail-message-deleted";


        // Class selector names
        const string k_IconResourcePath = "GameData/GameIcons";
        const string k_MailMessageAssetPath = "MailMessage"; 

        // Constructor
        public MailboxView(VisualElement topElement) : base(topElement)
        {
            m_MailMessageAsset = Resources.Load<VisualTreeAsset>(k_MailMessageAssetPath);
            m_GameIcons = Resources.Load<GameIconsSO>(k_IconResourcePath);

            m_NewMailIcon = m_GameIcons.newMailIcon;
            m_OldMailIcon = m_GameIcons.oldMailIcon;

            MailEvents.MailboxUpdated += OnMailboxUpdated;
            MailEvents.DeleteClicked += OnDeleteClicked;

        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // Store mail messages under the ScrollView
            m_MailboxContainer = m_TopElement.Q<VisualElement>("unity-content-container");
        }

        public override void Dispose()
        {
            base.Dispose();

            MailEvents.MailboxUpdated -= OnMailboxUpdated;
            MailEvents.DeleteClicked -= OnDeleteClicked;
        }

        void ResetCurrentIndex()
        {
            m_CurrentMessageIndex = 0;
            HighlightFirstMessage();
            MarkMailElementAsRead(GetFirstMailElement());
        }

        // Clears the 

        // Note: use a ListView for optimized performance

        void UpdateMailbox(List<MailMessageSO> messageList, VisualElement container)
        {
            // Clear existing or placeholder mail messages
            container.Clear();

            if (messageList.Count == 0)
                return;

            // Instantiate mail messages to fill the mailbox
            foreach (MailMessageSO msg in messageList)
            {
                if (msg != null)
                    CreateMailMessage(msg, container);
            }

            // Set the index back to the beginning and highlight first message
            ResetCurrentIndex();

            // Update Content with selected index 
            MailEvents.MessageSelected?.Invoke(m_CurrentMessageIndex);
        }

        // process a clicked item in the mailbox
        void ClickMessage(ClickEvent evt)
        {
            // the clicked mail item
            VisualElement clickedElement = evt.currentTarget as VisualElement;

            // highlight and mark the mail message read 
            MarkMailElementAsRead(clickedElement);

            VisualElement backgroundElement = clickedElement.Q(className: k_MailMessageClass);
            HighlightMessage(backgroundElement);

            AudioManager.PlayDefaultButtonSound();

            // Update Content with selected index 
            m_CurrentMessageIndex = clickedElement.parent.IndexOf(clickedElement);
            MailEvents.MessageSelected?.Invoke(m_CurrentMessageIndex);
        }


        // Highlight methods

        // Highlight a given element
        void HighlightMessage(VisualElement elementToHighlight)
        {
            if (elementToHighlight == null)
                return;

            // Deselect all other visuals
            GetAllMailElements().
                Where((element) => element.ClassListContains(k_MailMessageSelectedClass)).
                ForEach(UnhighlightMessage);

            elementToHighlight.AddToClassList(k_MailMessageSelectedClass);
        }

        // Unhighlight a given element
        void UnhighlightMessage(VisualElement elementToUnhighlight)
        {
            if (elementToUnhighlight == null)
                return;

            elementToUnhighlight.RemoveFromClassList(k_MailMessageSelectedClass);
        }

        void HighlightFirstMessage()
        {
            VisualElement firstElement = GetFirstMailElement();
            if (firstElement != null)
            {
                HighlightMessage(firstElement);
            }
        }

        // Generate one mail message and add it to the mailbox container
        void CreateMailMessage(MailMessageSO mailData, VisualElement mailboxContainer)
        {
            if (mailboxContainer == null || mailData == null || m_MailMessageAsset == null)
            {
                return;
            }

            // Instantiate the VisualTreeAsset of the mail message
            // note: this creates an extra TemplateContainer element above the instance
            TemplateContainer instance = m_MailMessageAsset.Instantiate();

            // Assign mail message class to first child of TemplateContainer (the mail message)
            instance.hierarchy[0].AddToClassList(k_MailMessageClass);

            mailboxContainer.Add(instance);
            instance.RegisterCallback<ClickEvent>(ClickMessage);

            // Fill out the date, subject, badge, etc.
            ReadMailData(mailData, instance);

        }

        // Get data from ScriptableObject
        void ReadMailData(MailMessageSO mailData, TemplateContainer instance)
        {
            // read ScriptableObject data
            Label subjectLine = instance.Q<Label>("mail-item__subject");
            subjectLine.text = mailData.SubjectLine;

            Label date = instance.Q<Label>("mail-item__date");
            date.text = mailData.FormattedDate;

            VisualElement badge = instance.Q<VisualElement>("mail-item__badge");
            badge.visible = mailData.IsImportant;

            VisualElement newIcon = instance.Q<VisualElement>("mail-item__new");
            newIcon.style.backgroundImage = (mailData.IsNew) ? new StyleBackground(m_NewMailIcon) : new StyleBackground(m_OldMailIcon);
        }

        // Changes unread icon (NewIcon) to read
        void MarkMailElementAsRead(VisualElement messageElement)
        {
            if (messageElement == null)
                return;

            MailEvents.MarkedAsRead.Invoke(m_CurrentMessageIndex);

            VisualElement newIcon = messageElement.Q<VisualElement>("mail-item__new");
            newIcon.style.backgroundImage = new StyleBackground(m_OldMailIcon);
        }

        // Get all VisualElements with mail message class
        UQueryBuilder<VisualElement> GetAllMailElements()
        {
            return m_TopElement.Query<VisualElement>(className: k_MailMessageClass);
        }

        // Returns the currently selected mail message
        VisualElement GetSelectedMailMessage()
        {
            return m_TopElement.Query<VisualElement>(className: k_MailMessageSelectedClass);
        }

        // Returns the first mail message
        VisualElement GetFirstMailElement()
        {
            return m_MailboxContainer.Query<VisualElement>(className: k_MailMessageClass);
        }

        // Event-handling methods

        void OnMailboxUpdated(List<MailMessageSO> messagesToShow)
        {
            UpdateMailbox(messagesToShow, m_MailboxContainer);

        }

        void OnDeleteClicked()
        {
            VisualElement elemToDelete = GetSelectedMailMessage().parent;
            elemToDelete.AddToClassList(k_MailMessageDeletedClass);
        }
    }
}
