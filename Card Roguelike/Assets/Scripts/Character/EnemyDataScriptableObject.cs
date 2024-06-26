using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Data")]
public class EnemyDataScriptableObject : ScriptableObject
{
    public Sprite sprite;
    public int weight;
}
