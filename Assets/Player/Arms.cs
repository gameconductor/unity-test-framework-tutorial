using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arms : MonoBehaviour
{
    public Player player;

    void OnAttackAnimEnd()
    {
        player.OnAttackAnimEnd();
    }

    void OnShockwaveAnimBlast()
    {
        player.OnShockwaveAnimBlast();
    }

    void OnTelegrabAttack()
    {
        player.GetComponent<Telegrab>().Throw();
    }
}
