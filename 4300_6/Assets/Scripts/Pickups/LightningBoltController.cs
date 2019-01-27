using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltController : MonoBehaviour
{
    // Inspector variables
    [SerializeField] float lightningHitArea = 0.6f;
    [SerializeField] float lightningDamage = 0.3f;

    // References
    [SerializeField] SpriteRenderer warningSprite = null;
    [SerializeField] SpriteRenderer lightningBoltSprite = null;

    // Private variables
    float blinkDurationInSeconds = 0.5f;
    float blinkAccelerator = 0.2f;
    bool playingLightningBoltAnimation = false;

    // Private methods
    IEnumerator PlayLightningAnimations()
    {
        // Toggle warning sprite
        warningSprite.enabled = !warningSprite.enabled;

        yield return new WaitForSeconds(blinkDurationInSeconds);

        // Reduce blink time for the next blink.
        blinkDurationInSeconds = blinkDurationInSeconds - (blinkDurationInSeconds * blinkAccelerator);

        if (blinkDurationInSeconds > 0.07f) // 0.07f is an arbitrary threshold.
        {
            StartCoroutine(PlayLightningAnimations());
        }
        else
        {
            // Play lightning bolt animation
            warningSprite.enabled = false;
            lightningBoltSprite.enabled = true;
            playingLightningBoltAnimation = true;

            // Damage players if applicable
            if (Mathf.Abs(transform.position.x - GameManager.instance.leftPlayer.transform.position.x) < lightningHitArea)
            {
                GameManager.instance.leftPlayer.GetComponent<LeftPlayer>().DamageOnce(lightningDamage);
            }
            else if (Mathf.Abs(transform.position.x - GameManager.instance.rightPlayer.transform.position.x) < lightningHitArea)
            {
                GameManager.instance.rightPlayer.GetComponent<RightPlayer>().DamageOnce(lightningDamage);
            }
        }
    }
    IEnumerator DestroySelfAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    private void Start()
    {
        warningSprite.enabled = false;
        lightningBoltSprite.enabled = false;
        StartCoroutine(PlayLightningAnimations());
    }

    private void Update()
    {
        if (playingLightningBoltAnimation)
        {
            StartCoroutine(DestroySelfAfterSeconds(1));
        }
    }
}
