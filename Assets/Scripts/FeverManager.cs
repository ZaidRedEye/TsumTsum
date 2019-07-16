using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FeverManager : MonoBehaviour, IBlockClearListener
{
	public static event Action<FeverManager> OnFeverStart;
	public static event Action<FeverManager> OnFeverEnd;
	public static event Action<float> OnFeverValueChanged; 
	
	[SerializeField] private int blockClearListenerPriority;
	[SerializeField] private float chargeUpRate = .03f;
	[SerializeField] private float feverBonusMultiplier;
	[SerializeField] private float feverDuration;

	private float _feverValue = 0; // when feverValue reaches 100, the fever mode begins
	private bool _isFever = false;

	private float _feverCountDownTimer;
	public float FeverBonusMultiplier => feverBonusMultiplier;

	public float FeverDuration => feverDuration;

	public bool IsFever => _isFever;

	public int BlockClearListenerPriority => blockClearListenerPriority;

	public float FeverValue
	{
		get => _feverValue;
		set
		{
			_feverValue = value;
			OnFeverValueChanged?.Invoke(_feverValue);
		}
	}

	private void Awake()
	{
		GameManager.OnRoundStart += OnRoundStart;
		GameManager.OnRoundEnd += OnRoundEnd;
	}

	private void OnDestroy()
	{
		GameManager.OnRoundStart -= OnRoundStart;
		GameManager.OnRoundEnd -= OnRoundEnd;
	}

	private void OnRoundStart(int round)
	{
		FeverValue = 0;
	}

	private void OnRoundEnd(int round)
	{
		FeverEnd();
	}

	void Update () {
		if(!IsFever) return;

		_feverCountDownTimer -= Time.deltaTime;
		FeverValue = _feverCountDownTimer / feverDuration;
		if(FeverValue <= 0)
			FeverEnd();
	}

	

	public void OnBlocksCleared(int chain)
	{
		if (IsFever) return;
	
		FeverValue = Mathf.Clamp01(FeverValue + chargeUpRate * chain);

		if (FeverValue >= 1)
		{
			FeverStart();
		}
	}
	
	public void FeverStart() {
		_isFever = true;
		_feverCountDownTimer = feverDuration;
		OnFeverStart?.Invoke(this);
	}

	public void FeverEnd() {
		_isFever = false;
		FeverValue = 0;
		OnFeverEnd?.Invoke(this);
	}
	

	public int CompareTo(IBlockClearListener other)
	{
		return BlockClearListenerPriority.CompareTo(other.BlockClearListenerPriority);
	}
}
