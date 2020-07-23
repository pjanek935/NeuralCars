using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public enum TooltipState
    {
        HIDDEN, VISIBLE, FADING_IN, FADING_OUT, WAITING_FOR_DISPLAY,
    }

    public enum TooltipAnchorPosition
    {
        CENTER, LEFT, RIGHT, UP, DOWN,
    }

    [SerializeField] Text thisText;
    [SerializeField] Text resizerText;
    [SerializeField] RectTransform resizerTransform;
    [SerializeField] CanvasGroup canvasGroup;

    const float waitTime = 0.4f;
    float timer = 0f;
    float showRate = 3f;

    public static Tooltip Instance
    {
        get;
        private set;
    }

    public TooltipState State
    {
        get;
        private set;
    }

    private void Awake ()
    {
        if (Instance != null)
        {
            Destroy (this.gameObject);
        }
        else
        {
            Instance = this;
            State = TooltipState.HIDDEN;
        }
    }

    private void OnDestroy ()
    {
        Instance = null;
    }

    public void Show (string message, RectTransform rectTransform, TooltipAnchorPosition anchorPosition)
    {
        if (rectTransform == null)
        {
            return;
        }

        resizerText.text = message;

        switch (State)
        {
            case TooltipState.HIDDEN:

                State = TooltipState.WAITING_FOR_DISPLAY;

                break;

            case TooltipState.FADING_OUT:

                State = TooltipState.FADING_IN;

                break;
        }

        StartCoroutine (waitForAndOfAFrameAndReposition (rectTransform, anchorPosition));
    }

    Vector3 getRectTransformOffsetToCenter (RectTransform rectTransform, out float height, out float width)
    {
        Vector3 result = new Vector3 ();
        width = 0f;
        height = 0f;

        if (rectTransform != null)
        {
            width = rectTransform.sizeDelta.x;
            height = rectTransform.sizeDelta.y;
            result.x = (0.5f - rectTransform.pivot.x) * width;
            result.y = (0.5f - rectTransform.pivot.y) * height;
        }

        return result;
    }

    /// <summary>
    /// The layouter has to resize according to new inserted text -
    /// need to wait for end of a frame to correctly reposition.
    /// </summary>
    /// <param name="parentRectTransform"></param>
    /// <param name="anchorPosition"></param>
    /// <returns></returns>
    IEnumerator waitForAndOfAFrameAndReposition (RectTransform parentRectTransform, TooltipAnchorPosition anchorPosition)
    {
        yield return new WaitForEndOfFrame ();

        float parentWidth;
        float parentHeight;
        Vector3 parentPosOffset 
            = getRectTransformOffsetToCenter (parentRectTransform, out parentHeight, out parentWidth); //offset from pos to calculate center

        switch (anchorPosition)
        {
            case TooltipAnchorPosition.CENTER:

                resizerTransform.pivot = new Vector2 (0.5f, 0.5f);

                break;

            case TooltipAnchorPosition.DOWN:

                resizerTransform.pivot = new Vector2 (0.5f, 1f);
                parentPosOffset.y -= parentHeight / 2f;

                break;

            case TooltipAnchorPosition.UP:

                resizerTransform.pivot = new Vector2 (0.5f, 0f);
                parentPosOffset.y += parentHeight / 2f;

                break;

            case TooltipAnchorPosition.LEFT:

                resizerTransform.pivot = new Vector2 (1f, 0.5f);
                parentPosOffset.x -= parentWidth / 2f;

                break;

            case TooltipAnchorPosition.RIGHT:

                resizerTransform.pivot = new Vector2 (0f, 0.5f);
                parentPosOffset.x += parentWidth / 2f;

                break;
        }

        Vector3 targetPos = parentRectTransform.position + parentPosOffset;
        resizerTransform.position = parentRectTransform.position + parentPosOffset;

        float width;
        float height;
        Vector3 currentCenter = getRectTransformOffsetToCenter (resizerTransform, out height, out width);
        currentCenter = targetPos + currentCenter;

        //rect sides positions
        float left = currentCenter.x - (width / 2f); 
        float right = currentCenter.x + (width / 2f);
        float up = currentCenter.y + (height / 2f);
        float down = currentCenter.y - (height / 2f);

        Vector3 offset = new Vector3 (); //the offset that rect must be moved to fit into a screen

        if (left < 0)
        {
            offset.x = -left;
        }

        if (right > Screen.width)
        {
            offset.x = Screen.width - right;
        }

        if (up > Screen.height)
        {
            offset.y = Screen.height - up;
        }

        if (down < 0)
        {
            offset.y = -down;
        }

        resizerTransform.position = resizerTransform.position + offset;
        RectTransform thisRectTransform = (RectTransform) transform;
        thisRectTransform.pivot = resizerTransform.pivot;
        thisRectTransform.position = resizerTransform.position;
        thisText.text = resizerText.text;
    }

    public void Hide ()
    {
        switch (State)
        {
            case TooltipState.VISIBLE:
            case TooltipState.FADING_IN:

                State = TooltipState.FADING_OUT;

                break;

            case TooltipState.WAITING_FOR_DISPLAY:

                State = TooltipState.HIDDEN;
                timer = 0f;

                break;
        }
    }

    private void Update ()
    {
        switch (State)
        {
            case TooltipState.FADING_IN:

                {
                    float newAlpha = canvasGroup.alpha + showRate * Time.deltaTime;

                    if (newAlpha >= 1f)
                    {
                        State = TooltipState.VISIBLE;
                    }

                    canvasGroup.alpha = newAlpha;
                }

                break;

            case TooltipState.FADING_OUT:

                {
                    float newAlpha = canvasGroup.alpha - showRate * Time.deltaTime;

                    if (newAlpha <= 0f)
                    {
                        State = TooltipState.HIDDEN;
                    }

                    canvasGroup.alpha = newAlpha;
                }

                break;

            case TooltipState.HIDDEN:

                break;

            case TooltipState.VISIBLE:

                break;

            case TooltipState.WAITING_FOR_DISPLAY:

                timer += Time.deltaTime;

                if (timer >= waitTime)
                {
                    timer = 0;
                    State = TooltipState.FADING_IN;
                }

                break;
        }
    }
}
