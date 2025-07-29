using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    [Header("UI组件")]
    public TMP_FontAsset chineseFont; // 中文字体资源
    public Image backgroundImage;
    public TextMeshProUGUI titleText;
    public Button startButton;
    public Button quitButton;
    public TextMeshProUGUI versionText;

    [Header("界面设置")]
    public string gameSceneName = "MainGame"; // 游戏主场景名称
    public Color buttonNormalColor = Color.white;
    public Color buttonHoverColor = Color.yellow;
    public Color buttonPressedColor = new Color(1f, 0.5f, 0f); // 橙色
    public float buttonHoverScale = 1.1f;
    public float buttonPressScale = 0.95f;

    [Header("本地化文本")]
    public string gameTitle = "对战游戏";
    public string startButtonText = "开始游戏";
    public string quitButtonText = "退出游戏";

    [Header("音效")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip backgroundMusic;


    

    // Start is called before the first frame update
    void Start()
    {
        // 应用字体到所有中文UI元素
        ApplyChineseFont();

        // 设置UI文本
        SetupUITexts();

        // 初始化按钮
        SetupButtons();

        // 播放背景音乐
        PlayBackgroundMusic();

    }

    private void ApplyChineseFont()
    {
        if (chineseFont == null)
        {
            Debug.LogError("请分配中文字体资源！");
            return;
        }

        titleText.font = chineseFont;
        startButton.GetComponentInChildren<TextMeshProUGUI>().font = chineseFont;
        quitButton.GetComponentInChildren<TextMeshProUGUI>().font = chineseFont;
        versionText.font = chineseFont;
    }

    private void SetupUITexts()
    {
        titleText.text = gameTitle;
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = startButtonText;
        quitButton.GetComponentInChildren<TextMeshProUGUI>().text = quitButtonText;
        versionText.text = $"版本 v{Application.version}";

        // 确保中文文本渲染质量
        titleText.enableAutoSizing = true;
        titleText.fontSizeMin = 24;
        titleText.fontSizeMax = 72;
    }

    private void SetupButtons()
    {
        /*
        // 添加悬停效果
        AddHoverEffect(startButton);
        AddHoverEffect(quitButton);
        */

        // 绑定点击事件
        startButton.onClick.AddListener(() => OnButtonClicked(StartGame));
        quitButton.onClick.AddListener(() => OnButtonClicked(QuitGame));
    }

    private void OnButtonClicked(System.Action action)
    {
        PlaySound(clickSound);
        // 禁用所有按钮防止多次点击
        startButton.interactable = false;
        quitButton.interactable = false;

        // 添加延迟效果
        Invoke(nameof(DisableAllButtons), 0.1f);

        // 执行操作
        action?.Invoke();
    }
    private void DisableAllButtons()
    {
        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    // 开始游戏
    private void StartGame()
    {
        // 可以添加场景过渡效果
        SceneManager.LoadScene(gameSceneName);
    }
    // 退出游戏
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // 播放音效
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
