using UnityEngine;
using Unity.Properties;


namespace UIToolkitDemo
{
    /// <summary>
    /// Stores consumable data / player settings.
    /// </summary>
    [System.Serializable]
    public class GameData
    {
        [SerializeField] uint m_Gold = 500;
        [SerializeField] uint m_Gems = 50;
        [SerializeField] uint m_HealthPotions = 6;
        [SerializeField] uint m_LevelUpPotions = 80;

        [SerializeField] string m_UserName;
        
        [SerializeField] bool m_IsToggled;
        [SerializeField] string m_Theme;
        
        [SerializeField] string m_LanguageSelection;
        [SerializeField] float m_MusicVolume;
        [SerializeField] float m_SfxVolume;
        
        [SerializeField] bool m_IsFpsCounterEnabled;
        [SerializeField] int m_TargetFrameRateSelection;

        // Properties with simple data binding
        [CreateProperty]
        public uint Gold
        {
            get => m_Gold;
            set => m_Gold = value;
        }

        [CreateProperty]
        public uint Gems
        {
            get => m_Gems;
            set => m_Gems = value;
        }

        [CreateProperty]
        public uint HealthPotions
        {
            get => m_HealthPotions;
            set => m_HealthPotions = value;
        }

        [CreateProperty]
        public uint LevelUpPotions
        {
            get => m_LevelUpPotions;
            set => m_LevelUpPotions = value;
        }

        [CreateProperty]
        public string UserName
        {
            get => m_UserName;
            set => m_UserName = value;
        }

        [CreateProperty]
        public bool IsToggled
        {
            get => m_IsToggled;
            set => m_IsToggled = value;
        }

        [CreateProperty]
        public string Theme
        {
            get => m_Theme;
            set => m_Theme = value;
        }

        [CreateProperty]
        public string LanguageSelection
        {
            get => m_LanguageSelection;
            set => m_LanguageSelection = value;
        }

        [CreateProperty]
        public float MusicVolume
        {
            get => m_MusicVolume;
            set => m_MusicVolume = value;
        }

        [CreateProperty]
        public float SfxVolume
        {
            get => m_SfxVolume;
            set => m_SfxVolume = value;
        }

        [CreateProperty]
        public bool IsFpsCounterEnabled
        {
            get => m_IsFpsCounterEnabled;
            set => m_IsFpsCounterEnabled = value;
        }

        [CreateProperty]
        public int TargetFrameRateSelection
        {
            get => m_TargetFrameRateSelection;
            set => m_TargetFrameRateSelection = value;
        }
        
        /// <summary>
        /// Constructor with starting values.
        /// </summary>
        // 
        public GameData()
        {
            // Player stats/data
            this.m_Gold = 500;
            this.m_Gems = 50;
            this.m_HealthPotions = 6;
            this.m_LevelUpPotions = 200;

            // Settings

            this.m_UserName = "GUEST_123456";
            this.m_IsToggled = false;
            this.m_Theme = "Default";
            
            this.m_LanguageSelection = "Item1";
            this.m_MusicVolume = 80f;
            this.m_SfxVolume = 80f;
            
            this.m_IsFpsCounterEnabled = false;
            this.m_TargetFrameRateSelection = 0;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadJson(string jsonFilepath)
        {
            JsonUtility.FromJsonOverwrite(jsonFilepath, this);
        }
    }
}