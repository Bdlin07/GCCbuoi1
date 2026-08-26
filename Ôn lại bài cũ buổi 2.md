### **1. Truyền tham trị (Pass by Value) - Mặc định trong C#**

- Bản chất: Khi truyền một biến vào hàm, C# sẽ tạo ra một bản sao (copy) hoàn toàn độc lập của biến đó và ném bản sao vào hàm.

- Hậu quả: Mọi thay đổi, tính toán, gán giá trị diễn ra với tham số đó bên trong hàm chỉ tác động lên bản sao. Biến gốc ở bên ngoài hoàn toàn vô can và không bị thay đổi.

- Đối tượng áp dụng: Mọi kiểu dữ liệu cơ bản (như int, float, bool, double, char...) đều được truyền theo tham trị.

```csharp
using System;
class Program
{
    static void CongThem(int so)
    {
        so += 10; // Chỉ có bản sao bên trong hàm tăng lên 10
        Console.WriteLine($"Bên trong hàm: {so}"); // In ra: 110
    }
    static void Main()
    {
        int x = 100;
        CongThem(x);
        // Biến x bên ngoài vẫn giữ nguyên giá trị cũ vì truyền tham trị
        Console.WriteLine($"Bên ngoài hàm: {x}"); // In ra: 100
    }
}
```

### **2. Truyền tham chiếu (Pass by Reference) - Dùng từ khóa ref**

- Bản chất: Thay vì tạo bản sao giá trị, C# sẽ cung cấp địa chỉ ô nhớ gốc (tham chiếu) của biến đó cho hàm.

- Hậu quả: Hàm trực tiếp làm việc với ô nhớ gốc. Bất kỳ thay đổi nào thực hiện bên trong hàm sẽ ngay lập tức làm thay đổi giá trị của biến gốc ở bên ngoài.

- Đối tượng áp dụng: Bất kỳ biến nào mà bạn muốn hàm có quyền can thiệp trực tiếp và thay đổi giá trị của nó, bằng cách thêm từ khóa ref (hoặc out) vào trước tên biến.

```csharp
using System;
class Program
{
    // Thêm ref để truyền địa chỉ ô nhớ gốc
    static void CongThem(ref int so)
    {
        so += 10; // Tác động thẳng vào ô nhớ gốc ở ngoài
        Console.WriteLine($"Bên trong hàm: {so}"); // In ra: 110
    }
    static void Main()
    {
        int x = 100;
        CongThem(ref x); // Bắt buộc phải có từ khóa ref khi gọi
        // Biến x bên ngoài đã bị thay đổi giá trị
        Console.WriteLine($"Bên ngoài hàm: {x}"); // In ra: 110
    }
}
```

### **1. Const (Hằng số)**

- **Ý nghĩa:** Dùng để khai báo một giá trị **không bao giờ thay đổi** từ lúc viết code cho đến khi chạy chương trình.

- **Đặc điểm quan trọng:**

- Phải được gán giá trị **ngay tại thời điểm khai báo**.

- Nó mang tính chất "tĩnh" (static) ngầm, nghĩa là bạn truy cập trực tiếp qua tên Class mà không cần tạo object (new).

- Giá trị của nó được thay thế trực tiếp bằng hằng số đó tại thời điểm biên dịch (Compile-time).

```csharp
class GameSettings
{
    // Khai báo hằng số số mạng tối đa
    public const int MaxLives = 3;
}

// Cách dùng:
// int lives = GameSettings.MaxLives;
// GameSettings.MaxLives = 5; -> BÁO LỖI ngay lập tức vì không thể thay đổi hằng số!
```

### **2. Readonly (Chỉ đọc khi chạy)**

- **Ý nghĩa:** Cũng dùng để tạo biến "chỉ đọc", nhưng linh hoạt hơn const.

- **Đặc điểm quan trọng:**

- Giá trị của nó chỉ có thể được gán **ngay lúc khai báo** HOẶC **bên trong hàm khởi tạo (Constructor)** của Class đó.

- Khi chương trình đã chạy (Run-time) và hàm khởi tạo đã chạy xong, giá trị của biến readonly sẽ bị đóng băng, không ai sửa được nữa.

- Thường dùng cho các cấu hình hoặc dữ liệu chỉ được nạp một lần khi khởi tạo đối tượng (ví dụ: ID của nhân vật, ngày tạo tài khoản).

```csharp
class Player
{
    // ID của người chơi chỉ được phép gán một lần duy nhất lúc tạo
    public readonly int PlayerId;

    public Player(int id)
    {
        PlayerId = id; // Hợp lệ trong Constructor
    }

    public void ChangeId()
    {
        // PlayerId = 10; -> BÁO LỖI! Không thể gán lại giá trị ngoài Constructor.
    }
}
```

### **3. ref (Tham chiếu biến)**

- **Ý nghĩa:** Bình thường trong C#, khi bạn truyền một biến vào hàm, C# sẽ tạo ra một bản sao (Copy) của biến đó, nên nếu bạn sửa biến trong hàm thì biến ở ngoài không bị ảnh hưởng. Từ khóa **ref** dùng để truyền **chính xác ô nhớ gốc** của biến vào hàm.

- **Đặc điểm quan trọng:**

- Biến được truyền vào **bắt buộc phải được gán giá trị** trước khi đưa vào hàm.

- Cả bên trong hàm và bên ngoài hàm đều có thể đọc và thay đổi giá trị của biến đó.

```csharp
class HP
{
    // Dùng ref để can thiệp trực tiếp vào biến gốc ở ngoài
    public static void TangMau(ref int hpHienTai)
    {
        hpHienTai += 50; // Máu gốc ở ngoài Main sẽ tăng theo
    }
}

class Program
{
    static void Main()
    {
        int myHp = 100;

        // Bắt buộc phải thêm từ khóa ref khi gọi hàm
        HP.TangMau(ref myHp);

        // Kết quả in ra sẽ là 150 vì biến gốc đã bị thay đổi
        System.Console.WriteLine(myHp);
    }
}
```

### **4. out (Tham số đầu ra)**

- **Ý nghĩa:** Giống như ref (truyền theo tham chiếu để thay đổi giá trị gốc), nhưng out chuyên dùng cho trường hợp **hàm cần trả về nhiều kết quả cùng lúc**.

- **Đặc điểm quan trọng:**

- Biến truyền vào **không cần phải gán giá trị trước** khi gọi hàm.

- **Quy tắc bắt buộc:** Hàm nhận tham số out **phải gán giá trị cho biến đó** trước khi kết thúc hàm (nếu không trình biên dịch sẽ báo lỗi).

```csharp
class Calculator
{
    // Hàm này vừa muốn chia lấy phần nguyên, vừa muốn lấy số dư thông qua out
    public static void Chia(int a, int b, out int thuong, out int soDu)
    {
        thuong = a / b;
        soDu = a % b; // Bắt buộc phải gán giá trị cho out trước khi thoát hàm
    }
}

class Program
{
    static void Main()
    {
        int t, d;

        // Không cần khởi tạo trước t và d, gọi hàm xong chúng sẽ tự nhận giá trị
        Calculator.Chia(10, 3, out t, out d);

        System.Console.WriteLine($"Thương: {t}, Số dư: {d}"); // Kết quả: Thương: 3, Số dư: 1
    }
}
```
