# Ôn lại bài buổi 3

## MonoBehaviour và vòng đời của script

* Namespace: UnityEngine  
* Kế thừa từ: Behaviour -> Component -> Object  
* Đặc điểm: Không thể khởi tạo trực tiếp bằng từ khóa new (ví dụ: new MyScript() sẽ báo lỗi). Thay vào đó,ta phải gắn nó vào một GameObject đang tồn tại hoặc dùng hàm gameObject.AddComponent<T>().

| Tên Phương thức | Thời điểm thực thi | Mục đích sử dụng chính |
| ----- | ----- | ----- |
| Awake() | Ngay khi đối tượng được nạp vào bộ nhớ (trước Start, chạy ngay cả khi GameObject bị tắt). | Khởi tạo biến cục bộ, lấy tham chiếu thành phần (GetComponent). |
| OnEnable() | Ngay khi GameObject hoặc Component được bật (SetActive(true)). | Đăng ký sự kiện (Event Listener), đặt lại trạng thái ban đầu. |
| Start() | Trước frame đầu tiên render, nhưng chỉ chạy nếu Script đang được bật. | Khởi tạo dữ liệu phụ thuộc vào các đối tượng khác trong Scene. |
| FixedUpdate() | Theo chu kỳ thời gian cố định (mặc định 0.02s / lần). | Xử lý các tính toán vật lý (Rigidbody, lực đẩy). |
| Update() | Được gọi mỗi khung hình (frame) một lần. | Xử lý input từ người chơi, logic di chuyển, kiểm tra sự kiện theo thời gian thực. |
| LateUpdate() | Chạy sau khi tất cả hàm Update() trong frame đó đã chạy xong. | Xử lý camera đuổi theo nhân vật, hoạt ảnh bám sát mục tiêu. |
| OnDisable() | Khi GameObject hoặc Component bị tắt đi (SetActive(false)). | Hủy đăng ký sự kiện, dừng các Coroutine ngầm để tránh rò rỉ bộ nhớ. |
| OnDestroy() | Khi đối tượng bị xóa vĩnh viễn khỏi Scene. | Dọn dẹp tài nguyên cuối cùng trước khi hủy. |

### Code ví dụ

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour  
{  
    // Biến cấu hình hiển thị trên Unity Inspector  
    [Header("Settings")]  
    public float moveSpeed = 5f;

    // 1. Khởi tạo dữ liệu nội bộ  
    private void Awake()  
    {  
        Debug.Log("Awake: Script đã sẵn sàng trong bộ nhớ.");  
    }

    // 2. Đăng ký sự kiện khi bật  
    private void OnEnable()  
    {  
        Debug.Log("OnEnable: Object đã được kích hoạt.");  
    }

    // 3. Chuẩn bị chạy logic đầu game  
    private void Start()  
    {  
        Debug.Log("Start: Bắt đầu trận chiến!");  
    }

    // 4. Xử lý vật lý định kỳ  
    private void FixedUpdate()  
    {  
        // Tính toán liên quan đến Rigidbody ở đây  
    }

    // 5. Xử lý logic và input mỗi frame  
    private void Update()  
    {  
        // Lấy input từ bàn phím (W, A, S, D hoặc phím mũi tên)  
        float moveX = Input.GetAxis("Horizontal");  
        float moveZ = Input.GetAxis("Vertical");

        // Di chuyển nhân vật dựa trên Transform  
        Vector3 moveDir = new Vector3(moveX, 0f, moveZ);  
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);  
    }

    // 6. Xử lý camera hoặc hiệu ứng bám theo sau khi Update đã xong  
    private void LateUpdate()  
    {  
        // Thường dùng cho Camera theo dõi Player  
    }

    // 7. Dọn dẹp khi bị tắt  
    private void OnDisable()  
    {  
        Debug.Log("OnDisable: Object đã tạm dừng hoạt động.");  
    }

    // 8. Hủy hoàn toàn  
    private void OnDestroy()  
    {  
        Debug.Log("OnDestroy: Object đã bị xóa khỏi game.");  
    }

}
```

---

# Ghi chú Unity: Gizmos, Assets, Transform và Time

> Đây là ghi chú bài tập về nhà, mục tiêu là đọc dễ hiểu và có ví dụ để thực hành.

## 1. Gizmos

### Gizmos là gì?

Gizmos là những hình vẽ hỗ trợ xuất hiện trong cửa sổ **Scene** của Unity. Gizmos giúp chúng ta nhìn thấy những thứ bình thường không có hình ảnh, ví dụ:

- Phạm vi phát hiện kẻ địch.  
- Vùng kiểm tra mặt đất.  
- Đường đi của nhân vật.  
- Hướng bắn của súng.  
- Các ô của một grid.

Gizmos chủ yếu dùng để hỗ trợ lúc làm game. Những hình này không phải hình ảnh thật trong game.

Muốn nhìn thấy Gizmos, cần bật nút **Gizmos** ở góc trên bên phải của cửa sổ Scene.

### OnDrawGizmos

Unity tự gọi hàm `OnDrawGizmos()` để vẽ Gizmos.

```csharp  
using UnityEngine;

