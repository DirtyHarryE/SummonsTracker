using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public class AttackDoubleDamageSavingThrowDataDrawer : AttackDoubleDamageDataDrawer
    {
        public AttackDoubleDamageSavingThrowDataDrawer(DoubleDamageSavingThrowData target) : base(target) { }

        protected override void OnBeforeDrawNote()
        {
            base.OnBeforeDrawNote();
            SavingThrowDataDrawer.DrawSavingThrow(GetRect, SerializedObject);
        }
    }
}