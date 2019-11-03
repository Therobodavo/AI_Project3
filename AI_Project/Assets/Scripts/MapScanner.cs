using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScanner : MonoBehaviour
{
    public static MapScanner instance;

    public float cellWidth;
    public float cellLength;
    public Vector3 startPoint;//The start point where the scanner start to scan the map
    public float mapWidth;//The width of the map
    public float mapLength;//The length of the map
    public float scanHeight;//The height from where the scanner will start to scan
    public float heightTolerance;//Tolerable height difference of character movement
    [HideInInspector]public Node[,] grid;//The information of the map
    [HideInInspector]public int xNumber;//The number of columns
    [HideInInspector]public int yNumber;//The number of rows

    public LayerMask scanLayers;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        ScanMap();
    }

    void ScanMap()
    {
        xNumber = (int)(mapWidth / cellWidth);
        yNumber = (int)(mapLength / cellLength);

        grid = new Node[xNumber, yNumber];
        for(int i = 0; i < xNumber; i++)
        {
            for(int j = 0; j < yNumber; j++)
            {
                grid[i, j] = new Node();
                grid[i, j].x = i;
                grid[i, j].y = j;
                Vector3 pos = new Vector3(startPoint.x + (i + 0.5f) * cellWidth, scanHeight, startPoint.z + (j + 0.5f) * cellLength);

                RaycastHit hit;
                if (Physics.Raycast(pos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, scanLayers))
                {
                    //Debug.Log(new Vector3(pos.x, pos.y - hit.distance, pos.z));
                    grid[i, j].position = new Vector3(pos.x, pos.y - hit.distance, pos.z);
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
                    {
                        //Debug.DrawLine(new Vector3(pos.x, pos.y - hit.distance + 4.0f, pos.z), new Vector3(pos.x, pos.y - hit.distance, pos.z), Color.red, 30.0f);
                        grid[i, j].isObstacle = true;
                    }
                    else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        grid[i, j].isObstacle = false;
                        //Debug.DrawLine(new Vector3(pos.x, pos.y - hit.distance + 0.5f, pos.z), new Vector3(pos.x, pos.y - hit.distance, pos.z), Color.yellow, 30.0f);
                    }
                }

            }
        }
        for (int i = 0; i < xNumber; i++)
        {
            for (int j = 0; j < yNumber; j++)
            {
                if(grid[i, j].isObstacle == false)
                {
                    grid[i, j].isObstacle = !CheckHeightTolerance(i, j);
                }
                if (grid[i, j].isObstacle == false)
                {
                    //Debug.DrawLine(new Vector3(grid[i, j].position.x, grid[i, j].position.y, grid[i, j].position.z), new Vector3(grid[i, j].position.x, grid[i, j].position.y + 0.5f, grid[i, j].position.z), Color.yellow, 30.0f);
                }
                else
                {
                    //Debug.DrawLine(new Vector3(grid[i, j].position.x, grid[i, j].position.y, grid[i, j].position.z), new Vector3(grid[i, j].position.x, grid[i, j].position.y + 4.0f, grid[i, j].position.z), Color.red, 30.0f);
                }
            }
        }
    }
    
    bool CheckHeightTolerance(int x, int y)
    {
        bool isWalkable = true;
        if (x - 1 >= 0 && y - 1 >= 0)
        {
            if(Mathf.Abs(grid[x, y].position.y - grid[x - 1, y - 1].position.y) > heightTolerance)
            {
                isWalkable = false;
            }
        }
        if (x - 1 >= 0 && y + 1 < yNumber)
        {
            if (Mathf.Abs(grid[x, y].position.y - grid[x - 1, y + 1].position.y) > heightTolerance)
            {
                isWalkable = false;
            }
        }
        if (x + 1 < xNumber && y - 1 >= 0)
        {
            if (Mathf.Abs(grid[x, y].position.y - grid[x + 1, y - 1].position.y) > heightTolerance)
            {
                isWalkable = false;
            }
        }
        if (x + 1 < xNumber && y + 1 < yNumber)
        {
            if (Mathf.Abs(grid[x, y].position.y - grid[x + 1, y + 1].position.y) > heightTolerance)
            {
                isWalkable = false;
            }
        }
        if(x - 1 >= 0)
        {
            if (Mathf.Abs(grid[x, y].position.y - grid[x - 1, y].position.y) > heightTolerance)
            {
                isWalkable = false;
            }
        }
        if (y - 1 >= 0)
        {
            if (Mathf.Abs(grid[x, y].position.y - grid[x, y - 1].position.y) > heightTolerance)
            {
                isWalkable = false;
            }
        }
        if (x + 1 < xNumber)
        {
            if (Mathf.Abs(grid[x, y].position.y - grid[x + 1, y].position.y) > heightTolerance)
            {
                isWalkable = false;
            }
        }
        if (y + 1 < yNumber)
        {
            if (Mathf.Abs(grid[x, y].position.y - grid[x, y + 1].position.y) > heightTolerance)
            {
                isWalkable = false;
            }
        }
        return isWalkable;
    }
}
