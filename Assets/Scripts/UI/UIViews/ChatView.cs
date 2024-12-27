using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

namespace UIToolkitDemo
{
    /// <summary>
    /// UI that shows text messages on the Home Screen to simulate a chat window.
    /// </summary>
    public class ChatView : UIView
    {
        // Use async await instead of coroutines (non-MonoBehaviour)
        const int k_DelayBetweenKeys = 20; // milliseconds
        const int k_DelayBetweenLines = 1000; // milliseconds

        Label m_ChatText;

        // Chat name color
        const string k_TagOpen = "<color=green>";
        const string k_TagClose = "</color>";


        public ChatView(VisualElement topElement) : base(topElement)
        {
            HomeEvents.ChatWindowShown += OnShowChats;
        }

        public override void Dispose()
        {
            base.Dispose();
            HomeEvents.ChatWindowShown -= OnShowChats;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_ChatText = m_TopElement.Q<Label>("home-chat__text");
        }

        void OnShowChats(List<ChatSO> chatData)
        {
            if (m_ChatText == null)
                return;

            // Fire and forget task
            _ = ChatRoutine(chatData); 
        }


        // Iterate through the chat messages and show one at a time
        async Task ChatRoutine(List<ChatSO> chatData)
        {
            while (true) // repeat ad infinitum
            {
                foreach (ChatSO chatObject in chatData)
                {
                    await AnimateMessageAsync(chatObject.chatname, chatObject.message);
                    await Task.Delay(k_DelayBetweenLines);
                }
            }
        }

        // Increment the UI Element text with a small delay between each character
        async Task AnimateMessageAsync(string chatName, string message)
        {
            m_ChatText.text = string.Empty;
            m_ChatText.text = k_TagOpen + " (" + chatName + ")" + k_TagClose + " ";

            foreach (char c in message.ToCharArray())
            {
                await Task.Delay(k_DelayBetweenKeys);
                m_ChatText.text += c;
            }
        }
    }
}

