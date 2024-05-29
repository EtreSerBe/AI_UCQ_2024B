using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    // Forward es la direcci�n hacie el frente del GameObject que sea due�o de este script.
    public Vector3 MovementDirection = Vector3.forward;

    // Start is called before the first frame update
    void Start()
    {
        // normalizamos el vector de MovementDirection para estar 100% seguros de que es una direcci�n normalizada.
        //
        if (MovementDirection.magnitude == 0.0f)
        {
            // Entonces no podemos normalizarlo. 
            // Aqu� imprimir�amos un error y abortar�amos el programa.
            Debug.LogError("ERROR, se trat� de normalizar un vector 0");
        }
        else
        {
            // Si su magnitud no es 0, entonces s� se puede normalizar.
            MovementDirection = MovementDirection.normalized;
        }


    }

    // Update is called once per frame
    // cu�ntas veces se ejecuta la funci�n update cada segundo?
    // Depende.
    // 60 FPS
    // 1000 milisegundos, si queremos 60 FPS, pues dividimos 1000 entre 60.
    // 1000/60 = 16.666... milisegundos.
    void Update()
    {
        // Qu� dice esta l�nea de c�digo?
        // transform.position es la posici�n del GameObject due�o de este script.
        // s�male a la posici�n de ese gameobject la direcci�n de movimiento.
        transform.position += MovementDirection * Time.deltaTime;

        // considerar velocidad

        // considerar aceleraci�n.
        // VelocidadActual = aceleraci�nDeGravedad*Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        MovementDirection = MovementDirection.normalized;
        Gizmos.DrawLine(transform.position, transform.position + MovementDirection * 10000f);
    }
}
