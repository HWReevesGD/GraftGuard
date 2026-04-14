using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A class for storing all the damage, DoT, and slowdown effects for a projectile

namespace GraftGuard
{
    public class Damage
    {
        // Fields
        private float baseDamage;
        private float damageOverTime;
        private float damageOverTimeDuration;
        private float speedMod;
        private float speedModDuration;

        // Properties
        public float BaseDamage { get { return baseDamage; } }
        public float DamageOverTime { get { return damageOverTime; } }
        public float DamageOverTimeDuration { get { return damageOverTimeDuration; } }
        public float SpeedMod { get { return speedMod; } }
        public float SpeedModDuration { get {return speedModDuration; } }

        // Constructors
        /// <summary>
        /// Builds the damage profile of the projectile including the following:
        ///  - direct damage that's dealt on contact
        ///  - damage dealt per second for a set number of seconds
        ///  - intensity and duration of slowdown effect (larger number has a greater slowdown effect)
        /// </summary>
        /// <param name="baseDamage">direct damage dealt on contact</param>
        /// <param name="damageOverTime">damage taken per second after getting hit</param>
        /// <param name="damageOverTimeDuration">Duration of the DoT in seconds</param>
        /// <param name="speedMod">modifies the enemy's speed (greater number slows down more)</param>
        /// <param name="speedModDuration">duration of the slowdown effect in seconds</param>
        public Damage(float baseDamage, float damageOverTime, float damageOverTimeDuration, float speedMod, float speedModDuration)
        {
            this.baseDamage = baseDamage;
            this.damageOverTime = damageOverTime;
            this.damageOverTimeDuration = damageOverTimeDuration;
            this.speedMod= speedMod;
            this.speedModDuration = speedModDuration;
        }
    }
}
