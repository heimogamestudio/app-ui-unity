using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Properties;

namespace UIToolkitDemo
{
    [Serializable]
    public struct CurrencyIcon
    {
        public Sprite icon;
        public CurrencyType currencyType;
    }

    [Serializable]
    public struct ShopItemTypeIcon
    {
        public Sprite icon;
        public ShopItemType shopItemType;
    }

    [Serializable]
    public struct CharacterClassIcon
    {
        public Sprite icon;
        public CharacterClass characterClass;
    }

    [Serializable]
    public struct RarityIcon
    {
        public Sprite icon;
        public Rarity rarity;
    }

    [Serializable]
    public struct AttackTypeIcon
    {
        public Sprite icon;
        public AttackType attackType;
    }

    // returns an icon matching a ShopItem,CurrencyIcon, CharacterClass, Rarity, or AttackType
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Icons", menuName = "UIToolkitDemo/Icons", order = 10)]
    public class GameIconsSO : ScriptableObject
    {
        [Header("Character")]
        public List<CharacterClassIcon> characterClassIcons;
        public List<RarityIcon> rarityIcons;
        public List<AttackTypeIcon> attackTypeIcons;

        [Header("Inventory")]
        public Sprite emptyGearSlotIcon;

        [Header("Shop")]
        public List<CurrencyIcon> currencyIcons;
        public List<ShopItemTypeIcon> shopItemTypeIcons;

        [Header("Mail")]
        public Sprite newMailIcon;
        public Sprite oldMailIcon;

        // Bindable properties for UI data binding
        [CreateProperty]
        public Sprite CharacterClassIcon { get; private set; }

        [CreateProperty]
        public Sprite RarityIcon { get; private set; }

        [CreateProperty]
        public Sprite AttackTypeIcon { get; private set; }
        
        // Method to update the icons based on current character stats
        public void UpdateIcons(CharacterClass charClass, Rarity rarity, AttackType attackType)
        {
            CharacterClassIcon = GetCharacterClassIcon(charClass);
            RarityIcon = GetRarityIcon(rarity);
            AttackTypeIcon = GetAttackTypeIcon(attackType);
        }
        
        public Sprite GetCurrencyIcon(CurrencyType currencyType)
        {
            if (currencyIcons == null || currencyIcons.Count == 0)
                return null;

            CurrencyIcon match = currencyIcons.Find(x => x.currencyType == currencyType);
            return match.icon;
        }

        public Sprite GetShopTypeIcon(ShopItemType shopItemType)
        {
            if (shopItemTypeIcons == null || shopItemTypeIcons.Count == 0)
                return null;

            ShopItemTypeIcon match = shopItemTypeIcons.Find(x => x.shopItemType == shopItemType);
            return match.icon;
        }

        public Sprite GetCharacterClassIcon(CharacterClass charClass)
        {
            if (characterClassIcons == null || characterClassIcons.Count == 0)
                return null;

            CharacterClassIcon match = characterClassIcons.Find(x => x.characterClass == charClass);
            return match.icon;
        }
        
        // get rarity icon
        public Sprite GetRarityIcon(Rarity rarity)
        {
            if (rarityIcons == null || rarityIcons.Count == 0)
                return null;

            RarityIcon match = rarityIcons.Find(x => x.rarity == rarity);
            return match.icon;
        }

        // get attackTypeIcon
        public Sprite GetAttackTypeIcon(AttackType attackType)
        {
            if (rarityIcons == null || rarityIcons.Count == 0)
                return null;

            AttackTypeIcon match = attackTypeIcons.Find(x => x.attackType == attackType);
            return match.icon;
        }

    }
}
