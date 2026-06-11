# Deploy IELTS PKM Backend (miễn phí — Oracle Cloud Always Free)

Chạy toàn bộ hệ thống (5 service + Gateway + MongoDB + PostgreSQL) trong Docker
trên 1 VM miễn phí, dùng được từ điện thoại ở bất kỳ đâu.

---

## 0. Chuẩn bị: push code mới lên GitHub

Trên máy bạn (thư mục `backend`):

```bash
git add Dockerfile docker-compose.prod.yml .env.example .dockerignore DEPLOY.md .gitignore
git add src/Gateway src/Services   # các file code mới
git commit -m "feat: containerize backend + production compose"
git push
```

> `.env` (bí mật thật) KHÔNG được commit — đã nằm trong `.gitignore`.

---

## 1. Tạo VM miễn phí trên Oracle Cloud

1. Đăng ký https://www.oracle.com/cloud/free/ (cần thẻ visa để xác minh, **không bị trừ tiền** ở Always Free).
2. Tạo Instance:
   - **Image:** Ubuntu 22.04
   - **Shape:** `VM.Standard.A1.Flex` (ARM) — chọn **2 OCPU / 12 GB RAM** (vẫn trong Always Free).
   - Tải xuống **SSH private key** khi tạo.
3. Ghi lại **Public IP** của VM.

### Mở cổng 5000 (2 nơi)
- **Oracle Security List:** VCN → Security Lists → Add Ingress Rule: Source `0.0.0.0/0`, TCP, Destination port **5000**.
- **Trên VM (sau khi SSH vào):**
  ```bash
  sudo iptables -I INPUT 6 -m state --state NEW -p tcp --dport 5000 -j ACCEPT
  sudo netfilter-persistent save
  ```

---

## 2. SSH vào VM & cài Docker

```bash
ssh -i <đường-dẫn-private-key> ubuntu@<PUBLIC_IP>

# Cài Docker + compose plugin
sudo apt update && sudo apt install -y docker.io docker-compose-plugin git
sudo usermod -aG docker $USER
exit                      # đăng xuất rồi SSH lại để áp quyền docker
```

---

## 3. Lấy code & tạo file bí mật

```bash
git clone https://github.com/HakaRyi/IELTSServer.git
cd IELTSServer        # (thư mục backend của bạn trên GitHub)

cp .env.example .env
nano .env             # điền GROQ_API_KEY thật, đặt mật khẩu DB + JWT_KEY mạnh
```

`.env` cần điền:
- `MONGO_PASSWORD`, `POSTGRES_PASSWORD` — đặt mật khẩu chỉ chữ-số (khỏi URL-encode).
- `JWT_KEY` — chuỗi ngẫu nhiên ≥ 32 ký tự.
- `GROQ_API_KEY` — key từ console.groq.com.

---

## 4. Chạy

```bash
docker compose -f docker-compose.prod.yml up -d --build
```

Lần đầu build ~5-10 phút. Kiểm tra:

```bash
docker compose -f docker-compose.prod.yml ps        # tất cả Up
docker compose -f docker-compose.prod.yml logs -f gateway
```

Test từ máy bạn:
```bash
curl http://<PUBLIC_IP>:5000/api/auth/register -H "Content-Type: application/json" \
  -d '{"email":"a@a.com","username":"test","password":"123456"}'
```
→ trả về JSON có accessToken là OK.

---

## 5. Trỏ app Flutter tới VM

Sửa **1 dòng** trong `mobile/ielts_pkm/lib/core/app_config.dart`:

```dart
static const String _gateway = 'http://<PUBLIC_IP>:5000';
```

Build APK để cài lên điện thoại thật:
```bash
cd mobile/ielts_pkm
flutter build apk --release
# file: build/app/outputs/flutter-apk/app-release.apk → copy vào điện thoại cài
```

Giờ app dùng được ở bất kỳ đâu (4G/WiFi khác).

---

## 6. Vận hành

```bash
# Cập nhật code mới
git pull && docker compose -f docker-compose.prod.yml up -d --build

# Xem log 1 service
docker compose -f docker-compose.prod.yml logs -f practice

# Dừng / khởi động lại
docker compose -f docker-compose.prod.yml restart
docker compose -f docker-compose.prod.yml down      # giữ data (volume)
```

Dữ liệu Mongo/Postgres lưu trong Docker volume `mongo_data` / `postgres_data` — không mất khi restart.

---

## 7. (Khuyến nghị về sau) Bật HTTPS

Hiện chạy HTTP trần (đủ để dùng). Để bảo mật thật:
- Mua/đăng ký domain miễn phí (DuckDNS), trỏ về Public IP.
- Đặt **Caddy** trước Gateway (tự xin Let's Encrypt SSL):
  ```
  yourdomain.duckdns.org {
      reverse_proxy gateway:5000
  }
  ```
- Đổi `_gateway` trong app thành `https://yourdomain...` và mở cổng 443.
