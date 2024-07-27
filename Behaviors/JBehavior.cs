using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLL.Behaviors
{
    public class JBehavior
    {
        private static List<JLevelWeatherEffect> jWeatherEffects = new List<JLevelWeatherEffect>();

        public static void AddWeatherEffect(JLevelWeatherEffect effect)
        {
            jWeatherEffects.Add(effect);
        }

        public static List<JLevelWeatherEffect> GetWeatherEffects()
        {
            return jWeatherEffects;
        }
    }
}
