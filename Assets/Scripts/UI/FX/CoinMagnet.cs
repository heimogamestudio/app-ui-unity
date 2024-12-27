using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace UIToolkitDemo
{
    [Serializable]
    public struct MagnetData
    {
        // gold, gems, health potion, power potion
        public ShopItemType ItemType;

        // ParticleSystem pool
        public ObjectPoolBehaviour FXPool;

        // forcefield target
        public ParticleSystemForceField ForceField;
    }

    // FX generator when buying an item from the shop
    public class CoinMagnet : MonoBehaviour
    {

        [Header("UI Elements")]
        [Tooltip("Locate screen space positions from this document's UI elements.")]
        [SerializeField] UIDocument m_Document;
        [Tooltip("Match a target VisualElement by name to each ShopItemType")]
        [SerializeField] List<MagnetData> m_MagnetData;

        [Header("Camera")]
        [Tooltip("Use Camera and Depth to calculate world space positions.")]
        [SerializeField] Camera m_Camera;
        [SerializeField] float m_ZDepth = 10f;
        [Tooltip("3D offset to the particle emission")]
        [SerializeField] Vector3 m_SourceOffset = new Vector3(0f, 0.1f, 0f);

        // start and end coordinates for effect
        void OnEnable()
        {
            // FX from ShopScreen
            ShopEvents.TransactionProcessed += OnTransactionProcessed;

            // FX from MailScreen
            ShopEvents.RewardProcessed += OnRewardProcessed;

            ThemeEvents.CameraUpdated += OnCameraUpdated;
        }



        void OnDisable()
        {
            ShopEvents.TransactionProcessed -= OnTransactionProcessed;
            ShopEvents.RewardProcessed -= OnRewardProcessed;

            ThemeEvents.CameraUpdated += OnCameraUpdated;
        }

        ObjectPoolBehaviour GetFXPool(ShopItemType itemType)
        {
            MagnetData magnetData = m_MagnetData.Find(x => x.ItemType == itemType);
            return magnetData.FXPool;
        }

        ParticleSystemForceField GetForcefield(ShopItemType itemType)
        {
            MagnetData magnetData =  m_MagnetData.Find(x => x.ItemType == itemType);
            return magnetData.ForceField;
        }
        
        void PlayPooledFX(Vector2 screenPos, ShopItemType contentType)
        {
            Vector3 worldPos = screenPos.ScreenPosToWorldPos(m_Camera, m_ZDepth) + m_SourceOffset;

            ObjectPoolBehaviour fxPool = GetFXPool(contentType);

            // Initialize ParticleSystem
            ParticleSystem ps = fxPool.GetPooledObject().GetComponent<ParticleSystem>();

            if (ps == null)
                return;

            ps.gameObject.SetActive(true);
            ps.gameObject.transform.position = worldPos;

            // Add the Forcefield for destination
            ParticleSystemForceField forceField = GetForcefield(contentType);
            forceField.gameObject.SetActive(true);

            // Update the ForceField position relative to the UI
            PositionToVisualElement positionToVisualElement = forceField.gameObject.GetComponent<PositionToVisualElement>();
            positionToVisualElement.MoveToElement();

            // Attach the ForceField to the particle system
            ParticleSystem.ExternalForcesModule externalForces = ps.externalForces;
            externalForces.enabled = true;
            externalForces.AddInfluence(forceField);

            ps.Play();

        }

        // event-handling methods

        // claiming free reward from MailScreen
        void OnRewardProcessed(ShopItemType rewardType, uint rewardQuantity, Vector2 screenPos)
        {
            // only play effect for gold or gem purchases
            if (rewardType == ShopItemType.HealthPotion || rewardType == ShopItemType.LevelUpPotion)
                return;

            PlayPooledFX(screenPos, rewardType);
        }

        // buying an item from the ShopScreen
        void OnTransactionProcessed(ShopItemSO shopItem, Vector2 screenPos)
        {
            // only play effect for gold or gem purchases
            if (shopItem.ContentType == ShopItemType.HealthPotion || shopItem.ContentType == ShopItemType.LevelUpPotion)
                return;

            PlayPooledFX(screenPos, shopItem.ContentType);
        }

        void OnCameraUpdated(Camera camera)
        {
            m_Camera = camera;
        }
    }
}
