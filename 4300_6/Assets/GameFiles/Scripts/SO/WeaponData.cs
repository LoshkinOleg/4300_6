using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "SO/WeaponData")]
public class WeaponData : ScriptableObject
{
    public float firerate = 1;
    public float projectileSpeed = 1;
    public int damage = 1;
    public float spread = 1;
    public float firingKnockback = 1;
    public float hitKnockback = 1;
    public int numberOfProjectiles = 1;
    public int ammo = 1;
}
