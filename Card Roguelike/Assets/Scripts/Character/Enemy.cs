using System.Collections;
using System.Collections.Generic;
using Tilemap;
using UnityEngine;

public class Enemy : BaseCharacter
{
    [SerializeField]
    private EnemyBehaviourTree behaviourTree;

    private Player _player;

    private void Start()
    {
        if (behaviourTree == null) return;

        behaviourTree = behaviourTree.Clone() as EnemyBehaviourTree;
        behaviourTree.SetOwner(this);

        _player = GameManager.Instance.GetPlayer();
    }

    public int GetRangeFromPlayer()
    {
        return HexTilemap.AxialDistance(AxialPosition, _player.AxialPosition);
    }


}
