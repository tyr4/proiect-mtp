using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ContainersAutoRefresh : AssetPostprocessor
{
    private static readonly string[] PowerupFolders = new[]
    {
        "Assets/Scriptable Objects/Powerups/One Time Buffs",
        "Assets/Scriptable Objects/Powerups/Passive Effects",
        "Assets/Scriptable Objects/Powerups/Projectiles"
    };
    
    private static readonly string[] EnemyFolders = new[]
    {
        "Assets/Scriptable Objects/Enemies"
    };

    private static void OnPostprocessAllAssets(
        string[] importedAssets, string[] deletedAssets,
        string[] movedAssets, string[] movedFromPaths)
    {
        RefreshAll<PowerupContainer>(
            assetFilter: "t:PowerupContainer",
            importedAssets: importedAssets,
            deletedAssets: deletedAssets,
            changePredicate: path =>
                path.Contains("Powerups/") &&
                !path.Contains("Powerup Container"),
            refreshAction: container =>
                RefreshContainer<PowerupContainer, Powerup>(
                    container,
                    PowerupFolders,
                    "t:Powerup",
                    (c, list) => c.Powerups = list
                ));
        
        RefreshAll<EnemyContainer>(
            assetFilter: "t:EnemyContainer",
            importedAssets: importedAssets,
            deletedAssets: deletedAssets,
            changePredicate: path =>
                path.Contains("Enemies/") &&
                !path.Contains("Enemy Container"),
            refreshAction: container =>
                RefreshContainer<EnemyContainer, Enemy>(
                    container,
                    EnemyFolders,
                    "t:Enemy",
                    (c, list) => c.Enemies = list
                ));
    }

    private static void RefreshContainer<TContainer, TItem>(
        TContainer container,
        string[] folders,
        string searchFilter,
        System.Action<TContainer, List<TItem>> assignAction
    )
        where TContainer : UnityEngine.Object
        where TItem : UnityEngine.Object
    {
        var list = new List<TItem>();

        foreach (string guid in AssetDatabase.FindAssets(searchFilter, folders))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<TItem>(path);

            if (obj != null)
                list.Add(obj);
        }

        assignAction(container, list);

        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();

        Debug.Log($"Refreshed {container.name}: {list.Count} items");
    }

    private static void RefreshAll<T>(
        string assetFilter,
        string[] importedAssets,
        string[] deletedAssets,
        System.Predicate<string> changePredicate,
        System.Action<T> refreshAction
    ) where T : UnityEngine.Object
    {
        bool changed =
            System.Array.Exists(importedAssets, changePredicate) ||
            System.Array.Exists(deletedAssets, changePredicate);

        if (!changed) return;

        string[] guids = AssetDatabase.FindAssets(assetFilter);

        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset == null) continue;

                refreshAction(asset);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }
}