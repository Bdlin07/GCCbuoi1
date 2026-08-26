Class là một kiểu dữ liệu do tự người lập trình định nghĩa ra, bên trong nó gói gọn cả dữ liệu (các biến mô tả đặc điểm) và hành vi (các hàm mô tả hành động).

Cấu trúc cơ bản của một Class trong C# :

Một class chuẩn chỉnh sẽ bao gồm hai thành phần chính:

Fields / Properties (Thuộc tính/Biến): Đại diện cho các đặc điểm, trạng thái (Ví dụ: tên, máu, sát thương, tốc độ...).

Methods (Phương thức/Hàm): Đại diện cho các hành động, kỹ năng mà nó có thể làm (Ví dụ: di chuyển, tấn công, chịu sát thương...).

Cấu trúc tổng quát:

```csharp
C# [Phạm vi truy cập] class TenClass { // 1. Biến (Fields) lưu trữ thông tin public kieu_du_lieu ten_bien;
// 2. Hàm khởi tạo (Constructor) - Dùng để bơm thông tin lúc mới sinh ra
public TenClass(tham_so)
{
    // Gán giá trị ban đầu
}

// 3. Phương thức (Methods) hành động
public kieu_tra_ve TenHam(tham_so)
{
    // Khối lệnh thực thi
}

}
```

**MonoBehaviour** là lớp cơ sở (base class) cốt lõi trong Unity mà mọi script C# đều phải kế thừa nếu muốn gắn (attach) trực tiếp vào một GameObject nằm trong Scene.

Nó đóng vai trò như một "cầu nối" (bridge) giữa mã nguồn C# và bộ máy vận hành của engine Unity. Nhờ có MonoBehaviour, Unity mới biết cách:

- Tự động gọi các hàm theo vòng đời (Lifecycle) như Awake(), Start(), Update().

- Cho phép kéo thả script trực tiếp vào giao diện Editor.

- Cung cấp khả năng truy cập vào các thành phần khác (GetComponent) và quản lý trạng thái hiển thị (SetActive).

## **Tài liệu Kỹ thuật (Documentation)**

### **Thông tin chung**

- **Namespace:** UnityEngine

- **Kế thừa từ:** Behaviour -\> Component -\> Object

- **Đặc điểm:** Không thể khởi tạo trực tiếp bằng từ khóa new (ví dụ: new MyScript() sẽ báo lỗi). Thay vào đó,ta phải gắn nó vào một GameObject đang tồn tại hoặc dùng hàm gameObject.AddComponent\<T\>().

| **Tên Phương thức** | **Thời điểm thực thi** | **Mục đích sử dụng chính** |
| --- | --- | --- |
| Awake() | Ngay khi đối tượng được nạp vào bộ nhớ (trước Start, chạy ngay cả khi GameObject bị tắt). | Khởi tạo biến cục bộ, lấy tham chiếu thành phần (GetComponent). |
| OnEnable() | Ngay khi GameObject hoặc Component được bật (SetActive(true)). | Đăng ký sự kiện (Event Listener), đặt lại trạng thái ban đầu. |
| Start() | Trước frame đầu tiên render, nhưng chỉ chạy nếu Script đang được bật. | Khởi tạo dữ liệu phụ thuộc vào các đối tượng khác trong Scene. |
| FixedUpdate() | Theo chu kỳ thời gian cố định (mặc định 0.02s / lần). | Xử lý các tính toán vật lý (Rigidbody, lực đẩy). |
| Update() | Được gọi **mỗi khung hình (frame)** một lần. | Xử lý input từ người chơi, logic di chuyển, kiểm tra sự kiện theo thời gian thực. |
| LateUpdate() | Chạy sau khi tất cả hàm Update() trong frame đó đã chạy xong. | Xử lý camera đuổi theo nhân vật, hoạt ảnh bám sát mục tiêu. |
| OnDisable() | Khi GameObject hoặc Component bị tắt đi (SetActive(false)). | Hủy đăng ký sự kiện, dừng các Coroutine ngầm để tránh rò rỉ bộ nhớ. |
| OnDestroy() | Khi đối tượng bị xóa vĩnh viễn khỏi Scene. | Dọn dẹp tài nguyên cuối cùng trước khi hủy. |

code ví dụ:

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
