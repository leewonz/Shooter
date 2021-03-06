﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Panda;


public class EnemyAI : MonoBehaviour {

    float FOV = 60;
    float playerBulletNear = 2;
    float sightLength = 50;
    LayerMask maskSolidOnly = 1 << 9;
    float destinationSize = 0.5f;

    Transform floorPoint;

    GameObject playerObject;
    Vector3 playerPosition = new Vector3();

    Vector3 moveDestination;
    List<Vector3> moveDestinations = new List<Vector3>();

    public float aimAngleH;
    public float aimAngleV;

    CharacterController controller;
    MovementHelper.Movement movement;

    Vector3 boxvis1;
    Vector3 boxvis2;
    Quaternion boxvis3;
    Vector3 boxvis4;
    float boxvis5;

    Vector3 linevis;

    GameObject gunInstance;
    EnemyGun gunScript;

    void Start() {
        controller = gameObject.GetComponent<CharacterController>();
        movement = new MovementHelper.Movement(gameObject, controller
            , _moveAcc: 75f, _moveMax: 6.0, _friction: 40f, _gravity: 27f);

        moveDestination = gameObject.transform.position;

        aimAngleH = gameObject.transform.rotation.eulerAngles.y;
        aimAngleV = 0;

        floorPoint = transform.Find("Floor Point");

        boxvis1 = new Vector3(0, 0, 0);
        boxvis2 = new Vector3(0, 0, 0);
        boxvis3 = Quaternion.identity;
        boxvis4 = new Vector3(0, 0, 0);
        boxvis5 = 0;

        linevis = new Vector3(0, 0, 0);

        playerObject = GameObject.FindGameObjectWithTag("Player");

        gunInstance = gameObject.transform.Find("Enemy Gun").gameObject;
        gunScript = gunInstance.GetComponent<EnemyGun>();

        //controller.enabled = false;
    }

    void Update() {
        playerPosition = playerObject.transform.position;

        movement.Update();

        gameObject.transform.rotation = Quaternion.Euler(0, aimAngleH, 0);

        BoxcastVisualization.DrawBoxCastBox(boxvis1
            , boxvis2, boxvis3
            , boxvis4, boxvis5, Color.red);

        //Debug.DrawLine(linevis, linevis + Vector3.down, Color.red);
    }

    private void OnDrawGizmos()
    {
        for (int i = 1; i < moveDestinations.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(moveDestinations[i - 1], moveDestinations[i]);
            Gizmos.DrawCube(moveDestinations[i], new Vector3(0.1f, 0.1f, 0.1f));
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(gameObject.transform.position, moveDestination);
        Gizmos.DrawSphere(moveDestination, 0.06f);
    }

    [Task]
    void IsPlayerSeen()
    {

        GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");

        float playerAngle = Vector3.Angle(
            gameObject.transform.rotation * Vector3.forward
            , (PlayerObj.transform.position - gameObject.transform.position).normalized);

        Task.current.debugInfo = "Angle: " + playerAngle.ToString();

        if (playerAngle <= FOV / 2)
        {
            if (!Physics.Raycast(gameObject.transform.position
                , (PlayerObj.transform.position - gameObject.transform.position).normalized
                , sightLength, maskSolidOnly)) //RAYCAST OPTIMISATION NEEDED
            {
                Task.current.Succeed(); return;
            }
        }
        Task.current.Fail();
    }

    [Task]
    void IsPlayerBulletNear()
    {
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("PlayerBullet"))
        {
            if ((bullet.transform.position - gameObject.transform.position).magnitude <= playerBulletNear)
            {
                Task.current.Succeed();
                return;
            }
            else { Task.current.Fail(); return; }
        }
        Task.current.Fail();
    }

    [Task]
    void SetRandomPosition(float minRange = 5.0f, float maxRange = 10.0f, int checkCount = 10)
    {
        Vector3? dest;

        for (int i = 0; i <= checkCount; i++)
        {
            dest = ReturnRandomPosition(minRange, maxRange);
            if (dest != null)
            {
                moveDestination = dest.Value;
                Task.current.debugInfo = "Dest: " + moveDestination.ToString();
                Task.current.Succeed(); return;
            }
        }
        Task.current.debugInfo = "Dest: null";
        Task.current.Fail();
    }

