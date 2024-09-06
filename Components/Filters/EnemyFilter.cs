using UnityEngine;

namespace JLL.Components.Filters
{
    public class EnemyFilter : JFilter<EnemyAI>
    {
        [Header("EnemyType")]
        public NameFilter enemyType = new NameFilter();
        public CheckFilter isInvulnerable = new CheckFilter();

        [Header("Enemy Stats")]
        public NumericFilter healthCheck = new NumericFilter() { value = 2f };

        public override void Filter(EnemyAI enemy)
        {
            bool success = true;

            if (enemyType.shouldCheck)
            {
                success &= enemyType.CheckValue(enemy.enemyType.enemyName);
            }

            if (isInvulnerable.shouldCheck)
            {
                success &= isInvulnerable.CheckValue(!enemy.enemyType.canDie);
            }

            if (healthCheck.shouldCheck)
            {
                success &= healthCheck.CheckValue(enemy.enemyHP);
            }

            Result(success, enemy);
        }
    }
}
