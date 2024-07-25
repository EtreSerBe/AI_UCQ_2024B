using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFSM : MonoBehaviour
{
    private BaseState CurrentState;

    // Necesitamos la función que nos permita cambiar de estado. Este nunca se necesita cambiar.
    public void ChangeState(BaseState NewState)
    {
        CurrentState.Exit();
        CurrentState = NewState;
        CurrentState.Enter();
    }

    // Start is called before the first frame update
    // El Start de la BaseFSM solo se extiende, pero no se modifica lo que tiene dentro.
    public virtual void Start()
    {
        CurrentState = GetInitialState();
        // Ahora nos toca entrar al estado (es decir, llamar su función Enter() )
        if (CurrentState == null)
            Debug.LogError("No hay un estado inicial válido asignado.");
        else
        {
            CurrentState.Enter();
        }
    }

    // Update is called once per frame
    // Este tampoco se necesita cambiar nunca, porque lo único que la FSM hace es actualizar el estado actual.
    void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }

    public virtual BaseState GetInitialState()
    {
        // Regresa null para que cause error porque la función de esta clase padre nunca debe de usarse, siempre 
        // se le debe de hacer un override.
        return null;
    }

    private void OnGUI()
    {
        string text = CurrentState != null ? CurrentState.Name : "No current State assigned";
        GUILayout.Label($"<size=40>{text}</size>");
    }
}
