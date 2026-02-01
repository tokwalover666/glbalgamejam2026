using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraDitherEffect : MonoBehaviour
{
    [Header("Material")]
    public Material ditherMaterial;

    [Header("Dither")]
    [Range(0f, 1f)] public float ditherStrength = 1f;
    [Range(2, 64)] public float colorSteps = 8;
    [Range(0.25f, 16f)] public float patternScale = 1f;

    [Header("Color Intensity")]
    [Range(0f, 3f)] public float saturation = 1f;
    [Range(0f, 3f)] public float contrast = 1f;
    [Range(-1f, 1f)] public float brightness = 0f;
    [Range(0f, 1f)] public float posterizeStrength = 0f;

    [Header("Style")]
    public bool lumaDither = false;

    [Header("Optional Pixelate")]
    [Tooltip("0 = off. Common values: 64, 128, 256")]
    [Range(0f, 512f)] public float pixelate = 0f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (ditherMaterial != null)
        {
            ditherMaterial.SetFloat("_DitherStrength", ditherStrength);
            ditherMaterial.SetFloat("_ColorSteps", colorSteps);
            ditherMaterial.SetFloat("_PatternScale", patternScale);

            ditherMaterial.SetFloat("_Saturation", saturation);
            ditherMaterial.SetFloat("_Contrast", contrast);
            ditherMaterial.SetFloat("_Brightness", brightness);
            ditherMaterial.SetFloat("_PosterizeStrength", posterizeStrength);

            ditherMaterial.SetFloat("_LumaDither", lumaDither ? 1f : 0f);
            ditherMaterial.SetFloat("_Pixelate", pixelate);

            Graphics.Blit(src, dest, ditherMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}