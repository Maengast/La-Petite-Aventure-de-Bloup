using PathFinder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : Character
{
    // Start is called before the first frame update
    [SerializeField] Transform victim;
    private PathFinding pathFinding;
    private float Speed = 3f;
    private List<Node> path;
    private Vector2 Movement;
    void Start()
    {
        Init();
        pathFinding = FindObjectOfType<PathFinding>();
    }

    // Update is called once per frame


    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, victim.position) > 1f)
        {

            Move(Movement);
        }
    }
    void Update()
    {

        path = pathFinding.FindPath(transform.position, victim.position);
        if (path != null && path.Count > 0)
        {
            Vector3 targetposition = path[0].Position;
            if (path[0].LinkType == PathLinkType.ground && IsGrounded)
            {
                Movement.x = targetposition.x - transform.position.x;
                SetBoolAnim("IsRunning", Movement.x > 0 || Movement.x < 0);
            }
            Movement.y = (OnJump) ? 1 : (IsGrounded) ? 0 : -1;
            if (path[0].LinkType == PathLinkType.fall)
            {
                Movement.x = targetposition.x - transform.position.x;
            }

            else if (CanJump && path[0].LinkType == PathLinkType.jump || path[0].LinkType == PathLinkType.runoff)
            {
                JumpAndFollow(targetposition, YSpeed) ;
                //Debug.Log("jump");
                //int direction = movedir.x > 0 ? 1 : -1;
                //Vector3 finalPosition = pathFinding.GetGrid().GetGridObject(path[0].GridX + direction, path[0].GridY).Position;

                //StartCoroutine(JumpAndFollow(finalPosition, 1f));

            }
            else if (OnJump && transform.position.y >= MaxJumpHeight)
            {
              //EndJump();
            }
        }
    }


    private IEnumerator JumpAndFollow(Vector3 targetPosition, float timeToJump)
    {
        var startPosition = transform.position;
        var lastTargetPosition = targetPosition;
        var initialVelocity = getInitialVelocity(lastTargetPosition - startPosition, timeToJump);

        var progress = 0f;
        while (progress < timeToJump)
        {
            progress += Time.deltaTime;
            if (targetPosition != lastTargetPosition)
            {
                lastTargetPosition = targetPosition;
                initialVelocity = getInitialVelocity(lastTargetPosition - startPosition, timeToJump);
            }

            transform.position = startPosition + (progress * initialVelocity) + (0.5f * Mathf.Pow(progress, 2) * Physics.gravity);
            yield return null;
        }
    }

    private Vector3 getInitialVelocity(Vector3 toTarget, float timeToJump)
    {
        return (toTarget - (0.5f * Mathf.Pow(timeToJump, 2) * Physics.gravity)) / timeToJump;
    }

    private void OnDrawGizmos()
    {
        if (path!=null)
        {
            Debug.Log("ondraw");
            Gizmos.DrawWireCube(pathFinding.transform.position, new Vector3(pathFinding.GetGrid().GetWidth(), pathFinding.GetGrid().GetHeight(), 1));//Draw a wire cube with the given dimensions from the Unity inspector

            if (pathFinding.GetGrid() != null)//If the grid is not empty
            {
                foreach (Node n in path)//Loop through every node in the grid
                {
                    if (n.IsWalkable)//If the current node is a wall node
                    {
                        Gizmos.color = Color.white;//Set the color of the node
                    }
                    else
                    {
                        Gizmos.color = Color.red;//Set the color of the node
                    }

                    Gizmos.DrawCube(n.Position, Vector3.one * (pathFinding.GetGrid().CellSize / 2));//Draw the node at the position of the node.
                }
            }
        }

    }

}
