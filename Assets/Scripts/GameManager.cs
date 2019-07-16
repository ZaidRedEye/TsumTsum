

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
	[Serializable]
	public enum GameModes
	{
		Record,
		Versus
	}
	public static event Action OnGameStart;
	public static event Action<int> OnRoundStart;
	public static event Action<int> OnRoundEnd;
	public static event Action OnIntervalStart;
	public static event Action OnGameEnd;

	public static GameModes GameMode = GameModes.Record;
	
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

	private void OnValidate()
	{
		
	}

	private void Awake()
	{
		_waitForTimerTick = new WaitForSeconds(tickDuration);
		_waitForInterval = new WaitForSeconds(intervalDuration);
		_waitForStartUp = new WaitForSeconds(introDuration);
		
		if(restartBtn)
			restartBtn.onClick.AddListener(() =>  SceneManager.LoadScene(SceneManager.GetActiveScene().name));
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
		
		OnGameEnd?.Invoke();
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
		OnGameStart?.Invoke();
		yield return _waitForStartUp;
	}

	private IEnumerator GameRoundRoutine()
	{
		gameStateText.text = $"Start Round {_roundCounter}";
		yield return new WaitForSeconds(0.3f);
		gameStateText.text = "";
		gameStateText.enabled = false;
		blocker.enabled = false;
		_timeCounter = roundDuration;
		_gameStartStamp = Time.time;
		OnRoundStart?.Invoke(_roundCounter);
		for (_timeCounter = roundDuration;  _timeCounter >= 0; --_timeCounter)
		{
			timeText.text = _timeCounter.ToString();
			yield return _waitForTimerTick;
		}
		OnRoundEnd?.Invoke(_roundCounter);
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
