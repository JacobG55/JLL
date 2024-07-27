using System;

namespace JLL.Behaviors
{
    public abstract class JLevelWeatherEffect
    {
        public abstract string getSceneName();

        public abstract void applyEffects(TimeOfDay timeOfDay, Random random);
    }
}
