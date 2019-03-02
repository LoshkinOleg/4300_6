using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoLeftTextController : MonoBehaviour
{
    // Inspector variables
    [SerializeField] float upwardsSpeed = 0.5f;
    [SerializeField] float periodInSeconds = 2f;

    // Private variables
    Transform target;
    float horizontalOffset;
    float verticalOffset;
    float x; // Variable used in cos(2*PI*x + PI/2). fixedDeltaTime is added to it every FixedUpdate().

    // Public methods
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

    // Private methods
    IEnumerator SelfDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    // Inherited methods
    private void FixedUpdate()
    {
        x += Time.deltaTime;
        transform.localPosition += new Vector3(Mathf.Cos((2*Mathf.PI*x)/ periodInSeconds + Mathf.PI/2) ,upwardsSpeed,0); // Moves the text upwards and sideways in a sinusoidal manner.
    }
}
