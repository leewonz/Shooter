using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MovementHelper;

public class MovePlayer : MonoBehaviour {

    CharacterController controller;
    Movement movement;
    MovementProfile movementProfileNormal;
    MovementProfile movementProfileAirdash;

    MoveCamera cameraScript;

    Transform thisTransform;
    GameObject currentGun;
    PlayerGun currentGunScript;

    float moveAcc = 90f;
    float moveMax = 6.2f;
    float moveFriction = 45f;
    float moveGravity = 26f;

    float moveAccAirdash = 48f;
    float moveFrictionAirdash = 20f;

    float jumpSpeed = 7.2f;

    bool dashOn = false;
    float dashDirection = 0;
    float dashValueMax = 24f;
    float dashValueAirMax = 11.2f;
    float dashValue = 0;

    float dashMul = 1f;
    float dashDec = 120f;
    float dashSecondMul = 1f;
    float dashSecondDec = 30f;

    float dashTime = 0;
    float dashTimeMax = 0.12f;

    float dashSteerTimeMax = 0.04f;

    bool wallJumpOn = false;
    float wallJumpCheckRadius = 1.0f;
    Vector3 wallJumpDirection;
    float wallJumpSpeedHorizontal = 7f;
    float wallJumpSpeedVertical = 6f;

    float wallJumpTime = 0;
    float wallJumpNoMoveTimeMax = 0.15f;

    LayerMask maskSolid = 1 << 9;
    LayerMask maskEnemy = 1 << 10;

    void Start () {
        controller = gameObject.GetComponent<CharacterController>();
        movement = new Movement(gameObject, controller
            , _moveAcc: moveAcc, _moveMax: moveMax
            , _friction: moveFriction, _gravity: moveGravity)
        {
            isRetainMotionEnabled = true
        };

        movementProfileNormal = new MovementProfile(_moveAcc: moveAcc, _moveMax: moveMax
            , _friction: moveFriction, _gravity: moveGravity,_isMoveEnabled: true
            , _isFrictionEnabled: true,_isGravityEnabled:true, _isRetainMotionEnabled: false);

        movementProfileAirdash = new MovementProfile(_moveAcc: moveAccAirdash, _moveMax: moveMax
            , _friction: moveFrictionAirdash, _gravity: moveGravity, _isRetainMotionEnabled: true);


        cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MoveCamera>();

        currentGun = transform.Find("Main Camera/Handgun").gameObject;
        currentGunScript = currentGun.GetComponent<PlayerGun>();

        thisTransform = gameObject.transform;
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
            movement.Jump(jumpSpeed);
        }

        //set viewbob
        if ((Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            || dashOn)
        {
            cameraScript.viewBobOn = false;
        }
        else
        {
            cameraScript.viewBobOn = true;
        }

        //Input - dash
        if (Input.GetButtonDown("Dash"))
        {
            WallJumpEnd();
            DashStart();
        }
        if (Input.GetButtonUp("Dash"))
        { ;}
        if (Input.GetButton("Dash"))
        { ;}
        /*
        else
        {
            if ((dashTime >= dashTimeMax) && dashOn)
            {
                DashEnd();
            }
        }*/
        if ((dashTime >= dashTimeMax) && dashOn)
        {
            DashEnd();
        }

        if (dashOn)
        {
            DashStep();
        }

        if (!dashOn && movement.collisionBelow)
        {
            movement.LoadProfile(movementProfileNormal);
        }

        //Input - wallJump
        
        if ( !movement.collisionBelow && Input.GetButtonDown("Jump") && !wallJumpOn && !dashOn)
        {
            if (WallJumpCheck())
            {
                WallJumpStart();
            }
        }

        if (wallJumpOn)
        {
            WallJumpStep();
        }

        if (movement.collisionBelow && wallJumpOn)
        {
            WallJumpEnd();
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

        //Dash always can interrupt WallJump
        //WallJump cannot interrupt Dash
        dashOn = true;
    }

    void DashStep()
    {
        if (dashTime <= dashSteerTimeMax)
        {
            dashDirection = (transform.rotation.eulerAngles.y)
                + AxisToDir(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        movement.SetMotion2d(Quaternion.Euler(0, dashDirection, 0) * new Vector3(0, 0, dashValue));
        if (movement.collisionBelow)
        {
            if (dashValue >= moveMax)
            {
                dashValue *= dashMul;
                dashValue = Mathf.Max(dashValue - dashDec * (Time.deltaTime), 0);
            }
            else
            {
                dashValue *= dashSecondMul;
                dashValue = Mathf.Max(dashValue - dashSecondDec * (Time.deltaTime), 0);
            }
        }
        else
        {
            movement.LoadProfile(movementProfileAirdash);
            dashValue = Mathf.Min(dashValue, dashValueAirMax);
        }
        dashTime += Time.deltaTime;
    }

    void DashEnd()
    {
        movement.isMoveEnabled = true;
        movement.isFrictionEnabled = true;
        dashOn = false;
    }

    bool WallJumpCheck()
    {
        /*
        Collider[] doubleJumpWalls = Physics.OverlapSphere(
            gameObject.transform.position, doubleJumpCheckRadius, maskSolid);
        */
        RaycastHit rayHitLeft = new RaycastHit();
        RaycastHit rayHitRight = new RaycastHit();
        RaycastHit rayHit = new RaycastHit();

        int wallJumpSide = 0; // -1: left / 1: right //UNUSED

        float cameraRotationH = cameraScript.cameraHorizontalAngle;

        //check left
        bool lineLeftExists = Physics.Linecast(thisTransform.position
            , thisTransform.position
            + Quaternion.Euler(0, cameraRotationH - 90, 0) * Vector3.forward * wallJumpCheckRadius
            , out rayHitLeft, maskSolid);

        //check right
        bool lineRightExists = Physics.Linecast(thisTransform.position
            , thisTransform.position
            + Quaternion.Euler(0, cameraRotationH + 90, 0) * Vector3.forward * wallJumpCheckRadius
            , out rayHitRight, maskSolid);

        //check sides and transfer hit info to rayHit
        if (lineLeftExists && !lineRightExists)
        { rayHit = rayHitLeft; wallJumpSide = -1; }
        if (!lineLeftExists && lineRightExists)
        { rayHit = rayHitRight; wallJumpSide = 1; }
        if (lineLeftExists && lineRightExists)
        {
            if (rayHitLeft.distance < rayHitRight.distance) // left
            { rayHit = rayHitLeft; wallJumpSide = 1; }
            else
            { rayHit = rayHitRight; wallJumpSide = -1; }
        }
        if (!lineLeftExists && !lineRightExists)
        { return false; }
        
        wallJumpDirection = rayHit.normal;

        movement.LoadProfile(movementProfileAirdash);

        return true;
    }

    void WallJumpStart()
    {
        wallJumpOn = true;
        movement.AddMotion2d(wallJumpDirection * wallJumpSpeedHorizontal);
        movement.Jump(wallJumpSpeedVertical);

        movement.isMoveEnabled = false;

        wallJumpTime = 0.0f;
    }

    void WallJumpStep()
    {
        wallJumpTime += Time.deltaTime;
        if (wallJumpTime >= wallJumpNoMoveTimeMax)
        {
            movement.isMoveEnabled = true;
        }
    }

    void WallJumpEnd()
    {
        if (!dashOn)
        {
            wallJumpOn = false;

            movement.isMoveEnabled = true;

            movement.LoadProfile(movementProfileNormal);
        }
    }
    
}

