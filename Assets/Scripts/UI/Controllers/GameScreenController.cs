using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UIToolkitDemo
{
    // non-UI logic for the GameScreen
    public class GameScreenController : MonoBehaviour
    {

        [Header("Scenes")]
        [SerializeField] string m_MainMenuSceneName = "MainMenu";
        [SerializeField] string m_GameSceneName = "Game";

        // temp storage to send back to GameDataManager
        GameData m_SettingsData;

        void OnEnable()
        {
            BattleGameplayManager.GameWon += OnGameWon;
            BattleGameplayManager.GameLost += OnGameLost;

            GameplayEvents.GamePaused += OnGamePaused;
            GameplayEvents.GameResumed += OnGameResumed;
            GameplayEvents.GameQuit += OnGameQuit;
            GameplayEvents.GameRestarted += OnGameRestarted;
            GameplayEvents.MusicVolumeChanged += OnMusicVolumeChanged;
            GameplayEvents.SfxVolumeChanged += OnSfxVolumeChanged;

            UnitController.UnitDied += OnUnitDied;

            SaveManager.GameDataLoaded += OnGameDataLoaded; 
        }

        void OnDisable()
        {
            BattleGameplayManager.GameWon -= OnGameWon;
            BattleGameplayManager.GameLost -= OnGameLost;

            GameplayEvents.GamePaused -= OnGamePaused;
            GameplayEvents.GameResumed -= OnGameResumed;
            GameplayEvents.GameQuit -= OnGameQuit;
            GameplayEvents.GameRestarted -= OnGameRestarted;
            GameplayEvents.MusicVolumeChanged -= OnMusicVolumeChanged;
            GameplayEvents.SfxVolumeChanged -= OnSfxVolumeChanged;

            UnitController.UnitDied -= OnUnitDied;

            SaveManager.GameDataLoaded -= OnGameDataLoaded;
        }

        IEnumerator PauseGameTime(float delay = 2f)
        {

            float pauseTime = Time.time + delay;
            float decrement = (delay > 0) ? Time.deltaTime / delay : Time.deltaTime;

            while (Time.timeScale > 0.1f || Time.time < pauseTime)
            {
                Time.timeScale = Mathf.Clamp(Time.timeScale - decrement, 0f, Time.timeScale - decrement);
                yield return null;
            }

            // ramp the timeScale down to 0
            Time.timeScale = 0f;
        }

        // scene-management methods
        void QuitGame()
        {
            Time.timeScale = 1f;
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                SceneManager.LoadSceneAsync(m_MainMenuSceneName);
        }

        void RestartLevel()
        {

            Time.timeScale = 1f;
#if UNITY_EDITOR
            if (Application.isPlaying)

#endif
                SceneManager.LoadSceneAsync(m_GameSceneName);
        }

        // event-handling methods
        void OnGameLost()
        {
            GameplayEvents.LoseScreenShown?.Invoke();
        }

        void OnGameWon()
        {
            GameplayEvents.WinScreenShown?.Invoke();
        }

        void OnGamePaused(float delay)
        {
            GameplayEvents.SettingsLoaded?.Invoke();
            StopAllCoroutines();
            StartCoroutine(PauseGameTime(delay));
        }

        void OnGameResumed()
        {
            GameplayEvents.SettingsUpdated?.Invoke(m_SettingsData);
            StopAllCoroutines();
            Time.timeScale = 1f;
        }

        void OnGameRestarted()
        {
            RestartLevel();
        }

        void OnGameQuit()
        {
            QuitGame();
        }

        void OnUnitDied(UnitController deadHero)
        {
            GameplayEvents.CharacterCardHidden?.Invoke(deadHero);
        }

        void OnSfxVolumeChanged(float sfxVolume)
        {
            m_SettingsData.MusicVolume = sfxVolume;

            GameplayEvents.SettingsUpdated?.Invoke(m_SettingsData);
        }

        void OnMusicVolumeChanged(float musicVolume)
        {
            m_SettingsData.SfxVolume = musicVolume;

            GameplayEvents.SettingsUpdated?.Invoke(m_SettingsData);
        }

        void OnGameDataLoaded(GameData gameData)
        {
            if (gameData == null)
                return;

            m_SettingsData = gameData;

            m_SettingsData.MusicVolume = gameData.MusicVolume;
            m_SettingsData.SfxVolume = gameData.SfxVolume;

            GameplayEvents.SettingsUpdated?.Invoke(gameData);

        }
    }
}