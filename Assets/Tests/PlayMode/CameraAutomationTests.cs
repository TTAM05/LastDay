using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class RealCameraAutomationTests
{
    [UnityTest]
    public IEnumerator AutomatedTest_Camera_TiltZRotation_OnPlayerDeath()
    {
        SceneManager.LoadScene("Map1");
        yield return null;
        yield return new WaitForSeconds(0.5f);

        FPSController fps = Object.FindFirstObjectByType<FPSController>();
        Assert.IsNotNull(fps, "LỖI AUTOMATION: Không tìm thấy FPSController!");

        fps.Die();
        yield return new WaitForSeconds(0.5f);

        float currentZAngle = fps.playerCamera.transform.localRotation.eulerAngles.z;

        Assert.IsTrue(currentZAngle > 45f && currentZAngle <= 90f,
            $"FAILED: Camera không nghiêng góc Z khi Player chết! Góc Z thực tế = {currentZAngle}");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_Camera_RecoilChangesWantedRotation()
    {
        SceneManager.LoadScene("Map1");
        yield return null;
        yield return new WaitForSeconds(0.5f);

        GunRecoil recoil = Object.FindFirstObjectByType<GunRecoil>();
        GunSystem gun = Object.FindFirstObjectByType<GunSystem>();
        FPSController fps = Object.FindFirstObjectByType<FPSController>();

        Assert.IsNotNull(recoil, "LỖI AUTOMATION: Không tìm thấy GunRecoil!");
        Assert.IsNotNull(fps, "LỖI AUTOMATION: Không tìm thấy FPSController!");
        Assert.IsNotNull(gun, "LỖI AUTOMATION: Không tìm thấy GunSystem!");

        float initialWantedX = fps.wantedCameraXRotation;

        recoil.Fire(gun.gunData);

        Assert.AreNotEqual(initialWantedX, fps.wantedCameraXRotation,
            "FAILED: Hàm GunRecoil.Fire() không làm thay đổi góc xoay wantedCameraXRotation của Camera!");
    }

    [UnityTest]
    public IEnumerator AutomatedTest_CameraShake_DisplacesPositionAndResetsOnFinish()
    {
        GameObject camObj = new GameObject("TestCamera");
        camObj.AddComponent<Camera>();

        Vector3 startPos = new Vector3(0, 1.5f, -10f);
        camObj.transform.localPosition = startPos;

        CameraShake shake = camObj.AddComponent<CameraShake>();

        shake.Shake(0.3f, 1.0f);
        yield return new WaitForSeconds(0.1f);

        Assert.AreNotEqual(startPos, camObj.transform.localPosition,
            "FAILED: CameraShake không làm thay đổi tọa độ Camera khi đang Shake!");

        yield return new WaitForSeconds(0.3f);

        Assert.AreEqual(startPos, camObj.transform.localPosition,
            "FAILED: CameraShake không trả Camera về đúng vị trí ban đầu sau khi kết thúc Shake!");

        Object.Destroy(camObj);
    }

    [UnityTest]
    public IEnumerator AutomatedTest_CameraShake_StopShake_InstantlyRestoresPosition()
    {
        GameObject camObj = new GameObject("TestCamera");
        camObj.AddComponent<Camera>();
        CameraShake shake = camObj.AddComponent<CameraShake>();

        Vector3 startPos = Vector3.zero;
        camObj.transform.localPosition = startPos;

        shake.Shake(10f, 2.0f);
        yield return null;

        shake.StopShake();

        Assert.AreEqual(startPos, camObj.transform.localPosition,
            "FAILED: Hàm StopShake() không khôi phục vị trí ban đầu ngay lập tức!");

        Object.Destroy(camObj);
    }

    [UnityTest]
    public IEnumerator AutomatedTest_MutantIntro_SwitchesAndRestoresMainCamera()
    {
        GameObject introObj = new GameObject("MutantIntroObj");
        MutantIntro mutantIntro = introObj.AddComponent<MutantIntro>();

        GameObject mainCam = new GameObject("MainCam");
        GameObject introCam = new GameObject("IntroCam");
        GameObject mutant = new GameObject("MutantBoss");

        mainCam.SetActive(true);
        introCam.SetActive(false);

        mutantIntro.mainCam = mainCam;
        mutantIntro.introCam = introCam;

        mutantIntro.StartCoroutine(mutantIntro.ShowMutant(mutant.transform, 0.4f));
        yield return new WaitForSeconds(0.1f);

        Assert.IsFalse(mainCam.activeSelf, "FAILED: MainCam chưa bị tắt khi Cutscene bắt đầu!");
        Assert.IsTrue(introCam.activeSelf, "FAILED: IntroCam chưa được bật khi Cutscene bắt đầu!");

        yield return new WaitForSeconds(0.4f);

        Assert.IsTrue(mainCam.activeSelf, "FAILED: MainCam không được bật lại sau khi hết Cutscene!");
        Assert.IsFalse(introCam.activeSelf, "FAILED: IntroCam không bị tắt sau khi hết Cutscene!");

        Object.Destroy(introObj);
        Object.Destroy(mainCam);
        Object.Destroy(introCam);
        Object.Destroy(mutant);
    }

    [UnityTest]
    public IEnumerator AutomatedTest_MutantIntro_DisabledMidCutscene_LeavesMainCamLocked()
    {
        GameObject introObj = new GameObject("MutantIntroObj");
        MutantIntro mutantIntro = introObj.AddComponent<MutantIntro>();

        GameObject mainCam = new GameObject("MainCam");
        GameObject introCam = new GameObject("IntroCam");
        GameObject mutant = new GameObject("MutantBoss");

        mainCam.SetActive(true);
        introCam.SetActive(false);

        mutantIntro.mainCam = mainCam;
        mutantIntro.introCam = introCam;

        mutantIntro.StartCoroutine(mutantIntro.ShowMutant(mutant.transform, 1.0f));
        yield return new WaitForSeconds(0.1f);

        introObj.SetActive(false);
        yield return null;

        Assert.IsFalse(mainCam.activeSelf, "FAILED: mainCam bị khóa (Disable) vĩnh viễn do Cutscene bị ngắt dở dang!");

        Object.Destroy(introObj);
        Object.Destroy(mainCam);
        Object.Destroy(introCam);
        Object.Destroy(mutant);
    }
}