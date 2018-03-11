using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimation : MonoBehaviour {

    //public int modelCount = 0;
    public Mesh[] mesh;
    public float frameTime;
    public bool repeat;
    public bool destroyParent;

    MeshFilter meshFilter;

    int frameNumber = 0;
    float frameTimeCurrent;

	// Use this for initialization
	void Start () {
        meshFilter = gameObject.GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update() {

        frameTimeCurrent += Time.deltaTime;
        if (frameTimeCurrent >= frameTime)
        {
            frameNumber++;
            frameTimeCurrent -= frameTime;

            if (frameNumber > mesh.Length - 1)
            {
                if (repeat)
                {
                    frameNumber = 0;
                }
                else
                {
                    if (destroyParent)
                    {
                        Destroy(gameObject.transform.parent.gameObject);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }

                }

            }
            else
            {
                meshFilter.mesh = mesh[frameNumber];
            }
        }
    }
}
