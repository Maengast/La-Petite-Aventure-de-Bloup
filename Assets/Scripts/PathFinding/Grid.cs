using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public int JumpDistance = 5;
        float BoxWidth, BoxHeight;

        public float CornerOffset = 2;

        public void InitGrid(Vector2 gridOrigin, Vector2 gridSize, float cornerOffset, int jumpDistance)
        {
	        JumpDistance = jumpDistance;
	        CornerOffset = cornerOffset;
	        DefineBox(gridOrigin, gridSize);
	        CreateGrid();
        }

        void DefineBox(Vector2 gridOrigin, Vector2 gridSize)
        {
	        BoxWidth = gridSize.x + JumpDistance;
            BoxHeight = gridSize.y + JumpDistance;
            float boxX = gridOrigin.x;
            float boxY = gridOrigin.y;
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
            //
            // Get jumpoints of each corners
            // Check if there are jump points
            // Raycast at specific angle to check jump point
            // If valid create jump link
            AddJumpAndFallLinksToCorners();

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
            // Get bottom left position of node
            Vector2 bottomLeft = new Vector2(position.x - (CellSize / 2)+0.1f, position.y - (CellSize / 2)+0.1f);

            // Get top right position of node
            Vector2 topRight = new Vector2(position.x + (CellSize / 2)-0.1f, position.y + (CellSize / 2)-0.1f);

            // Check if an unwalkable surface stands between these two points
            return !(Physics2D.OverlapArea(bottomLeft, topRight, UnwalkableMask));
        } 

    
        private void AddLedgesAndCorners()
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    Node node = GetGridObject(x, y);
                    if (node.IsWalkable && Blocked(x, y + 1) && y + 1 < GridSizeY)
                    {   // If the bottom ledge is blocked or bottom of y axis
                        node.Ledge = true;
                        Ledges.Add(node);
                        if (!Blocked(x - 1, y + 1) || !Blocked(x + 1, y + 1))
                        {
	                        Corners.Add(node);
                        }
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

        private void AddJumpAndFallLinksToCorners()
        {
           
            foreach (Node corner in Corners)
            {
                // Discover the direction the tile is facing
                int direction = Blocked(corner.GridX - 1, corner.GridY + 1) ? 1 : -1;

                // Step over the facing direction 1 tile
                Node overhang = GetGridObject(corner.GridX + direction, corner.GridY);
                RaycastHit2D hit;
	            float x = direction;
	            float y = 0;
	            List<Node> jumpPoints = new List<Node>();
                
                // Find trhe best jump point from corner
	            while (y > -1f)
	            {
		            hit = Physics2D.Raycast(
		                overhang.Position,
		                new Vector2(x, y),
		                JumpDistance + CornerOffset,
		                JumpMask
	                );
	                x -= Mathf.Sign(direction) * 0.05f;
	                y -= 0.05f;
					
	                if (hit.collider && !OutOfBounds((int)hit.point.x, (int)hit.point.y)) 
                    { 
		                Node node = NodeFromWorldPoint(hit.point);
		                if(!node.Ledge) continue;
		                if (jumpPoints.Count > 0)
		                {
			                //Check if new jump point is better than the last
			                Node lastJumpPoint = jumpPoints.Last();
			                float distance = Mathf.Abs(Vector2.Distance(node.Position, overhang.Position));
			                float lastDistance = Mathf.Abs(Vector2.Distance(lastJumpPoint.Position, overhang.Position));
			                if (Mathf.Approximately(lastJumpPoint.Position.y,node.Position.y))
			                {
				                //Remove the last if the new is better
				                if (distance <= lastDistance)
				                {
					                jumpPoints.Remove(lastJumpPoint);
				                }
				                else
				                {
					                continue;
				                }
			                }
		                }
		                jumpPoints.Add(node);
                    }
	            }
	            //Get a node that fits more than corner to make a link
	            Node jumpOverhang = GetGridObject(corner.GridX - direction, corner.GridY);
	            foreach (Node jumpPoint in jumpPoints)
	            {
		            //Get a node that fits more than current jump point to make a link
		            Node newJumpPoint = GetGridObject(jumpPoint.GridX + direction, jumpPoint.GridY);
		            int distance = (int)Mathf.Floor(Vector3.Distance(jumpOverhang.Position, newJumpPoint.Position));
		            // platform corner to node link
		            jumpOverhang.AddLink(newJumpPoint, distance, PathLinkType.jump);
		            // GameObject test = new GameObject("jumpOverhang");
		            // test.transform.position = jumpOverhang.Position;
		            // current node to platform corner
		            newJumpPoint.AddLink(jumpOverhang, distance, PathLinkType.jump);
		            GameObject jump = new GameObject("jump");
		            jump.transform.position = newJumpPoint.Position;
	            }
	            
                // Shoot a raycast straight down to the end of the boundary to look for a fall
                //Detect if out of bounds
                if (OutOfBounds(corner.GridX + direction, corner.GridY)) return;

                overhang = GetGridObject(corner.GridX + direction* (int)CornerOffset, corner.GridY);
				
                hit = Physics2D.Raycast(
	                overhang.Position,
	                Vector2.down,
	                overhang.Position.y,
	                UnwalkableMask);
                
                // If we hit something add a fall link at the hit target
                if (hit.collider)
                {
	                Node node = NodeFromWorldPoint(hit.point);
	                int distance = (int)Mathf.Floor(Vector3.Distance(corner.Position, node.Position));
	                corner.AddLink(node, distance, PathLinkType.fall);
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

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = Mathf.Abs(BoxPosition.x - worldPosition.x) / CellSize;
            float percentY = Mathf.Abs(BoxPosition.y - worldPosition.y) / CellSize;
            int x = Mathf.RoundToInt(percentX);
            int y = Mathf.RoundToInt(percentY);

            return GridArray[x, y];
        }

    }
}