using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Image[] stickerImages;
    [SerializeField] private Scrollbar feverBar;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool isLocalPlayer;
    [SerializeField] private PlayerController opponent;
    
    private PlayerGameLog _playerGameLog;
    private int _curRoundIdx;
    
    private void OnValidate()
    {
        if (!nameLabel)
        { 
            nameLabel = GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(tm=> tm.name == "NameLabel");
        }
        
        if (!scoreLabel)
        { 
            scoreLabel = GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(tm=> tm.name == "ScoreLabel");
        }

        if (stickerImages.Length <= 0)
        {
            stickerImages = GetComponentsInChildren<Image>().Where(i=> i.transform.parent.name == "Stickers").ToArray();
        }

        if (!feverBar)
        {
            feverBar = GetComponentInChildren<Scrollbar>();
        }

        if (!canvasGroup)
            canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Awake()
    {
        if (isLocalPlayer)
            SetAsLocalPlayer();
        else
            SetAsRemotePlayer();
    }

    private void SetAsLocalPlayer()
    {
        ScoreManager.OnScoreChanged += OnScoreChanged;
        FeverManager.OnFeverStart += OnFeverStart;
        FeverManager.OnFeverEnd += OnFeverEnd;
        GameManager.OnRoundStart += OnRoundStart;
        GameManager.OnRoundEnd += OnRoundEnd;
        GameManager.OnGameEnd += OnGameEnd;

        var startStickerIdx = HackedSaveManager.GetInt(SAVE_KEY.STICKER_START);
        var winStickerIdx = HackedSaveManager.GetInt(SAVE_KEY.STICKER_WIN);
        var loseStickerIdx = HackedSaveManager.GetInt(SAVE_KEY.STICKER_LOSE);
        var stickerIndices = new List<int> {startStickerIdx, winStickerIdx, loseStickerIdx};

        for (int i = 0; i < 2;)
        {
            var randomIdx = Random.Range(0, StickerData.Instance.StickerSprites.Length);
            if(stickerIndices.Contains(randomIdx)) continue;
            stickerIndices.Add(randomIdx);
            ++i;
        }
    
        _playerGameLog = new PlayerGameLog(HackedSaveManager.GetString(SAVE_KEY.DISPLAY_NAME), stickerIndices.ToArray());

        for (int i = 0; i < stickerImages.Length && i < stickerIndices.Count; i++)
        {
            if(!stickerImages[i]) continue;
            stickerImages[i].overrideSprite = StickerData.Instance.StickerSprites[stickerIndices[i]];
        }

        if (nameLabel) nameLabel.text = _playerGameLog.name;
    }

    private void SetAsRemotePlayer()
    {
        switch (GameManager.GameMode)
        {
            case GameManager.GameModes.Record:
                if (canvasGroup)
                {
                      canvasGroup.alpha = 0;
                      canvasGroup.interactable = false;
                }
                break;
            case GameManager.GameModes.Versus:
                break;
        }    
    }

    private void OnDestroy()
    {
        if(!isLocalPlayer) return;
        ScoreManager.OnScoreChanged -= OnScoreChanged;
        FeverManager.OnFeverStart -= OnFeverStart;
        FeverManager.OnFeverEnd -= OnFeverEnd;
        GameManager.OnRoundStart -= OnRoundStart;
        GameManager.OnRoundEnd -= OnRoundEnd;
        GameManager.OnGameEnd -= OnGameEnd;
    }

    private void OnScoreChanged(int score)
    {
        if (scoreLabel) scoreLabel.text = score.ToString();
        
        _playerGameLog.rounds[_curRoundIdx].scoreLogs.Add(new ScoreLog
        {
            currentScore = score,
            timeStamp = Time.time - _playerGameLog.rounds[_curRoundIdx].startTimeStamp
        });
    }

    private void OnFeverValueChanged(float feverValue)
    {
        if (feverBar) feverBar.value = feverValue;
    }

    private void OnFeverStart(FeverManager feverManager)
    {
        OnFeverValueChanged(feverManager.FeverValue);
        FeverManager.OnFeverValueChanged += OnFeverValueChanged;
        _playerGameLog.rounds[_curRoundIdx].feverLogs.Add(new FeverLog()
        {
            timeStamp = Time.time - _playerGameLog.rounds[_curRoundIdx].startTimeStamp
        });
    }

    private void OnFeverEnd(FeverManager feverManager)
    {
        FeverManager.OnFeverValueChanged -= OnFeverValueChanged;
    }

    private void OnRoundStart(int round)
    {
        if(!isLocalPlayer) return;

        _curRoundIdx = round - 1;
        if (_playerGameLog.rounds.Count < round)
        {
            _playerGameLog.rounds.Add(new Round
            {
                roundNo = round,
                startTimeStamp = Time.time,
                scoreLogs = new List<ScoreLog>(),
                feverLogs = new List<FeverLog>()
            });
        }
    }

    private void OnRoundEnd(int round)
    {
        var latestScore = _playerGameLog.rounds[_curRoundIdx].scoreLogs.Last().currentScore;
        _playerGameLog.totalScore += latestScore;

    }

    private void OnGameEnd()
    {
        _playerGameLog.SaveToFile(true);
    }
}
