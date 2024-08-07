using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PatrolStateScriptableObject", order = 1)]
public class PatrolStateScriptableObject : ScriptableObject
{
    public float VisionAngle = 45.0f;
    public float VisionDistance = 10.0f;
    // Cuánto tiempo tiene que ver al Infiltrador para pasar al estado de Alerta.
    // como referencia: https://www.youtube.com/clip/UgkxIfuLTVvptjElLkcgE3VMJmSU6qCytLei

    public float TimeSeeingInfiltratorBeforeEnteringAlert = 2.0f;
    // Cuánto tiempo tiene que pasar sin ver al infiltrador antes de olvidarlo
    // (es decir, si ya no lo ha visto en tanto tiempo, entonces se relaja y borra el tiempo
    // que se había acumulado en TimeSeeingInfiltratorBeforeEnteringAlert).
    public float TimeToForget = 5.0f;
    // Qué tantos grados gira en su posición este enemigo cuando está patrullando.
    public float RotationAngle = 45.0f;
    // Cada cuánto tiempo va a rotar el patrullero en su posición.
    public float TimeToRotate = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
