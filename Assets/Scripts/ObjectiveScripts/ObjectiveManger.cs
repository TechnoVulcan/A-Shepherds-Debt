using UnityEngine;
using TMPro;

public class ObjectiveManger : MonoBehaviour
{
    [Header("UI REFERENCES")]
    [SerializeField] private TMP_Text objectiveText;

    [Header("OBJECTIVE")]
    [SerializeField] private ObjectiveSO currentObjective;

    private int objectiveIndex;

    private void Start()
    {
        objectiveIndex = 0;

        if (currentObjective != null)
        {
            ShowObjective();
        }
    }

    public void SetObjective(ObjectiveSO newObjective)
    {
        if (newObjective == null)
            return;

        currentObjective = newObjective;
        objectiveIndex = 0;

        ShowObjective();
    }

    private void ShowObjective()
    {
        if (currentObjective == null)
            return;

        if (objectiveIndex >= currentObjective.lines.Length)
            return;

        ObjectiveLine line = currentObjective.lines[objectiveIndex];

        objectiveText.text = line.text;

        objectiveIndex++;
    }
}