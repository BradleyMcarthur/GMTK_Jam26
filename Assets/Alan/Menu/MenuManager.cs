using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject SettingsGameCanvas;

    public void OpenSettingsCanvas()
    {
        SettingsGameCanvas.SetActive(true);
    }
    
    public void CloseSettingsCanvas()
    {
        SettingsGameCanvas.SetActive(false);
    }
}
