using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Inspector variables
    [SerializeField] float screenEdgeBuffer = 1;
    [SerializeField] float minimalZoom = 1;
    [SerializeField] float maximalZoom = 5;
    [SerializeField] GameObject[] players = new GameObject[2];

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
        desiredSize += screenEdgeBuffer;

        if (mainCamera.orthographicSize * mainCamera.aspect * 2 > GameManager.instance.gameViewHorizontalDistanceInMeters)
        {
            return desiredSize = maximalZoom;
        }

        desiredSize = Mathf.Max(desiredSize, minimalZoom);
        desiredSize = Mathf.Min(desiredSize, maximalZoom);

        return desiredSize;
    }
    bool CheckIfCameraGoesOutsideBoundsHorizontally()
    {
        if (mainCamera.orthographicSize * mainCamera.aspect + Mathf.Abs(GameManager.instance.averagePlayerPosition.x) >= GameManager.instance.gameViewHorizontalDistanceInMeters / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool CheckIfCameraGoesOutsideBoundsVertically()
    {
        if (mainCamera.orthographicSize + Mathf.Abs(GameManager.instance.averagePlayerPosition.y) >= GameManager.instance.gameViewVerticalDistanceInMeters / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void BringCameraInsideBoundsHorizontally()
    {
        if (GameManager.instance.averagePlayerPosition.x <= 0)
        {
            transform.position += new Vector3((mainCamera.orthographicSize * mainCamera.aspect) - (GameManager.instance.gameViewHorizontalDistanceInMeters / 2 + transform.position.x), 0, 0);
        }
        else
        {
            transform.position -= new Vector3(((mainCamera.orthographicSize * mainCamera.aspect) + transform.position.x) - GameManager.instance.gameViewHorizontalDistanceInMeters / 2, 0, 0);
        }
    }
    void BringCameraInsideBoundsVertically()
    {
        if (GameManager.instance.averagePlayerPosition.y <= 0)
        {
            transform.position += new Vector3(0, (mainCamera.orthographicSize) - (GameManager.instance.gameViewVerticalDistanceInMeters / 2 + transform.position.y), 0);
        }
        else
        {
            transform.position -= new Vector3(0, (mainCamera.orthographicSize + transform.position.y) - (GameManager.instance.gameViewVerticalDistanceInMeters / 2), 0);
        }
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
        // Set position
        transform.position = GameManager.instance.averagePlayerPosition + new Vector3(0, 0, transform.position.z);

        // Manage camera zoom
        mainCamera.orthographicSize = FindRequiredSize();

        // Prevent the camera from going out of bounds.
        if (CheckIfCameraGoesOutsideBoundsHorizontally())
        {
            BringCameraInsideBoundsHorizontally();
        }
        if (CheckIfCameraGoesOutsideBoundsVertically())
        {
            BringCameraInsideBoundsVertically();
        }
    }
    #endregion
}
