using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightShield : MonoBehaviour
{
    [SerializeField] int hitCounter = 3;
    [SerializeField] float horizontalShieldOffsetFromPlayer = 0.7f;

    public void Hit()
    {
        hitCounter--;
    }

    private void FixedUpdate()
    {
        transform.position = GameManager.instance.rightPlayer.transform.position + new Vector3(-horizontalShieldOffsetFromPlayer, 0, 0);
    }

    private void Update()
    {
        if (hitCounter < 1)
        {
            // Play destruction feedback

            Destroy(gameObject);
        }
    }
}
