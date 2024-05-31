using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    // Forward es la direcci�n hacie el frente del GameObject que sea due�o de este script.
    public Vector3 MovementDirection = Vector3.forward;

    public Vector3 GravityForce = Vector3.up * -9.81f;

    public Vector3 Velocity = Vector3.zero;
    

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
        // CUADRO 1 DE ACTUALIZACI�N

        // qu� cambio causa el tener una aceleraci�n?
        // la aceleraci�n incrementa la velocidad. Cu�nto la incrementa? 
        // pues la magnitud de esa aceleraci�n cada segundo.
        Velocity += GravityForce * Time.deltaTime;

        // si deltatime = 16ms
        // 0, 0, 0 += (0, -9.8, 0) * 0.016s;
        // 0, 0, 0, += (0, -.1568, 0)

        //Ah� ya aplicamos aceleraci�n a la velocidad.
        // ahora toca aplicar la velocidad a la posici�n.
        transform.position += Velocity * Time.deltaTime;

        // si delta time = 16ms = 0.016s
        // (0, 0, 0) += (0, -.1568, 0) * 0.016s;
        // (0, -0.0025088, 0)

        // AC� HAREMOS CUADRO 2 DE ACTUALIZACI�N

        // aplicar aceleraci�n a la velocidad
        // si deltatime = 0.025s = 25ms
        // 0, -.1568, 0 += (0, -9.8, 0) * 0.025s;
        // 0, -.1568, 0, += (0, -.245, 0)
        // 0, -.4018, 0  VELOCIDAD AL FINAL DEL CUADRO 2

        // aplicamos velocidad a la posici�n:
        // (0, -0.0025088, 0) += (0, -.4018, 0) * .025s
        // (0, -0.0025088, 0) += (0, -.01, 0) 


        // Qu� dice esta l�nea de c�digo?
        // transform.position es la posici�n del GameObject due�o de este script.
        // s�male a la posici�n de ese gameobject la direcci�n de movimiento.
        // transform.position += MovementDirection * Time.deltaTime;

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
