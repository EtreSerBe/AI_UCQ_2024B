using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : BaseState
{
    public AlertState(BaseFSM inBaseFSM) : base("Alert", inBaseFSM)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        // Ahora le digo que cambie al estado de Patrullaje. (Solo para ejemplificar).
        // No se les olvide la parte del Casteo.
        FSMRef.ChangeState( ((EnemyFSM)FSMRef).PatrolStateRef );
        return;
    }
}
