using UnityEngine;

[CreateAssetMenu(fileName ="ObjectiveSO", menuName ="Objective")]
public class ObjectiveSO : ScriptableObject
{
    public ObjectiveLine[] lines;
}

[System.Serializable]
public class ObjectiveLine
{
    [TextArea(1,2)]public string text;
}