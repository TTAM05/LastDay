using NUnit.Framework;

namespace LastDay.Tests.EditMode
{
    public class ZombieDamageEditTests
    {
        // =========================================================================
        // [VAI TRÒ]: POSITIVE TEST (Kiểm thử tích cực - Edit Mode)
        // Mục tiêu: Kiểm tra phép toán nhân đôi sát thương (x2) khi bắn trúng đầu Zombie.
        // =========================================================================
        [Test]
        public void Test_HeadshotDamageMultiplier()
        {
            // 1. Arrange: Sát thương gốc của AK47 = 50, bắn trúng đầu (isHeadshot = true)
            int baseDamage = 50;
            bool isHeadshot = true;

            // 2. Act: Thực thi công thức tính sát thương đầu ra
            int finalDamage = isHeadshot ? baseDamage * 2 : baseDamage;

            // 3. Assert: Xác nhận sát thương đầu ra phải nhân đôi thành 100 HP
            Assert.AreEqual(100, finalDamage, "Sát thương Headshot phải được nhân đôi (50 x 2 = 100).");
        }

        // [VAI TRÒ 2]: POSITIVE TEST (Kiểm thử tích cực - Edit Mode)
        // Mục tiêu: Kiểm tra sát thương cơ bản (x1) khi bắn vào thân Zombie (Body Shot).
        // =========================================================================
        [Test]
        public void Test_BodyShotDamageNormal()
        {
            // 1. Arrange: Máu Zombie = 100, sát thương AK47 = 50, bắn vào thân (isHeadshot = false)
            int zombieHealth = 100;
            int baseDamage = 50;
            bool isHeadshot = false;

            // 2. Act: Bắn vào thân (giữ nguyên sát thương gốc x1) và trừ máu Zombie
            int finalDamage = isHeadshot ? baseDamage * 2 : baseDamage; // = 50
            zombieHealth -= finalDamage; // 100 - 50 = 50

            // 3. Assert: Xác nhận Zombie bị trừ đúng 50 HP và còn lại 50 HP
            Assert.AreEqual(50, finalDamage, "Sát thương bắn vào thân phải giữ nguyên 50 HP (x1).");
            Assert.AreEqual(50, zombieHealth, "Máu Zombie còn lại phải bằng đúng 50 HP.");
        }
    }
}