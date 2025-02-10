using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int SphereRadius;

    public int SectorSize;

    public float SphereRotationSpeed;

    public int ColorsCount;
}