using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    // Setup variables
    [HideInInspector] public GameObject target
    {
        private get
        {
            return _target;
        }
        set
        {
            if (value == GameManager.Instance.Players[0].gameObject)
            {
                leftPlayerIsTarget = true;
                gameObject.tag = "Player1Shield";
            }
            else
            {
                gameObject.tag = "Player2Shield";
            }
            _target = value;
        }
    }

    // Inspector variables
    [SerializeField] int hitCounter = 7;

    // Private variables
    GameObject _target = null;
    bool leftPlayerIsTarget;

    // Public methods
    public void Hit(int damage)
    {
        hitCounter -= damage;
    }

    // Inherited methods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (leftPlayerIsTarget)
        {
            if (collision.gameObject.tag == "Player2Projectile")
            {
                hitCounter--;
            }
        }
        else
        {
            if (collision.gameObject.tag == "Player1Projectile")
            {
                hitCounter--;
            }
        }
    }
    private void FixedUpdate()
    {
        // Follow player.
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
