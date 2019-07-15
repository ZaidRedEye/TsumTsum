using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bomb : MonoBehaviour, IPointerDownHandler
{
	[SerializeField] private float explosionRadius = 1.5f;
	[SerializeField] private int targetLimit = 8;
	public static event Action<Bomb, List<Block>> OnBombExploded;
	private Collider2D[] _results;
	private readonly List<Block> _hitBlocks = new List<Block>();

	private void Awake()
	{
		_results = new Collider2D[targetLimit];
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		var size = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, _results);
		_hitBlocks.Clear();
		for (int i = 0; i < size; i++)
		{
			var c = _results[i];
			var block = c.GetComponent<Block>();
			if(!block) continue;
			_hitBlocks.Add(block);
		}
		
		OnBombExploded?.Invoke(this, _hitBlocks);
		Destroy (gameObject);
	}

	
}
