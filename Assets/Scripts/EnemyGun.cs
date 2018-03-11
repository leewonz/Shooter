using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour {

    public GameObject bulletObj;
    public int bulletCountMax = 3;
    public float reloadTimeMax = 2;
    public float fireInterval = 0.25f;
    public float spread = 2;
    public float bulletSpeed = 30.0f;
    public float bulletLife = 2f;

    [HideInInspector]
    public float bulletCountCurrent;
    [HideInInspector]
    public float fireIntervalCurrent;
    [HideInInspector]
    public float reloadTimeCurrent;

    float maxFireDistance = 200f;

    Transform enemyTransform;
    Transform gunTipTransform;

    // Use this for initialization
    void Start () {
        float bulletCountCurrent = bulletCountMax;

        enemyTransform = transform.parent;
        gunTipTransform = transform.Find("GunTip");
    }
	
	// Update is called once per frame
	void Update () {
        fireIntervalCurrent += Time.deltaTime;
        if (bulletCountCurrent <= 0)
        {
            reloadTimeCurrent += Time.deltaTime;
            if (reloadTimeCurrent >= reloadTimeMax)
            {
                bulletCountCurrent = bulletCountMax;
            }
        }
    }

    public void Fire(float horizontalAngle, float verticalAngle)
    {
        if (fireIntervalCurrent > fireInterval && bulletCountCurrent > 0)
        {
            Quaternion bulletRotation;
            GameObject bullet;
            EnemyBullet bulletScript;

            Quaternion spreadRandomRotation = Random.rotation;
            float spreadRandomAngle = spread;

            int _rayMask = 1 << 8 | 1 << 9; // includes Player and solid
            RaycastHit _rayHit;
            bool _isRay = Physics.Raycast(transform.position
                , Quaternion.Euler(verticalAngle, horizontalAngle, 0) * Vector3.forward
                , out _rayHit, maxDistance: maxFireDistance, layerMask: _rayMask);

            bullet = Instantiate(bulletObj);
            bullet.transform.position = gunTipTransform.position;
            bulletScript = bullet.GetComponent<EnemyBullet>();

            //ray points to object?
            if (_isRay/* && (_rayHit.point - transform.position).sqrMagnitude >= 1f*1f*/)
            {
                bulletRotation = Quaternion.LookRotation(
                    _rayHit.point - bullet.transform.position, Vector3.up);
            }
            else//ray points to nothing?
            {
                bulletRotation = Quaternion.Euler(verticalAngle,
                horizontalAngle, 0);
            }
            //apply random spread
            bulletRotation = Quaternion.RotateTowards(bulletRotation, spreadRandomRotation
                    , spreadRandomAngle);

            bullet.transform.rotation = bulletRotation;
            bulletScript.directionInit = bulletRotation * Vector3.forward;
            bulletScript.speedInit = bulletSpeed;
            bulletScript.lifeMax = bulletLife;


            bulletCountCurrent--;
            fireIntervalCurrent = 0;

            if (bulletCountCurrent == 0)
            {
                reloadTimeCurrent = 0;
            }
        }
    }
}