    [Task]
    void SetRandomFleePosition(float maxAngle = 90f
        , float minRange = 5.0f, float maxRange = 10.0f, int checkCount = 10)
    {
        Vector3 targetPosition = playerObject.transform.position;
        Vector3? dest;

        for (int i = 0; i <= checkCount; i++)
        {
            dest = ReturnRandomPosition(minRange, maxRange);
            if (dest != null)
            {
                Vector3 currentDirection = transform.position - targetPosition;
                Vector3 newDirection = dest.Value - targetPosition;

                if ((Quaternion.Angle(Quaternion.Euler(currentDirection), Quaternion.Euler(newDirection))
                    <= maxAngle )
                    && currentDirection.sqrMagnitude + (minRange* minRange) <= newDirection.sqrMagnitude)
                {
                    moveDestination = dest.Value;
                    Task.current.debugInfo = "Dest: " + moveDestination.ToString();
                    Task.current.Succeed(); return;
                }
            }
        }
        Task.current.debugInfo = "Dest: null";
        Task.current.Fail();
    }

    [Task]
    void SetRandomPath(float minRange = 5.0f, float maxRange = 10.0f, int checkCount = 10)
    {
        NavMeshPath destPath;

        for (int i = 0; i <= checkCount; i++)
        {
            destPath = ReturnRandomPath(minRange, maxRange);
            if (destPath != null)
            {
                // add path
                moveDestination = destPath.corners[1];
                
                moveDestinations.Clear();
                for (int j = 1; j < destPath.corners.Length; j++)
                {
                    moveDestinations.Add(destPath.corners[j]);
                }
                Task.current.debugInfo = "Dest count: " + destPath.corners.Length.ToString();
                Task.current.Succeed(); return;
            }
        }
        Task.current.debugInfo = "Path not found";
        Task.current.Fail();
    }

    [Task]
    void SetRandomFleePath(float maxAngle = 90f
        , float minRange = 5.0f, float maxRange = 10.0f, int checkCount = 10)
    {
        //SetRandomFleePath(90.0,2.0,6.0,10)
        NavMeshPath destPath;

        Vector3 targetPosition = playerObject.transform.position;

        for (int i = 0; i <= checkCount; i++)
        {
            destPath = ReturnRandomPath(minRange, maxRange);
            if (destPath != null)
            {
                //flee condition
                for (int k = 1; k < destPath.corners.Length; k++)
                {
                    Vector3 cornerPostiion = destPath.corners[k];

                    Vector3 currentDirection = transform.position - targetPosition;
                    Vector3 newDirection = cornerPostiion - targetPosition;

                    Task.current.debugInfo = currentDirection.sqrMagnitude.ToString() + " / "
                        + newDirection . sqrMagnitude.ToString();
                    // for each corner:
                    // is corner within target angle?
                    // and equal or farther than initial distance?
                    if ((Quaternion.Angle(Quaternion.Euler(currentDirection), Quaternion.Euler(newDirection))
                        <= maxAngle)
                        && currentDirection.sqrMagnitude <= newDirection.sqrMagnitude)
                    {
                        //for last corner: is corner farther than initial distance + minRange?
                        if ((k == destPath.corners.Length - 1)
                            && (currentDirection.sqrMagnitude + (minRange * minRange)
                            <= newDirection.sqrMagnitude))
                        {
                            //condition met for this corner iteration
                        }
                        else
                        {
                            Task.current.debugInfo = "Path not found";
                            Task.current.Fail(); return;
                        }
                    }
                    else
                    {
                        Task.current.debugInfo = "Path not found";
                        Task.current.Fail(); return;
                    }
                }

                {
                    // add path
                    moveDestination = destPath.corners[1];

                    moveDestinations.Clear();
                    for (int j = 1; j < destPath.corners.Length; j++)
                    {
                        moveDestinations.Add(destPath.corners[j]);
                    }
                    //Task.current.debugInfo = "Dest count: " + destPath.corners.Length.ToString();
                    Task.current.Succeed(); return;
                }
            }
        }
        Task.current.debugInfo = "Path not found";
        Task.current.Fail();
    }

