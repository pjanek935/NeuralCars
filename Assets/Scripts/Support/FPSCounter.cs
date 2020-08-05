using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] Text text;

    int frameCount = 0;
    float deltaTime = 0f;
    float fps = 0f;
    float updateRate = 4; //4 updates per second

    public float FPS
    {
        get { return fps; }
    }

    private void Awake()
    {
        if (!Application.isEditor)
        {
            this.gameObject.SetActive (false);
        }
    }

    void Update()
    {
        frameCount++;
        deltaTime += Time.unscaledDeltaTime;

        if (deltaTime > (1f / updateRate))
        {
            fps = frameCount / deltaTime;
            frameCount = 0;
            deltaTime -= 1f / updateRate;
        }

        if (text != null)
        {
            text.text = "FPS: " + fps;
        }
    }
}
