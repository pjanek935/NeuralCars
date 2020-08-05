using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] SpriteAtlas uiSpriteAtlas;

    static SpriteManager instance;

    public static SpriteManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public Sprite GetLanguageSprite (Language language)
    {
        Sprite result = uiSpriteAtlas.GetSprite (language.ToString ().ToLower ());

        return result;
    }
}
