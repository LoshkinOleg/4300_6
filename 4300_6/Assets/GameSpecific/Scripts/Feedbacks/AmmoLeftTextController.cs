using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoLeftTextController : MonoBehaviour
{
    public float lifetime;
    public Transform target;
    public float horizontalOffset;
    public float verticalOffset;

    IEnumerator SelfDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    public void Init(Transform parent , Transform target, float horizontalOffset, float verticalOffset, string ammoLeft, Color color, int wordsSize, int numbersSize)
    {
        transform.SetParent(parent, false);

        TMP_Text text = GetComponent<TMP_Text>();
        text.text = ammoLeft;
        text.color = color;
        if (ammoLeft == "*Clack!*")
        {
            text.fontSize = wordsSize;
        }
        else
        {
            text.fontSize = numbersSize;
        }

        StartCoroutine(SelfDestroy(lifetime));
    }

    private void FixedUpdate()
    {
        transform.position = target.position + new Vector3(horizontalOffset, verticalOffset, 0);
    }

}
