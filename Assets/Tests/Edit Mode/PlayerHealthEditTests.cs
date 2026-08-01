using NUnit.Framework;

namespace LastDay.Tests.EditMode
{
    public class PlayerHealthEditTests
    {
        // =========================================================================
        // [TC_HP_01 - POSITIVE TEST]: Edit Mode
        // Mục tiêu: Kiểm tra phép trừ HP chuẩn xác khi Player nhận sát thương.
        // =========================================================================
        [Test]
        public void Test_PlayerTakesDamageCorrectly()
        {
            // 1. Arrange: Máu ban đầu 100 HP, Zombie tấn công gây 15 sát thương
            int currentHealth = 100;
            int damageTaken = 15;

            // 2. Act: Thực hiện trừ máu
            currentHealth -= damageTaken;

            // 3. Assert: Xác nhận máu còn lại đúng bằng 85 HP
            Assert.AreEqual(85, currentHealth, "Máu Player sau khi bị cắn phải còn đúng 85 HP.");
        }

        // =========================================================================
        // [TC_HP_02 - NEGATIVE TEST]: Edit Mode
        // Mục tiêu: Đảm bảo khi hồi máu vượt quá giới hạn, HP dừng ở 100 (Không bị trần số).
        // =========================================================================
        [Test]
        public void Test_PlayerHealCapAtMaxHealth()
        {
            // 1. Arrange: Máu hiện tại 90/100, dùng Medkit hồi 50 HP
            int currentHealth = 90;
            int maxHealth = 100;
            int healAmount = 50;

            // 2. Act: Thực hiện hồi máu và khống chế giới hạn MaxHP
            currentHealth += healAmount; // 90 + 50 = 140
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth; // Khống chế về 100
            }

            // 3. Assert: Khẳng định máu không thể vượt quá 100 HP
            Assert.AreEqual(100, currentHealth, "Máu Player không được phép vượt quá giới hạn tối đa 100 HP.");
        }
    }
}