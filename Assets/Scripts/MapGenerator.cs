using System;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;

    [SerializeField] string seed;
    [SerializeField] bool useRandomSeed;

    [Range(0, 100)]
    [SerializeField] int randomFillPercent;

    [SerializeField] int smoothIterations = 5;

    int[,] map;

    void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        map = new int[width, height];

        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random random = new System.Random(seed.GetHashCode());

        foreach (int x in Enumerable.Range(0, width))
        {
            foreach (int y in Enumerable.Range(0, height))
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = random.Next(0, 100) > randomFillPercent ? 0 : 1;
                }
            }
        }

        foreach (int i in Enumerable.Range(0, smoothIterations))
        {
            SmoothMap();
        }
    }

    void SmoothMap()
    {
        foreach (int x in Enumerable.Range(0, width))
        {
            foreach (int y in Enumerable.Range(0, height))
            {
                int neighborWallCount = GetNeighborWallCount(x, y);

                if (neighborWallCount > 4)
                {
                    map[x, y] = 1;
                }
                else if (neighborWallCount < 4)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetNeighborWallCount(int x, int y)
    {
        int wallCount = 0;
        foreach (int neighborX in Enumerable.Range(x - 1, 3))
        {
            foreach (int neighborY in Enumerable.Range(y - 1, 3))
            {
                bool isInMap = neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height;
                bool isSelf = neighborX == x && neighborY == y;

                if (isInMap && !isSelf)
                {
                    wallCount += map[neighborX, neighborY];
                }
                else if (!isInMap)
                {
                    wallCount += 1;
                }
            }
        }

        return wallCount;
    }

    void OnDrawGizmos()
    {
        if (map == null)
        {
            return;
        }

        foreach (int x in Enumerable.Range(0, width))
        {
            foreach (int y in Enumerable.Range(0, height))
            {
                Gizmos.color = map[x, y] == 1 ? Color.black : Color.white;
                Vector3 position = new Vector3(-width / 2 + x, 0, -height / 2 + y);
                Gizmos.DrawCube(position, Vector3.one);
            }
        }
    }
}