    [Task]
    void SetRandomAttackPath(float minRange = 5.0f, float maxRange = 10.0f
        , float minPlayerDistance = 2.0f, int checkCount = 20)
    {
        NavMeshPath destPath;

        for (int i = 0; i <= checkCount; i++)
        {
            destPath = ReturnRandomPath(minRange, maxRange);
            if (destPath != null)//condition: path exists?
            {
                Vector3 destPos = destPath.corners[destPath.corners.Length - 1];

                if (!Physics.Linecast(playerPosition, destPos, maskSolidOnly)
                    && (destPos - playerPosition).sqrMagnitude
                    > (minPlayerDistance * minPlayerDistance))
                {
                    // add path
                    moveDestination = destPath.corners[1];

                    moveDestinations.Clear();
                    for (int j = 1; j < destPath.corners.Length; j++)
                    {
                        moveDestinations.Add(destPath.corners[j]);
                    }
                    Task.current.debugInfo = "Dest count: " + destPath.corners.Length.ToString();
                    Task.current.Succeed(); return;
                }
            }
        }
        Task.current.debugInfo = "Path not found";
        Task.current.Fail();
    }

    [Task]
    void MoveToPosition()
    {
        movement.Move((moveDestination - floorPoint.position).normalized);

        Task.current.debugInfo = (floorPoint.position - moveDestination).magnitude.ToString();

        if ((floorPoint.position - moveDestination).magnitude <= destinationSize / 2)
        {

            if (moveDestinations.Count > 1)
            {
                moveDestinations.RemoveAt(0);
                moveDestination = moveDestinations[0];
            }
        }

        Task.current.Succeed();
    }

    /*
    [Task]
    void IsInDestination()
    {
        if ((gameObject.transform.position - moveDestination).magnitude <= destinationSize / 2)
        {
            Task.current.Succeed(); return;
        }
        else { Task.current.Fail(); }
    }*/
    [Task]
    void IsInDestination()
    {
        if ((floorPoint.position - moveDestination).magnitude <= destinationSize / 2)
        {
            if (moveDestinations.Count <= 1)
            {
                Task.current.Succeed(); return;
            }
            else { Task.current.Fail(); }
        }
        else { Task.current.Fail(); }
    }



    [Task]
    void RotateToDestination(float amount = 360.0f)
    {
        Vector3 currentPos = gameObject.transform.position;

        float amountTimeBased = amount * Time.deltaTime;

        float currentRotation = aimAngleH;
        float destinationRotation = Quaternion.LookRotation(
            new Vector3(moveDestination.x - currentPos.x, currentPos.y
            , moveDestination.z - currentPos.z), Vector3.up).eulerAngles.y;

        float rotationDiff = Mathf.Abs(ClampAngle(destinationRotation)
            - ClampAngle(currentRotation));

        Task.current.debugInfo = "Angle: " + rotationDiff.ToString();

        if (rotationDiff < amountTimeBased ||
            (rotationDiff < 360.0f && rotationDiff > 360.0f - amountTimeBased))
        {
            aimAngleH = destinationRotation;
            Task.current.Succeed();
        }
        else
        {
            if (ClampAngle(destinationRotation - currentRotation) <= 180)
            {
                aimAngleH += amountTimeBased;
            }
            else
            {
                aimAngleH -= amountTimeBased;
            }
        }

    }

    [Task]
    void SetRandomAttackPosition(float minRange = 5.0f, float maxRange = 10.0f, int checkCount = 10)
    {

    }

    [Task]
    void RotateToPlayer(float amount = 360.0f)
    {
        Vector3 newDir;

        Quaternion newLookRot;

        float amountTimeBased = amount * Time.deltaTime;

        Vector3 currentPos = gameObject.transform.position;
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        Quaternion currentLookRot = Quaternion.Euler(aimAngleV, aimAngleH, 0);
        Quaternion playerLookRot = Quaternion.LookRotation(playerPos - currentPos, Vector3.up);

        float angle = Vector3.Angle(currentLookRot * Vector3.forward, playerLookRot * Vector3.forward);

        Task.current.debugInfo = "Angle: " + angle.ToString();

        if (angle <= amountTimeBased)
        {
            newLookRot = playerLookRot;
        }
        else
        {
            newLookRot = Quaternion.Lerp(currentLookRot, playerLookRot, amountTimeBased / angle);
        }
        newDir = Quaternion.LookRotation(newLookRot * Vector3.forward, Vector3.up).eulerAngles;

        aimAngleH = newDir.y;
        aimAngleV = newDir.x;

        Task.current.Succeed();
    }

