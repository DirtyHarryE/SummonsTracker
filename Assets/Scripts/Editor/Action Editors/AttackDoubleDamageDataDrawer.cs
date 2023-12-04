using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public class AttackDoubleDamageDataDrawer : AttackDataDrawer
    {
        public AttackDoubleDamageDataDrawer(AttackSecondDamageData target) : base(target) { }

        protected override void OnBeforeDrawNote()
        {
            base.OnBeforeDrawNote();
            DrawDamageDice(SerializedObject.FindProperty("_secondaryDamage"), SerializedObject.FindProperty("_secondaryDamageType"));
        }
    }
}