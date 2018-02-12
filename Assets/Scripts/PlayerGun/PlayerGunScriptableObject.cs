using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun Name", menuName = "Custom/PlayerGun")]

public class PlayerGunScriptableObject : ScriptableObject {

    public enum GunType {projectile,hitscan};

    public GunType gunType = GunType.projectile;

    public bool holdClickToFire = true;

    public GameObject gunObj;
    public GameObject bulletObj;

    public int clipSize;
    public int maxBullet;

    public float fireInterval;
    public float reloadTime;

    public float spread;
    public float recoil;
    public float recoilMax;

    public float bulletSpeed;
}
