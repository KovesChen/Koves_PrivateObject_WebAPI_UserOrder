# KOVES CHEN UserOrder Web API

## 專案說明
UserOrder Web API 是一個簡單的訂單管理系統，提供使用者、商品、訂單的 CRUD 與查詢功能。  

### 🛠️ 技術棧
* **.NET 8.0** – Web API 框架
* **Entity Framework Core** – ORM 資料存取
* **SQL Server** – 資料庫
* **AutoMapper + DTO** – 資料轉換與回傳模型
* **JWT Authentication** – 驗證與授權機制

### 🎯 功能模組
* 使用者帳號管理 (註冊 / 查詢 / 修改 / 刪除)
* 商品管理 (查詢 / 新增 / 修改 / 下架)
* 訂單管理 (查詢 / 新增 / 修改狀態 / 取消)
* JWT 登入認證與角色授權

### 🏗️ 架構設計
* 採用 **DI (依賴注入)** 管理服務生命週期
* 使用 **Interface + Service 分層**，確保程式可測試、可維護
* 控制器僅負責 API 輸入輸出，商業邏輯封裝於 Service

### 🚀 Docker 部署 (嘗試中)
* 我正在將 UserOrder Web API 容器化，以方便跨平台部署。  
* 目前已完成基本 Dockerfile，但可能還需要調整資料庫連線與環境設定。

---

## 授權規則
| 角色  | 功能 |
|-------|---------------------------------------------------|
| Admin | CRUD 所有資源 (使用者 / 商品 / 訂單)               |
| User  | 新增訂單、查詢自己的訂單、查看商品清單               |

所有 API 請求需要 JWT Token，放入 Header：
```

Authorization: Bearer {token}

````

---
## Controllers & API 文件
# 📖 API 文件 (ASP.NET Core 8.0)

## 📌 API 索引總覽

### 🔑 LoginController – 認證與角色授權

* [1.1 登入認證](#11-登入認證)

### 👤 UserController – 使用者帳號管理

* [2.1 查詢使用者](#21-查詢使用者)
* [2.2 註冊帳號](#22-註冊帳號)
* [2.3 更新帳號](#23-更新帳號)
* [2.4 刪除帳號](#24-刪除帳號)

### 🛒 ProductController – 商品管理

* [3.1 查詢商品清單](#31-查詢商品清單)
* [3.2 新增商品](#32-新增商品)
* [3.3 修改商品資料](#33-修改商品資料)
* [3.4 刪除商品](#34-刪除商品-邏輯刪除)

### 📦 OrderController – 訂單管理

* [4.1 查詢訂單清單](#41-查詢訂單清單)
* [4.2 新增訂單](#42-新增訂單)
* [4.3 修改訂單狀態](#43-修改訂單狀態)
* [4.4 刪除訂單](#44-刪除訂單)
---

### 1. LoginController   認證與角色授權

#### 1.1 登入認證 
- **URL:** `POST /api/user/Login`
- **授權:** 無

##### 📥 Request Body (`LoginPostParamenter`)
```jsonc
{
  "Account": "User",
  "Password": "123456789"
}
````

##### 📤 Response (成功)

```jsonc
{
  "success": true,
  "result": "JWT_TOKEN_HERE",
  "message": "認證成功",
  "details": "",
  "erro": ""
}
```

##### ⚠️ 錯誤回應

```jsonc
{
    "success": false,
    "result": null,
    "message": "帳號密碼錯誤",
    "details": "",
    "erro": ""
}


`````````

### 2. UserController  使用者帳號管理

#### 2.1 查詢使用者

* **URL:** `GET /api/Users`
* **授權角色:** `Admin`  => 可以查全部清單 / `User` => 僅能查自己的資料

---

##### 📥 Request Query (`GetUserListParameter`)

```jsonc
{
  "Id": 1,                 // Optional, 使用者 ID
  "Account": "testuser",   // Optional, 帳號
  "Email": "test@domain.com" // Optional, 信箱
}
```

---

