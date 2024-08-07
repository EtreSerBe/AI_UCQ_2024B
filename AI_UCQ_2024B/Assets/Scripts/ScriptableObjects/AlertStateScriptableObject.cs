using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AlertStateScriptableObject", order = 1)]
public class AlertStateScriptableObject : ScriptableObject
{

    // Alert State
    public float VisionAngleAlert = 60.0f;
    public float VisionDistanceAlert = 15.0f;
    // Cuánto tiempo tiene que ver al Infiltrador para pasar al estado de Alerta.
    public float TimeSeeingInfiltratorBeforeEnteringAttack = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
