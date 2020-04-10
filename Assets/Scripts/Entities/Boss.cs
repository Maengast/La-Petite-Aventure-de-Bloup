using System;
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
    public  float MaxStamina = 100;
    float maxDistanceToTarget; 
    float maxTimeToTarget; 
    void Start()
    {
        base.Init();
        player = FindObjectOfType<Player>();
        pathFinding = FindObjectOfType<PathFinding>();
        StaminaBar.SetMaxStamina(MaxStamina);
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
    

    public Node GetNodeToFollow()
    {
        return path[currentPoint];
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


}
