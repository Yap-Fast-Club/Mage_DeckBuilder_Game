using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order =0)]
public class CardSO : ScriptableObject
{
    public string Title;
    [ResizableTextArea]
    public string Description;
    public uint Cost;
    public uint Damage;



}
