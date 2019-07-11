using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bomb : MonoBehaviour, IPointerDownHandler
{
	public event Action<List<Block>> OnBombPointerDown;
	private readonly Collider2D[] _results = new Collider2D[8];
	private readonly List<Block> _hitBlocks = new List<Block>();
	public void OnPointerDown(PointerEventData eventData)
	{
		var size = Physics2D.OverlapCircleNonAlloc(transform.position, 1.5f, _results);
		_hitBlocks.Clear();
		for (int i = 0; i < size; i++)
		{
			var c = _results[i];
			var block = c.GetComponent<Block>();
			if(!block) continue;
			_hitBlocks.Add(block);
		}
		
		OnBombPointerDown?.Invoke(_hitBlocks);
		Destroy (gameObject);
	}

	private void OnDisable()
	{
		OnBombPointerDown = null;
	}
}
