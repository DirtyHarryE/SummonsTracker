using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Characters
{
    [ActionMenuItem("Attack/Double Damage/No Saving Throw")]
    public class AttackSecondDamageData : AttackData
    {
        [SerializeField]
        private Rolling.Dice _secondaryDamage = new Rolling.Dice(1, 8, 0);
        [SerializeField]
        private DamageTypes _secondaryDamageType = DamageTypes.Necrotic;


        public override Action Instantiate()
        {
            var atk = (Attack)base.Instantiate();
            var damages = new List<Attack.Damage>(atk.Damages);
            damages.Add(new Attack.Damage(_secondaryDamage, _secondaryDamageType));
            atk.Damages = damages.ToArray();
            return atk;
        }
    }
}