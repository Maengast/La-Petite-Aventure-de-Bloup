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
        private Node[,] _gridArray;

        private int _gridSizeX, _gridSizeY;
        private Vector2 _boxPosition;
        public float CellSize;

        private List<Node> _ledges = new List<Node>();
        private List<Node> _corners = new List<Node>();

        public float JumpDistance = 5;
        public float CornerOffset = 2;
        private float _boxWidth, _boxHeight;
        private List<LedgePoint> _ledgePoints = new List<LedgePoint>();
        
        public GameObject LedgePlateform;
        
        private GameObject _areaBox;
        
        private void Start()
        {
            DefineBox();
            CreateGrid();
        }

        void DefineBox()
        {
	        _areaBox = GameObject.Find("Background");
            _boxWidth = _areaBox.GetComponent<SpriteRenderer>().bounds.size.x;
            _boxHeight = _areaBox.GetComponent<SpriteRenderer>().bounds.size.y;
            float boxX = _areaBox.GetComponent<SpriteRenderer>().bounds.center.x - (_areaBox.GetComponent<SpriteRenderer>().bounds.size.x / 2);
            float boxY = _areaBox.GetComponent<SpriteRenderer>().bounds.center.y + (_areaBox.GetComponent<SpriteRenderer>().bounds.size.y / 2);
            _boxPosition = new Vector2(boxX, boxY);
        }

        // Construct Grid
        void CreateGrid()
        {
            _gridSizeX = Mathf.RoundToInt(_boxWidth / CellSize);
            _gridSizeY = Mathf.RoundToInt(_boxHeight / CellSize);
            _gridArray = new Node[_gridSizeX, _gridSizeY];

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
            foreach ( LedgePoint ledgePoint in _ledgePoints)
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
            if (!_gridArray[x,y].IsWalkable) return true;

            return false;
        }

        private void CreateNodes()
        {
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    Vector3 nodePosition = new Vector3((x * CellSize) + _boxPosition.x + (CellSize / 2), -(y * CellSize) + _boxPosition.y + (CellSize / 2), 0);

                    // Check if there are obstacles
                    bool walkable = IsNodeWalkable(nodePosition);

                    // Create new node
                    _gridArray[x, y] = new Node(walkable, nodePosition, x, y);
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
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    Node node = GetGridObject(x, y);
                    if (node.IsWalkable && Blocked(x, y + 1) && y + 1 < _gridSizeY)
                    {   // If the bottom ledge is blocked or bottom of y axis
                        node.Ledge = true;
                        _ledges.Add(node);
                        if (!Blocked(x - 1, y + 1) || !Blocked(x + 1, y + 1))
                            _corners.Add(node);
                    }
                }
            }
        }

        private void AddWalkLinks()
        {
            foreach (Node ledge in _ledges)
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
            foreach (Node corner in _corners)
            {
                // Discover the direction the tile is facing
                int direction = Blocked(corner.GridX - 1, corner.GridY + 1) ? 1 : -1;

                // Step over the facing direction 1 tile
                Node overhang = GetGridObject(corner.GridX + direction*(int)CornerOffset, corner.GridY);

                //Vector2 origin = new Vector2(overhang.Position.x + direction * CornerOffset, overhang.Position.y);
                // Shoot a raycast straight down to the end of the boundary to look for a fall
                RaycastHit2D hit = Physics2D.Raycast(
                    overhang.Position,
                    Vector2.down,
                    (GetHeight() - overhang.GridY) * CellSize,
                    UnwalkableMask);

                // If we hit something add a fall link at the hit target
                if (hit.collider)
                {
                    Node node = NodeFromWorldPoint(hit.point);
                    int distance = (int)Mathf.Floor(Vector3.Distance(corner.Position, node.Position));
                    corner.AddLink(node, distance, PathLinkType.fall);
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
                    Node node = NodeFromWorldPoint(hit.point);
                    float high = Math.Abs(overhang.Position.y) - Math.Abs(node.Position.y);
                    if(high < JumpDistance)
                    {
                        int distance = (int)Mathf.Floor(Vector3.Distance(corner.Position, node.Position));
                        // platform corner to node link
                        corner.AddLink(node, distance, PathLinkType.runoff);
                        // current nose to platform corner
                        node.AddLink(corner, distance, PathLinkType.runoff);
                    }
                }
                // Creata a ledge point for evaluating corner jumps
                GameObject pointObj = (GameObject)Instantiate(LedgePlateform);
                LedgePoint point = pointObj.GetComponent<LedgePoint>();
                pointObj.transform.position = GetGridObject(corner.GridX, corner.GridY).Position;
                _ledgePoints.Add(point);
                point.X = corner.GridX;
                point.Y = corner.GridY;
                point.Direction = direction;
                point.Position = GetGridObject(corner.GridX, corner.GridY).Position;
                _ledgePoints.Add(point);
            }
        }

        private void AddJumpLinks() {
            // Loop through all ledge corners
            foreach (LedgePoint ledgePoint in _ledgePoints)
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
            return _gridArray;
        }

        public int GetWidth()
        {
            return _gridSizeX;
        }

        public int GetHeight()
        {
            return _gridSizeY;
        }
        public Node GetGridObject(int x, int y)
        {
            return _gridArray[x, y];
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = Mathf.Abs(_boxPosition.x - worldPosition.x) / CellSize;
            float percentY = Mathf.Abs(_boxPosition.y - worldPosition.y) / CellSize;
            percentX = Mathf.Floor(percentX);
            percentY = Mathf.Floor(percentY);
            int x = Mathf.RoundToInt(percentX);
            int y = Mathf.RoundToInt(percentY);

            return _gridArray[x, y];

        }


    }
}

