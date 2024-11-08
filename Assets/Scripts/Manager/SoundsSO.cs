using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SoundsSO : ScriptableObject {
    public AudioClip PlayerHitWall;
    public AudioClip buttonPress;
    public float PlayerHitWallVolume = 1f;
    public float buttonPressVolume = .6f;
}
