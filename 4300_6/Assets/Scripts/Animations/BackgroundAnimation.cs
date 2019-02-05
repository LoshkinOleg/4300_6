using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float layer1_speed = 1;
    [SerializeField] float spriteSize = 1000;

    // References
    [SerializeField] GameObject[] layer1 = new GameObject[2];
    [SerializeField] GameObject[] layer2 = new GameObject[2];
    [SerializeField] GameObject[] layer3 = new GameObject[2];

    // Private variables
    float layer2_speed;
    float layer3_speed;
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Start()
    {
        layer2_speed = layer1_speed / 2;
        layer3_speed = layer2_speed / 2;
    }

    private void FixedUpdate()
    {
        // Follow players
        transform.position = GameManager.instance.averagePlayerPosition;

        // Parallax
        for (int i = 0; i < 2; i++)
        {
            layer1[i].transform.localPosition += new Vector3(0,layer1_speed*Time.fixedDeltaTime,0);
            layer2[i].transform.localPosition += new Vector3(0, layer2_speed * Time.fixedDeltaTime, 0);
            layer3[i].transform.localPosition += new Vector3(0, layer3_speed * Time.fixedDeltaTime, 0);
            if (layer1[i].transform.localPosition.y > spriteSize / 100)
            {
                layer1[i].transform.localPosition = new Vector3(0, -spriteSize/100, 0);
            }
            if (layer2[i].transform.localPosition.y > spriteSize / 100)
            {
                layer2[i].transform.localPosition = new Vector3(0, -spriteSize / 100, 0);
            }
            if (layer3[i].transform.localPosition.y > spriteSize / 100)
            {
                layer3[i].transform.localPosition = new Vector3(0, -spriteSize / 100, 0);
            }
        }
    }
    #endregion
}
