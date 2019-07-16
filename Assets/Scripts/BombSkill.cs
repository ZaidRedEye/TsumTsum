using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombSkill : MonoBehaviour, IBlockClearListener
{
    public static event Action<BombSkill> OnSkillActivated;
    public static event Action<List<Block>> OnSkillUsed;
    
    [SerializeField] private int blockClearListenerPriority;
    [SerializeField] private float chargeUpRate;
    [SerializeField] private float explosionRadius;
    [SerializeField] private int targetLimit = 8;
    [SerializeField] private Button skillButton;

    private float _charge;
    public int BlockClearListenerPriority => blockClearListenerPriority;

    private Collider2D[] _results;
    private readonly List<Block> _hitBlocks = new List<Block>();

    private void Awake()
    {
        _results = new Collider2D[targetLimit];
        GameManager.OnRoundStart += OnRoundStart;
        GameManager.OnRoundEnd += OnRoundEnd;
        skillButton.onClick.AddListener(ActivateSkill);
    }

    private void OnDestroy()
    {
        GameManager.OnRoundStart -= OnRoundStart;
        GameManager.OnRoundEnd -= OnRoundEnd;
    }

    private void Start()
    {
      
    }
    
   

    private void OnRoundStart(int round)
    {
        if (skillButton)
        {
            skillButton.interactable = false;
        }
        _charge = 0;
    }

    private void OnRoundEnd(int round)
    {
		
    }

    public void ActivateSkill()
    {
        if (skillButton)
            skillButton.interactable = false;

        _charge = 0;
        
        OnSkillActivated?.Invoke(this);

        Block.OnBlockSelected += OnBlockSelected;
    }

    private void OnBlockSelected(Block selectedBlock)
    {
        var size = Physics2D.OverlapCircleNonAlloc(selectedBlock.transform.position, explosionRadius, _results);
        _hitBlocks.Clear();
        for (int i = 0; i < size; i++)
        {
            var c = _results[i];
            var block = c.GetComponent<Block>();
            if(!block) continue;
            _hitBlocks.Add(block);
        }
        
        _hitBlocks.Add(selectedBlock);
        OnSkillUsed?.Invoke(_hitBlocks);
        Block.OnBlockSelected -= OnBlockSelected;
    }


    public int CompareTo(IBlockClearListener other)
    {
        return BlockClearListenerPriority.CompareTo(other.BlockClearListenerPriority);
    }

    
    public void OnBlocksCleared(int chain)
    {
        _charge += chargeUpRate * chain; // need to clear 30 blocks to enter fever mode
        _charge = Mathf.Clamp01(_charge);
        
        if (_charge >= 1 && skillButton)
        {
            skillButton.interactable = true;
        }
    }
}