##### 📤 Response (Admin → `GetUserListALLDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "id": 1,
      "userName": "admin01",
      "passwordHash": "*****",
      "email": "admin@domain.com",
      "active": "A",                // A=啟用, D=停用
      "creater": 1001,
      "createdAt": "2025-09-16T12:34:56Z"
    }
  ],
  "message": "取得成功"
}
```

---

##### 📤 Response (User → `GetUserDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "id": 2,
      "account": "normalUser",
      "email": "user@domain.com"
    }
  ],
  "message": "取得成功"
}
```

---

##### ⚠️ 錯誤回應

```jsonc
{
  "success": false,
  "message": "帳號 1023 取得失敗",
  "details": "資料庫錯誤訊息 (僅 Admin 可見)",
  "erro": "Exception inner message"
}


`````````

#### 2.2 註冊帳號

* **URL:** `POST /api/Users`
* **授權角色:** `Admin`

##### 📥 Request Body (`PostUserParameter`)

```jsonc
{
  "account": "newUser01",      // 必填, 6~15字元
  "password": "password123",   // 必填, 9~15字元
  "email": "user01@domain.com" // 必填, Email 格式
}
```

##### 📤 Response (成功)

```jsonc
{
  "success": true,
  "result": [
    {
      "id": 101,
      "account": "newUser01",
      "email": "user01@domain.com"
    }
  ],
  "message": "新增成功",
  "details": "",
  "erro": ""
}
```

---

##### ⚠️ 錯誤回應

```jsonc
{
    "success": false,
    "result": null,
    "message": "帳號 Koves96 資料新增失敗",
    "details": "使用者名稱Koves96已存在",
    "erro": null
}

`````````

#### 2.3 更新帳號

* **URL:** `PATCH /api/Users`
* **授權角色:** `Admin` => 能改全部資料 / `User` => 只能改密碼

##### 📥 Request Body (`UpdUserParamenter`)

```jsonc
{
  "id": 101,                // 必填
  "account": "updatedUser", // Optional, 6~15字元
  "password": "newPass123", // Optional, 9~15字元
  "email": "update@domain.com" // Optional, Email 格式
}
```

##### 📤 Response (Admin → `GetUserListALLDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "id": 101,
      "userName": "updatedUser",
      "passwordHash": "*****",
      "email": "update@domain.com",
      "active": "A",
      "creater": 1001,
      "createdAt": "2025-09-16T12:34:56Z"
    }
  ],
  "message": "更新成功"
}
```

##### 📤 Response (User → `GetUserDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "id": 101,
      "account": "updatedUser",
      "email": "update@domain.com"
    }
  ],
  "message": "更新成功"
}
```

---

##### ⚠️ 錯誤回應

```jsonc
{
    "success": false,
    "result": null,
    "message": "帳號Id 1029 資料更新失敗",
    "details": "帳號不存在!",
    "erro": null
}

`````````

#### 2.4 刪除帳號

* **URL:** `DELETE /api/Users`
* **授權角色:** `Admin` / `User`

##### 📥 Request Query (`DelUserParamenter`)

```jsonc
{
  "id": 101 // 必填
}
```

##### 📤 Response (Admin → 資料庫刪除)

```jsonc
{
  "success": true,
  "result": true,
  "message": "帳號 =>101刪除成功"
}
```

##### 📤 Response (User → 僅停用 `Active = D`)

```jsonc
{
  "success": true,
  "result": true,
  "message": "帳號 =>101刪除成功"
}
```

---

##### ⚠️ 錯誤回應

```jsonc
{
    "success": false,
    "result": null,
    "message": "帳號 1029 刪除失敗",
    "details": "帳號不存在!",
    "erro": null
}

