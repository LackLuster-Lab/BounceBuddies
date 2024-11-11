using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SoundsSO : ScriptableObject {
    public AudioClip PlayerHitWall;
    public AudioClip buttonPress;
    public AudioClip Projectile;
    public AudioClip Explosion;
    public float PlayerHitWallVolume = 1f;
    public float buttonPressVolume = .6f;
    public float ProjectileVolume = 1f;
    public float ExplosionVolume = .6f;
}
