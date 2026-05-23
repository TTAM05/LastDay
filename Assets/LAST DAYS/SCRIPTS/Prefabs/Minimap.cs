using UnityEngine;

public class MiniMap : MonoBehaviour
{
    private bool oldFog;

    void OnPreRender()
    {
        oldFog = RenderSettings.fog;
        RenderSettings.fog = false;
    }

    void OnPostRender()
    {
        RenderSettings.fog = oldFog;
    }
}