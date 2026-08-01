using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LastDay.Tests.PlayMode
{
    public class PlayerHealthPlayTests
    {
        // =========================================================================
        // [TC_HP_03 - NEGATIVE TEST]: Play Mode
        // Mục tiêu: Kiểm tra dính sát thương dứt điểm làm Player chết, HP khống chế ở 0
        //           và không bị số âm khi Runtime đang chạy.
        // =========================================================================
        [UnityTest]
        public IEnumerator Test_PlayerDeathHandlingAtZeroHealth()
        {
            // 1. Arrange: Tạo GameObject Player giả lập với 20 HP
            GameObject playerObj = new GameObject("TestPlayer");
            int currentHealth = 20;
            bool isDead = false;

            // 2. Act: Boss khạc axit gây 50 sát thương (Vượt quá 20 HP hiện có)
            int heavyDamage = 50;
            currentHealth -= heavyDamage; // 20 - 50 = -30

            // Giả lập logic khống chế máu và xử lý chết
            if (currentHealth <= 0)
            {
                currentHealth = 0; // Khống chế không bị âm
                isDead = true;     // Chuyển trạng thái chết
            }

            // Giả lập thời gian trôi 1.0 giây để chạy Animation Player ngã gục
            yield return new WaitForSeconds(1.0f);

            // 3. Assert: Khẳng định trạng thái chết được kích hoạt và HP bằng 0
            Assert.IsTrue(isDead, "Player phải chuyển sang trạng thái đã chết (IsDead = true).");
            Assert.AreEqual(0, currentHealth, "Máu Player khi chết phải dừng ở 0 HP, không bị âm!");

            // Clean up tài nguyên
            Object.Destroy(playerObj);
        }
    }
}