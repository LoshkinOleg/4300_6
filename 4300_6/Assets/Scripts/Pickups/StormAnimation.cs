using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormAnimation : MonoBehaviour
{
    // Inspector variables
    [SerializeField] Vector2[] positions = new Vector2[2];
    [SerializeField] float speed = 5f;

    // Private variables
    float startTime;
    float journeyLength;
    bool playAnimation = false;
    bool goingFromAtoB = false;

    public void PlayAnimation()
    {
        goingFromAtoB = !goingFromAtoB;
        startTime = Time.time;
        playAnimation = true;
    }

    void MoveFromAtoB()
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float journeyFraction = distanceCovered / journeyLength;
        transform.position = Vector3.Lerp(positions[0], positions[1], journeyFraction);

        if (Vector3.Distance(transform.position, positions[1]) < 0.05f)
        {
            playAnimation = false;
        }
    }
    void MoveFromBtoA()
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float journeyFraction = distanceCovered / journeyLength;
        transform.position = Vector3.Lerp(positions[1], positions[0], journeyFraction);

        if (Vector3.Distance(transform.position, positions[0]) < 0.05f)
        {
            playAnimation = false;
        }
    }

    private void Start()
    {
        journeyLength = Vector3.Distance(positions[0], positions[1]);
    }

    private void Update()
    {
        if (playAnimation)
        {
            if (goingFromAtoB)
            {
                MoveFromAtoB();
            }
            else
            {
                MoveFromBtoA();
            }
        }
    }
}