public class DrawTest : MonoBehaviour  
{  
    private void OnDrawGizmos()  
    {  
        Gizmos.color = Color.red;  
        Gizmos.DrawWireCube(transform.position, Vector3.one);  
    }  
}  
```

Kết quả là một hình vuông rỗng màu đỏ được vẽ tại vị trí của GameObject.

### OnDrawGizmosSelected

`OnDrawGizmosSelected()` chỉ vẽ khi GameObject đang được chọn trong cửa sổ Scene hoặc Hierarchy.

```csharp  
private void OnDrawGizmosSelected()  
{  
    Gizmos.color = Color.yellow;  
    Gizmos.DrawWireSphere(transform.position, 3f);  
}  
```

Cách này phù hợp khi Scene có nhiều Gizmos và chúng ta không muốn lúc nào cũng nhìn thấy tất cả.

### Một số lệnh Gizmos thường dùng

| Lệnh | Công dụng |  
|---|---|  
| `Gizmos.DrawLine(a, b)` | Vẽ đường thẳng từ điểm `a` đến điểm `b`. |  
| `Gizmos.DrawRay(a, b)` | Vẽ tia bắt đầu từ `a`, hướng theo `b`. |  
| `Gizmos.DrawWireCube(a, b)` | Vẽ hình hộp rỗng. |  
| `Gizmos.DrawCube(a, b)` | Vẽ hình hộp đặc. |  
| `Gizmos.DrawWireSphere(a, b)` | Vẽ hình tròn hoặc hình cầu rỗng. |  
| `Gizmos.DrawSphere(a, b)` | Vẽ hình tròn hoặc hình cầu đặc. |  
| `Gizmos.color` | Thay đổi màu của hình được vẽ. |

Trong đó:

- `a` thường là vị trí.  
- `b` có thể là kích thước, bán kính hoặc hướng tùy theo lệnh.

### Ví dụ vẽ vùng phát hiện

```csharp  
using UnityEngine;

public class EnemyRange : MonoBehaviour  
{  
    [SerializeField] float range = 5f;

    private void OnDrawGizmos()  
    {  
        Gizmos.color = Color.red;  
        Gizmos.DrawWireSphere(transform.position, range);  
    }  
}  
```

Khi thay đổi `Range` trong Inspector, hình tròn cũng thay đổi theo.

### Ví dụ vẽ grid có spacing

```csharp  
using UnityEngine;

public class Grid : MonoBehaviour  
{  
    [SerializeField] Vector2Int grid = new Vector2Int(5, 4);  
    [SerializeField] float size = 1f;  
    [SerializeField] float spacing = 0.2f;

