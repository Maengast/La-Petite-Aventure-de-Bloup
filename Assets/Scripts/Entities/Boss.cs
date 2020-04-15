using DataBase;
using PathFinder;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : Character
{
    // Start is called before the first frame update
    Player player;
    private PathFinding pathFinding;
    private List<Node> path;
    public BarScript StaminaBar;
    private int currentPoint;
    float _currentStamina = 100;
    float MaxStamina = 100;
    Transform _firePoint;
   
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
        _firePoint = transform.Find("FirePoint");
        StaminaBar.SetMaxValue(characterInfo.MaxStamina);
        StartCoroutine("UpdateStamina");
        StartCoroutine("StartSearchPathToPlayer");


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

            AttackModel attackModel = ChooseBestAttack();
            if(attackModel != null)
            {
                RaycastHit2D hit = Physics2D.Raycast(_firePoint.position, transform.right, attackModel.AttackRange);
                if (hit.collider &&  hit.collider.tag == "Player")
                {
                    SetTriggerAnim(attackModel.Name);
                    Attack attack = AttackFactory.GetAttack(attackModel);
                    UseStamina(attackModel.Cost);
                    Attack(attack);
                }
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

    private IEnumerator StartSearchPathToPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            UpdatePath();
        }
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


    IEnumerator UpdateStamina()
    {
        while (true)
        {
            // update stamina every 3seconds
            yield return new WaitForSeconds(3f);
            if (_currentStamina < MaxStamina)
            {
                _currentStamina += (MaxStamina /10);
                StaminaBar.SetValue(_currentStamina);
            }
        }

    }

    public void UseStamina(float stamina)
    {
        _currentStamina = _currentStamina - stamina > 0 ? _currentStamina - stamina : 0;
        StaminaBar.SetValue(_currentStamina);
    }

    public void Attack(IAttack attack)
    {
      
        attack.Launch(this);
    }

    public AttackModel ChooseBestAttack()
    {
        List<AttackModel>[] attackListArray = new List<AttackModel>[] { GetLowestCostAttacks(), GetMostDamagesAttacks() } ;

        List<AttackModel> bestChoice = attackListArray[0];

        foreach(List<AttackModel> attacks in attackListArray)
        {
            if(GetTotalDamage(attacks) < GetTotalDamage(bestChoice))
            {
                bestChoice = attacks;
            }
        }

        if (bestChoice.Count > 0)
            return bestChoice[0];
        return null;
    }

    private int GetTotalDamage(List<AttackModel> attacks)
    {

        int totalDamages = 0;
        foreach (AttackModel attack in attacks)
        {
            totalDamages += attack.Damage;
        }
        return totalDamages;

    }

    private List<AttackModel> GetLowestCostAttacks()
    {
        // Order attacks by cost
        IEnumerable orderedAttacks = AttackInventory.Attacks.OrderBy(att => att.Cost).ThenByDescending(att => att.Damage);

        // Return all available attacks and order them by descending number of damage
        return GetAvailableAttacks(orderedAttacks).OrderByDescending(att => att.Damage).ToList();
    }

    List<AttackModel> GetAvailableAttacks(IEnumerable attacks)
    {
        float stamina = 0;
        List<AttackModel> bestAttacks = new List<AttackModel>();
        // Loop on all attacks
        foreach (AttackModel attackModel in attacks)
        {
            // Boss has enough stamina and attack range can touch boss
            if (!IsInRange(attackModel) || !HasEnoughStamina(attackModel))
            {
                continue;
            }
            bool hasEnoughtStam = true;
            while (hasEnoughtStam)
            {
                // loop while all attacks cost inferior to current stamina
                if (stamina < _currentStamina)
                {
                    stamina += attackModel.Cost; // if yes add attack cost to totalStamina Required
                    bestAttacks.Add(attackModel);// add current attacks to best attacks list
                }
                else
                {
                    hasEnoughtStam = false;
                }
            }
        }
        return bestAttacks;
    }


    private List<AttackModel> GetMostDamagesAttacks()
    {
        IEnumerable orderedAttacks = AttackInventory.Attacks.OrderByDescending(att => att.Damage);
        return GetAvailableAttacks(orderedAttacks);
    }

    bool IsInRange(AttackModel attack)
    {
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (attack.Type == AttackType.Distance && distance <= (int)AttackType.Distance)
        {
            return false;
        }
        return attack.AttackRange > distance;
    }

    bool HasEnoughStamina(AttackModel attack)
    {
        return attack.Cost < _currentStamina;
    }

    //private void OnDrawGizmos()
    //{
    //    if (path != null)
    //    {
    //        Gizmos.DrawWireCube(pathFinding.transform.position, new Vector3(pathFinding.GetGrid().GetWidth(), pathFinding.GetGrid().GetHeight(), 1));//Draw a wire cube with the given dimensions from the Unity inspector

    //        if (pathFinding.GetGrid() != null)//If the grid is not empty
    //        {
    //            foreach (Node n in path)//Loop through every node in the grid
    //            {
    //                if (n.IsWalkable)//If the current node is a wall node
    //                {
    //                    Gizmos.color = Color.white;//Set the color of the node
    //                }
    //                else
    //                {
    //                    Gizmos.color = Color.red;//Set the color of the node
    //                }

    //                Gizmos.DrawCube(n.Position, Vector3.one * (pathFinding.GetGrid().CellSize / 2));//Draw the node at the position of the node.
    //            }
    //        }
    //    }

    //}


}
