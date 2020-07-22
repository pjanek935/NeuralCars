using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof (RectTransform))]
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string message;
    [SerializeField] Tooltip.TooltipAnchorPosition anchorPosition = Tooltip.TooltipAnchorPosition.CENTER;

    public void OnPointerEnter (PointerEventData eventData)
    {
        Tooltip.Instance.Show (message, (RectTransform) transform);
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        Tooltip.Instance.Hide ();
    }
}
