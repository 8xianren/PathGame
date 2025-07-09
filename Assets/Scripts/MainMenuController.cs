using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI元素")]
    public CanvasGroup mainCanvasGroup;
    public TextMeshProUGUI gameTitleText;
    public TextMeshProUGUI startButtonText;
    public TextMeshProUGUI quitButtonText;
    public TextMeshProUGUI versionText;
    public Button startButton;
    public Button quitButton;
    public Image backgroundImage;

    public string gameSceneName = "GameScene";

    [Header("中文文本设置")]
    public TMP_FontAsset chineseFont;
    public string titleText = "我的游戏";
    public string startText = "开始游戏";
    public string quitText = "退出游戏";

    [Header("动画设置")]
    public float fadeInDuration = 1.5f;
    public float buttonHoverScale = 1.1f;
    public float buttonPressScale = 0.95f;
    public float buttonAnimationSpeed = 0.1f;
    public float titleFloatDistance = 20f;
    public float titleFloatSpeed = 1f;

    private Vector3 startButtonOriginalScale;
    private Vector3 quitButtonOriginalScale;
    private Vector3 titleOriginalPosition;

    void Start()
    {
        ApplyChineseFont();
        InitializeUI();
        StartCoroutine(FadeInUI());
    }

    // 应用中文字体
    void ApplyChineseFont()
    {
        if (chineseFont != null)
        {
            gameTitleText.font = chineseFont;
            startButtonText.font = chineseFont;
            quitButtonText.font = chineseFont;
            versionText.font = chineseFont;

            // 设置文本
            gameTitleText.text = titleText;
            startButtonText.text = startText;
            quitButtonText.text = quitText;

            // 版本信息
            versionText.text = $"版本 {Application.version}";
        }
        else
        {
            Debug.LogError("中文字体未分配!");
        }
    }

    // 初始化UI元素
    void InitializeUI()
    {
        // 保存原始值用于动画
        startButtonOriginalScale = startButton.transform.localScale;
        quitButtonOriginalScale = quitButton.transform.localScale;
        titleOriginalPosition = gameTitleText.transform.position;

        // 初始设置
        mainCanvasGroup.alpha = 0f;

        // 注册按钮事件
        startButton.onClick.AddListener(OnStartButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);

        // 添加按钮动画组件
        AddButtonAnimation(startButton);
        AddButtonAnimation(quitButton);
    }

    // 添加按钮动画
    void AddButtonAnimation(Button button)
    {
        var animator = button.gameObject.AddComponent<ButtonAnimation>();
        animator.buttonText = button.GetComponentInChildren<Text>();
        animator.normalScale = button == startButton ?
            startButtonOriginalScale : quitButtonOriginalScale;
        animator.hoverScale = button == startButton ?
            startButtonOriginalScale * buttonHoverScale :
            quitButtonOriginalScale * buttonHoverScale;
        animator.pressScale = button == startButton ?
            startButtonOriginalScale * buttonPressScale :
            quitButtonOriginalScale * buttonPressScale;
        animator.animationSpeed = buttonAnimationSpeed;
    }


    IEnumerator FadeInUI()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            mainCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCanvasGroup.alpha = 1f;

        // 开始标题浮动动画
        StartCoroutine(AnimateTitle());
    }

    // 标题浮动效果
    IEnumerator AnimateTitle()
    {
        while (true)
        {
            // 计算基于时间的浮动
            float yOffset = Mathf.Sin(Time.time * titleFloatSpeed) * titleFloatDistance;
            gameTitleText.transform.position = titleOriginalPosition + new Vector3(0f, yOffset, 0f);
            yield return null;
        }
    }

    // 开始游戏按钮事件
    void OnStartButtonClicked()
    {
        StartCoroutine(TransitionToGame());
    }

    // 退出游戏按钮事件
    void OnQuitButtonClicked()
    {
        StartCoroutine(QuitGame());
    }

    // 过渡到游戏场景
    IEnumerator TransitionToGame()
    {
        // 禁用按钮防止多次点击
        startButton.interactable = false;
        quitButton.interactable = false;

        // 淡出效果
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            mainCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCanvasGroup.alpha = 0f;

        // 加载游戏场景
        SceneManager.LoadScene(gameSceneName);
    }

    // 退出游戏
    IEnumerator QuitGame()
    {
        // 禁用按钮
        startButton.interactable = false;
        quitButton.interactable = false;

        // 淡出效果
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration / 2)
        {
            mainCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / (fadeInDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCanvasGroup.alpha = 0f;

        // 退出游戏
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    

     void Update()
    {
        AdjustUIForScreenSize();
    }
    
    void AdjustUIForScreenSize()
    {
        // 根据屏幕比例调整
        float aspectRatio = (float)Screen.width / Screen.height;
        float targetRatio = 1920f / 1080f;
        float scaleFactor = Mathf.Clamp(aspectRatio / targetRatio, 0.8f, 1.2f);
        
        // 调整标题尺寸
        gameTitleText.fontSize = (int)(72 * scaleFactor);
        
        // 调整按钮尺寸
        Vector3 buttonScale = new Vector3(scaleFactor, scaleFactor, 1);
        startButton.transform.localScale = buttonScale;
        quitButton.transform.localScale = buttonScale;
        
        // 调整按钮文本大小
        startButtonText.fontSize = (int)(36 * scaleFactor);
        quitButtonText.fontSize = (int)(36 * scaleFactor);
    }
}

