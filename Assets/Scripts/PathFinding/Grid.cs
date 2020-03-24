using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinder
{
    public class Grid : MonoBehaviour
    {

        public LayerMask UnwalkableMask;
        public LayerMask JumpMask;
        Node[,] GridArray;

        int GridSizeX, GridSizeY;
        Vector2 BoxPosition;
        public float CellSize;

        List<Node> Ledges = new List<Node>();
        List<Node> Corners = new List<Node>();

        float JumpDistance = 4;
        float BoxWidth, BoxHeight;
        List<LedgePoint> LedgePoints = new List<LedgePoint>();
        [SerializeField] GameObject LedgePlateform;
        GameObject AreaBox;
        private void Start()
        {
            DefineBox();
            CreateGrid();
        }

        void DefineBox()
        {

            AreaBox = GameObject.Find("Background");
            BoxWidth = AreaBox.GetComponent<SpriteRenderer>().bounds.size.x;
            BoxHeight = AreaBox.GetComponent<SpriteRenderer>().bounds.size.y;
            float boxX = AreaBox.GetComponent<SpriteRenderer>().bounds.center.x - (AreaBox.GetComponent<SpriteRenderer>().bounds.size.x / 2);
            float boxY = AreaBox.GetComponent<SpriteRenderer>().bounds.center.y + (AreaBox.GetComponent<SpriteRenderer>().bounds.size.y / 2);
            BoxPosition = new Vector2(boxX, boxY);
        }

        // Construct Grid
        void CreateGrid()
        {
            GridSizeX = Mathf.RoundToInt(BoxWidth / CellSize);
            GridSizeY = Mathf.RoundToInt(BoxHeight / CellSize);
            GridArray = new Node[GridSizeX, GridSizeY];

            //
            // Start by creating nodes
            //
            CreateNodes();

            //
            // Then get ledges and corners nodes in the grid 
            //
            AddLedgesAndCorners();

            //
            // For each ledge, add a link to all ledge neighbors
            //
            AddWalkLinks();

            // Find ledge corner fall points
            // Raycast at specific angle to check fall point
            // If valid create a one way link
            AddRunOffAndFallLinksToCorners();

            // Get jumpoints of each leadgePoints
            // Check if there are jump points
            // Loop on jumpPoints
            // Raycast at specific angle to check jump point
            // If valid create jump link

            AddJumpLinks();


            // Cleanup leftover ledge corner colliders
            ClearUpLedgePoint();

        }

        void ClearUpLedgePoint()
        {
            foreach ( LedgePoint ledgePoint in LedgePoints)
            {
                // Destroy useless ledgePoint gameobject
                Destroy(ledgePoint.gameObject);
            }
        }
        void SetJumpDistance(float jumpDistance)
        {
            this.JumpDistance = jumpDistance;
        }

        public bool OutOfBounds(int x, int y)
        {
            return x < 0 || x >= GetWidth() ||
                y < 0 || y >= GetHeight();
        }

        public bool Blocked(int x, int y)
        {
            if (OutOfBounds(x, y)) return true;
            if (!GridArray[x,y].IsWalkable) return true;

            return false;
        }

        private void CreateNodes()
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    Vector3 nodePosition = new Vector3((x * CellSize) + BoxPosition.x + (CellSize / 2), -(y * CellSize) + BoxPosition.y + (CellSize / 2), 0);

                    // Check if there are obstacles
                    bool walkable = IsNodeWalkable(nodePosition);

                    // Create new node
                    GridArray[x, y] = new Node(walkable, nodePosition, x, y);
                }
            }
        }

        private bool IsNodeWalkable(Vector3 position)
        {
            // Get top left position of node
            Vector2 topLeft = new Vector2(position.x - (CellSize / 2), position.y - (CellSize / 2));

            // Get bottom right position of node
            Vector2 bottomRight = new Vector2(position.x + (CellSize / 2), position.y + (CellSize / 2));

            // Check if an unwalkable surface stands between these two points
            return !(Physics2D.OverlapArea(topLeft, bottomRight, UnwalkableMask));
        } 

    
        private void AddLedgesAndCorners()
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    Node tile = GetGridObject(x, y);
                    if (tile.IsWalkable && Blocked(x, y + 1) && y + 1 < GridSizeY)
                    {   // If the bottom ledge is blocked or bottom of y axis
                        tile.Ledge = true;
                        Ledges.Add(tile);
                        if (!Blocked(x - 1, y + 1) || !Blocked(x + 1, y + 1))
                            Corners.Add(tile);
                    }
                }
            }
        }

        private void AddWalkLinks()
        {
            foreach (Node ledge in Ledges)
            {
                int x = ledge.GridX;
                int y = ledge.GridY;

                // Check left and right neighbor
                for (int index = -1; index <= 1; index++)
                {

                    // Check if we are looking current cell
                    if (index == 0) { continue; /*If yes jump to next cell*/ }
                    if (!Blocked(x + index, y))
                    {
                        Node node = GetGridObject(x + index, y);
                        // if neighbour node is a ledge, add a walk link between ledge and neighbour node
                        if (node.Ledge) ledge.AddLink(node, 1);

                    }

                }
            }

        }

        private void AddRunOffAndFallLinksToCorners()
        {
            foreach (Node corner in Corners)
            {
                // Discover the direction the tile is facing
                int direction = Blocked(corner.GridX - 1, corner.GridY + 1) ? 1 : -1;

                // Step over the facing direction 1 tile
                Node overhang = GetGridObject(corner.GridX + direction, corner.GridY);

                // Shoot a raycast straight down to the end of the boundary to look for a fall
                RaycastHit2D hit = Physics2D.Raycast(
                    overhang.Position,
                    -Vector2.up,
                    (GetHeight() - overhang.GridY) * CellSize,
                    UnwalkableMask);

                // If we hit something add a fall link at the hit target
                if (hit.collider)
                {
                    Node tileTarget = NodeFromWorldPoint(hit.point);
                    int distance = (int)Mathf.Floor(Vector3.Distance(corner.Position, tileTarget.Position));
                    corner.AddLink(tileTarget, distance, PathLinkType.fall);
                }

                // Find corner runoff point
                hit = Physics2D.Raycast(
                    overhang.Position,
                    new Vector2(0.2f * direction, -0.5f),
                    (GetHeight() - overhang.GridY) * CellSize,
                    UnwalkableMask
                    );

                // If valid create a 2 way link
                if (hit.collider)
                {
                    Node tileTarget = NodeFromWorldPoint(hit.point);
                    int distance = (int)Mathf.Floor(Vector3.Distance(corner.Position, tileTarget.Position));
                    corner.AddLink(tileTarget, distance, PathLinkType.runoff);
                    tileTarget.AddLink(corner, distance, PathLinkType.runoff);
                }



                // Creata a ledge point for evaluating corner jumps

                GameObject pointObj = (GameObject)Instantiate(LedgePlateform);
                LedgePoint point = pointObj.GetComponent<LedgePoint>();
                pointObj.transform.position = GetGridObject(corner.GridX, corner.GridY).Position;
                LedgePoints.Add(point);
                point.X = corner.GridX;
                point.Y = corner.GridY;
                point.Direction = direction;
                point.Position = GetGridObject(corner.GridX, corner.GridY).Position;
                LedgePoints.Add(point);

            }
        }

        private void AddJumpLinks() {
            // Loop through all ledge corners
            foreach (LedgePoint ledgePoint in LedgePoints)
            {
                // Get ledge Point corresponding node
                Node node = GetGridObject(ledgePoint.X, ledgePoint.Y);

                RaycastHit2D[] jumpPoints = Physics2D.BoxCastAll(
                    ledgePoint.Position,
                    new Vector2(1, JumpDistance),
                    0,
                    new Vector2(JumpDistance * ledgePoint.Direction, 0),
                    JumpDistance,
                    JumpMask);

                foreach (RaycastHit2D jumpPoint in jumpPoints)
                {
                    int distance = (int)Mathf.Floor(Vector3.Distance(ledgePoint.Position, jumpPoint.transform.position));

                    RaycastHit2D hit = Physics2D.Raycast(
                        ledgePoint.Position,
                        (jumpPoint.transform.position - ledgePoint.Position).normalized,
                        Vector3.Distance(ledgePoint.Position, jumpPoint.transform.position),
                        UnwalkableMask);

                    if (hit.collider)
                    {
                        continue;
                    }

                    LedgePoint targetLedgePoint = jumpPoint.collider.GetComponent<LedgePoint>();
                    // Add jump link on node
                    node.AddLink(GetGridObject(targetLedgePoint.X, targetLedgePoint.Y), distance, PathLinkType.jump);
                }
            }
        }

        public Node[,] GetGridArray()
        {
            return GridArray;
        }

        public int GetWidth()
        {
            return GridSizeX;
        }

        public int GetHeight()
        {
            return GridSizeY;
        }
        public Node GetGridObject(int x, int y)
        {
            return GridArray[x, y];
        }

        public List<Node> GetNeighbours(Node node)
        {
            // Create new cell neighbours list
            List<Node> neighbours = new List<Node>();

            // Check Est and West cells
            for (int x = -1; x <= 1; x++)
            {
                // Check north and South cells
                for (int y = -1; y <= 1; y++)
                {
                    // Check if examining current cell
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;

                    if (!OutOfBounds(checkX, checkY))
                    {
                        neighbours.Add(GetGridObject(checkX, checkY));
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPoint(Vector3 _worldPosition)
        {
            float percentX = Mathf.Abs(BoxPosition.x - _worldPosition.x) / CellSize;
            float percentY = Mathf.Abs(BoxPosition.y - _worldPosition.y) / CellSize;
            percentX = Mathf.Floor(percentX);
            percentY = Mathf.Floor(percentY);
            int x = Mathf.RoundToInt(percentX);
            int y = Mathf.RoundToInt(percentY);

            return GridArray[x, y];

        }


    }
}

