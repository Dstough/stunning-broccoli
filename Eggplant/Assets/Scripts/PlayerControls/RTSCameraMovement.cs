using UnityEngine;
using UnityEngine.UI;

public class RTSCameraMovement : MonoBehaviour
{
    public Camera MainCamera;
    public Text DebugLabel;
    public bool ScrollEnabled = true;
    public float ScrollSpeed = 1;
    public int zBound = 0;
    public int xBound = 0;

    private const float SpeedScalar = .1f;
    private const int EdgeDistanceBuffer = 2;

    #region MonoBehaviour

    void Update()
    {
        if (!ScrollEnabled)
            return;

        var newCameraPosition = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z);

        PrintMousePosition();
        MouseScroll(ref newCameraPosition);
        
        //TODO: Account for keyboard input to scroll screen
        //TODO: Check position against world bounds.

        if (
           (newCameraPosition.z <= zBound && newCameraPosition.z >= -zBound) &&
           (newCameraPosition.x <= xBound && newCameraPosition.x >= -xBound) 
        )
            MainCamera.transform.position = newCameraPosition;
    }

    private void OnApplicationFocus(bool focus)
    {
        ScrollEnabled = focus;

        if (focus)
            Cursor.lockState = CursorLockMode.Confined;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    #endregion

    private void PrintMousePosition()
    {
        DebugLabel.text = Input.mousePosition.x + ", " + Input.mousePosition.y;
    }

    private void MouseScroll(ref Vector3 newCameraPosition)
    {
        if (Input.mousePosition.x <= EdgeDistanceBuffer)
            newCameraPosition.x -= ScrollSpeed * SpeedScalar;
        else if (Input.mousePosition.x >= Screen.width - EdgeDistanceBuffer)
            newCameraPosition.x += ScrollSpeed * SpeedScalar;

        if (Input.mousePosition.y <= EdgeDistanceBuffer)
            newCameraPosition.z -= ScrollSpeed * SpeedScalar;
        else if (Input.mousePosition.y >= Screen.height - EdgeDistanceBuffer)
            newCameraPosition.z += ScrollSpeed * SpeedScalar;
        
        //TODO: Account for middle click scrolling.
    }
}