using Helpers;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnityRemoteHelper : MonoBehaviour
{
    private bool isRemoteConnected = false;

    private void Start()
    {
        if (!Application.isEditor || !Defines.IsMobile())
            Destroy(gameObject);
    }

    void Update()
    {
#if UNITY_EDITOR
        bool connected = UnityEditor.EditorApplication.isRemoteConnected;
        
        if (!connected && isRemoteConnected)
        {
            Debug.Log("Unity Remote disconnected, enable mouse input");
            isRemoteConnected = false;
            InputSystem.EnableDevice(Mouse.current);
        }
        else if (connected && !isRemoteConnected)
        {
            Debug.Log("Unity Remote connected, disable mouse input");
            isRemoteConnected = true;
            InputSystem.DisableDevice(Mouse.current);
        }
#endif
    }
}
