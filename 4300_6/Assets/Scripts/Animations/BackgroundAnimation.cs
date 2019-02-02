using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float layer1_speed = 1;
    [SerializeField] float spriteSize = 1000;

    // References
    [SerializeField] GameObject[] layer1 = new GameObject[2];
    [SerializeField] GameObject[] layer2 = new GameObject[2];
    [SerializeField] GameObject[] layer3 = new GameObject[2];

    // Private variables
    Vector3 targetPosition;
    // Layer1
    Vector3[] layer1_startingPosition = new Vector3[2];
    float[] layer1_lerpStartTime = new float[2];
    bool layer1_firstLoop = true;
    // Layer2
    float layer2_speed;
    Vector3[] layer2_startingPosition = new Vector3[2];
    float[] layer2_lerpStartTime = new float[2];
    bool layer2_firstLoop = true;
    // Layer3
    float layer3_speed;
    Vector3[] layer3_startingPosition = new Vector3[2];
    float[] layer3_lerpStartTime = new float[2];
    bool layer3_firstLoop = true;
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Start()
    {
        targetPosition = new Vector3(0, spriteSize / 100, 0);
        // Layer1
        layer1_startingPosition[0] = new Vector3(0,-spriteSize/100,0);
        layer1_startingPosition[1] = new Vector3(0, 0, 0);
        // Layer2
        layer2_speed = layer1_speed / 2;
        layer2_startingPosition[0] = new Vector3(0, -spriteSize / 100, 0);
        layer2_startingPosition[1] = new Vector3(0, 0, 0);
        // Layer3
        layer3_speed = layer2_speed / 2;
        layer3_startingPosition[0] = new Vector3(0, -spriteSize / 100, 0);
        layer3_startingPosition[1] = new Vector3(0, 0, 0);
    }

    private void FixedUpdate()
    {
        // Layer1
        if (layer1_firstLoop)
        {
            for (int i = 0; i < 2; i++)
            {
                float distanceTraveled = (Time.time - layer1_lerpStartTime[i]) * layer1_speed;
                float fraction = distanceTraveled / Vector3.Distance(layer1_startingPosition[i], targetPosition);
                layer1[i].transform.position = Vector3.Lerp(layer1_startingPosition[i], targetPosition, fraction);

                if (layer1[i].transform.position.y > targetPosition.y - 0.05f)
                {
                    if (i == 1)
                    {
                        layer1_startingPosition[i] = new Vector3(0, -spriteSize / 100, 0);
                        layer1_lerpStartTime[i] = Time.time;
                        layer1_firstLoop = false;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                float distanceTraveled = (Time.time - layer1_lerpStartTime[i]) * layer1_speed;
                float fraction = distanceTraveled / Vector3.Distance(layer1_startingPosition[i], targetPosition);
                layer1[i].transform.position = Vector3.Lerp(layer1_startingPosition[i], targetPosition, fraction);

                if (layer1[i].transform.position.y > targetPosition.y - 0.05f)
                {
                    layer1[i].transform.position = layer1_startingPosition[i];
                    layer1_lerpStartTime[i] = Time.time;
                }
            }
        }

        // Layer2
        if (layer2_firstLoop)
        {
            for (int i = 0; i < 2; i++)
            {
                float distanceTraveled = (Time.time - layer2_lerpStartTime[i]) * layer2_speed;
                float fraction = distanceTraveled / Vector3.Distance(layer2_startingPosition[i], targetPosition);
                layer2[i].transform.position = Vector3.Lerp(layer2_startingPosition[i], targetPosition, fraction);

                if (layer2[i].transform.position.y > targetPosition.y - 0.05f)
                {
                    if (i == 1)
                    {
                        layer2_startingPosition[i] = new Vector3(0, -spriteSize / 100, 0);
                        layer2_lerpStartTime[i] = Time.time;
                        layer2_firstLoop = false;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                float distanceTraveled = (Time.time - layer2_lerpStartTime[i]) * layer2_speed;
                float fraction = distanceTraveled / Vector3.Distance(layer2_startingPosition[i], targetPosition);
                layer2[i].transform.position = Vector3.Lerp(layer2_startingPosition[i], targetPosition, fraction);

                if (layer2[i].transform.position.y > targetPosition.y - 0.05f)
                {
                    layer2[i].transform.position = layer2_startingPosition[i];
                    layer2_lerpStartTime[i] = Time.time;
                }
            }
        }

        // Layer3
        if (layer3_firstLoop)
        {
            for (int i = 0; i < 2; i++)
            {
                float distanceTraveled = (Time.time - layer3_lerpStartTime[i]) * layer3_speed;
                float fraction = distanceTraveled / Vector3.Distance(layer3_startingPosition[i], targetPosition);
                layer3[i].transform.position = Vector3.Lerp(layer3_startingPosition[i], targetPosition, fraction);

                if (layer3[i].transform.position.y > targetPosition.y - 0.05f)
                {
                    if (i == 1)
                    {
                        layer3_startingPosition[i] = new Vector3(0, -spriteSize / 100, 0);
                        layer3_lerpStartTime[i] = Time.time;
                        layer3_firstLoop = false;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                float distanceTraveled = (Time.time - layer3_lerpStartTime[i]) * layer3_speed;
                float fraction = distanceTraveled / Vector3.Distance(layer3_startingPosition[i], targetPosition);
                layer3[i].transform.position = Vector3.Lerp(layer3_startingPosition[i], targetPosition, fraction);

                if (layer3[i].transform.position.y > targetPosition.y - 0.05f)
                {
                    layer3[i].transform.position = layer3_startingPosition[i];
                    layer3_lerpStartTime[i] = Time.time;
                }
            }
        }
    }
    #endregion
}
