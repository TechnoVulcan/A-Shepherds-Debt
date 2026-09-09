using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public TMP_Text objectiveText;

    public void UpdateObjective(string text)
    {
        objectiveText.text = text;
    }
}