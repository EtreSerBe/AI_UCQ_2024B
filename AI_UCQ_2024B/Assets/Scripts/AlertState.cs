using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : BaseState
{
    // GameObject que es el Player al cual nuestro patrullero debe detectar.
    public GameObject PlayerGameObject;

    // Referencia al Scriptable Object que contiene varios valores que describen cómo se debe comportar este estado.
    private AlertStateScriptableObject _stateValues;

    public AlertState(BaseFSM inBaseFSM) : base("Alert", inBaseFSM)
    {

    }

    public virtual void InitializeState(BaseFSM inFSMRef, GameObject inPlayerGameObject,
        AlertStateScriptableObject inStateValues)
    {
        base.InitializeState(inFSMRef);

        _stateValues = inStateValues;
        PlayerGameObject = inPlayerGameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        // base.OnUpdate();

        // Ahora le digo que cambie al estado de Patrullaje. (Solo para ejemplificar).
        // No se les olvide la parte del Casteo.
        //FSMRef.ChangeState( ((EnemyFSM)FSMRef).PatrolStateRef );
        return;
    }
}