    private void OnDrawGizmos()  
    {  
        Gizmos.color = Color.red;

        float a = grid.x * size + (grid.x - 1) * spacing;  
        float b = grid.y * size + (grid.y - 1) * spacing;  
        float c = size + spacing;

        for (int i = 0; i < grid.x; i++)  
        {  
            for (int j = 0; j < grid.y; j++)  
            {  
                float x = -a / 2 + size / 2 + i * c;  
                float y = -b / 2 + size / 2 + j * c;

                Gizmos.DrawWireCube(  
                    new Vector3(x, y, 0),  
                    new Vector3(size, size, 0)  
                );  
            }  
        }  
    }  
}  
```

Ý nghĩa các biến:

- `grid.x`: số cột.  
- `grid.y`: số hàng.  
- `size`: kích thước mỗi ô.  
- `spacing`: khoảng cách giữa các ô.  
- `a`: tổng chiều rộng của grid.  
- `b`: tổng chiều cao của grid.  
- `c`: khoảng cách từ tâm ô này đến tâm ô tiếp theo.

## 2. Add Assets

### Asset là gì?

Asset là những tài nguyên được sử dụng trong project Unity, ví dụ:

- Hình ảnh và sprite.  
- Âm thanh.  
- Video.  
- Model 3D.  
- Animation.  
- Material.  
- Font chữ.  
- Script C#.  
- Prefab.

Các tài nguyên của project thường được đặt trong thư mục `Assets`.

### Cách thêm asset từ máy tính

Cách đơn giản nhất:

1. Mở cửa sổ **Project** trong Unity.  
2. Mở thư mục `Assets` hoặc thư mục con muốn sử dụng.  
3. Kéo file từ máy tính vào cửa sổ Project.  
4. Unity sẽ tự động import file.

Cũng có thể chọn:

```text  
Assets > Import New Asset  
```

Sau đó chọn file cần đưa vào project.

### Thêm file Unity Package

Với file có đuôi `.unitypackage`:

```text  
Assets > Import Package > Custom Package  
```

Chọn file, chọn các tài nguyên muốn lấy rồi bấm **Import**.

### Thêm asset từ Asset Store

Asset lấy từ Unity Asset Store thường được tải và import bằng cửa sổ **Package Manager**:

```text  
Window > Package Manager  
```

Sau khi tải về, bấm **Import** để thêm asset vào project.

### Import Settings

Khi chọn một asset trong cửa sổ Project, Inspector sẽ hiện các thiết lập import.

Ví dụ với hình ảnh:

- `Texture Type`: chọn kiểu ảnh, ví dụ `Sprite (2D and UI)`.  
- `Pixels Per Unit`: số pixel tương ứng với một đơn vị trong Unity.  
- `Filter Mode`: cách làm mịn hình ảnh.  
- `Compression`: mức nén ảnh.

Sau khi thay đổi thiết lập, bấm **Apply**.

### Lưu ý về file `.meta`

Unity tạo một file `.meta` cho mỗi asset. File này lưu thông tin nhận dạng và thiết lập của asset.

Không nên tự ý xóa hoặc làm mất file `.meta`, đặc biệt khi làm project theo nhóm. Nếu file `.meta` bị thay đổi, các liên kết đến sprite, material hoặc prefab có thể bị mất.

## 3. Transform

### Transform là gì?

Mọi GameObject trong Unity đều có component `Transform`. Không thể xóa Transform khỏi GameObject.

Transform quản lý ba thông tin chính:

- `Position`: vị trí.  
- `Rotation`: góc xoay.  
- `Scale`: kích thước hoặc tỉ lệ.

### Position

`transform.position` là vị trí của GameObject trong thế giới.

```csharp  
transform.position = new Vector3(3, 2, 0);  
```

Đoạn code trên đặt GameObject tại:

```text  
X = 3  
Y = 2  
Z = 0  
```

Có thể cộng thêm vào vị trí hiện tại:

```csharp  
transform.position += new Vector3(1, 0, 0);  
```

GameObject sẽ dịch sang phải một đơn vị.

### Rotation

Rotation là góc xoay của GameObject.

```csharp  
transform.rotation = Quaternion.Euler(0, 0, 45);  
```

Trong game 2D, chúng ta thường xoay quanh trục Z.

Cũng có thể dùng `Rotate`:

```csharp  
transform.Rotate(0, 0, 10);  
```

Mỗi lần dòng code chạy, GameObject xoay thêm 10 độ.

### Scale

Scale là tỉ lệ kích thước của GameObject.

```csharp  
transform.localScale = new Vector3(2, 2, 1);  
```

GameObject sẽ lớn gấp đôi theo trục X và Y.

```text  
Scale = (1, 1, 1): kích thước bình thường  
Scale = (2, 2, 1): lớn gấp đôi  
Scale = (0.5, 0.5, 1): nhỏ còn một nửa  
```

### Translate

`Translate` dùng để di chuyển GameObject.

```csharp  
transform.Translate(Vector3.right);  
```

Để di chuyển theo tốc độ ổn định, cần nhân với `Time.deltaTime`:

```csharp  
transform.Translate(Vector3.right * speed * Time.deltaTime);  
```

### World Space và Local Space

- `transform.position`: vị trí theo thế giới.  
- `transform.localPosition`: vị trí so với GameObject cha.  
- `Space.World`: di chuyển hoặc xoay theo trục của thế giới.  
- `Space.Self`: di chuyển hoặc xoay theo hướng riêng của GameObject.

Ví dụ:

```csharp  
transform.Translate(Vector3.right, Space.World);  
```

GameObject đi theo hướng phải của thế giới.

```csharp  
transform.Translate(Vector3.right, Space.Self);  
```

GameObject đi theo hướng phải của chính nó. Nếu GameObject đã xoay, hướng di chuyển cũng thay đổi.

### Parent và Child

Có thể đặt một Transform làm con của Transform khác:

```csharp  
transform.SetParent(parentTransform);  
```

Khi GameObject cha di chuyển, GameObject con cũng di chuyển theo.

### Ví dụ di chuyển và xoay

```csharp  
using UnityEngine;

