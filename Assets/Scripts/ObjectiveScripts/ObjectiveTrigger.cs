using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    [Header("OBJECTIVE")]
    [SerializeField] private ObjectiveSO objectiveToSet;

    [Header("REFERENCES")]
    [SerializeField] private ObjectiveManger objectiveManager;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered)
            return;

        if (!collision.CompareTag("Player"))
            return;

        if (objectiveManager == null)
        {
            Debug.LogError("ObjectiveTrigger: Objective Manager is not assigned!", this);
            return;
        }

        if (objectiveToSet == null)
        {
            Debug.LogError("ObjectiveTrigger: Objective SO is not assigned!", this);
            return;
        }

        triggered = true;

        objectiveManager.SetObjective(objectiveToSet);

        Debug.Log("Objective changed to: " + objectiveToSet.name);

        // Trigger only once
        Destroy(gameObject);
    }
}