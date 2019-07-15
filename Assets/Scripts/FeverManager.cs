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
	
	[SerializeField] private int blockClearListenerPriority;
	[SerializeField] private float chargeUpRate = .03f;
	[SerializeField] private float feverBonusMultiplier;
	[SerializeField] private float feverDuration;
	[SerializeField] private Scrollbar scrollbar;
	
	private float _feverValue = 0; // when feverValue reaches 100, the fever mode begins
	private bool _isFever = false;

	private float _feverCountDownTimer;
	public float FeverBonusMultiplier => feverBonusMultiplier;

	public float FeverDuration => feverDuration;

	public bool IsFever => _isFever;

	public int BlockClearListenerPriority => blockClearListenerPriority;

	private void Awake()
	{
		GameFlowManager.OnGameStart += OnGameStart;
		GameFlowManager.OnGameEnd += OnGameEnd;
	}

	private void OnDestroy()
	{
		GameFlowManager.OnGameStart -= OnGameStart;
		GameFlowManager.OnGameEnd -= OnGameEnd;
	}

	private void OnGameStart()
	{
		_feverValue = 0;
		SyncFeverGui();
	}

	private void OnGameEnd()
	{
		FeverEnd();
		SyncFeverGui();
	}

	void Update () {
		if(!IsFever) return;

		_feverCountDownTimer -= Time.deltaTime;
		_feverValue = _feverCountDownTimer / feverDuration;
		SyncFeverGui();
		if(_feverValue <= 0)
			FeverEnd();
	}

	

	public void OnBlocksCleared(int chain)
	{
		if (IsFever) return;
		_feverValue += chargeUpRate * chain; // need to clear 30 blocks to enter fever mode
		_feverValue = Mathf.Clamp01(_feverValue);
		SyncFeverGui();
		
		if (_feverValue >= 1)
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
		_feverValue = 0;
		OnFeverEnd?.Invoke(this);
	}

	private void SyncFeverGui() {
		if (scrollbar) {
			scrollbar.size = _feverValue;
		}
	}

	public int CompareTo(IBlockClearListener other)
	{
		return BlockClearListenerPriority.CompareTo(other.BlockClearListenerPriority);
	}
}
