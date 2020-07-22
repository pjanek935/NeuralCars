using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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

    [SerializeField] Text text;
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

    public void Show (string message, RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            return;
        }

        this.transform.position = rectTransform.transform.position;
        text.text = message;

        switch (State)
        {
            case TooltipState.HIDDEN:

                State = TooltipState.WAITING_FOR_DISPLAY;

                break;

            case TooltipState.FADING_OUT:

                State = TooltipState.FADING_IN;

                break;
        }
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
