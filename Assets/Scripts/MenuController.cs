using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] TMP_InputField displayNameInputField;
    [SerializeField] StickerData stickerData;
    [SerializeField] Transform stickerContentParent;
    [SerializeField] GameObject stickerSelectPrefab;
    [SerializeField] Image startSticker;
    [SerializeField] Image winSticker;
    [SerializeField] Image loseSticker;
    [SerializeField] GameObject stickerSelectScrollView;

    SAVE_KEY selectedSticker;

    private void Start()
    {
        displayNameInputField.text = HackedSaveManager.GetString(SAVE_KEY.DISPLAY_NAME);
        selectedSticker = SAVE_KEY.INVALID;

        for(int i = 0; i < stickerData.StickerSprites.Length; ++i)
        {
            GameObject newStickerBtn = Instantiate(stickerSelectPrefab);
            newStickerBtn.GetComponent<StickerSelectController>().Init(stickerData.StickerSprites[i],i);
            newStickerBtn.transform.SetParent(stickerContentParent);
            newStickerBtn.transform.localScale = Vector3.one;
        }

        startSticker.sprite = stickerData.StickerSprites[HackedSaveManager.GetInt(SAVE_KEY.STICKER_START)];
        winSticker.sprite = stickerData.StickerSprites[HackedSaveManager.GetInt(SAVE_KEY.STICKER_WIN)];
        loseSticker.sprite = stickerData.StickerSprites[HackedSaveManager.GetInt(SAVE_KEY.STICKER_LOSE)];
    }

    public void OnDisplayNameOnSubmit()
    {
        HackedSaveManager.SetString(SAVE_KEY.DISPLAY_NAME, displayNameInputField.text);
    }

    public void ChangeSticker(int key)
    {
        selectedSticker = (SAVE_KEY)key;
    }

    public void SelectSticker(int stickerIndex)
    {
        stickerSelectScrollView.SetActive(false);
        HackedSaveManager.SetInt(selectedSticker, stickerIndex);

        switch(selectedSticker)
        {
            case SAVE_KEY.STICKER_WIN:
                winSticker.sprite = stickerData.StickerSprites[stickerIndex];
                break;
            case SAVE_KEY.STICKER_START:
                startSticker.sprite = stickerData.StickerSprites[stickerIndex];
                break;
            case SAVE_KEY.STICKER_LOSE:
                loseSticker.sprite = stickerData.StickerSprites[stickerIndex];
                break;
        }
    }
}
