using System;
using DataBase;
using PathFinder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : Character
{
	[Header("Character Components")]
	public StaminaBar StaminaBar;
	
	//Boss Pathfinding
    private Player _player;
    private PathFinding _pathFinding;
    private List<Node> _path;
    private int currentPoint;
    
    //Boss Stamina
    private float _currentStamina = 100;
    private float _maxStamina = 100;
   
    private BossInfo _info;
    private float trajectoryTime;
    /**
     * Called when Object and components are fully instantiated
     * Start is called before the first frame update
     */ 
    void Start()
    {
	    //Set Boss Stats
	    BossInfo _info = BossDb.GetAllBoss()[0];
        SetBossStats(_info);
        Init(); //Init Boss
        
        //Find required components
        _player = FindObjectOfType<Player>();
        _pathFinding = FindObjectOfType<PathFinding>();
        
        InvokeRepeating("UpdateStamina", 0f, 3f);
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
    void Update()
    {
	    //No _path , no move
	    if (_path == null || currentPoint >= _path.Count)
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
            ChooseBestAttack();
            SetBoolAnim("IsRunning", true);
			
            //Jump if is on jump node
            if (targetNode.LinkType == PathLinkType.jump || targetNode.LinkType == PathLinkType.runoff)
            {
	            Jump();
            }

            if (targetNode.LinkType == PathLinkType.fall)
            {
	            
            }
            
            //Change node target if boss has reached the target
            float distance = Vector2.Distance(transform.position, _path[currentPoint].Position);
            if (distance < 0.1f)
            {
                currentPoint++;
            }
        }
        
	    //Move Boss
        MoveTo(targetPos);
    }
	
    /**
     * Called every fixed frame-rate frame
     * Do physics calculations
     */
    protected override void FixedUpdate()
    {
	    //Update path to take if to reach the player
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
	    
	    currentPoint++;
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
            _pathFinding.FindPath(transform.position, _player.transform.position, OnPathComplete);
    }

    void OnPathComplete(List<Node> _path)
    {
        this._path = _path;
        currentPoint = 0;
    }


    void UpdateStamina()
    {
        if (_currentStamina < _maxStamina)
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


}
