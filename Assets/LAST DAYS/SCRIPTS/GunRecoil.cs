using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [Header("Recoil")]
    public float recoilX = 2f;        // giật lên
    public float recoilY = 1f;        // giật ngang (random)
    public float recoilZ = 0.5f;      // giật xoay

    [Header("Smooth")]
    public float returnSpeed = 10f;   // tốc độ về vị trí cũ
    public float recoilSpeed = 20f;   // tốc độ giật

    // rotation target
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    void Update()
    {
        // giảm dần target về 0
        targetRotation = Vector3.Lerp(
            targetRotation,
            Vector3.zero,
            returnSpeed * Time.deltaTime
        );

        // smooth từ current → target
        currentRotation = Vector3.Slerp(
            currentRotation,
            targetRotation,
            recoilSpeed * Time.deltaTime
        );

        // apply rotation vào camera/gun
        transform.localRotation =
            Quaternion.Euler(currentRotation);
    }

    // =========================================================
    // GỌI HÀM NÀY KHI BẮN
    // =========================================================
    public void Fire()
    {
        // giật lên + random ngang
        targetRotation += new Vector3(
            -recoilX,
            Random.Range(-recoilY, recoilY),
            Random.Range(-recoilZ, recoilZ)
        );
    }
}