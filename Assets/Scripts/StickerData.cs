using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StickerData",menuName ="Data/StickerData")]
public class StickerData : ScriptableObject
{
    private static StickerData _instance;
    
    public static StickerData Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.Load<StickerData>("Data/StickerData");
            return _instance;
        }
    }
    
    public Sprite[] StickerSprites;
    public Sprite RandomSticker { get { return StickerSprites[Random.Range(0, StickerSprites.Length)]; } }

 
}
