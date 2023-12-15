using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SummonsTracker.EditorUtilities
{
    public static class SerializedObjectHelper 
    {
        public static void Copy(SerializedObject from, SerializedObject to)
        {
            var iterator = from.GetIterator();
            iterator.NextVisible(true);
            bool enterChildren;
            do
            {
                enterChildren = true;
                using var fromProp = from.FindProperty(iterator.propertyPath);
                using var toProp = to.FindProperty(iterator.propertyPath);
                if (fromProp.isArray && toProp.isArray)
                {
                    toProp.arraySize = fromProp.arraySize;
                    enterChildren = true;
                }
                else if (fromProp.propertyType == toProp.propertyType)
                {
                    enterChildren = false;
                    switch (fromProp.propertyType)
                    {
                        case SerializedPropertyType.Integer:
                            toProp.intValue = fromProp.intValue;
                            break;
                        case SerializedPropertyType.Boolean:
                            toProp.boolValue = fromProp.boolValue;
                            break;
                        case SerializedPropertyType.Float:
                            toProp.floatValue = fromProp.floatValue;
                            break;
                        case SerializedPropertyType.String:
                            toProp.stringValue = fromProp.stringValue;
                            break;
                        case SerializedPropertyType.Color:
                            toProp.colorValue = fromProp.colorValue;
                            break;
                        case SerializedPropertyType.ObjectReference:
                            toProp.objectReferenceValue = fromProp.objectReferenceValue;
                            break;
                        case SerializedPropertyType.Enum:
                            toProp.intValue = fromProp.intValue;
                            break;
                        case SerializedPropertyType.Vector2:
                            toProp.vector2Value = fromProp.vector2Value;
                            break;
                        case SerializedPropertyType.Vector3:
                            toProp.vector3Value = fromProp.vector3Value;
                            break;
                        case SerializedPropertyType.Vector4:
                            toProp.vector4Value = fromProp.vector4Value;
                            break;
                        case SerializedPropertyType.Rect:
                            toProp.rectValue = fromProp.rectValue;
                            break;
                        case SerializedPropertyType.Character:
                            toProp.stringValue = fromProp.stringValue;
                            break;
                        case SerializedPropertyType.AnimationCurve:
                            toProp.animationCurveValue = fromProp.animationCurveValue;
                            break;
                        case SerializedPropertyType.Bounds:
                            toProp.boundsValue = fromProp.boundsValue;
                            break;
                        case SerializedPropertyType.Quaternion:
                            toProp.quaternionValue = fromProp.quaternionValue;
                            break;
                        case SerializedPropertyType.Vector2Int:
                            toProp.vector2IntValue = fromProp.vector2IntValue;
                            break;
                        case SerializedPropertyType.Vector3Int:
                            toProp.vector3IntValue = fromProp.vector3IntValue;
                            break;
                        case SerializedPropertyType.RectInt:
                            toProp.rectIntValue = fromProp.rectIntValue;
                            break;
                        case SerializedPropertyType.BoundsInt:
                            toProp.boundsIntValue = fromProp.boundsIntValue;
                            break;
                        default:
                            enterChildren = true;
                            break;
                    }
                }
            } while (iterator.Next(enterChildren));
            to.ApplyModifiedProperties();
        }
    }
}