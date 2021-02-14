using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGrid : MonoBehaviour
{
    [SerializeField] bool displayGrid;
    Node[,] grid;
    [SerializeField] Vector2 gridWorldSize;
    int gridSizeX, gridSizeY; // the # nodes along the X and Y axis of the grid
    [SerializeField] float nodeRadius;
    float nodeDiameter;

    [System.Serializable]
    class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
    [SerializeField] TerrainType[] walkableRegions;
    [SerializeField] LayerMask unwalkableLayerMask;
    LayerMask walkableLayerMask;
    Dictionary<int, int> walkableRegionsDict = new Dictionary<int, int>();

    [SerializeField] Transform player;

    void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        nodeDiameter = 2 * nodeRadius;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft =
            transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        foreach (var region in walkableRegions)
        {
            walkableLayerMask.value |= region.terrainMask.value;
            walkableRegionsDict.Add((int)Mathf.Log(region.terrainMask.value, 2f), region.terrainPenalty);
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint =
                    worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableLayerMask);

                int penalty = 0;
                if (walkable)
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, walkableLayerMask))
                    {
                        walkableRegionsDict.TryGetValue(hit.collider.gameObject.layer, out penalty);
                    }
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, penalty);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGrid)
        {
            Node playerNode = NodeFromWorldPoint(player.position);
            foreach (var n in grid)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(0f, 10f, n.MovementPenalty));
                Gizmos.color = n.Walkable ? Gizmos.color : Color.red;
                if (playerNode == n)
                {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawCube(n.WorldPos, Vector3.one * nodeDiameter);
            }
        }
    }

    private Node NodeFromWorldPoint(Vector3 position)
    {
        float percentX = Mathf.Clamp01((position.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((position.z + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.FloorToInt(Mathf.Min(percentX * gridSizeX, gridSizeX - 1));
        int y = Mathf.FloorToInt(Mathf.Min(percentY * gridSizeY, gridSizeY - 1));

        return grid[x, y];
    }
}
