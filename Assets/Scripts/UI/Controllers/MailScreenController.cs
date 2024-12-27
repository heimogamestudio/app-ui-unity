using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UIToolkitDemo
{
    // Presenter/Controller for simulated mail messages
    // Contains non-UI logic and sends data from the ScriptableObjects to the MailScreen.
    public class MailController : MonoBehaviour
    {
        const string k_InboxTabName = "tabs__inbox-tab";
        const string k_DeletedTabName = "tabs__deleted-tab";
        const string k_ResourcePath = "GameData/MailMessages";

        string m_SelectedTab;
        // mail messages stored as ScriptableObjects to simulate mail data
        List<MailMessageSO> m_MailMessages = new List<MailMessageSO>();
        List<MailMessageSO> m_InboxMessages = new List<MailMessageSO>();
        List<MailMessageSO> m_DeletedMessages = new List<MailMessageSO>();

        void OnEnable()
        {
            MailEvents.MarkedAsRead += OnMarkedAsRead;
            MailEvents.ClaimRewardClicked += OnClaimReward;
            MailEvents.MessageDeleted += OnDeleteMessage;
            MailEvents.UndeleteClicked += OnUndeleteMessage;

            MailEvents.TabSelected += OnTabSelected;
            MailEvents.MessageSelected += OnMessageSelected;
        }

        void OnDisable()
        {
            MailEvents.MarkedAsRead -= OnMarkedAsRead;
            MailEvents.ClaimRewardClicked -= OnClaimReward;
            MailEvents.MessageDeleted -= OnDeleteMessage;
            MailEvents.UndeleteClicked -= OnUndeleteMessage;

            MailEvents.TabSelected -= OnTabSelected;
            MailEvents.MessageSelected -= OnMessageSelected;
        }

        void Start()
        {
            LoadMailMessages();

            m_SelectedTab = k_InboxTabName;
            UpdateView();
        }

        void LoadMailMessages()
        {
            m_MailMessages.Clear();

            // Load the ScriptableObjects from the Resources directory (default = Resources/GameData/MailMessages)
            m_MailMessages.AddRange(Resources.LoadAll<MailMessageSO>(k_ResourcePath));

            // Separate lists for easier display
            m_InboxMessages = m_MailMessages.Where(x => !x.IsDeleted).ToList();
            m_DeletedMessages = m_MailMessages.Where(x => x.IsDeleted).ToList();
        }

        // Show the mailboxes in the MailScreen interface
        void UpdateView()
        {
            // Sort and generate elements from MailScreen
            m_InboxMessages = SortMailbox(m_InboxMessages);
            m_DeletedMessages = SortMailbox(m_DeletedMessages);

            UpdateMailboxByTab(m_SelectedTab);
            ShowMessage(0);
        }

        // Choose the mailbox (Inbox or Deleted) with a given tab name
        void UpdateMailboxByTab(string tabName)
        {
            if (tabName == k_InboxTabName)
            {
                MailEvents.MailboxUpdated?.Invoke(m_InboxMessages);
            }
            else if (tabName == k_DeletedTabName)
            {
                MailEvents.MailboxUpdated?.Invoke(m_DeletedMessages);
            }
            else
            {
                Debug.LogWarning($"[MailScreenController] UpdateMailboxByTab: invalid tab name {tabName}");
            }
        }

        // Order messages by validated Date property
        List<MailMessageSO> SortMailbox(List<MailMessageSO> originalList)
        {
            return originalList.OrderBy(x => x.Date).Reverse().ToList();
        }

        //Returns one mail message from the List of mail messages by index

        MailMessageSO GetInboxMessage(int index)
        {
            return GetMessageByIndex(m_InboxMessages, index);
        }

        MailMessageSO GetDeletedMessage(int index)
        {
            return GetMessageByIndex(m_DeletedMessages, index);
        }

        MailMessageSO GetMessageByIndex(List<MailMessageSO> selectedList, int index)
        {
            if (index < 0 || index >= selectedList.Count)
                return null;

            return selectedList[index];
        }

        // Change mail message icon from new to old
        void MarkMessageAsRead(int indexToRead)
        {
            MailMessageSO msgToRead = GetInboxMessage(indexToRead);

            if (msgToRead != null && msgToRead.IsNew)
            {
                msgToRead.IsNew = false;
            }
        }

        void DeleteMessage(int indexToDelete)
        {
            MailMessageSO msgToDelete = GetInboxMessage(indexToDelete);

            if (msgToDelete == null)
                return;

            // mark as deleted move from Inbox to Deleted List
            msgToDelete.IsDeleted = true;
            m_DeletedMessages.Add(msgToDelete);
            m_InboxMessages.Remove(msgToDelete);

            // rebuild the interface
            UpdateView();
        }

        // Event-handling methods

        void OnDeleteMessage(int index)
        {
            DeleteMessage(index);
        }

        void OnUndeleteMessage(int indexToUndelete)
        {
            MailMessageSO msgToUndelete = GetDeletedMessage(indexToUndelete);

            if (msgToUndelete == null)
                return;

            msgToUndelete.IsDeleted = false;
            m_DeletedMessages.Remove(msgToUndelete);
            m_InboxMessages.Add(msgToUndelete);

            // rebuild the interface
            UpdateView();
        }


        void OnClaimReward(int indexToClaim, Vector2 screenPos)
        {
            MailMessageSO messageWithReward = GetInboxMessage(indexToClaim);

            if (messageWithReward == null)
                return;

            MailEvents.RewardClaimed?.Invoke(messageWithReward, screenPos);

            messageWithReward.IsClaimed = true;
        }

        void OnMarkedAsRead(int index)
        {
            MarkMessageAsRead(index);
        }

        void OnTabSelected(string tabName)
        {
            m_SelectedTab = tabName;
            UpdateView();
        }

        void OnMessageSelected(int index)
        {
            ShowMessage(index);
        }

        void ShowMessage(int index)
        {
            List<MailMessageSO> selectedList = (m_SelectedTab == k_InboxTabName) ? m_InboxMessages : m_DeletedMessages;

            // Find the mail message by index
            MailMessageSO messageToShow = GetMessageByIndex(selectedList, index);

            // Send the message to the MailContentView if valid
            if (messageToShow != null)
                MailEvents.MessageShown?.Invoke(messageToShow);

            // Otherwise, show "No message selected"
            else
                MailEvents.ShowEmptyMessage?.Invoke();
        }


    }
}