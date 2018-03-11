using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveCamera : MonoBehaviour {
    public int sensitivity;

    
    public float cameraHorizontalAngle = 0;
    
    public float cameraVerticalAngle = 0;

    float sensitivityMul = 0.2f;
    GameObject parent;
    GameObject camera;

    [HideInInspector]
    public bool viewBobOn = false;
    bool viewBobLock = false;
    float viewBobTime = 0;
    float viewBobTimeMul = 600.0f;
    float viewBobValue = 0.0f;
    float viewBobValueMul = 0.04f;
    int viewBobCount = 0;
    Vector3 cameraLocalPos;


    // Use this for initialization
    void Start () {
        parent = gameObject.transform.parent.gameObject;
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraLocalPos = camera.transform.localPosition;

        //cameraHorizontalAngle = parent.transform.rotation.y;
        //cameraVerticalAngle = 0;
    }
	
	// Update is called once per frame
	void Update () {
        cameraHorizontalAngle += 
            Input.GetAxisRaw("Mouse X") * sensitivity * sensitivityMul;

        cameraVerticalAngle -=
            Input.GetAxisRaw("Mouse Y") * sensitivity * sensitivityMul;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -90.0f, 90.0f);


    }

    void LateUpdate()
    {
        parent.transform.rotation =
    Quaternion.Euler(new Vector3(0, cameraHorizontalAngle, 0));
        camera.transform.localRotation =
            Quaternion.Euler(new Vector3(cameraVerticalAngle, 0, 0));

        if (viewBobOn)
        {
            viewBobLock = false;
            viewBobTime += Time.deltaTime;

            viewBobValue = Mathf.Abs(
                Mathf.Sin(Mathf.Deg2Rad * viewBobTime * viewBobTimeMul));

            viewBobCount = Mathf.CeilToInt(
                (viewBobTime * viewBobTimeMul) / 180.0f);
        }

        if (!viewBobOn)
        {
            if (viewBobCount < Mathf.CeilToInt(
                (viewBobTime * viewBobTimeMul) / 180.0f))
            {
                viewBobTime = 0;
                viewBobCount = 0;
                viewBobLock = true;
            }
            else
            {
                if (viewBobLock == false)
                {
                    viewBobTime += Time.deltaTime;

                    viewBobValue = Mathf.Abs(
                        Mathf.Sin(Mathf.Deg2Rad * viewBobTime * viewBobTimeMul));
                }
            }
        }
        //Debug.Log(viewBobCount.ToString());
        camera.transform.localPosition =
            cameraLocalPos + new Vector3(0
            , viewBobValue * viewBobValueMul, 0);
    }
}
