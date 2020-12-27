using UnityEngine;
using UnityEngine.Events;

public class OnMouseEventListener : MonoBehaviour
{
    public UnityAction OnDown;

    private void OnMouseDown ()
    {
        OnDown?.Invoke ();
    }
}
