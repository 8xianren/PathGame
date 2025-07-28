using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DepthNormalOutline : MonoBehaviour
{
    [Header("描边设置")]
    public Shader outlineShader;
    public Color outlineColor = Color.black;
    [Range(0.1f, 10f)] public float outlineThickness = 1f;
    
    [Header("边缘敏感度")]
    [Range(0.1f, 10f)] public float depthSensitivity = 1f;
    [Range(0.1f, 10f)] public float normalSensitivity = 1f;
    [Range(0f, 1f)] public float normalThreshold = 0.5f;
    
    [Header("卡通色彩增强")]
    [Range(0.5f, 3f)] public float saturation = 1.5f;
    [Range(1, 20)] public int quantizeLevels = 6;
    [Range(0.5f, 2f)] public float contrast = 1.2f;
    [Range(0f, 2f)] public float vibrance = 0.8f;
    [Range(0f, 1f)] public float highlightBoost = 0.2f;
    
    private Material outlineMaterial;
    
    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
        
        if (outlineShader != null && outlineMaterial == null)
        {
            outlineMaterial = new Material(outlineShader);
            outlineMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }
    
    void OnDisable()
    {
        if (outlineMaterial != null)
        {
            DestroyImmediate(outlineMaterial);
        }
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (outlineMaterial != null)
        {
            // 设置描边参数
            outlineMaterial.SetColor("_OutlineColor", outlineColor);
            outlineMaterial.SetFloat("_OutlineThickness", outlineThickness);
            outlineMaterial.SetFloat("_DepthSensitivity", depthSensitivity);
            outlineMaterial.SetFloat("_NormalSensitivity", normalSensitivity);
            outlineMaterial.SetFloat("_NormalThreshold", normalThreshold);
            
            // 设置颜色增强参数
            outlineMaterial.SetFloat("_Saturation", saturation);
            outlineMaterial.SetFloat("_QuantizeLevels", quantizeLevels);
            outlineMaterial.SetFloat("_Contrast", contrast);
            outlineMaterial.SetFloat("_Vibrance", vibrance);
            outlineMaterial.SetFloat("_HighlightBoost", highlightBoost);
            
            Graphics.Blit(source, destination, outlineMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}