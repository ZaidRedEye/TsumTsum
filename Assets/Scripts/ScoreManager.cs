using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * ScoreManager takes care of the scoring system. If it finds a UI Text element named ScoreGUI,
 * it updates the ScoreGUI text, but it works fine without ScoreGUI.
 */
public class ScoreManager : MonoBehaviour, IBlockClearListener
{
	public static event Action<int> OnScoreChanged;
	
	[SerializeField] private int blockClearListenerPriority;
	[SerializeField] private int blockScore = 100;
	[SerializeField] private int chainBonus = 50;
	[SerializeField] private float comboTimeOut = 3f;
	[SerializeField] private float comboMultiplier = .1f;
	private int _score = 0;
	private float _feverBonus = 1;
	private float _comboBonus;
	private int _comboCount;
	private Coroutine _comboTimeOutRoutine;
	
	public int BlockClearListenerPriority => blockClearListenerPriority;

	public int Score
	{
		get => _score;
		private set
		{
			_score = value;
			OnScoreChanged?.Invoke(_score);
		}
	}

	void Start()
	{
		FeverManager.OnFeverStart += OnFeverStart;
		FeverManager.OnFeverEnd += OnFeverEnd;
		GameManager.OnRoundStart += OnRoundStart;
		GameManager.OnRoundEnd += OnRoundEnd;
	}

	private void OnDestroy()
	{
		FeverManager.OnFeverStart -= OnFeverStart;
		FeverManager.OnFeverEnd -= OnFeverEnd;
		GameManager.OnRoundStart -= OnRoundStart;
		GameManager.OnRoundEnd -= OnRoundEnd;
	}

	private void OnFeverStart(FeverManager feverManager)
	{
		_feverBonus = feverManager.FeverBonusMultiplier;
	}

	private void OnFeverEnd(FeverManager feverManager)
	{
		_feverBonus = 1;
	}

	private void OnRoundStart(int round)
	{
		Score = 0;
	}

	private void OnRoundEnd(int round)
	{
		_feverBonus = 1;
		_comboBonus = 1;
		if(_comboTimeOutRoutine != null)
			StopCoroutine(_comboTimeOutRoutine);
	}

	public void OnBlocksCleared(int chain)
	{
		IncrementCombo();
		Score += CalculateScore(chain);
	}

	private void IncrementCombo()
	{
		++_comboCount;
		if(_comboTimeOutRoutine != null)
			StopCoroutine(_comboTimeOutRoutine);
		
		_comboTimeOutRoutine = StartCoroutine(ComboTimeOutTimer());
	}

	public int CalculateScore(int chain)
	{
		var chainScore = CalculateChainScore (chain);
		var comboBonus = CalculateComboBonus ();
		return Mathf.CeilToInt(chainScore * _feverBonus * comboBonus);
	}

	private int CalculateChainScore(int chain)
	{
		var score = 0;
		for (int i = 0; i < chain; i++)
		{
			score += blockScore + i * chainBonus;
		}

		return score;
	}

	private IEnumerator ComboTimeOutTimer()
	{
		yield return new WaitForSeconds(comboTimeOut);
		_comboCount = 0;
	}

	private float CalculateComboBonus()
	{
		var comboBonus = 1f;

		if (_comboCount <= 1) return comboBonus;

		comboBonus += comboMultiplier * (_comboCount - 1);
		return comboBonus;
	}

	public int CompareTo(IBlockClearListener other)
	{
		return BlockClearListenerPriority.CompareTo(other.BlockClearListenerPriority);
	}
}
