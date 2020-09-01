using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayCanvas : MonoBehaviour
{
    [SerializeField] GameObject background;
    [SerializeField] List<Transform> popupsFrames = new List<Transform> ();
    [SerializeField] List<CanvasGroup> popupsCanvases = new List<CanvasGroup> ();

    private void Awake ()
    {
        background.gameObject.SetActive (true);

        for (int i = 0; i < popupsFrames.Count; i ++)
        {
            if (popupsFrames [i] != null)
            {
                popupsFrames [i].localScale = Vector3.zero;
            }
        }

        for (int i = 0; i < popupsCanvases.Count; i ++)
        {
            if (popupsCanvases [i] != null)
            {
                popupsCanvases [i].alpha = 0f;
                popupsCanvases [i].interactable = false;
                popupsCanvases [i].blocksRaycasts = false;
            }
        }
    }
}
