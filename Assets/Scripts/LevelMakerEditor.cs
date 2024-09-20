using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using static UnityEditor.PlayerSettings;
using UnityEngine.Experimental.GlobalIllumination;

public class LevelMakerEditor : EditorWindow
{
    int height = 6;
    int width = 4;
    int levelNumber = 1;
    string levelFileName = "level_1.json";
    List<GridItemType> grid;
    List<CupType> listCupType = new List<CupType>();
    CupType selectedCupType = CupType.None;

    [MenuItem("Window/Level Maker")]
    public static void ShowWindow()
    {
        GetWindow<LevelMakerEditor>("Level Maker");
    }

    void OnEnable()
    {
        LoadLevel();
    }

    void OnGUI()
    {
        GUILayout.Label("Level Editor", EditorStyles.boldLabel);
        GUILayout.Space(20);
        levelNumber = EditorGUILayout.IntField("Level Number", levelNumber);
        if (GUILayout.Button("Save Level", GUILayout.Width(80), GUILayout.Height(30)))
        {
            SaveLevel();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Load Level", GUILayout.Width(80), GUILayout.Height(30)))
        {
            LoadLevel();
        }
        GUILayout.Space(10);
        width = EditorGUILayout.IntField("Width", width);
        GUILayout.Space(5);
        height = EditorGUILayout.IntField("Height", height);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Cup Type ", EditorStyles.label, new GUILayoutOption[] { GUILayout.Width(50) });
        CupType cupType = selectedCupType;
        selectedCupType = (CupType)EditorGUILayout.EnumPopup(selectedCupType, GUILayout.Width(93));
        if (GUILayout.Button("Add", GUILayout.Width(50), GUILayout.Height(20)))
        {
            listCupType.Add(selectedCupType);
        }
        GUILayout.Label("Added Cup Types", EditorStyles.boldLabel);
        foreach (var type in listCupType)
        {
            GUILayout.Label(type.ToString());
        }
        GUILayout.EndHorizontal();

        levelFileName = $"{levelNumber}.json";

        // Initialize grid when size changes
        if (grid == null || grid.Count != width * height)
        {
            grid = Enumerable.Repeat(GridItemType.Empty, width * height).ToList();
        }
        GUILayout.Space(20);
        DrawGrid();
    }

    private void DrawGrid()
    {
        GUILayout.Label("Blocks:");
        int index = 0;
        
        for (int i = 0; i < width; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < height; j++)
            {
                Texture2D texture = GetBlockTexture(grid[index]);

                if (GUILayout.Button(new GUIContent(texture), GUILayout.Width(50), GUILayout.Height(50)))
                {
                    grid[index] = grid[index] == GridItemType.Box ? GridItemType.Empty : GridItemType.Box;

                    Repaint();
                    Debug.Log($"Clicked on Grid ({i},{j}), new type: {grid[index]}, index: {index}");
                }
                index++; 
            }
            GUILayout.EndHorizontal();
        }
    }

    private void SaveLevel()
    {
        LevelData levelData = new LevelData
        {
            height = height,
            width = width,
            gridItemData = grid,
            levelNumber = levelNumber,
            requiredCupTypes = listCupType
        };

        string json = JsonUtility.ToJson(levelData);
        System.IO.File.WriteAllText("Assets/Levels/level_" + levelFileName, json);
        Debug.Log("Level saved to " + levelFileName);
    }

    private void LoadLevel()
    {
        string path = "Assets/Levels/level_" + levelFileName;
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            height = levelData.height;
            width = levelData.width;
            grid = levelData.gridItemData;
            levelNumber = levelData.levelNumber;
            listCupType = levelData.requiredCupTypes;

            Repaint();
            Debug.Log("Level loaded from " + levelFileName);
        }
        else
        {
            Debug.LogWarning("Level file does not exist: " + levelFileName);
        }
    }

    Texture2D GetBlockTexture(GridItemType gridType)
    {
        switch (gridType)
        {
            case GridItemType.Empty: return Resources.Load<Texture2D>("Textures/Empty");
            case GridItemType.Box: return Resources.Load<Texture2D>("Textures/Box");
            default: return null;
        }
    }
    
}

[System.Serializable]
public class LevelData
{
    public int height;
    public int width;
    public List<GridItemType> gridItemData;
    public List<CupType> requiredCupTypes;
    public int levelNumber;
}

public enum GridItemType
{
    Empty,
    Box
}
