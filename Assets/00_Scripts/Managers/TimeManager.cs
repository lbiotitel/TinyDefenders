using Unity.Netcode;
using UnityEngine;

public enum TimeMode{
    X1,
    X2,
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


    public void SetTimeMode(TimeMode mode)
    {
        if (!NetUtils.IsHost()) return;

        switch (mode)
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
