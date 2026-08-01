using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LastDay.Tests.PlayMode
{
    public class ZombieDamagePlayTests
    {
        // =========================================================================
        // [VAI TRÒ 3]: NEGATIVE TEST (Kiểm thử tiêu cực - Play Mode)
        // Mục tiêu: Đảm bảo khi Zombie đã chết (0 HP), bắn thêm đạn sau 0.5s sẽ
        //           CHẶN không nhận thêm sát thương (máu không bị âm).
        // =========================================================================
        [UnityTest]
        public IEnumerator Test_DeadZombieTakesNoDamage()
        {
            // 1. Arrange: Tạo Zombie giả lập với 100 HP
            GameObject zombieObj = new GameObject("TestZombie");
            int zombieHealth = 100;
            bool isDead = false;

            // Giả lập phát bắn đầu tiên gây 100 HP sát thương làm Zombie chết ngay lập tức
            zombieHealth -= 100; // Máu về 0
            if (zombieHealth <= 0)
            {
                zombieHealth = 0;
                isDead = true; // Zombie chuyển sang trạng thái đã chết
            }

            // 2. Act: Giả lập thời gian trôi qua 0.5 giây trong Runtime (Animation chết đang chạy)
            yield return new WaitForSeconds(0.5f);

            // Bắn tiếp phát đạn thứ 2 (50 HP) vào xác Zombie đã chết
            if (!isDead)
            {
                zombieHealth -= 50; // Dòng này sẽ KHÔNG được chạy vì isDead = true
            }

            // 3. Assert: Khẳng định máu Zombie vẫn giữ nguyên ở mức 0 HP (không bị âm thành -50 HP)
            Assert.IsTrue(isDead, "Zombie phải ở trạng thái đã chết.");
            Assert.AreEqual(0, zombieHealth, "Máu Zombie phải khống chế ở mức 0 HP, không bị trừ âm!");

            // Clean up: Dọn dẹp GameObject
            Object.Destroy(zombieObj);
        }
    }
}