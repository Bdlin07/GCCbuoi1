# Trigger và Raycast 2D trong Unity

## Mục lục

1. [Hiểu nhanh: Trigger và Raycast khác nhau thế nào?](#1-hiểu-nhanh-trigger-và-raycast-khác-nhau-thế-nào)
2. [Điều kiện để Trigger 2D hoạt động](#2-điều-kiện-để-trigger-2d-hoạt-động)
3. [`OnTriggerEnter2D`](#3-ontriggerenter2d)
4. [`OnTriggerStay2D`](#4-ontriggerstay2d)
5. [`OnTriggerExit2D`](#5-ontriggerexit2d)
6. [Trigger trả về cái gì?](#6-trigger-trả-về-cái-gì)
7. [Ví dụ Trigger 2D hoàn chỉnh](#7-ví-dụ-trigger-2d-hoàn-chỉnh)
8. [Raycast 2D là gì?](#8-raycast-2d-là-gì)
9. [`Physics2D.Raycast`](#9-physics2draycast)
10. [Đọc dữ liệu trong `RaycastHit2D`](#10-đọc-dữ-liệu-trong-raycasthit2d)
11. [LayerMask và bộ lọc](#11-layermask-và-bộ-lọc)
12. [Các biến thể của Raycast 2D](#12-các-biến-thể-của-raycast-2d)
13. [Các shape cast: Circle, Box, Capsule](#13-các-shape-cast-circle-box-capsule)
14. [`Collider2D.Cast` và `Rigidbody2D.Cast`](#14-collider2dcast-và-rigidbody2dcast)
15. [Overlap khác Cast như thế nào?](#15-overlap-khác-cast-như-thế-nào)
16. [Nên dùng hàm nào trong từng tình huống?](#16-nên-dùng-hàm-nào-trong-từng-tình-huống)
17. [Lỗi thường gặp và cách debug](#17-lỗi-thường-gặp-và-cách-debug)
18. [Tối ưu hiệu năng](#18-tối-ưu-hiệu-năng)
19. [Bảng ghi nhớ nhanh](#19-bảng-ghi-nhớ-nhanh)

---

## 1. Hiểu nhanh: Trigger và Raycast khác nhau thế nào?

### Trigger là sự kiện được Unity báo cho mình

Bạn đặt một `Collider2D`, bật `Is Trigger`, rồi chờ một collider khác đi vào vùng đó. Khi trạng thái chồng lấn thay đổi, Unity tự gọi code của bạn:

- Vừa đi vào: `OnTriggerEnter2D`
- Vẫn còn ở bên trong: `OnTriggerStay2D`
- Vừa đi ra: `OnTriggerExit2D`

Trigger hợp với những vùng tồn tại sẵn trong màn chơi, ví dụ:

- Vùng nhặt vật phẩm
- Cửa dịch chuyển
- Checkpoint
- Vùng gây độc hoặc hồi máu
- Vùng phát hiện người chơi của AI
- Khu vực bắt đầu hội thoại

Trigger không có tác dụng chặn vật thể. Hai collider có thể đi xuyên qua nhau vì trigger dùng để **phát hiện**, không dùng để tạo va chạm vật lý cứng.

### Raycast là câu hỏi do code chủ động đặt ra

`Physics2D.Raycast` giống như bắn một tia vô hình từ điểm A theo một hướng nào đó và hỏi hệ vật lý:

> “Trên đường đi có collider nào không? Nếu có thì collider gần nhất là gì, nằm cách bao xa, tia chạm ở đâu?”

Raycast hợp với những kiểm tra tức thời, ví dụ:

- Nhân vật có đang đứng trên đất không?
- Trước mặt có tường không?
- Súng bắn trúng vật gì?
- Kẻ địch có nhìn thấy người chơi không?
- Chuột đang trỏ vào collider nào?

Khác biệt quan trọng là trigger chạy theo **sự kiện**, còn raycast chỉ chạy khi **code của bạn gọi hàm**.

---

## 2. Điều kiện để Trigger 2D hoạt động

Muốn nhận được callback trigger 2D, cần kiểm tra các điều kiện sau.

### 2.1. Cả hai vật thể phải có `Collider2D`

Ví dụ hợp lệ:

- `BoxCollider2D` gặp `CircleCollider2D`
- `CapsuleCollider2D` gặp `TilemapCollider2D`
- `PolygonCollider2D` gặp `BoxCollider2D`

Không được trộn collider 2D với collider 3D. `BoxCollider2D` và `BoxCollider` thuộc hai hệ vật lý riêng, nên chúng không phát hiện nhau.

### 2.2. Ít nhất một collider phải bật `Is Trigger`

Trong Inspector của collider, đánh dấu:

```text
Is Trigger = true
```

Nếu cả hai đều không phải trigger thì Unity xử lý đây là collision bình thường và gọi nhóm `OnCollision...2D`, không gọi `OnTrigger...2D`.

### 2.3. Ít nhất một bên phải có `Rigidbody2D`

Đây là lỗi cấu hình rất hay gặp. Hai `Collider2D` đứng riêng, không có `Rigidbody2D` ở bên nào, thường được xem là hai collider tĩnh và sẽ không tạo trigger event như bạn mong đợi.

Cấu hình phổ biến nhất là:

- Vùng trigger: có `Collider2D`, bật `Is Trigger`; có thể không cần `Rigidbody2D` nếu đứng yên.
- Player: có `Collider2D` và `Rigidbody2D`.

Nếu gắn `Rigidbody2D` cho vùng trigger đứng yên, có thể đặt `Body Type = Kinematic` hoặc `Static` tùy cách thiết kế. Quan trọng nhất là trong cặp tương tác phải có ít nhất một `Rigidbody2D`.

### 2.4. Layer của hai vật thể phải được phép tương tác

Vào:

```text
Edit > Project Settings > Physics 2D > Layer Collision Matrix
```

Nếu ô giao giữa hai layer bị bỏ chọn, hai vật thể sẽ bị hệ vật lý 2D bỏ qua. Khi đó callback trigger cũng không chạy.

### 2.5. Script phải viết đúng tên và đúng tham số

Chữ hoa, chữ thường và hậu tố `2D` phải chính xác:

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
}

private void OnTriggerStay2D(Collider2D other)
{
}

private void OnTriggerExit2D(Collider2D other)
{
}
```

Không có callback tên là `OnTrigger2D()` hoặc `OnTrigger()`. “On Trigger” chỉ là cách gọi chung của nhóm sự kiện Enter, Stay và Exit.

---

## 3. `OnTriggerEnter2D`

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log($"{other.name} vừa đi vào trigger");
}
```

### Khi nào được gọi?

Unity gọi hàm này khi hai `Collider2D` bắt đầu chạm hoặc chồng lên nhau, trong đó ít nhất một collider là trigger và ít nhất một bên có `Rigidbody2D`.

Bạn có thể hiểu đây là khoảnh khắc trạng thái đổi từ:

```text
Không chạm nhau -> Bắt đầu chạm nhau
```

Nó chỉ được gọi một lần cho mỗi lần đi vào, không gọi liên tục trong toàn bộ thời gian vật thể đứng bên trong.

Nếu player đi vào, đi ra rồi lại đi vào thì `OnTriggerEnter2D` sẽ được gọi thêm lần nữa.

### Dùng khi nào?

Dùng cho hành động chỉ cần xảy ra một lần vào lúc bước vào vùng:

- Nhặt coin
- Mở cửa
- Bật checkpoint
- Chuyển scene
- Bắt đầu hội thoại
- Hiện thông báo “Nhấn E để tương tác”
- Thêm một đối tượng vào danh sách mục tiêu đang ở trong vùng

### `other` là gì?

`other` là `Collider2D` của phía còn lại trong lần tương tác đó.

Ví dụ script nằm trên vùng coin, player đi vào coin, thì `other` thường là collider của player:

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log(other.gameObject.name);
    Debug.Log(other.transform.position);
    Debug.Log(other.gameObject.layer);
}
```

### Kiểm tra đúng đối tượng

Cách thường dùng nhất là tag:

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Player"))
        return;

    Debug.Log("Player đã đi vào");
}
```

`CompareTag` được ưu tiên hơn so sánh `other.tag == "Player"`: rõ ý và tránh một số lỗi do tag không tồn tại.

Cũng có thể kiểm tra component:

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
    {
        health.Heal(20);
    }
}
```

Cách kiểm tra component thường tốt khi ý nghĩa gameplay nằm ở khả năng của đối tượng, chẳng hạn “bất cứ thứ gì có `PlayerHealth` đều được hồi máu”, thay vì phụ thuộc hoàn toàn vào tag.

### Một đối tượng có nhiều collider thì sao?

Nếu player có nhiều collider con, mỗi collider có thể tạo một cặp overlap riêng. Vì vậy callback có thể chạy nhiều lần, dù nhìn bằng mắt chỉ thấy một player đi vào.

Cách xử lý tùy game:

- Chỉ cho một collider của player tương tác với trigger.
- Đặt collider dùng để tương tác ở một layer riêng.
- Tìm component ở parent bằng `GetComponentInParent<T>()`.
- Dùng `HashSet` để đếm hoặc lưu các đối tượng thực sự đang ở trong vùng.

Ví dụ lấy component từ parent:

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    PlayerController player = other.GetComponentInParent<PlayerController>();
    if (player == null)
        return;

    player.ActivateCheckpoint(transform.position);
}
```

---

## 4. `OnTriggerStay2D`

```csharp
private void OnTriggerStay2D(Collider2D other)
{
    Debug.Log($"{other.name} vẫn đang ở trong trigger");
}
```

### Khi nào được gọi?

Hàm được gửi lặp lại trong các bước mô phỏng vật lý khi hai collider vẫn còn chạm hoặc chồng lên nhau.

Nên hình dung nó gần với nhịp vật lý của `FixedUpdate`, không phải một bộ đếm thời gian tuyệt đối và cũng không nên mặc định rằng nó chạy đúng một lần trong mỗi `Update`. Tùy tốc độ khung hình, trong một frame hiển thị có thể có không có, một hoặc nhiều bước vật lý.

### Dùng khi nào?

Hợp với hiệu ứng cần duy trì trong lúc đối tượng ở trong vùng:

- Vùng độc trừ máu theo thời gian
- Vùng gió liên tục đẩy nhân vật
- Vùng hồi máu dần
- Giữ công tắc khi một vật nặng còn đặt lên trên
- Cho phép tương tác trong lúc player đứng gần NPC

### Cẩn thận khi cộng/trừ theo từng lần gọi

Code sau phụ thuộc vào số bước vật lý:

```csharp
private void OnTriggerStay2D(Collider2D other)
{
    if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
    {
        health.TakeDamage(1); // 1 máu cho mỗi physics step
    }
}
```

Nếu mục tiêu là sát thương theo giây, hãy nhân với `Time.fixedDeltaTime` nếu callback đang chạy theo nhịp vật lý:

```csharp
[SerializeField] private float damagePerSecond = 10f;

private void OnTriggerStay2D(Collider2D other)
{
    if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
    {
        health.TakeDamage(damagePerSecond * Time.fixedDeltaTime);
    }
}
```

Trong một hệ thống lớn hơn, cách dễ kiểm soát hơn là dùng Enter/Exit để quản lý danh sách đối tượng trong vùng, còn việc gây sát thương theo thời gian được xử lý ở một nơi riêng.

### Không nên nhét thao tác nặng vào `Stay`

`OnTriggerStay2D` có thể được gọi rất nhiều lần, nhất là khi vùng chứa nhiều collider. Tránh:

- Liên tục gọi `FindObjectOfType`
- Tạo và hủy object mỗi physics step
- Gọi `GetComponent` lặp lại nhiều lần nếu có thể cache
- Ghi `Debug.Log` liên tục trong bản build thật
- Chạy thuật toán tìm đường hoặc truy vấn lớn mỗi lần callback

---

## 5. `OnTriggerExit2D`

```csharp
private void OnTriggerExit2D(Collider2D other)
{
    Debug.Log($"{other.name} vừa rời trigger");
}
```

### Khi nào được gọi?

Hàm được gọi khi một cặp collider đang overlap chuyển sang không còn overlap nữa:

```text
Đang chạm nhau -> Không còn chạm nhau
```

Nó thường được dùng để đảo lại trạng thái đã bật trong `OnTriggerEnter2D`.

Ví dụ:

```csharp
private bool canTalk;

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
        canTalk = true;
}

private void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Player"))
        canTalk = false;
}
```

### Dùng khi nào?

- Ẩn nút tương tác khi player đi xa
- Ngừng gây sát thương vùng
- Tắt nhạc hoặc hiệu ứng của một khu vực
- Xóa mục tiêu khỏi danh sách phát hiện của AI
- Đóng cửa sau khi player rời vùng
- Đặt lại cờ `isInside`

### Đừng coi Exit là cơ chế dọn dẹp được bảo đảm tuyệt đối

Trong gameplay thực tế, collider có thể bị disable, GameObject có thể bị `SetActive(false)`, bị destroy hoặc scene có thể đổi khi nó còn ở trong trigger. Tùy tình huống, bạn không nên đặt toàn bộ sự an toàn của trạng thái vào giả định rằng Exit chắc chắn luôn chạy.

Nếu trạng thái quan trọng, hãy có thêm đường dọn dẹp trong `OnDisable`, `OnDestroy`, lúc đổi scene hoặc lúc reset màn chơi.

### Nhiều collider và lỗi bật/tắt quá sớm

Giả sử player có hai collider. Collider A rời vùng trước nhưng collider B vẫn còn ở trong. Nếu chỉ dùng một biến `bool`, `OnTriggerExit2D` của A có thể đặt `isInside = false` quá sớm.

Có thể dùng bộ đếm:

```csharp
private int playerColliderCount;

private void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Player"))
        return;

    playerColliderCount++;
}

private void OnTriggerExit2D(Collider2D other)
{
    if (!other.CompareTag("Player"))
        return;

    playerColliderCount = Mathf.Max(0, playerColliderCount - 1);
}

private bool IsPlayerInside => playerColliderCount > 0;
```

Nếu có khả năng nhận Enter trùng cho cùng một collider do logic khác, dùng `HashSet<Collider2D>` sẽ chắc chắn hơn.

---

## 6. Trigger trả về cái gì?

Ba callback trigger đều có kiểu `void`:

```csharp
private void OnTriggerEnter2D(Collider2D other)
private void OnTriggerStay2D(Collider2D other)
private void OnTriggerExit2D(Collider2D other)
```

`void` nghĩa là hàm **không trả về giá trị** cho code của bạn. Đây là message/callback do Unity gọi.

Thứ bạn nhận được là tham số:

```csharp
Collider2D other
```

Từ `other`, có thể truy cập:

```csharp
other.gameObject          // GameObject sở hữu collider
other.transform           // Transform của collider
other.attachedRigidbody   // Rigidbody2D gắn với collider, có thể null
other.bounds              // Bounds của collider
other.isTrigger           // Collider bên kia có phải trigger không
other.gameObject.layer    // Layer dưới dạng số
other.CompareTag("Player")
```

Ví dụ lấy `Rigidbody2D`:

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    Rigidbody2D body = other.attachedRigidbody;

    if (body != null)
    {
        body.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
    }
}
```

Lưu ý: trigger callback chỉ cung cấp `Collider2D`, không cung cấp đầy đủ dữ liệu tiếp xúc giống `Collision2D`. Trigger không phải va chạm cứng, nên nếu cần điểm chạm và normal theo kiểu collision, bạn thường phải dùng cast/query thích hợp hoặc thiết kế lại bằng collision.

---

## 7. Ví dụ Trigger 2D hoàn chỉnh

### Ví dụ 1: Coin chỉ được nhặt một lần

```csharp
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        PlayerInventory inventory =
            other.GetComponentInParent<PlayerInventory>();

        if (inventory == null)
            return;

        collected = true;
        inventory.AddCoins(value);
        Destroy(gameObject);
    }
}
```

Biến `collected` ngăn xử lý trùng trong trường hợp player có nhiều collider hoặc nhiều callback xảy ra trước khi object bị destroy vào cuối frame.

### Ví dụ 2: Vùng tương tác dùng Enter và Exit

```csharp
using UnityEngine;

public class InteractionZone : MonoBehaviour
{
    private PlayerController currentPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null)
            return;

        currentPlayer = player;
        currentPlayer.ShowInteractHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null || player != currentPlayer)
            return;

        currentPlayer.ShowInteractHint(false);
        currentPlayer = null;
    }

    private void OnDisable()
    {
        if (currentPlayer != null)
            currentPlayer.ShowInteractHint(false);

        currentPlayer = null;
    }
}
```

### Ví dụ 3: Theo dõi nhiều mục tiêu bằng `HashSet`

```csharp
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionZone : MonoBehaviour
{
    private readonly HashSet<Enemy> enemies = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
            enemies.Add(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
            enemies.Remove(enemy);
    }

    private void OnDisable()
    {
        enemies.Clear();
    }
}
```

`HashSet` tự tránh phần tử trùng, khá tiện khi một nhân vật có nhiều collider con.

---

## 8. Raycast 2D là gì?

Raycast 2D bắn một đường thẳng vô hình trong scene 2D. Tia có:

- `origin`: điểm bắt đầu
- `direction`: hướng
- `distance`: khoảng cách tối đa
- `layerMask`: những layer được phép trúng
- Các bộ lọc bổ sung tùy overload

Hình dung:

```text
origin o--------------------------> direction
             X collider bị trúng
```

Tia không có bề rộng. Nếu muốn kiểm tra một vùng dày bằng kích thước nhân vật, hãy dùng `CircleCast`, `BoxCast`, `CapsuleCast` hoặc cast chính collider.

Raycast không tự chạy. Mỗi lần cần kiểm tra, bạn phải gọi `Physics2D.Raycast(...)`.

---

## 9. `Physics2D.Raycast`

Overload dễ dùng nhất có dạng:

```csharp
RaycastHit2D Physics2D.Raycast(
    Vector2 origin,
    Vector2 direction,
    float distance = Mathf.Infinity,
    int layerMask = Physics2D.DefaultRaycastLayers,
    float minDepth = -Mathf.Infinity,
    float maxDepth = Mathf.Infinity
);
```

Không cần ghi hết mọi tham số. Ví dụ tối giản:

```csharp
RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
```

Nhưng trong game thật, thường nên giới hạn khoảng cách và layer:

```csharp
[SerializeField] private float groundCheckDistance = 0.2f;
[SerializeField] private LayerMask groundLayer;

private bool IsGrounded()
{
    RaycastHit2D hit = Physics2D.Raycast(
        transform.position,
        Vector2.down,
        groundCheckDistance,
        groundLayer
    );

    return hit.collider != null;
}
```

### Ý nghĩa từng tham số

#### `origin`

Điểm bắt đầu trong tọa độ world.

```csharp
Vector2 origin = transform.position;
```

Với ground check, đừng luôn bắn từ tâm nhân vật nếu collider cao. Có thể đặt một empty GameObject ở chân:

```csharp
Vector2 origin = groundCheckPoint.position;
```

#### `direction`

Hướng tia bay. Các hướng quen thuộc:

```csharp
Vector2.down
Vector2.up
Vector2.left
Vector2.right
```

Hướng từ A đến B:

```csharp
Vector2 direction = (target.position - transform.position).normalized;
```

Unity có thể xử lý vector chưa normalize, nhưng normalize giúp ý nghĩa của hướng và khoảng cách rõ ràng, tránh code khó đọc.

#### `distance`

Độ dài tối đa của tia theo world unit.

```csharp
float distance = 5f;
```

Không nên để `Mathf.Infinity` theo thói quen khi chỉ cần kiểm tra gần nhân vật. Giới hạn hợp lý làm ý đồ rõ hơn và thường giảm công việc truy vấn.

#### `layerMask`

Bộ lọc layer. Ray chỉ xét collider thuộc những layer đã chọn.

```csharp
[SerializeField] private LayerMask groundLayer;
```

Sau đó kéo chọn layer trong Inspector.

#### `minDepth`, `maxDepth`

Lọc collider theo tọa độ Z. Trong game 2D, Z đôi khi được dùng để chia lớp độ sâu. Hai tham số này không phải khoảng cách tia; chúng lọc object theo Z.

Phần lớn game 2D không cần truyền hai giá trị này.

### Raycast trả về cái gì?

Overload trên trả về một `RaycastHit2D`, đại diện cho hit gần nhất. Nếu không trúng collider nào thì:

```csharp
hit.collider == null
```

Có thể viết:

```csharp
if (hit.collider != null)
{
    Debug.Log($"Trúng {hit.collider.name}");
}
```

`RaycastHit2D` có chuyển đổi ngầm sang `bool`, nên cú pháp này cũng dùng được:

```csharp
if (hit)
{
    Debug.Log($"Trúng {hit.collider.name}");
}
```

Tuy vậy, `hit.collider != null` thường dễ hiểu hơn với người mới.

---

## 10. Đọc dữ liệu trong `RaycastHit2D`

Ví dụ:

```csharp
RaycastHit2D hit = Physics2D.Raycast(
    firePoint.position,
    firePoint.right,
    20f,
    hittableLayer
);

if (hit.collider == null)
    return;

Debug.Log($"Object: {hit.collider.name}");
Debug.Log($"Điểm chạm: {hit.point}");
Debug.Log($"Pháp tuyến: {hit.normal}");
Debug.Log($"Khoảng cách: {hit.distance}");
Debug.Log($"Tỉ lệ trên tia: {hit.fraction}");
```

Các thuộc tính hay dùng:

| Thuộc tính | Ý nghĩa |
|---|---|
| `collider` | `Collider2D` bị trúng; `null` nếu trượt |
| `transform` | Transform liên quan đến collider bị trúng |
| `rigidbody` | `Rigidbody2D` gắn với collider, có thể `null` |
| `point` | Điểm tia chạm vào bề mặt trong world space |
| `normal` | Vector vuông góc với bề mặt tại điểm chạm |
| `distance` | Khoảng cách từ origin tới hit |
| `fraction` | Tỉ lệ vị trí hit trên toàn độ dài cast |
| `centroid` | Vị trí tâm của shape tại thời điểm shape cast chạm vật |

`normal` rất hữu ích để:

- Biết mặt đất nghiêng theo hướng nào
- Phản xạ đạn
- Căn hiệu ứng theo bề mặt
- Phân biệt đang chạm sàn, tường hay trần

Ví dụ kiểm tra bề mặt có đủ hướng lên để coi là mặt đất:

```csharp
bool isGround = hit.collider != null && hit.normal.y > 0.7f;
```

### Trường hợp origin nằm bên trong collider

`Physics2D.Raycast` có thể báo collider nằm ngay tại điểm bắt đầu. Vì tia bắt đầu bên trong nên không có một bề mặt đi vào rõ ràng để tính normal theo cách thông thường. Kết quả dạng này có `fraction == 0`, và normal được đặt ngược hướng tia.

Nếu không muốn tự trúng collider của nhân vật:

- Đặt player và ground/enemy vào layer khác nhau rồi lọc bằng `LayerMask`.
- Đặt origin ra ngoài collider của player.
- Dùng `Collider2D.Cast` hoặc một cách kiểm tra phù hợp hơn với hình dạng thật.

---

## 11. LayerMask và bộ lọc

### 11.1. Dùng `LayerMask` từ Inspector

Đây là cách dễ bảo trì:

```csharp
[SerializeField] private LayerMask targetLayers;

private void Shoot()
{
    RaycastHit2D hit = Physics2D.Raycast(
        transform.position,
        transform.right,
        10f,
        targetLayers
    );
}
```

`LayerMask` có thể chứa nhiều layer cùng lúc.

### 11.2. Tạo mask bằng code

```csharp
int mask = LayerMask.GetMask("Ground", "Platform");
```

Chỉ layer `Ground`:

```csharp
int groundMask = 1 << LayerMask.NameToLayer("Ground");
```

Mọi layer trừ Player:

```csharp
int playerLayer = LayerMask.NameToLayer("Player");
int maskWithoutPlayer = ~(1 << playerLayer);
```

### 11.3. Lỗi kinh điển: truyền số layer thay vì bitmask

Sai nếu `Ground` có index 6:

```csharp
int groundLayer = LayerMask.NameToLayer("Ground");
Physics2D.Raycast(origin, direction, distance, groundLayer);
```

`layerMask` cần một **bitmask**, không phải index layer. Đúng:

```csharp
int groundMask = LayerMask.GetMask("Ground");
```

hoặc:

```csharp
int groundMask = 1 << LayerMask.NameToLayer("Ground");
```

### 11.4. Layer index và LayerMask không phải cùng một thứ

Mỗi GameObject chỉ nằm trên **một layer**, được lưu bằng một số từ `0` đến `31`:

```csharp
int objectLayerIndex = gameObject.layer;
```

Trong khi đó, `LayerMask` là một số nguyên có 32 bit. Mỗi bit đại diện cho trạng thái bật/tắt của một layer:

```text
Layer index:   3   2   1   0
Mask bit:      1   0   1   0
               ^       ^
             lấy 3    lấy 1
```

Vì thế:

- `gameObject.layer` là **một index**.
- Tham số `layerMask` của raycast là **một tập hợp layer**.
- Không truyền thẳng `gameObject.layer` vào vị trí cần mask.

Chuyển index thành mask bằng phép dịch bit:

```csharp
int layerIndex = gameObject.layer;
int mask = 1 << layerIndex;
```

Ví dụ layer số 6 sẽ trở thành bit thứ 6 được bật, tức `1 << 6`.

### 11.5. Chọn một hoặc nhiều layer

Chọn bằng tên thường dễ đọc nhất:

```csharp
int groundMask = LayerMask.GetMask("Ground");
int environmentMask = LayerMask.GetMask("Ground", "Wall", "Platform");
```

Ghép các mask có sẵn bằng toán tử OR `|`:

```csharp
int groundMask = LayerMask.GetMask("Ground");
int platformMask = LayerMask.GetMask("Platform");
int combinedMask = groundMask | platformMask;
```

Hoặc ghép từ index:

```csharp
int groundIndex = LayerMask.NameToLayer("Ground");
int platformIndex = LayerMask.NameToLayer("Platform");

int combinedMask = (1 << groundIndex) | (1 << platformIndex);
```

Trong đa số trường hợp, `LayerMask.GetMask(...)` gọn và ít lỗi hơn.

### 11.6. Loại trừ một layer

Toán tử `~` đảo toàn bộ bit. Ví dụ lấy mọi layer trừ Player:

```csharp
int playerMask = LayerMask.GetMask("Player");
int everythingExceptPlayer = ~playerMask;
```

Loại trừ nhiều layer:

```csharp
int ignoredMask = LayerMask.GetMask("Player", "UI", "Pickup");
int queryMask = ~ignoredMask;
```

Dùng vào raycast:

```csharp
RaycastHit2D hit = Physics2D.Raycast(
    origin,
    direction,
    20f,
    queryMask
);
```

Một số giá trị thường gặp:

```csharp
int nothing = 0;             // Không lấy layer nào
int everything = ~0;         // Lấy tất cả 32 layer
int allLayers = Physics2D.AllLayers;
int defaultRaycastLayers = Physics2D.DefaultRaycastLayers;
```

`DefaultRaycastLayers` không hoàn toàn đồng nghĩa với “mọi layer”; theo cấu hình mặc định, layer `Ignore Raycast` được loại khỏi các truy vấn raycast mặc định.

### 11.7. Kiểm tra một GameObject có nằm trong mask không

Đây là mẫu bitwise rất hữu ích trong callback trigger:

```csharp
[SerializeField] private LayerMask acceptedLayers;

private bool IsInLayerMask(GameObject target)
{
    return (acceptedLayers.value & (1 << target.layer)) != 0;
}
```

Áp dụng:

```csharp
[SerializeField] private LayerMask damageableLayers;

private void OnTriggerEnter2D(Collider2D other)
{
    bool isAccepted =
        (damageableLayers.value & (1 << other.gameObject.layer)) != 0;

    if (!isAccepted)
        return;

    Debug.Log($"{other.name} thuộc layer được chấp nhận");
}
```

Giải thích biểu thức:

```csharp
damageableLayers.value       // toàn bộ các bit đang bật trong mask
1 << other.gameObject.layer  // bit của layer bên kia
&                            // chỉ giữ bit xuất hiện ở cả hai bên
!= 0                         // khác 0 nghĩa là layer có trong mask
```

Nếu phải dùng nhiều nơi, viết extension method cho dễ đọc:

```csharp
using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool Contains(this LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
```

Sử dụng:

```csharp
if (!damageableLayers.Contains(other.gameObject.layer))
    return;
```

### 11.8. Cẩn thận với `LayerMask.NameToLayer`

Nếu tên layer không tồn tại, `LayerMask.NameToLayer` trả về `-1`:

```csharp
int layerIndex = LayerMask.NameToLayer("Ground");
```

Do đó, nên kiểm tra khi tạo mask động:

```csharp
int groundIndex = LayerMask.NameToLayer("Ground");

if (groundIndex == -1)
{
    Debug.LogError("Chưa tạo layer Ground trong Project Settings");
    return;
}

int groundMask = 1 << groundIndex;
```

`LayerMask.GetMask("TênKhôngTồnTại")` không thể tự tạo layer mới. Nếu tất cả tên đưa vào đều không hợp lệ, kết quả mask có thể bằng `0`, khiến raycast không trúng gì.

Tên layer phân biệt chính xác theo chuỗi, nên `"Ground"` và `"ground"` không nên được xem là giống nhau.

### 11.9. LayerMask trong Inspector

Khai báo thế này:

```csharp
[SerializeField] private LayerMask groundLayers;
```

Inspector sẽ hiện một danh sách cho phép tick nhiều layer. Đây thường là lựa chọn tốt nhất vì:

- Designer thay đổi được mà không sửa code.
- Nhìn Inspector biết query đang nhận layer nào.
- Không phụ thuộc vào chuỗi `"Ground"` nằm rải rác trong code.
- Có thể dùng prefab khác nhau với mask khác nhau.

Ví dụ component ground check hoàn chỉnh:

```csharp
using UnityEngine;

public class SimpleGroundCheck : MonoBehaviour
{
    [SerializeField] private Transform checkPoint;
    [SerializeField] private float distance = 0.2f;
    [SerializeField] private LayerMask groundLayers;

    public bool IsGrounded { get; private set; }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            checkPoint.position,
            Vector2.down,
            distance,
            groundLayers
        );

        IsGrounded = hit.collider != null;
    }
}
```

Tên biến số nhiều như `groundLayers` nhắc người đọc rằng mask có thể chứa nhiều layer, dù hiện tại Inspector chỉ chọn một layer.

### 11.10. LayerMask của query và Layer Collision Matrix khác nhau

Hai thứ này đều nói về layer nhưng làm hai việc khác nhau.

#### Layer Collision Matrix

Thiết lập ở:

```text
Edit > Project Settings > Physics 2D > Layer Collision Matrix
```

Nó quyết định những cặp layer nào được hệ vật lý 2D cho phép tạo contact, collision và trigger interaction nói chung.

Ví dụ bỏ tick giữa `Player` và `EnemySensor` có thể khiến callback trigger giữa hai layer không được gửi.

#### LayerMask truyền vào raycast

Mask của raycast chỉ lọc **lần query cụ thể đó**:

```csharp
Physics2D.Raycast(origin, direction, distance, groundLayers);
```

Nó không đổi layer của object, không sửa Layer Collision Matrix và không ảnh hưởng các raycast khác.

Có thể ghi nhớ:

```text
Layer Collision Matrix = luật tương tác chung của project
LayerMask trong raycast = bộ lọc riêng của câu hỏi hiện tại
```

### 11.11. LayerMask khác Tag như thế nào?

Một GameObject có:

- Một `Layer`
- Một `Tag`

Layer thường hợp với hệ vật lý và camera:

- Raycast lọc Ground, Enemy, Wall
- Camera culling
- Quy tắc va chạm giữa các nhóm

Tag thường hợp với nhận dạng vai trò gameplay:

- Đây có phải Player không?
- Đây có phải Boss không?
- Đây có phải Checkpoint không?

Bạn có thể dùng cả hai:

```csharp
RaycastHit2D hit = Physics2D.Raycast(
    origin,
    direction,
    10f,
    hittableLayers
);

if (hit.collider != null && hit.collider.CompareTag("Enemy"))
{
    Debug.Log("Bắn trúng enemy");
}
```

Layer lọc sớm để hệ vật lý không trả về những nhóm không quan tâm; tag/component dùng để quyết định hành vi sau khi đã hit.

### 11.12. LayerMask có quyết định raycast trúng Trigger không?

LayerMask chỉ quyết định **layer nào** được xét. Việc collider có `Is Trigger` được raycast nhận hay bỏ qua là một bộ lọc khác.

Với các overload đơn giản của `Physics2D.Raycast`, hành vi này chịu ảnh hưởng bởi cài đặt query trigger toàn cục của Physics 2D. Với overload dùng `ContactFilter2D`, có thể nói rõ:

```csharp
ContactFilter2D filter = new ContactFilter2D();
filter.SetLayerMask(targetLayers);
filter.useTriggers = false; // bỏ qua trigger
```

Hoặc:

```csharp
filter.useTriggers = true; // cho phép nhận trigger
```

Như vậy một query có thể đồng thời yêu cầu:

```text
Chỉ layer Enemy + bỏ qua mọi collider Is Trigger
```

### 11.13. `ContactFilter2D`

Khi cần bộ lọc chi tiết và lấy nhiều kết quả vào mảng/List, dùng `ContactFilter2D`:

```csharp
private readonly RaycastHit2D[] results = new RaycastHit2D[16];

private int Scan(Vector2 origin, Vector2 direction)
{
    ContactFilter2D filter = new ContactFilter2D();
    filter.SetLayerMask(LayerMask.GetMask("Enemy"));
    filter.useTriggers = false;

    return Physics2D.Raycast(
        origin,
        direction,
        filter,
        results,
        10f
    );
}
```

`ContactFilter2D` có thể lọc theo:

- Layer mask
- Z depth
- Góc của normal
- Có lấy trigger hay không

Nếu không muốn lọc gì:

```csharp
ContactFilter2D filter = new ContactFilter2D().NoFilter();
```

Tên property có thể khác nhẹ giữa các phiên bản Unity; nếu IDE báo lỗi, kiểm tra Scripting API đúng phiên bản dự án.

---

## 12. Các biến thể của Raycast 2D

### 12.1. `Physics2D.Raycast`: lấy hit gần nhất

```csharp
RaycastHit2D hit = Physics2D.Raycast(
    origin,
    direction,
    distance,
    layerMask
);
```

Dùng khi chỉ quan tâm vật đầu tiên trên đường tia, ví dụ viên đạn hitscan bị tường chặn.

### 12.2. `Physics2D.RaycastAll`: lấy tất cả hit

```csharp
RaycastHit2D[] hits = Physics2D.RaycastAll(
    origin,
    direction,
    distance,
    layerMask
);

foreach (RaycastHit2D hit in hits)
{
    Debug.Log(hit.collider.name);
}
```

Hàm trả về `RaycastHit2D[]`. Kết quả 2D được sắp theo khoảng cách tăng dần, nên phần tử đầu thường là hit gần nhất.

Dùng khi:

- Tia xuyên qua nhiều kẻ địch
- Laser tác động lên tất cả mục tiêu trên đường
- Muốn tự quyết định collider nào sẽ chặn tia

Nhược điểm: mỗi lần gọi có thể tạo mảng mới, gây cấp phát bộ nhớ. Gọi thỉnh thoảng thì thường không sao; gọi dày đặc cho nhiều đối tượng thì nên dùng overload nhận buffer/List.

### 12.3. Overload ghi vào mảng có sẵn

```csharp
private readonly RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

private void Scan()
{
    ContactFilter2D filter = new ContactFilter2D();
    filter.SetLayerMask(LayerMask.GetMask("Enemy"));
    filter.useTriggers = false;

    int hitCount = Physics2D.Raycast(
        transform.position,
        transform.right,
        filter,
        hitBuffer,
        10f
    );

    for (int i = 0; i < hitCount; i++)
    {
        Debug.Log(hitBuffer[i].collider.name);
    }
}
```

Hàm này trả về `int`: số kết quả đã ghi vào mảng.

Điểm cần nhớ:

- Kích thước mảng là số hit tối đa giữ được.
- Nếu có 30 hit mà buffer chỉ có 16 phần tử, bạn chỉ nhận tối đa 16 kết quả.
- Chỉ đọc từ index `0` tới `hitCount - 1`.
- Không cần xóa toàn bộ mảng sau mỗi lần gọi.
- Tái sử dụng buffer giúp tránh tạo rác bộ nhớ liên tục.

### 12.4. Overload ghi vào `List<RaycastHit2D>`

```csharp
using System.Collections.Generic;
using UnityEngine;

public class RayScanner : MonoBehaviour
{
    private readonly List<RaycastHit2D> hits = new(16);

    public void Scan()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Enemy"));

        int hitCount = Physics2D.Raycast(
            transform.position,
            transform.right,
            filter,
            hits,
            10f
        );

        for (int i = 0; i < hitCount; i++)
            Debug.Log(hits[i].collider.name);
    }
}
```

Hàm cũng trả về `int`. Unity có thể resize List nếu capacity không đủ. Nếu List đã có capacity đủ lớn và được tái sử dụng, cách này giảm cấp phát bộ nhớ.

### 12.5. `RaycastNonAlloc`

Bạn có thể gặp `Physics2D.RaycastNonAlloc` trong tutorial cũ. Ở các API Unity mới, nhóm hàm `NonAlloc` 2D được hướng tới thay thế bằng chính overload `Physics2D.Raycast` nhận `RaycastHit2D[]` hoặc `List<RaycastHit2D>`.

Nói ngắn gọn:

- Code cũ: `Physics2D.RaycastNonAlloc(...)`
- Code mới nên ưu tiên: `Physics2D.Raycast(..., results, ...)`

Hai kiểu cùng theo ý tưởng: đưa buffer có sẵn vào để tránh tạo mảng mới mỗi lần.

### 12.6. `Physics2D.Linecast`

Raycast nhận **điểm bắt đầu + hướng + khoảng cách**. Linecast nhận thẳng **điểm bắt đầu + điểm kết thúc**:

```csharp
RaycastHit2D hit = Physics2D.Linecast(
    eye.position,
    player.position,
    obstacleLayer
);
```

Linecast rất tự nhiên khi kiểm tra đường nhìn giữa hai điểm:

```csharp
bool HasClearLineOfSight(Transform target)
{
    RaycastHit2D hit = Physics2D.Linecast(
        eye.position,
        target.position,
        obstacleLayer
    );

    return hit.collider == null;
}
```

Có các biến thể tương tự như `LinecastAll` và overload dùng kết quả có sẵn, tùy phiên bản Unity.

### 12.7. `Physics2D.GetRayIntersection`

Hàm này nhận một `Ray` 3D nhưng kiểm tra giao cắt với các `Collider2D`. Nó hữu ích khi camera tạo ray 3D từ vị trí chuột:

```csharp
Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

if (hit.collider != null)
{
    Debug.Log($"Đã click vào {hit.collider.name}");
}
```

Đừng nhầm nó với `Physics.Raycast` 3D. `GetRayIntersection` vẫn trả về dữ liệu hit của hệ 2D.

---

## 13. Các shape cast: Circle, Box, Capsule

Một ray mỏng có thể lọt qua mép vật hoặc không đại diện đúng kích thước nhân vật. Shape cast giải quyết việc này bằng cách quét cả một hình qua không gian.

### 13.1. `Physics2D.CircleCast`

Hình dung như kéo một hình tròn từ origin theo direction:

```csharp
RaycastHit2D hit = Physics2D.CircleCast(
    origin: transform.position,
    radius: 0.4f,
    direction: Vector2.down,
    distance: 0.2f,
    layerMask: groundLayer
);
```

Dùng cho:

- Ground check ổn định hơn một tia mỏng
- Kiểm tra đường đi của vật tròn
- Vùng dò có độ dày

Kết quả vẫn là `RaycastHit2D`. Có `CircleCastAll` và overload ghi nhiều kết quả vào collection.

### 13.2. `Physics2D.BoxCast`

Quét một hình chữ nhật:

```csharp
RaycastHit2D hit = Physics2D.BoxCast(
    origin: boxCenter,
    size: boxSize,
    angle: 0f,
    direction: Vector2.down,
    distance: groundCheckDistance,
    layerMask: groundLayer
);
```

Các tham số đáng chú ý:

- `size`: kích thước đầy đủ của box, không phải half extents.
- `angle`: góc quay tính bằng độ.
- `direction`: hướng quét.
- `distance`: quãng đường quét, không tính kích thước box.

BoxCast rất hợp với nhân vật có `BoxCollider2D`, kiểm tra mặt đất dưới toàn bộ bề ngang chân hoặc kiểm tra không gian trước mặt.

### 13.3. `Physics2D.CapsuleCast`

Quét một capsule:

```csharp
RaycastHit2D hit = Physics2D.CapsuleCast(
    origin: transform.position,
    size: new Vector2(0.8f, 1.8f),
    capsuleDirection: CapsuleDirection2D.Vertical,
    angle: 0f,
    direction: movement.normalized,
    distance: movement.magnitude,
    layerMask: obstacleLayer
);
```

Dùng khi collider chính của nhân vật là capsule hoặc khi muốn mô phỏng đường quét có đầu tròn.

### 13.4. Bản `All` và bản nhận buffer/List

Các shape cast thường có cùng kiểu biến thể:

- `CircleCast`: hit gần nhất
- `CircleCastAll`: tất cả hit, trả mảng
- `CircleCast` overload nhận array/List: ghi nhiều hit vào collection có sẵn
- Tương tự với `BoxCast` và `CapsuleCast`

Tutorial cũ có thể dùng `CircleCastNonAlloc`, `BoxCastNonAlloc`, `CapsuleCastNonAlloc`. Với Unity mới, ưu tiên overload cùng tên nhận collection nếu phiên bản dự án hỗ trợ.

### 13.5. Cast không giống kiểm tra overlap ban đầu

Cast trả lời câu hỏi “nếu hình này di chuyển theo hướng đó thì nó gặp gì?”. Nếu shape đã nằm chồng trong collider ngay từ đầu, thông tin `point`, `normal` hoặc khoảng cách có thể có quy ước đặc biệt.

Nếu câu hỏi thực sự là “ngay tại vị trí này đang có collider nào?”, dùng nhóm `Overlap...` sẽ đúng ý hơn.

---

## 14. `Collider2D.Cast` và `Rigidbody2D.Cast`

Thay vì tự nhập kích thước cho CircleCast/BoxCast, bạn có thể cast chính collider đang có.

### `Collider2D.Cast`

```csharp
using UnityEngine;

public class CharacterMover2D : MonoBehaviour
{
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private LayerMask obstacleLayer;

    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];

    private bool CanMove(Vector2 movement)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(obstacleLayer);
        filter.useTriggers = false;

        int count = bodyCollider.Cast(
            movement.normalized,
            filter,
            hits,
            movement.magnitude
        );

        return count == 0;
    }
}
```

Ưu điểm là hình dạng, kích thước, rotation và offset lấy trực tiếp từ collider thật. Không phải giữ cho tham số BoxCast khớp thủ công với Inspector.

### `Rigidbody2D.Cast`

`Rigidbody2D.Cast` có thể cast các collider gắn với rigidbody, phù hợp khi một body có nhiều collider tạo thành hình phức hợp.

Dùng hai hàm này khi kiểm tra chuyển động của chính vật thể. Dùng `Physics2D.CircleCast/BoxCast/CapsuleCast` khi muốn truy vấn bằng một hình độc lập, không nhất thiết trùng collider hiện có.

---

## 15. Overlap khác Cast như thế nào?

Đây là chỗ người mới rất dễ chọn nhầm API.

### Cast

Cast quét một hình theo một hướng và một khoảng cách:

```text
[hình tại A] =====quét=====> [hình tại B]
```

Ví dụ:

- `Raycast`
- `CircleCast`
- `BoxCast`
- `CapsuleCast`

Dùng để hỏi “trên đường đi sẽ gặp gì?”.

### Overlap

Overlap kiểm tra ngay một vùng tại chỗ, không có hướng và không có quãng đường quét:

- `Physics2D.OverlapPoint`
- `Physics2D.OverlapCircle`
- `Physics2D.OverlapBox`
- `Physics2D.OverlapCapsule`
- `Physics2D.OverlapArea`
- `Physics2D.OverlapCollider`

Dùng để hỏi “ngay trong vùng này đang có gì?”.

Ví dụ đòn đánh cận chiến:

```csharp
Collider2D[] targets = Physics2D.OverlapCircleAll(
    attackPoint.position,
    attackRadius,
    enemyLayer
);

foreach (Collider2D target in targets)
{
    if (target.TryGetComponent<EnemyHealth>(out EnemyHealth health))
        health.TakeDamage(20);
}
```

Nếu chỉ cần biết có hay không, dùng bản trả một collider hoặc overload buffer phù hợp thay vì luôn lấy toàn bộ mảng.

---

## 16. Nên dùng hàm nào trong từng tình huống?

| Tình huống | Lựa chọn gợi ý |
|---|---|
| Player bước vào vùng checkpoint | `OnTriggerEnter2D` |
| Gây độc khi còn đứng trong vùng | Enter/Exit quản lý trạng thái, hoặc `OnTriggerStay2D` cho trường hợp đơn giản |
| Tắt UI khi player rời NPC | `OnTriggerExit2D` |
| Kiểm tra dưới chân có nền không | `Raycast`, `CircleCast` hoặc `BoxCast` |
| Kiểm tra giữa mắt AI và player có tường | `Linecast` hoặc `Raycast` |
| Đạn hitscan trúng vật đầu tiên | `Raycast` |
| Laser xuyên nhiều mục tiêu | `RaycastAll` hoặc overload dùng buffer |
| Nhân vật có vừa với đường đi không | `CapsuleCast`, `BoxCast` hoặc `Collider2D.Cast` |
| Đòn đánh tròn ngay tại chỗ | `OverlapCircle`/`OverlapCircleAll` |
| Chọn object 2D bằng ray từ camera | `Physics2D.GetRayIntersection` hoặc đổi tọa độ chuột sang world rồi `OverlapPoint` |
| Kiểm tra chuyển động của collider thật | `Collider2D.Cast` |
| Body có nhiều collider cần quét cùng nhau | `Rigidbody2D.Cast` |

Quy tắc chọn nhanh:

1. Một vùng tồn tại lâu và cần biết Enter/Stay/Exit: dùng trigger.
2. Một câu hỏi tức thời theo đường thẳng: dùng Raycast/Linecast.
3. Cần độ dày hoặc kích thước nhân vật: dùng shape cast.
4. Chỉ cần kiểm tra một vùng tại chỗ: dùng Overlap.

---

## 17. Lỗi thường gặp và cách debug

### Trigger không chạy

Kiểm tra lần lượt:

1. Cả hai bên có đúng `Collider2D` chưa?
2. Ít nhất một collider có bật `Is Trigger` chưa?
3. Ít nhất một bên có `Rigidbody2D` chưa?
4. Script có nằm trên GameObject liên quan đến collider/rigidbody không?
5. Tên hàm có đúng `OnTriggerEnter2D` không?
6. Tham số có đúng `Collider2D other` không?
7. Hai layer có được phép tương tác trong Physics 2D Layer Collision Matrix không?
8. Có đang trộn component 2D và 3D không?
9. GameObject hoặc component có bị disable không?
10. Vật thể có di chuyển quá nhanh và bỏ qua vùng mỏng không?

Với vật thể nhanh, cân nhắc:

- Tăng độ dày trigger.
- Chọn Collision Detection phù hợp trên `Rigidbody2D`.
- Dùng cast giữa vị trí cũ và vị trí mới nếu cần phát hiện chắc chắn.
- Di chuyển bằng API vật lý phù hợp thay vì thay `transform.position` tùy tiện.

### Raycast không trúng

Kiểm tra:

- `direction` có phải `Vector2.zero` không?
- `distance` có quá ngắn không?
- Origin có đúng vị trí world không?
- LayerMask có chứa layer mục tiêu không?
- Mục tiêu có `Collider2D`, không phải collider 3D, đúng không?
- Cài đặt query có bỏ qua trigger không?
- Origin có nằm trong chính collider gây kết quả khó hiểu không?
- Transform vừa thay đổi nhưng physics state chưa được đồng bộ chưa?

### Raycast tự trúng player

Giải pháp tốt nhất thường là layer:

- Player ở layer `Player`.
- Ground ở layer `Ground`.
- Ray ground check chỉ nhận `Ground`.

Đừng bắn tia rồi mới kiểm tra tên object để bỏ qua player nếu có thể lọc ngay từ query.

### Dùng sai hướng local và world

`Vector2.right` luôn là hướng phải của world. Nếu sprite quay bằng Transform và muốn bắn theo trục local:

```csharp
Vector2 direction = transform.right;
```

Trong nhiều game 2D, nhân vật flip bằng cách đổi `localScale.x` hoặc `SpriteRenderer.flipX`; hai cách này không phải lúc nào cũng làm `transform.right` đổi như bạn tưởng. Cách rõ ràng là giữ biến hướng nhìn:

```csharp
private int facingDirection = 1; // 1 phải, -1 trái
Vector2 direction = Vector2.right * facingDirection;
```

### Nhầm `size` với half size

Trong `Physics2D.BoxCast`, `size` là kích thước đầy đủ. Nếu collider rộng 1 và cao 2:

```csharp
Vector2 size = new Vector2(1f, 2f);
```

Không chia đôi như tham số `halfExtents` của một số API 3D.

### Không thấy tia trong Scene

Raycast là phép tính, Unity không tự vẽ tia. Dùng `Debug.DrawRay`:

```csharp
Vector2 origin = groundCheckPoint.position;
Vector2 direction = Vector2.down;
float distance = 0.3f;

Debug.DrawRay(
    origin,
    direction * distance,
    Color.red
);
```

`Debug.DrawRay` nhận vector độ dài, nên cần nhân `direction * distance`.

Để xem rõ hơn, mở Scene view và bật Gizmos.

Với BoxCast/CircleCast, có thể viết `OnDrawGizmosSelected` để vẽ vị trí đầu và cuối của shape. Gizmo chỉ minh họa; nó không phải kết quả vật lý thật.

---

## 18. Tối ưu hiệu năng

Không cần ám ảnh tối ưu quá sớm, nhưng nên giữ vài thói quen tốt.

### Giới hạn layer và khoảng cách

```csharp
Physics2D.Raycast(origin, direction, 2f, groundLayer);
```

Tốt hơn về ý nghĩa và phạm vi truy vấn so với luôn dùng khoảng cách vô hạn, mọi layer.

### Đừng raycast nhiều lần cho cùng một câu hỏi

Kém gọn:

```csharp
if (Physics2D.Raycast(origin, direction, distance, mask).collider != null)
{
    Debug.Log(Physics2D.Raycast(origin, direction, distance, mask).point);
}
```

Tốt hơn:

```csharp
RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, mask);

if (hit.collider != null)
{
    Debug.Log(hit.point);
}
```

### Tái sử dụng array/List cho query chạy thường xuyên

Nếu lấy nhiều hit mỗi physics step, dùng overload nhận array/List có sẵn. Chọn buffer đủ lớn và theo dõi trường hợp `hitCount == buffer.Length`, vì đây có thể là dấu hiệu buffer đã đầy và kết quả bị cắt.

### Cache component hợp lý

Trong callback chạy liên tục, tránh tìm component lại nếu có thể lưu tham chiếu từ lúc Enter và xóa lúc Exit.

### Chọn `Update` hay `FixedUpdate` theo mục đích

- Kiểm tra phục vụ mô phỏng vật lý/chuyển động Rigidbody2D: thường đặt ở `FixedUpdate`.
- Kiểm tra theo input chuột mỗi frame: thường đặt ở `Update`.
- Callback Trigger: Unity tự gọi theo mô phỏng vật lý; không tự gọi lại chúng từ `Update`.

Không có luật “mọi raycast bắt buộc phải ở FixedUpdate”. Hãy chọn vòng lặp khớp với dữ liệu và hành vi cần điều khiển.

---

## 19. Bảng ghi nhớ nhanh

### Trigger callback

| Hàm | Khi được gọi | Trả về | Nhận vào |
|---|---|---|---|
| `OnTriggerEnter2D` | Bắt đầu overlap | `void` | `Collider2D other` |
| `OnTriggerStay2D` | Vẫn đang overlap qua các bước vật lý | `void` | `Collider2D other` |
| `OnTriggerExit2D` | Kết thúc overlap | `void` | `Collider2D other` |

### Query 2D

| Hàm | Câu hỏi nó trả lời | Kết quả thường gặp |
|---|---|---|
| `Physics2D.Raycast` | Tia gặp vật gần nhất nào? | `RaycastHit2D` |
| `Physics2D.RaycastAll` | Tia gặp tất cả vật nào? | `RaycastHit2D[]` |
| `Physics2D.Raycast(..., results, ...)` | Ghi nhiều hit vào buffer/List | `int` số hit |
| `Physics2D.Linecast` | Đoạn thẳng A-B gặp gì? | `RaycastHit2D` |
| `Physics2D.CircleCast` | Hình tròn quét theo hướng gặp gì? | `RaycastHit2D` |
| `Physics2D.BoxCast` | Hình hộp quét theo hướng gặp gì? | `RaycastHit2D` |
| `Physics2D.CapsuleCast` | Capsule quét theo hướng gặp gì? | `RaycastHit2D` |
| `Collider2D.Cast` | Collider cụ thể di chuyển sẽ gặp gì? | `int` và collection kết quả |
| `Rigidbody2D.Cast` | Cả body/các collider của nó di chuyển sẽ gặp gì? | `int` và collection kết quả |
| `Physics2D.Overlap...` | Ngay trong vùng này đang có gì? | Collider hoặc collection collider |

### Mẫu ground check gọn, thực dụng

```csharp
using UnityEngine;

public class GroundChecker2D : MonoBehaviour
{
    [SerializeField] private Transform checkPoint;
    [SerializeField] private Vector2 checkSize = new Vector2(0.7f, 0.1f);
    [SerializeField] private float checkDistance = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            checkPoint.position,
            checkSize,
            0f,
            Vector2.down,
            checkDistance,
            groundLayer
        );

        IsGrounded = hit.collider != null && hit.normal.y > 0.5f;
    }

    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null)
            return;

        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Vector3 end = checkPoint.position + Vector3.down * checkDistance;
        Gizmos.DrawWireCube(end, checkSize);
    }
}
```
