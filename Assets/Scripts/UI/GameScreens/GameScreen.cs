using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using System.Linq;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(UIDocument))]
    public class GameScreen : MonoBehaviour
    {

        [Header("Menu Screen elements")][Tooltip("String IDs to query Visual Elements")]
        [SerializeField] string m_PauseScreenName = "PauseScreen";
        [SerializeField] string m_WinScreenName = "GameWinScreen";
        [SerializeField] string m_LoseScreenName = "GameLoseScreen";

        [Header("Blur")]
        [SerializeField] Volume m_Volume;

        const float k_DelayWinScreen = 2f;

        // string IDs
        // references to functional UI elements (buttons and screens)
        VisualElement m_PauseScreen;
        VisualElement m_WinScreen;
        VisualElement m_LoseScreen;
        VisualElement m_CharPortraitContainer;

        Slider m_MusicSlider;
        Slider m_SfxSlider;

        Button m_PauseButton;
        Button m_PauseResumeButton;
        Button m_PauseQuitButton;
        Button m_PauseBackButton;

        Button m_WinNextButton;
        Button m_LoseQuitButton;
        Button m_LoseRetryButton;

        UIDocument m_GameScreen;

        bool m_IsGameOver;

        void OnEnable()
        {
            SetVisualElements();
            RegisterButtonCallbacks();

            if (m_Volume == null)
                m_Volume = FindFirstObjectByType<Volume>();

            GameplayEvents.WinScreenShown += OnGameWon;
            GameplayEvents.LoseScreenShown += OnGameLost;

            GameplayEvents.CharacterCardHidden += OnHideCharacterCard;
            GameplayEvents.SettingsUpdated += OnSettingsUpdated;

            UnitController.SpecialCharged += OnSpecialCharged;
            UnitController.SpecialDischarged += OnSpecialDischarged;
        }

        void OnDisable()
        {
            GameplayEvents.WinScreenShown -= OnGameWon;
            GameplayEvents.LoseScreenShown -= OnGameLost;

            GameplayEvents.CharacterCardHidden -= OnHideCharacterCard;
            GameplayEvents.SettingsUpdated -= OnSettingsUpdated;

            UnitController.SpecialCharged -= OnSpecialCharged;
            UnitController.SpecialDischarged -= OnSpecialDischarged;
        }
  
        void SetVisualElements()
        {
            m_GameScreen = GetComponent<UIDocument>();
            VisualElement rootElement = m_GameScreen.rootVisualElement;

            m_PauseScreen = rootElement.Q(m_PauseScreenName);
            m_WinScreen = rootElement.Q(m_WinScreenName);
            m_LoseScreen = rootElement.Q(m_LoseScreenName);

            m_PauseButton = rootElement.Q<Button>("pause__button");
            m_PauseResumeButton = rootElement.Q<Button>("pause__resume-button");
            m_PauseQuitButton = rootElement.Q<Button>("pause__quit-button");
            m_PauseBackButton = rootElement.Q<Button>("pause__back-button");

            m_WinNextButton = rootElement.Q<Button>("game-win__next-button");
            m_LoseQuitButton = rootElement.Q<Button>("game-lose__quit-button");
            m_LoseRetryButton = rootElement.Q<Button>("game-lose__retry-button");
            m_CharPortraitContainer = rootElement.Q<VisualElement>("game-char__container");

            m_MusicSlider = rootElement.Q<Slider>("pause__music-slider");
            m_SfxSlider = rootElement.Q<Slider>("pause__sfx-slider");
        }

        void RegisterButtonCallbacks()
        {
            // set up buttons with RegisterCallback
            m_PauseButton.RegisterCallback<ClickEvent>(ShowPauseScreen);
            m_PauseResumeButton.RegisterCallback<ClickEvent>(ResumeGame);
            m_PauseBackButton.RegisterCallback<ClickEvent>(ResumeGame);
            m_PauseQuitButton.RegisterCallback<ClickEvent>(QuitGame);

            m_WinNextButton.RegisterCallback<ClickEvent>(QuitGame);
            m_LoseQuitButton.RegisterCallback<ClickEvent>(QuitGame);
            m_LoseRetryButton.RegisterCallback<ClickEvent>(RestartGame);

            m_MusicSlider.RegisterValueChangedCallback(ChangeMusicVolume);
            m_SfxSlider.RegisterValueChangedCallback(ChangeSfxVolume);
        }

        void Start()
        {
            BlurBackground(false);
        }

        void ShowVisualElement(VisualElement visualElement, bool state)
        {
            if (visualElement == null)
                return;

            visualElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // add the character portrait to the container
        public void AddHero(CharacterCard card)
        {
            if (m_CharPortraitContainer == null)
            {
                SetVisualElements();
            }

            m_CharPortraitContainer.Add(card.CharacterTemplate);
            card.CharacterTemplate.pickingMode = PickingMode.Ignore;
            EnableFrameFX(card.CharacterTemplate, false);
        }

        void ShowPauseScreen(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            GameplayEvents.GamePaused?.Invoke(1f);
            
            ShowVisualElement(m_PauseScreen, true);
            ShowVisualElement(m_PauseButton, false);

            BlurBackground(true);

            m_CharPortraitContainer.style.display = DisplayStyle.None;
        }

        void RestartGame(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            GameplayEvents.GameRestarted?.Invoke();
        }
        void QuitGame(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();
            GameplayEvents.GameQuit?.Invoke();
        }

        void ResumeGame(ClickEvent evt)
        {
            GameplayEvents.GameResumed?.Invoke();
            AudioManager.PlayDefaultButtonSound();
            ShowVisualElement(m_PauseScreen, false);
            ShowVisualElement(m_PauseButton, true);
            BlurBackground(false);

            m_CharPortraitContainer.style.display = DisplayStyle.Flex;
        }

        // use Volume to blur the background GameObjects
        void BlurBackground(bool state)
        {
            if (m_Volume == null)
                return;

            DepthOfField blurDOF;
            if (m_Volume.profile.TryGet<DepthOfField>(out blurDOF))
            {
                blurDOF.active = state;
            }
        }

        //  disable the CharacterCard Visual Element
        void HideCharacterCard(UnitController heroUnit)
        {
            // enemy does have CharacterCard
            if (heroUnit.CharacterCard == null)
                return;

            if (m_CharPortraitContainer == null)
                return;

            // disable character card
            VisualElement charCard = GetCharacterCard(heroUnit);
            charCard.style.display = DisplayStyle.None;
        }

        // find the matching CharacterCard
        VisualElement GetCharacterCard(UnitController heroUnit)
        {
            // the hero unit's template Visual Tree Asset
            TemplateContainer cardTemplate = heroUnit.CharacterCard.CharacterTemplate;

            // all character portrait cards
            List<VisualElement> cardElements = m_CharPortraitContainer.Children().ToList();

            // return the match
            foreach (VisualElement card in cardElements)
            {
                if (card == cardTemplate)
                {
                    return card;
                }
            }
            return null;
        }

        // frame fx for special abilities
        void EnableFrameFX(VisualElement card, bool state)
        {
            if (card == null)
                return;

            VisualElement frameFx = card.Q<VisualElement>("game-char__fx-frame");
            ShowVisualElement(frameFx, state);
        }

        IEnumerator GameLostRoutine()
        {
            // wait, then show lose screen and blur bg
            yield return new WaitForSeconds(k_DelayWinScreen);

            // hide UI
            m_CharPortraitContainer.style.display = DisplayStyle.None;
            m_PauseButton.style.display = DisplayStyle.None;

            AudioManager.PlayDefeatSound();
            ShowVisualElement(m_LoseScreen, true);
            BlurBackground(true);
        }

        IEnumerator GameWonRoutine()
        {
            Time.timeScale = 0.5f;
            yield return new WaitForSeconds(k_DelayWinScreen);

            // hide the UI
            m_CharPortraitContainer.style.display = DisplayStyle.None;
            m_PauseButton.style.display = DisplayStyle.None;

            AudioManager.PlayVictorySound();
            ShowVisualElement(m_WinScreen, true);
        }

        // volume settings
        void ChangeSfxVolume(ChangeEvent<float> evt)
        {
            GameplayEvents.MusicVolumeChanged?.Invoke(evt.newValue);
        }

        void ChangeMusicVolume(ChangeEvent<float> evt)
        {
            GameplayEvents.SfxVolumeChanged?.Invoke(evt.newValue);
        }

        // event-handling methods
        void OnGameWon()
        {
            if (m_IsGameOver)
                return;

            m_IsGameOver = true;
            StartCoroutine(GameWonRoutine());
        }

        void OnGameLost()
        {
            if (m_IsGameOver)
                return;

            m_IsGameOver = true;
            StartCoroutine(GameLostRoutine());
        }

        void OnHideCharacterCard(UnitController unit)
        {
            HideCharacterCard(unit);
        }

        void OnSpecialDischarged(UnitController unit)
        {
            VisualElement card = GetCharacterCard(unit);
            EnableFrameFX(card, false);
        }

        void OnSpecialCharged(UnitController unit)
        {
            VisualElement card = GetCharacterCard(unit);
            EnableFrameFX(card, true);
        }

        void OnSettingsUpdated(GameData gameData)
        {
            m_MusicSlider.value = gameData.MusicVolume;
            m_SfxSlider.value = gameData.SfxVolume;
        }
    }
}
