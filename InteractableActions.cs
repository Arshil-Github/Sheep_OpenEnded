using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DragDrop Object")]
public class Interactable : ScriptableObject
{
    public string objectName;
    public Sprite sprite;

    public AnimationClip animToPlay;
}
