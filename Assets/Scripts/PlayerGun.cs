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

    GameObject bullet;
    PlayerBullet bulletScript;
    GameObject camera;
    MoveCamera cameraScript;

    Quaternion bulletDirection; 

    // Use this for initialization
    void Start () {
        fireIntervalCurrent = 0;
        reloadTimeCurrent = 0;

        camera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraScript = camera.GetComponent<MoveCamera>();
        
    }
	
	// Update is called once per frame
	void Update () {
        //update
        fireIntervalCurrent += Time.deltaTime;
        reloadTimeCurrent += Time.deltaTime;

        gameObject.transform.localRotation
            = (Quaternion.Euler(new Vector3(camera.GetComponent<MoveCamera>().cameraVerticalAngle, 0, 0)));

        if ((fireTrigger && gun.holdClickToFire)
            || (fireTriggerDown && !gun.holdClickToFire))
        {
            if (fireIntervalCurrent >= gun.fireInterval)
            {
                fireIntervalCurrent = 0.0f;

                //create bullet
                bullet = Instantiate(gun.bulletObj);
                bullet.transform.position = gameObject.transform.Find("GunTip").position;
                bulletScript = bullet.GetComponent<PlayerBullet>();

                //set direction for bullet
                int _rayMask = ~(1 << 8 + 1 << 2); // excludes Player and IgnoreRaycast
                RaycastHit _rayHit;
                bool _isRay = Physics.Raycast(camera.transform.position
                    , Quaternion.Euler(cameraScript.cameraVerticalAngle,
                    cameraScript.cameraHorizontalAngle, 0) * Vector3.forward, out _rayHit
                    , maxDistance: Mathf.Infinity ,layerMask: _rayMask);

                if (_isRay)
                {
                    bulletDirection = Quaternion.LookRotation(
                        _rayHit.point - bullet.transform.position, Vector3.up);
                }
                else
                {
                    bulletDirection = Quaternion.Euler(cameraScript.cameraVerticalAngle,
                    cameraScript.cameraHorizontalAngle, 0);
                }
                
                // apply direction for bullet
                bullet.transform.rotation = bulletDirection;
                bulletScript.directionInit = bulletDirection * Vector3.forward;
                bulletScript.speedInit = gun.bulletSpeed;
            }
        }
    }
}
