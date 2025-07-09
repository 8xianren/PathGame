using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IPointerDownHandler, 
    IPointerUpHandler
{
    [Header("动画设置")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = Vector3.one * 1.1f;
    public Vector3 pressScale = Vector3.one * 0.95f;
    public float animationSpeed = 0.1f;
    public Text buttonText;
    
    // 颜色设置
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color pressColor = new Color(1f, 0.5f, 0f);
    
    private Image buttonImage;
    private bool isHovering = false;
    private Coroutine scaleCoroutine;
    private Coroutine colorCoroutine;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        
        // 确保初始状态
        transform.localScale = normalScale;
        if (buttonImage) buttonImage.color = normalColor;
        if (buttonText) buttonText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        // 缩放动画
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(hoverScale));
        
        // 颜色动画
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(ChangeColorTo(hoverColor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        
        // 缩放动画
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(normalScale));
        
        // 颜色动画
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(ChangeColorTo(normalColor));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 缩放动画
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        transform.localScale = pressScale;
        
        // 颜色动画
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        if (buttonImage) buttonImage.color = pressColor;
        if (buttonText) buttonText.color = pressColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 恢复状态
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(isHovering ? hoverScale : normalScale));
        
        // 颜色动画
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(ChangeColorTo(isHovering ? hoverColor : normalColor));
    }

    // 平滑缩放
    IEnumerator ScaleTo(Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 currentScale = transform.localScale;
        
        while (elapsedTime < animationSpeed)
        {
            transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / animationSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = targetScale;
    }

    // 平滑颜色过渡
    IEnumerator ChangeColorTo(Color targetColor)
    {
        float elapsedTime = 0f;
        
        if (buttonImage)
        {
            Color startColor = buttonImage.color;
            while (elapsedTime < animationSpeed)
            {
                buttonImage.color = Color.Lerp(startColor, targetColor, elapsedTime / animationSpeed);
                if (buttonText) buttonText.color = Color.Lerp(startColor, targetColor, elapsedTime / animationSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            buttonImage.color = targetColor;
        }
        
        if (buttonText) buttonText.color = targetColor;
    }
}