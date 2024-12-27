using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class MailTabView : UIView
    {

        VisualElement m_InboxTab;
        VisualElement m_DeletedTab;


        //const string k_TabClassName = "mailtab";  // Tab element ID
        const string k_SelectedTabClassName = "selected-mailtab";  // Highlighted tab selector

        public MailTabView(VisualElement topElement) : base(topElement)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
            MailEvents.TabSelected?.Invoke(m_InboxTab.name);
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_InboxTab = m_TopElement.Q("tabs__inbox-tab");
            m_DeletedTab = m_TopElement.Q("tabs__deleted-tab");
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            m_InboxTab.RegisterCallback<ClickEvent>(SelectInboxTab);
            m_DeletedTab.RegisterCallback<ClickEvent>(SelectDeletedTab);
        }

        // Select the deleted messages mailbox tab
        void SelectDeletedTab(ClickEvent evt)
        {
     
            m_InboxTab.RemoveFromClassList(k_SelectedTabClassName);
            m_DeletedTab.AddToClassList(k_SelectedTabClassName);

            // Send the tab name to the Controller
            VisualElement clickedTab = evt.currentTarget as VisualElement;
            MailEvents.TabSelected(clickedTab.name);


        }

        // Select the inbox mailbox tab
        void SelectInboxTab(ClickEvent evt)
        {

            m_DeletedTab.RemoveFromClassList(k_SelectedTabClassName);
            m_InboxTab.AddToClassList(k_SelectedTabClassName);

            // Send the tab name to the Controller
            VisualElement clickedTab = evt.currentTarget as VisualElement;
            MailEvents.TabSelected(clickedTab.name);
            

        }

        public override void Dispose()
        {
            base.Dispose();

            // Optional: garbage collector will clean up automatically unless
            // referencing an external object

            m_InboxTab.UnregisterCallback<ClickEvent>(SelectInboxTab);
            m_DeletedTab.UnregisterCallback<ClickEvent>(SelectDeletedTab);
        }
    }
}