public class MoveObject : MonoBehaviour  
{  
    [SerializeField] float moveSpeed = 5f;  
    [SerializeField] float rotateSpeed = 100f;

    private void Update()  
    {  
        transform.Translate(  
            Vector3.right * moveSpeed * Time.deltaTime  
        );

        transform.Rotate(  
            Vector3.forward * rotateSpeed * Time.deltaTime  
        );  
    }  
}  
```

> Nếu GameObject dùng `Rigidbody` hoặc `Rigidbody2D`, không nên liên tục thay đổi Transform để di chuyển vật lý. Khi đó nên dùng velocity, force hoặc MovePosition.

## 4. Time trong Unity

### Time dùng để làm gì?

`Time` cung cấp các thông tin liên quan đến thời gian trong game.

Ví dụ:

- Thời gian giữa hai frame.  
- Game đã chạy được bao lâu.  
- Thay đổi tốc độ của game.  
- Làm bộ đếm thời gian.  
- Tạm dừng game.

### Time.deltaTime

`Time.deltaTime` là thời gian từ frame trước đến frame hiện tại, tính bằng giây.

Không dùng `deltaTime`:

```csharp  
transform.Translate(Vector3.right * speed);  
```

Game chạy nhiều FPS sẽ di chuyển nhanh hơn game chạy ít FPS.

Có dùng `deltaTime`:

```csharp  
transform.Translate(Vector3.right * speed * Time.deltaTime);  
```

GameObject sẽ di chuyển gần như cùng tốc độ trên các máy có FPS khác nhau.

Ví dụ nếu:

```text  
speed = 5  
Time.deltaTime = 0.02  
```

Khoảng di chuyển trong frame đó là:

```text  
5 × 0.02 = 0.1 đơn vị  
```

### Time.time

`Time.time` là số giây đã trôi qua kể từ lúc game bắt đầu.

```csharp  
Debug.Log(Time.time);  
```

Có thể dùng để kiểm tra cooldown:

```csharp  
float nextAttack;

