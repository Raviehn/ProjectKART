using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public float threshold;
    public int stepSize;

    public Color road;
    public Color offroad;
    public Color impassable;

    int stepsX;
    int stepsZ;

    int roadWeight = 3;
    int curbWeight = 4;
    int offroadWeight = 8;

    Color colorGUI;
    Vector3 positionGUI;

    Texture2D courseTexture;

    List<Collider> obstacles;

    Node[,] grid;

    void Awake()
    {        
        courseTexture = GameObject.FindGameObjectWithTag("Track").GetComponentInChildren<SpriteRenderer>().sprite.texture;

        obstacles = new List<Collider>();
        CreateGrid();
        AdvanceGrid();
    }

    public int MaxSize
    {
        get
        {
            return stepsX * stepsZ;
        }
    }

    void CreateGrid()
    {
        Color courseColour;
        Vector3 texturePosition;
        Vector3 worldPosition;
        stepsX = courseTexture.width / stepSize;
        stepsZ = courseTexture.height / stepSize;

        grid = new Node[stepsX, stepsZ];

        AddObstaclesToList("Tube");

        for (int x = 0; x < stepsX; x++)
            for (int z = 0; z < stepsZ; z++)
            {
                // need to add step / 2 because of the translation of the plane. it doesn't start at 0,0
                texturePosition = new Vector3(x * stepSize + stepSize / 2, 0, z * stepSize + stepSize / 2);
                worldPosition = new Vector3(texturePosition.x / 25f - courseTexture.width / 50f, 0, texturePosition.z / 25f - courseTexture.height / 50f);

                courseColour  = courseTexture.GetPixel((int)texturePosition.x, (int)texturePosition.z);

                if (IsColourSimilar(courseColour, road) || IsColourSimilar(courseColour, Color.white) || IsColourSimilar(courseColour, Color.black))
                    grid[x, z] = IsBorder((int)texturePosition.x, (int)texturePosition.z) ? new Node(worldPosition, false, 0, x, z) : new Node(worldPosition, true, roadWeight, x, z);
                else if (IsColourSimilar(courseColour, offroad))
                    grid[x, z] = new Node(worldPosition, true, offroadWeight, x, z);
                else
                    grid[x, z] = new Node(worldPosition, false, 0, x, z);
            }
    }

    void AdvanceGrid()
    {
        for (int x = 2; x < stepsX - 2; x++)
            for (int y = 2; y < stepsZ - 2; y++)
                if (grid[x, y].weight == roadWeight)
                {
                    bool isBorder = false;

                    for (int xc = -2; xc <= 2; xc++)
                        for (int yc = -2; yc <= 2; yc++)
                            if (grid[x + xc, y + yc].weight == offroadWeight || !grid[x + xc, y + yc].free)
                                isBorder = true;

                    if (isBorder)
                        grid[x, y].weight = curbWeight;
                }
    }

    bool IsColourSimilar(Color colourA, Color colourB)
    {
        bool similar = true;

        if (colourA.r < colourB.r - threshold || colourA.r > colourB.r + threshold)
            similar = false;
        else if (colourA.g < colourB.g - threshold || colourA.g > colourB.g + threshold)
            similar = false;
        else if (colourA.b < colourB.b - threshold || colourA.b > colourB.b + threshold)
            similar = false;

        return similar;
    }

    bool IsBorder(int x, int y)
    {
        bool border = false;

        for (int xKernel = -1; xKernel <= 1; xKernel++)
            for (int yKernel = -1; yKernel <= 1; yKernel++)
            {
                Color courseColour = courseTexture.GetPixel(x + xKernel, y + yKernel);
                if (IsColourSimilar(courseColour, impassable))
                    border = true;
            }

        return border;
    }

    public Node GetNodeFromWorldPos(Vector3 worldPos)
    {
        int x = 0;// Mathf.RoundToInt((trackCamera.orthographicSize + worldPos.x) * stepsX / trackCamera.orthographicSize / 2);
        int z = 0;// Mathf.RoundToInt((trackCamera.orthographicSize + worldPos.z) * stepsZ / trackCamera.orthographicSize / 2);

        if (x < 0 || z < 0 || x >= stepsX || z >= stepsZ)
            return grid[0, 0];
        else
            return grid[x, z];
    }

    void AddObstaclesToList(string obstacle)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(obstacle);

        foreach (GameObject go in objects)
            obstacles.Add(go.GetComponent<Collider>());
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkZ = node.gridZ + z;

                if (checkX >= 0 && checkX < stepsX && checkZ >= 0 && checkZ < stepsZ)
                {
                    neighbours.Add(grid[checkX, checkZ]);
                }
            }
        }

        return neighbours;
    }

    void OnGUI()
    {
        //GUI.DrawTexture(new Rect(250, 110, 128, 128), rt, ScaleMode.ScaleToFit, true, 1.0F);
        //GUI.Label(new Rect(10, 70, 1000, 20), colorGUI.ToString());
        //GUI.Label(new Rect(10, 90, 1000, 20), rt.GetPixel(0, 0).ToString());
    }

    // draw grid
    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (n.weight > 0 && n.weight < 9)
                {
                    if (n.weight == roadWeight)
                        Gizmos.color = Color.white;
                    else if (n.weight == curbWeight)
                        Gizmos.color = Color.grey;
                    else if (n.weight > curbWeight)
                        Gizmos.color = Color.red;

                    Gizmos.DrawCube(n.worldPos, Vector3.one * .1f);
                }
            }
        }
    }
}
