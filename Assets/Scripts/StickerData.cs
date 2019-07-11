using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StickerData",menuName ="Data/StickerData")]
public class StickerData : ScriptableObject
{
    public Sprite[] StickerSprites;
    public Sprite RandomSticker { get { return StickerSprites[Random.Range(0, StickerSprites.Length)]; } }
}
