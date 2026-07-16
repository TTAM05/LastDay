using NUnit.Framework;

public class PlayerLogicTests
{
    [Test]
    public void Player_TakesDamage_CurrentHPLowersCorrectly()
    {
        // 1. ARRANGE (Thiết lập)
        int currentHP = 100;
        int damageReceived = 30;

        // 2. ACT (Hành động)
        int actualHPAfterDamage = currentHP - damageReceived;

        // 3. ASSERT (Khẳng định kết quả mong đợi: 100 - 30 = 70)
        int expectedHP = 70;
        Assert.AreEqual(expectedHP, actualHPAfterDamage);
    }
}