`````````

### 3. ProductController  商品管理

#### 3.1 查詢商品清單

* **URL:** `GET /api/Products`
* **授權角色:** `Admin` / `User`

##### 📥 Request Query (`GetProductListParamenter`)

```jsonc
{
  "productId": 101,        // Optional
  "productName": "電腦"     // Optional
}
```

##### 📤 Response (Admin → `GetProductListALLInfoDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "productId": 101,
      "productName": "電腦",
      "price": 29999,
      "stock": 10,
      "active": "A",
      "creater": 1001,
      "createdAt": "2025-09-16T12:34:56Z"
    }
  ],
  "message": "取得成功"
}
```

##### 📤 Response (User → `GetProductListDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "productId": 101,
      "productName": "電腦",
      "price": 29999,
      "stock": 10
    }
  ],
  "message": "取得成功"
}
```

##### 📤 Response (錯誤範例)

```jsonc
{
  "success": false,
  "result": null,
  "message": "商品清單取得失敗",
  "details": "資料庫錯誤訊息 (僅 Admin 可見)",
  "erro": "Exception inner message"
}
```

---

#### 3.2 新增商品

* **URL:** `POST /api/Products`
* **授權角色:** `Admin`

##### 📥 Request Body (`PostProductParamenter`)

```jsonc
{
  "productName": "電腦",
  "price": 29999,
  "stock": 10
}
```

##### 📤 Response (成功範例)

```jsonc
{
  "success": true,
  "result": [
    {
      "productId": 101,
      "productName": "電腦",
      "price": 29999,
      "stock": 10,
      "active": "A",
      "creater": 1001,
      "createdAt": "2025-09-16T12:34:56Z"
    }
  ],
  "message": "新增成功"
}
```

##### 📤 Response (錯誤範例)

```jsonc
{
  "success": false,
  "result": null,
  "message": "商品名 電腦 資料新增失敗",
  "details": "資料庫錯誤訊息 (僅 Admin 可見)",
  "erro": "Exception inner message"
}
```

---

#### 3.3 修改商品資料

* **URL:** `PATCH /api/Products`
* **授權角色:** `Admin`

##### 📥 Request Body (`updProductParamenter`)

```jsonc
{
  "productId": 101,
  "productName": "筆電",
  "price": 27999,
  "stock": 8
}
```

##### 📤 Response (成功範例)

```jsonc
{
  "success": true,
  "result": [
    {
      "productId": 101,
      "productName": "筆電",
      "price": 27999,
      "stock": 8,
      "active": "A",
      "creater": 1001,
      "createdAt": "2025-09-16T12:34:56Z"
    }
  ],
  "message": "更新成功"
}
```

##### 📤 Response (錯誤範例)

```jsonc
{
  "success": false,
  "result": null,
  "message": "商品 101 資料更新失敗",
  "details": "資料庫連線錯誤",
  "erro": "Exception inner message"
}
```

---

#### 3.4 刪除商品 (邏輯刪除)

* **URL:** `DELETE /api/Products`
* **授權角色:** `Admin`

##### 📥 Request Query (`DelProductParamenter`)

```jsonc
{
  "productId": 101
}
```

##### 📤 Response (成功範例)

```jsonc
{
  "success": true,
  "result": true,
  "message": "產品 =>101刪除成功"
}
```

##### 📤 Response (失敗範例)

```jsonc
{
  "success": false,
  "result": null,
  "message": "產品 101 刪除失敗",
  "details": "資料庫連線錯誤",
  "erro": "Exception inner message"
}

