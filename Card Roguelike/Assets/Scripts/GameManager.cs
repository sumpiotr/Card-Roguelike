using Camera;
using Map;
using System.Collections;
using System.Collections.Generic;
using Tilemap;
using Tilemap.Tile;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance = null;

    [SerializeField]
    private Player playerPrefab;

    [SerializeField]
    private CameraController cameraController;

    private Player _player = null;


    private bool fightMode = false;



    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
    }

    void Start()
    {
        MapManager.Instance.GenerateMap();
        CardObjectsManager.Instance.HideCards();
    }

    public Player GetPlayer()
    {
        return _player;
    }

    public bool IsFightModeActive()
    {
        return fightMode;
    }

    public void SetPlayer(Player player)
    {
        cameraController.SetTarget(player.transform);
        _player = player;
    }
}
