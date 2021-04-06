using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;

    List<Vector3> vertices;
    List<int> triangles;

    public void GenerateMesh(int[,] map, float squareSize)
    {
        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        foreach (int x in Enumerable.Range(0, squareGrid.squares.GetLength(0)))
        {
            foreach (int y in Enumerable.Range(0, squareGrid.squares.GetLength(1)))
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
    }

    void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0:
                break;

            // 1 Point
            case 1:
                MeshFromPoints(square.bottom, square.bottomLeft, square.left);
                break;
            case 2:
                MeshFromPoints(square.right, square.bottomRight, square.bottom);
                break;
            case 4:
                MeshFromPoints(square.top, square.topRight, square.right);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.top, square.left);
                break;

            // 2 Point
            case 3:
                MeshFromPoints(square.right, square.bottomRight, square.bottomLeft, square.left);
                break;
            case 6:
                MeshFromPoints(square.top, square.topRight, square.bottomRight, square.bottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.top, square.bottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.right, square.left);
                break;
            case 5:
                MeshFromPoints(square.top, square.topRight, square.right, square.bottom, square.bottomLeft, square.left);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.top, square.right, square.bottomRight, square.bottom, square.left);
                break;

            // 3 Point
            case 7:
                MeshFromPoints(square.top, square.topRight, square.bottomRight, square.bottomLeft, square.left);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.top, square.right, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.right, square.bottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottom, square.left);
                break;

            // 4 Point
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
        }
    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3)
        {
            CreateTriangle(points[0], points[1], points[2]);
        }

        if (points.Length >= 4)
        {
            CreateTriangle(points[0], points[2], points[3]);
        }

        if (points.Length >= 5)
        {
            CreateTriangle(points[0], points[3], points[4]);
        }

        if (points.Length >= 6)
        {
            CreateTriangle(points[0], points[4], points[5]);
        }
    }

    void AssignVertices(Node[] points)
    {
        foreach (Node point in points)
        {
            if (point.vertexIndex == -1)
            {
                point.vertexIndex = vertices.Count;
                vertices.Add(point.position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);
    }

    void OnDrawGizmosSelected()
    {
        /**
        if (squareGrid == null)
        {
            return;
        }

        foreach (int x in Enumerable.Range(0, squareGrid.squares.GetLength(0)))
        {
            foreach (int y in Enumerable.Range(0, squareGrid.squares.GetLength(1)))
            {
                ControlNode node = this.squareGrid.squares[x, y].topLeft;
                Gizmos.color = node.active ? Color.black : Color.white;
                Gizmos.DrawCube(node.position, Vector3.one * 0.4f);

                node = this.squareGrid.squares[x, y].topRight;
                Gizmos.color = node.active ? Color.black : Color.white;
                Gizmos.DrawCube(node.position, Vector3.one * 0.4f);

                node = this.squareGrid.squares[x, y].bottomLeft;
                Gizmos.color = node.active ? Color.black : Color.white;
                Gizmos.DrawCube(node.position, Vector3.one * 0.4f);

                node = this.squareGrid.squares[x, y].bottomRight;
                Gizmos.color = node.active ? Color.black : Color.white;
                Gizmos.DrawCube(node.position, Vector3.one * 0.4f);

                Gizmos.color = Color.gray;

                Gizmos.DrawCube(this.squareGrid.squares[x, y].top.position, Vector3.one * 0.1f);
                Gizmos.DrawCube(this.squareGrid.squares[x, y].right.position, Vector3.one * 0.1f);
                Gizmos.DrawCube(this.squareGrid.squares[x, y].bottom.position, Vector3.one * 0.1f);
                Gizmos.DrawCube(this.squareGrid.squares[x, y].left.position, Vector3.one * 0.1f);
            }
        }
        **/
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);

            float mapWorldWidth = mapWidth * squareSize;
            float mapWorldHeight = mapHeight * squareSize;

            ControlNode[,] controlNodes = new ControlNode[mapWidth, mapHeight];
            foreach (int x in Enumerable.Range(0, mapWidth))
            {
                foreach (int y in Enumerable.Range(0, mapHeight))
                {
                    Vector3 position = new Vector3(
                        -mapWorldWidth / 2 + x * squareSize + squareSize / 2,
                        0,
                        -mapWorldHeight / 2 + y * squareSize + squareSize / 2);
                    bool active = map[x, y] == 1;

                    controlNodes[x, y] = new ControlNode(position, active, squareSize);
                }
            }

            this.squares = new Square[mapWidth - 1, mapHeight - 1];
            foreach (int x in Enumerable.Range(0, mapWidth - 1))
            {
                foreach (int y in Enumerable.Range(0, mapHeight - 1))
                {
                    this.squares[x, y] = new Square(
                        controlNodes[x, y + 1],
                        controlNodes[x + 1, y + 1],
                        controlNodes[x + 1, y],
                        controlNodes[x, y]);
                }
            }
        }
    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomLeft, bottomRight;
        public Node left, top, right, bottom;
        public int configuration;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;

            this.left = bottomLeft.above;
            this.top = topLeft.right;
            this.right = bottomRight.above;
            this.bottom = bottomLeft.right;

            if (topLeft.active)
            {
                configuration += 8;
            }
            if (topRight.active)
            {
                configuration += 4;
            }
            if (bottomRight.active)
            {
                configuration += 2;
            }
            if (bottomLeft.active)
            {
                configuration += 1;
            }
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex;

        public Node(Vector3 position)
        {
            this.position = position;
            this.vertexIndex = -1;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 position, bool active, float squareSize) : base(position)
        {
            this.active = active;
            this.above = new Node(position + Vector3.forward * squareSize / 2f);
            this.right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }

}
