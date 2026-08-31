# Ghi chú ôn bài: Unity 2D

> **Nội dung:** New Input System, Physics 2D, va chạm, Rigidbody2D, Trigger, Raycast 2D, Layer Mask và các cách di chuyển nhân vật.
>
> **Mục đích:** Viết để hiểu và có thể trình bày lại với thầy, không đi quá sâu vào công thức hay kiến trúc dự án lớn.

## 1. New Input System

New Input System là cách Unity nhận thao tác từ bàn phím, chuột hoặc tay cầm. Điểm dễ hiểu nhất là: mình đặt tên cho hành động trước, rồi mới gán phím cho nó.

| Thứ cần nhớ | Hiểu đơn giản |
|---|---|
| Action Map | Một nhóm hành động, ví dụ `Gameplay` hoặc `UI`. |
| Action | Việc người chơi muốn làm, ví dụ `Move`, `Jump`, `Attack`. |
| Binding | Phím hoặc nút dùng để thực hiện Action, ví dụ WASD hoặc Space. |

### Cách tạo nhanh

1. Mở **Window > Package Manager** và cài **Input System**.
2. Tạo một **Input Actions** trong cửa sổ Project.
3. Tạo Action Map tên `Gameplay`.
4. Tạo `Move` kiểu **Value/Vector2** và `Jump` kiểu **Button**.
5. Gán WASD cho `Move`, Space cho `Jump`.

> **Nói ngắn gọn:** `Move` trả về `Vector2` để biết hướng; `Jump` chỉ cần biết nút có được nhấn hay không.

### Ví dụ đọc input

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    private Vector2 moveInput;
    private bool jumpPressed;

    private void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        jumpPressed = jumpAction.action.WasPressedThisFrame();
    }
}
```

> **Quan trọng:** Đọc input trong `Update`. Nếu dùng `Rigidbody2D` thì áp dụng chuyển động trong `FixedUpdate`.

## 2. Physics 2D

Physics 2D là hệ thống giúp vật thể rơi, va chạm, trượt, nảy và bị lực tác động. Hai component thường đi cùng nhau là `Rigidbody2D` và `Collider2D`.

- `Rigidbody2D`: quản lý chuyển động vật lý như vận tốc, trọng lực và khối lượng.
- `Collider2D`: tạo hình va chạm. Sprite nhìn thấy được không có nghĩa là đã có collider.
- `Physics Material 2D`: chỉnh ma sát và độ nảy.

> **Dễ nhầm:** `Rigidbody2D` không tự tạo hình va chạm. Muốn chạm được vật khác thì vẫn cần `Collider2D`.

### Update và FixedUpdate

`Update` chạy theo từng khung hình. `FixedUpdate` chạy theo nhịp vật lý. Vì vậy input thường được đọc trong `Update`, còn `linearVelocity`, `AddForce` hoặc `MovePosition` nên được xử lý trong `FixedUpdate`.

```csharp
private void Update()
{
    moveInput = moveAction.action.ReadValue<Vector2>();
}

private void FixedUpdate()
{
    rb.linearVelocity = new Vector2(
        moveInput.x * speed,
        rb.linearVelocity.y
    );
}
```

## 3. Va chạm và điều kiện để va chạm

Va chạm xảy ra khi hai `Collider2D` chạm nhau và hệ thống Physics 2D cho phép chúng tương tác.

### Điều kiện cần kiểm tra

- Cả hai vật đều có `Collider2D`.
- Ít nhất một bên có `Rigidbody2D`.
- Hai collider đang chạm nhau thật, không chỉ có hai sprite chồng lên nhau.
- Cả hai đang dùng hệ 2D, không trộn Collider 3D với `Rigidbody2D`.
- Layer Collision Matrix không chặn cặp layer đó.
- Nếu muốn va chạm bình thường thì **Is Trigger** phải tắt.

### Các hàm va chạm

- `OnCollisionEnter2D`: gọi khi vừa bắt đầu chạm.
- `OnCollisionStay2D`: gọi khi vẫn đang chạm.
- `OnCollisionExit2D`: gọi khi rời nhau.

```csharp
private void OnCollisionEnter2D(Collision2D other)
{
    if (other.gameObject.CompareTag("Enemy"))
        Debug.Log("Player chạm Enemy");
}
```

## 4. Rigidbody2D: Dynamic, Kinematic và Static

| Loại | Hiểu đơn giản | Ví dụ |
|---|---|---|
| Dynamic | Bị trọng lực, lực và va chạm tác động. | Player, quái, thùng, bóng |
| Kinematic | Code tự điều khiển; không bị trọng lực đẩy. | Platform di động, cửa |
| Static | Đứng yên, dùng làm môi trường. | Nền, tường |

> **Cách chọn:** Vật cần phản ứng vật lý thì dùng **Dynamic**. Vật di chuyển theo đường viết sẵn thì dùng **Kinematic**. Vật không di chuyển thì dùng **Static**.

### Lưu ý

- Không nên đổi `transform.position` liên tục cho `Rigidbody2D` Dynamic.
- Dynamic thường va chạm được với Dynamic, Kinematic và Static.
- Kinematic mặc định chủ yếu tương tác với Dynamic.
- Platform di động nên dùng Kinematic kết hợp `MovePosition` trong `FixedUpdate`.

## 5. Trigger

Trigger vẫn phát hiện hai collider đi vào nhau nhưng không đẩy chúng ra. Vì thế hai vật có thể đi xuyên qua nhau.

- Bật **Is Trigger** trên `Collider2D`.
- Thường vẫn cần ít nhất một `Rigidbody2D`.
- Dùng cho nhặt đồ, checkpoint, vùng gây sát thương hoặc cửa dịch chuyển.

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Player đã nhặt vật phẩm");
        gameObject.SetActive(false);
    }
}
```

