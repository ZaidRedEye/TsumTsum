using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
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
    [SerializeField] private GameObject playerSelectionScrollView;
    [SerializeField] private Transform playerSelectionContentRoot;
    
    SAVE_KEY selectedSticker;

    private void Awake()
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

        if(playerSelectionContentRoot == null) return;
        
        var playerCnt = 0;
        foreach (var playerGameLog in PlayerLogRepository.Instance.PlayerGameLogs)
        {
            if (playerCnt >= playerSelectionContentRoot.childCount)
            {
                Instantiate(playerSelectionContentRoot.GetChild(0).gameObject,
                            playerSelectionContentRoot);
            }

            var entry = playerSelectionContentRoot.GetChild(playerCnt++);

            foreach (var label in entry.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                switch (label.name)
                {
                    case "Name":
                        label.text = playerGameLog.name;
                        break;
                    case "Date":
                        label.text = new DateTime(playerGameLog.dateTime).ToString("g");
                        break;
                    case "Score":
                        label.text = playerGameLog.totalScore.ToString();
                        break;
                }
            }

        }
    }

    public void OnDisplayNameOnSubmit()
    {
        HackedSaveManager.SetString(SAVE_KEY.DISPLAY_NAME, displayNameInputField.text);
    }

    public void ChangeSticker(int key)
    {
        selectedSticker = (SAVE_KEY)key;
    }

    private void OnGameModeSelected(GameManager.GameModes gameMode)
    {
        GameManager.GameMode = gameMode;
        SceneManager.LoadScene(1);
    }

    public void SelectRecordMode()
    {
        OnGameModeSelected(GameManager.GameModes.Record);
    }

    public void SelectRandomChallenge()
    {
        OnGameModeSelected(GameManager.GameModes.Versus);
    }

    public void SelectChallenger()
    {
        
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
