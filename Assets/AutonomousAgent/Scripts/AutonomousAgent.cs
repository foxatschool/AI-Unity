using System.Drawing;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AutonomousAgent : AIAgent
{
    [SerializeField] Movement movement;
    [SerializeField] Perception fleePerception;
    [SerializeField] Perception seekPerception;
    [SerializeField] Perception flockPerception;

    [Header("Wander")]
    [SerializeField] float wanderRadius = 1;
    [SerializeField] float wanderDistance = 1;
    [SerializeField] float wanderDisplacement = 1;

    float wanderAngle = 0.0f;

    [Header("Flocking")]
    [SerializeField] float separationWeight = 1.5f;


    void Start()
    {
        wanderAngle = Random.Range(0.0f, 360.0f);
    }

    void Update()
    {
        bool hasTarget = false;

        if (seekPerception != null)
        {
            var gameObjects = seekPerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                Debug.Log("Seeking " + gameObjects[0].name);
                Vector3 force = Seek(gameObjects[0]);
                movement.ApplyForce(force);
                foreach (var go in gameObjects)
                {
                    Debug.DrawLine(transform.position, go.transform.position, UnityEngine.Color.purple);
                }
                hasTarget = true;
            }
        }

        if (fleePerception != null)
        {
            var gameObjects = fleePerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                Vector3 force = Flee(gameObjects[0]);
                movement.ApplyForce(force);
                //foreach (var go in gameObjects)
                //{
                //    Debug.DrawLine(transform.position, go.transform.position, UnityEngine.Color.orange);
                //}
                hasTarget = true;
            }

        }

        //foreach (var go in gameObjects)
        //{
        //    Debug.DrawLine(transform.position, go.transform.position, Color.white);
        //}
        //if (movement.Velocity.sqrMagnitude > 0)
        //{
        //    movement.ApplyForce(Vector3.forward);
        //}

        transform.position = Utilities.Wrap(transform.position, new Vector3(-15, 0, -15), new Vector3(15, 0, 15));
        if (!hasTarget)
        {
            movement.ApplyForce(Wander());
        }
    }

    Vector3 Flee(GameObject go)
    {
        Vector3 direction = transform.position - go.transform.position;
        Vector3 force = GetSteeringForce(direction);
        
        return force;
    }

    Vector3 Seek(GameObject go)
    {
        Vector3 direction = go.transform.position - transform.position;
        Vector3 force = GetSteeringForce(direction);

        return force;
    }

    Vector3 GetSteeringForce(Vector3 direction)
    {
        Vector3 desired = direction.normalized * movement.maxSpeed;
        Vector3 steer = desired - movement.Velocity;
        Vector3 force = Vector3.ClampMagnitude(steer, movement.maxForce);

        return force;
    }

    private Vector3 Wander()
    {
        // randomly adjust the wander angle within (+/-) displacement range 
        wanderAngle += Random.Range(-wanderDisplacement, wanderDisplacement);

        // calculate a point on the wander circle using the wander angle 
        Quaternion rotation = Quaternion.AngleAxis(wanderAngle,transform.up);
        Vector3 pointOnCircle = rotation * (transform.forward * wanderRadius);

        // project the wander circle in front of the agent 
        Vector3 circleCenter = transform.forward * wanderDistance;

        // steer toward the target point (circle center + point on circle) 
        Vector3 force = GetSteeringForce(circleCenter + pointOnCircle);

        Debug.DrawLine(transform.position, transform.position + circleCenter, UnityEngine.Color.blue);
        Debug.DrawLine(transform.position, transform.position + circleCenter + pointOnCircle, UnityEngine.Color.red);

        return force;

    }

    private Vector3 Cohesion(GameObject[] neighbors)
    {
        Vector3 positions = Vector3.zero;
        // accumulate the position vectors of the neighbors
        foreach (GameObject neigbor in neighbors)
	    {
        // add neighbor position to positions
        positions += neigbor.transform.position;

        }

        // average the positions to get the center of the neighbors
        Vector3 center = positions / neighbors.Length;
        // create direction vector to point towards the center of the neighbors from agent position
        Vector3 direction = center - transform.position;

        // steer towards the center point
        Vector3 force = direction;


    return force;
    }

  //  private Vector3 Separation(GameObject[] neighbors, float radius)
  //  {
  //      Vector3 separation = Vector3.zero;
  //      // accumulate the separation vectors of the neighbors
  //      foreach (GameObject neigbor in neighbors)
  //      {
  //          // get direction vector away from neighbor
  //          Vector3 direction = ;
  //          float distance = < direction length >
  //      // check if within separation radius
  //      if (< distance greater than 0 and less than radius>)
		//{
  //              // scale separation vector inversely proportional to the direction distance
  //              // closer the distance the stronger the separation
  //              separation += direction * (1 / distance);
  //          }
  //      }

  //      // steer towards the separation point
  //      Vector3 force = (< separation length is greater than 0 >) ? GetSteeringForce(separation) : Vector3.zero;

  //      return force;
  //  }
}


