using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FeverManager : MonoBehaviour
{
	public static event Action onFeverStart;
	public static event Action onFeverEnd;
	
	[SerializeField] private float chargeUpRate = .03f;
	[SerializeField] private float feverBonusMultiplier;
	[SerializeField] private float feverDuration;
	[SerializeField] private Scrollbar scrollbar;
	float feverValue = 0; // when feverValue reaches 100, the fever mode begins
	bool isFever = false;

	private float _feverCountDownTimer;
	public float FeverBonusMultiplier => feverBonusMultiplier;

	public float FeverDuration => feverDuration;


	private void Start()
	{
		feverValue = 0;
		SyncFeverGUI();
	}

	void Update () {
		if(!isFever) return;

		_feverCountDownTimer -= Time.deltaTime;
		feverValue = _feverCountDownTimer / feverDuration;
		SyncFeverGUI();
		if(feverValue <= 0)
			OnFeverEnd();
	}
	
	public void AddFeverValue(int chain) {
		if (isFever) return;
		feverValue += chargeUpRate * chain; // need to clear 30 blocks to enter fever mode
		feverValue = Mathf.Clamp01(feverValue);
		SyncFeverGUI();
		
		if (feverValue >= 1) {
			OnFeverStart ();
		}
	}

	public bool IsFever() {
		return isFever;
	}

	void OnFeverStart() {
		isFever = true;
		_feverCountDownTimer = feverDuration;
		onFeverStart?.Invoke();
	}

	void OnFeverEnd() {
		isFever = false;
		onFeverEnd?.Invoke();
	}

	void SyncFeverGUI() {
		if (scrollbar) {
			scrollbar.size = feverValue;
		}
	}
}
