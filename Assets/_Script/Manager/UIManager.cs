using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    bool isPause;
    bool isSetting;
    [SerializeField] Image PauseButton;
    [SerializeField] GameObject SettingUI;
    [SerializeField] Image SettingButton;
    [SerializeField] Sprite[] PauseButtonImage;
    private void Start()
    {
        isPause = false;
        isSetting = false;
        SettingUI.SetActive(isSetting);
    }
    public void PauseGame()
    {
        if (isPause)
        {
            isPause = false;
            Time.timeScale = 1.0f;
            PauseButton.sprite = PauseButtonImage[0];
        }
        else
        {
            isPause = true;
            Time.timeScale = 0;
            PauseButton.sprite = PauseButtonImage[1];
        }
        SettingButton.gameObject.SetActive(!isPause);
        OrderManager.Instance.canReceiveOrder = !isPause;
    }
    public void SettingGame()
    {
        if (isSetting)
        {
            isSetting = false;
            Time.timeScale = 1.0f;
        }
        else
        {
            isSetting = true;
            Time.timeScale = 0;
        }
        PauseButton.gameObject.SetActive(!isSetting);
        OrderManager.Instance.canReceiveOrder = !isSetting;
        SettingUI.SetActive(isSetting);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void RemakeGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
