using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementHelper
{
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
        public bool isRetainMotionEnabled;

        public bool collisionBelow;

        public MovementProfile profile;

        //Private
        bool isMoved;
        float prevMotionMagnitude;
        float prevMotionV;
        Vector3 prevPosition;


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
            isRetainMotionEnabled = false;

            prevMotionMagnitude = 0;
            prevMotionV = 0;
            prevPosition = new Vector3();

            profile = SaveProfile();
        }

        public Vector2 GetMotion() { return motion; }

        public void SetMotion(Vector3 direction)
        {
            motion = new Vector2(direction.x, direction.z);
            motionV = direction.y;

            prevMotionMagnitude = motion.magnitude;
        }

        public void SetMotion2d(Vector3 direction)
        {
            motion = new Vector2(direction.x, direction.z);

            prevMotionMagnitude = motion.magnitude;
        }

        public void AddMotion(Vector3 direction)
        {
            motion += new Vector2(direction.x, direction.z);
            motionV += direction.y;

            prevMotionMagnitude = motion.magnitude;
        }

        public void AddMotion2d(Vector3 direction)
        {
            motion += new Vector2(direction.x, direction.z);

            prevMotionMagnitude = motion.magnitude;
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
                motion += (moveDirection * ((float)moveAcc * Time.deltaTime));
                if (motion.magnitude >= moveMax)
                {
                    if (isRetainMotionEnabled)
                    {
                        motion = motion.normalized * Mathf.Min(motion.magnitude, prevMotionMagnitude);
                        motion = motion.normalized * Mathf.Max(motion.magnitude, (float)moveMax);
                    }
                    else
                    {
                        motion = motion.normalized * (float)moveMax;
                    }
                }
            }

            if (!isMoved && isFrictionEnabled)
            {
                if (motion.magnitude <= (float)friction * Time.deltaTime)
                {
                    motion = Vector2.zero;
                }
                else
                {
                    motion -= motion.normalized * ((float)friction * Time.deltaTime);
                }
            }

            if (isGravityEnabled)
            {
                motionV -= gravity * Time.deltaTime;
            }
            //Debug.Log((thisController.collisionFlags & CollisionFlags.Below).ToString());
            if ((thisController.collisionFlags & CollisionFlags.Below) != 0)
            {
                motionV = Mathf.Max(motionV, -0.002f);
                collisionBelow = true;
            }
            else
            {
                collisionBelow = false;
            }

            
            //Debug.Log(thisController.collisionFlags.ToString());

            if ((thisController.collisionFlags & CollisionFlags.Above) != 0)
            {
                motionV = Mathf.Min(motionV, 0);
            }

            thisController.Move(new Vector3(motion.x, motionV, motion.y) * Time.deltaTime);
            /*
            if (!collisionBelow && motion.y < 0 &&
                thisController.transform.position.y - prevPosition.y < motion.y * Time.deltaTime)
            {
                thisController.transform.position = new Vector3(
                thisController.transform.position.x, prevPosition.y, thisController.transform.position.z);
            }*/

            //Post-move set
            isMoved = false;

            prevMotionMagnitude = motion.magnitude;
            prevMotionV = motionV;
            prevPosition = thisController.transform.position;

            if (true)
            {
                Debug.Log(motion.ToString() + motionV.ToString() + "/ " + thisController.transform.position);
            }
        }

        public void LoadProfile(MovementProfile profile)
        {
            if (profile.moveAcc.HasValue) { moveAcc = profile.moveAcc.Value; }
            if (profile.moveMax.HasValue) { moveMax = profile.moveMax.Value; }
            if (profile.friction.HasValue) { friction = profile.friction.Value; }
            if (profile.gravity.HasValue) { gravity = profile.gravity.Value; }
            if (profile.isMoveEnabled.HasValue) { isMoveEnabled = profile.isMoveEnabled.Value; }
            if (profile.isFrictionEnabled.HasValue) { isFrictionEnabled = profile.isFrictionEnabled.Value; }
            if (profile.isGravityEnabled.HasValue) { isGravityEnabled = profile.isGravityEnabled.Value; }
            if (profile.isRetainMotionEnabled.HasValue) { isRetainMotionEnabled = profile.isRetainMotionEnabled.Value; }

            this.profile = profile;
        }

        public MovementProfile SaveProfile()
        {
            return new MovementProfile(moveAcc, moveMax, friction, gravity, isMoveEnabled
                , isFrictionEnabled, isGravityEnabled, isRetainMotionEnabled);
        }


        //public void EnableMove(bool isEnabled) { }
    }

    public struct MovementProfile
    {
        public double? moveAcc, moveMax;
        public double? friction;
        public float? gravity;

        public bool? isMoveEnabled;
        public bool? isFrictionEnabled;
        public bool? isGravityEnabled;
        public bool? isRetainMotionEnabled;


        public MovementProfile(double? _moveAcc = null, double? _moveMax = null, double? _friction = null
            , float? _gravity = null, bool? _isMoveEnabled = null, bool? _isFrictionEnabled = null
            , bool? _isGravityEnabled = null, bool? _isRetainMotionEnabled = null)
        {

            moveAcc = _moveAcc; moveMax = _moveMax; friction = _friction;
            gravity = _gravity;

            isMoveEnabled = _isMoveEnabled;
            isFrictionEnabled = _isFrictionEnabled;
            isGravityEnabled = _isGravityEnabled;
            isRetainMotionEnabled = _isRetainMotionEnabled;
        }

    }
}