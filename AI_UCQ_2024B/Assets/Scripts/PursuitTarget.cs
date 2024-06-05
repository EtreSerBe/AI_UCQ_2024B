using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PursuitTarget : SteeringBehaviors
{
    private float AccumulatedTime = 0f;
    public float circleRadius = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update() 
    {
        AccumulatedTime += Time.deltaTime;
        // Queremos que se mueva de manera predecible con una cierta velocidad, de preferencia constante.
        // Un ciclo
        // El círculo es un ejemplo perfecto de un ciclo.
        float x = Mathf.Sin(AccumulatedTime);
        float y = Mathf.Cos(AccumulatedTime);
        Vector3 targetPosition = new Vector3(x*circleRadius, y*circleRadius, 0.0f);
        Vector3 SeekVector = Seek(targetPosition);

        // Una fuerza de magnitud = Force (que es una variable de esta clase), multiplicado por la dirección deseada
        // (que es la variable DesiredDirectionNormalized que tenemos aquí arribita).

        rb.AddForce(SeekVector * Force, ForceMode.Acceleration);

        LimitToMaxSpeed();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float x = Mathf.Sin(AccumulatedTime);
        float y = Mathf.Cos(AccumulatedTime);
        Vector3 targetPosition = new Vector3(x * circleRadius, y * circleRadius, 0.0f);
        Gizmos.DrawWireSphere(targetPosition, 1.0f);
    }
}
