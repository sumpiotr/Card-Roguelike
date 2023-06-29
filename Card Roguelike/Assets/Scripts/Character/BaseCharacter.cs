using Actions;
using Actions.ScriptableObjects;
using Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour, ICharacter
{

    private Vector2Int _axialPosition;

    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public Vector2Int AxialPosition { get => _axialPosition; set => _axialPosition = value; }
}
