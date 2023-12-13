using SummonsTracker.Characters;
using SummonsTracker.Text;
using System;

namespace SummonsTracker.Save
{
    [Serializable]
    public class SaveDamage
    {
        public int Number;
        public int Faces;
        public int Modifiers;
        public DamageTypes DamageType;

        public SaveDamage( int number, int faces,int modifiers, DamageTypes damageType)
        {
            Number = number;
            Faces = faces;
            Modifiers = modifiers;
            DamageType = damageType;
        }

        public override string ToString()
        {
            return $"{Number}d{Faces}{TextUtils.AddPlus(Modifiers)} {DamageType}";
        }
    }
}