using Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tilemap;
using Tilemap.Tile;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : BaseCharacter
{
    [SerializeField]
    private EnemyBehaviourTree behaviourTree;

    private Player _player;

    private List<ActionData> _actionsList;

    private void Start()
    {
        _player = GameManager.Instance.GetPlayer();

        _actionsList = new List<ActionData>();

        if (behaviourTree == null) return;

        behaviourTree.SetOwner(this);
        behaviourTree = behaviourTree.Clone() as EnemyBehaviourTree;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeTurn();
        }
    }


    public void TakeTurn()
    {
        _actionsList.Clear();
        behaviourTree.Update();
        while(behaviourTree.treeState == NodeState.Running)
        {
            behaviourTree.Update();
        }

        if(_actionsList.Count > 0)
        {
            ResolveActionSet(_actionsList);
        }
    }


    public void PrepareAction(ActionData action)
    {
        _actionsList.Add(action);
    }
    

  

    protected override void PlayMove(ActionData actionData, Action onResolved)
    {
        switch (actionData.range.rangeType)
        {
            case RangeType.Target:
                StartCoroutine(nameof(MoveToPlayer), actionData.value);
                break;
        }
    }

    private IEnumerator MoveToPlayer(int speed)
    {
        List<PathNode> path = AStar.findPath(HexTilemap.Instance, AxialPosition, _player.AxialPosition);
        if(path.Count > 0)path.RemoveAt(path.Count-1);
        int walkedTiles = 0;
        if(speed > 0 && path.Count > 0)HexTilemap.Instance.GetTile(AxialPosition).SetOccupiedCharacter(null);
        foreach (PathNode pathNode in path)
        {
            if (walkedTiles == speed) break;
            TileObject tile = HexTilemap.Instance.GetTile(pathNode.nodePosition);
            transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, transform.position.z);
            yield return new WaitForSeconds(Time.deltaTime * 500);
            walkedTiles++;
        }
        if (walkedTiles != 0)
        {
            TileObject finalNode = HexTilemap.Instance.GetTile(path[walkedTiles - 1].nodePosition);
            finalNode.SetOccupiedCharacter(this);
        }

    }

    public int GetRangeFromPlayer()
    {
        return HexTilemap.AxialDistance(AxialPosition, _player.AxialPosition);
    }


}
