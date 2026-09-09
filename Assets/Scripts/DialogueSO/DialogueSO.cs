using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public DialogueLine[] lines;

    public bool unlockGirlBarrier;
    public bool lockedExit;
    public bool talkedToStatue;

    public bool makeGirlDisappear;
    public int disappearAtLine;
    public bool enableFog;
    public bool teleportToStatue;
    public bool playChurchBells;

    public bool autoAdvance;
    public float autoAdvanceDelay = 2f;

    public bool startEnding;
    public int endingNumber = 3;

    public bool stopGirlCrying;

    public bool enableChoice;
    public int choiceLineIndex;

    public DialogueSO yesDialogue;
    public DialogueSO noDialogue;

    public bool enableStatueBackground;
    public StatueBackground[] statueBackgrounds;

    public bool enableStatueInteractionAudio;

    public bool playTimelineAfterDialogue;
    public PlayableAsset timelineToPlay;
    public DialogueSO dialogueAfterTimeline;
}

[System.Serializable]
public class StatueBackground
{
    public Sprite backgroundImage;
    public int changeAtLine;
}

[System.Serializable]
public class DialogueLine
{
    public ActorSO speaker;

    [TextArea(3, 60)]
    public string text;
}