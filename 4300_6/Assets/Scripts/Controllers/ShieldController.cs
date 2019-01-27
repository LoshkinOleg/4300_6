using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [HideInInspector] public bool targetLeftPlayer = true;
    [SerializeField] int hitCounter = 3;

    public void Hit()
    {
        hitCounter--;
    }

    private void FixedUpdate()
    {
        if (targetLeftPlayer)
        {
            transform.position = GameManager.instance.leftPlayer.transform.position + new Vector3(0.5f,0,0);
        }
        else
        {
            transform.position = GameManager.instance.rightPlayer.transform.position + new Vector3(-0.5f, 0, 0);
        }
    }

    private void Update()
    {
        if (hitCounter < 1)
        {
            Destroy(gameObject);
        }
    }
}
