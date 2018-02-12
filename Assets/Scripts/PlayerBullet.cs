using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {

    public float speedInit;
    public Vector3 directionInit;

    float speed;
    Vector3 direction;

    // Use this for initialization
    void Start () {
        speed = speedInit;
        directionInit = directionInit.normalized;
        direction = directionInit;
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position += direction * speed;

    }
}
