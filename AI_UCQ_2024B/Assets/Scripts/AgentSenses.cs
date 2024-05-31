using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



// Este script es para darle (simular) a un agente de IA sistemas sensoriales.
// Por ejemplo, la vista o el oído.

public class AgentSenses : MonoBehaviour
{
    // Queremos que nuestro agente de IA tenga un rango de visión.
    public float VisionRange = 10.0f;
    // Si un objeto que debería de ver está dentro de ese rango de visión, entonces debería poder verlo;
    // si está fuera de ese rango, no lo podrá ver.

    /*
     * ¿Por qué hacemos funciones en el código?
     * Más fácil de depurar,
     * Más fácil de reutilizar
     * Más ordenado
     * Menos propenso a error humano.
     */

    // 1) Poner las cosas de interés seteadas de antemano.
    // en este caso, eso podría ser poner una referencia del objeto que nos interesa en el editor.
    public Transform InfiltratorTransform = null;

    // Este video se usó de referencia para la parte del cono de visión y el ángulo.
    // https://youtu.be/lV47ED8h61k?si=IV2KaIP9fU4Pwwxj



    // Start is called before the first frame update
    void Start()
    {
        Vector3 myVectorA = new Vector3(1, 1, 0);
        Vector3 myVectorB = new Vector3(3, 4, 0);

        Vector3 myDistVectorBToA = VectorDiff(myVectorB, myVectorA);
        Vector3 myDistVectorAToB = VectorDiff(myVectorA, myVectorB);

        // NOTA: Un operador (en este caso, el operador minus '-' o símbolo de menos) está "sobrecargado" para poder realizar la 
        // resta por componente entre dos vectores A y B. Es decir, (A.x - B.x), (A.y - B.y), (A.z, B.z).
        // Esto nos da como resultado un vector C, cuyos 'x', 'y', y 'z' son el resultado de cada resta individual.
        // Es decir, es exactamente como la función "VectorDiff" que nosotros realizamos abajo.
        // Para más información: https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/operators/
        // https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/operators/operator-overloading
        Vector3 myDistVectorAToBMinus = myVectorA - myVectorB;

        // Vector3.Distance()
        Debug.Log(" myDistVectorAToB is: " + myDistVectorAToB.ToString());
        Debug.Log(" myDistVectorAToBMinus is: " + myDistVectorAToBMinus.ToString());

        Debug.Log("La magnitud del vector myDistVectorAToB es: " + Magnitude(myDistVectorAToB));
        Debug.Log("La magnitud del vector myDistVectorAToB, con la función de unity, es: " + Vector3.Magnitude(myDistVectorAToB));

        Vector3 TestPosition = new Vector3(1, 2, 3);

        // Vamos a usar la magnitud que calculamos y nuestra variable del rango de visión, para determinar si X punto en el 
        // espacio está dentro del rango de visión del agente que posea este script.
        // Para esto, usaremos la posición de nuestro agente como el origen.
        Vector3 TestMinusCharacter = VectorDiff(TestPosition, transform.position);
        // Ahora queremos la magnitud de este vector
        float VecMagnitud = Magnitude(TestMinusCharacter);

        // Si están hablando de un "si pasa tal cosa..." o "tengo que checar tal cosa", muy probablemente eso se 
        // vaya a traducir en un "if" en el código.
        // En este ejemplo, queremos ver si la magnitud entre nuestro personaje y nuestro de punto de prueba
        // es mayor o menor que el rango de visión, para determinar si lo ve (o vería) o no.
        if (VecMagnitud > VisionRange)
        {
            Debug.Log("No lo veo");
        }
        else
        {
            Debug.Log("Lo veo");
        }

        // Muchas veces vamos a querer saber la dirección en la que estamos viendo.
        // Por ejemplo, para hacer una comparación entre la dirección en que estoy mirando, y la dirección en que está mi objetivo.
    }

    // Método de pitágoras para cálculo de la hipotenusa de un triángulo rectángulo.
    // Ésto lo hará con un vector3.
    // Nos regresa un solo valor, que es la magnitud de un vector.
    float Magnitude(Vector3 in_Vector)
    {
        // Aplicamos teorema de pitágoras a este vector de distancia.
        // La hipotenusa/magnitud la raíz cuadrada de la suma de los cuadrados de los componentes.
        float sqrX = in_Vector.x * in_Vector.x;
        float sqrY = in_Vector.y * in_Vector.y;
        float sqrZ = in_Vector.z * in_Vector.z;
        return Mathf.Sqrt(sqrX + sqrY + sqrZ);
    }


    // Una función que nos regresa un vector que es la diferencia entre el vector Destino menos el vector Origen
    Vector3 VectorDiff(Vector3 destination, Vector3 origin)
    {
        return new Vector3(destination.x - origin.x, destination.y - origin.y, destination.z - origin.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Diferencia entre mi posición y la posición de mi objetivo.
        Vector3 DistVector = VectorDiff(InfiltratorTransform.position, transform.position);

        // Queremos la magnitud de esa distancia.
        float DistMagnitude = Magnitude(DistVector);

        // Si la distancia entre la posición de mi objetivo y mi posición es mayor que mi rango de visión, entonces...
        if (DistMagnitude > VisionRange)
        {
            // Entonces no podemos ver a ese objetivo.
            Debug.Log("No lo veo");
        }
        else
        {
            // Entonces sí lo podría ver.
            Debug.Log("Lo veo");
        }
    }

    public bool TargetIsInRange(Vector3 targetPosition)
    {
        // Diferencia entre mi posición y la posición de mi objetivo.
        Vector3 distVector = VectorDiff(targetPosition, transform.position);

        // Queremos la magnitud de esa distancia.
        float distMagnitude = Magnitude(distVector);

        // Si la distancia entre la posición de mi objetivo y mi posición es mayor que mi rango de visión, entonces...
        if (distMagnitude > VisionRange)
        {
            // Entonces no podemos ver a ese objetivo.
            return false;
        }

        // Entonces sí lo podría ver.
        return true;
    }

    // Agregar una función que se va a parecer un poco a TargetIsInRange, pero que va a hacer la parte del Cono de visión.


    // Gizmo
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // Nos ayuda a dibujar feedback visual en la vista de Escena del Editor.
        Gizmos.DrawLine(transform.position, InfiltratorTransform.position);

        // Antes de dibujar nuestra esfera, podemos cambiar su color según si nuestro objetivo está dentro o fuera de nuestro
        // rango de visión.
        if (TargetIsInRange(InfiltratorTransform.position))
        {
            // Si sí está en rango, hacemos que la esfera sea roja.
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        

        // Dibujo de nuestra esfera de visión desde la posición del Agente. Por lo tanto, usamos un radio = Rango de visión.
        Gizmos.DrawWireSphere(transform.position, VisionRange);


    }
}
