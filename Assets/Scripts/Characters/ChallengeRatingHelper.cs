using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public static class ChallengeRatingHelper
    {
        public const char SLASH = '\u2044';
        public const string HALF = "\u00BD";
        public const string QUARTER = "\u00BC";
        public const string EIGHTH = "\u215B";
        public const string UNKNOWN = "Unknown";

        public static string[] Ratings { get; private set; }
        public static float[] RatingValues { get; private set; }

        static ChallengeRatingHelper()
        {
            Ratings = GetChallengeRatings().ToArray();
            RatingValues = Ratings.Select(CRToFloat).ToArray();

            FloatToCRDict = new Dictionary<float, string>();
            CRToFloatDict = new Dictionary<string, float>();

            for (int i = 0; i < Ratings.Length; i++)
            {
                FloatToCRDict.Add(RatingValues[i], Ratings[i]);
                CRToFloatDict.Add(Ratings[i], RatingValues[i]);
            }
        }

        public static float CRToFloat(string cr)
        {
            var result = -1f;
            if (CRToFloatDict!= null && CRToFloatDict.TryGetValue(cr, out result))
            {
                return result;
            }
            if (InternalCRToFloat(cr, out result))
            {
                return result;
            }
            var bOpenInd = cr.IndexOf('(');
            var bCloseInd = cr.IndexOf(')');

            if (bOpenInd != -1 && bCloseInd != -1)
            {
                if (InternalCRToFloat($"{cr.Substring(0, bOpenInd).Trim()}{cr.Substring(bCloseInd + 1).Trim()}", out result))
                {
                    return result;
                }
            }
            if (bOpenInd != -1 )
            {
                if (InternalCRToFloat(cr.Substring(0, bOpenInd).Trim(), out result))
                {
                    return result;
                }
            }
            var split = cr.Split(' ');
            for (int i = 0; i < split.Length; i++)
            {
                if (InternalCRToFloat(split[i], out result))
                {
                    return result;
                }
            }
            return -1;
        }

        public static string FloatToCR(float f)
        {
            if (FloatToCRDict != null && FloatToCRDict.TryGetValue(f, out var cr))
            {
                return cr;
            }
            if (f < 0)
            {
                return UNKNOWN;
            }
            for (int i = 0; i < Ratings.Length; i++)
            {
                if (Mathf.Approximately(f, CRToFloat(Ratings[i])))
                {
                    return Ratings[i];
                }
            }
            return UNKNOWN;
        }

        private static bool InternalCRToFloat(string cr, out float result)
        {
            switch (cr.Trim())
            {
                case UNKNOWN:
                    result = -1;
                    return true;
                case EIGHTH:
                    result = 1f / 8f;
                    return true;
                case QUARTER:
                    result = 0.25f;
                    return true;
                case HALF:
                    result = 0.5f;
                    return true;
                default:
                    if (float.TryParse(cr, out var f))
                    {
                        result = f;
                        return true;
                    }
                    var seperators = new[] { new string(SLASH, 1), "/", ".", "," };
                    for (int i = 0; i < seperators.Length; i++)
                    {
                        var s = seperators[i];
                        var index = cr.IndexOf(s);
                        if (index != -1)
                        {
                            if (float.TryParse(cr.Substring(0, index).Trim(), out var u) && float.TryParse(cr.Substring(index + s.Length).Trim(), out var d))
                            {
                                result = u / d;
                                return true;
                            }
                        }
                    }
                    result = -1;
                    return false;
            }
        }

        public static IEnumerable<string> GetChallengeRatings()
        {
            yield return "0";
            yield return EIGHTH; // "$"1{SLASH}8";
            yield return QUARTER; // $"1{SLASH}4";
            yield return HALF; // $"1{SLASH}2";
            for (int i = 1; i <= 30; i++)
            {
                yield return i.ToString();
            }
            yield return UNKNOWN;
        }

        private static Dictionary<float, string> FloatToCRDict;
        private static Dictionary<string, float> CRToFloatDict;
    }
}