using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour {

    CharacterController controller;
    MovementHelper.Movement movement;

    MoveCamera camera;

    GameObject currentGun;
    PlayerGun currentGunScript;

    float moveAcc = 0.03f;
    float moveMax = 0.09f;
    float moveFriction = 0.015f;
    float moveGravity = 0.45f;

    bool dashOn = false;
    float dashDirection = 0;
    float dashValueMax = 0.25f;
    float dashValueAirMax = 0.25f;
    float dashValue = 0;
    float dashMul = 0.96f;
    float dashDec = 0.002f;
    float dashSecondMul = 0.96f;
    float dashSecondDec = 0.001f;
    float dashTime = 0;
    float dashTimeMax = 0.25f;


    void Start () {
        controller = gameObject.GetComponent<CharacterController>();
        movement = new MovementHelper.Movement(gameObject, controller
            , _moveAcc: moveAcc, _moveMax: moveMax
            , _friction: moveFriction, _gravity: moveGravity);

        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MoveCamera>();

        currentGun = transform.Find("Handgun").gameObject;
        currentGunScript = currentGun.GetComponent<PlayerGun>();
    }
	
	
	void Update () {

        //Input - movement
        movement.Move(transform.rotation
            * (new Vector3(Input.GetAxisRaw("Horizontal") * 0.15f, 0
            , Input.GetAxisRaw("Vertical") * 0.15f)));
        //update movement
        movement.Update();

        //Input - jump
        if (Input.GetButtonDown("Jump") && movement.collisionBelow == true)
        {
            movement.Jump(0.12f);
        }

        //set viewbob
        if ((Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            || dashOn)
        {
            camera.viewBobOn = false;
        }
        else
        {
            camera.viewBobOn = true;
        }

        //Input - dash
        if (Input.GetButtonDown("Dash"))
        {
            DashStart();
        }
        if (Input.GetButtonUp("Dash"))
        { ;}
        if (Input.GetButton("Dash"))
        { ;}
        else
        {
            if ((dashTime >= dashTimeMax) && /*movement.collisionBelow &&*/ dashOn)
            {
                DashEnd();
            }
        }

        if (dashOn)
        {
            DashStep();
        }

        //Input - fire
        if (Input.GetButton("Fire"))
        { currentGunScript.fireTrigger = true; }
        else
        { currentGunScript.fireTrigger = false; }

        if (Input.GetButton("Fire2"))
        { currentGunScript.fireTrigger2 = true; }
        else
        { currentGunScript.fireTrigger2 = false; }

        if (Input.GetButtonDown("Fire"))
        { currentGunScript.fireTriggerDown = true; }
        else
        { currentGunScript.fireTriggerDown = false; }

        if (Input.GetButtonDown("Fire2"))
        { currentGunScript.fireTriggerDown2 = true; }
        else
        { currentGunScript.fireTriggerDown2 = false; }


        //Debug.Log(movement.collisionBelow.ToString());
        //Debug.Log((controller.collisionFlags & CollisionFlags.Below).ToString());
        /*
        character.Move( transform.rotation
            * (new Vector3(Input.GetAxisRaw("Horizontal") * 0.15f, 0
            , Input.GetAxisRaw("Vertical") * 0.15f)) );
        */

    }
    float AxisToDir(float x, float y)
    {
        if (x == 0 && y == 0) { return 0;}
        if (x == 0 && y >= 0) { return 0; }
        if (x == 0 && y <= 0) { return 180; }
        if (x <= 0 && y == 0) { return 270; }
        if (x <= 0 && y >= 0) { return 270 + 45; }
        if (x <= 0 && y <= 0) { return 270 - 45; }
        if (x >= 0 && y == 0) { return 90; }
        if (x >= 0 && y >= 0) { return 90 - 45; }
        if (x >= 0 && y <= 0) { return 90 + 45; }

        return 0;
    }

    void DashStart()
    {
        movement.isMoveEnabled = false;
        movement.isFrictionEnabled = false;
        dashValue = dashValueMax;

        dashDirection = (transform.rotation.eulerAngles.y)
            + AxisToDir(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        dashTime = 0.0f;

        dashOn = true;
    }

    void DashStep()
    {
        movement.SetMotion2d(Quaternion.Euler(0, dashDirection, 0) * new Vector3(0, 0, dashValue));
        if (movement.collisionBelow)
        {
            if (dashValue >= moveMax)
            {
                dashValue *= dashMul;
                dashValue = Mathf.Max(dashValue - dashDec, 0);
            }
            else
            {
                dashValue *= dashSecondMul;
                dashValue = Mathf.Max(dashValue - dashSecondDec, 0);
            }
        }
        dashTime += Time.deltaTime;
    }

    void DashEnd()
    {
        movement.isMoveEnabled = true;
        movement.isFrictionEnabled = true;
        dashOn = false;
    }
}

