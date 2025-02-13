using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Waves", menuName = "Level Data/Level Waves")]
public class LevelWavesSO : ScriptableObject
{
    public string LevelName = "Encounter ";
    public List<Wave> Waves;
}
