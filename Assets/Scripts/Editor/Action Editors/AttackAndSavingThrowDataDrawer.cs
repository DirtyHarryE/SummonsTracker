using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public class AttackAndSavingThrowDataDrawer : AttackDataDrawer
    {
        public AttackAndSavingThrowDataDrawer(AttackAndSavingThrowData target) : base(target) { }

        protected override void OnBeforeDrawNote()
        {
            base.OnBeforeDrawNote();
            SavingThrowDataDrawer.DrawSavingThrow(GetRect, SerializedObject);
        }
    }
}