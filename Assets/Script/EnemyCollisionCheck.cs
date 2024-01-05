using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionCheck : MonoBehaviour
{
    /// <summary>
    /// enemy or wall inside this area
    /// </summary>
    [HideInInspector] public bool isOn = false;

    private string groundTag = "Ground";
    private string enemyTag = "Enemy";

    #region// judge contact
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == groundTag || collision.tag == enemyTag)
        {
            isOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == groundTag || collision.tag == enemyTag)
        {
            isOn = false;
        }
    }
    #endregion
}
