using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    public String Name = "BaseState";
    // Necesita "conocer" (tener una referencia o forma de contactar) a la máquina de estados que es su dueña.
    public BaseFSM FSMRef;

    public BaseState(string inName, BaseFSM inBaseFSM)
    {
        Name = inName;
        FSMRef = inBaseFSM;
    }

    // Start is called before the first frame update
    public virtual void Enter()
    {
        // Vamos a poner inicializaciones, pedir memoria, recursos, etc.

        Debug.Log("Enter del estado: " + Name);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Debug.Log("Update del estado: " + Name);
    }

    public virtual void Exit()
    {
        // Vamos a liberar memoria, quitar recursos, ocultar cosas que ya no sean necesarias, etc.
        Debug.Log("Exit del estado: " + Name);
    }
}
