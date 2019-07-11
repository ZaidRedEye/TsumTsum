using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerSelectController : MonoBehaviour
{
    public Image stickerImage;
    public int stickerIndex;

    public void OnSelected()
    {
        GameObject menuObject = GameObject.FindGameObjectWithTag("MenuController");
        MenuController menuController = menuObject.GetComponent<MenuController>();
        menuController.SelectSticker(stickerIndex);
    }

    public void Init(Sprite sprite, int index)
    {
        stickerImage.sprite = sprite;
        stickerIndex = index;
    }
}
