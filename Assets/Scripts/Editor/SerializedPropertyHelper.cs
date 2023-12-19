using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.EditorUtilities
{
    public static class SerializedPropertyHelper
    {
        public static void AssignEnumObj(this SerializedProperty property, object value) =>
            AssignEnumByName(property, value.ToString());
        public static void AssignEnumByName(this SerializedProperty property, string enumName)
        {
            var names = property.enumNames;
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == enumName.ToString())
                {
                    property.enumValueIndex = i;
                    break;
                }
            }
        }

        public static object ReturnValueAsObject(this SerializedProperty property) => property.propertyType switch
        {
            SerializedPropertyType.Generic => GetValueWithReflection(property),
            SerializedPropertyType.Integer => property.intValue,
            SerializedPropertyType.Boolean => property.boolValue,
            SerializedPropertyType.Float => property.floatValue,
            SerializedPropertyType.String => property.stringValue,
            SerializedPropertyType.Color => property.colorValue,
            SerializedPropertyType.ObjectReference => property.objectReferenceValue,
            SerializedPropertyType.LayerMask => property.intValue,
            SerializedPropertyType.Enum => property.GetValueWithReflection(),
            SerializedPropertyType.Vector2 => property.vector2Value,
            SerializedPropertyType.Vector3 => property.vector3Value,
            SerializedPropertyType.Vector4 => property.vector4Value,
            SerializedPropertyType.Rect => property.rectValue,
            SerializedPropertyType.ArraySize => property.intValue,
            SerializedPropertyType.Character => property.stringValue,
            SerializedPropertyType.AnimationCurve => property.animationCurveValue,
            SerializedPropertyType.Bounds => property.boundsValue,
            SerializedPropertyType.Gradient => property.GetValueWithReflection(),
            SerializedPropertyType.Quaternion => property.quaternionValue,
            SerializedPropertyType.ExposedReference => property.GetValueWithReflection(),
            SerializedPropertyType.FixedBufferSize => property.intValue,
            SerializedPropertyType.Vector2Int => property.vector2IntValue,
            SerializedPropertyType.Vector3Int => property.vector3IntValue,
            SerializedPropertyType.RectInt => property.rectIntValue,
            SerializedPropertyType.BoundsInt => property.boundsIntValue,
            SerializedPropertyType.ManagedReference => property.GetValueWithReflection(),
            _ => property.GetValueWithReflection(),
        };

        public static string GetEnumWithReflection(this SerializedProperty property)
        {
            if (TryGetValueWithReflection(property, out var value))
            {
                return value;
            }
            var enumIndex = property.enumValueIndex;
            var enumNames = property.enumNames;
            if (0 <= enumIndex && enumIndex < enumNames.Length)
            {
                return enumNames[enumIndex];
            }
            var intVal = property.intValue;
            if (0 <= intVal && intVal < enumNames.Length)
            {
                return enumNames[intVal];
            }
            return "Error";
        }

        public static string GetValueWithReflection(this SerializedProperty property)
        {
            if (TryGetValueWithReflection(property, out var value))
            {
                return value;
            }
            return value;
        }

        private static bool TryGetValueWithReflection(this SerializedProperty property, out string value)
        {
            if (property == null)
            {
                value = "null";
                return false;
            }
            var target = property.serializedObject.targetObject as object;
            var splitProperty = property.propertyPath.Split('.');
            for (int i = 0; i < splitProperty.Length; i++)
            {
                var subPath = string.Join(".", splitProperty.Take(i + 1).ToArray());
                using var subProperty = property.serializedObject.FindProperty(subPath);

                var propertyIsArray = subProperty != null && subProperty.isArray;
                string name = splitProperty[i];
                var f = GetField(name, target);
                if (f == null)
                {
                    value = "ERROR";
                    return false;
                }
                target = f.GetValue(target);

                if (propertyIsArray)
                {
                    var type = target.GetType();

                    var afterArray = property.propertyPath.Substring(subPath.Length);
                    var brOpen = afterArray.IndexOf('[');
                    var brClose = afterArray.IndexOf(']');
                    if (brOpen != -1 && brClose != -1)
                    {
                        var indexStr = afterArray.Substring(brOpen + 1, brClose - brOpen - 1);
                        if (int.TryParse(indexStr, out var index))
                        {
                            var enumerable = target as IEnumerable;
                            var myInd = 0;
                            foreach (var item in enumerable)
                            {
                                if(index == myInd)
                                {
                                    target = item;
                                    i += 2;
                                    var newSubPath = string.Join(".", splitProperty.Take(i + 1).ToArray());
                                    break;
                                }
                                myInd += 1;
                            }
                        }
                    }
                }
            }
            value = target.ToString();
            return true;
        }

        private static FieldInfo GetField(string name, object target)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var type = target.GetType();
            while (type != null)
            {
                var field = type.GetField(name, flags);
                if (field != null)
                {
                    return field;
                }
                type = type.BaseType;
            }
            return null;
        }

    }
}