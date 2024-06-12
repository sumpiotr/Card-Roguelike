using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "EncountersList", menuName = "Enemy/EncountersList")]
public class EncountersListScriptableObject : ScriptableObject
{
    public List<EncounterListItem> encounters;

    internal EncounterListItem GetRandomEncounter()
    {
        if (encounters.Count == 0) return null;
        return encounters[Random.Range(0, encounters.Count)];
    }
}


[System.Serializable]
public class EncounterListItem
{
    public List<EnemyDataScriptableObject> enemies;
}