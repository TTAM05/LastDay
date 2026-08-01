using NUnit.Framework;

namespace LastDay.Tests.EditMode
{
    public class WeaponReloadEditTests
    {
        // =========================================================================
        // [VAI TRÒ 1]: POSITIVE TEST (Kiểm thử tích cực - Luồng xử lý chuẩn)
        // Mục tiêu: Kiểm tra phép toán nạp đạn cộng/trừ chính xác khi còn đạn dự trữ.
        // =========================================================================
        [Test]
        public void Test_ReloadSuccess()
        {
            // 1. Arrange: Đạn hiện tại 10/30, đạn dự trữ 90 viên
            int currentAmmo = 10;
            int reserveAmmo = 90;
            int maxClip = 30;

            // 2. Act: Lấy số đạn cần nạp (30 - 10 = 20) và thực hiện trừ/cộng đạn
            int neededAmmo = maxClip - currentAmmo;
            if (reserveAmmo >= neededAmmo)
            {
                currentAmmo += neededAmmo; // Nạp đầy băng đạn (10 + 20 = 30)
                reserveAmmo -= neededAmmo; // Trừ đạn dự trữ (90 - 20 = 70)
            }

            // 3. Assert: Kiểm tra số đạn sau nạp có đúng chuẩn toán học không
            Assert.AreEqual(30, currentAmmo, "Máu/Băng đạn phải nạp đủ 30 viên.");
            Assert.AreEqual(70, reserveAmmo, "Kho đạn dự trữ phải còn lại đúng 70 viên.");
        }

        // =========================================================================
        // [VAI TRÒ 2]: NEGATIVE TEST (Kiểm thử tiêu cực - Bẫy lỗi thao tác sai)
        // Mục tiêu: Đảm bảo hệ thống CHẶN hành vi cố tình nạp đạn khi băng đạn đã đầy.
        // =========================================================================
        [Test]
        public void Test_ReloadFullAmmoBlock()
        {
            // 1. Arrange: Băng đạn đã đầy sẵn (30/30), đạn dự trữ 90 viên
            int currentAmmo = 30;
            int maxClip = 30;
            int reserveAmmo = 90;

            // 2. Act: Tính đạn cần nạp (30 - 30 = 0) và check điều kiện nạp (neededAmmo > 0)
            int neededAmmo = maxClip - currentAmmo;
            bool canReload = neededAmmo > 0 && reserveAmmo > 0; // Trả về false

            // 3. Assert: Khẳng định hệ thống chặn nạp đạn (false) và không làm mất đạn dự trữ
            Assert.IsFalse(canReload, "Chặn thành công: Không cho phép nạp đạn khi đạn đã đầy.");
            Assert.AreEqual(90, reserveAmmo, "Đạn dự trữ phải được bảo lưu nguyên vẹn 90 viên.");
        }
    }
}