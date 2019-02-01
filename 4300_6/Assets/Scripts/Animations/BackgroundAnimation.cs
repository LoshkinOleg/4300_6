using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    // Attributes
    #region MyRegion
    // Inspector variables
    [SerializeField] float spriteHeight = 1;
    [SerializeField] float scrollingSpeed_0 = 1;

    // References
    [SerializeField] GameObject[] scrollingSprites_0 = new GameObject[2];
    [SerializeField] GameObject[] scrollingSprites_1 = new GameObject[2];
    [SerializeField] GameObject[] scrollingSprites_2 = new GameObject[2];

    // Private variables
    float scrollingSpeed_1;
    float scrollingSpeed_2;
    Vector3 targetPosition;
    Vector3[] lerpStartPositions = new Vector3[2];
    float[] lerpStartTime_0 = new float[2];
    float[] lerpStartTime_1 = new float[2];
    float[] lerpStartTime_2 = new float[2];
    float[] journeyLengths = new float[2];
    bool firstScrollingLoop_0 = true;
    bool firstScrollingLoop_1 = true;
    bool firstScrollingLoop_2 = true;
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
            lerpStartTime_0[i] = Time.time;
            lerpStartTime_1[i] = Time.time;
            lerpStartTime_2[i] = Time.time;
            journeyLengths[i] = Vector3.Distance(targetPosition, scrollingSprites_0[i].transform.position);
        }
    }
    void SetUpSpriteForLooping_0(int i)
    {
        lerpStartPositions[i] = new Vector3(0, -spriteHeight / 100, 0);
        scrollingSprites_0[i].transform.position = lerpStartPositions[i];
        lerpStartTime_0[i] = Time.time;
        journeyLengths[i] = Vector3.Distance(targetPosition, scrollingSprites_0[i].transform.position);
        firstScrollingLoop_0 = false;
    }
    void SetUpSpriteForLooping_1(int i)
    {
        lerpStartPositions[i] = new Vector3(0, -spriteHeight / 100, 0);
        scrollingSprites_1[i].transform.position = lerpStartPositions[i];
        lerpStartTime_1[i] = Time.time;
        journeyLengths[i] = Vector3.Distance(targetPosition, scrollingSprites_1[i].transform.position);
        firstScrollingLoop_1 = false;
    }
    void SetUpSpriteForLooping_2(int i)
    {
        lerpStartPositions[i] = new Vector3(0, -spriteHeight / 100, 0);
        scrollingSprites_2[i].transform.position = lerpStartPositions[i];
        lerpStartTime_2[i] = Time.time;
        journeyLengths[i] = Vector3.Distance(targetPosition, scrollingSprites_2[i].transform.position);
        firstScrollingLoop_2 = false;
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Start()
    {
        // Set up Scrolling_0_Sprite off the screen at the top.
        scrollingSprites_0[0].transform.position -= new Vector3(0, spriteHeight / 100, 0); // /100 because 1 unit = 100px
        scrollingSprites_1[0].transform.position -= new Vector3(0, spriteHeight / 100, 0);
        scrollingSprites_2[0].transform.position -= new Vector3(0, spriteHeight / 100, 0);
        SetUpLerpVariables();
        scrollingSpeed_1 = scrollingSpeed_0 / 2;
        scrollingSpeed_2 = scrollingSpeed_1 / 2;
    }

    private void Update()
    {
        // Lerp the sprites.
        for (int i = 0; i < 2; i++)
        {
            float distanceCovered_0 = (Time.time - lerpStartTime_0[i]) * scrollingSpeed_0;
            float journeyFractionCovered_0 = distanceCovered_0 / journeyLengths[i];
            scrollingSprites_0[i].transform.position = Vector3.Lerp(lerpStartPositions[i], targetPosition, journeyFractionCovered_0);
            float distanceCovered_1 = (Time.time - lerpStartTime_1[i]) * scrollingSpeed_1;
            float journeyFractionCovered_1 = distanceCovered_1 / journeyLengths[i];
            scrollingSprites_1[i].transform.position = Vector3.Lerp(lerpStartPositions[i], targetPosition, journeyFractionCovered_1);
            float distanceCovered_2 = (Time.time - lerpStartTime_2[i]) * scrollingSpeed_2;
            float journeyFractionCovered_2 = distanceCovered_2 / journeyLengths[i];
            scrollingSprites_2[i].transform.position = Vector3.Lerp(lerpStartPositions[i], targetPosition, journeyFractionCovered_2);

            if (scrollingSprites_0[i].transform.position.y >= (spriteHeight / 100) - 0.05f)
            {
                if (firstScrollingLoop_0 && scrollingSprites_0[i] == scrollingSprites_0[1])
                {
                    SetUpSpriteForLooping_0(i);
                }
                else
                {
                    // Reset sprite off the screen at the top and reset timer.
                    scrollingSprites_0[i].transform.position = lerpStartPositions[i];
                    lerpStartTime_0[i] = Time.time;
                }
            }
            if (scrollingSprites_1[i].transform.position.y >= (spriteHeight / 100) - 0.05f)
            {
                if (firstScrollingLoop_1 && scrollingSprites_1[i] == scrollingSprites_1[1])
                {
                    SetUpSpriteForLooping_1(i);
                }
                else
                {
                    // Reset sprite off the screen at the top and reset timer.
                    scrollingSprites_1[i].transform.position = lerpStartPositions[i];
                    lerpStartTime_1[i] = Time.time;
                }
            }
            if (scrollingSprites_2[i].transform.position.y >= (spriteHeight / 100) - 0.05f)
            {
                if (firstScrollingLoop_2 && scrollingSprites_2[i] == scrollingSprites_2[1])
                {
                    SetUpSpriteForLooping_2(i);
                }
                else
                {
                    // Reset sprite off the screen at the top and reset timer.
                    scrollingSprites_2[i].transform.position = lerpStartPositions[i];
                    lerpStartTime_2[i] = Time.time;
                }
            }
        }
    }
    #endregion
}
