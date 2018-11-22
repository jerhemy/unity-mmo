using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class Grid : MonoBehaviour {
 
    //Variables
    public Node[,] grid;
 
    private int gridSizeX;
    private int gridSizeZ;
 
    //Parametre
    public Vector3 gridSize = new Vector3(10f,10f,10f);
    public float cellSize = 1f;
    public float stepOffset = 1f;
    public LayerMask layersToIgnore;
    // Use this for initialization
    void Start ()
    {
        gridSizeX = Mathf.RoundToInt(gridSize.x/cellSize);
        gridSizeZ = Mathf.RoundToInt(gridSize.z/cellSize);
 
 
    }
 
    // Update is called once per frame
    void Update ()
    {
        CreateGrid();
    }
 
    public void CreateGrid()
    {
        grid = new Node[gridSizeX,gridSizeZ];
        Vector3 gridBottomLeft = transform.position - Vector3.right * gridSize.x/2 - Vector3.forward * gridSize.z/2;
 
        //Create node
        for(int x = 0; x < gridSizeX; x++)
            for(int z = 0; z < gridSizeZ; z++)
            {
 
                Vector3 worldPoint = gridBottomLeft + Vector3.right * (x * cellSize + cellSize/2)
                    + Vector3.forward * (z * cellSize + cellSize/2);
                grid[x,z] = new Node(worldPoint,true);
            }
 
        //Raycast terrain
        foreach(Node n in grid)
        {
            Ray ray = new Ray(n.worldPosition + new Vector3(0,gridSize.y,0),Vector3.down);
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(ray,out hit,gridSize.y*2,layersToIgnore))
            {
                n.worldPosition = hit.point + new Vector3(0,0.1f,0);
            }
        }
 
        //Create connection
        for(int x = 0; x < gridSizeX ; x++)
            for(int z = 0; z < gridSizeZ; z++)
            {
 
                if(x+1 < gridSizeX)
                    grid[x,z].connections[0] = new Connection(grid[x,z].worldPosition,grid[x+1,z].worldPosition,true);
                if(z+1 < gridSizeZ)
                    grid[x,z].connections[1] = new Connection(grid[x,z].worldPosition,grid[x,z+1].worldPosition,true);
                if(x+1 < gridSizeX && z+1 < gridSizeZ)
                    grid[x,z].connections[2] = new Connection(grid[x,z].worldPosition,grid[x+1,z+1].worldPosition,true);
                if(x+1 < gridSizeX && z-1 >= 0)
                    grid[x,z].connections[3] = new Connection(grid[x,z].worldPosition,grid[x+1,z-1].worldPosition,true);
            }
 
 
 
        //Remove invalid Connection
        foreach(Node n in grid)
            foreach(Connection c in n.connections)
            {
                if(c != null)
                {
                    if(Mathf.Abs(c.start.y - c.end.y) > stepOffset)
                    {
                        c.valid = false;
                    }
 
                }
 
            }
 
 
    }
 
 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position,gridSize);
 
 
 
        if(grid != null)
        {
            foreach(Node n in grid)
            {
                Gizmos.color = (n.open)?Color.yellow:Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one  * cellSize/8f);
 
                foreach(Connection c in n.connections)
                {
                    if(c != null && c.valid)
                    {
                        Gizmos.color = (c.open)?Color.white:Color.red;
                        Gizmos.DrawLine(c.start,c.end);
                    }
                }
 
            }
        }
    }
}