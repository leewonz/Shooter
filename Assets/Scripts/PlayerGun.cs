using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

    public PlayerGunScriptableObject gun;

    [HideInInspector]
    public bool fireTrigger = false;
    [HideInInspector]
    public bool fireTrigger2 = false;
    [HideInInspector]
    public bool fireTriggerDown = false;
    [HideInInspector]
    public bool fireTriggerDown2 = false;

    float fireIntervalCurrent;
    float reloadTimeCurrent;

    int layerMaskPlayer = 1 << 2;
    int layerMaskEnemy = 1 << 10;
    int layerMaskSolid = 1 << 9;

    GameObject bullet;
    GameObject lightObj;
    GameObject fireObj;
    PlayerBullet bulletScript;
    GameObject camera;
    MoveCamera cameraScript;
    Transform gunTipTransform;

    float currentRecoil = 0;

    float bulletClippingDistanceMin = 4.0f;

    Quaternion bulletDirection; 

    // Use this for initialization
    void Start () {
        fireIntervalCurrent = 0;
        reloadTimeCurrent = 0;

        camera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraScript = camera.GetComponent<MoveCamera>();

        gunTipTransform = gameObject.transform.Find("GunModel/GunTip");
    }
	
	// Update is called once per frame
	void Update () {
        
        //update
        fireIntervalCurrent += Time.deltaTime;
        reloadTimeCurrent += Time.deltaTime;

        //gameObject.transform.localRotation
        //    = (Quaternion.Euler(new Vector3(camera.GetComponent<MoveCamera>().cameraVerticalAngle, 0, 0)));
        gameObject.transform.localPosition = 
            (new Vector3(0
            , Mathf.Asin(cameraScript.cameraVerticalAngle / -90) * -(Mathf.Rad2Deg / 90) * 0.25f
            + cameraScript.viewBobValue * -cameraScript.viewBobValueMul
            , 0));

        if ((fireTrigger && gun.holdClickToFire)
            || (fireTriggerDown && !gun.holdClickToFire))
        {
            if (fireIntervalCurrent >= gun.fireInterval)
            {
                currentRecoil = Mathf.Min(currentRecoil + gun.recoil, gun.recoilMax);

                fireIntervalCurrent = 0.0f;

                //set direction for bullet
                int _rayMask = ~(1 << 8 | 1 << 2); // excludes Player and IgnoreRaycast
                RaycastHit _rayHit;
                bool _isRay = Physics.Raycast(camera.transform.position
                    , Quaternion.Euler(cameraScript.cameraVerticalAngle,
                    cameraScript.cameraHorizontalAngle, 0) * Vector3.forward, out _rayHit
                    , maxDistance: Mathf.Infinity, layerMask: _rayMask);

                float _rayHitDistance;
                
                for (int i = 1; i <= gun.bulletCount; i++)
                {
                    //create bullet
                    bullet = Instantiate(gun.bulletObj);
                    bullet.transform.position = gunTipTransform.position;
                    bulletScript = bullet.GetComponent<PlayerBullet>();

                    //create light
                    if (gun.lightObj != null && (i == 1))
                    {
                        lightObj = Instantiate(gun.lightObj);
                        lightObj.transform.position = gunTipTransform.position;
                    }
                    //create fire
                    if (gun.gunFireObj != null && (i == 1))
                    {
                        fireObj = Instantiate(gun.gunFireObj);
                        fireObj.transform.position = gunTipTransform.position;
                        fireObj.transform.parent = gameObject.transform;
                    }
                    

                    //set random spread
                    Quaternion spreadRandomRotation = Random.rotation;
                    //float spreadRandomAngle = Random.Range(0, gun.spread + currentRecoil);
                    float spreadRandomAngle = currentRecoil + gun.spread;
                    //Debug.Log("Hi " + gun.spread);

                    //ray points to object?
                    if (_isRay/* && (_rayHit.point - transform.position).sqrMagnitude >= 1f*1f*/)
                    {
                        bulletDirection = Quaternion.LookRotation(
                            _rayHit.point - bullet.transform.position, Vector3.up);
                        _rayHitDistance = _rayHit.distance;
                    }
                    else//ray points to nothing?
                    {
                        bulletDirection = Quaternion.Euler(cameraScript.cameraVerticalAngle,
                        cameraScript.cameraHorizontalAngle, 0);
                        _rayHitDistance = Mathf.Infinity;
                    }
                    //apply random spread
                    //bulletDirection = Quaternion.RotateTowards(bulletDirection, spreadRandomRotation
                    //        , spreadRandomAngle);
                    bulletDirection = Quaternion.Lerp(
                        bulletDirection
                        , spreadRandomRotation
                        , spreadRandomAngle / 180);

                    // apply direction for bullet
                    bullet.transform.rotation = bulletDirection;
                    bulletScript.directionInit = bulletDirection * Vector3.forward;
                    bulletScript.speedInit = gun.bulletSpeed;

                    // apply direction for light
                    lightObj.transform.rotation = bulletDirection;
                    // apply direction for fire
                    fireObj.transform.rotation = bulletDirection;

                    // apply life for bullet
                    bulletScript.lifeMax = gun.bulletLife;

                    
                    bool? gunTip_HitPointLineCast = null;
                    bool body_GunTipLineCast = false;
                    
                    //gunTip - hitPoint lineCast
                    if (Physics.Linecast(gunTipTransform.position
                        , _rayHit.point + (gunTipTransform.position - _rayHit.point).normalized * 0.5f
                        , layerMaskSolid) || Physics.CheckSphere(gunTipTransform.position,0.01f))
                    {
                        if (_rayHitDistance >= bulletClippingDistanceMin) //destination is far enough
                        {
                            if (Physics.OverlapSphere(gunTipTransform.position
                                + (_rayHit.point - gunTipTransform.position).normalized
                                * bulletClippingDistanceMin, 0.01f, layerMaskSolid).Length == 0)
                            // is clipping end point is free?
                            {
                                gunTip_HitPointLineCast = true;
                                //Debug.Log("gunTip_HitPointLineCast = " + gunTip_HitPointLineCast.ToString());
                            }
                            else
                            { gunTip_HitPointLineCast = false;
                                //Debug.Log("A");
                            }
                        }
                        else
                        { gunTip_HitPointLineCast = false;
                            //Debug.Log("B");
                        }
                    }
                    else
                    { gunTip_HitPointLineCast = false;
                        //Debug.Log("C");
                    }

                    if (gunTip_HitPointLineCast.HasValue)
                    {
                        if (gunTip_HitPointLineCast.Value)
                        {
                            bulletScript.clippingDistance = bulletClippingDistanceMin;
                            bulletScript.clippingEnabled = true;
                        }
                        else
                        {
                            bulletScript.clippingDistance = 0.0f;
                            bulletScript.clippingEnabled = false;
                        }
                    }
                    else { Debug.LogWarning("gunTip_HitPointLineCast in PlayerGun has no value"); }

                    //body - gunTip lineCast
                    if (Physics.Linecast(transform.position, gunTipTransform.position, layerMaskSolid))
                    {
                        if (_rayHitDistance < bulletClippingDistanceMin)
                        {
                            
                            bulletScript.lifeMax = 0.0f;
                            
                        }
                    }
                }
            }
        }
        if (fireIntervalCurrent >= gun.fireInterval)
        {
            currentRecoil = Mathf.Max(0, currentRecoil - gun.recoilDecrease * Time.deltaTime);
        }

    }

    public float CurrentSpread()
    {
        return gun.spread + currentRecoil;
    }
}
