using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Inspector variables
    [SerializeField] float screenEdgeBuffer = 1;
    [SerializeField] float minimalZoom = 6.5f;
    [SerializeField] GameObject[] players;

    // References
    Camera mainCamera = null;

    // Private methods
    #region Private methods
    float FindRequiredSize()
    {
        float desiredSize = 0;

        for (int i = 0; i < players.Length; i++)
        {
            Vector2 distanceToPlayerV2 = players[i].transform.position - GameManager.instance.averagePlayerPosition;
            desiredSize = Mathf.Max(Mathf.Abs(distanceToPlayerV2.y), Mathf.Abs(distanceToPlayerV2.x) / mainCamera.aspect); // Pick between vertical and horizontal camera view distances
        }
        desiredSize = Mathf.Max(desiredSize, minimalZoom);
        desiredSize += screenEdgeBuffer;

        return desiredSize;
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    void Start()
    {
        mainCamera = Camera.main;
    }
    void FixedUpdate()
    {
        transform.position = GameManager.instance.averagePlayerPosition + new Vector3(0,0,transform.position.z);
        mainCamera.orthographicSize = FindRequiredSize();
    }
    #endregion
}
