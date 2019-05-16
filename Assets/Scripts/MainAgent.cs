// Assignment project of Intelligent Computational Media 
// 16 May , John Cheongun Lee
// 
// this is a mainAgent class
//


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class MainAgent : Agent
{
    private Rigidbody mainRigidBody;
    private bool targetEaten;
    private bool isDead;
    private RayPerception rayObjDetector;

    public Transform pivotTransform;
    public Transform targetTransform;
    public Transform obstacleTransform;
    public float moveForce;

    public override void InitializeAgent()
    {
        targetEaten = false;
        isDead = false;

        agentParameters.maxStep = 60000;

        mainRigidBody = GetComponent<Rigidbody>();
        rayObjDetector = GetComponent<RayPerception>();
    }

    void ResetTarget()
    {
        Vector3 pos;

        targetEaten = false;

        pos = new Vector3(Random.Range(-7f, 7f), 0.4f, Random.Range(4f, 7f));

        targetTransform.position = (pos + pivotTransform.position);

        pos = new Vector3(0f, 0.5f, Random.Range(-3f, 3f));

        obstacleTransform.position = (pos + pivotTransform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead") == true || other.CompareTag("obstacle") == true || other.CompareTag("wall") == true)
        {
            isDead = true;
        }
        else if (other.CompareTag("target") == true)
        {
            targetEaten = true;
        }
    }

    public override void AgentReset()
    {
        Vector3 pos;

        pos = new Vector3(Random.Range(-7f, 7f), 0.5f, Random.Range(-10f, -7f));

        transform.position = (pos + pivotTransform.position);

        isDead = false;

        mainRigidBody.velocity = Vector3.zero;

        ResetTarget();
    }

    public override void CollectObservations()
    {
        float rayDistance = 15f;
        float[] rayAngles = { 20f, 60f, 90f, 120f, 160f };
        //string[] detectableObjects = { "target","wall"};
        //string[] detectableObjects = { "target", "obstacle" };
        string[] detectableObjects = { "target", "obstacle","wall" };

        AddVectorObs(GetStepCount() / (float)agentParameters.maxStep);
        AddVectorObs(rayObjDetector.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
    }

    public override void AgentAction(float[] VectorAction, string TextAction)
    {
        int action;

        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        AddReward(-1f / agentParameters.maxStep);

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {
            dirToGo = transform.forward * Mathf.Clamp(VectorAction[0], -1f, 1f);
            rotateDir = transform.up * Mathf.Clamp(VectorAction[1], -1f, 1f);
        }
        else
        {
            action = Mathf.FloorToInt(VectorAction[0]);

            switch (action)
            {
                case 1:
                    dirToGo = transform.forward * 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 3:
                    rotateDir = transform.up * 1f;
                    break;
                case 4:
                    rotateDir = transform.up * -1f;
                    break;
            }
        }

        transform.Rotate(rotateDir, Time.deltaTime * 150f);

        mainRigidBody.AddForce(dirToGo * moveForce, ForceMode.VelocityChange);

        if (targetEaten == true)
        {
            AddReward(1.0f);
            Done();
        }
        else if (isDead == true)
        {
            AddReward(-1.0f);
            Done();
        }

        Monitor.Log(name, GetCumulativeReward(), transform);
    }


}