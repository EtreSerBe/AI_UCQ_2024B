using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    public GameObject PlayerGameObject;

    public PatrolState(BaseFSM inBaseFSM) : base("Patrol", inBaseFSM)
    {

    }
    
    // Update is called once per frame by the FSM in this case.
    public override void Update()
    {
        float h = 5.5f;

        int i = (int)h;

        float j = (float)i;

        Debug.Log("h: " + h + " i: " + i + " j: " + j);

        base.Update();

        Debug.Log(PlayerGameObject.name);

        // Aquí es 100% NECESARIO que hagamos el cast de nuestro FSMRef (que es de la clase BaseFSM) y lo 
        // casteemos a la clase que SÍ contiene la variable a la que queremos acceder, en este caso, AlertStateRef.
        FSMRef.ChangeState(((EnemyFSM)FSMRef).AlertStateRef);
        return;  // SIEMPRE, SIEMPRE, SIEMPREEE después del ChangeState.
        // SIEMPRE que se haga un ChangeState, lo ideal es hacer un return para salirnos de lo que estaba ejecutando dicho estado.
        //Debug.Log("Yo no me debería de imprimir porque ya no estoy en el estado de patrullaje.");
    }
}
