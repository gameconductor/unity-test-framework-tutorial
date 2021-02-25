using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arms : MonoBehaviour
{
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
