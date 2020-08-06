using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipTriggerCollider : MonoBehaviour
{
    [SerializeField] string message;
    [SerializeField] Tooltip.TooltipAnchorPosition anchorPosition = Tooltip.TooltipAnchorPosition.CENTER;

    private void OnMouseEnter ()
    {
        Tooltip.Instance.ShowFromScene (message, transform.position, anchorPosition);
    }

    private void OnMouseExit ()
    {
        Tooltip.Instance.Hide ();
    }
}