> **Phân biệt:** Collision có phản lực vật lý; Trigger chỉ báo rằng hai vùng đang chồng lên nhau.

## 6. Raycast 2D

Raycast 2D giống như bắn một tia vô hình từ một điểm theo một hướng. Nếu tia gặp `Collider2D`, Unity trả về vật bị trúng và vị trí trúng.

```csharp
RaycastHit2D hit = Physics2D.Raycast(
    transform.position,
    Vector2.down,
    0.2f,
    groundMask
);

bool isGrounded = hit.collider != null;
```

- `Origin`: điểm bắt đầu bắn tia.
- `Direction`: hướng bắn, ví dụ `Vector2.down`.
- `Distance`: tia dài bao nhiêu.
- `Layer Mask`: tia được phép nhìn thấy những layer nào.

**Ứng dụng:** kiểm tra chạm đất, nhìn thấy kẻ địch, bắn súng hoặc kiểm tra tường trước mặt.

> **Lỗi hay gặp:** Dùng nhầm `Physics.Raycast` của 3D thay vì `Physics2D.Raycast`.

## 7. Layer Mask

Layer dùng để chia GameObject thành các nhóm. Layer Mask dùng để chọn một hoặc nhiều nhóm mà phép kiểm tra vật lý cần quan tâm.

- Ví dụ `groundMask` chỉ chọn layer `Ground`, nên ray kiểm tra chân không trúng Enemy hoặc Player.
- Layer Collision Matrix quyết định layer nào được va chạm với layer nào.
- Tag dùng để nhận biết vai trò như Player hoặc Enemy; Layer thường dùng để lọc vật lý và camera.

```csharp
[SerializeField] private LayerMask groundMask;

// Hoặc tạo mask bằng tên layer:
int mask = LayerMask.GetMask("Ground", "Wall");
```

> **Nhớ:** Layer Mask là một tập layer, không phải một số layer đơn lẻ. Nên chọn mask trong Inspector cho dễ nhìn.

## 8. Các cách di chuyển nhân vật

### Cách 1: đổi Transform

Đơn giản nhưng không phù hợp khi Player dùng `Rigidbody2D` Dynamic, vì có thể làm xuyên tường hoặc bỏ qua xử lý va chạm.

### Cách 2: đặt linearVelocity

Dễ điều khiển, phản hồi nhanh và vẫn dùng được va chạm vật lý. Đây là cách dễ dùng cho platformer.

```csharp
rb.linearVelocity = new Vector2(
    moveInput.x * moveSpeed,
    rb.linearVelocity.y
);
```

### Cách 3: MovePosition

Phù hợp với `Rigidbody2D` Kinematic như platform di động hoặc NPC đi theo đường cố định.

```csharp
Vector2 next = rb.position
    + direction * speed * Time.fixedDeltaTime;

rb.MovePosition(next);
```

### Cách 4: AddForce

Tạo cảm giác có quán tính và tự nhiên hơn, nhưng khó dừng chính xác. Hợp với bóng, tàu hoặc game thiên về vật lý.

```csharp
rb.AddForce(moveInput * force, ForceMode2D.Force);
```

### Chọn nhanh

| Trường hợp | Nên dùng |
|---|---|
| Player platformer | Dynamic + `linearVelocity` |
| Platform di động | Kinematic + `MovePosition` |
| Bóng hoặc tàu có quán tính | Dynamic + `AddForce` |
| Vật không cần physics | Transform |

## 9. Ví dụ Player 2D ngắn gọn

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class SimplePlayer2D : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpSpeed = 11f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    private void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        jumpPressed |= jumpAction.action.WasPressedThisFrame();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            moveInput.x * speed,
            rb.linearVelocity.y
        );

        bool grounded = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            0.15f,
            groundMask
        );

        if (grounded && jumpPressed)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpSpeed
            );
        }

        jumpPressed = false;
    }
}
```

> **Ý chính:** Input tạo ra hướng di chuyển; `Rigidbody2D` thực hiện chuyển động; Raycast kiểm tra chân có chạm Ground hay không.

## 10. Tóm tắt để trình bày lại

- New Input System biến phím bấm thành các Action như `Move` và `Jump`.
- `Rigidbody2D` quản lý chuyển động vật lý; `Collider2D` tạo hình va chạm.
- Muốn có callback va chạm: hai `Collider2D`, ít nhất một `Rigidbody2D` và layer không bị chặn.
- Dynamic chịu vật lý; Kinematic do code điều khiển; Static đứng yên.
- Trigger phát hiện đi vào vùng nhưng không tạo phản lực.
- Raycast bắn tia để kiểm tra vật ở một hướng; Layer Mask giúp lọc vật cần kiểm tra.
- Player thường dùng `linearVelocity`; platform dùng `MovePosition`; vật có quán tính dùng `AddForce`.

> **Một câu chốt:** Input nói người chơi muốn làm gì; Physics 2D quyết định nhân vật di chuyển và va chạm như thế nào.
