using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // High-level manager for the various parts of the Main Menu UI. Here we use one master UXML and one UIDocument.
 
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {

        UIDocument m_MainMenuDocument;

        UIView m_CurrentView;
        UIView m_PreviousView;

        // List of all UIViews
        List<UIView> m_AllViews = new List<UIView>();

        // Modal screens
        UIView m_HomeView;  // Landing screen
        UIView m_CharView;  // Character screen
        UIView m_InfoView;  // Resource links for more information
        UIView m_ShopView;  // Shop screen for gold/gem/potions
        UIView m_MailView;  // Mail screen

        // Overlay screens
        UIView m_InventoryView;  // Inventory from character screen
        UIView m_SettingsView;  // Overlay screen for settings

        // Toolbars
        UIView m_OptionsBarView;  // Quick access to gold/gem and Settings
        UIView m_MenuBarView;  // Navigation bar for menu screens
        UIView m_LevelMeterView;  // Radial progress bar that show total character progression

        // VisualTree string IDs for UIViews; each represents one branch of the tree
        const string k_HomeViewName = "HomeScreen";
        const string k_InfoViewName = "InfoScreen";
        const string k_CharViewName = "CharScreen";
        const string k_ShopViewName = "ShopScreen";
        const string k_MailViewName = "MailScreen";
        const string k_InventoryViewName = "InventoryScreen";
        const string k_SettingsViewName = "SettingsScreen";
        const string k_OptionsBarViewName = "OptionsBar";
        const string k_MenuBarViewName = "MenuBar";
        const string k_LevelMeterViewName = "LevelMeter";
        
        void OnEnable()
        {
            m_MainMenuDocument = GetComponent<UIDocument>();
 
            SetupViews();
            
            SubscribeToEvents();
      
            // Start with the home screen
            ShowModalView(m_HomeView);

        }

        void SubscribeToEvents()
        {
            MainMenuUIEvents.HomeScreenShown += OnHomeScreenShown;
            MainMenuUIEvents.CharScreenShown += OnCharScreenShown;
            MainMenuUIEvents.InfoScreenShown += OnInfoScreenShown;
            MainMenuUIEvents.ShopScreenShown += OnShopScreenShown;
            MainMenuUIEvents.MailScreenShown += OnMailScreenShown;

            MainMenuUIEvents.InventoryScreenShown += OnInventoryScreenShown;
            MainMenuUIEvents.InventoryScreenHidden += OnInventoryScreenHidden;
            MainMenuUIEvents.SettingsScreenShown += OnSettingsScreenShown;
            MainMenuUIEvents.SettingsScreenHidden += OnSettingsScreenHidden;
        }

        void OnDisable()
        {
            UnsubscribeFromEvents();

            foreach (UIView view in m_AllViews)
            {
                view.Dispose();
            }
        }

        void UnsubscribeFromEvents()
        {
            MainMenuUIEvents.HomeScreenShown -= OnHomeScreenShown;
            MainMenuUIEvents.CharScreenShown -= OnCharScreenShown;
            MainMenuUIEvents.InfoScreenShown -= OnInfoScreenShown;
            MainMenuUIEvents.ShopScreenShown -= OnShopScreenShown;
            MainMenuUIEvents.MailScreenShown -= OnMailScreenShown;

            MainMenuUIEvents.InventoryScreenShown -= OnInventoryScreenShown;
            MainMenuUIEvents.InventoryScreenHidden -= OnInventoryScreenHidden;
            MainMenuUIEvents.SettingsScreenShown -= OnSettingsScreenShown;
            MainMenuUIEvents.SettingsScreenHidden -= OnSettingsScreenHidden;
        }
        
        void Start()
        {
            Time.timeScale = 1f;
        }

        void SetupViews()
        {
            VisualElement root = m_MainMenuDocument.rootVisualElement;

            // Create full-screen modal views: HomeView, CharView, InfoView, ShopView, MailView
            m_HomeView = new HomeView(root.Q<VisualElement>(k_HomeViewName)); // Landing modal screen
            m_CharView = new CharView(root.Q<VisualElement>(k_CharViewName)); // Character screen
            m_InfoView = new InfoView(root.Q<VisualElement>(k_InfoViewName)); // Links and resources screen
            m_ShopView = new ShopView(root.Q<VisualElement>(k_ShopViewName)); // Shop screen
            m_MailView = new MailView(root.Q<VisualElement>(k_MailViewName)); // Mail screen

            // Overlay views (popup modal with background)
            m_InventoryView = new InventoryView(root.Q<VisualElement>(k_InventoryViewName));  // Gear equipment overlay
            m_SettingsView = new SettingsView(root.Q<VisualElement>(k_SettingsViewName)); // Game settings overlay

            // Toolbars 
            LevelMeterData meterData = CharEvents.GetLevelMeterData.Invoke();
            m_LevelMeterView = new LevelMeterView(root.Q<VisualElement>(k_LevelMeterViewName), meterData); // Radial level meter
            m_LevelMeterView.Initialize();
            
            m_OptionsBarView = new OptionsBarView(root.Q<VisualElement>(k_OptionsBarViewName)); // Settings/Shop toolbar
            m_MenuBarView = new MenuBarView(root.Q<VisualElement>(k_MenuBarViewName)); // Screen selection toolbar
            
            // Track modal UI Views in a List for disposal 
            m_AllViews.Add(m_HomeView);
            m_AllViews.Add(m_CharView);
            m_AllViews.Add(m_InfoView);
            m_AllViews.Add(m_ShopView);
            m_AllViews.Add(m_MailView);
            m_AllViews.Add(m_InventoryView);
            m_AllViews.Add(m_SettingsView);
            m_AllViews.Add(m_LevelMeterView);
            m_AllViews.Add(m_OptionsBarView);
            m_AllViews.Add(m_MenuBarView);

            // UI Views enabled by default
            m_HomeView.Show();
            m_OptionsBarView.Show();
            m_MenuBarView.Show();
            m_LevelMeterView.Show();
        }

        // Toggle modal screens on/off
        void ShowModalView(UIView newView)
        {
            if (m_CurrentView != null)
                m_CurrentView.Hide();

            m_PreviousView = m_CurrentView;
            m_CurrentView = newView;

            // Show the screen and notify any listeners that the main menu has updated

            if (m_CurrentView != null)
            {
                m_CurrentView.Show();
                MainMenuUIEvents.CurrentViewChanged?.Invoke(m_CurrentView.GetType().Name);
            }
        }

        // Modal screen methods. 
        void OnHomeScreenShown()
        {
            ShowModalView(m_HomeView);
        }

        void OnCharScreenShown()
        {
            ShowModalView(m_CharView);
        }

        void OnInfoScreenShown()
        {
            ShowModalView(m_InfoView);
        }

        void OnShopScreenShown()
        {
            ShowModalView(m_ShopView);
        }

        void OnMailScreenShown()
        {
            ShowModalView(m_MailView);
        }

        // Overlay Screen Methods. These open up modal UIViews but with a reference to the previous screen.

        void OnSettingsScreenShown()
        {

            m_PreviousView = m_CurrentView;
            m_SettingsView.Show();
        }

        void OnInventoryScreenShown()
        {
            m_PreviousView = m_CurrentView;
            m_InventoryView.Show();
        }

        void OnSettingsScreenHidden()
        {
            m_SettingsView.Hide();

            if (m_PreviousView != null)
            {
                m_PreviousView.Show();
                m_CurrentView = m_PreviousView;
                MainMenuUIEvents.CurrentViewChanged?.Invoke(m_CurrentView.GetType().Name);
            }
        }

        void OnInventoryScreenHidden()
        {
            // Hide the Inventory screen
            m_InventoryView.Hide();

            // Update the current screen to the previous screen
            if (m_PreviousView != null)
            {
                m_PreviousView.Show();
                m_CurrentView = m_PreviousView;
                MainMenuUIEvents.CurrentViewChanged?.Invoke(m_CurrentView.GetType().Name);
            }
        }
    }
}