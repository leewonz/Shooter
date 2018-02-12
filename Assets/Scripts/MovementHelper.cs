using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHelper : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public class Movement
    {
        //requirements
        //1. motion of any direction can be applied.
        //2. has move() function(as a default method for character movement).
        //3. has Update() function where transform position and collision is
        //    calculated.
        //4. friction is applied unless move() is at least once called before
        //    previous Update() call.
        //5. has collisionStep var for optimization of collision detection.

        //Public
        Vector2 motion;
        float motionV;

        GameObject thisObj;
        CharacterController thisController;

        double moveAcc, moveMax;
        double friction;
        float gravity;

        Vector2 moveDirection;
        //double collisionStep;

        public bool isMoveEnabled;
        public bool isFrictionEnabled;
        public bool isGravityEnabled;

        public bool collisionBelow;

        //Private
        bool isMoved;
        

        public Movement(GameObject obj, CharacterController _thisController, double _moveAcc = 0.05
            , double _moveMax = 0.2, double _friction = 0.02, float _gravity = 9.81f)
        {
            
            thisObj = obj;
            thisController = _thisController;

            moveAcc = _moveAcc; moveMax = _moveMax; friction = _friction;
            gravity = _gravity;

            motion = new Vector2(0, 0);
            motionV = 0;
            isMoved = false;
            isMoveEnabled = true;
            isFrictionEnabled = true;
            isGravityEnabled = true;
        }

        public Vector2 GetMotion() { return motion; }

        public void SetMotion(Vector3 direction)
        {
            motion = new Vector2(direction.x,direction.z);
            motionV = direction.y;
        }

        public void SetMotion2d(Vector3 direction)
        {
            motion = new Vector2(direction.x, direction.z);
        }

        public void AddMotion(Vector3 direction)
        {
            motion += new Vector2(direction.x, direction.z);
            motionV += direction.y;
        }

        public void AddMotion2d(Vector3 direction)
        {
            motion += new Vector2(direction.x, direction.z);
        }

        public void Move(Vector3 direction)
        {
            Vector2 dir2 = new Vector2(direction.x, direction.z);
            if (dir2.magnitude < 0.01f)
            {; }
            else
            {
                moveDirection = dir2.normalized;
                isMoved = true;
            }
        }

        public void Jump(float speed)
        {
            motionV = speed;
        }

        public void Update()
        {
            //1. apply friction
            //2. calculate collision
            if (isMoved && isMoveEnabled)
            {
                motion += (moveDirection * (float)moveAcc);
                if (motion.magnitude >= moveMax)
                {
                    motion = motion.normalized * (float)moveMax;
                }
            }

            if (!isMoved && isFrictionEnabled)
            {
                if (motion.magnitude <= friction)
                {
                    motion = Vector2.zero;
                }
                else
                {
                    motion -= motion.normalized * (float)friction;
                }
            }

            if (isGravityEnabled)
            {
                motionV -= gravity * Time.deltaTime;
            }

            if ((thisController.collisionFlags & CollisionFlags.Below) != 0)
            {
                motionV = Mathf.Max(motionV, -0.002f);
                collisionBelow = true;
            }
            else
            {
                collisionBelow = false;
            }
            Debug.Log(thisController.collisionFlags.ToString());

            if ((thisController.collisionFlags & CollisionFlags.Above) != 0)
            {
                motionV = Mathf.Min(motionV, 0);
            }

            thisController.Move( new Vector3(motion.x, motionV, motion.y) );

            isMoved = false;

        }

        //public void EnableMove(bool isEnabled) { }
    }
    
}