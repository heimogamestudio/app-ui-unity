using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class DebugLogView : MonoBehaviour
    {
        [SerializeField] private UIDocument m_UILogDocument;  // Your UI Document that contains the UI
        private Label m_LogLabel;
        private const int MaxLogs = 10;  // Maximum number of log lines to show
        private string[] m_LogMessages = new string[MaxLogs]{ "", "", "", "", "", "", "", "", "", "" };  // Store the log messages
        private int m_LogIndex = 0;
        
        void OnEnable()
        {
            // Subscribe to the logMessageReceived event
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            if (m_LogLabel != null)
                m_LogLabel.text = string.Empty;
            // Unsubscribe when the script is disabled
            Application.logMessageReceived -= HandleLog;
        }
        
        void Start()
        {
            // Find the Label element in the UI to display the logs
            var rootElement = m_UILogDocument.rootVisualElement;
            m_LogLabel = rootElement.Q<Label>("log__label"); 
            
            UpdateLogUI();
        }
        
        // Method that gets called every time a log is created
        void HandleLog(string logString, string stackTrace, LogType type)
        {
            // Add the new log message to the array
            m_LogMessages[m_LogIndex] = logString;
            m_LogIndex = (m_LogIndex + 1) % MaxLogs;  // Circular buffer

            // Update the UI with the latest logs
            UpdateLogUI();
        }

        // Update the UI to show the latest log messages
        void UpdateLogUI()
        {
            // Join all the logs into a single string and update the Label text
            string combinedLogs = string.Join("\n", m_LogMessages);
            if (m_LogLabel != null)
            {
                m_LogLabel.text = combinedLogs;
            }
        }
    }
}
