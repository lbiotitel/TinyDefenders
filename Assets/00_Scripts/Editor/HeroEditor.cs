#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
#endif

[CustomEditor(typeof(HeroScriptable))]
public class HeroEditor : Editor
{
    private ReorderableList debuffTypeList;
    private ReorderableList skillDebuffTypeList;
    private void OnEnable()
    {
        DrawDebuffEditor();
        DrawSkillDebuffEditor();
    }

    private void DrawDebuffEditor()
    {
        SerializedProperty debuffTypeProperty = serializedObject.FindProperty("debuffEffects");
        debuffTypeList = new ReorderableList(serializedObject, debuffTypeProperty, true, true, true, true);

        debuffTypeList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Hero Debuff Types");
        };

        //set elements height
        debuffTypeList.elementHeightCallback = (index) =>
        {
            SerializedProperty element = debuffTypeProperty.GetArrayElementAtIndex(index);
            SerializedProperty parameterProp = element.FindPropertyRelative("values");

            float baseHeight = EditorGUIUtility.singleLineHeight + 6.0f;
            float paramHeight = parameterProp.arraySize * (EditorGUIUtility.singleLineHeight + 4.0f);

            return baseHeight + paramHeight + 10.0f;
        };

        //draw elements
        debuffTypeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = debuffTypeProperty.GetArrayElementAtIndex((int)index);
            rect.y += 2;

            SerializedProperty effectTypeProp = element.FindPropertyRelative("debuffType");
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                effectTypeProp, new GUIContent("Debuff Type"));

            DebuffType debuffType = (DebuffType)effectTypeProp.enumValueIndex;
            SerializedProperty parameterProp = element.FindPropertyRelative("values");


            rect.y += EditorGUIUtility.singleLineHeight + 4;

            switch (debuffType)
            {
                case DebuffType.SLOW:
                    parameterProp.arraySize = 4;
                    DrawParameterField("Chance", rect, parameterProp, 0);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Debuff Level", rect, parameterProp, 1);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Slow Amount", rect, parameterProp, 2);

                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Slow Duration", rect, parameterProp, 3);

                    break;

                case DebuffType.STUN:
                    parameterProp.arraySize = 2;

                    DrawParameterField("Chance", rect, parameterProp, 0);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Stun Duration", rect, parameterProp, 1);
                    break;

                case DebuffType.BLEED:
                    parameterProp.arraySize = 3;

                    DrawParameterField("Chance", rect, parameterProp, 0);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Bleed Duration", rect, parameterProp, 1);

                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Bleed Percent", rect, parameterProp, 2);

                    break;

                case DebuffType.POISION:
                    parameterProp.arraySize = 3;
                    DrawParameterField("Chance", rect, parameterProp, 0);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Poision Duration", rect, parameterProp, 1);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Poision Tick Damage", rect, parameterProp, 2);

                    break;
            }
        };

        debuffTypeList.onAddCallback = (ReorderableList list) =>
        {
            int index = list.serializedProperty.arraySize;
            list.serializedProperty.InsertArrayElementAtIndex(index);

            SerializedProperty newElement = list.serializedProperty.GetArrayElementAtIndex((int)index);
            newElement.FindPropertyRelative("debuffType").enumValueIndex = 0;
            newElement.FindPropertyRelative("values").arraySize = 0;
        };
    }

    private void DrawSkillDebuffEditor()
    {
        SerializedProperty skillDebuffTypeProperty = serializedObject.FindProperty("skillDebuffEffects");
        skillDebuffTypeList = new ReorderableList(serializedObject, skillDebuffTypeProperty, true, true, true, true);

        skillDebuffTypeList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Hero Skill Debuff Types");
        };

        //set elements height
        skillDebuffTypeList.elementHeightCallback = (index) =>
        {
            SerializedProperty element = skillDebuffTypeProperty.GetArrayElementAtIndex(index);
            SerializedProperty parameterProp = element.FindPropertyRelative("values");

            float baseHeight = EditorGUIUtility.singleLineHeight + 6.0f;
            float paramHeight = parameterProp.arraySize * (EditorGUIUtility.singleLineHeight + 4.0f);

            return baseHeight + paramHeight + 10.0f;
        };

        //draw elements
        skillDebuffTypeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = skillDebuffTypeProperty.GetArrayElementAtIndex((int)index);
            rect.y += 2;

            SerializedProperty effectTypeProp = element.FindPropertyRelative("debuffType");
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                effectTypeProp, new GUIContent("Debuff Type"));

            DebuffType debuffType = (DebuffType)effectTypeProp.enumValueIndex;
            SerializedProperty parameterProp = element.FindPropertyRelative("values");


            rect.y += EditorGUIUtility.singleLineHeight + 4;

            switch (debuffType)
            {
                case DebuffType.SLOW:
                    parameterProp.arraySize = 4;
                    DrawParameterField("Chance", rect, parameterProp, 0);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Debuff Level", rect, parameterProp, 1);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Slow Amount", rect, parameterProp, 2);

                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Slow Duration", rect, parameterProp, 3);

                    break;

                case DebuffType.STUN:
                    parameterProp.arraySize = 2;

                    DrawParameterField("Chance", rect, parameterProp, 0);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Stun Duration", rect, parameterProp, 1);
                    break;

                case DebuffType.BLEED:
                    parameterProp.arraySize = 3;

                    DrawParameterField("Chance", rect, parameterProp, 0);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Bleed Duration", rect, parameterProp, 1);

                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Bleed Percent", rect, parameterProp, 2);

                    break;

                case DebuffType.POISION:
                    parameterProp.arraySize = 3;
                    DrawParameterField("Chance", rect, parameterProp, 0);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Poision Duration", rect, parameterProp, 1);
                    rect.y += EditorGUIUtility.singleLineHeight + 4;

                    DrawParameterField("Poision Tick Damage", rect, parameterProp, 2);

                    break;
            }
        };

        skillDebuffTypeList.onAddCallback = (ReorderableList list) =>
        {
            int index = list.serializedProperty.arraySize;
            list.serializedProperty.InsertArrayElementAtIndex(index);

            SerializedProperty newElement = list.serializedProperty.GetArrayElementAtIndex((int)index);
            newElement.FindPropertyRelative("debuffType").enumValueIndex = 0;
            newElement.FindPropertyRelative("values").arraySize = 0;
        };
    }

    private void DrawParameterField(string fieldName, Rect rect, SerializedProperty parameterProp,
        int index)
    {
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight),
                            fieldName);

        parameterProp.GetArrayElementAtIndex(index).floatValue =
            EditorGUI.FloatField(new Rect(rect.x + 130.0f, rect.y, 100, EditorGUIUtility.singleLineHeight),
            parameterProp.GetArrayElementAtIndex(index).floatValue);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rarity"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("heroName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("heroCost"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("heroMainImage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("heroDesc"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("heroTribe"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("heroClass"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("runtimeAnimatorController"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("atk"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("atkSpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("atkRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("criticalRate"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("criticalDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillAcc"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("projectile"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillData"));
        EditorGUILayout.Space(20f);

        debuffTypeList.DoLayoutList();
        skillDebuffTypeList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
