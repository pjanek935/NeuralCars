using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Image), typeof (RectTransform))]
public class UILineRenderer : MonoBehaviour
{
    [Range (0, 100)] [SerializeField] float width = 1;
    [SerializeField] Vector2 start;
    [SerializeField] Vector2 end;

    private void OnValidate ()
    {
        refresh ();
    }

    public Vector2 Start
    {
        get { return start; }

        set
        { 
            start = value;
            refresh ();
        }
    }

    public Vector2 End
    {
        get { return end; }

        set
        { 
            end = value;
            refresh ();
        }
    }

    RectTransform rectTransform = null;

    private void Awake ()
    {
        getRectTransformIfNeeded ();
    }

    public void SetPoints (Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
        refresh ();
    }

    void refresh ()
    {
        getRectTransformIfNeeded ();
        float dist = Vector2.Distance (start, end);
        rectTransform.sizeDelta = new Vector2 (width, dist);
        rectTransform.position = new Vector3 (start.x, start.y, rectTransform.position.z);
        Vector3 dir = start - end;
        rectTransform.rotation = Quaternion.LookRotation (rectTransform.forward, dir);
    }

    void getRectTransformIfNeeded ()
    {
        if (rectTransform == null)
        {
            rectTransform = (RectTransform) transform;
        }
    }
}
