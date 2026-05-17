using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public FPSController fps;

    [Header("Recoil")]

    public float recoilX = 2f;
    public float recoilY = 1f;

    public void Fire()
    {
        float recoilHorizontal =
            Random.Range(-recoilY, recoilY);

        fps.wantedCameraXRotation -= recoilX;

        fps.wantedYRotation += recoilHorizontal;

        Debug.Log("RECOIL");
    }
}