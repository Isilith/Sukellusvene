using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineDriver : MonoBehaviour
{

    private float movementSpeed = 0;
    private float angle = 0;
    private float depthAngle;
    private float depth = 1000;
    public float maxDepth = 1100;
    public float minDepth = 900;
    public float depthDamageInterWall = 15f;
    bool depthTimer = false;

    public GameObject depthText;
    
    void Start()
    {
        StartCoroutine(DepthRandomizator());
    }


    public void SetMovementSpeed(float newSpeed)
    {
        movementSpeed = newSpeed;
    }
    
    public void SetAngle(float newAngle)
    {
        angle = newAngle;
    }

    public void SetDepthAngle(float newAngle)
    {
        depthAngle = newAngle;
    }

	void Update ()
    {
        //if (movementSpeed != 0)
        {
            transform.Translate(transform.right * movementSpeed * Time.deltaTime,Space.World);
            if (angle != transform.rotation.eulerAngles.y)
            {
                transform.eulerAngles = new Vector3(0, angle, 0);
            }
        }
        depth += depthAngle* Time.deltaTime;
        Helper.SetButtonText(depthText, depth.ToString("F2"));
        //depthText

        if (depthTimer == false)
        {
            if (depth < minDepth || depth > maxDepth)
            {
                StartCoroutine(DepthDamageTimer());
            }
        }

    }

    IEnumerator DepthRandomizator()
    {
        yield return null;
        
        while (true)
        {
            float endTime = Time.time + Random.Range(5, 10);
            float sinkAngle = Random.Range(-2.0f, 2.0f);
            while (true)
            {
                depth += sinkAngle * Time.deltaTime;
                if (endTime < Time.time)
                {
                    break;
                }
                yield return null;
            }
        }
    }

    IEnumerator DepthDamageTimer()
    {
        float endTime = Time.time + depthDamageInterWall;
        depthTimer = true;
        while (true)
        {
            if (endTime < Time.time)
            {
                if (depth > maxDepth || depth < minDepth)
                {
                    GameController.DealDamage();
                    break;
                }
            }
            else
            {
                if (depth < maxDepth && depth > minDepth)
                {
                    break;
                }
            }
            yield return null;
        }
        depthTimer = false;
    }


}
