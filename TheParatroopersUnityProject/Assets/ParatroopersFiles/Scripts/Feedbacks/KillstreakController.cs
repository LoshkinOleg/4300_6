using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillstreakController : MonoBehaviour
{
    // Inspector variables
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Sprite[] killstreak_Sprites = new Sprite[5]; // 0: blank sprite, 1: private, 2: corp, 3: sergent, 4: commander
    [SerializeField] Vector3 whenInCenterScale = new Vector3(3,3,0);

    // Private variables
    Vector2 defaultPosition;
    Vector2 screenCenter = new Vector2();
    float lerp_startTime;
    float lerp_journeyLength;
    float lerp_speed;
    Vector3 lerp_initalScale;
    int currentSpriteIndex;
    int move; // -1: move to corner, 0: don't move, 1: move to center.

    // Public methods
    public void DisplayKillstreak(int spriteIndex)
    {
        if (spriteIndex != currentSpriteIndex) // Prevents last rank from popping up every time you get a kill past 4+ successive kills.
        {
            if (currentSpriteIndex != 0 && spriteIndex == 0) // Change killstreak sprite to empty right away.
            {
                spriteRenderer.sprite = killstreak_Sprites[0];
            }
            lerp_startTime = Time.time;
            lerp_journeyLength = Vector3.Distance(defaultPosition, screenCenter);
            lerp_speed = lerp_journeyLength / (1f / 3f); // v = d/t, here t = 1/3 of a second.
            lerp_initalScale = transform.localScale;
            move = 1;
            currentSpriteIndex = spriteIndex;

        }
    }

    // Private methods
    IEnumerator ReturnToCorner()
    {
        yield return new WaitForSeconds(1f/3f);

        lerp_startTime = Time.time;
        lerp_journeyLength = Vector3.Distance(defaultPosition, screenCenter);
        lerp_speed = lerp_journeyLength / (1f / 3f);
        move = -1;
    }

    // Inherited methods
    private void Start()
    {
        defaultPosition = transform.localPosition;
    }
    private void FixedUpdate()
    {
        if (move == 1)
        {
            float distanceCovered = (Time.time - lerp_startTime) * lerp_speed;
            float journeyCoveredFraction = distanceCovered / lerp_journeyLength;
            transform.localPosition = Vector3.Lerp(defaultPosition, screenCenter, journeyCoveredFraction);
            transform.localScale = Vector3.Lerp(lerp_initalScale, whenInCenterScale, journeyCoveredFraction);

            if (Vector3.Distance(transform.localPosition, screenCenter) <= 0.05f)
            {
                move = 0;
                spriteRenderer.sprite = killstreak_Sprites[currentSpriteIndex];
                StartCoroutine(ReturnToCorner());
            }
        }
        else if(move == -1)
        {
            float distanceCovered = (Time.time - lerp_startTime) * lerp_speed;
            float journeyCoveredFraction = distanceCovered / lerp_journeyLength;
            transform.localPosition = Vector3.Lerp(screenCenter, defaultPosition, journeyCoveredFraction);
            transform.localScale = Vector3.Lerp(whenInCenterScale, lerp_initalScale, journeyCoveredFraction);

            if (Vector3.Distance(transform.localPosition, defaultPosition) <= 0.05f)
            {
                move = 0;
            }
        }
    }
}
