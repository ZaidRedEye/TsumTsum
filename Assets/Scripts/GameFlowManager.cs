//using System.Collections;
//using System.Collections.Generic;

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Timer counts down 1 min. If it finds a UI Text element named TimeGUI, it updates
 * the TimeGUI text, but it works fine without TimeGUI.
 */
public class GameFlowManager : MonoBehaviour
{
	public static event Action OnIntroStart;
	public static event Action OnGameStart;
	public static event Action OnGameEnd;
	public static event Action OnIntervalStart;

	[SerializeField] private TextMeshProUGUI timeText;
	[SerializeField] private Image blocker;
	[SerializeField] private TextMeshProUGUI gameStateText;
	[SerializeField] private Button restartBtn;
	[SerializeField] private int totalRounds;
	[SerializeField] private int introDuration;
	[SerializeField] private int roundDuration; 
	[SerializeField] private int intervalDuration;
	[SerializeField] private float tickDuration;
	private int _timeCounter = 0;
	private int _roundCounter = 0;
	private WaitForSeconds _waitForTimerTick;
	private WaitForSeconds _waitForInterval;
	private WaitForSeconds _waitForStartUp;
	private float _gameStartStamp;

	private void Awake()
	{
		_waitForTimerTick = new WaitForSeconds(tickDuration);
		_waitForInterval = new WaitForSeconds(intervalDuration);
		_waitForStartUp = new WaitForSeconds(introDuration);
		
		if(restartBtn)
			restartBtn.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
	}

	private IEnumerator Start()
	{
		if(restartBtn)
			restartBtn.gameObject.SetActive(false);
		
		yield return StartCoroutine(StartUpRoutine());
		for (_roundCounter = 1; _roundCounter <= totalRounds; ++_roundCounter)
		{
			yield return StartCoroutine(GameRoundRoutine());
			
			if(_roundCounter < totalRounds)
				yield return StartCoroutine(IntervalRoutine());
		}
		blocker.enabled = true;
		gameStateText.enabled = false;
		if(restartBtn)
			restartBtn.gameObject.SetActive(true);
	}

	private IEnumerator StartUpRoutine()
	{
		blocker.enabled = true;
		gameStateText.enabled = true;
		gameStateText.text = "Get Ready";
		timeText.text = "";
		OnIntroStart?.Invoke();
		yield return _waitForStartUp;
	}

	private IEnumerator GameRoundRoutine()
	{
		gameStateText.text = $"Start Round {_roundCounter}";
		yield return new WaitForSeconds(0.6f);
		gameStateText.text = "";
		gameStateText.enabled = false;
		blocker.enabled = false;
		_timeCounter = roundDuration;
		_gameStartStamp = Time.time;
		OnGameStart?.Invoke();
		for (_timeCounter = roundDuration;  _timeCounter >= 0; --_timeCounter)
		{
			timeText.text = _timeCounter.ToString();
			yield return _waitForTimerTick;
		}
		OnGameEnd?.Invoke();
	}
	
	private IEnumerator IntervalRoutine()
	{
		blocker.enabled = true;
		gameStateText.enabled = true;
		gameStateText.text = "Interval";
		timeText.text = "";
		OnIntervalStart?.Invoke();
		yield return _waitForInterval;
	}
	
}
