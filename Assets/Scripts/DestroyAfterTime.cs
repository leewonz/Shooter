using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

    public float time;
    float timeCurrent = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timeCurrent += Time.deltaTime;
        if (timeCurrent > time)
        {
            Destroy(gameObject);
        }

    }
}
