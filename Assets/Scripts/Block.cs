using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
	public static event Action<Block> OnBlockSelected;
	public static event Action<Block> OnBlockEntered;
	public static event Action<Block> OnBlockDeselected;

	[SerializeField] private SpriteRenderer spriteRenderer;
	
	[SerializeField] private Sprite[] blockSprites;

	int _blockType;
	bool _isOnChain;

	public bool IsOnChain
	{
		get { return _isOnChain; }
		set
		{
			_isOnChain = value;
			OnSetOnChain();
		}
	}

	private void OnValidate()
	{
		if (!spriteRenderer)
			spriteRenderer = GetComponent<SpriteRenderer>();
		
	}

	void Start ()
	{
		_blockType = UnityEngine.Random.Range(0, blockSprites.Length);
		name = $"Block_{_blockType}";
		spriteRenderer.sprite = blockSprites[_blockType];

		transform.position = new Vector3 (UnityEngine.Random.Range (-2.0f, 2.0f), 10, 0);
		transform.eulerAngles = new Vector3 (0, 0, UnityEngine.Random.Range (-40f, 40f));
	}

	private void OnSetOnChain()
	{
		if (_isOnChain)
		{
			SetTransparency(.5f);
		}
		else
		{
			SetTransparency(1);
		}
	}

	public void SetTransparency(float transparency) {
		Color color = spriteRenderer.color;
		color.a = transparency;
		spriteRenderer.color = color;
	}

	public static bool IsSameType(Block block1, Block block2) {
		return (block1 && block2 && block1._blockType == block2._blockType);
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		OnBlockSelected?.Invoke(this);
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		OnBlockEntered?.Invoke(this);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		OnBlockDeselected?.Invoke(this);
	}
}
