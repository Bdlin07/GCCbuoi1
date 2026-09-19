# Part 1 – Finite State Machine trong Unity

## Mục lục

1. [FSM là gì?](#1-fsm-là-gì)
2. [Các thành phần của FSM](#2-các-thành-phần-của-fsm)
3. [Cách 1: Enum FSM với switch-case](#3-cách-1-enum-fsm-với-switch-case)
4. [Ví dụ Enum FSM hoàn chỉnh](#4-ví-dụ-enum-fsm-hoàn-chỉnh)
5. [Cách 2: State Pattern dùng Interface](#5-cách-2-state-pattern-dùng-interface)
6. [Ví dụ Interface FSM hoàn chỉnh](#6-ví-dụ-interface-fsm-hoàn-chỉnh)
7. [Dùng Abstract Class thay Interface](#7-dùng-abstract-class-thay-interface)
8. [So sánh Enum, Interface và Abstract Class](#8-so-sánh-enum-interface-và-abstract-class)
9. [FSM và Animator khác nhau thế nào?](#9-fsm-và-animator-khác-nhau-thế-nào)
10. [Cách tổ chức transition](#10-cách-tổ-chức-transition)
11. [Lỗi thường gặp](#11-lỗi-thường-gặp)
12. [Chọn cách nào cho bài của mình?](#12-chọn-cách-nào-cho-bài-của-mình)
13. [Bảng ghi nhớ nhanh](#13-bảng-ghi-nhớ-nhanh)

---

## 1. FSM là gì?

FSM là viết tắt của **Finite State Machine**, tiếng Việt thường gọi là **máy trạng thái hữu hạn**.

Hiểu đơn giản:

> Một đối tượng có một số trạng thái nhất định. Tại một thời điểm, nó đang ở một trạng thái. Khi điều kiện phù hợp, nó chuyển sang trạng thái khác.

Ví dụ một enemy có thể có các trạng thái:

```text
Idle   – đứng yên
Patrol – đi tuần
Chase  – đuổi theo Player
Attack – tấn công
Dead   – đã chết
```

Luồng có thể là:

```text
Idle ──hết thời gian chờ──> Patrol
Patrol ──thấy Player─────> Chase
Chase ──đủ gần───────────> Attack
Attack ──Player chạy xa──> Chase
Chase ──mất dấu Player───> Patrol
Bất kỳ state ──hết máu───> Dead
```

FSM giúp mình tránh viết một đống `if` chồng chéo như:

```csharp
if (!isDead)
{
    if (canSeePlayer)
    {
        if (canAttack)
        {
            // Tấn công
        }
        else
        {
            // Đuổi theo
        }
    }
    else
    {
        // Đi tuần
    }
}
```

Khi game lớn lên, những biến như `isRunning`, `isAttacking`, `isDead`, `isHurt` có thể cùng đúng một lúc và tạo ra trạng thái vô lý. FSM buộc mình nghĩ rõ:

```text
Hiện tại đối tượng đang ở state nào?
State đó được làm gì?
Khi nào nó được phép rời state?
```

---

## 2. Các thành phần của FSM

### 2.1. State

State là trạng thái hiện tại của đối tượng.

Ví dụ:

```text
Player đang Jump
Enemy đang Chase
Door đang Open
Game đang Paused
```

### 2.2. Transition

Transition là việc chuyển từ state này sang state khác:

```text
Idle → Run
Run → Jump
Jump → Fall
Fall → Idle
```

### 2.3. Condition

Condition là điều kiện để transition xảy ra:

```text
Speed > 0
IsGrounded = false
DistanceToPlayer < AttackRange
Health <= 0
```

### 2.4. Enter, Update và Exit

Một state thường có ba giai đoạn:

```text
Enter
  ↓
Update lặp lại khi state còn active
  ↓
Exit
```

#### Enter

Chạy một lần khi vừa bước vào state.

Ví dụ khi vào Attack:

- Dừng di chuyển.
- Bật animation Attack.
- Reset bộ đếm thời gian đánh.

#### Update

Chạy lặp lại trong lúc đang ở state.

Ví dụ trong Chase:

- Theo dõi vị trí Player.
- Kiểm tra khoảng cách.
- Chuyển sang Attack khi đủ gần.

#### Exit

Chạy một lần trước khi rời state.

Ví dụ khi rời Attack:

- Tắt hitbox.
- Hủy effect tấn công.
- Dọn dữ liệu tạm.

### 2.5. Tại một thời điểm có bao nhiêu state?

Trong FSM đơn giản, một đối tượng chỉ có **một state chính đang active**.

```text
currentState = EnemyState.Chase
```

Nhưng game lớn có thể có nhiều FSM song song:

```text
Movement FSM: Grounded / Airborne
Combat FSM:   Normal / Attacking / Blocking
Game FSM:     Playing / Paused / GameOver
```

Đừng nhét mọi khái niệm vào một FSM duy nhất nếu chúng thực sự độc lập với nhau.

---

## 3. Cách 1: Enum FSM với switch-case

### 3.1. Ý tưởng

Mình tạo một `enum` liệt kê các state:

```csharp
public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Dead
}
```

Sau đó lưu state hiện tại:

```csharp
private EnemyState currentState;
```

Và dùng `switch-case`:

```csharp
private void Update()
{
    switch (currentState)
    {
        case EnemyState.Idle:
            UpdateIdle();
            break;

        case EnemyState.Patrol:
            UpdatePatrol();
            break;

        case EnemyState.Chase:
            UpdateChase();
            break;

        case EnemyState.Attack:
            UpdateAttack();
            break;

        case EnemyState.Dead:
            break;
    }
}
```

### 3.2. Enum là gì?

`enum` là kiểu liệt kê một tập hợp giá trị có tên.

Thay vì dùng số khó hiểu:

```csharp
int state = 2;
```

mình dùng:

```csharp
EnemyState state = EnemyState.Chase;
```

Code dễ đọc hơn và hạn chế gán giá trị linh tinh.

### 3.3. Hàm ChangeState

Không nên gán `currentState` rải rác khắp script:

```csharp
currentState = EnemyState.Attack;
```

Nên gom vào một hàm:

```csharp
private void ChangeState(EnemyState newState)
{
    if (currentState == newState)
        return;

    ExitState(currentState);
    currentState = newState;
    EnterState(currentState);
}
```

Luồng chuyển state:

```text
Exit state cũ
      ↓
Đổi currentState
      ↓
Enter state mới
```

### 3.4. EnterState

```csharp
private void EnterState(EnemyState state)
{
    switch (state)
    {
        case EnemyState.Idle:
            Debug.Log("Vào Idle");
            break;

        case EnemyState.Chase:
            Debug.Log("Bắt đầu đuổi Player");
            break;

        case EnemyState.Attack:
            Debug.Log("Bắt đầu Attack");
            break;
    }
}
```

### 3.5. ExitState

```csharp
private void ExitState(EnemyState state)
{
    switch (state)
    {
        case EnemyState.Attack:
            Debug.Log("Rời Attack");
            break;
    }
}
```

### 3.6. Ưu điểm của Enum FSM

- Dễ hiểu với người mới.
- Ít file.
- Dễ debug vì state hiện tại là một enum.
- Phù hợp Player hoặc Enemy chỉ có vài state.
- Làm prototype nhanh.

### 3.7. Nhược điểm của Enum FSM

- `switch-case` dài khi có nhiều state.
- Một script chứa quá nhiều trách nhiệm.
- Thêm state mới phải sửa nhiều switch.
- Logic Enter, Update và Exit dễ nằm xa nhau.
- Khó tái sử dụng state cho đối tượng khác.
- Dễ biến thành một file hàng nghìn dòng.

Enum FSM không phải cách “sai”. Nó rất tốt khi bài toán nhỏ và state đơn giản.

---

## 4. Ví dụ Enum FSM hoàn chỉnh

Ví dụ Enemy 2D có bốn state:

```text
Idle
Chase
Attack
Dead
```

Enemy dùng `Rigidbody2D` Kinematic và `MovePosition` để di chuyển.

```csharp
using UnityEngine;

public class EnemyEnumFSM : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }

    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseRange = 6f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    private Rigidbody2D body;
    private EnemyState currentState;
    private float attackTimer;
    private int health = 3;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentState = EnemyState.Idle;
        EnterState(currentState);
    }

    private void Update()
    {
        UpdateState();
    }

    private void FixedUpdate()
    {
        FixedUpdateState();
    }

    private void UpdateState()
    {
        if (currentState == EnemyState.Dead)
            return;

        float distance = DistanceToPlayer();

        switch (currentState)
        {
            case EnemyState.Idle:
                if (distance <= chaseRange)
                {
                    ChangeState(EnemyState.Chase);
                }
                break;

            case EnemyState.Chase:
                if (distance <= attackRange)
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (distance > chaseRange)
                {
                    ChangeState(EnemyState.Idle);
                }
                break;

            case EnemyState.Attack:
                attackTimer -= Time.deltaTime;

                if (distance > attackRange)
                {
                    ChangeState(EnemyState.Chase);
                }
                else if (attackTimer <= 0f)
                {
                    Attack();
                    attackTimer = attackCooldown;
                }
                break;
        }
    }

    private void FixedUpdateState()
    {
        if (currentState != EnemyState.Chase)
            return;

        Vector2 targetPosition = player.position;

        Vector2 nextPosition = Vector2.MoveTowards(
            body.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        body.MovePosition(nextPosition);
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }

    private void EnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                Debug.Log("Enemy vào Idle");
                break;

            case EnemyState.Chase:
                Debug.Log("Enemy bắt đầu Chase");
                break;

            case EnemyState.Attack:
                attackTimer = 0f;
                Debug.Log("Enemy vào tầm Attack");
                break;

            case EnemyState.Dead:
                body.simulated = false;
                Debug.Log("Enemy đã chết");
                break;
        }
    }

    private void ExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Attack:
                Debug.Log("Enemy rời Attack");
                break;
        }
    }

    private float DistanceToPlayer()
    {
        return Vector2.Distance(
            transform.position,
            player.position
        );
    }

    private void Attack()
    {
        Debug.Log("Enemy tấn công Player");
    }

    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Dead)
            return;

        health -= damage;

        if (health <= 0)
        {
            ChangeState(EnemyState.Dead);
        }
    }
}
```

### 4.1. Cấu hình Inspector

Enemy cần:

- `Rigidbody2D`.
- `Collider2D`.
- Script `EnemyEnumFSM`.
- `Rigidbody2D > Body Type = Kinematic`.
- Kéo Player vào trường `Player`.

### 4.2. Luồng hoạt động

```text
Start
  ↓
Idle
  ↓ Player vào chaseRange
Chase
  ↓ Player vào attackRange
Attack
  ↓ Player chạy xa
Chase hoặc Idle

Health <= 0
  ↓
Dead
```

### 4.3. Vì sao movement nằm trong FixedUpdate?

Enemy dùng `Rigidbody2D.MovePosition`, nên việc di chuyển được thực hiện trong `FixedUpdate`.

Phần ra quyết định có thể ở `Update`:

```text
Update      → nghĩ và đổi state
FixedUpdate → thực hiện chuyển động vật lý
```

---

## 5. Cách 2: State Pattern dùng Interface

### 5.1. Ý tưởng

Thay vì một enum và nhiều switch, mỗi state trở thành một object/class riêng:

```text
IdleState.cs
ChaseState.cs
AttackState.cs
DeadState.cs
```

Tất cả state tuân theo cùng một interface:

```csharp
public interface IState
{
    void Enter();
    void Tick();
    void FixedTick();
    void Exit();
}
```

State Machine không cần biết chi tiết Idle hoặc Chase làm gì. Nó chỉ biết state nào cũng có bốn hàm trên.

### 5.2. Interface là gì?

Interface là một “hợp đồng”. Nó yêu cầu class triển khai các thành viên nhất định.

```csharp
public interface IState
{
    void Enter();
    void Tick();
    void Exit();
}
```

Class triển khai:

```csharp
public class IdleState : IState
{
    public void Enter()
    {
    }

    public void Tick()
    {
    }

    public void Exit()
    {
    }
}
```

Interface thường chỉ nói state **phải có gì**, không giữ field dùng chung và không triển khai logic chung theo cách của abstract base class.

### 5.3. StateMachine class

```csharp
public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState newState)
    {
        if (newState == null || CurrentState == newState)
            return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Tick()
    {
        CurrentState?.Tick();
    }

    public void FixedTick()
    {
        CurrentState?.FixedTick();
    }
}
```

`StateMachine` không chứa logic enemy. Nó chỉ quản lý vòng đời state.

### 5.4. Mỗi state giữ reference tới context

State cần truy cập dữ liệu của Enemy:

```csharp
public class EnemyIdleState : IState
{
    private readonly EnemyInterfaceFSM enemy;

    public EnemyIdleState(EnemyInterfaceFSM enemy)
    {
        this.enemy = enemy;
    }
}
```

`enemy` ở đây thường được gọi là **context** hoặc **owner**: đối tượng mà FSM đang điều khiển.

### 5.5. Tạo state một lần và tái sử dụng

Nên tạo state trong `Awake`:

```csharp
idleState = new EnemyIdleState(this);
chaseState = new EnemyChaseState(this);
```

Sau đó chuyển qua lại giữa những instance đó.

Không nên tạo object mới liên tục mỗi lần chuyển nếu state không cần instance mới:

```csharp
stateMachine.ChangeState(new EnemyChaseState(this));
```

Cách này tạo allocation không cần thiết và làm state data khó quản lý.

### 5.6. Ưu điểm của Interface FSM

- Mỗi state nằm trong class riêng.
- Thêm state mới ít ảnh hưởng state cũ.
- Logic Enter/Tick/Exit nằm gần nhau.
- Dễ tái sử dụng và test từng state.
- File owner/brain gọn hơn.
- Phù hợp AI hoặc Player có nhiều state phức tạp.

### 5.7. Nhược điểm của Interface FSM

- Nhiều class và nhiều file hơn.
- Khó nhìn với người mới nếu bài rất nhỏ.
- Phải truyền reference/context đúng cách.
- Dễ thiết kế quá phức tạp trước khi thật sự cần.
- State có thể phụ thuộc chéo nếu tổ chức không cẩn thận.

---

## 6. Ví dụ Interface FSM hoàn chỉnh

Ví dụ dưới đây giữ toàn bộ class trong một file để dễ đọc. Trong project thật, có thể tách mỗi state ra một file riêng.

Tên file:

```text
EnemyInterfaceFSM.cs
```

```csharp
using UnityEngine;

public class EnemyInterfaceFSM : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseRange = 6f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    private Rigidbody2D body;
    private StateMachine stateMachine;

    private EnemyIdleState idleState;
    private EnemyChaseState chaseState;
    private EnemyAttackState attackState;
    private EnemyDeadState deadState;

    private int health = 3;

    public Transform Player => player;
    public float MoveSpeed => moveSpeed;
    public float ChaseRange => chaseRange;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;

    public float DistanceToPlayer => Vector2.Distance(
        transform.position,
        player.position
    );

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();

        idleState = new EnemyIdleState(this);
        chaseState = new EnemyChaseState(this);
        attackState = new EnemyAttackState(this);
        deadState = new EnemyDeadState(this);
    }

    private void Start()
    {
        ChangeToIdle();
    }

    private void Update()
    {
        stateMachine.Tick();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedTick();
    }

    public void ChangeToIdle()
    {
        stateMachine.ChangeState(idleState);
    }

    public void ChangeToChase()
    {
        stateMachine.ChangeState(chaseState);
    }

    public void ChangeToAttack()
    {
        stateMachine.ChangeState(attackState);
    }

    public void ChangeToDead()
    {
        stateMachine.ChangeState(deadState);
    }

    public void MoveTowardsPlayer()
    {
        Vector2 nextPosition = Vector2.MoveTowards(
            body.position,
            player.position,
            moveSpeed * Time.fixedDeltaTime
        );

        body.MovePosition(nextPosition);
    }

    public void StopPhysics()
    {
        body.simulated = false;
    }

    public void AttackPlayer()
    {
        Debug.Log("Enemy tấn công Player");
    }

    public void TakeDamage(int damage)
    {
        if (stateMachine.CurrentState == deadState)
            return;

        health -= damage;

        if (health <= 0)
        {
            ChangeToDead();
        }
    }
}

public interface IState
{
    void Enter();
    void Tick();
    void FixedTick();
    void Exit();
}

public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState newState)
    {
        if (newState == null || CurrentState == newState)
            return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Tick()
    {
        CurrentState?.Tick();
    }

    public void FixedTick()
    {
        CurrentState?.FixedTick();
    }
}

public class EnemyIdleState : IState
{
    private readonly EnemyInterfaceFSM enemy;

    public EnemyIdleState(EnemyInterfaceFSM enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Vào Idle");
    }

    public void Tick()
    {
        if (enemy.DistanceToPlayer <= enemy.ChaseRange)
        {
            enemy.ChangeToChase();
        }
    }

    public void FixedTick()
    {
    }

    public void Exit()
    {
        Debug.Log("Rời Idle");
    }
}

public class EnemyChaseState : IState
{
    private readonly EnemyInterfaceFSM enemy;

    public EnemyChaseState(EnemyInterfaceFSM enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Vào Chase");
    }

    public void Tick()
    {
        float distance = enemy.DistanceToPlayer;

        if (distance <= enemy.AttackRange)
        {
            enemy.ChangeToAttack();
        }
        else if (distance > enemy.ChaseRange)
        {
            enemy.ChangeToIdle();
        }
    }

    public void FixedTick()
    {
        enemy.MoveTowardsPlayer();
    }

    public void Exit()
    {
        Debug.Log("Rời Chase");
    }
}

public class EnemyAttackState : IState
{
    private readonly EnemyInterfaceFSM enemy;
    private float attackTimer;

    public EnemyAttackState(EnemyInterfaceFSM enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        attackTimer = 0f;
        Debug.Log("Vào Attack");
    }

    public void Tick()
    {
        if (enemy.DistanceToPlayer > enemy.AttackRange)
        {
            enemy.ChangeToChase();
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            enemy.AttackPlayer();
            attackTimer = enemy.AttackCooldown;
        }
    }

    public void FixedTick()
    {
    }

    public void Exit()
    {
        Debug.Log("Rời Attack");
    }
}

public class EnemyDeadState : IState
{
    private readonly EnemyInterfaceFSM enemy;

    public EnemyDeadState(EnemyInterfaceFSM enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.StopPhysics();
        Debug.Log("Vào Dead");
    }

    public void Tick()
    {
    }

    public void FixedTick()
    {
    }

    public void Exit()
    {
    }
}
```

### 6.1. State Machine không phải MonoBehaviour

`StateMachine` và các state ở ví dụ trên là class C# bình thường. Chúng không cần gắn vào GameObject.

Chỉ `EnemyInterfaceFSM` kế thừa `MonoBehaviour` và được gắn lên Enemy.

Điều này có nghĩa:

- State không có `Update()` tự động.
- Owner phải gọi `stateMachine.Tick()` trong `Update`.
- Owner phải gọi `stateMachine.FixedTick()` trong `FixedUpdate`.

### 6.2. Vì sao state gọi ngược lại owner?

Ví dụ:

```csharp
enemy.MoveTowardsPlayer();
```

State quyết định **khi nào** cần di chuyển. Enemy owner biết **di chuyển bằng component nào**.

Nhờ vậy state không phải tự tìm `Rigidbody2D` hoặc tự quản lý mọi component.

### 6.3. Có cần public hết không?

Không. Ví dụ để các class chung một file nên dùng nhiều `public` cho dễ theo dõi. Trong project thật, có thể dùng `internal`, để class mặc định hoặc tổ chức namespace tùy kiến trúc.

---

## 7. Dùng Abstract Class thay Interface

### 7.1. Abstract Class là gì?

Abstract class là class cơ sở không được tạo instance trực tiếp. Nó có thể:

- Bắt class con phải triển khai abstract method.
- Giữ field dùng chung.
- Có constructor.
- Có method đã triển khai sẵn.
- Cho phép class con override khi cần.

Ví dụ:

```csharp
public abstract class EnemyState
{
    protected readonly EnemyInterfaceFSM enemy;

    protected EnemyState(EnemyInterfaceFSM enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Tick();

    public virtual void FixedTick()
    {
    }

    public virtual void Exit()
    {
    }
}
```

State con:

```csharp
public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyInterfaceFSM enemy)
        : base(enemy)
    {
    }

    public override void Enter()
    {
        Debug.Log("Vào Idle");
    }

    public override void Tick()
    {
        if (enemy.DistanceToPlayer <= enemy.ChaseRange)
        {
            enemy.ChangeToChase();
        }
    }
}
```

Vì `FixedTick` và `Exit` đã có implementation rỗng trong base class, state con không bắt buộc override nếu không cần.

### 7.2. StateMachine dùng Abstract State

```csharp
public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }

    public void ChangeState(EnemyState newState)
    {
        if (newState == null || CurrentState == newState)
            return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Tick()
    {
        CurrentState?.Tick();
    }

    public void FixedTick()
    {
        CurrentState?.FixedTick();
    }
}
```

### 7.3. Ưu điểm của Abstract Class

- Chia sẻ owner/context qua field `protected`.
- Có code mặc định cho method không bắt buộc.
- Giảm phần method rỗng.
- Chia sẻ utility cho tất cả state.
- Dễ tạo base state chuyên cho một loại nhân vật.

### 7.4. Nhược điểm của Abstract Class

C# chỉ cho class kế thừa một class cha.

```csharp
public class EnemyIdleState : EnemyState
```

Sau khi đã kế thừa `EnemyState`, nó không thể kế thừa thêm một class khác.

Một class có thể triển khai nhiều interface:

```csharp
public class SomeState : IState, IDamageable, IResettable
```

### 7.5. Có thể kết hợp Interface và Abstract Class

```csharp
public interface IState
{
    void Enter();
    void Tick();
    void FixedTick();
    void Exit();
}

public abstract class EnemyState : IState
{
    protected readonly EnemyInterfaceFSM enemy;

    protected EnemyState(EnemyInterfaceFSM enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Tick();

    public virtual void FixedTick()
    {
    }

    public virtual void Exit()
    {
    }
}
```

Cách này cho phép `StateMachine` làm việc với `IState`, trong khi các Enemy State vẫn được dùng chung logic từ `EnemyState`.

Đây là một thiết kế tốt khi project lớn hơn, nhưng với bài nhỏ có thể chọn riêng Interface hoặc Abstract Class để tránh phức tạp quá sớm.

---

## 8. So sánh Enum, Interface và Abstract Class

| Tiêu chí | Enum + switch | Interface State | Abstract State |
|---|---|---|---|
| Độ dễ học | Dễ nhất | Trung bình | Trung bình |
| Số file | Ít | Nhiều hơn | Nhiều hơn |
| Bài nhỏ | Rất hợp | Có thể hơi dư | Có thể hơi dư |
| Bài lớn | Dễ phình to | Dễ mở rộng | Dễ mở rộng |
| Logic mỗi state | Thường chung một script | Class riêng | Class riêng |
| Code dùng chung | Viết trong owner/helper | Không trực tiếp qua interface | Viết trong base class |
| Đa kế thừa kiểu | Không liên quan | Một class dùng nhiều interface | Chỉ kế thừa một class cha |
| Method mặc định | Không | Tùy phiên bản/ngữ cảnh, thường không dùng cho bài cơ bản | Có virtual method |
| Thêm state mới | Sửa enum và switch | Tạo class mới | Tạo class con mới |

### Chọn nhanh

```text
2–5 state, logic ngắn
→ enum + switch

Nhiều state, mỗi state có logic riêng
→ Interface State Pattern

Nhiều state cùng loại, cần dùng chung owner và method
→ Abstract State
```

---

## 9. FSM và Animator khác nhau thế nào?

Đây là chỗ rất dễ nhầm vì Animator Controller cũng có State Machine.

### Gameplay FSM

Quản lý luật game:

- Player có nhận input không.
- Enemy có đuổi Player không.
- Có được gây damage không.
- Nhân vật còn sống hay đã chết.
- Có được chuyển trạng thái không.

### Animator State Machine

Quản lý hình ảnh animation:

- Phát Idle clip.
- Chuyển sang Run clip.
- Phát Attack clip.
- Blend giữa các animation.

### Cách phối hợp tốt

```text
Gameplay FSM quyết định Enemy chuyển sang Chase
        ↓
ChaseState điều khiển movement
        ↓
ChaseState hoặc animation script đặt Animator Speed
        ↓
Animator phát animation Run
```

Gameplay FSM là nguồn quyết định hành vi. Animator thể hiện hành vi đó bằng hình ảnh.

Ví dụ trong state Attack:

```csharp
public void Enter()
{
    enemy.Animator.SetTrigger("Attack");
}
```

Không nên để việc Animator đang ở state gì là nguồn dữ liệu duy nhất quyết định Enemy còn sống hay được phép gây damage.

---

## 10. Cách tổ chức transition

Có hai hướng chính.

### 10.1. State tự quyết định transition

```csharp
public void Tick()
{
    if (enemy.DistanceToPlayer <= enemy.AttackRange)
    {
        enemy.ChangeToAttack();
    }
}
```

Ưu điểm:

- Logic của state nằm cùng một chỗ.
- Dễ đọc với FSM nhỏ và vừa.

Nhược điểm:

- State cần biết state đích hoặc biết method chuyển state trên owner.
- Nhiều transition toàn cục như Dead có thể lặp ở nhiều state.

### 10.2. Owner hoặc State Machine kiểm tra transition

State chỉ xử lý hành vi; owner kiểm tra điều kiện chung.

```csharp
private void Update()
{
    if (health <= 0)
    {
        ChangeToDead();
        return;
    }

    stateMachine.Tick();
}
```

Ưu điểm:

- Điều kiện toàn cục như Dead, Stunned được xử lý một chỗ.
- State bớt phụ thuộc lẫn nhau.

Nhược điểm:

- Logic state và transition có thể nằm ở nhiều nơi.

### 10.3. Cách thực dụng

Có thể kết hợp:

- Transition riêng của state nằm trong state.
- Transition toàn cục nằm trong owner.

Ví dụ:

```text
Chase → Attack: ChaseState kiểm tra
Attack → Chase: AttackState kiểm tra
Any State → Dead: Enemy owner kiểm tra health
```

### 10.4. Return ngay sau khi đổi state

```csharp
if (enemy.DistanceToPlayer > enemy.AttackRange)
{
    enemy.ChangeToChase();
    return;
}
```

Sau khi đổi state, code còn lại của state cũ không nên tiếp tục chạy trong cùng Tick. `return` giúp luồng rõ ràng và tránh state cũ vừa chuyển đi lại tiếp tục tấn công.

### 10.5. Không đổi về cùng state liên tục

Trong `ChangeState`:

```csharp
if (CurrentState == newState)
    return;
```

Nếu không có kiểm tra này, state có thể Exit rồi Enter lại mỗi frame, liên tục reset timer hoặc animation.

---

## 11. Lỗi thường gặp

### 11.1. Chỉ gán state mà không gọi Enter/Exit

```csharp
currentState = EnemyState.Attack;
```

Cách này bỏ qua logic dọn state cũ và chuẩn bị state mới.

Nên dùng:

```csharp
ChangeState(EnemyState.Attack);
```

### 11.2. Gọi Enter mỗi frame

Sai:

```csharp
private void Update()
{
    EnterAttack();
    UpdateAttack();
}
```

Enter chỉ nên chạy một lần lúc vừa chuyển state.

### 11.3. Quên state ban đầu

Nếu không gọi state đầu tiên:

```csharp
stateMachine.ChangeState(idleState);
```

thì `CurrentState` là null và không có gì chạy.

### 11.4. Tạo state mới mỗi frame

Sai:

```csharp
stateMachine.ChangeState(new EnemyChaseState(this));
```

nếu đoạn này bị gọi lặp lại thường xuyên.

Nên tạo một lần trong Awake rồi tái sử dụng.

### 11.5. State cũ tiếp tục chạy sau transition

```csharp
if (shouldLeave)
{
    ChangeState(otherState);
}

DoOldStateAction();
```

Thêm `return` sau `ChangeState` nếu phần còn lại không được phép chạy.

### 11.6. Hai state cùng điều khiển một dữ liệu

Ví dụ movement code nằm cả trong owner lẫn state, khiến velocity bị ghi hai lần.

Hãy quy định rõ:

```text
State quyết định hành vi
Owner cung cấp API thực thi
```

hoặc một quy tắc nhất quán khác.

### 11.7. Nhét mọi logic vào Enter

Enter chỉ chạy một lần. Logic cần lặp theo thời gian phải nằm trong Tick hoặc coroutine được quản lý rõ.

### 11.8. Coroutine từ state cũ vẫn chạy

Nếu state bắt đầu coroutine rồi rời state, coroutine không tự biết phải dừng.

Cần lưu handle và dừng trong Exit nếu thiết kế yêu cầu:

```csharp
public void Exit()
{
    enemy.StopAttackCoroutine();
}
```

### 11.9. Event vẫn còn đăng ký sau khi rời state

Nếu Enter đăng ký event:

```csharp
enemy.OnDamaged += HandleDamage;
```

thì Exit nên hủy:

```csharp
enemy.OnDamaged -= HandleDamage;
```

### 11.10. State biết quá nhiều

Nếu một state trực tiếp truy cập mọi component, UI, audio, scene manager và object khác, nó trở nên khó tái sử dụng.

Cho owner cung cấp các method có ý nghĩa:

```csharp
enemy.MoveTowardsPlayer();
enemy.PlayAttackAnimation();
enemy.DealDamage();
```

### 11.11. FSM quá phức tạp cho bài nhỏ

Không cần 20 file để quản lý một chiếc cửa có hai state Open và Closed. `enum + switch` hoặc thậm chí một Bool có thể đủ.

Mục tiêu của kiến trúc là làm code dễ hiểu hơn, không phải làm nhiều class hơn.

### 11.12. Quên xử lý reference null

Nếu Player chưa được gán:

```csharp
Vector2.Distance(transform.position, player.position)
```

sẽ gây `NullReferenceException`.

Trong bài thực tế, kiểm tra Inspector hoặc thêm cảnh báo trong Awake:

```csharp
if (player == null)
{
    Debug.LogError("Chưa gán Player cho Enemy", this);
}
```

---

## 12. Chọn cách nào cho bài của mình?

### Dùng Enum FSM nếu

- Đây là lần đầu học FSM.
- Đối tượng có ít state.
- Logic mỗi state ngắn.
- Cần làm prototype nhanh.
- Muốn nhìn toàn bộ luồng trong một file.

Ví dụ:

- Cửa Open/Closed.
- Game Playing/Paused/GameOver.
- Enemy Idle/Chase/Attack cơ bản.

### Dùng Interface FSM nếu

- Có nhiều loại state.
- Mỗi state có nhiều logic riêng.
- Muốn mỗi state nằm trong class riêng.
- Muốn StateMachine làm việc với nhiều nhóm state khác nhau.
- Cần test hoặc tái sử dụng state.

### Dùng Abstract Class nếu

- Các state cùng loại có nhiều field/method dùng chung.
- Muốn base class giữ owner.
- Muốn có method mặc định rỗng.
- Muốn các state kế thừa utility chung.

### Lộ trình học gợi ý

```text
Bước 1: enum + switch-case
        ↓
Hiểu State, Transition, Enter, Update, Exit
        ↓
Bước 2: tách IState và StateMachine
        ↓
Hiểu State Pattern
        ↓
Bước 3: dùng abstract base state khi thấy code lặp
```

Không cần nhảy thẳng vào kiến trúc phức tạp khi chưa thấy vấn đề mà nó giải quyết.

---

## 13. Bảng ghi nhớ nhanh

### Khái niệm

| Khái niệm | Ý nghĩa |
|---|---|
| State | Trạng thái hiện tại |
| Transition | Chuyển từ state này sang state khác |
| Condition | Điều kiện cho phép transition |
| Enter | Chạy một lần khi vào state |
| Tick/Update | Chạy lặp khi state active |
| Exit | Chạy một lần khi rời state |
| Context/Owner | Đối tượng được FSM điều khiển |

### Enum FSM

```csharp
public enum PlayerState
{
    Idle,
    Run,
    Jump
}

private PlayerState currentState;
```

```csharp
switch (currentState)
{
    case PlayerState.Idle:
        UpdateIdle();
        break;

    case PlayerState.Run:
        UpdateRun();
        break;
}
```

### Interface FSM

```csharp
public interface IState
{
    void Enter();
    void Tick();
    void FixedTick();
    void Exit();
}
```

### Abstract State

```csharp
public abstract class BaseState
{
    public abstract void Enter();
    public abstract void Tick();

    public virtual void Exit()
    {
    }
}
```

### ChangeState chuẩn

```csharp
public void ChangeState(IState newState)
{
    if (newState == null || CurrentState == newState)
        return;

    CurrentState?.Exit();
    CurrentState = newState;
    CurrentState.Enter();
}
```

### Câu chốt để nhớ

```text
FSM = State + Transition + Condition.

Enum FSM gom state trong enum và xử lý bằng switch-case.

State Pattern biến mỗi state thành một object riêng.

Interface quy định state phải làm được gì.

Abstract class vừa quy định cấu trúc,
vừa chia sẻ field và code dùng chung.

FSM gameplay quyết định hành vi.
Animator thể hiện hành vi bằng animation.
```
