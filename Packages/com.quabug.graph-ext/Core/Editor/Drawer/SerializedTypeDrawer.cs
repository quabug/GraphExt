using System;
using System.Collections.Generic;
using System.Linq;
using Shtif;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    [CustomPropertyDrawer(typeof(SerializedTypeAttribute))]
    public class SerializedTypeDrawer : PropertyDrawer
    {
        private string[] _options;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, label);

            position.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
            position.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;

            var attr = (SerializedTypeAttribute) attribute;
            var (renamePattern, renameReplacement) = attr.RenamePatter.ParseReplaceRegex();
            var self = property.GetDeclaringObject();
            var categoryMethodInfo = attr.CategoryName == null ? null : property.GetSiblingMethodInfo(attr.CategoryName);
            var categoryFunc = categoryMethodInfo == null
                ? null
                : (Func<Type, string>) categoryMethodInfo.CreateDelegate(typeof(Func<Type, string>), self)
            ;
            var whereMethodInfo = attr.Where == null ? null : property.GetSiblingMethodInfo(attr.Where);
            var whereFunc = whereMethodInfo == null
                ? type => true
                : (Func<Type, bool>) whereMethodInfo.CreateDelegate(typeof(Func<Type, bool>), self)
            ;

            ShowButton();

            void ShowButton()
            {
                var type = Type.GetType(property.stringValue);

                var storedColor = GUI.backgroundColor;
                GUI.backgroundColor = attr.Nullable == false && type == null ? new Color(1, 0, 0) : new Color(0.1f, 0.55f, 0.9f, 1f);

                var content = type == null ? new GUIContent("Null ( Assign )") : MakeContent(type);
                if (GUI.Button(position, content)) ShowContextMenuForManagedReference();

                GUI.backgroundColor = storedColor;
            }

            GUIContent MakeContent(Type type)
            {
                var entryName = type.FullName;
                if (renamePattern != null) entryName = renamePattern.Replace(entryName, renameReplacement);
                if (attr.DisplayAssemblyName) entryName += "  ( " + type.Assembly.GetName().Name + " )";
                if (categoryFunc != null)
                {
                    var category = categoryFunc(type);
                    if (!string.IsNullOrEmpty(category)) entryName = category + "/" + entryName;
                }
                return new GUIContent(entryName, $"{type.FullName} ( {type.Assembly.GetName().Name} )");
            }

            void ShowContextMenuForManagedReference()
            {
                var context = new GenericMenu();
                FillContextMenu(context);
                var popup = GenericMenuPopup.Get(context, "");
                popup.showSearch = true;
                popup.showTooltip = false;
                popup.resizeToContent = true;
                popup.Show(new Vector2(position.x, position.y));
            }

            void FillContextMenu(GenericMenu contextMenu)
            {
                // Adds "Make Null" menu command
                if (attr.Nullable) contextMenu.AddItem(new GUIContent("Null"), false, SetNull);

                // Collects appropriate types
                var appropriateTypes = GetAppropriateTypesForAssigningToManagedReference();

                // Adds appropriate types to menu
                var typeContentMap =
                    from type in appropriateTypes
                    select (type, content: MakeContent(type))
                ;

                if (attr.AlphabeticalOrder) typeContentMap = typeContentMap.OrderBy(t => t.content.text);
                foreach (var (type, content) in typeContentMap)
                    contextMenu.AddItem(content, false, AssignNewInstanceCommand, type);
            }

            void SetNull()
            {
                property.serializedObject.Update();
                property.stringValue = "";
                property.serializedObject.ApplyModifiedProperties();
            }

            void AssignNewInstanceCommand(object typeObject)
            {
                var type = (Type)typeObject;
                if (type == property.GetObject()?.GetType()) return;

                property.serializedObject.Update();
                property.stringValue = type.AssemblyQualifiedName;
                property.serializedObject.ApplyModifiedProperties();
            }

            IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference()
            {
                var baseType = attr.BaseType ?? typeof(object);
                var types = TypeCache.GetTypesDerivedFrom(baseType).Where(whereFunc);
                if (attr.InstantializableType) types = types.Where(type => !type.IsAbstract && !type.IsGenericType);
                if (attr.HasDefaultConstructor) types = types.Where(type => type.GetConstructors().Any(ci => !ci.GetParameters().Any()));
                return types;
            }
        }
    }

}