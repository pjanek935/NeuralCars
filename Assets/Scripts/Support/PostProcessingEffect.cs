using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class PostProcessingEffect : MonoBehaviour
{
    [SerializeField] protected Material material;

    protected int dstWidth;
    protected int dstHeight;

    public Material Material
    {
        get { return material; }
    }

    protected virtual void applyEffect(RenderTexture src, RenderTexture dst)
    {
        if (material != null)
        {
            Graphics.Blit (src, dst, material);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        dstWidth = src.width;
        dstHeight = src.height;

        applyEffect (src, dst);
    }
}
