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
    private List<Node> path;
    public StaminaBar staminaBar;
    [SerializeField] float _currentStamina = 100;
    private float _maxStamina = 100;
    void Start()
    {
        base.Init();
        pathFinding = FindObjectOfType<PathFinding>();
        staminaBar.SetMaxStamina(_maxStamina);
        StartCoroutine("UpdateStamina");
    }

    // Update is called once per frame


    private void FixedUpdate()
    {
        CalcCurrentSpeed();
    }
    void Update()
    {
        Vector3 targetposition = transform.position;
        base.Update();
        if(IsGrounded)
        {
            SetBoolAnim("IsRunning", _movementDirection.x > 0 || _movementDirection.x < 0);

            path = pathFinding.FindPath(transform.position, victim.position);
            if (path != null && path.Count > 0)
            {
                targetposition = path[0].Position;

                if (path[0].LinkType == PathLinkType.jump || path[0].LinkType == PathLinkType.runoff)
                {
                    StartCoroutine(JumpToTarget(targetposition));

                }

            }
        }
        Move(targetposition);

    }

    private IEnumerator JumpToTarget(Vector3 targetPosition)
    {
        
        Vector3 startPosition = transform.position;

        // Time = Distance / Velocity
        float timeToJump = Vector3.Distance(transform.position, targetPosition) / JumpSpeed;
        var progress = 0f;
        base.SwitchJumpState();
        while (progress < 1.0f)
        {
            progress += Time.deltaTime / timeToJump;         
            float height = Mathf.Sin(Mathf.PI * progress) * Mathf.Abs(targetPosition.y - startPosition.y);
            //if (Mathf.Cos(Mathf.PI * progress) < 0)
            //{
            //    base.SwitchJumpState();
            //}
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress) + Vector3.up * height;
            yield return null;
        }
    }

    IEnumerator UpdateStamina()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if (_currentStamina < _maxStamina)
            {
                _currentStamina += 3;
                staminaBar.SetStamina(_currentStamina);
            }
        }
    }

    public void UseStamina(float stamina)
    {
        _currentStamina = _currentStamina - stamina > 0 ? _currentStamina - stamina : 0;
        staminaBar.SetStamina(_currentStamina);
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
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
