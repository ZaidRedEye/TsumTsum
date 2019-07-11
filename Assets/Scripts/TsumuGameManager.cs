using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsumuGameManager: MonoBehaviour {

	public GameObject blockPrefab;
	public GameObject bombPrefab;
	[SerializeField] private float minChainDistance = 1.5f;
	[SerializeField] private int bombMinChain = 6;
	
	Block firstBlock;
	Block lastBlock;
	List<Block> removeBlockList = new List<Block> ();

	ScoreManager scoreManager;
	FeverManager feverManager;
	TimeManager timeManager;
	RaycastHit2D[] _chainResults = new RaycastHit2D[2];
	Collider2D[] _bombResults = new Collider2D[8];
	
	void Start () {
		StartCoroutine (GenerateBlocks (45));
		scoreManager = gameObject.AddComponent<ScoreManager> ();
		feverManager = gameObject.GetComponent<FeverManager> ();
		timeManager = gameObject.AddComponent<TimeManager> ();
	}

	IEnumerator GenerateBlocks(int n){
		for (int i = 0; i < n; i++) {
			// Generate a new block every 0.02 seconds
			yield return new WaitForSeconds (0.02f);
			var block = Instantiate (blockPrefab, transform, true).GetComponent<Block>();
			if(!block) break;

			block.onPointerEnter += OnPointerEnterBlock;
			block.onPointerDown += OnPointerDownBlock;
			block.onPointerUp += OnPointerUpBlock;

		}
	}

	void GenerateBomb(Vector3 position) {
		var bomb = Instantiate (bombPrefab, transform, true).GetComponent<Bomb>();
		bomb.transform.position = position;
		bomb.OnBombPointerDown += OnPointerDownBomb;
	}
	
	private void OnPointerDownBlock(Block block)
	{
		if (!block || firstBlock) return;
		firstBlock = block;
		lastBlock = block;
		AddToRemoveBlockList(block);
	}
	
	private void OnPointerEnterBlock(Block block)
	{
		if (!block || block == lastBlock || !IsNewBlockRemovable(block)) return;
		
		lastBlock = block;
		AddToRemoveBlockList (block);
	}
	
	private void OnPointerUpBlock(Block block)
	{
		if(!firstBlock) return;
		
		var count = removeBlockList.Count;
		if (count >= 3) {
			OnBlockClear(count);
			if (count >= bombMinChain) {
				// Since lastBlock is deleted in ClearRemoveBlockList, this has to go before that
				GenerateBomb (lastBlock.transform.position);
			}
			ClearRemoveBlockList ();
		} else {
			ResetRemoveBlockList ();
		}
		firstBlock = null;
		lastBlock  = null;
	}

	bool IsNewBlockRemovable(Block newBlock) {
		if (!Block.IsSameType (newBlock, firstBlock)) {
			return false;
		}

		if (newBlock.IsOnChain) {
			return false;
		}

		var hits = Physics2D.RaycastNonAlloc(lastBlock.transform.position,
			(newBlock.transform.position - lastBlock.transform.position), _chainResults, minChainDistance);

		// The first hit is ignored because it returns the origin itself
		if (hits > 1) {
			if (_chainResults [1].collider != null) {
				if (_chainResults [1].collider.gameObject == newBlock.gameObject) {
					return true;
				}
			}
		}
		return false;
	}

	void AddToRemoveBlockList(Block block) {
		removeBlockList.Add (block);
		block.IsOnChain = true;
	}

	// Distroy and move everyting out of removeBlockList
	void ClearRemoveBlockList() {
		foreach (var block in removeBlockList) {
			Destroy (block.gameObject);
		}
		removeBlockList.Clear();
	}

	// Move everything out of removeBlockList without distroying
	void ResetRemoveBlockList() {
		foreach (var block in removeBlockList) {
			block.IsOnChain = false;
		}
		removeBlockList.Clear ();
	}

	public void OnBlockClear(int chain) {
		scoreManager.AddScore (ScoreManager.CalculateScore (chain, 1, feverManager.IsFever()));
		feverManager.AddFeverValue (chain);
		StartCoroutine (GenerateBlocks (chain));
	}

	public void OnPointerDownBomb(List<Block> hitBlocks)
	{
		OnBlockClear(hitBlocks.Count);

		foreach (var hitBlock in hitBlocks)
		{
			Destroy(hitBlock.gameObject);
		}
	}
}