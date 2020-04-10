using System;
using DataBase;
using PathFinder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : Character
{
    // Start is called before the first frame update
    Player player;
    private PathFinding pathFinding;
    private List<Node> path;
    public StaminaBar StaminaBar;
    private int currentPoint;
    float _currentStamina = 100;
    float maxDistanceToTarget; 
    float maxTimeToTarget; 
    float MaxStamina = 100;
   
    public BossInfo BossInfo;
    void Start()
    {
        BossInfo characterInfo = BossDb.GetAllBoss()[0];
        Speed = characterInfo.Speed;
        AttackMultiplier = characterInfo.Attack_Multiplier;
        MaxStamina = characterInfo.MaxStamina;
        MaxLife = characterInfo.MaxLife;
        base.Init();
        player = FindObjectOfType<Player>();
        pathFinding = FindObjectOfType<PathFinding>();
        StaminaBar.SetMaxStamina(characterInfo.MaxStamina);
        InvokeRepeating("UpdateStamina", 0f, 3f);
        StartSearchPathToPlayer();
    }

    void Update()
    {
        if (path == null || currentPoint >= path.Count)
        {
            SetBoolAnim("IsRunning", false);
            return;
        }
      
        if (OnGround)
        {
            ChooseBestAttack();
            SetBoolAnim("IsRunning", true);

            float distance = Vector2.Distance(transform.position, path[currentPoint].Position);
            if (distance < 1f)
            {
                currentPoint++;
            }
        }
        
        MoveTo(GetNodeToFollow().Position);
    }
    
    /**
     * Move Character to target position
     */
    public virtual void MoveTo(Vector3 target)
    {
	    Vector2 distance = (target - transform.position);
	    if (!OnGround)
	    {
		    float time = (Mathf.Abs(distance.x )+3.0f) / currentXSpeed;
		    speedMultiplier = time / maxTimeToTarget;
		    currentXSpeed *= speedMultiplier;
	    }
	    _movementDirection.x = distance.normalized.x;
    }
    

    public int GetCurrentPoint()
    {
        return currentPoint;
    }
    public List<Node> GetPath()
    {
        return path;
    }

    public override void Jump()
    {
	    maxDistanceToTarget = 2 * Mathf.Pow(XSpeed, 2) / Gravity;
	    maxTimeToTarget = 2 * XSpeed / Gravity;
	    currentPoint++;
        Vector2 movement = path[currentPoint].Position - transform.position;
        JumpHeight = Mathf.Abs(movement.y)+ 2f;
        base.Jump();
    }

    public void StartSearchPathToPlayer()
    {
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }
    void UpdatePath()
    {
        if (pathFinding.IsDone && !InJump && OnGround)
            pathFinding.FindPath(transform.position, player.transform.position, OnPathComplete);
    }

    void OnPathComplete(List<Node> path)
    {
        this.path = path;
        currentPoint = 0;
    }


    void UpdateStamina()
    {
        if (_currentStamina < MaxStamina)
        {
            _currentStamina += 3;
            StaminaBar.SetStamina(_currentStamina);
        }
    }

    public void UseStamina(float stamina)
    {
        _currentStamina = _currentStamina - stamina > 0 ? _currentStamina - stamina : 0;
        StaminaBar.SetStamina(_currentStamina);
    }

    public void Attack(IAttack attack)
    {
      
        attack.Lauch(this);
    }

    public Attack ChooseBestAttack()
    {
        Dictionary<string, int> attackWeights = new Dictionary<string, int>();
       foreach(AttackModel attackModel in AttackInventory.Attacks)
        {
            //Debug.Log(attackModel.name);
        }
        return null;
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
