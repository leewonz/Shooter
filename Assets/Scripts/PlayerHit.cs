using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour {

    float health = 8;
    bool isHit = false;

    DrawGUI drawGui;

	// Use this for initialization
	void Start () {
        drawGui = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DrawGUI>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Hit(float damage)
    {
        this.health -= damage;
        Debug.Log(damage.ToString());

        drawGui.HitEffect();
    }
}
