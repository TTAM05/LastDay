using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public FPSController fps;

    public void Fire(GunData gunData)
    {
        float recoilHorizontal =
            Random.Range(-gunData.recoilY, gunData.recoilY);

        fps.wantedCameraXRotation -= gunData.recoilX;

        fps.wantedYRotation += recoilHorizontal;

        Debug.Log("RECOIL");
    }
}