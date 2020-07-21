using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent (typeof (Image))]
public class ImageFader : MonoBehaviour
{
    Image image;

    void getImageIfNeeded ()
    {
        if (image == null)
        {
            image = GetComponent<Image> ();
        }
    }

    public void FadeIn (UnityAction onFinished = null)
    {
        getImageIfNeeded ();
        image.raycastTarget = true;
        image.DOFade (1f, GlobalConst.SHOW_AND_HIDE_TIME).OnComplete (() => onFinished?.Invoke ());
    }

    public void FadeOut (UnityAction onFinished = null)
    {
        getImageIfNeeded ();
        image.DOFade (0f, GlobalConst.SHOW_AND_HIDE_TIME).OnComplete (() =>
        {
            image.raycastTarget = false;
            onFinished?.Invoke ();
        });
    }
}
