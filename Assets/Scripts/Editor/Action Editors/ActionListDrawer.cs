using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SummonsTracker.Characters
{
    public class ActionListDrawer
    {
        public ActionListDrawer(SerializedObject serializedObject, CharacterData characterData)
        {
            _serializedObject = serializedObject;
            _characterData = characterData;
            var actionsProp = serializedObject.FindProperty("_actions");
            _reorderableList = new ReorderableList(serializedObject, actionsProp)
            {
                drawHeaderCallback = r => EditorGUI.LabelField(r, "Actions"),
                onAddDropdownCallback = OnAddDropdownCallback,
                onRemoveCallback = OnRemoveCallback,
                drawElementCallback = DrawElementCallback,
                onReorderCallbackWithDetails = OnReorderCallback,
                elementHeightCallback = ElementHeightCallback,
            };
        }

        public void DrawLayout()
        {
            _reorderableList.DoLayoutList();
        }

        private float ElementHeightCallback(int index)
        {
            return GetActionDrawer(index).GetHeight();
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var serializedProperty = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            if (serializedProperty == null)
            {
                EditorGUI.TextField(rect, "An error has occured");
                return;
            }
            GetActionDrawer(index).Draw(rect);
        }

        private void OnAddDropdownCallback(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();

            var actionType = typeof(ActionData);
            var types = Assembly.GetAssembly(actionType)
                                .GetTypes()
                                .Where(t => t.IsClass && !t.IsAbstract && (t.IsSubclassOf(actionType) || t == actionType));
            foreach (var t in types)
            {
                var attribute = t.GetCustomAttributes()
                    .Select(a => a as ActionMenuItem)
                    .Where(a => a != null)
                    .FirstOrDefault();
                var path = attribute != null ? attribute.MenuPath : $"Other/{ObjectNames.NicifyVariableName(t.Name)}";
                menu.AddItem(new GUIContent(path),
                    false,
                    delegate
                    {
                        var instance = ScriptableObject.CreateInstance(t);
                        instance.name = t == typeof(MultiattackData) ? "Multiattack" : $"New {ObjectNames.NicifyVariableName(t.Name)}";
                        AssetDatabase.AddObjectToAsset(instance, _characterData);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_characterData));

                        var size = list.serializedProperty.arraySize;
                        list.serializedProperty.arraySize = size + 1;
                        using (var newProp = list.serializedProperty.GetArrayElementAtIndex(size))
                        {
                            newProp.objectReferenceValue = instance;
                        }
                        list.serializedProperty.serializedObject.ApplyModifiedProperties();
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    });
            }
            menu.DropDown(buttonRect);
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            using (var propToRemove = list.serializedProperty.GetArrayElementAtIndex(list.index))
            {
                UnityEngine.Object.DestroyImmediate(propToRemove.objectReferenceValue, true);
            }
            list.serializedProperty.DeleteArrayElementAtIndex(list.index);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_characterData));
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private void OnReorderCallback(ReorderableList list, int oldIndex, int newIndex)
        {
            if (_actionDrawers.TryGetValue(oldIndex, out var oldDrawer))
            {
                _actionDrawers.Remove(oldIndex);
            }
            if (_actionDrawers.TryGetValue(newIndex, out var newDrawer))
            {
                _actionDrawers.Remove(newIndex);
            }
            if (oldDrawer != null)
            {
                _actionDrawers.Add(newIndex, oldDrawer);
            }
            if (newDrawer != null)
            {
                _actionDrawers.Add(oldIndex, newDrawer);
            }
        }

        private ActionDrawer GetActionDrawer(int index)
        {
            var obj = _reorderableList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;
            if (!_actionDrawers.TryGetValue(index, out var drawer) || drawer.Target != obj)
            {
                if (obj is AttackAndSavingThrowData attackAndSavingThrowData)
                {
                    CreateDrawer(index, () => new AttackAndSavingThrowDataDrawer(attackAndSavingThrowData));
                }
                else if (obj is DoubleDamageSavingThrowData doubleDamageSavingThrowData)
                {
                    CreateDrawer(index, () => new AttackDoubleDamageSavingThrowDataDrawer(doubleDamageSavingThrowData));
                }
                else if (obj is AttackSecondDamageData attackSecondDamageData)
                {
                    CreateDrawer(index, () => new AttackDoubleDamageDataDrawer(attackSecondDamageData));
                }
                else if (obj is AttackData attackData)
                {
                    CreateDrawer(index, () => new AttackDataDrawer(attackData));
                }
                else if (obj is MultiattackData multiattack)
                {
                    CreateDrawer(index, () => new MultiattackDataDrawer(multiattack, _serializedObject));
                }
                else if (obj is SavingThrowData savingThrowData)
                {
                    CreateDrawer(index, () => new SavingThrowDataDrawer(savingThrowData));
                }
                else if (obj is ActionData actionData)
                {
                    CreateDrawer(index, () => new ActionDataDrawer(actionData));
                }
            }
            return _actionDrawers[index];
        }

        private void CreateDrawer(int index, Func<ActionDrawer> func)
        {
            if (_actionDrawers.ContainsKey(index))
            {
                _actionDrawers[index] = func();
            }
            else
            {
                _actionDrawers.Add(index, func());
            }
        }

        private Dictionary<int, ActionDrawer> _actionDrawers = new Dictionary<int, ActionDrawer>();

        private ReorderableList _reorderableList;

        private SerializedObject _serializedObject;

        private CharacterData _characterData;
    }
}