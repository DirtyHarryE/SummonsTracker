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
        public bool UsePersistentData;

        public ActionListDrawer(SerializedObject serializedObject, CharacterData characterData, bool usePersistentData = true)
        {
            UsePersistentData = usePersistentData;
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
            if (_reorderableList == null)
            {
                EditorGUILayout.HelpBox("List is null!", MessageType.Error, true);
            }
            else
            {
                _reorderableList.DoLayoutList();
            }
        }

        private float ElementHeightCallback(int index)
        {
            if (GetActionDrawer(index, out var drawer))
            {
                return drawer.GetHeight();
            }
            return EditorStyles.label.CalcSize(EditorGUIUtility.IconContent("console.erroricon.sml")).y;
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var serializedProperty = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            if (serializedProperty == null)
            {
                EditorGUI.TextField(rect, "An error has occured");
                return;
            }
            if (GetActionDrawer(index, out var drawer))
            {
                drawer.Draw(rect);
            }
            else
            {
                var content = new GUIContent(EditorGUIUtility.IconContent("console.erroricon.sml"))
                {
                    text = "Drawer not Found!"
                };
                EditorGUI.LabelField(rect, content);
            }
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
                        if (UsePersistentData)
                        {
                            AssetDatabase.AddObjectToAsset(instance, _characterData);
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_characterData));
                        }
                        var size = list.serializedProperty.arraySize;
                        list.serializedProperty.arraySize = size + 1;
                        using (var newProp = list.serializedProperty.GetArrayElementAtIndex(size))
                        {
                            newProp.objectReferenceValue = instance;
                        }
                        list.serializedProperty.serializedObject.ApplyModifiedProperties();
                        if (UsePersistentData)
                        {
                            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                        }
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
            if (UsePersistentData)
            {
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_characterData));
            }
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
            if (UsePersistentData)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
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

        private bool GetActionDrawer(int index, out ActionDrawer drawer)
        {
            var obj = _reorderableList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;

            if (_actionDrawers.TryGetValue(index, out var myDrawer) && myDrawer.Target == obj)
            {
                drawer = myDrawer;
                return true;
            }
            if (obj is AttackAndSavingThrowData attackAndSavingThrowData)
            {
                drawer = CreateDrawer(index, () => new AttackAndSavingThrowDataDrawer(attackAndSavingThrowData));
                return true;
            }
            else if (obj is DoubleDamageSavingThrowData doubleDamageSavingThrowData)
            {
                drawer = CreateDrawer(index, () => new AttackDoubleDamageSavingThrowDataDrawer(doubleDamageSavingThrowData));
                return true;
            }
            else if (obj is AttackSecondDamageData attackSecondDamageData)
            {
                drawer = CreateDrawer(index, () => new AttackDoubleDamageDataDrawer(attackSecondDamageData));
                return true;
            }
            else if (obj is AttackData attackData)
            {
                drawer = CreateDrawer(index, () => new AttackDataDrawer(attackData));
                return true;
            }
            else if (obj is MultiattackData multiattack)
            {
                drawer = CreateDrawer(index, () => new MultiattackDataDrawer(multiattack, _serializedObject));
                return true;
            }
            else if (obj is SavingThrowData savingThrowData)
            {
                drawer = CreateDrawer(index, () => new SavingThrowDataDrawer(savingThrowData));
                return true;
            }
            else if (obj is ActionData actionData)
            {
                drawer = CreateDrawer(index, () => new ActionDataDrawer(actionData));
                return true;
            }
            else
            {
                drawer = null;
                return false;
            }
            return true;
        }

        private ActionDrawer CreateDrawer(int index, Func<ActionDrawer> func)
        {
            var drawer = func();
            if (_actionDrawers.ContainsKey(index))
            {
                _actionDrawers[index] = drawer;
            }
            else
            {
                _actionDrawers.Add(index, drawer);
            }
            return drawer;
        }

        private Dictionary<int, ActionDrawer> _actionDrawers = new Dictionary<int, ActionDrawer>();

        private ReorderableList _reorderableList;

        private SerializedObject _serializedObject;

        private CharacterData _characterData;
    }
}