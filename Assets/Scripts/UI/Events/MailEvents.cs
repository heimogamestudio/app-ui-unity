using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// Public static delegates associated with the MailScreen/MailScreenController.
    ///
    /// Note: these are "events" in the conceptual sense and not the strict C# sense.
    /// </summary>
    public static class MailEvents
    {
        // Select a tab button in MailTabView
        public static Action<string> TabSelected;

        // Update the mailbox with a list of mail messages (Inbox or Deleted)
        public static Action<List<MailMessageSO>> MailboxUpdated;

        // Changed the icon of a mail message in the MailboxView
        public static Action<int> MarkedAsRead;

        // Selected a mail message in the Mailbox by index
        public static Action<int> MessageSelected;

        // Show label that no message is available
        public static Action ShowEmptyMessage;

        // Show a specific mail message in the MailContent
        public static Action<MailMessageSO> MessageShown;

        // Play back an coin/gem effect from a mail message
        public static Action<MailMessageSO, Vector2> RewardClaimed;

        public static Action DeleteClicked;
        public static Action<int> MessageDeleted;

        public static Action<int> UndeleteClicked;

        public static Action<int, Vector2> ClaimRewardClicked;

    }
}
