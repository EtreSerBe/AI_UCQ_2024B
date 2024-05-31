using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SteeringBehaviors : MonoBehaviour
{
    // Velocidad m�xima a la que nuestro personaje puede ir.
    // sirve para decirle a la aceleraci�n que ya no incremente m�s la velocidad.
    // PREGUNTA: �De qu� tipo de variable deber�a ser nuestra velocidad m�xima y por qu�?
    // float, porque �nicamente necesitamos limitar la magnitud, sin importar la direcci�n.
    public float MaxSpeed = 20.0f;

    // La magnitud de la fuerza que le vamos a aplicar al rigidbody.
    public float Force = 10.0f;

    // una referencia al Rigidbody component que tiene nuestro GameObject en el editor.
    public Rigidbody rb;
    public AgentSenses Senses;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Senses = GetComponent<AgentSenses>();
        // rb = GetComponent<CapsuleCollider>();
        // rb = GetComponent<MeshRenderer>();
        // Component.Destroy(rb);  // si destruy�ramos el componente rigidbody aqu� en el c�digo, tambi�n
        // lo estar�amos borrando del GameObject que lo tiene asignado en el editor.
    }

    // Update is called once per frame
    void Update()
    {
        // Si hacemos este cambio a la variable RB, se ver� reflejado en el editor
        //rb.angularDrag = rb.angularDrag + 0.01f;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;

        bool MouseIsInRange = Senses.TargetIsInRange(mouseWorldPos);

        // Algo as� deber�an de poder hacer con su tarea del cono de visi�n.
        // bool MouseIsInVisionCone = Senses.TargetIsInVisionCone(mouseWorldPos);


        if (MouseIsInRange)
        {
            Debug.Log("El mouse s� est� en rango");
            // Si ya la puede "ver" o sentir, pues ya deber�a poder reaccionar ante ello., En este caso, perseguirlo.

            // para perseguir a alguien, nos tenemos que mover en la direcci�n en la que est�n ellos, respecto a nuestra posici�n.
            Vector3 DesiredDirection = mouseWorldPos - transform.position;
            // Ahorita queremos solo la direcci�n, entonces guardamos la normalizaci�n de dicho vector.
            Vector3 DesiredDirectionNormalized = DesiredDirection.normalized;

            // Una fuerza de magnitud = Force (que es una variable de esta clase), multiplicado por la direcci�n deseada
            // (que es la variable DesiredDirectionNormalized que tenemos aqu� arribita).

            rb.AddForce(DesiredDirectionNormalized * -1.0f * Force, ForceMode.Acceleration);

            // Ahora necesitamos limitar velocidad, para que no supere a la m�xima velocidad (MaxSpeed).
            // Checamos la magnitud de la velocidad.
            // si esa magnitud es mayor que la MaxSpeed, entonces hay que limitarlo
            if (rb.velocity.magnitude > MaxSpeed)
            {
                // c�mo le dir�an: sigues yendo en la misma direcci�n, pero tu magnitud es distinta.
                // Qu� estamos tratando de cambiar? queremos cambiar la velocidad de nuestro rigidbody (es decir, rb.velocity)
                // la queremos limitar a que su magnitud sea la de nuestra velocidad m�xima
                rb.velocity = rb.velocity.normalized * MaxSpeed;  // la misma direcci�n de movimiento, pero con la magnitud del l�mite que le ponemos (MaxSpeed)
            }

        }
        else
        {
            Debug.Log("El mouse NO est� en rango");
            rb.AddForce(rb.velocity.normalized*-1.0f * Force, ForceMode.Acceleration);
        }




    }
}