private void Update()  
{  
    if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttack)  
    {  
        Debug.Log("Attack");  
        nextAttack = Time.time + 2f;  
    }  
}  
```

Nhân vật chỉ có thể tấn công sau mỗi 2 giây.

### Time.fixedDeltaTime

`Time.fixedDeltaTime` là khoảng thời gian giữa các lần Unity chạy `FixedUpdate`.

```csharp  
private void FixedUpdate()  
{  
    Debug.Log(Time.fixedDeltaTime);  
}  
```

Giá trị thường gặp là `0.02`, tương đương khoảng 50 lần `FixedUpdate` mỗi giây.

`FixedUpdate` thường dùng cho vật lý như Rigidbody, lực và vận tốc.

### Time.timeScale

`Time.timeScale` điều chỉnh tốc độ thời gian trong game.

```csharp  
Time.timeScale = 1;  
```

Game chạy bình thường.

```csharp  
Time.timeScale = 0.5f;  
```

Game chạy chậm còn một nửa.

```csharp  
Time.timeScale = 0;  
```

Phần lớn hoạt động phụ thuộc thời gian sẽ dừng lại. Cách này thường được dùng để pause game.

```csharp  
public void PauseGame()  
{  
    Time.timeScale = 0;  
}

public void ContinueGame()  
{  
    Time.timeScale = 1;  
}  
```

### Time.unscaledDeltaTime

`Time.unscaledDeltaTime` giống `Time.deltaTime`, nhưng không bị ảnh hưởng bởi `Time.timeScale`.

Nó hữu ích cho:

- Menu pause vẫn có animation.  
- Đồng hồ vẫn chạy khi game đang dừng.  
- Hiệu ứng UI không phụ thuộc tốc độ game.

```csharp  
uiAnimationTime += Time.unscaledDeltaTime;  
```

### Time.realtimeSinceStartup

Đây là thời gian thực đã trôi qua kể từ lúc game bắt đầu và không bị ảnh hưởng bởi `timeScale`.

```csharp  
Debug.Log(Time.realtimeSinceStartup);  
```

### Time.timeSinceLevelLoad

Cho biết Scene hiện tại đã được chạy bao nhiêu giây.

```csharp  
Debug.Log(Time.timeSinceLevelLoad);  
```

### Time.frameCount

Cho biết tổng số frame Unity đã chạy.

```csharp  
Debug.Log(Time.frameCount);  
```

### Time.smoothDeltaTime

`Time.smoothDeltaTime` là giá trị `deltaTime` đã được làm mượt. Nó có thể hữu ích khi muốn chuyển động hoặc phép tính ít bị ảnh hưởng bởi những frame đột nhiên chậm.

### Bảng tóm tắt Time

| Thuộc tính | Ý nghĩa |  
|---|---|  
| `Time.deltaTime` | Thời gian giữa hai frame. |  
| `Time.time` | Thời gian game đã chạy. |  
| `Time.fixedDeltaTime` | Khoảng thời gian giữa các lần FixedUpdate. |  
| `Time.timeScale` | Tốc độ thời gian trong game. |  
| `Time.unscaledDeltaTime` | Delta time không bị ảnh hưởng bởi timeScale. |  
| `Time.realtimeSinceStartup` | Thời gian thực từ lúc game bắt đầu. |  
| `Time.timeSinceLevelLoad` | Thời gian từ lúc Scene được load. |  
| `Time.frameCount` | Tổng số frame đã chạy. |  
| `Time.smoothDeltaTime` | Delta time đã được làm mượt. |

## 5. Tóm tắt

- Gizmos dùng để vẽ hình hỗ trợ trong cửa sổ Scene.  
- `OnDrawGizmos` luôn vẽ, còn `OnDrawGizmosSelected` chỉ vẽ khi chọn GameObject.  
- Assets là hình ảnh, âm thanh, model, script và các tài nguyên của project.  
- Có thể kéo file trực tiếp vào thư mục Assets hoặc dùng các lệnh Import.  
- Transform quản lý Position, Rotation và Scale của GameObject.  
- `transform.position` là vị trí theo thế giới; `localPosition` là vị trí so với object cha.  
- `Time.deltaTime` giúp chuyển động không phụ thuộc FPS.  
- `Time.timeScale` dùng để chỉnh tốc độ hoặc tạm dừng game.
