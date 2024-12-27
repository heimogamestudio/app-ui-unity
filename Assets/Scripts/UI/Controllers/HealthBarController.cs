
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(UIDocument))]
    public class HealthBarController : MonoBehaviour
    {

        [Header("HealthBar Elements")]
        [SerializeField] string m_HealthBarName = "HealthBarBase";
        [SerializeField] string m_CharacterName = "Character Name";

        [SerializeField] Vector2 m_WorldSize = new Vector2(1.2f, 0.6f);
        [SerializeField] bool m_ShowStat = true;
        [SerializeField] bool m_ShowNameplate = true;
        [SerializeField] StyleSheet m_StyleSheetOverride;

        [SerializeField] float m_LowHPPercent = 25;
        [SerializeField] Sprite m_LowHPImage;
        [SerializeField] Transform transformToFollow;

        // string IDs
        const string k_NamePlate = "HealthBarTitleBackground";
        const string k_Stat = "HealthBarStat";
        const string k_HPFillImage = "HealthBarProgress";

        HealthBarComponent m_HealthBar;
        StyleBackground m_OriginalHPImage;
        UIDocument m_HealthBarDoc;
        

        void OnEnable()
        {
            MediaQueryEvents.CameraResized += OnCameraResized;
        }

        void OnDisable()
        {
            MediaQueryEvents.CameraResized -= OnCameraResized;
        }

        void HealthBarSetup()
        {
            m_HealthBarDoc = GetComponent<UIDocument>();
            
            VisualElement rootElement = m_HealthBarDoc.rootVisualElement;
            
            if (m_StyleSheetOverride != null)
            {
                rootElement.styleSheets.Clear();
                rootElement.styleSheets.Add(m_StyleSheetOverride);
            }
            
            m_HealthBar = rootElement.Q<HealthBarComponent>(m_HealthBarName);
            m_HealthBar.HealthBarTitle = m_CharacterName;

            m_HealthBar.HealthData = new HealthData();

            ShowNameAndStats(m_ShowNameplate, m_ShowStat);
            MoveToWorldPosition(m_HealthBar, transformToFollow.position, m_WorldSize);

        }

        public void DisplayHealthBar(bool state)
        {
            if (m_HealthBarDoc == null)
                return;

            VisualElement rootElement = m_HealthBarDoc.rootVisualElement;
            rootElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }


        /// <summary>
        /// Setup a health bar
        /// </summary>
        /// <param name="health"></param>
        /// <param name="maxHealth"></param>
        public void SetHealth(int health, int maxHealth)
        {
            if (m_HealthBar == null)
            {
                HealthBarSetup();
            }
            
            m_HealthBar.HealthData.CurrentHealth = health;
            m_HealthBar.HealthData.MaximumHealth = maxHealth;
        }

      
        // Switch health bar sprites when low on HP
        public void UpdateHealth(int health)
        {
            if (m_OriginalHPImage == null)
            {
                // Store the original background style to reset the health bar sprite
                m_OriginalHPImage = m_HealthBar.Q<VisualElement>(k_HPFillImage).style.backgroundImage;
            }

            float lowHealth = m_HealthBar.HealthData.MaximumHealth * m_LowHPPercent / 100;
            VisualElement fill = m_HealthBar.Q<VisualElement>(k_HPFillImage);

            if (health < lowHealth && m_LowHPImage != null)
            {
                fill.style.backgroundImage = new StyleBackground(m_LowHPImage);
            }
            else
            {
                fill.style.backgroundImage = m_OriginalHPImage;
            }

            m_HealthBar.HealthData.CurrentHealth = health;
        }

        void ShowNameAndStats(bool nameVisible, bool statVisible)
        {
            VisualElement nameplate = m_HealthBar.Q<VisualElement>(k_NamePlate);
            VisualElement stat = m_HealthBar.Q<Label>(k_Stat);

            if (nameplate != null)
            {
                nameplate.visible = nameVisible;
            }

            if (stat != null)
            {
                stat.visible = statVisible;
            }
 
        }

        // moves health bar to match world position
        void MoveToWorldPosition(VisualElement element, Vector3 worldPosition, Vector2 worldSize)
        {
            Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, worldSize, Camera.main);
            element.transform.position = rect.position;

        }

        // Refresh health bar setup when camera updates
        void OnCameraResized()
        {
            UpdateHealthBar();
        }

        void UpdateHealthBar()
        {
            ShowNameAndStats(m_ShowNameplate, m_ShowStat);
            MoveToWorldPosition(m_HealthBar, transformToFollow.position, m_WorldSize);
        }

        void LateUpdate()
        {
            UpdateHealthBar();
        }
    }
}
