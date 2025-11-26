using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public System.Action<GameObject> OnTriggerEnter;
    
    public void OnTriggerEnterEvent(GameObject other)
    {
        OnTriggerEnter?.Invoke(other);
    }
    
    void HandleTrigger(GameCollider2D colA, GameCollider2D colB)
    {   
        GameCollider2D triggerCol = colA.isTrigger ? colA : colB;
        GameCollider2D otherCol = colA.isTrigger ? colB : colA;
    
        Debug.Log($"Trigger detectado: {triggerCol.name} con {otherCol.name}");
    
        TriggerHandler handler = triggerCol.GetComponent<TriggerHandler>();
        if (handler != null)
        {
            handler.OnTriggerEnterEvent(otherCol.gameObject);
        }
        else
        {
            Debug.LogError($"No hay TriggerHandler en {triggerCol.name}");
        }
    }
}
