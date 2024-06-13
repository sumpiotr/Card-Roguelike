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

    private Player _player;

    private EnemyDataScriptableObject _data;

    private EnemyBehaviourTree _behaviourTree;

    private List<ActionData> _actionsList;

    private void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _actionsList = new List<ActionData>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeTurn();
        }
    }

    public void SetupData(EnemyDataScriptableObject data)
    {
        _data = data;
        GetComponent<SpriteRenderer>().sprite = data.sprite;
        EnemyBehaviourTree original = Resources.Load("EnemiesTrees/" + _data.name) as EnemyBehaviourTree;
        _behaviourTree = original.Clone() as EnemyBehaviourTree;
        _behaviourTree.SetOwner(this);
    }

    public EnemyDataScriptableObject GetData()
    {
        return _data;
    }


    public void TakeTurn()
    {
        _actionsList.Clear();
        _behaviourTree.Update();
        while(_behaviourTree.treeState == NodeState.Running)
        {
            _behaviourTree.Update();
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

    #region ResolveAction

    protected override void PlayAttack(ActionData actionData, Action onResolved)
    {
        Debug.Log("attack");
        _player.TakeDamage(actionData.value);
        onResolved();
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

    protected override void PlayAdvance(ActionData actionData, Action onResolved)
    {
        Advance(actionData.value, _player.AxialPosition);
        onResolved();
    }

    protected override void PlayRetreat(ActionData actionData, Action onResolved)
    {
        Retreat(actionData.value, _player.AxialPosition);
        onResolved();
    }

    protected override void PlayPush(ActionData actionData, Action onResolved)
    {
        _player.Retreat(actionData.value, AxialPosition);
        onResolved();
    }

    protected override void PlayPull(ActionData actionData, Action onResolved)
    {
        _player.Advance(actionData.value, AxialPosition);
        onResolved();
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

    #endregion

    public int GetRangeFromPlayer()
    {
        return HexTilemap.AxialDistance(AxialPosition, _player.AxialPosition);
    }


}
