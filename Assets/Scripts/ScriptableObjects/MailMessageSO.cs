using UnityEngine;
using System;
using Unity.Properties;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/MailMessages/MailMessageGameData",
        menuName = "UIToolkitDemo/MailMessage", order = 5)]
    public class MailMessageSO : ScriptableObject
    {
        // Constants
        
        // Max number of character in the subject line
        const int k_MaxSubjectLine = 14;

        // Resource location for sprites/icons
        const string k_ResourcePath = "GameData/GameIcons";
        
        // appears in left 
        [SerializeField] string sender;

        // appears as a title 
        [SerializeField] string subjectLine;

        // format: MM/dd/yyyy
        [SerializeField] string date;

        // body of email text
        [TextArea] [SerializeField] string emailText;

        // image at end of email
        [SerializeField] Sprite emailPicAttachment;

        // footer of email shows a free shopItem
        [SerializeField] uint rewardValue;

        // type of free shopItem
        [SerializeField] ShopItemType rewardType;

        // has the gift been claimed
        [SerializeField] bool isClaimed;

        // important messages show a badge next to sender
        [SerializeField] bool isImportant;

        // has not been read
        [SerializeField] bool isNew;

        // deleted messages appear in the second tab
        [SerializeField] bool isDeleted;
        
        // ScriptableObject pairing icons with currency/shop item types
        GameIconsSO m_GameIconsData;
        
        // Properties

        // appears in left 
        [CreateProperty] public string Sender => sender;

        // appears as a title 
        [CreateProperty]
        public string SubjectLine => string.IsNullOrEmpty(subjectLine)
            ? "..."
            : (subjectLine.Length <= k_MaxSubjectLine
                ? subjectLine
                : subjectLine.Substring(0, k_MaxSubjectLine) + "...");

        [CreateProperty]
        public DateTime Date =>
            (DateTime.TryParse(date, out var parsedDate)) ? parsedDate : DateTime.MinValue;
        
        // format: MM/dd/yyyy
        [CreateProperty] public string FormattedDate => Date.ToString("MM/dd/yyyy");

        // body of email text
        [CreateProperty] public string EmailText => emailText;

        // image at end of email
        [CreateProperty] public Sprite EmailPicAttachment => emailPicAttachment;

        // footer of email shows a free shopItem
        [CreateProperty] public uint RewardValue => rewardValue;

        // type of free shopItem
        [CreateProperty] public ShopItemType RewardType => rewardType;

        // Retrieve the icon based on the rewardType
        [CreateProperty]
        public Sprite RewardIcon
        {
            get
            {
                if (m_GameIconsData != null)
                {
                    return m_GameIconsData.GetShopTypeIcon(rewardType); 
                }
                Debug.LogWarning("[MailMessageSO] RewardIcon: GameIconsSO data not found.");
                return null;
            }
        }
        
        // has the gift been claimed
        [CreateProperty]
        public bool IsClaimed
        {
            get => isClaimed;
            set => isClaimed = value;
        }

        // important messages show a badge next to sender
        [CreateProperty] public bool IsImportant => isImportant;

        // has not been read
        [CreateProperty]
        public bool IsNew
        {
            get => isNew;
            set => isNew = value;
        }

        // deleted messages appear in the second tab
        [CreateProperty]
        public bool IsDeleted
        {
            get => isDeleted;
            set => isDeleted = value;
        }

        // Controls visibility of the gift amount 
        [CreateProperty]
        public DisplayStyle GiftAmountDisplayStyle =>
            !IsClaimed && RewardValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        
        // Controls visibility of the gift icon
        [CreateProperty] public DisplayStyle GiftIconDisplayStyle =>
            !IsClaimed && RewardValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;

        // Controls visibility of the claim button
        [CreateProperty] public DisplayStyle ClaimButtonDisplayStyle =>
            !IsClaimed && RewardValue > 0 && !IsDeleted ? DisplayStyle.Flex : DisplayStyle.None;

        // Controls visibility of the delete button
        [CreateProperty] public DisplayStyle DeleteButtonDisplayStyle =>
            !IsDeleted ? DisplayStyle.Flex : DisplayStyle.None;

        // Controls visibility of the undelete button
        [CreateProperty] public DisplayStyle UndeleteButtonDisplayStyle =>
            IsDeleted ? DisplayStyle.Flex : DisplayStyle.None;

        void OnEnable()
        {
            // Load the icon data from resources (similar to ShopItemSO)
            m_GameIconsData = Resources.Load<GameIconsSO>(k_ResourcePath);
        }
    }
}
