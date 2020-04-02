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
    private int currentPoint;
    [SerializeField] float _currentStamina = 100;
    private float _maxStamina = 100;
    void Start()
    {
        base.Init();
        pathFinding = FindObjectOfType<PathFinding>();
        staminaBar.SetMaxStamina(_maxStamina);

        InvokeRepeating("UpdateStamina", 0f, 3f);

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

    public Node GetPath()
    {
        return path[currentPoint];
    }

    public override void Jump()
    {
        SwitchJumpState();
        currentPoint++;
        Vector2 movement = path[currentPoint].Position - transform.position;
        JumpHeight = movement.y + 2f;
        Rigidbody.velocity = CalculateJumpVelocity(movement);
        StartCoroutine("Jumping");
    }

    Vector2 CalculateJumpVelocity(Vector2 movement)
    {
        float time = Mathf.Sqrt(Mathf.Abs(2 * JumpHeight / Gravity)) + Mathf.Sqrt(Mathf.Abs(-2*(movement.y - JumpHeight) /Gravity));
        float velocityY =  Mathf.Sqrt(Mathf.Abs(2 * Gravity * JumpHeight));
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
        if(pathFinding.IsDone)
            pathFinding.FindPath(transform.position, victim.position, OnPathComplete);
    }

    void OnPathComplete(List<Node> path)
    {
        this.path = path;
        currentPoint = 0;
    }


    void UpdateStamina()
    {
        if (_currentStamina < _maxStamina)
        {
            _currentStamina += 3;
            staminaBar.SetStamina(_currentStamina);
        }
    }

    public void UseStamina(float stamina)
    {
        _currentStamina = _currentStamina - stamina > 0 ? _currentStamina - stamina : 0;
        staminaBar.SetStamina(_currentStamina);
    }

    public Attack GetBestBossAttack()
    {
        return new Attack();
    }



}
