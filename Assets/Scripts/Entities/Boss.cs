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
        SwitchJumpState();
        currentPoint++;
        Vector2 movement = path[currentPoint].Position - transform.position;
        JumpHeight = movement.y + 2f;
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
        float velocityY =  Mathf.Sqrt(2 * Gravity * JumpHeight);
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
