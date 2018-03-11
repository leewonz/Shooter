using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

    [HideInInspector]
    public float speedInit;
    [HideInInspector]
    public Vector3 directionInit;
    [HideInInspector]
    public Vector3 positionInit;

    float speed;
    Vector3 direction;

    Vector3 thisPos;

    int layerMaskPlayer = 1 << 8;
    int layerMaskEnemy = 1 << 10;
    int layerMaskSolid = 1 << 9;

    [HideInInspector]
    public float lifeMax = 0;
    float lifeCurrent = 0; //increments till lifeMax
    [HideInInspector]
    public bool clippingEnabled;//unused
    [HideInInspector]
    public float clippingDistance = 0.0f;//unused

    public GameObject hitEffect;


    // Use this for initialization
    void Start()
    {
        speed = speedInit;
        directionInit = directionInit.normalized;
        direction = directionInit;

        positionInit = transform.position;

        thisPos = gameObject.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {

        thisPos = gameObject.transform.position;
        

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
            if ((1 << hit.transform.gameObject.layer) == layerMaskSolid)//solid
            {
                if ((clippingEnabled == false
                    || (thisPos - positionInit).sqrMagnitude >= clippingDistance * clippingDistance))
                {
                    MakeHitEffect(hitEffect, hit.point - (direction * 0.2f)
                        , Quaternion.LookRotation(direction, Vector3.up));
                    Destroy(gameObject);
                }
            }
            if ((1 << hit.transform.gameObject.layer) == layerMaskPlayer)//enemy
            {
                MakeHitEffect(hitEffect, hit.point - (direction * 0.2f)
                    , Quaternion.LookRotation(direction, Vector3.up));

                //hit.transform.parent.gameObject.GetComponent<EnemyHit>().Hit(10);

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
}
