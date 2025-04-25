using Unity.Netcode;
using UnityEngine;

public enum TimeMode{
    X1,
    X2,
    X3,
    X4,
    FREEZE,
}

public class TimeManager : NetworkBehaviour
{
    public static TimeManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private TimeMode currentTimeNode = TimeMode.X1;


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            SetTimeMode(++currentTimeNode);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SetTimeMode(--currentTimeNode);
    }
#endif


    public void SetTimeMode(TimeMode mode)
    {
        if (!NetUtils.IsHost()) return;

        switch (mode)
        {
            case TimeMode.FREEZE:
                Time.timeScale = 0f;
                break;
            case TimeMode.X1:
                Time.timeScale = 1f;
                break;
            case TimeMode.X2:
                Time.timeScale = 2f;
                break;
            case TimeMode.X3:
                Time.timeScale = 3f;
                break;
            case TimeMode.X4:
                Time.timeScale = 4f;
                break;
            default:

                break;
        }

        SetTimeModeClientRpc((int)mode);
    }

    [ClientRpc]
    private void SetTimeModeClientRpc(int mode)
    {
        TimeMode timeMode = (TimeMode)mode;

        switch (timeMode)
        {
            case TimeMode.X1:
                Time.timeScale = 1f;
                break;
            case TimeMode.X2:
                Time.timeScale = 2f;
                break;
            case TimeMode.FREEZE:
                Time.timeScale = 0f;
                break;
        }

    }

}
