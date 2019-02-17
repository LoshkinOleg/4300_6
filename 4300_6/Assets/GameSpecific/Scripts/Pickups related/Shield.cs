using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] int hitCounter = 3;
    [HideInInspector] public GameObject target
    {
        private get
        {
            return _target;
        }
        set
        {
            if (value == GameManager.Instance.Player1.gameObject)
            {
                leftPlayerIsTarget = true;
            }
            _target = value;
        }
    }
    GameObject _target = null;
    bool leftPlayerIsTarget;

    public void Hit()
    {
        hitCounter--;
    }

    // Inherited methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (leftPlayerIsTarget)
        {
            if (collision.gameObject.tag == "Player2Projectile")
            {
                Hit();
            }
        }
        else
        {
            if (collision.gameObject.tag == "Player1Projectile")
            {
                Hit();
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position = target.transform.position;
    }

    private void Update()
    {
        if (hitCounter < 1)
        {
            Destroy(gameObject);
        }
    }
}