`````````

### 4. ProductController  商品管理

---

#### 4.1 查詢訂單清單

* **URL:** `GET /api/Order`
* **授權角色:** `Admin` / `User`

##### 📥 Request Query (`GetOrderListParamenter`)

```jsonc
{
  "orderId": 1001,   // Optional
  "userId": 5        // Optional (User角色會自動帶入自己的UserId)
}
```

##### 📤 Response (Admin → `GetOrderListALLDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "orderId": 1001,
      "userId": 5,
      "totalAmount": 2500,
      "status": "Created",
      "creater": 1003,
      "createdAt": "2025-09-16T12:30:00Z",
      "orderItems": [
        {
          "itemNo": 1,
          "orderId": 1001,
          "productId": 101,
          "productName": "筆電",
          "quantity": 1,
          "unitPrice": 2500,
          "totPrice": 2500,
          "createdAt": "2025-09-16T12:30:00Z"
        }
      ]
    }
  ],
  "message": "取得成功"
}
```

##### 📤 Response (User → `GetOrderListDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "orderId": 1001,
      "userId": 5,
      "totalAmount": 2500,
      "status": "Created",
      "orderItems": [
        {
          "itemNo": 1,
          "orderId": 1001,
          "productId": 101,
          "productName": "筆電",
          "quantity": 1,
          "unitPrice": 2500,
          "totPrice": 2500
        }
      ]
    }
  ],
  "message": "取得成功"
}
```

##### 📤 Response (錯誤範例)

```jsonc
{
  "success": false,
  "result": null,
  "message": "訂單清單取得失敗",
  "details": "資料庫連線錯誤",
  "erro": "Exception inner message"
}
```

---

#### 4.2 新增訂單

* **URL:** `POST /api/Order`
* **授權角色:** `Admin` / `User`

##### 📥 Request Body (`PostOrderParamenter`)

```jsonc
{
  "userId": 5,
  "productList": [
    { "productId": 101, "quantity": 2 },
    { "productId": 102, "quantity": 1 }
  ]
}
```

##### 📤 Response (User → `GetOrderListDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "orderId": 1002,
      "userId": 5,
      "totalAmount": 3000,
      "status": "Created",
      "orderItems": [
        { "itemNo": 1, "orderId": 1002, "productId": 101, "productName": "筆電", "quantity": 2, "unitPrice": 1000, "totPrice": 2000 },
        { "itemNo": 2, "orderId": 1002, "productId": 102, "productName": "滑鼠", "quantity": 1, "unitPrice": 1000, "totPrice": 1000 }
      ]
    }
  ],
  "message": "新增成功"
}
```

##### 📤 Response (Admin → `GetOrderListALLDto`)

```jsonc
{
  "success": true,
  "result": [
    {
      "orderId": 1002,
      "userId": 5,
      "totalAmount": 3000,
      "status": "Created",
      "creater": 1,
      "createdAt": "2025-09-16T12:35:00Z",
      "orderItems": [
        { "itemNo": 1, "orderId": 1002, "productId": 101, "productName": "筆電", "quantity": 2, "unitPrice": 1000, "totPrice": 2000, "createdAt": "2025-09-16T12:35:00Z" },
        { "itemNo": 2, "orderId": 1002, "productId": 102, "productName": "滑鼠", "quantity": 1, "unitPrice": 1000, "totPrice": 1000, "createdAt": "2025-09-16T12:35:00Z" }
      ]
    }
  ],
  "message": "新增成功"
}
```

##### 📤 Response (錯誤範例)

```jsonc
{
  "success": false,
  "result": null,
  "message": "訂單資料新增失敗",
  "details": "資料庫連線錯誤",
  "erro": "Exception inner message"
}
```

---

#### 4.3 修改訂單狀態

* **URL:** `PATCH /api/Order`
* **授權角色:** `Admin`

##### 📥 Request Body (`UpdOrderParamenter`)

```jsonc
{
  "orderId": 1001,
  "status": "Shipped"
}
```

##### 📤 Response (成功範例)

```jsonc
{
  "success": true,
  "result": [
    {
      "orderId": 1001,
      "userId": 5,
      "totalAmount": 2500,
      "status": "Shipped",
      "creater": 1,
      "createdAt": "2025-09-16T12:30:00Z"
    }
  ],
  "message": "更新成功"
}
```

##### 📤 Response (錯誤範例)

```jsonc
{
  "success": false,
  "result": null,
  "message": "訂單 1001 資料更新失敗",
  "details": "Invalid status value",
  "erro": "Exception inner message"
}

```

---

#### 4.4 刪除訂單 

* **URL:** `DELETE /api/Order`
* **授權角色:** `Admin`

##### 📥 Request Query (`DelOrderParamenter`)

```jsonc
{
  "orderId": 1001
}
```

##### 📤 Response (成功範例)

```jsonc
{
  "success": true,
  "result": [
    {
      "orderId": 1001,
      "userId": 5,
      "totalAmount": 2500,
      "status": "Cancelled",
      "creater": 1,
      "createdAt": "2025-09-16T12:30:00Z"
    }
  ],
  "message": "刪除成功"
}
```

##### 📤 Response (錯誤範例)

```jsonc
{
  "success": false,
  "result": null,
  "message": "訂單 1001 刪除失敗",
  "details": "資料庫連線失敗",
  "erro": "Database connection error"
}

`````````
