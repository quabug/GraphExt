using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Shtif;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    [CustomPropertyDrawer(typeof(SerializeReferenceDrawerAttribute))]
    internal class SerializeReferenceDrawerAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelPosition, label);

            var attr = (SerializeReferenceDrawerAttribute) attribute;
            var filters = GetAllBuiltInTypeRestrictions();
            var (renamePattern, renameReplacement) = attr.RenamePatter.ParseReplaceRegex();
            var categoryMethodInfo = attr.CategoryName == null ? null : property.GetSiblingMethodInfo(attr.CategoryName);
            var categoryFunc = categoryMethodInfo == null
                ? null
                : (Func<Type, string>) categoryMethodInfo.CreateDelegate(typeof(Func<Type, string>), property.serializedObject.targetObject)
            ;

            var isNullable = string.IsNullOrEmpty(attr.NullableVariable) ? attr.Nullable : (bool)property.GetSiblingValue(attr.NullableVariable);

            DrawSelectionButtonForManagedReference();

            EditorGUI.PropertyField(position, property, GUIContent.none, true);

            EditorGUI.EndProperty();

            Func<Type, bool> GetAllBuiltInTypeRestrictions()
            {
                var baseType = attr.TypeRestrict;
                if (baseType == null && !string.IsNullOrEmpty(attr.TypeRestrictBySiblingProperty))
                    baseType = property.GetSiblingPropertyInfo(attr.TypeRestrictBySiblingProperty).PropertyType;
                if (baseType == null && !string.IsNullOrEmpty(attr.TypeRestrictBySiblingTypeName))
                    baseType = Type.GetType((string)property.GetSiblingValue(attr.TypeRestrictBySiblingTypeName));
                return baseType == null ? (Func<Type, bool>)(_ => true) : TypeCache.GetTypesDerivedFrom(baseType).Contains;
            }

            void DrawSelectionButtonForManagedReference()
            {
                var buttonPosition = position;
                buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
                buttonPosition.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
                buttonPosition.height = EditorGUIUtility.singleLineHeight;

                var referenceType = property.GetManagedFullType();

                var storedIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                var storedColor = GUI.backgroundColor;
                GUI.backgroundColor = !isNullable && referenceType == null ? new Color(1, 0, 0) : new Color(0.1f, 0.55f, 0.9f, 1f);

                var content = referenceType == null ? new GUIContent("Null ( Assign )") : MakeContent(referenceType);
                if (GUI.Button(buttonPosition, content))
                    ShowContextMenuForManagedReference();

                GUI.backgroundColor = storedColor;
                EditorGUI.indentLevel = storedIndent;

                void ShowContextMenuForManagedReference()
                {
                    var context = new GenericMenu();
                    FillContextMenu(context);
                    var popup = GenericMenuPopup.Get(context, "");
                    popup.showSearch = false;
                    popup.showTooltip = false;
                    popup.resizeToContent = true;
                    popup.Show(new Vector2(buttonPosition.x, buttonPosition.y));
                }
            }

            void FillContextMenu(GenericMenu contextMenu)
            {
                // Adds "Make Null" menu command
                if (isNullable) contextMenu.AddItem(new GUIContent("Null"), false, SetManagedReferenceToNull);

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

            void SetManagedReferenceToNull()
            {
                property.serializedObject.Update();
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }

            void AssignNewInstanceCommand(object typeObject)
            {
                var type = (Type)typeObject;
                if (type == property.GetObject()?.GetType()) return;

                var instance = Activator.CreateInstance(type);
                property.serializedObject.Update();
                property.managedReferenceValue = instance;
                property.serializedObject.ApplyModifiedProperties();
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

            IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference()
            {
                var fieldType = GetTypeFromName(property.managedReferenceFieldTypename);
                var appropriateTypes = new List<Type>();
                if (fieldType == null) return appropriateTypes;

                // Get and filter all appropriate types
                var derivedTypes = TypeCache.GetTypesDerivedFrom(fieldType);
                foreach (var type in derivedTypes)
                {
                    // Skips unity engine Objects (because they are not serialized by SerializeReference)
                    if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                        continue;
                    // Skip abstract classes because they should not be instantiated
                    if (type.IsAbstract)
                        continue;
                    if (type.IsGenericType)
                        continue;
                    if (!type.IsPublic && !type.IsNestedPublic)
                        continue;
                    // Skip types that has no public empty constructors (activator can not create them)
                    if (type.IsClass && type.GetConstructor(Type.EmptyTypes) == null) // Structs still can be created (strangely)
                        continue;
                    // Filter types by provided filters if there is ones
                    if (filters.Invoke(type) == false)
                        continue;

                    appropriateTypes.Add(type);
                }

                return appropriateTypes;
            }

            Type GetTypeFromName(string fullName)
            {
                var names = fullName.Split(' ');
                return names.Length != 2 ? null : Type.GetType($"{names[1]}, {names[0]}");
            }
        }
    }

    internal static class SerializeReferenceExtensions
    {
        public static object GetSiblingValue(this SerializedProperty property, string name)
        {
            var obj = GetDeclaringObject(property);
            var type = obj.GetType();
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fieldInfo = type.GetField(name, flags);
            if (fieldInfo != null) return fieldInfo.GetValue(obj);
            var propertyInfo = type.GetProperty(name, flags);
            if (propertyInfo != null) return propertyInfo.GetValue(obj);
            var methodInfo = type.GetMethod(name, flags);
            return methodInfo.Invoke(obj, Array.Empty<object>());
        }

        public static PropertyInfo GetSiblingPropertyInfo(this SerializedProperty property, string propertyName)
        {
            var obj = GetDeclaringObject(property);
            return obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static MethodInfo GetSiblingMethodInfo(this SerializedProperty property, string methodName)
        {
            var obj = GetDeclaringObject(property);
            return obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static object GetDeclaringObject(this SerializedProperty property)
        {
            return GetDeclaringField(property).field;
        }

        public static (object field, FieldInfo fieldInfo) GetDeclaringField(this SerializedProperty property)
        {
            return property.GetFieldsByPath().Reverse().Skip(1).First();
        }

        public static object GetObject(this SerializedProperty property)
        {
            return property.GetFieldsByPath().Last().field;
        }

        private static Regex _arrayData = new Regex(@"^data\[(\d+)\]$");

        public static IEnumerable<(object field, FieldInfo fi)> GetFieldsByPath(this SerializedProperty property)
        {
            var obj = (object)property.serializedObject.targetObject;
            FieldInfo fi = null;
            yield return (obj, fi);
            var pathList = property.propertyPath.Split('.');
            for (var i = 0; i < pathList.Length; i++)
            {
                var fieldName = pathList[i];
                if (fieldName == "Array" && i + 1 < pathList.Length && _arrayData.IsMatch(pathList[i + 1]))
                {
                    i++;
                    var itemIndex = int.Parse(_arrayData.Match(pathList[i]).Groups[1].Value);
                    var array = (IList)obj;
                    obj = array != null && itemIndex < array.Count ? array[itemIndex] : null;
                    yield return (obj, fi);
                }
                else
                {
                    var t = Field(obj, obj?.GetType() ?? fi.FieldType, fieldName);
                    obj = t.field;
                    fi = t.fi;
                    yield return t;
                }
            }

            (object field, FieldInfo fi) Field(object declaringObject, Type declaringType, string fieldName)
            {
                var fieldInfo = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var fieldValue = declaringObject == null ? null : fieldInfo.GetValue(declaringObject);
                return (fieldValue, fieldInfo);
            }
        }

        internal static (Regex, string) ParseReplaceRegex(this string pattern, string separator = "||")
        {
            if (string.IsNullOrEmpty(pattern)) return (null, null);
            var patterns = pattern.Split(new[] { separator }, StringSplitOptions.None);
            if (patterns.Length == 2) return (new Regex(patterns[0]), patterns[1]);
            throw new ArgumentException($"invalid number of separator ({separator}) in pattern ({pattern})");
        }

        private static Func<SerializedProperty, Type, Type> _getDrawerTypeForPropertyAndType;

        [CanBeNull] public static Type GetManagedFullType([NotNull] this SerializedProperty property)
        {
            return property.propertyType != SerializedPropertyType.ManagedReference
                ? null
                : GetTypeByTypename(property.managedReferenceFullTypename)
            ;
        }

        private static Type GetTypeByTypename(string typename)
        {
            var names = typename.Split(' ');
            return names.Length != 2 ? null : Type.GetType($"{names[1]}, {names[0]}");
        }
    }
}