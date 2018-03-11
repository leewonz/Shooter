using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {

    public float speedInit;
    public Vector3 directionInit;

    public Vector3 positionInit;

    float speed;
    Vector3 direction;

    Vector3 thisPos;

    int layerMaskEnemy = 1 << 10;
    int layerMaskSolid = 1 << 9;

    public float lifeMax = 0;
    float lifeCurrent = 0; //increments till lifeMax

    public bool clippingEnabled;
    public float clippingDistance = 0.0f;

    public GameObject hitEffect;
    GameObject playerObject;
    GameObject cameraObject;
    Vector3 CameraRotation;

    // Use this for initialization
    void Start () {
        speed = speedInit;
        directionInit = directionInit.normalized;
        direction = directionInit;

        positionInit = transform.position;

        thisPos = gameObject.transform.position;

        playerObject = GameObject.FindGameObjectWithTag("Player"); //unused
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera"); //unused
        CameraRotation = new Vector3(cameraObject.GetComponent<MoveCamera>().cameraVerticalAngle
            , cameraObject.GetComponent<MoveCamera>().cameraHorizontalAngle,0); //unused
        
    }
	
	// Update is called once per frame
	void Update () {
        
        thisPos = gameObject.transform.position;

        CameraRotation = new Vector3(cameraObject.GetComponent<MoveCamera>().cameraVerticalAngle
            , cameraObject.GetComponent<MoveCamera>().cameraHorizontalAngle, 0);

        //life
        lifeCurrent += Time.deltaTime;
        if (lifeCurrent >= lifeMax)
        {
            MakeHitEffect(hitEffect, gameObject.transform.position - (direction * 0.2f)
                , Quaternion.LookRotation(direction, Vector3.up));
            Destroy(gameObject);
        }

        //check if bullet is hit
        //raycast init
        RaycastHit hit = new RaycastHit();
        bool isRayHit = Physics.Raycast(thisPos, direction, out hit, speed * Time.deltaTime
            , layerMaskEnemy | layerMaskSolid);

        //is ray hit by something?
        if (isRayHit)
        {
            if (hit.transform.gameObject.layer == 9)//solid
            {
                if ((clippingEnabled == false
                    || (thisPos - positionInit).sqrMagnitude >= clippingDistance * clippingDistance))
                {
                    MakeHitEffect(hitEffect, hit.point - (direction * 0.2f)
                        , Quaternion.LookRotation(direction, Vector3.up));
                    Destroy(gameObject);
                }
            }
            if (hit.transform.gameObject.layer == 10)//enemy
            {
                MakeHitEffect(hitEffect, hit.point - (direction * 0.2f)
                    , Quaternion.LookRotation(direction, Vector3.up));

                hit.transform.parent.gameObject.GetComponent<EnemyHit>().Hit(10);

                Destroy(gameObject);
            }
        }

        //update movement
        gameObject.transform.position += direction * (speed * Time.deltaTime);
        
    }

    void MakeHitEffect(GameObject obj, Vector3 position, Quaternion rotation)
    {
        GameObject hitEffectInstance;
        hitEffectInstance = Instantiate(obj);
        hitEffectInstance.transform.position = position;
        hitEffectInstance.transform.rotation = rotation;
    }
    /*
        Collider[] startHitColliders = Physics.OverlapSphere(thisPos, 0.01f, layerMaskEnemy| layerMaskSolid);
        //Debug.Log(startHitColliders[0].ToString());
        if (startHitColliders.Length >= 1)
        {
            if (1 << (startHitColliders[0].gameObject.layer) == layerMaskSolid)
            {
                MakeHitEffect(hitEffect, thisPos - (direction * 0.1f)
                        , Quaternion.LookRotation(Quaternion.Euler(CameraRotation) * Vector3.forward, Vector3.up));
                Destroy(gameObject);
            }
            if (1 << (startHitColliders[0].gameObject.layer) == layerMaskEnemy)
            {
                MakeHitEffect(hitEffect, thisPos - (direction * 0.1f)
                        , Quaternion.LookRotation(Quaternion.Euler(CameraRotation) * Vector3.forward, Vector3.up));

                startHitColliders[0].gameObject.transform.parent.gameObject.GetComponent<EnemyHit>().Hit(10);

                Destroy(gameObject);
            }
        }*/
}


