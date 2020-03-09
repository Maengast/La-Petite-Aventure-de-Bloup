using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform victim;
    private PathFinding pathFinding;
    void Start()
    {
        pathFinding = FindObjectOfType<PathFinding>();
    }

    // Update is called once per frame

    // Update is called once per frame
    void Update()
    {
        Move();

    }

    void Move()
    {
        if (Vector3.Distance(transform.position, victim.position) > 1f)
        {
            List<Node> path = pathFinding.FindPath(transform.position, victim.position);
            if (path != null && path.Count > 1)
            {
                Vector3 targetPosition = path[0].Position;
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                transform.position = transform.position + moveDir * 1f * Time.deltaTime;
            }
        }
    }

    private void OnDrawGizmos()
    {
        pathFinding = FindObjectOfType<PathFinding>();
        Gizmos.DrawWireCube(pathFinding.transform.position, new Vector3(pathFinding.GridWorldSize.x, pathFinding.GridWorldSize.y, 1));//Draw a wire cube with the given dimensions from the Unity inspector

        if (pathFinding.grid != null)//If the grid is not empty
        {
            foreach (Node n in pathFinding.grid.GetGrid())//Loop through every node in the grid
            {
                if (n.IsWalkable)//If the current node is a wall node
                {
                    Gizmos.color = Color.white;//Set the color of the node
                }
                else
                {
                    Gizmos.color = Color.red;//Set the color of the node
                }

                Gizmos.DrawCube(n.Position, Vector3.one * (pathFinding.CellSize));//Draw the node at the position of the node.
            }
        }
    }



    IEnumerator WaitForSeconds(float seconds)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(seconds);
    }

}
