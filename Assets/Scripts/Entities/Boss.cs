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
        SwitchJumpState();
        currentPoint++;
        Vector2 movement = path[currentPoint].Position - transform.position;
        JumpHeight = Mathf.Abs(movement.y )+ 2f;
        StartCoroutine("Jumping");
        // Apply new velocity to rigibody
        Rigidbody.velocity = CalculateJumpVelocity(movement);

    }

    Vector2 CalculateJumpVelocity(Vector2 movement)
    {
        // Calculate jump time
        // timeup = Square of 2time multiply by maxjumpheight diveded by gravity 
        // timedown timeup = Square of minus 2 time multiply by distancey divided by gravity
        // time = timeup + timedown
        float time = Mathf.Sqrt(Mathf.Abs(2 * JumpHeight / Gravity)) + Mathf.Sqrt(Mathf.Abs(-2*(movement.y - JumpHeight) /Gravity));
        // Velocity = Square of 2 times gravity multiply by maxjumpHeight
        float velocityY =  Mathf.Sqrt(Mathf.Abs(2 * Gravity * JumpHeight));
        // Velocity x = DistanceX divided by time
        float velocityX = movement.x / time;
        Vector2 velocity = new Vector2(velocityX , velocityY);
        return velocity;
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
