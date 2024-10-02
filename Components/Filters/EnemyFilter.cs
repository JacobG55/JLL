using UnityEngine;

namespace JLL.Components.Filters
{
    public class EnemyFilter : JFilter<EnemyAI>
    {
        [Header("EnemyType")]
        [Tooltip("Checks if the name of the EnemyType matches the given name")]
        public NameFilter enemyType = new NameFilter();
        [Tooltip("Checks EnemyType's 'canDie' property")]
        public CheckFilter isInvulnerable = new CheckFilter();

        [Header("Enemy Stats")]
        public NumericFilter healthCheck = new NumericFilter() { value = 2f };

        public override void Filter(EnemyAI enemy)
        {
            if (enemyType.shouldCheck && !enemyType.CheckValue(enemy.enemyType.enemyName))
            {
                goto Failed;
            }

            if (isInvulnerable.shouldCheck && !isInvulnerable.CheckValue(!enemy.enemyType.canDie))
            {
                goto Failed;
            }

            if (healthCheck.shouldCheck && !healthCheck.CheckValue(enemy.enemyHP))
            {
                goto Failed;
            }

            Result(enemy, true);
            return;

            Failed:
            Result(enemy);
        }
    }
}
