# Action, Delegate, UnityEvent và Coroutine trong Unity

## Mục lục

1. [Bức tranh tổng thể](#1-bức-tranh-tổng-thể)
2. [Delegate – hiểu sơ qua](#2-delegate--hiểu-sơ-qua)
3. [Action – delegate có sẵn](#3-action--delegate-có-sẵn)
4. [Event trong C#](#4-event-trong-c)
5. [UnityEvent – phần trọng tâm](#5-unityevent--phần-trọng-tâm)
6. [Cách đăng ký sự kiện với từng kiểu](#6-cách-đăng-ký-sự-kiện-với-từng-kiểu)
7. [Cách gọi sự kiện bằng Invoke](#7-cách-gọi-sự-kiện-bằng-invoke)
8. [Coroutine là gì?](#8-coroutine-là-gì)
9. [Luồng chạy của Coroutine](#9-luồng-chạy-của-coroutine)
10. [`yield return null`](#10-yield-return-null)
11. [`WaitForSeconds`](#11-waitforseconds)
12. [`StartCoroutine`](#12-startcoroutine)
13. [Dừng Coroutine](#13-dừng-coroutine)
14. [Các kiểu yield hữu ích khác](#14-các-kiểu-yield-hữu-ích-khác)
15. [Kết hợp UnityEvent và Coroutine](#15-kết-hợp-unityevent-và-coroutine)
16. [Lỗi thường gặp](#16-lỗi-thường-gặp)
17. [Bảng ghi nhớ nhanh](#17-bảng-ghi-nhớ-nhanh)

---

## 1. Bức tranh tổng thể

Trong bài này có hai nhóm kiến thức khá khác nhau:

- `delegate`, `Action`, C# `event` và `UnityEvent` dùng để **thông báo rằng một việc vừa xảy ra**.
- Coroutine dùng để **trải một công việc ra qua nhiều frame hoặc chờ một khoảng thời gian**.

Ví dụ trong game bắn súng:

```text
Player bắn
   ↓
Gun phát sự kiện OnShot
   ├── UI cập nhật số đạn
   ├── Audio phát tiếng súng
   └── Camera rung

Gun bắt đầu Coroutine hồi chiêu
   ↓
Chờ 1 giây
   ↓
Cho phép bắn tiếp
```

Gun chỉ cần báo “đã bắn”. Nó không nhất thiết phải biết UI, âm thanh hoặc camera sẽ xử lý thế nào. Đây là lý do event giúp code bớt dính chặt vào nhau.

Coroutine lại giải quyết phần “chờ 1 giây”. Nếu dùng một vòng lặp chờ bình thường, game có thể bị đứng. Coroutine cho phép Unity tiếp tục chạy những frame khác rồi quay lại công việc đang chờ.

---

## 2. Delegate – hiểu sơ qua

### 2.1. Delegate là gì?

Delegate là một kiểu dữ liệu có thể giữ tham chiếu tới một hoặc nhiều hàm.

Nói đơn giản:

> Biến bình thường giữ số hoặc chuỗi; biến delegate giữ một hàm để gọi sau.

Ví dụ tự khai báo một delegate:

```csharp
public delegate void SimpleCallback();
```

Dòng này tạo ra một kiểu delegate tên `SimpleCallback`. Nó chỉ nhận những hàm:

- Không có tham số.
- Trả về `void`.

Sử dụng:

```csharp
using UnityEngine;

public class DelegateExample : MonoBehaviour
{
    public delegate void SimpleCallback();

    private SimpleCallback callback;

    private void Start()
    {
        callback += SayHello;
        callback += PlaySound;

        callback?.Invoke();
    }

    private void SayHello()
    {
        Debug.Log("Xin chào");
    }

    private void PlaySound()
    {
        Debug.Log("Phát âm thanh");
    }
}
```

Khi gọi `callback`, hai hàm đã đăng ký sẽ lần lượt chạy.

### 2.2. Chữ ký hàm phải khớp

Delegate có tham số:

```csharp
public delegate void DamageCallback(int damage);
```

Hàm đăng ký phải nhận một `int` và trả về `void`:

```csharp
private void ShowDamage(int damage)
{
    Debug.Log("Sát thương: " + damage);
}
```

Đăng ký và gọi:

```csharp
private DamageCallback onDamage;

private void Start()
{
    onDamage += ShowDamage;
    onDamage?.Invoke(10);
}
```

Hàm này không thể đăng ký vì chữ ký không khớp:

```csharp
private void WrongMethod(string message)
{
}
```

### 2.3. Thêm và gỡ hàm

Đăng ký bằng `+=`:

```csharp
callback += SayHello;
```

Hủy đăng ký bằng `-=`:

```csharp
callback -= SayHello;
```

Gán bằng dấu `=` sẽ thay toàn bộ danh sách hiện tại:

```csharp
callback = SayHello;
```

Nếu mục đích là thêm một listener mới, thường dùng `+=` chứ không dùng `=`.

### 2.4. Tại sao có dấu `?`

```csharp
callback?.Invoke();
```

Delegate có thể đang là `null` vì chưa có hàm nào đăng ký. `?.Invoke()` có nghĩa là:

> Nếu callback khác null thì gọi; nếu đang null thì bỏ qua.

Viết đầy đủ sẽ là:

```csharp
if (callback != null)
{
    callback.Invoke();
}
```

---

## 3. Action – delegate có sẵn

### 3.1. Action là gì?

`Action` là một loại delegate được C# chuẩn bị sẵn. Khi dùng `Action`, mình không cần tự viết dòng `public delegate...` cho những trường hợp thông thường.

Để dùng `Action`:

```csharp
using System;
```

Không có tham số:

```csharp
private Action onGameOver;
```

Có một tham số:

```csharp
private Action<int> onScoreChanged;
```

Có hai tham số:

```csharp
private Action<int, string> onItemCollected;
```

`Action` luôn đại diện cho hàm trả về `void`.

### 3.2. Ví dụ Action không có tham số

```csharp
using System;
using UnityEngine;

public class ActionExample : MonoBehaviour
{
    private Action onGameOver;

    private void Start()
    {
        onGameOver += ShowGameOver;
        onGameOver += StopMusic;

        onGameOver?.Invoke();
    }

    private void ShowGameOver()
    {
        Debug.Log("Hiện màn hình Game Over");
    }

    private void StopMusic()
    {
        Debug.Log("Dừng nhạc");
    }
}
```

### 3.3. Ví dụ Action có tham số

```csharp
using System;
using UnityEngine;

public class ScoreExample : MonoBehaviour
{
    private Action<int> onScoreChanged;

    private void Start()
    {
        onScoreChanged += PrintScore;

        onScoreChanged?.Invoke(100);
    }

    private void PrintScore(int score)
    {
        Debug.Log("Điểm hiện tại: " + score);
    }
}
```

Giá trị `100` trong `Invoke(100)` được truyền vào tham số `score` của `PrintScore`.

### 3.4. Action khác Func thế nào?

Chỉ cần nhớ sơ qua:

```csharp
Action<int> printScore;      // Nhận int, trả về void
Func<int, bool> isPositive;  // Nhận int, trả về bool
```

Nếu chỉ muốn phát thông báo, `Action` thường phù hợp. Nếu cần nhận lại một giá trị từ hàm, có thể dùng `Func` hoặc custom delegate có return type.

---

## 4. Event trong C#

### 4.1. Vì sao thêm từ khóa `event`?

Một biến `Action` để public sẽ khá dễ bị code bên ngoài thay đổi hoặc tự ý gọi:

```csharp
public Action onPlayerDied;
```

Script khác có thể làm những việc không mong muốn:

```csharp
player.onPlayerDied = null;
player.onPlayerDied?.Invoke();
```

Để bảo vệ nó, thêm từ khóa `event`:

```csharp
public event Action OnPlayerDied;
```

Lúc này script khác chỉ được:

- Đăng ký bằng `+=`.
- Hủy đăng ký bằng `-=`.

Chỉ class khai báo event mới có quyền phát event bằng `Invoke`.

### 4.2. Ví dụ Publisher và Subscriber

`PlayerHealth` là nơi phát sự kiện:

```csharp
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public event Action<int> OnHealthChanged;
    public event Action OnPlayerDied;

    [SerializeField] private int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0);

        OnHealthChanged?.Invoke(health);

        if (health == 0)
        {
            OnPlayerDied?.Invoke();
        }
    }
}
```

`HealthLogger` là nơi lắng nghe:

```csharp
using UnityEngine;

public class HealthLogger : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += PrintHealth;
        playerHealth.OnPlayerDied += PrintGameOver;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= PrintHealth;
        playerHealth.OnPlayerDied -= PrintGameOver;
    }

    private void PrintHealth(int health)
    {
        Debug.Log("Máu còn lại: " + health);
    }

    private void PrintGameOver()
    {
        Debug.Log("Player đã chết");
    }
}
```

Ở đây:

- `PlayerHealth` là publisher: đối tượng phát thông báo.
- `HealthLogger` là subscriber/listener: đối tượng đăng ký nghe.
- Publisher không cần biết có bao nhiêu listener.

### 4.3. Vì sao đăng ký trong OnEnable và gỡ trong OnDisable?

Mẫu phổ biến:

```csharp
private void OnEnable()
{
    source.SomeEvent += HandleEvent;
}

private void OnDisable()
{
    source.SomeEvent -= HandleEvent;
}
```

Điều này giúp tránh:

- Đăng ký trùng sau nhiều lần bật/tắt object.
- Một object đã bị vô hiệu hóa nhưng vẫn nhận sự kiện.
- Event giữ tham chiếu tới listener lâu hơn mong muốn.

Điều kiện là `source` phải tồn tại lúc `OnEnable` và `OnDisable` chạy. Nếu vòng đời hai object đặc biệt, cần kiểm tra null hoặc chọn nơi đăng ký phù hợp hơn.

---

## 5. UnityEvent – phần trọng tâm

### 5.1. UnityEvent là gì?

`UnityEvent` là hệ thống event riêng của Unity. Điểm mạnh lớn nhất là nó có thể xuất hiện trong Inspector để mình kéo thả GameObject và chọn hàm cần gọi mà không phải viết toàn bộ kết nối bằng code.

Để dùng:

```csharp
using UnityEngine.Events;
```

Khai báo:

```csharp
[SerializeField] private UnityEvent onShoot;
```

Gọi event:

```csharp
onShoot.Invoke();
```

### 5.2. Ví dụ UnityEvent cơ bản

```csharp
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    [SerializeField] private UnityEvent onShoot;

    public void Shoot()
    {
        Debug.Log("Bắn đạn");

        onShoot.Invoke();
    }
}
```

Sau khi gắn script vào GameObject, trường `On Shoot` sẽ xuất hiện trong Inspector.

### 5.3. Đăng ký UnityEvent trong Inspector

Các bước:

1. Chọn GameObject đang chứa script có `UnityEvent`.
2. Tìm trường event trong Inspector, ví dụ `On Shoot`.
3. Nhấn dấu `+`.
4. Kéo GameObject nhận sự kiện vào ô trống.
5. Mở danh sách hàm.
6. Chọn component và hàm muốn gọi.

Có thể nhấn `+` nhiều lần để gọi nhiều hàm:

```text
On Shoot
├── AudioSource.Play()
├── CameraShake.Shake()
└── AmmoUI.Refresh()
```

Khi `onShoot.Invoke()` chạy, tất cả callback đã đăng ký đều được gọi.

Listener được gắn trong Inspector và được lưu cùng scene/prefab thường được gọi là **persistent listener**.

### 5.4. Đăng ký UnityEvent bằng code

Ngoài Inspector, có thể đăng ký lúc runtime bằng `AddListener`:

```csharp
private void OnEnable()
{
    gun.OnShoot.AddListener(PlayEffect);
}

private void OnDisable()
{
    gun.OnShoot.RemoveListener(PlayEffect);
}

private void PlayEffect()
{
    Debug.Log("Chạy hiệu ứng bắn");
}
```

Để script khác truy cập được event, có thể khai báo:

```csharp
public UnityEvent OnShoot;
```

Hoặc giữ field private và chỉ mở property đọc:

```csharp
[SerializeField] private UnityEvent onShoot;

public UnityEvent OnShoot => onShoot;
```

Listener thêm bằng `AddListener` là **runtime listener**. Nó không trở thành một dòng được lưu sẵn trong Inspector.

### 5.5. UnityEvent có tham số

Không có tham số:

```csharp
public UnityEvent OnGameOver;
```

Có một tham số:

```csharp
public UnityEvent<int> OnHealthChanged;
```

Gọi:

```csharp
OnHealthChanged.Invoke(80);
```

Listener phải có chữ ký phù hợp:

```csharp
private void ShowHealth(int health)
{
    Debug.Log("Máu: " + health);
}
```

Đăng ký:

```csharp
OnHealthChanged.AddListener(ShowHealth);
```

Nếu cần tương thích với những phiên bản hoặc cách serialize cũ, có thể tạo class riêng:

```csharp
using System;
using UnityEngine.Events;

[Serializable]
public class IntEvent : UnityEvent<int>
{
}
```

Sau đó dùng:

```csharp
[SerializeField] private IntEvent onHealthChanged;
```

UnityEvent hỗ trợ tối đa bốn tham số generic, nhưng event quá nhiều tham số thường khó đọc và khó cấu hình. Nếu cần truyền cả một cụm dữ liệu, có thể đóng gói chúng trong class hoặc struct riêng.

### 5.6. Static và Dynamic trong Inspector

Khi UnityEvent có tham số, Inspector có thể hiển thị hai nhóm lựa chọn.

#### Dynamic

Hàm nhận giá trị thật được truyền từ `Invoke`:

```csharp
OnHealthChanged.Invoke(currentHealth);
```

Nếu chọn hàm dynamic `ShowHealth(int value)`, nó nhận đúng `currentHealth` tại thời điểm event được gọi.

#### Static

Giá trị được nhập sẵn trong Inspector.

Ví dụ dù code gọi:

```csharp
OnHealthChanged.Invoke(80);
```

nhưng listener static được đặt sẵn tham số `10`, hàm đó sẽ nhận `10`.

Ghi nhớ:

```text
Dynamic = lấy dữ liệu từ Invoke
Static  = lấy dữ liệu nhập sẵn trong Inspector
```

### 5.7. UnityEvent phù hợp khi nào?

UnityEvent phù hợp khi:

- Muốn kết nối hành vi trong Inspector.
- Designer cần đổi callback mà không sửa code.
- Làm Button `onClick`.
- Prefab cần cấu hình hành vi riêng.
- Event không bị gọi với tần suất quá dày.
- Muốn kéo thả AudioSource, Animator hoặc component khác.

C# event/Action thường phù hợp hơn khi:

- Kết nối hoàn toàn bằng code.
- Muốn kiểm soát quyền phát event.
- Event được gọi thường xuyên.
- Muốn refactor an toàn hơn và giảm phụ thuộc vào cấu hình Inspector.

### 5.8. UnityEvent và Button

`Button.onClick` chính là một dạng UnityEvent.

Có thể đăng ký trong Inspector hoặc bằng code:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Button playButton;

    private void OnEnable()
    {
        playButton.onClick.AddListener(StartGame);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(StartGame);
    }

    private void StartGame()
    {
        Debug.Log("Bắt đầu game");
    }
}
```

---

## 6. Cách đăng ký sự kiện với từng kiểu

### Custom delegate

```csharp
public delegate void MessageHandler(string message);
public MessageHandler OnMessage;

private void OnEnable()
{
    OnMessage += PrintMessage;
}

private void OnDisable()
{
    OnMessage -= PrintMessage;
}
```

### Action

```csharp
public Action<int> OnScoreChanged;

private void OnEnable()
{
    OnScoreChanged += ShowScore;
}

private void OnDisable()
{
    OnScoreChanged -= ShowScore;
}
```

### C# event dùng Action

```csharp
public event Action<int> OnScoreChanged;

private void OnEnable()
{
    score.OnScoreChanged += ShowScore;
}

private void OnDisable()
{
    score.OnScoreChanged -= ShowScore;
}
```

### UnityEvent bằng code

```csharp
private void OnEnable()
{
    score.OnScoreChanged.AddListener(ShowScore);
}

private void OnDisable()
{
    score.OnScoreChanged.RemoveListener(ShowScore);
}
```

### UnityEvent trong Inspector

```text
Nhấn +
→ Kéo object vào ô
→ Chọn component
→ Chọn hàm
```

Không viết `+=` khi làm việc trực tiếp với `UnityEvent`. Hãy dùng `AddListener` và `RemoveListener`.

---

## 7. Cách gọi sự kiện bằng Invoke

### Delegate, Action và C# event

```csharp
OnScoreChanged?.Invoke(newScore);
```

Dấu `?` tránh lỗi nếu chưa có listener.

### UnityEvent

```csharp
OnScoreChanged.Invoke(newScore);
```

UnityEvent là một object. Hãy bảo đảm field đã được Unity serialize hoặc được khởi tạo:

```csharp
[SerializeField]
private UnityEvent onShoot = new UnityEvent();
```

Có thể kiểm tra null nếu event được gán theo cách khác:

```csharp
if (onShoot != null)
{
    onShoot.Invoke();
}
```

### Cẩn thận: có nhiều thứ cùng tên Invoke

Hai dòng sau không phải cùng một hệ thống:

```csharp
onShoot.Invoke();       // Phát UnityEvent
Invoke("Shoot", 1f);   // MonoBehaviour.Invoke: gọi hàm theo tên sau 1 giây
```

Trong bài này, khi nói “gọi event bằng Invoke”, ý chính là:

```csharp
unityEvent.Invoke();
```

---

## 8. Coroutine là gì?

Coroutine là một hàm có thể:

- Chạy một đoạn.
- Tạm dừng tại `yield return`.
- Nhường cho Unity tiếp tục chạy game.
- Quay lại chạy tiếp vào thời điểm phù hợp.

Coroutine thường trả về `IEnumerator`:

```csharp
private IEnumerator ExampleCoroutine()
{
    Debug.Log("Bắt đầu");

    yield return null;

    Debug.Log("Frame tiếp theo");
}
```

Cần namespace:

```csharp
using System.Collections;
```

### Coroutine không phải thread

Coroutine mặc định vẫn chạy trên main thread của Unity. Nó không tự đưa công việc nặng sang CPU thread khác.

Đoạn này vẫn có thể làm game đứng:

```csharp
private IEnumerator BadCoroutine()
{
    for (int i = 0; i < 1_000_000_000; i++)
    {
        // Công việc nặng, không yield.
    }

    yield return null;
}
```

Lý do là Unity phải chạy hết vòng lặp mới tới được `yield`.

Coroutine hữu ích khi công việc vốn có thể chia nhỏ theo thời gian, ví dụ:

- Chờ hồi chiêu.
- Đếm ngược.
- Nhấp nháy sprite qua nhiều frame.
- Di chuyển dần tới một vị trí.
- Chờ animation hoặc điều kiện.
- Thực hiện một kiểm tra mỗi 0,2 giây thay vì mỗi frame.

---

## 9. Luồng chạy của Coroutine

Xem coroutine sau:

```csharp
private IEnumerator TestFlow()
{
    Debug.Log("A");

    yield return null;

    Debug.Log("B");

    yield return new WaitForSeconds(2f);

    Debug.Log("C");
}
```

Khởi động:

```csharp
StartCoroutine(TestFlow());
```

Luồng chạy:

```text
StartCoroutine được gọi
        ↓
In A ngay trong lượt chạy hiện tại
        ↓
Gặp yield return null
        ↓
Tạm dừng coroutine, game vẫn tiếp tục
        ↓
Frame sau quay lại và in B
        ↓
Gặp WaitForSeconds(2f)
        ↓
Tạm dừng khoảng 2 giây
        ↓
Quay lại và in C
        ↓
Đi tới cuối hàm, coroutine kết thúc
```

Điểm quan trọng:

- `StartCoroutine` không có nghĩa là toàn bộ hàm sẽ đợi tới frame sau mới bắt đầu.
- Đoạn trước `yield` đầu tiên thường chạy ngay.
- Mỗi lần gặp `yield`, coroutine tạm dừng.
- Khi điều kiện chờ hoàn thành, nó tiếp tục từ dòng ngay sau `yield`.
- Khi đi tới cuối hàm, coroutine tự kết thúc.

---

## 10. `yield return null`

```csharp
yield return null;
```

Ý nghĩa:

> Dừng coroutine tại đây và tiếp tục vào frame sau.

Ví dụ đếm frame:

```csharp
private IEnumerator CountFrames()
{
    for (int i = 1; i <= 5; i++)
    {
        Debug.Log("Frame coroutine: " + i);

        yield return null;
    }
}
```

Mỗi vòng lặp chạy ở một frame khác nhau.

### Ví dụ di chuyển dần

```csharp
private IEnumerator MoveToPosition(Vector3 target, float speed)
{
    while (Vector3.Distance(transform.position, target) > 0.01f)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        yield return null;
    }

    transform.position = target;
}
```

Nếu bỏ `yield return null`, vòng `while` cố chạy hết trong một frame và có thể làm game treo.

---

## 11. `WaitForSeconds`

### 11.1. Chờ theo giây

```csharp
yield return new WaitForSeconds(2f);
```

Coroutine tạm dừng và được tiếp tục sau khoảng 2 giây game time.

Ví dụ hồi chiêu:

```csharp
private IEnumerator Cooldown()
{
    Debug.Log("Bắt đầu hồi chiêu");

    yield return new WaitForSeconds(1f);

    Debug.Log("Có thể dùng kỹ năng tiếp");
}
```

### 11.2. WaitForSeconds chịu ảnh hưởng bởi timeScale

`WaitForSeconds` dùng thời gian đã scale. Nếu:

```csharp
Time.timeScale = 0f;
```

thì game time dừng và `WaitForSeconds` cũng chưa tiếp tục.

Nếu cần chờ theo thời gian thật, không phụ thuộc `timeScale`:

```csharp
yield return new WaitForSecondsRealtime(2f);
```

Ví dụ UI đếm ngược vẫn chạy khi game đang pause có thể dùng `WaitForSecondsRealtime`.

### 11.3. Thời gian chờ không chính xác tuyệt đối tới từng mili giây

Coroutine chỉ có thể tiếp tục tại một thời điểm Unity xử lý frame phù hợp. Vì vậy, “2 giây” nên hiểu là tiếp tục ở frame thích hợp sau khi khoảng chờ đã đạt, không phải một bộ hẹn giờ real-time chính xác tuyệt đối.

### 11.4. Có thể tái sử dụng WaitForSeconds cố định

Nếu một vòng lặp luôn chờ cùng một khoảng thời gian:

```csharp
private readonly WaitForSeconds waitTime = new WaitForSeconds(0.2f);

private IEnumerator ScanLoop()
{
    while (true)
    {
        ScanEnemy();

        yield return waitTime;
    }
}
```

Cách này tránh tạo một object `WaitForSeconds` mới ở mọi vòng. Chỉ dùng cách cache này khi khoảng chờ là cố định.

---

## 12. `StartCoroutine`

### 12.1. Cách khuyên dùng

```csharp
StartCoroutine(Flash());
```

Coroutine:

```csharp
private IEnumerator Flash()
{
    Debug.Log("Bật sáng");

    yield return new WaitForSeconds(0.2f);

    Debug.Log("Tắt sáng");
}
```

### 12.2. Lưu Coroutine để dừng sau

```csharp
private Coroutine flashCoroutine;

public void StartFlash()
{
    flashCoroutine = StartCoroutine(Flash());
}
```

`StartCoroutine` trả về một `Coroutine`. Handle này hữu ích khi cần dừng đúng lần chạy đã bắt đầu.

### 12.3. Tránh chạy trùng nhiều coroutine

Nếu gọi `StartFlash()` liên tục, nhiều bản `Flash()` có thể chạy song song.

Có thể dừng bản cũ trước khi bắt đầu bản mới:

```csharp
private Coroutine flashCoroutine;

public void StartFlash()
{
    if (flashCoroutine != null)
    {
        StopCoroutine(flashCoroutine);
    }

    flashCoroutine = StartCoroutine(Flash());
}

private IEnumerator Flash()
{
    Debug.Log("Bật sáng");

    yield return new WaitForSeconds(0.2f);

    Debug.Log("Tắt sáng");

    flashCoroutine = null;
}
```

### 12.4. Chờ một coroutine khác chạy xong

```csharp
private IEnumerator FullSequence()
{
    Debug.Log("Bắt đầu chuỗi");

    yield return StartCoroutine(FirstStep());

    Debug.Log("FirstStep đã xong");

    yield return StartCoroutine(SecondStep());

    Debug.Log("Toàn bộ chuỗi đã xong");
}
```

Nếu chỉ viết:

```csharp
StartCoroutine(FirstStep());
StartCoroutine(SecondStep());
```

thì hai coroutine có thể chạy song song. `yield return StartCoroutine(...)` có nghĩa là chờ coroutine con hoàn thành rồi mới chạy dòng tiếp theo.

### 12.5. Có thể bắt đầu bằng tên hàm, nhưng thường không nên

Unity hỗ trợ:

```csharp
StartCoroutine("Flash");
```

Nhược điểm:

- Dùng chuỗi nên dễ gõ sai.
- Đổi tên hàm mà quên đổi chuỗi.
- Khó truyền nhiều tham số.
- Ít an toàn khi refactor.

Thường nên dùng:

```csharp
StartCoroutine(Flash());
```

---

## 13. Dừng Coroutine

### 13.1. Dừng bằng Coroutine handle

Đây là cách rõ ràng và thường được khuyên dùng:

```csharp
private Coroutine reloadCoroutine;

public void StartReload()
{
    reloadCoroutine = StartCoroutine(Reload());
}

public void CancelReload()
{
    if (reloadCoroutine == null)
        return;

    StopCoroutine(reloadCoroutine);
    reloadCoroutine = null;
}

private IEnumerator Reload()
{
    Debug.Log("Đang nạp đạn");

    yield return new WaitForSeconds(2f);

    Debug.Log("Nạp xong");
    reloadCoroutine = null;
}
```

### 13.2. Dừng bằng IEnumerator

Phải lưu đúng instance đã dùng để bắt đầu:

```csharp
private IEnumerator reloadRoutine;

public void StartReload()
{
    reloadRoutine = Reload();
    StartCoroutine(reloadRoutine);
}

public void CancelReload()
{
    if (reloadRoutine != null)
    {
        StopCoroutine(reloadRoutine);
        reloadRoutine = null;
    }
}
```

Sai:

```csharp
StartCoroutine(Reload());
StopCoroutine(Reload());
```

Hai lần `Reload()` tạo ra hai `IEnumerator` khác nhau. Dòng Stop không trỏ vào instance đang chạy.

### 13.3. Dừng bằng tên

```csharp
StartCoroutine("Reload");
StopCoroutine("Reload");
```

Cách này dùng được nhưng có các nhược điểm của chuỗi. Nếu bắt đầu bằng tên thì dừng bằng tên; không trộn các kiểu overload với nhau.

### 13.4. `StopAllCoroutines`

```csharp
StopAllCoroutines();
```

Hàm này dừng tất cả coroutine được chạy bởi chính `MonoBehaviour` đó. Nó không dừng coroutine trên mọi script trong game.

Hãy dùng cẩn thận vì có thể dừng cả những coroutine khác mà script vẫn cần.

### 13.5. `yield break`

Coroutine có thể tự kết thúc sớm:

```csharp
private IEnumerator HealOverTime()
{
    if (health >= maxHealth)
    {
        yield break;
    }

    yield return new WaitForSeconds(1f);
    health++;
}
```

`yield break` giống như `return` dành cho iterator/coroutine: kết thúc coroutine ngay tại đó.

### 13.6. Disable object có làm coroutine dừng không?

Cần phân biệt hai trường hợp:

- `gameObject.SetActive(false)` làm GameObject mất active: coroutine gắn với object đó bị dừng.
- `myBehaviour.enabled = false` chỉ tắt component MonoBehaviour: coroutine đã chạy không tự động dừng chỉ vì component bị disable.

Nếu muốn chắc chắn dừng khi component bị tắt:

```csharp
private void OnDisable()
{
    StopAllCoroutines();
}
```

Tuy nhiên, chỉ thêm dòng này khi đúng với thiết kế của script, vì nó dừng toàn bộ coroutine đang chạy trên component đó.

---

## 14. Các kiểu yield hữu ích khác

Ngoài hai loại chính của bài, Unity còn có một số kiểu chờ phổ biến.

### Chờ tới cuối frame

```csharp
yield return new WaitForEndOfFrame();
```

Thường dùng khi cần đợi các bước render trong frame gần hoàn tất, ví dụ một số trường hợp chụp màn hình.

### Chờ physics step tiếp theo

```csharp
yield return new WaitForFixedUpdate();
```

Coroutine tiếp tục ở nhịp FixedUpdate tiếp theo.

### Chờ tới khi điều kiện đúng

```csharp
yield return new WaitUntil(() => health <= 0);
```

### Chờ trong khi điều kiện còn đúng

```csharp
yield return new WaitWhile(() => isLoading);
```

### Chờ thời gian thật

```csharp
yield return new WaitForSecondsRealtime(2f);
```

Bảng nhớ nhanh:

| Yield | Khi tiếp tục |
|---|---|
| `yield return null` | Frame sau |
| `WaitForSeconds` | Sau một khoảng game time |
| `WaitForSecondsRealtime` | Sau một khoảng thời gian thật |
| `WaitForFixedUpdate` | Physics step tiếp theo |
| `WaitForEndOfFrame` | Gần cuối frame hiện tại |
| `WaitUntil` | Khi điều kiện trở thành true |
| `WaitWhile` | Khi điều kiện trở thành false |

---

## 15. Kết hợp UnityEvent và Coroutine

Ví dụ này mô phỏng một khẩu súng:

- Bắn một lần.
- Phát `UnityEvent` để UI, âm thanh hoặc hiệu ứng nghe.
- Khóa bắn.
- Chạy coroutine chờ hồi chiêu.
- Cho phép bắn lại.

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SimpleGun : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private UnityEvent onShot;
    [SerializeField] private UnityEvent onReady;

    private bool canShoot = true;
    private Coroutine cooldownCoroutine;

    public UnityEvent OnShot => onShot;

    public void Shoot()
    {
        if (!canShoot)
        {
            Debug.Log("Súng đang hồi");
            return;
        }

        canShoot = false;

        Debug.Log("Bắn");
        onShot.Invoke();

        cooldownCoroutine = StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);

        canShoot = true;
        cooldownCoroutine = null;

        Debug.Log("Có thể bắn tiếp");
        onReady.Invoke();
    }

    public void CancelCooldown()
    {
        if (cooldownCoroutine == null)
            return;

        StopCoroutine(cooldownCoroutine);
        cooldownCoroutine = null;
        canShoot = true;
    }
}
```

Một listener đăng ký bằng code:

```csharp
using UnityEngine;

public class ShotLogger : MonoBehaviour
{
    [SerializeField] private SimpleGun gun;

    private void OnEnable()
    {
        gun.OnShot.AddListener(PrintShot);
    }

    private void OnDisable()
    {
        gun.OnShot.RemoveListener(PrintShot);
    }

    private void PrintShot()
    {
        Debug.Log("Listener nhận được sự kiện bắn");
    }
}
```

Ngoài listener bằng code, có thể vào Inspector của `SimpleGun` và thêm callback vào `On Shot` hoặc `On Ready`.

Luồng chạy:

```text
Shoot()
   ↓
Kiểm tra canShoot
   ↓
canShoot = false
   ↓
onShot.Invoke()
   ↓
Các listener chạy
   ↓
StartCoroutine(Cooldown())
   ↓
WaitForSeconds(cooldownTime)
   ↓
canShoot = true
   ↓
onReady.Invoke()
```

---

## 16. Lỗi thường gặp

### 16.1. Quên đăng ký listener

Event được Invoke nhưng không có gì xảy ra vì chưa có listener.

Với UnityEvent, kiểm tra cả:

- Đã nhấn `+` trong Inspector chưa?
- Đã kéo đúng object chưa?
- Đã chọn đúng hàm chưa?
- Dòng callback có đang để `Off` không?

### 16.2. Đăng ký nhiều lần nhưng không gỡ

```csharp
private void OnEnable()
{
    source.OnChanged += HandleChanged;
}
```

Nếu không `-=`, listener có thể bị gọi lặp nhiều lần sau các vòng bật/tắt hoặc sống lâu ngoài mong muốn.

### 16.3. Gỡ lambda không đúng cách

Đoạn này không gỡ được lambda cũ:

```csharp
onScoreChanged += value => Debug.Log(value);
onScoreChanged -= value => Debug.Log(value);
```

Hai biểu thức tạo hai delegate instance khác nhau.

Cách đơn giản là dùng hàm có tên:

```csharp
onScoreChanged += PrintScore;
onScoreChanged -= PrintScore;

private void PrintScore(int value)
{
    Debug.Log(value);
}
```

Với UnityEvent cũng nên dùng cùng một method reference cho `AddListener` và `RemoveListener`.

### 16.4. Tự gọi C# event từ class khác

Nếu khai báo:

```csharp
public event Action OnDied;
```

script ngoài không được làm:

```csharp
player.OnDied?.Invoke();
```

Đây là chủ ý của `event`: chỉ class sở hữu event được quyền phát thông báo.

### 16.5. Coroutine không chạy vì chỉ gọi hàm thường

Sai:

```csharp
Cooldown();
```

Gọi một hàm trả về `IEnumerator` như vậy chỉ tạo iterator, không đăng ký nó với Unity để chạy qua các frame.

Đúng:

```csharp
StartCoroutine(Cooldown());
```

### 16.6. Vòng lặp coroutine thiếu yield

Sai:

```csharp
while (true)
{
    Debug.Log("Loop");
}
```

Game có thể treo vì vòng lặp không nhường lại quyền chạy cho Unity.

Đúng:

```csharp
while (true)
{
    Debug.Log("Loop");
    yield return null;
}
```

### 16.7. Start nhiều lần ngoài ý muốn

Mỗi lần gọi `StartCoroutine` tạo một lần chạy riêng. Nếu nhấn nút năm lần, có thể có năm coroutine cùng hoạt động.

Có thể dùng biến `Coroutine`, biến `bool` hoặc dừng bản cũ trước khi start bản mới.

### 16.8. Dùng WaitForSeconds khi game pause

Nếu `Time.timeScale = 0`, `WaitForSeconds` chưa tiếp tục. Dùng:

```csharp
yield return new WaitForSecondsRealtime(seconds);
```

nếu hành vi phải chạy trong lúc pause.

### 16.9. Nghĩ coroutine chạy trên thread khác

Coroutine không làm cho thuật toán nặng tự nhiên trở nên nhẹ. Nếu mỗi lần chạy giữa hai `yield` vẫn quá nặng, frame vẫn bị giật.

---

## 17. Bảng ghi nhớ nhanh

### Event

| Kiểu | Đăng ký | Hủy đăng ký | Phát sự kiện |
|---|---|---|---|
| Delegate | `+= Method` | `-= Method` | `?.Invoke(...)` |
| Action | `+= Method` | `-= Method` | `?.Invoke(...)` |
| C# event | `+= Method` | `-= Method` | `?.Invoke(...)` trong class sở hữu |
| UnityEvent | `AddListener(Method)` | `RemoveListener(Method)` | `.Invoke(...)` |
| UnityEvent Inspector | Nhấn `+`, kéo object, chọn hàm | Xóa dòng listener | `.Invoke(...)` từ code |

### Coroutine

| Việc cần làm | Cú pháp |
|---|---|
| Khai báo | `IEnumerator MyCoroutine()` |
| Bắt đầu | `StartCoroutine(MyCoroutine())` |
| Chờ một frame | `yield return null` |
| Chờ game time | `yield return new WaitForSeconds(1f)` |
| Chờ real time | `yield return new WaitForSecondsRealtime(1f)` |
| Chờ điều kiện | `yield return new WaitUntil(() => condition)` |
| Tự kết thúc | `yield break` |
| Dừng một coroutine | `StopCoroutine(coroutineHandle)` |
| Dừng toàn bộ trên script | `StopAllCoroutines()` |

### Câu chốt để nhớ

```text
Delegate = kiểu có thể giữ hàm
Action = delegate có sẵn, trả về void
C# event = delegate/Action được bảo vệ quyền Invoke
UnityEvent = event có thể cấu hình trong Inspector
Invoke = phát event, gọi toàn bộ listener
Coroutine = hàm có thể tạm dừng và tiếp tục qua nhiều frame
yield = điểm coroutine nhường luồng và chờ
```
