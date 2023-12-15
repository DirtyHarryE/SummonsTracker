using SummonsTracker.Licensing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SummonsTracker.Licensing
{
    public abstract class LicensedScriptableObject : ScriptableObject, ILicensing
    {
        Licenses ILicensing.License => _license;

        [SerializeField]
        private Licenses _license;
    }
}