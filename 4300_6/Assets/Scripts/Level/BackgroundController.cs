using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    // Attributes
    #region MyRegion
    // Inspector variables
    [SerializeField] float spriteHeight = 1;
    [SerializeField] float scrollingSpeed = 1;

    // References
    [SerializeField] GameObject[] scrollingSprites = new GameObject[2];

    // Private variables
    Vector3 targetPosition;
    Vector3[] lerpStartPositions = new Vector3[2];
    float[] lerpStartTime = new float[2];
    float[] journeyLengths = new float[2];
    bool firstScrollingLoop = true;
    #endregion

    // Private methods
    #region Private methods
    void SetUpLerpVariables()
    {

        targetPosition = new Vector3(0, spriteHeight / 100, 0);
        lerpStartPositions[0] = new Vector3(0, -spriteHeight / 100, 0);
        lerpStartPositions[1] = new Vector3();
        for (int i = 0; i < 2; i++)
        {
            lerpStartTime[i] = Time.time;
            journeyLengths[i] = Vector3.Distance(targetPosition, scrollingSprites[i].transform.position);
        }
    }
    void SetUpScrollingSprite1ForLooping(int i)
    {
        lerpStartPositions[i] = new Vector3(0, -spriteHeight / 100, 0);
        scrollingSprites[i].transform.position = lerpStartPositions[i];
        lerpStartTime[i] = Time.time;
        journeyLengths[i] = Vector3.Distance(targetPosition, scrollingSprites[i].transform.position);
        firstScrollingLoop = false;
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Start()
    {
        // Set up Scrolling_0_Sprite off the screen at the top.
        scrollingSprites[0].transform.position -= new Vector3(0, spriteHeight / 100, 0); // /100 because 1 unit = 100px

        SetUpLerpVariables();
    }

    private void Update()
    {
        // Lerp the sprites.
        for (int i = 0; i < 2; i++)
        {
            float distanceCovered = (Time.time - lerpStartTime[i]) * scrollingSpeed;
            float journeyFractionCovered = distanceCovered / journeyLengths[i];
            scrollingSprites[i].transform.position = Vector3.Lerp(lerpStartPositions[i], targetPosition, journeyFractionCovered);

            if (scrollingSprites[i].transform.position.y >= (spriteHeight / 100) - 0.05f)
            {
                if (firstScrollingLoop && scrollingSprites[i] == scrollingSprites[1])
                {
                    SetUpScrollingSprite1ForLooping(i);
                }
                else
                {
                    // Reset sprite off the screen at the top and reset timer.
                    scrollingSprites[i].transform.position = lerpStartPositions[i];
                    lerpStartTime[i] = Time.time;
                }
            }
        }
    }
    #endregion
}
