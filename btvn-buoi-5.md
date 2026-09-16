# Animator Controller, Animator Component, Animation Clip, Blend Tree và Finite State Machine

## Mục lục

1. [Bức tranh tổng thể](#1-bức-tranh-tổng-thể)
2. [Animation Clip](#2-animation-clip)
3. [Animator Component](#3-animator-component)
4. [Animator Controller](#4-animator-controller)
5. [State và Transition](#5-state-và-transition)
6. [Animator Parameters](#6-animator-parameters)
7. [Blend Tree](#7-blend-tree)
8. [Finite State Machine](#8-finite-state-machine)
9. [Kết nối Animator với code](#9-kết-nối-animator-với-code)
10. [Ví dụ Animator cho Player 2D](#10-ví-dụ-animator-cho-player-2d)
11. [Quy trình tạo Animator từ đầu](#11-quy-trình-tạo-animator-từ-đầu)
12. [Lỗi thường gặp](#12-lỗi-thường-gặp)
13. [Tối ưu và tổ chức Controller](#13-tối-ưu-và-tổ-chức-controller)
14. [Bảng ghi nhớ nhanh](#14-bảng-ghi-nhớ-nhanh)

---

## 1. Bức tranh tổng thể

Hệ animation của Unity có nhiều phần với tên khá giống nhau. Có thể hình dung chúng như sau:

```text
Animation Clip
    │
    │ chứa chuyển động/hình ảnh theo thời gian
    ▼
State trong Animator Controller
    │
    │ kết nối bằng Transition và điều khiển bằng Parameter
    ▼
Animator Controller
    │
    │ được gán vào
    ▼
Animator Component trên GameObject
    │
    ▼
GameObject phát animation trong Scene
```

Ví dụ với Player 2D:

- `Player_Idle.anim`: các frame nhân vật đứng yên.
- `Player_Run.anim`: các frame nhân vật chạy.
- `Player_Jump.anim`: các frame nhân vật nhảy.
- `Player.controller`: chứa state Idle, Run, Jump và luật chuyển qua lại.
- `Animator` trên Player: chạy `Player.controller` trong game.
- Script Player: gửi tốc độ, trạng thái chạm đất hoặc lệnh tấn công vào Animator.

### Mỗi phần trả lời một câu hỏi

| Thành phần | Câu hỏi nó trả lời |
|---|---|
| Animation Clip | “Hình ảnh hoặc thuộc tính thay đổi thế nào theo thời gian?” |
| Animator Component | “GameObject nào đang chạy animation?” |
| Animator Controller | “Có những animation nào và chuyển giữa chúng ra sao?” |
| Blend Tree | “Trộn nhiều animation liên tục theo một hoặc hai giá trị thế nào?” |
| Finite State Machine | “Ở mỗi thời điểm hệ thống đang ở trạng thái nào và khi nào đổi trạng thái?” |

Điểm dễ nhầm nhất:

> Animator Component không chứa toàn bộ logic state. Nó là component chạy Animator Controller trên một GameObject.

---

## 2. Animation Clip

### 2.1. Animation Clip là gì?

Animation Clip là đơn vị dữ liệu animation nhỏ nhất trong Unity. Nó lưu cách một hoặc nhiều thuộc tính thay đổi theo thời gian.

Một clip có thể lưu:

- Vị trí, góc quay và scale của Transform.
- Frame sprite của `SpriteRenderer`.
- Giá trị màu.
- Trạng thái bật/tắt của một thuộc tính có thể animate.
- Chuyển động xương của nhân vật.
- Những curve số khác.

Ví dụ:

```text
Player_Idle.anim
Player_Run.anim
Player_Jump.anim
Player_Attack.anim
Player_Hurt.anim
```

Mỗi file `.anim` thường đại diện cho một hành động hoặc một đoạn chuyển động.

### 2.2. Clip 2D hoạt động thế nào?

Với sprite animation, clip thường thay đổi thuộc tính:

```text
SpriteRenderer.sprite
```

theo thời gian:

```text
0.00s → Idle_0
0.10s → Idle_1
0.20s → Idle_2
0.30s → Idle_3
```

Khi chạy, Unity thay sprite liên tục nên mắt mình thấy nhân vật đang chuyển động.

### 2.3. Tạo Animation Clip 2D nhanh

1. Chọn các sprite frame trong cửa sổ Project.
2. Kéo chúng vào Scene hoặc cửa sổ Animation.
3. Unity hỏi nơi lưu file `.anim`.
4. Đặt tên rõ ràng, ví dụ `Player_Run.anim`.
5. Unity có thể tự tạo thêm Animator Controller nếu GameObject chưa có.

Cũng có thể:

1. Chọn Player trong Hierarchy.
2. Mở `Window > Animation > Animation`.
3. Nhấn `Create`.
4. Lưu clip.
5. Kéo các sprite vào timeline.

### 2.4. Samples là gì?

`Samples` là số mẫu animation trong một giây. Với sprite animation, có thể hiểu gần giống số frame được phát mỗi giây.

Ví dụ clip có 6 sprite:

- Samples = 6: chu kỳ gần một giây.
- Samples = 12: chu kỳ nhanh khoảng nửa giây.
- Samples càng cao thì clip chạy càng nhanh nếu khoảng cách giữa các key không đổi.

Không có một con số đúng cho mọi game. Pixel art thường dùng tốc độ vừa phải để nhìn rõ từng frame.

### 2.5. Loop Time

Bật `Loop Time` nếu clip cần lặp lại:

- Idle.
- Walk.
- Run.
- Bay liên tục.

Thường tắt loop với hành động chỉ phát một lần:

- Attack.
- Hurt.
- Death.
- Open Door.

Nếu Attack bật loop nhưng không có transition thoát hợp lý, nhân vật có thể đánh mãi không dừng.

### 2.6. Loop Pose

`Loop Pose` giúp đầu và cuối chu kỳ khớp mượt hơn, đặc biệt hữu ích với animation rig/humanoid. Với sprite frame-by-frame, chất lượng vòng lặp chủ yếu phụ thuộc vào chính các frame đã vẽ và thứ tự của chúng.

### 2.7. Animation Event

Có thể đặt Animation Event trên timeline để gọi một hàm tại đúng thời điểm trong clip.

Ví dụ Attack:

```text
Frame vung kiếm bắt đầu
        ↓
Frame kiếm chạm mục tiêu → Animation Event gọi EnableHitbox()
        ↓
Frame vung xong → gọi DisableHitbox()
```

Hàm nhận event phải nằm trên một component phù hợp của GameObject chạy animation:

```csharp
public void EnableHitbox()
{
    attackHitbox.SetActive(true);
}

public void DisableHitbox()
{
    attackHitbox.SetActive(false);
}
```

Animation Event tiện cho thời điểm gắn chặt với hình ảnh. Tuy nhiên, không nên nhét toàn bộ logic game quan trọng vào event mà không có kiểm soát, vì đổi clip hoặc xóa event có thể làm gameplay hỏng khó tìm.

### 2.8. Clip không tự quyết định khi nào được phát

Animation Clip chỉ chứa dữ liệu animation. Nó không tự biết:

- Player có đang chạy không.
- Player có chạm đất không.
- Quái đã chết chưa.
- Có được phép chuyển sang Attack không.

Những quyết định đó thuộc Animator Controller, Transition, Parameters hoặc code gameplay.

---

## 3. Animator Component

### 3.1. Animator Component là gì?

Animator là component gắn trên GameObject trong Scene hoặc Prefab. Nó kết nối GameObject với Animator Controller và chạy animation lúc game hoạt động.

Có thể thêm bằng:

```text
Add Component > Animator
```

Với Player 2D:

```text
Player
├── Rigidbody2D
├── Collider2D
├── SpriteRenderer
├── Animator
└── PlayerMovement script
```

### 3.2. Controller

Trường `Controller` nhận một Animator Controller asset có đuôi `.controller`.

```text
Player Animator
└── Controller: Player.controller
```

Nếu Animator không có Controller:

- Không có state machine để chạy.
- Các clip trong Project không tự phát trên object.
- Gọi parameter bằng code không tạo được hiệu ứng mong muốn.

### 3.3. Avatar

`Avatar` chủ yếu dùng cho nhân vật rig, đặc biệt humanoid, để ánh xạ hệ xương.

Với sprite animation 2D thông thường:

```text
Avatar = None
```

Đây là bình thường, không phải lỗi.

### 3.4. Apply Root Motion

Root Motion nghĩa là vị trí hoặc góc quay của cả nhân vật được lấy từ chuyển động đã ghi trong animation.

Nếu bật `Apply Root Motion`:

- Animation có thể điều khiển việc nhân vật tiến về phía trước.
- Hợp với một số nhân vật 3D có animation chứa chuyển động gốc.

Với platformer 2D điều khiển bằng `Rigidbody2D`, thường để:

```text
Apply Root Motion = Off
```

Khi đó code gameplay chịu trách nhiệm di chuyển, còn Animator chỉ thay đổi hình ảnh.

Không nên vừa cho clip thay đổi vị trí Player, vừa cho Rigidbody2D thay đổi vị trí mà không có chủ ý. Hai hệ cùng điều khiển Transform có thể gây trượt, giật hoặc va chạm khó đoán.

### 3.5. Update Mode

#### Normal

Animator cập nhật theo nhịp `Update` và chịu ảnh hưởng của `Time.timeScale`.

Đây là lựa chọn mặc định, phù hợp với phần lớn animation nhân vật và object thông thường.

#### Animate Physics

Animator cập nhật đồng bộ hơn với nhịp vật lý. Dùng khi animation cần liên hệ chặt với physics interaction.

Không phải cứ Player có `Rigidbody2D` là bắt buộc chọn Animate Physics. Nhiều game 2D vẫn dùng Normal cho hình ảnh và xử lý vật lý trong `FixedUpdate`.

#### Unscaled Time

Animator bỏ qua `Time.timeScale`.

Hợp với:

- UI animation trong lúc pause.
- Loading animation.
- Hiệu ứng muốn chạy ngay cả khi game time dừng.

### 3.6. Culling Mode

Culling Mode quyết định Animator làm gì khi Renderer không được nhìn thấy.

#### Always Animate

Animation vẫn được cập nhật đầy đủ khi object ngoài màn hình.

Ưu điểm: trạng thái và Transform vẫn tiếp tục chính xác.

Nhược điểm: tốn thêm công việc xử lý.

#### Cull Update Transforms

Giảm một phần cập nhật Transform/IK khi không nhìn thấy.

#### Cull Completely

Ngừng cập nhật animation khi không nhìn thấy.

Cần cẩn thận với object gameplay quan trọng. Nếu animation hoặc StateMachineBehaviour đang dùng để phát một logic nào đó, culling có thể làm hành vi ngoài màn hình khác mong đợi.

### 3.7. Animator trong code

Lấy component:

```csharp
private Animator animator;

private void Awake()
{
    animator = GetComponent<Animator>();
}
```

Sau đó có thể gửi parameter:

```csharp
animator.SetFloat("Speed", 2f);
animator.SetBool("IsGrounded", true);
animator.SetTrigger("Attack");
```

Animator Component là object runtime mà script giao tiếp. Animator Controller là asset chứa sơ đồ mà component đó đang chạy.

---

## 4. Animator Controller

### 4.1. Animator Controller là gì?

Animator Controller là asset chứa:

- Các state.
- Motion của mỗi state.
- Transition giữa các state.
- Parameters.
- Layers.
- Sub-State Machine.

Nó thường có đuôi:

```text
.controller
```

Tạo bằng:

```text
Project > Create > Animator Controller
```

Mở bằng cách double-click asset hoặc vào:

```text
Window > Animation > Animator
```

### 4.2. State trong Controller

Mỗi state đại diện cho một trạng thái animation:

```text
Idle
Run
Jump
Fall
Attack
Hurt
Death
```

Một state có trường `Motion`. Motion có thể là:

- Một Animation Clip.
- Một Blend Tree.
- Một Blend Tree lồng bên trong Blend Tree khác.

### 4.3. Default State

State màu cam là default state. Khi Animator bắt đầu chạy layer đó, nó đi từ `Entry` vào default state nếu không có nhánh Entry khác phù hợp.

Đặt default state:

```text
Chuột phải vào state > Set as Layer Default State
```

Với Player, default thường là `Idle` hoặc state `Locomotion` chứa Blend Tree.

### 4.4. Entry

`Entry` là điểm đi vào State Machine.

Thông thường:

```text
Entry → Default State
```

Trong state machine phức tạp, Entry có thể rẽ sang state khác dựa trên điều kiện.

### 4.5. Any State

`Any State` đại diện cho khả năng chuyển từ hầu hết state hiện tại sang một state đích.

Ví dụ:

```text
Any State -- Hurt Trigger --> Hurt
Any State -- IsDead = true --> Death
```

Nó hữu ích khi Hurt hoặc Death có thể xảy ra trong nhiều trạng thái.

Không nên dùng Any State cho mọi thứ. Quá nhiều transition từ Any State có thể:

- Khó đọc sơ đồ.
- Tự chuyển lại chính state đó.
- Cắt ngang animation ngoài ý muốn.
- Tạo xung đột ưu tiên transition.

### 4.6. Exit

`Exit` thường có ý nghĩa rõ hơn trong Sub-State Machine. Khi state bên trong chuyển tới Exit, luồng thoát khỏi state machine con để quay về cấp cha theo transition đã thiết kế.

### 4.7. Sub-State Machine

Khi Controller lớn, có thể nhóm state:

```text
Base Layer
├── Locomotion
│   ├── Idle
│   ├── Walk
│   └── Run
├── Airborne
│   ├── Jump
│   └── Fall
└── Combat
    ├── Attack1
    ├── Attack2
    └── Hurt
```

Sub-State Machine giúp sơ đồ gọn hơn. Nó không tự làm logic tốt hơn; vẫn cần transition và parameter rõ ràng.

### 4.8. Animator Layers

Animator Controller có thể có nhiều layer.

Ví dụ:

- Base Layer: chạy, nhảy, đứng.
- Upper Body Layer: cầm súng, ngắm, bắn.

Layer có weight và có thể dùng Avatar Mask để chỉ tác động lên một phần cơ thể. Đây là kỹ thuật hữu ích cho nhân vật rig. Với sprite 2D đơn giản, thường chỉ cần một layer cho tới khi thật sự có lý do tách.

---

## 5. State và Transition

### 5.1. Transition là gì?

Transition là đường nối và luật chuyển từ state này sang state khác.

Ví dụ:

```text
Idle -- Speed > 0.1 --> Run
Run  -- Speed < 0.1 --> Idle
```

Transition quyết định:

- Khi nào được chuyển.
- Mất bao lâu để blend giữa hai state.
- Có cần chờ clip hiện tại chạy tới một mốc nào đó không.
- Transition có thể bị ngắt bởi transition khác không.

### 5.2. Tạo Transition

```text
Chuột phải state nguồn
→ Make Transition
→ Click state đích
```

Ví dụ cần cả hai chiều:

```text
Idle → Run
Run → Idle
```

Transition chỉ có một chiều. Tạo mũi tên Idle → Run không tự sinh mũi tên Run → Idle.

### 5.3. Conditions

Conditions dùng Animator Parameters để quyết định chuyển state.

Ví dụ:

```text
Parameter: Speed
Condition: Greater 0.1
```

Nếu transition có nhiều condition, tất cả phải đúng thì transition mới đủ điều kiện.

Ví dụ Jump → Fall:

```text
IsGrounded = false
VerticalSpeed < 0
```

### 5.4. Has Exit Time

`Has Exit Time` yêu cầu state hiện tại phát tới một mốc thời gian nhất định trước khi transition xảy ra.

Exit Time dùng normalized time:

```text
0.0  = đầu clip
0.5  = nửa clip
1.0  = hết một vòng clip
```

Ví dụ:

```text
Exit Time = 0.75
```

nghĩa là transition chỉ có thể xảy ra khi clip đã đi tới khoảng 75% chu kỳ.

Hợp với:

- Attack phát gần xong rồi mới trở về Idle.
- Hurt cần hiện đủ phản ứng.
- Animation mở cửa cần chạy tới cuối.

Thường không hợp với chuyển động cần phản hồi ngay:

- Idle sang Run khi người chơi bắt đầu di chuyển.
- Run sang Idle khi thả phím.
- Rơi xuống vực.

Nếu bật Has Exit Time cho Idle → Run, nhân vật có thể nhấn chạy nhưng hình ảnh phản hồi trễ.

### 5.5. Exit Time kết hợp Condition

Nếu transition vừa bật Has Exit Time vừa có condition, Unity cần cả hai:

```text
Đã tới Exit Time
VÀ
Condition đang đúng
```

Đây là nguyên nhân phổ biến khiến parameter đã đổi nhưng state chưa chuyển ngay.

### 5.6. Transition Duration

Transition Duration quyết định thời gian blend giữa source và destination.

Với animation 3D mượt, blend có thể làm chuyển động tự nhiên hơn.

Với pixel art 2D, thời gian blend lớn đôi khi tạo cảm giác mờ hoặc chuyển frame kỳ lạ. Có thể dùng duration rất nhỏ hoặc bằng 0 nếu muốn đổi clip dứt khoát.

`Fixed Duration` quyết định Duration được hiểu theo:

- Giây, nếu bật.
- Tỉ lệ normalized time của state nguồn, nếu tắt.

### 5.7. Transition Offset

Offset quyết định state đích bắt đầu từ vị trí nào trong clip.

Ví dụ offset `0.5` có thể khiến state đích bắt đầu ở nửa clip. Phần lớn trường hợp cơ bản để 0.

### 5.8. Self Transition

Transition từ một state quay lại chính nó có thể khởi động lại animation. Hãy cẩn thận với Any State → Attack/Hurt. Nếu cho phép tự transition, việc gọi trigger liên tục có thể restart clip liên tục và animation không bao giờ phát xong.

### 5.9. Write Defaults

`Write Defaults` liên quan tới cách Animator ghi giá trị mặc định cho những thuộc tính không được animation hiện tại điều khiển.

Đây là phần dễ tạo lỗi khi Controller có nhiều state/layer. Điều quan trọng cho người mới:

- Giữ cách dùng nhất quán giữa các state.
- Đừng bật/tắt ngẫu nhiên theo từng state.
- Nếu thấy thuộc tính bị giữ lại hoặc bị reset lạ khi chuyển animation, hãy kiểm tra Write Defaults và các curve mà clip đang animate.

Với project mới, nhiều team chọn tắt Write Defaults và quản lý rõ clip nào điều khiển thuộc tính nào. Nhưng điều quan trọng hơn cả là sự nhất quán trong toàn Controller.

---

## 6. Animator Parameters

Animator Controller có bốn loại parameter chính:

| Loại | Giá trị | Ví dụ |
|---|---|---|
| Float | Số thực | Speed, VerticalSpeed |
| Int | Số nguyên | WeaponType, ComboIndex |
| Bool | True/False | IsGrounded, IsDead |
| Trigger | Tín hiệu một lần | Attack, Hurt, Jump |

### 6.1. Float

Phù hợp với giá trị liên tục:

```csharp
animator.SetFloat("Speed", speed);
```

Dùng cho:

- Tốc độ di chuyển.
- Hướng ngang/dọc của Blend Tree.
- Độ nghiêng.
- Khoảng cách hoặc mức blend.

Có thể làm mượt thay đổi:

```csharp
animator.SetFloat(
    "Speed",
    targetSpeed,
    0.1f,
    Time.deltaTime
);
```

`0.1f` là damp time, giúp giá trị không đổi quá gắt.

### 6.2. Int

Phù hợp với lựa chọn rời rạc có nhiều mức:

```csharp
animator.SetInteger("WeaponType", 2);
```

Ví dụ:

```text
0 = không vũ khí
1 = kiếm
2 = cung
3 = súng
```

Nếu chỉ có hai trạng thái, Bool thường dễ hiểu hơn Int.

### 6.3. Bool

Bool giữ trạng thái cho tới khi code đổi lại:

```csharp
animator.SetBool("IsGrounded", isGrounded);
animator.SetBool("IsDead", health <= 0);
```

Phù hợp với câu hỏi đang đúng hay sai:

- Có chạm đất không?
- Có đang cúi không?
- Có chết chưa?
- Có đang cầm súng không?

### 6.4. Trigger

Trigger là tín hiệu dùng cho một lần chuyển state:

```csharp
animator.SetTrigger("Attack");
```

Khác Bool:

- Bool giữ true cho tới khi tự đặt false.
- Trigger được Animator sử dụng như tín hiệu và reset sau khi được transition tiêu thụ.

Phù hợp với:

- Attack.
- Hurt.
- Dodge.
- Open.

Có thể hủy trigger chưa muốn dùng:

```csharp
animator.ResetTrigger("Attack");
```

### 6.5. Tên parameter phải khớp chính xác

```csharp
animator.SetBool("IsGrounded", true);
```

Nếu Controller đặt tên `isGrounded` nhưng code dùng `IsGrounded`, Unity coi đó là hai tên khác nhau.

Để giảm lỗi chuỗi và tối ưu lookup, có thể tạo hash:

```csharp
private static readonly int SpeedHash =
    Animator.StringToHash("Speed");

private static readonly int GroundedHash =
    Animator.StringToHash("IsGrounded");
```

Sử dụng:

```csharp
animator.SetFloat(SpeedHash, speed);
animator.SetBool(GroundedHash, isGrounded);
```

---

## 7. Blend Tree

### 7.1. Blend Tree là gì?

Blend Tree trộn nhiều Animation Clip dựa trên một hoặc nhiều parameter.

Không có Blend Tree:

```text
Idle state
Walk state
Run state
Nhiều transition qua lại
```

Có Blend Tree:

```text
Locomotion state
└── Blend Tree
    ├── Idle
    ├── Walk
    └── Run
```

Parameter `Speed` thay đổi liên tục, Unity tự tính trọng số của từng clip.

Blend Tree không phải là một animation mới. Nó là hệ thống trộn các Motion đã có.

### 7.2. Khi nào nên dùng Blend Tree?

Nên dùng khi các animation là các phiên bản liên tục của cùng một nhóm chuyển động:

- Idle → Walk → Run theo tốc độ.
- Đi trái/phải/lên/xuống theo hướng.
- Aim theo hướng chuột.
- Đi chậm và chạy nhanh theo cùng một hướng.
- Nghiêng người theo vận tốc.

Không nên dùng chỉ vì muốn nối hai hành động không liên tục như:

- Idle và Death.
- Run và OpenChest.
- Attack và Hurt.

Các hành động khác bản chất thường nên là state riêng.

### 7.3. Tạo Blend Tree

Trong Animator Window:

```text
Chuột phải vùng trống
→ Create State
→ From New Blend Tree
```

Double-click state Blend Tree để đi vào cấu hình.

Các bước cơ bản:

1. Chọn Blend Type.
2. Chọn parameter điều khiển.
3. Nhấn `+`.
4. Chọn `Add Motion Field`.
5. Kéo Animation Clip vào từng ô Motion.
6. Đặt Threshold hoặc Position.

### 7.4. 1D Blend Tree

1D dùng một Float parameter.

Ví dụ:

```text
Parameter: Speed

Threshold 0.0 → Idle
Threshold 0.5 → Walk
Threshold 1.0 → Run
```

Khi `Speed = 0`, trọng số Idle lớn nhất.

Khi `Speed = 1`, trọng số Run lớn nhất.

Khi `Speed = 0.75`, Unity trộn Walk và Run theo vị trí giữa hai threshold.

Code:

```csharp
float speed = Mathf.Abs(rb.linearVelocity.x);
animator.SetFloat("Speed", speed);
```

Nếu muốn chuẩn hóa Speed về 0–1:

```csharp
float normalizedSpeed = Mathf.InverseLerp(
    0f,
    maxSpeed,
    Mathf.Abs(rb.linearVelocity.x)
);

animator.SetFloat("Speed", normalizedSpeed);
```

### 7.5. 2D Blend Tree

2D dùng hai Float parameter, ví dụ:

```text
MoveX
MoveY
```

Phù hợp với top-down game:

```text
( 0,  1) → Walk Up
( 0, -1) → Walk Down
(-1,  0) → Walk Left
( 1,  0) → Walk Right
( 0,  0) → Idle
```

Code:

```csharp
animator.SetFloat("MoveX", moveInput.x);
animator.SetFloat("MoveY", moveInput.y);
```

### 7.6. 2D Simple Directional

Hợp khi các motion biểu diễn các hướng khác nhau và chỉ có một motion cho mỗi hướng:

- Walk Up.
- Walk Down.
- Walk Left.
- Walk Right.

Có thể có Idle tại `(0, 0)`.

Không phù hợp nếu có nhiều motion cùng một hướng, ví dụ vừa Walk Right vừa Run Right.

### 7.7. 2D Freeform Directional

Hợp với dữ liệu theo hướng nhưng cho phép nhiều motion cùng một hướng với độ lớn khác nhau:

```text
(0, 1) → Walk Up
(0, 2) → Run Up
(1, 0) → Walk Right
(2, 0) → Run Right
```

Thường nên có motion ở `(0, 0)`, ví dụ Idle.

### 7.8. 2D Freeform Cartesian

Hợp khi hai parameter không chỉ đại diện cho hướng, mà là hai khái niệm độc lập.

Ví dụ:

```text
X = tốc độ quay
Y = tốc độ tiến
```

Nó phù hợp khi khoảng cách trong mặt phẳng parameter mang ý nghĩa pha trộn, không nhất thiết chỉ là vector hướng.

### 7.9. Direct Blend Tree

Direct cho phép mỗi child motion có một parameter weight riêng.

Ví dụ:

```text
SmileWeight
BlinkWeight
LookWeight
```

Nó thường dùng cho facial animation hoặc hệ cần kiểm soát trực tiếp trọng số từng motion, không phải lựa chọn đầu tiên cho movement cơ bản.

### 7.10. Blend Tree và tốc độ thật

Parameter của Blend Tree không tự di chuyển nhân vật. Nó chỉ điều khiển hình ảnh được trộn.

Ví dụ:

```csharp
rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
```

- Rigidbody2D làm Player di chuyển thật.
- Animator nhận tốc độ thật để chọn hình ảnh phù hợp.

Nếu chỉ SetFloat mà không thay đổi Rigidbody2D, nhân vật có thể hiện animation chạy nhưng đứng nguyên tại chỗ.

### 7.11. Sprite 2D có cần blend không?

Sprite frame-by-frame vẫn dùng Blend Tree được. Tuy nhiên, do mỗi clip là chuỗi hình rời rạc, kết quả không “nội suy hình dáng” mượt như rig 3D. Blend Tree vẫn hữu ích để:

- Chọn trọng số animation theo tốc độ.
- Quản lý locomotion gọn hơn.
- Chuyển hướng top-down.

Với pixel art cần chuyển clip thật dứt khoát, state riêng đôi khi lại dễ kiểm soát hơn. Không phải lúc nào Blend Tree cũng tốt hơn.

---

## 8. Finite State Machine

### 8.1. FSM là gì?

Finite State Machine, viết tắt FSM, là máy trạng thái hữu hạn.

Một hệ FSM có:

- Một tập hợp state hữu hạn.
- Tại một thời điểm, mỗi layer thường có một state đang hoạt động.
- Điều kiện để chuyển từ state này sang state khác.
- Hành động xảy ra khi vào, ở trong hoặc rời state.

Ví dụ Player:

```text
Idle
Run
Jump
Fall
Attack
Hurt
Dead
```

### 8.2. Ba thành phần chính

#### State

Trạng thái hiện tại:

```text
Player đang Idle
Enemy đang Patrol
Door đang Open
```

#### Transition

Đường chuyển:

```text
Idle → Run
Run → Jump
Jump → Fall
Fall → Idle
```

#### Condition hoặc Event

Lý do được chuyển:

```text
Speed > 0
IsGrounded = false
Health <= 0
Attack Trigger được gọi
```

### 8.3. Animator Controller chính là một FSM hình ảnh

Trong Animator:

- State là node.
- Transition là mũi tên.
- Parameter và Exit Time là điều kiện.
- Motion là clip hoặc Blend Tree được chạy trong state.

Ví dụ:

```text
          Speed > 0.1
    Idle ─────────────→ Run
      ↑                  │
      └──────────────────┘
          Speed < 0.1

Any State ── Hurt Trigger ──→ Hurt
Any State ── IsDead = true ─→ Death
```

### 8.4. Animator FSM và Gameplay FSM không hoàn toàn giống nhau

Animator FSM nên tập trung vào trạng thái hình ảnh:

- Idle animation.
- Run animation.
- Jump animation.
- Attack animation.

Gameplay FSM có thể quản lý luật lớn hơn:

- Player được phép điều khiển hay đang bị khóa.
- Enemy đang tuần tra, đuổi theo hay tấn công.
- Game đang Playing, Paused hay GameOver.

Không nên ép toàn bộ gameplay vào Animator Controller chỉ vì Animator có state machine. Animator là nơi rất tốt để điều khiển presentation/animation, nhưng code gameplay vẫn nên giữ quyền quyết định quan trọng.

Ví dụ tốt:

```text
Code quyết định Player đã chết
        ↓
Code khóa input và physics phù hợp
        ↓
Code đặt IsDead = true cho Animator
        ↓
Animator chuyển sang Death animation
```

Không nên để việc Death animation kết thúc hay chưa là nguồn duy nhất quyết định dữ liệu Player còn sống hay đã chết.

### 8.5. Ví dụ FSM bằng enum trong code

```csharp
public enum PlayerState
{
    Idle,
    Running,
    Jumping,
    Falling,
    Dead
}
```

```csharp
private PlayerState currentState;

private void ChangeState(PlayerState newState)
{
    if (currentState == newState)
        return;

    ExitState(currentState);
    currentState = newState;
    EnterState(currentState);
}
```

Với dự án nhỏ, không nhất thiết phải viết một hệ FSM class phức tạp. Chỉ cần hiểu tư duy state, transition và condition là đã giúp code rõ hơn rất nhiều.

### 8.6. StateMachineBehaviour

Unity cho phép thêm `StateMachineBehaviour` vào state để nhận callback như:

- `OnStateEnter`.
- `OnStateUpdate`.
- `OnStateExit`.

Ví dụ:

```csharp
using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        Debug.Log("Bắt đầu state Attack");
    }

    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        Debug.Log("Rời state Attack");
    }
}
```

Nó hữu ích cho logic gắn trực tiếp với vòng đời state animation. Nhưng cũng giống Animation Event, không nên giấu quá nhiều gameplay quan trọng trong nhiều behaviour rải rác khiến luồng khó theo dõi.

---

## 9. Kết nối Animator với code

### 9.1. Lấy Animator

```csharp
private Animator animator;

private void Awake()
{
    animator = GetComponent<Animator>();
}
```

Có thể yêu cầu component:

```csharp
[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
}
```

### 9.2. Gửi parameter

```csharp
animator.SetFloat("Speed", speed);
animator.SetInteger("WeaponType", weaponType);
animator.SetBool("IsGrounded", isGrounded);
animator.SetTrigger("Attack");
```

### 9.3. Đọc parameter

```csharp
float speed = animator.GetFloat("Speed");
bool grounded = animator.GetBool("IsGrounded");
int weapon = animator.GetInteger("WeaponType");
```

Thông thường gameplay đã có sẵn dữ liệu này nên không cần đọc ngược từ Animator. Code nên là nguồn dữ liệu, Animator nhận dữ liệu để hiển thị.

### 9.4. `Animator.Play`

```csharp
animator.Play("Attack");
```

Lệnh này đưa Animator trực tiếp tới state có tên tương ứng.

Nó hữu ích cho một số trường hợp, nhưng nếu dùng cho mọi animation thì có thể bỏ qua lợi ích của transition và làm logic Controller khó hiểu.

Tên đầy đủ có thể gồm layer:

```csharp
animator.Play("Base Layer.Attack");
```

### 9.5. `Animator.CrossFade`

```csharp
animator.CrossFade("Base Layer.Run", 0.1f);
```

CrossFade chuyển sang state khác với một khoảng blend. Nó hữu ích khi điều khiển trực tiếp state bằng code nhưng vẫn muốn chuyển mượt.

### 9.6. Dùng Parameter hay Play/CrossFade?

Dùng Parameter khi:

- Muốn Controller tự quản lý transition.
- Sơ đồ state rõ ràng.
- Designer cần chỉnh Exit Time và Duration.
- Có nhiều điều kiện animation.

Dùng Play/CrossFade khi:

- Code là FSM chính và Animator chỉ hiển thị state.
- Muốn điều khiển trực tiếp, ít transition.
- Biết rõ state nào phải được phát ngay.

Không có lựa chọn đúng tuyệt đối. Điều quan trọng là chọn một hướng nhất quán, tránh vừa transition tự động vừa liên tục `Play` state khác làm hai cơ chế tranh nhau.

### 9.7. Kiểm tra state hiện tại

```csharp
AnimatorStateInfo stateInfo =
    animator.GetCurrentAnimatorStateInfo(0);

if (stateInfo.IsName("Base Layer.Attack"))
{
    Debug.Log("Đang ở state Attack");
}
```

Số `0` là index của layer đầu tiên, thường là Base Layer.

Có thể xem normalized time:

```csharp
float progress = stateInfo.normalizedTime;
```

Với clip không loop:

- Gần 0: đầu clip.
- Gần 0.5: nửa clip.
- Từ 1 trở lên: đã tới cuối chu kỳ đầu.

Đừng phụ thuộc quá mức vào kiểm tra normalized time trong nhiều script; Transition, Animation Event hoặc StateMachineBehaviour có thể rõ hơn tùy bài toán.

---

## 10. Ví dụ Animator cho Player 2D

### 10.1. Controller đề xuất

Parameters:

```text
Speed         Float
VerticalSpeed Float
IsGrounded    Bool
Attack        Trigger
Hurt          Trigger
IsDead        Bool
```

States:

```text
Locomotion (1D Blend Tree: Idle/Run)
Jump
Fall
Attack
Hurt
Death
```

Transitions:

```text
Locomotion → Jump
    IsGrounded = false
    VerticalSpeed > 0

Jump → Fall
    VerticalSpeed < 0

Fall → Locomotion
    IsGrounded = true

Any State → Attack
    Attack trigger

Attack → Locomotion
    Has Exit Time

Any State → Hurt
    Hurt trigger

Any State → Death
    IsDead = true
```

Tùy gameplay, Any State → Attack có thể không phù hợp nếu không được phép đánh lúc chết hoặc đang Hurt. Có thể thêm condition hoặc chỉ tạo Attack transition từ những state thật sự được phép.

### 10.2. Script cập nhật Animator

```csharp
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimation2D : MonoBehaviour
{
    [SerializeField] private PlayerGroundCheck groundCheck;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Animator animator;
    private Rigidbody2D body;

    private static readonly int SpeedHash =
        Animator.StringToHash("Speed");

    private static readonly int VerticalSpeedHash =
        Animator.StringToHash("VerticalSpeed");

    private static readonly int GroundedHash =
        Animator.StringToHash("IsGrounded");

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");

    private static readonly int HurtHash =
        Animator.StringToHash("Hurt");

    private static readonly int DeadHash =
        Animator.StringToHash("IsDead");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 velocity = body.linearVelocity;

        animator.SetFloat(
            SpeedHash,
            Mathf.Abs(velocity.x),
            0.1f,
            Time.deltaTime
        );

        animator.SetFloat(VerticalSpeedHash, velocity.y);
        animator.SetBool(GroundedHash, groundCheck.IsGrounded);

        if (Mathf.Abs(velocity.x) > 0.01f)
        {
            spriteRenderer.flipX = velocity.x < 0f;
        }
    }

    public void PlayAttack()
    {
        animator.SetTrigger(AttackHash);
    }

    public void PlayHurt()
    {
        animator.SetTrigger(HurtHash);
    }

    public void SetDead(bool isDead)
    {
        animator.SetBool(DeadHash, isDead);
    }
}
```

`PlayerGroundCheck` ở đây đại diện cho script kiểm tra đất của project:

```csharp
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private Transform checkPoint;
    [SerializeField] private Vector2 checkSize =
        new Vector2(0.7f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    private void FixedUpdate()
    {
        IsGrounded = Physics2D.OverlapBox(
            checkPoint.position,
            checkSize,
            0f,
            groundLayer
        );
    }
}
```

Nếu phiên bản Unity của project dùng `Rigidbody2D.velocity`, thay:

```csharp
body.linearVelocity
```

bằng:

```csharp
body.velocity
```

### 10.3. Vì sao flip SpriteRenderer thay vì scale Player?

```csharp
spriteRenderer.flipX = velocity.x < 0f;
```

Cách này chỉ lật hình ảnh, không lật:

- Collider.
- GroundCheck.
- UI con.
- Các object gameplay khác.

Nếu thay `transform.localScale.x` của toàn Player, mọi child có thể bị lật theo. Có lúc đây là điều mình muốn, nhưng phải hiểu rõ ảnh hưởng.

### 10.4. Tách animation khỏi movement

`PlayerAnimation2D` không tự làm Player di chuyển. Nó đọc trạng thái từ Rigidbody2D và ground check rồi cập nhật hình ảnh.

Đây là cách tách trách nhiệm tốt:

```text
PlayerMovement
    → điều khiển Rigidbody2D

PlayerGroundCheck
    → xác định có chạm đất

PlayerAnimation2D
    → đọc dữ liệu và cập nhật Animator
```

---

## 11. Quy trình tạo Animator từ đầu

### Bước 1: Chuẩn bị clip

Tạo:

```text
Player_Idle.anim
Player_Run.anim
Player_Jump.anim
Player_Fall.anim
Player_Attack.anim
```

Bật Loop Time cho Idle và Run. Jump, Fall hoặc Attack tùy nội dung clip và thiết kế.

### Bước 2: Tạo Animator Controller

```text
Project > Create > Animator Controller
```

Đặt tên:

```text
Player.controller
```

### Bước 3: Gán vào Animator Component

Chọn Player và kéo `Player.controller` vào:

```text
Animator > Controller
```

### Bước 4: Tạo state

Kéo clip vào Animator Window để tạo state hoặc chuột phải tạo state rồi gán Motion.

Đặt Locomotion/Idle làm default state.

### Bước 5: Tạo parameters

Trong tab Parameters:

```text
Speed         Float
VerticalSpeed Float
IsGrounded    Bool
Attack        Trigger
```

### Bước 6: Tạo transition

Ví dụ:

```text
Idle → Run: Speed Greater 0.1
Run → Idle: Speed Less 0.1
Any State → Attack: Attack trigger
Attack → Idle: Has Exit Time
```

Tắt Has Exit Time trên Idle ↔ Run để phản hồi ngay.

### Bước 7: Viết script cập nhật parameter

```csharp
animator.SetFloat("Speed", Mathf.Abs(body.linearVelocity.x));
animator.SetBool("IsGrounded", isGrounded);
```

### Bước 8: Play Mode và quan sát

Khi chạy game, mở Animator Window. State đang active sẽ được tô sáng và transition đang chạy sẽ hiển thị luồng.

Theo dõi Parameters để xem giá trị có đổi đúng không. Đây là cách debug nhanh nhất.

### Bước 9: Chỉnh transition

Nếu phản hồi chậm:

- Kiểm tra Has Exit Time.
- Giảm Transition Duration.
- Kiểm tra Conditions.
- Kiểm tra parameter trong code.

Nếu animation bị restart liên tục:

- Kiểm tra Any State self transition.
- Kiểm tra SetTrigger có bị gọi mỗi frame không.
- Kiểm tra có gọi Animator.Play liên tục trong Update không.

---

## 12. Lỗi thường gặp

### 12.1. Animation không chạy

Kiểm tra:

- GameObject có Animator không?
- Animator đã được gán Controller chưa?
- State có Motion chưa?
- Clip có dữ liệu/frame chưa?
- Animator component có bị disable không?
- GameObject có active không?
- Culling Mode có dừng animation ngoài màn hình không?

### 12.2. Parameter đổi nhưng không chuyển state

Kiểm tra:

- Tên parameter có đúng cả chữ hoa/chữ thường không?
- Transition có condition đúng không?
- Có bật Has Exit Time không?
- Có nhiều condition và một condition chưa đúng không?
- Transition có đúng chiều không?
- State hiện tại có đường transition tới state đích không?
- Transition khác có độ ưu tiên hoặc khả năng interrupt không?

### 12.3. Animation phản hồi chậm

Nguyên nhân thường gặp:

- Has Exit Time đang bật.
- Transition Duration quá dài.
- Parameter được cập nhật muộn.
- Float damping quá lớn.
- Clip hiện tại bắt buộc phát tới một mốc mới được thoát.

### 12.4. Attack phát liên tục

Kiểm tra:

- Có gọi `SetTrigger("Attack")` trong `Update` mỗi frame không?
- Input có dùng `WasPressedThisFrame` hay đang coi giữ nút là nhiều lần bấm?
- Any State → Attack có cho phép transition tới chính nó không?
- Attack clip có bật Loop Time không?
- Attack → Locomotion đã có transition thoát chưa?

### 12.5. Nhân vật chạy nhưng animation vẫn Idle

```csharp
animator.SetFloat("Speed", speed);
```

Kiểm tra `speed` bằng Debug.Log và theo dõi parameter trong Animator Window.

Có thể đang lấy input nhưng threshold dùng tốc độ thật, hoặc ngược lại. Ví dụ input chỉ từ 0 tới 1 nhưng threshold Run lại đặt 5.

### 12.6. Animation chạy nhưng nhân vật không di chuyển

Animator chỉ hiển thị animation nếu không dùng Root Motion. Vẫn cần code movement:

```csharp
body.linearVelocity = new Vector2(
    moveInput.x * moveSpeed,
    body.linearVelocity.y
);
```

### 12.7. Nhân vật trượt chân

Hình chạy nhanh/chậm không khớp tốc độ di chuyển thật.

Có thể xử lý bằng:

- Chỉnh Samples hoặc speed của state/clip.
- Chỉnh threshold trong Blend Tree.
- Truyền tốc độ thực tế cho Animator.
- Thiết kế clip phù hợp với tốc độ gameplay.

### 12.8. Animator.Play trong Update làm animation đứng ở đầu

Sai:

```csharp
private void Update()
{
    animator.Play("Attack");
}
```

Mỗi frame có thể liên tục ép state hoặc restart theo cách điều khiển cụ thể, làm clip không tiến triển như mong muốn.

Chỉ gọi khi state cần thay đổi hoặc dùng parameter/transition.

### 12.9. State bị kẹt

Ví dụ kẹt ở Attack:

- Attack không có transition thoát.
- Exit Time không được đạt do clip bị restart.
- Condition thoát chưa đúng.
- Clip loop và transition cấu hình sai.
- State Speed bằng 0.

### 12.10. Sprite bị lật cả collider hoặc UI

Nếu lật cả Player bằng scale âm, mọi child cũng bị ảnh hưởng. Có thể chỉ lật:

```csharp
spriteRenderer.flipX = true;
```

hoặc đặt sprite vào child riêng rồi chỉ lật child đó.

### 12.11. State name không tìm thấy

Khi dùng Play/CrossFade, tên state và layer phải đúng:

```csharp
animator.CrossFade("Base Layer.Run", 0.1f);
```

Nếu đổi tên state trong Controller, chuỗi trong code không tự đổi theo.

### 12.12. Dùng quá nhiều Bool

Controller có thể rơi vào tổ hợp khó hiểu:

```text
IsRunning = true
IsJumping = true
IsFalling = true
IsAttacking = true
```

Hãy phân biệt:

- Dữ liệu thật: `Speed`, `VerticalSpeed`, `IsGrounded`.
- Hành động một lần: Trigger `Attack`, `Hurt`.
- Trạng thái loại trừ nhau: có thể do FSM code hoặc cấu trúc transition quyết định.

---

## 13. Tối ưu và tổ chức Controller

### 13.1. Đặt tên thống nhất

Ví dụ:

```text
State: Idle, Run, Jump, Fall, Attack
Parameter: Speed, VerticalSpeed, IsGrounded, Attack
Clip: Player_Idle, Player_Run, Player_Jump
```

Tránh tên mơ hồ:

```text
New State
Blend Tree 1
bool1
anim2
```

### 13.2. Dùng hash cho parameter gọi thường xuyên

```csharp
private static readonly int SpeedHash =
    Animator.StringToHash("Speed");
```

```csharp
animator.SetFloat(SpeedHash, speed);
```

Lợi ích:

- Không lookup chuỗi lặp lại.
- Tập trung tên parameter ở một chỗ.
- Giảm lỗi gõ chuỗi rải rác.

### 13.3. Đừng gọi Set nếu dữ liệu không liên quan?

Các parameter movement có thể cập nhật mỗi frame vì chúng phản ánh vận tốc liên tục.

Những hành động một lần không nên gọi liên tục:

```csharp
animator.SetTrigger(AttackHash);
```

Chỉ gọi khi thật sự phát sinh hành động Attack.

### 13.4. Dùng Blend Tree để giảm transition rác

Idle, Walk và Run cùng nhóm locomotion có thể đặt trong một Blend Tree thay vì tạo nhiều mũi tên qua lại.

Nhưng không nên đưa mọi animation vào một Blend Tree khổng lồ. Attack, Hurt và Death vẫn nên tách state nếu chúng có luồng riêng.

### 13.5. Dùng Sub-State Machine khi sơ đồ quá lớn

Nhóm theo ý nghĩa:

```text
Locomotion
Airborne
Combat
Special
```

Không cần tạo Sub-State Machine khi Controller chỉ có vài state. Thêm tầng quá sớm có thể làm việc theo dõi luồng khó hơn.

### 13.6. Hạn chế Any State

Any State hợp với transition thật sự có thể xảy ra từ nhiều nơi:

- Death.
- Hurt.
- Một số phản ứng toàn cục.

Không nên biến Any State thành trung tâm của mọi transition.

### 13.7. Animation không nên là nguồn dữ liệu gameplay duy nhất

Ví dụ máu nên nằm trong `Health` component, không nằm trong Animator parameter như nguồn chính.

Luồng tốt:

```text
Health component tính máu
        ↓
Nếu chết, khóa gameplay
        ↓
Gửi IsDead cho Animator
        ↓
Animator phát Death clip
```

### 13.8. Profile trước khi tối ưu quá mức

Với project nhỏ, sự rõ ràng quan trọng hơn việc tối ưu vài lệnh `SetFloat`. Khi có nhiều Animator hoặc controller phức tạp, dùng Unity Profiler để tìm vấn đề thật thay vì đoán.

---

## 14. Bảng ghi nhớ nhanh

### Thành phần

| Thành phần | Nằm ở đâu? | Vai trò |
|---|---|---|
| Animation Clip | Asset `.anim` | Lưu dữ liệu thay đổi theo thời gian |
| Animator Controller | Asset `.controller` | Chứa state, transition, parameter, layer |
| Animator Component | Trên GameObject | Chạy Controller trong Scene |
| Blend Tree | Motion bên trong state | Trộn nhiều clip theo parameter |
| FSM | Mô hình tư duy/luồng state | Quản lý state và điều kiện chuyển |

### Parameter

| Parameter | Dùng cho |
|---|---|
| Float | Tốc độ, hướng, giá trị blend liên tục |
| Int | Lựa chọn nhiều mức rời rạc |
| Bool | Trạng thái đúng/sai được giữ lại |
| Trigger | Tín hiệu hành động một lần |

### API hay dùng

```csharp
animator.SetFloat("Speed", speed);
animator.SetInteger("WeaponType", type);
animator.SetBool("IsGrounded", grounded);
animator.SetTrigger("Attack");
animator.ResetTrigger("Attack");

animator.Play("Base Layer.Attack");
animator.CrossFade("Base Layer.Run", 0.1f);
```

### Chọn nhanh

| Nhu cầu | Nên dùng |
|---|---|
| Lưu frame chạy | Animation Clip |
| Điều khiển Idle/Run/Jump | Animator Controller |
| Cho Player chạy Controller | Animator Component |
| Trộn Idle/Walk/Run theo tốc độ | 1D Blend Tree |
| Chọn animation theo hướng X/Y | 2D Blend Tree |
| Thiết kế luồng trạng thái | FSM |
| Hành động xảy ra một lần | Trigger parameter |
| Trạng thái kéo dài | Bool parameter |

### Câu chốt để nhớ

```text
Clip là nội dung chuyển động.
State là trạng thái đang phát Motion.
Transition là luật đổi state.
Parameter là dữ liệu điều khiển transition và Blend Tree.
Controller là sơ đồ quản lý tất cả những phần trên.
Animator Component là thứ chạy Controller trên GameObject.
Blend Tree trộn nhiều clip trong cùng một state.
FSM là tư duy tổ chức state và điều kiện chuyển đổi.
```
