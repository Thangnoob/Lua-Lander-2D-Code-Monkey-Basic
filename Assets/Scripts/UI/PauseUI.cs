using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button musicVolumeButton;
    [SerializeField] private TextMeshProUGUI musicVolumeButtonText;
    [SerializeField] private Button soundVolumeButton;
    [SerializeField] private TextMeshProUGUI soundVolumeButtonText;

    private void Awake()
    {
        musicVolumeButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeMusicVolume();
            musicVolumeButtonText.text = "MUSIC " + MusicManager.Instance.GetMusicVolume();
        });
        soundVolumeButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeSoundVolume();
            soundVolumeButtonText.text = "SOUND " + SoundManager.Instance.GetSoundVolume();
        });
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.UnPauseGame();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnPaused += GameManager_OnGameUnPaused;

        soundVolumeButtonText.text = "SOUND " + SoundManager.Instance.GetSoundVolume();
        musicVolumeButtonText.text = "MUSIC " + MusicManager.Instance.GetMusicVolume();
        Hide();
    }

    private void GameManager_OnGameUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        resumeButton.Select();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
