using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackStateData", menuName = "ScriptableObjects/AttackStateScriptableObject", order = 1)]
public class AttackStateScriptableObject : ScriptableObject
{
    public float VisionAngle = 60.0f;
    public float VisionDistance = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
