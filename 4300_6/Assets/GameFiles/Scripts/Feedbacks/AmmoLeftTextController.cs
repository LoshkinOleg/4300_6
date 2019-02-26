using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoLeftTextController : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [HideInInspector] public float horizontalOffset;
    [HideInInspector] public float verticalOffset;

    IEnumerator SelfDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    public void Init(Transform parent , Transform target, float horizontalOffset, float verticalOffset, string ammoLeft, Color color, int wordsSize, int numbersSize, float lifetime)
    {
        this.target = target;
        this.horizontalOffset = horizontalOffset;
        this.verticalOffset = verticalOffset;

        transform.SetParent(parent, false);
        transform.position = target.position + new Vector3(horizontalOffset, verticalOffset, 0);

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
}
