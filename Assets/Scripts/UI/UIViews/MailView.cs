using UnityEngine.UIElements;

namespace UIToolkitDemo
{

    /// <summary>
    /// This UI demonstrates how to display text stored in multiple ScriptableObject
    /// assets, simulating an email client. This manages the lifecycle of three other
    /// UIViews: MailboxView, MailContentView, and the MailTabView
    /// 
    /// </summary>
    public class MailView : UIView
    {

        // UI consists of three parts: tab buttons for Inbox/Deleted, a scrollview to show the messages, and
        // a content window. Each 

        VisualElement m_MailboxContainer;
        VisualElement m_ContentContainer;
        VisualElement m_TabContainer;

        MailboxView m_MailboxView;
        MailContentView m_MailContentView;
        MailTabView m_MailTabView;

        public MailView(VisualElement topElement): base(topElement)
        {
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_MailboxContainer = m_TopElement.Q<VisualElement>("mailbox__container");
            m_ContentContainer = m_TopElement.Q<VisualElement>("content__container");
            m_TabContainer = m_TopElement.Q<VisualElement>("tabs__container");
        }

        // Set up the sub-displays (MailboxView, MailContentView, MailTabView)
        public override void Initialize()
        {
            base.Initialize();

            // Displays tab buttons to select 
            m_MailTabView = new MailTabView(m_TabContainer);
            m_MailTabView.Show();

            // Display a list of messages
            m_MailboxView = new MailboxView(m_MailboxContainer);
            m_MailboxView.Show();

            // Displays the contents of a selected mail message
            m_MailContentView = new MailContentView(m_ContentContainer);
            m_MailContentView.Show();
        }

        // Dispose of all sub-displays
        public override void Dispose()
        {
            base.Dispose();
            m_MailboxView.Dispose();
            m_MailContentView.Dispose();
            m_MailTabView.Dispose();
        }

    }
}