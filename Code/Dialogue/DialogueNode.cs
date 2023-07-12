using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewNode", menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    [TextArea(5, 20)]
    public string textID;
    public AudioClip[] audio;
    public DialogueNode TargetNode;
    public string emisorID;
    public Color nameColor;
}



