using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyActionNode : ActionNode 
{
    [HideInInspector]public Enemy owner;
}
