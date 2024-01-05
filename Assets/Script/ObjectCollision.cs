using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// connect from player to object

public class ObjectCollision : MonoBehaviour
{
    [Header("jump height when player shit on this")] public float boundHeight;
    /// <summary>
    /// shit or not on this object
    /// </summary>
    [HideInInspector] public bool playerStepOn;
}
