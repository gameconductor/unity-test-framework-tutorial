using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arms : MonoBehaviour
{
    public Player player;

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    void OnAttackAnimEnd()
    {
        if (player)
        {
            player.OnAttackAnimEnd();
        }
    }

    void OnShockwaveAnimBlast()
    {
        if (player)
        {
            player.OnShockwaveAnimBlast();
        }
    }

    void OnTelegrabAttack()
    {
        if (player)
        {
            player.GetComponent<Telegrab>().Throw();
        }
    }
}
