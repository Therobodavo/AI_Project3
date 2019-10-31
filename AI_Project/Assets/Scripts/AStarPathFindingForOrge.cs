using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AStarPathFindingForOrge : MonoBehaviour
{


    private List<Node> openList = new List<Node>();
    private List<Node> closeList = new List<Node>();
    private Stack<Node> path = new Stack<Node>();

    private Node[,] nodes;

    private List<Vector3> pathRecords = new List<Vector3>();
    private int targetPointIndex = -1;

    private Vector3 targetLocation;


    void Start()
    {

    }


    void Update()
    {
        //Check if reach next path point & Deal with movement
        if (targetPointIndex < pathRecords.Count && targetPointIndex >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(pathRecords[targetPointIndex].x, pathRecords[targetPointIndex].y + 1.0f, pathRecords[targetPointIndex].z), 10.0f * Time.deltaTime);
            transform.LookAt(new Vector3(pathRecords[targetPointIndex].x, pathRecords[targetPointIndex].y + 1.0f, pathRecords[targetPointIndex].z));
            if (Vector3.Distance(transform.position, new Vector3(pathRecords[targetPointIndex].x, pathRecords[targetPointIndex].y + 1.0f, pathRecords[targetPointIndex].z)) < 0.01f && targetPointIndex < pathRecords.Count)
            {
                //Debug.Log(targetPointIndex);
                targetPointIndex++;
                if (targetPointIndex == pathRecords.Count)
                {
                    targetPointIndex = -1;
                    //Code here if you want to do something when Orge finish movement
                    ReachTarget();
                }
            }
        }


    }

    //Will be called when the Orge reach its current pathfinding target
    public void ReachTarget()
    {

    }

    bool PathFinding(Vector3 startPoint, Vector3 endPoint)
    {
        int startX;
        int startY;
        int endX;
        int endY;
        CopyNodes();
        startX = (int)GetClosestNode(startPoint).x;
        startY = (int)GetClosestNode(startPoint).y;
        endX = (int)GetClosestNode(endPoint).x;
        endY = (int)GetClosestNode(endPoint).y;
        //Debug.Log(GetClosestNode(endPoint));
        if (endX < 0 || endY < 0 || endX > MapScanner.instance.xNumber || endY > MapScanner.instance.yNumber)
        {
            Debug.Log("Can't reach end.");
            return false;
        }
        else if (nodes[endX, endY].isObstacle)
        {
            Debug.Log("Can't reach end.");
            return false;
        }

        Debug.DrawLine(new Vector3(nodes[startX, startY].position.x, nodes[startX, startY].position.y, nodes[startX, startY].position.z), new Vector3(nodes[startX, startY].position.x, nodes[startX, startY].position.y + 0.5f, nodes[startX, startY].position.z), Color.yellow, 30.0f);
        Debug.DrawLine(new Vector3(nodes[endX, endY].position.x, nodes[endX, endY].position.y, nodes[endX, endY].position.z), new Vector3(nodes[endX, endY].position.x, nodes[endX, endY].position.y + 0.5f, nodes[endX, endY].position.z), Color.yellow, 30.0f);

        Node currentNode = null;
        openList.Clear();
        closeList.Clear();
        path.Clear();
        openList.Add(nodes[startX, startY]);

        while (openList.Count > 0)
        {
            currentNode = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f <= currentNode.f && openList[i].h < currentNode.h)
                {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            if (currentNode.x == endX && currentNode.y == endY)
            {
                if (openList.Count > 1)
                {
                    if (currentNode.parent != null)
                    {
                        if (IsInclude(currentNode.position.x, currentNode.parent.position.x, endPoint.x) || IsInclude(currentNode.position.z, currentNode.parent.position.z, endPoint.z))
                        {
                            currentNode = currentNode.parent;
                        }
                    }
                }
                if (openList.Count > 1)
                {
                    Node temp = FindStartPoint(currentNode);
                    if (temp.parent != null)
                    {
                        if (IsInclude(temp.position.x, temp.parent.position.x, startPoint.x) || IsInclude(temp.position.z, temp.parent.position.z, startPoint.z))
                        {
                            temp.parent = null;
                        }
                    }
                }
                GeneratePath(currentNode);
                return true;
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    int p = currentNode.x + i;
                    int q = currentNode.y + j;
                    if (p < 0 || q < 0 || p >= MapScanner.instance.xNumber || q >= MapScanner.instance.yNumber || nodes[p, q].isObstacle || closeList.Contains(nodes[p, q]))
                    {
                        continue;
                    }

                    int g = currentNode.g + (int)(Mathf.Sqrt(i * i + j * j) * 10);

                    if (nodes[p, q].g == 0 || nodes[p, q].g > g)
                    {
                        nodes[p, q].g = g;
                        nodes[p, q].parent = currentNode;
                        nodes[p, q].h = (Mathf.Abs(p - endX) + Mathf.Abs(q - endY)) * 10;
                        nodes[p, q].f = nodes[p, q].g + nodes[p, q].h;
                        openList.Add(nodes[p, q]);
                    }


                }
            }
            if (openList.Count == 0)
            {
                Debug.Log("Can't reach end.");
                return false;
            }
            openList.Sort();
        }
        return true;
    }

    void GeneratePath(Node node)
    {
        if (node.parent != null)
        {
            path.Push(node.parent);
            GeneratePath(node.parent);
        }
        else
        {
            RecordPathAsVector();
            return;
        }
    }

    void RecordPathAsVector()
    {
        pathRecords.Clear();
        while (path.Count > 0)
        {
            pathRecords.Add(path.Pop().position);
            //Debug.DrawLine(new Vector3(node.position.x, node.position.y, node.position.z), new Vector3(node.position.x, node.position.y + 2.0f, node.position.z), Color.green, 30.0f);
        }
        pathRecords.Add(targetLocation);
        targetPointIndex = 0;
        Debug.DrawLine(new Vector3(pathRecords[0].x, pathRecords[0].y, pathRecords[0].z), new Vector3(transform.position.x, transform.position.y, transform.position.z), Color.green, 30.0f);
        for (int i = 1; i < pathRecords.Count; i++)
        {
            //Debug.Log(pathRecords[i]);
            Debug.DrawLine(new Vector3(pathRecords[i].x, pathRecords[i].y, pathRecords[i].z), new Vector3(pathRecords[i - 1].x, pathRecords[i - 1].y, pathRecords[i - 1].z), Color.green, 30.0f);
        }
    }

    Vector2 GetClosestNode(Vector3 position)
    {
        float deltaWidth = (position.x - MapScanner.instance.startPoint.x) / MapScanner.instance.cellWidth;
        float deltaLength = (position.z - MapScanner.instance.startPoint.z) / MapScanner.instance.cellLength;
        int x, y;
        if (deltaWidth - (int)deltaWidth < 0.5f)
        {
            x = (int)deltaWidth - 1;
        }
        else
        {
            x = (int)deltaWidth;
        }
        if (deltaLength - (int)deltaLength < 0.5f)
        {
            y = (int)deltaLength - 1;
        }
        else
        {
            y = (int)deltaLength;
        }
        return new Vector2(x, y);
    }

    void CopyNodes()
    {
        nodes = new Node[MapScanner.instance.xNumber, MapScanner.instance.yNumber];
        for (int i = 0; i < MapScanner.instance.xNumber; i++)
        {
            for (int j = 0; j < MapScanner.instance.yNumber; j++)
            {
                nodes[i, j] = new Node();
                nodes[i, j].isObstacle = MapScanner.instance.grid[i, j].isObstacle;
                nodes[i, j].position = MapScanner.instance.grid[i, j].position;
                nodes[i, j].x = MapScanner.instance.grid[i, j].x;
                nodes[i, j].y = MapScanner.instance.grid[i, j].y;
            }
        }
    }

    //Set a ramdom reachable location in the scene and go there
    void SetRandomLocationAsTarget()
    {
        CopyNodes();
        Vector3 randomPoint = new Vector3(45, 6, 50);
        bool isOK = false;
        while (!isOK)
        {
            randomPoint = new Vector3((int)Random.Range(1, 49), 30, (int)Random.Range(1, 69));

            RaycastHit hit;
            if (Physics.Raycast(randomPoint, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, MapScanner.instance.scanLayers))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {

                    randomPoint = new Vector3(randomPoint.x, 30 - hit.distance, randomPoint.z);
                    if (!nodes[(int)GetClosestNode(randomPoint).x, (int)GetClosestNode(randomPoint).y].isObstacle)
                    {
                        isOK = true;
                    }
                }
            }
        }
        SetLocationAsTarget(randomPoint);
    }

    //Use this method to move the Orge, parameter is the target location
    //If AI can find a path, return true. If AI can't find a path, return false and the AI will do nothing
    public bool SetLocationAsTarget(Vector3 location)
    {
        bool canReach;
        pathRecords.Clear();
        targetPointIndex = -1;
        targetLocation = location;

        canReach = PathFinding(transform.position, location);
        if (pathRecords.Count > GetComponent<Orge>().speed + 2)
        {
            canReach = false;
            pathRecords.Clear();
        }
        return canReach;
    }



    bool IsInclude(float a, float b, float c)
    {
        float x, y;
        if (a < b)
        {
            x = a;
            y = b;
        }
        else
        {
            x = b;
            y = a;
        }
        if (x < c && c < y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Node FindStartPoint(Node node)
    {
        Node temp = node;
        if (temp.parent == null || temp.parent.parent == null)
        {
            return temp;
        }
        while (temp.parent.parent != null)
        {
            temp = temp.parent;

        }
        return temp;
    }
}
