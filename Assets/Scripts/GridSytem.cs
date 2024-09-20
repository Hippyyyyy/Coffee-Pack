using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridSytem : MonoBehaviour
{
    [SerializeField] int height;
    [SerializeField] int width;
    [SerializeField] float offset;
    [SerializeField] List<Node> nodes;
    [SerializeField] GridItem gridItemPrefab;
    [SerializeField] List<GridItem> gridItems;
    List<GridItemType> gridItemTypes = new List<GridItemType>();
    List<CupType> requiredCupTypes;
    //[SerializeField] TraySO traySO;
    [SerializeField] Transform trayGroup;
    [SerializeField] Tray trayPrefab;
    [SerializeField] List<Vector3> listPositionTray;
    [SerializeField] List<Tray> trays;

    private void Awake()
    {
        nodes = new List<Node>();
        SetUpPositionCup();
        string filePath = "Assets/Levels/level_1.json";
        LoadLevelData(filePath);
    }

    private void Start()
    {
        InitGridSystem();
        SpawnGrid();
        FindNodeNeighbors();
        FindGridNeighbors();
        SpawnTray();
    }

    public void LoadLevelData(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            width = levelData.width;
            height = levelData.height;
            gridItemTypes = levelData.gridItemData;
            requiredCupTypes = levelData.requiredCupTypes;
        }
        else
        {
            Debug.LogWarning("Level file does not exist: " + filePath);
        }
    }

    public void InitGridSystem()
    {
        int a = 0;
        if (nodes.Count > 0) return;

        float centerX = (width - 1) / 2f;
        float centerZ = (height - 1) / 2f;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Node node = new Node();
                node.Id = a++;
                node.X = i;
                node.Z = j;

                node.PosX = (i - centerX) * offset;
                node.PosZ = (j - centerZ) * offset / 1.44f;

                nodes.Add(node);
            }
        }
    }
    public void SpawnGrid()
    {
        if (gridItems.Count > 0) return;
        Vector3 pos = Vector3.zero;
        foreach (Node node in nodes)
        {
            pos.x = node.PosX;
            pos.y = 0.5f;
            pos.z = node.PosZ;

            var gridItem = Instantiate(gridItemPrefab);
            gridItem.transform.SetParent(transform);
            gridItem.transform.localPosition = pos;
            gridItems.Add(gridItem);
            gridItem.name = "Grid Item " + node.Id;
            node.GridItem = gridItem;
            gridItem.Node = node;
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            node.GridType = gridItemTypes[i];
            node.GridItem.ActiveBox(node.GridType);
        }
    }
    public Node FindNearestNode(Vector3 position)
    {
        Node nearestNode = null;

        Vector3 boxSize = new Vector3(0.5f, 0.5f, 0.5f);

        Collider[] hitColliders = Physics.OverlapBox(position, boxSize);

        foreach (Collider collider in hitColliders)
        {
            GridItem item = collider.GetComponent<GridItem>();
            if (item != null)
            {
                nearestNode = item.Node;
                break;
            }
        }

        return nearestNode;
    }
    void FindNodeNeighbors()
    {

        int x;
        int z;
        foreach (Node node in nodes)
        {
            x = node.X;
            z = node.Z;
            node.Up = GetNodeForXY(x, z + 1);
            node.Down = GetNodeForXY(x, z - 1);
            node.Right = GetNodeForXY(x + 1, z);
            node.Left = GetNodeForXY(x - 1, z);
        }
    }
    void FindGridNeighbors()
    {
        foreach (Node node in nodes)
        {
            if (!node.GridItem) continue;
            node.GridItem.Neighbors.Add(node.Up?.GridItem);
            node.GridItem.Neighbors.Add(node.Down?.GridItem);
            node.GridItem.Neighbors.Add(node.Right?.GridItem);
            node.GridItem.Neighbors.Add(node.Left?.GridItem);
        }
    }
    Node GetNodeForXY(int x, int z)
    {
        foreach (Node node in nodes)
        {
            if (node.X == x && node.Z == z)
            {
                return node;
            }
        }

        return null;
    }

    public List<CupType> GetRequiredCupTypes()
    {
        return requiredCupTypes;
    }

    void SetUpPositionCup()
    {
        listPositionTray = new List<Vector3>();
        listPositionTray.Add(new Vector3(-2.2f, 37.4f, -36.6f));
        listPositionTray.Add(new Vector3(0.2f, 37.4f, -36.6f));
        listPositionTray.Add(new Vector3(2.6f, 37.4f, -36.6f));
    }

    public void RemoveTray(Tray tray)
    {
        trays.Remove(tray);
        if (trays.Count == 0)
        {
            SpawnTray();
        }
    }

    void SpawnTray()
    {
        float delayTime = 0;
        for (int i = 0; i < 3; i++)
        {
            var tray = Instantiate(trayPrefab, trayGroup);
            var pos = listPositionTray[i];
            tray.transform.DOLocalMove(pos, 0.7f).SetDelay(delayTime).SetEase(Ease.OutBack);
            tray.SetDefaultPos(pos);
            trays.Add(tray);
        }
    }

    public void CompleteGame()
    {
        float delayTime = 0;
        for (int i = 0; i < gridItems.Count; i++)
        {
            var grid = gridItems[i];
            DOVirtual.DelayedCall(delayTime, () =>
            {
                grid.PlayParticleBox();
                grid?.Tray.gameObject.SetActive(false);
                grid.Tray = null;
                grid.Node.Tray = null;
            });
            delayTime += 0.06f;
        }
    }
}
