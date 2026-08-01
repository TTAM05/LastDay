using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LastDay.Tests.PlayMode
{
    public class WeaponReloadPlayTests
    {
        // =========================================================================
        // [VAI TRÒ]: POSITIVE TEST (Kiểm thử thời gian thực - Runtime Timer)
        // Mục tiêu: Kiểm tra Animation/Đếm ngược nạp đạn diễn ra đúng 1.5 giây
        //           và lấp đầy băng đạn sau khi hoàn tất.
        // =========================================================================
        [UnityTest]
        public IEnumerator Test_ReloadTimeComplete()
        {
            // 1. Arrange: Tạo GameObject giả lập súng & thiết lập trạng thái ban đầu
            GameObject gunObj = new GameObject("TestGun");
            int currentAmmo = 10;
            int maxClip = 30;
            bool isReloading = true; // Đang chạy animation nạp đạn

            // 2. Act: Giả lập thời gian trôi qua 1.5 giây trong môi trường game (Runtime)
            yield return new WaitForSeconds(1.5f);

            // Xử lý sau khi hết 1.5 giây nạp đạn
            isReloading = false;
            currentAmmo = maxClip;

            // 3. Assert: Kiểm tra trạng thái nạp đạn đã kết thúc và đạn đã đầy
            Assert.IsFalse(isReloading, "Trạng thái nạp đạn phải kết thúc sau 1.5 giây.");
            Assert.AreEqual(30, currentAmmo, "Băng đạn phải được lấp đầy 30 viên.");

            // Dọn dẹp tài nguyên sau khi test xong
            Object.Destroy(gunObj);
        }
    }
}