using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Hierarchy Window Group Header
/// http://diegogiacomelli.com.br/unitytips-changing-the-style-of-the-hierarchy-window-group-header/
/// </summary>
[InitializeOnLoad]
public static class HierarchyWindowGroupHeader
{
    static readonly HierarchyWindowGroupHeaderSettings _settings;
    static readonly GUIStyle _style;

    static HierarchyWindowGroupHeader()
    {
        _settings = HierarchyWindowGroupHeaderSettings.Instance;
        _style = new GUIStyle();
        UpdateStyle();
        _settings.Changed.AddListener(UpdateStyle);
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }


    static void UpdateStyle()
    {
        _style.fontSize = _settings.FontSize;
        _style.fontStyle = _settings.FontStyle;
        _style.alignment = _settings.Alignment;
        _style.normal.textColor = _settings.TextColor;

        EditorApplication.RepaintHierarchyWindow();
    }

    static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject != null && gameObject.name.StartsWith(_settings.NameStartsWith, StringComparison.Ordinal))
        {
            EditorGUI.DrawRect(selectionRect, _settings.BackgroundColor);
            EditorGUI.LabelField(selectionRect, gameObject.name.Replace(_settings.RemoveString, "").ToUpperInvariant(), _style);
        }
    }
}


public class HierarchyWindowGroupHeaderSettings : ScriptableObject
{
    [HideInInspector] public UnityEvent Changed;
    public string NameStartsWith = "---";
    public string RemoveString = "-";
    public FontStyle FontStyle = FontStyle.Bold;
    public int FontSize = 14;
    public TextAnchor Alignment = TextAnchor.MiddleCenter;
    public Color TextColor = Color.black;
    public Color BackgroundColor = Color.gray;
    static HierarchyWindowGroupHeaderSettings _instance;
    public static HierarchyWindowGroupHeaderSettings Instance => _instance ?? (_instance = LoadAsset());

    void OnValidate()
    {
        Changed?.Invoke();
    }

    private static HierarchyWindowGroupHeaderSettings LoadAsset()
    {
        var path = GetAssetPath();
        var asset = AssetDatabase.LoadAssetAtPath<HierarchyWindowGroupHeaderSettings>(path);
        if (asset == null)
        {
            asset = CreateInstance<HierarchyWindowGroupHeaderSettings>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
        }

        return asset;
    }


    private static string GetAssetPath([CallerFilePath] string callerFilePath = null)
    {
        var folder = Path.GetDirectoryName(callerFilePath);
#if UNITY_EDITOR_WIN
        folder = folder.Substring(folder.LastIndexOf(@"\Assets\", StringComparison.Ordinal) + 1);
#else
        folder = folder.Substring(folder.LastIndexOf("/Assets/", StringComparison.Ordinal) + 1);

#endif
        return Path.Combine(folder, "HierarchyWindowGroupHeaderSettings.asset");
    }
}