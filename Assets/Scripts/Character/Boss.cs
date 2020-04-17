using System;
using DataBase;
using PathFinder;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : Character
{
	[Header("Character Components")]
	public BarScript StaminaBar;
    public bool HasDetectedVictim;

    //Boss Pathfinding
    private Player _player;
    private PathFinding _pathFinding;
    private List<Node> _path;
    private int currentPoint;
    
    //Boss Stamina
    private float _currentStamina = 100;
    private float _maxStamina = 100;
    private BoxCollider2D _boxCollider;
    Transform _firePoint;
   
    private BossInfo _info;
    private float trajectoryTime;
    /**
     * Called when Object and components are fully instantiated
     * Start is called before the first frame update
     */ 
    void Start()
    {
	    //Set Boss Stats
	    _info = _levelManager.GetBossInfo();
        SetBossStats(_info);
        Init(); //Init Boss

        // Set if boss has detected player to false
        HasDetectedVictim = false;

        //Find required components
        _player = FindObjectOfType<Player>();
        _pathFinding = FindObjectOfType<PathFinding>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _firePoint = transform.Find("FirePoint");
        // StaminaBar.SetMaxValue(_info.MaxStamina);
        // StartCoroutine("UpdateStamina");
        //Update path if _player exist
        if(_player) UpdatePath();
    }
	
    /**
     * Set stats of boss
     */
    private void SetBossStats(BossInfo _info)
    {
	    SetCharacterStats(_info);
	    _maxStamina = _info.MaxStamina;
	    trajectoryTime = 2 * Speed / Gravity;
    }
	
    /**
     * Called every frame
     */
    protected override void Update()
    {
        //Do nothing if not alive
	    base.Update();

        CheckAttack();
        //No _path , no move
        if (_path == null || currentPoint >= _path.Count || !HasDetectedVictim)
        {
            SetBoolAnim("IsRunning", false);
            _movementDirection.x = 0;
            return;
        }

	    //Get target node of _path
	    Node targetNode = _path[currentPoint];
	    Vector2 targetPos = targetNode.Position;
        if (OnGround)
        {
            SetBoolAnim("IsRunning", true);
            RaycastHit2D checkDodge = Physics2D.Raycast(transform.position + transform.right * 2f, -transform.right, 10f, LayerMask.GetMask("AttackObject"));
            // Dodge if ennemies attack object in front of him
            if (checkDodge.collider && checkDodge.collider.GetComponent<AttackObject>().Launcher != this)
            {
                AttackObject attackObject = checkDodge.collider.GetComponent<AttackObject>();
                Dodge(attackObject);
                return;
            }
            //Jump if is on jump node
            if (targetNode.LinkType == PathLinkType.jump && !IsJumping())
            {
                Jump();
            }
            if (targetNode.LinkType == PathLinkType.fall)
            {
	            
            }
            //Change node target if boss has reached the target
            float distance = Vector2.Distance(transform.position, targetPos);
            if (distance < 1f)
            {
                currentPoint++;
            }
        }

        //Move Boss
        MoveTo(targetPos);

    }

    private void Dodge(AttackObject obj)
    {
        if (!Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size ,0f , Vector3.up, 5f,LayerMask.GetMask("Ground")).collider)
        {
            JumpHeight = 5f;
            base.Jump();
        }
        else
        {
            _movementDirection.x = obj.Direction.x;
        }
    }

    private void CheckAttack()
    {
        AttackModel attackModel = ChooseBestAttack();
        if (attackModel != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(_firePoint.position, transform.right, attackModel.AttackRange);
            if (hit.collider && hit.collider.tag == "Player")
            {
                SetTriggerAnim(attackModel.Name);
                Attack attack = AttackFactory.GetAttack(attackModel);
                UseStamina(attackModel.Cost);
                Attack(attack);
            }
        }
    }

    /**
     * Called every fixed frame-rate frame
     * Do physics calculations
     */
    protected override void FixedUpdate()
    {
	    //Update path to take if player reached
	    if(_player) UpdatePath();
	    base.FixedUpdate();
	    
    }

    /**
     * Move Character to target position
     */
    public virtual void MoveTo(Vector3 targetPos)
    {
	    Vector2 distance = (targetPos - transform.position);
	    if (!OnGround)
	    {
		    float time = (Mathf.Abs(distance.x )+2.0f)/currentSpeed;
		    float speedMultiplier = time / trajectoryTime;
		    currentSpeed *= speedMultiplier;
	    }

	    _movementDirection.x = distance.normalized.x;
    }


    /**
     * Override Character.Jump()
     * Define boss jump behavior
     */
    protected override void Jump()
    {
	    CalcJumpHeigth();
	    base.Jump();
	    // Vector2 pos = transform.position;
	    // Vector2 targPos = _path[currentPoint].Position;
	    // float velocity = Mathf.Sqrt(2 * Gravity * JumpHeight + Mathf.Pow(currentSpeed, 2));
	    // float jumpAngle = Mathf.Atan(Mathf.Pow(velocity, 2) +
	    //                              Mathf.Sqrt(Mathf.Pow(velocity, 4) - Gravity *
	    //                                         (Gravity * Mathf.Pow(targPos.x-pos.x, 2) +
	    //                                          2 * targPos.y-pos.y * Mathf.Pow(velocity, 2))) / (Gravity * targPos.x-pos.x));
	    // Debug.Log(jumpAngle);
	    // _movementDirection.x = Mathf.Cos(jumpAngle);
	    // _movementDirection.y = Mathf.Sin(jumpAngle);
    }

    private void CalcJumpHeigth()
    {
        Vector2 distanceToTarget = _path[currentPoint].Position - transform.position;
	    if (Mathf.Sign(distanceToTarget.y) < 0)
	    {
		    JumpHeight = 0;
	    }
	    else
	    {
		    JumpHeight = Mathf.Abs(distanceToTarget.y)+ 2f;
	    }
    }

    public override void SetOnGround(bool value)
    {
	    base.SetOnGround(value);
	    if(value)currentSpeed = Speed;
    }


    void UpdatePath()
    {
	    if (_pathFinding.IsDone && !IsJumping() && OnGround)
	    {
		    _pathFinding.FindPath(transform.position, _player.transform.position, OnPathComplete);
	    }
    }

    void OnPathComplete(List<Node> _path)
    {
        this._path = _path;
        currentPoint = 0;
    }


    IEnumerator UpdateStamina()
    {
        while (true)
        {
            // update stamina every 3seconds
            yield return new WaitForSeconds(3f);
            if (_currentStamina < _maxStamina)
            {
                _currentStamina += (_maxStamina /10);
                StaminaBar.SetValue(_currentStamina);
            }
        }

    }

    public void UseStamina(float stamina)
    {
        _currentStamina = _currentStamina - stamina > 0 ? _currentStamina - stamina : 0;
        //StaminaBar.SetValue(_currentStamina);
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
        float distance = Mathf.Abs(_player.transform.position.x - transform.position.x);
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

    protected override void Die()
    {
	    _levelManager.CharacterDie(false);
	    base.Die();
    }

    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            Gizmos.DrawWireCube(_pathFinding.transform.position, new Vector3(_pathFinding.GetGrid().GetWidth(), _pathFinding.GetGrid().GetHeight(), 1));//Draw a wire cube with the given dimensions from the Unity inspector
    
            if (_pathFinding.GetGrid() != null)//If the grid is not empty
            {
                foreach (Node n in _path)//Loop through every node in the grid
                {
                    if (n.IsWalkable)//If the current node is a wall node
                    {
                        Gizmos.color = Color.white;//Set the color of the node
                    }
                    else
                    {
                        Gizmos.color = Color.red;//Set the color of the node
                    }
    
                    Gizmos.DrawCube(n.Position, Vector3.one * (_pathFinding.GetGrid().CellSize / 2));//Draw the node at the position of the node.
                }
            }
        }
    }
    
    // private void OnDrawGizmos()
    // {
	   //  if (_pathFinding != null)
	   //  {
		  //   //Gizmos.DrawWireCube(_pathFinding.transform.position, new Vector3(_pathFinding.GetGrid().GetWidth(), _pathFinding.GetGrid().GetHeight(), 1));//Draw a wire cube with the given dimensions from the Unity inspector
    //
		  //   if (_pathFinding.GetGrid() != null)//If the grid is not empty
		  //   {
			 //    foreach (Node n in _pathFinding.GetGrid().GetGridArray())//Loop through every node in the grid
			 //    {
				//     if (n.IsWalkable)//If the current node is a wall node
				//     {
				// 	    Gizmos.color = Color.white;//Set the color of the node
				//     }
				//     else
				//     {
				// 	    Gizmos.color = Color.red;//Set the color of the node
				//     }
    //
				//     Gizmos.DrawCube(n.Position, Vector3.one * (_pathFinding.GetGrid().CellSize/2));//Draw the node at the position of the node.
			 //    }
		  //   }
	   //  }
    // }


}
