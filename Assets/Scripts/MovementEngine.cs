using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEngine : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    class Movement
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
        double posX, posY;
        Vector2 motion;

        GameObject thisObj;

        double moveAcc, moveMax;
        double friction;

        double collisionStep;

        //Private
        bool isMoved;
        bool isMoveEnabled;
        bool isFrictionEnabled;
        

        public Movement(double x, double y, GameObject obj, double _moveAcc = 0.1
            , double _moveMax = 0.5, double _friction = 0.05, double _collisionStep = 1.0)
        {
            posX = x; posY = y;
            thisObj = obj;

            moveAcc = _moveAcc; moveMax = _moveMax; friction = _friction;
            collisionStep = _collisionStep;

            motion = new Vector2(0, 0);
            isMoved = false;
            isMoveEnabled = true;
            isFrictionEnabled = true;
        }

        public double GetPosX() { return posX; }

        public double GetPosY() { return posY; }

        public Vector2 GetMotion() { return motion; }

        public void SetPosX(double x) { posX = x; }

        public void SetPosY(double y) { posY = y; }

        public void SetMotion(Vector2 mo) { motion = mo; }

        public void AddMotion(Vector2 mo) { motion += mo; }

        public void Move() { }

        public void Update()
        {
            //1. apply friction
            //2. calculate collision

            
        }

        public void EnableMove(bool isEnabled) { }
    }
    
}