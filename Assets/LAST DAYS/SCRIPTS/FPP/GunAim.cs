using UnityEngine;

public class GunAim : MonoBehaviour
{
    [Header("References")]

    // camera FPS
    public Camera cam;

    // đầu nòng súng
    public Transform muzzle;

    [Header("Settings")]

    // khoảng cách aim
    public float range = 1000f;

    // tốc độ xoay súng
    public float aimSmooth = 20f;

    void Update()
    {
        AimGun();
    }

    void AimGun()
    {
        // ray từ giữa màn hình
        Ray ray = cam.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 targetPoint;

        // nếu ray trúng vật
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            targetPoint = hit.point;
        }
        else
        {
            // nếu không trúng gì
            targetPoint = ray.GetPoint(range);
        }

        // hướng từ nòng súng tới target
        Vector3 dir =
            (targetPoint - muzzle.position).normalized;

        // rotation mục tiêu
        Quaternion targetRot =
            Quaternion.LookRotation(dir);

        // xoay mượt nòng súng
        muzzle.rotation = Quaternion.Lerp(
            muzzle.rotation,
            targetRot,
            aimSmooth * Time.deltaTime
        );

        // debug
        // Debug.DrawLine(
        //     muzzle.position,
        //     targetPoint,
        //     Color.red
        // );
    }
}