    [Task]
    void FireAll()
    {

        gunScript.Fire(aimAngleH, aimAngleV);
        /*
        public float bulletCountCurrent;
        public float fireIntervalCurrent;
        public float reloadTimeCurrent;*/

        Task.current.debugInfo = string.Format("Bullets: {0} / {1} Reload: {2} / {3}",
            gunScript.bulletCountCurrent, gunScript.bulletCountMax
            , gunScript.reloadTimeCurrent, gunScript.reloadTimeMax);

        if (gunScript.bulletCountCurrent == 0)
        {
            Task.current.Succeed();
        }
    }
    

    [Task]
    void CheckDistanceFromPlayer(float distance)
    {

        if ((transform.position - playerObject.transform.position).sqrMagnitude < distance * distance)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }



    float ClampAngle(float angle)
    {
        if(angle > 360)
            angle -= 360;
        else if(angle < 0)
            angle += 360;
        return angle;
    }

    Vector3? ReturnRandomPosition(float minRange = 5.0f, float maxRange = 10.0f)
    {

        Vector3? dest = null;

        float direction = Random.Range(0, 360);
        float distance = Random.Range(minRange, maxRange);

        Transform thisTransform = gameObject.transform;

        Vector3 directionVector = Quaternion.Euler(0, direction, 0) * Vector3.forward;

        float forwardGap = 0.5f; // offset for start point of boxcast
        Vector3 forwardGapVector = thisTransform.rotation * Vector3.forward * forwardGap;

        if (Physics.BoxCast(
            floorPoint.position + (Vector3.up * 0.1f) + forwardGapVector
            , new Vector3(1,0.1f,1) * 0.4f, directionVector
            , Quaternion.Euler(0, direction, 0), distance - forwardGap, maskSolidOnly))
        {
            return null;
        }
        else
        {
            if (!Physics.Raycast(thisTransform.position + directionVector * distance
                , Vector3.down, 1.0f, maskSolidOnly))
            {
                return null;
            }
            else
            {
                /*BoxcastVisualization.DrawBoxCastBox(
                thisTransform.position + forwardGapVector
            , new Vector3(1, 0.1f, 1) * 0.4f, thisTransform.rotation
            , directionVector, distance - forwardGap, Color.red);*/
                boxvis1 = floorPoint.position + (Vector3.up * 0.1f) + forwardGapVector;
                boxvis2 = new Vector3(1, 0.1f, 1) * 0.4f;
                boxvis3 = Quaternion.Euler(0, direction, 0);
                boxvis4 = directionVector;
                boxvis5 = distance - forwardGap;

                linevis = thisTransform.position + directionVector * distance;

                //Debug.DrawLine(thisTransform.position + directionVector * distance,
                //    thisTransform.position + directionVector * distance + Vector3.down, Color.red);
                

                dest =
                    floorPoint.position + (Vector3.up * 0.1f) + directionVector * distance;
            }
        }
        return dest;
    }

    NavMeshPath ReturnRandomPath(float minRange = 5.0f, float maxRange = 10.0f) //returns null if path was not found
    {
        NavMeshPath destPath = new NavMeshPath();

        Vector3 dest;

        float direction = Random.Range(0, 360);
        float distance = Random.Range(minRange, maxRange);

        Transform thisTransform = gameObject.transform;

        Vector3 directionVector = Quaternion.Euler(0, direction, 0) * Vector3.forward;

        dest = thisTransform.position + directionVector * distance;

        NavMesh.CalculatePath(thisTransform.position, dest, NavMesh.AllAreas, destPath);

        if (destPath.status != NavMeshPathStatus.PathComplete)
        {
            return null;
        }
        else
        {
            float pathLength = 0;
            for (int i = 1; i < destPath.corners.Length; i++)
            {
                pathLength += (destPath.corners[i] - destPath.corners[i - 1]).magnitude;
            }
            if (pathLength >= maxRange)
            {
                return null;
            }
            else
            {
                return destPath;
            }
        }
    }

    /*
    [Task]
    void WaitRandom(float minSec, float maxSec)
    {
        float timeMax = 99.0f;
        float time = 99.0f;
        if (Task.current.isStarting)
        {
            timeMax = Random.Range(minSec, maxSec);
            time = timeMax;
        }
        time -= Time.deltaTime;
        Task.current.debugInfo = string.Format("t = {0} / {1}", time, timeMax);
        if (time <= 0)
        {
            Task.current.Succeed();
            Task.current.debugInfo = "t = 0 / " + timeMax.ToString();
            return;
        }
    }*/
}
