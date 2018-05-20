using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour {

    float health = 60;

    public Material hitMaterial;

    GameObject modelObject;
    Color modelOriginalColor;
    Material modelOriginalMaterial;
    MeshRenderer modelRenderer;

    float flashDuration = 0.05f;

    // Use this for initialization
    void Start () {
        modelObject = transform.Find("Model").gameObject;
        modelRenderer = modelObject.GetComponent<MeshRenderer>();
        modelOriginalColor = modelRenderer.material.color;
        modelOriginalMaterial = modelRenderer.material;
    }
	
	// Update is called once per frame
	void Update () {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
	}

    public void Hit(float damage)
    {
        this.health -= damage;
        Debug.Log(damage.ToString());
        StartCoroutine("FlashOnHit");
    }

    IEnumerator FlashOnHit()
    {
        modelRenderer.material.color = Color.white;
        modelRenderer.material = hitMaterial;
        for (float timeCurrent = 0; timeCurrent <= flashDuration; timeCurrent += Time.deltaTime)
        {
            yield return null;
        }
        modelRenderer.material.color = modelOriginalColor;
        modelRenderer.material = modelOriginalMaterial;
    }
}
