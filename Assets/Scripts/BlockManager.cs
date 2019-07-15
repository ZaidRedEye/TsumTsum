using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager: MonoBehaviour
{
	public GameObject blockPrefab;
	public GameObject bombPrefab;
	[SerializeField] private int maxBlocks = 45;
	[SerializeField] private int minChain = 3;
	[SerializeField] private float minChainDistance = 1.5f;
	[SerializeField] private int bombMinChain = 6;
	
	Block _firstBlock;
	Block _lastBlock;
	readonly List<Block> _clearList = new List<Block> ();

	ScoreManager scoreManager;
	FeverManager feverManager;
	GameFlowManager _gameFlowManager;
	RaycastHit2D[] _chainResults = new RaycastHit2D[2];

	private bool _blockSelection;
	
	private readonly List<IBlockClearListener> _blockClearListeners = new List<IBlockClearListener>();

	private readonly List<GameObject> _spawnedBlocks = new List<GameObject>();
	
	private void Awake()
	{
		Block.OnBlockEntered += OnBlockEntered;
		Block.OnBlockSelected += OnBlockSelected;
		Block.OnBlockDeselected += OnBlockDeselected;
		Bomb.OnBombExploded += OnBombExploded;
		BombSkill.OnSkillActivated += OnBombSkillActivated;
		BombSkill.OnSkillUsed += OnBombSkillUsed;
		GameFlowManager.OnGameStart += OnGameStart;
		GameFlowManager.OnGameEnd += OnGameEnd;
		
		_blockClearListeners.AddRange(GetComponentsInChildren<IBlockClearListener>());
		_blockClearListeners.Sort();
	}
	
	private void OnDestroy()
	{
		Block.OnBlockEntered -= OnBlockEntered;
		Block.OnBlockSelected -= OnBlockSelected;
		Block.OnBlockDeselected -= OnBlockDeselected;
		Bomb.OnBombExploded -= OnBombExploded;
		BombSkill.OnSkillActivated -= OnBombSkillActivated;
		BombSkill.OnSkillUsed -= OnBombSkillUsed;
		GameFlowManager.OnGameStart -= OnGameStart;
		GameFlowManager.OnGameEnd -= OnGameEnd;
	}
	
	private void OnGameStart()
	{
		_firstBlock = null;
		_lastBlock  = null;
		StartCoroutine (GenerateBlocks (maxBlocks));
	}
	private void OnGameEnd()
	{
		foreach (var spawnedBlock in _spawnedBlocks)
		{
			Destroy(spawnedBlock.gameObject);
		}
		_spawnedBlocks.Clear();
		_clearList.Clear();
	}
	
	IEnumerator GenerateBlocks(int n){
		for (int i = 0; i < n; i++) {
			// Generate a new block every 0.02 seconds
			yield return new WaitForSeconds (0.02f);
			_spawnedBlocks.Add(Instantiate(blockPrefab, transform, true));

		}
	}

	void GenerateBomb(Vector3 position) {
		var bomb = Instantiate (bombPrefab, transform, true);
		bomb.transform.position = position;
		_spawnedBlocks.Add(bomb);
	}
	
	private void OnBlockSelected(Block block)
	{
		if (!block || _firstBlock || _blockSelection) return;
		_firstBlock = block;
		_lastBlock = block;
		AddToClearList(block);
	}
	
	private void OnBlockEntered(Block block)
	{
		if (!block || block == _lastBlock || !IsNewBlockRemovable(block)) return;
		
		_lastBlock = block;
		AddToClearList(block);
	}
	
	private void OnBlockDeselected(Block block)
	{
		if(!_firstBlock) return;
		
		var count = _clearList.Count;
		if (count >= minChain) {
			
			if (count >= bombMinChain) {
				// Since lastBlock is deleted in ClearRemoveBlockList, this has to go before that
				GenerateBomb (_lastBlock.transform.position);
			}
			ClearBlocks();
		} else {
			ResetClearList ();
		}
		_firstBlock = null;
		_lastBlock  = null;
	}

	bool IsNewBlockRemovable(Block newBlock) 
	{
		if (!Block.IsSameType (newBlock, _firstBlock) || newBlock.IsOnChain) return false; 
		
		var hits = Physics2D.RaycastNonAlloc(_lastBlock.transform.position,
			(newBlock.transform.position - _lastBlock.transform.position), _chainResults, minChainDistance);

		// The first hit is ignored because it returns the origin itself
		
		if (hits <= 1 || _chainResults[1].collider == null) return false;
		
		return _chainResults[1].collider.gameObject == newBlock.gameObject;
	}

	private void AddToClearList(Block block) {
		_clearList.Add (block);
		block.IsOnChain = true;
	}

	// Move everything out of removeBlockList without distroying
	private void ResetClearList() {
		foreach (var block in _clearList) {
			if(block) block.IsOnChain = false;
		}
		_clearList.Clear ();
	}

	private void ClearBlocks()
	{
		var chain = _clearList.Count;
		foreach (var block in _clearList)
		{
			_spawnedBlocks.Remove(block.gameObject);
			Destroy(block.gameObject);
		}
			
		_clearList.Clear();
		
		foreach (var blockClearListener in _blockClearListeners)
			blockClearListener.OnBlocksCleared(chain);
		
		StartCoroutine(GenerateBlocks(chain));
	}
	
	private void OnBombExploded(Bomb bomb, List<Block> hitBlocks)
	{
		foreach (var hitBlock in hitBlocks)
			AddToClearList(hitBlock);
		
		_spawnedBlocks.Remove(bomb.gameObject);
		hitBlocks.Clear();
		ClearBlocks();
	}

	private void OnBombSkillActivated(BombSkill bombSkill)
	{
		_blockSelection = true;
	}

	private void OnBombSkillUsed(List<Block> hitBlocks)
	{
		_blockSelection = false;
		
		foreach (var hitBlock in hitBlocks)
			AddToClearList(hitBlock);
	
		hitBlocks.Clear();
		ClearBlocks();
		
	}
}

public interface IBlockClearListener: IComparable<IBlockClearListener>
{
	int BlockClearListenerPriority { get; }
	void OnBlocksCleared(int chain);
}
