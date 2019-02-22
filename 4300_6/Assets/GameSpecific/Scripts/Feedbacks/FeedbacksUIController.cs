using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FeedbacksUIController : MonoBehaviour
{
    [SerializeField] GameObject textPrefab = null;
    [SerializeField] float horizontalOffset = 0.5f;
    [SerializeField] float verticalOffset = 0.5f;

    public void InstantiateText(GameObject caller , string ammoLeft, Color color)
    {
        GameObject newText = Instantiate(textPrefab);
        newText.transform.SetParent(gameObject.transform, false);
        newText.transform.position = caller.transform.position + new Vector3(horizontalOffset,verticalOffset,0);
        TMP_Text newText_TMP = newText.GetComponent<TMP_Text>();
        newText_TMP.text = ammoLeft;
        newText_TMP.color = color;
        if (ammoLeft == "*Clack!*")
        {
            newText_TMP.fontSize = 2;
        }
        else
        {
            newText_TMP.fontSize = 4;
        }
    }
}
