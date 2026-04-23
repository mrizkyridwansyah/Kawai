1. 📘 Overview
    Aplikasi ini adalah sistem WMS (Warehouse Management Sistem) berbasis web yang digunakan untuk kontrol stock di Kawai. Sistem dibangun menggunakan:
    - Frontend  : Nuxt 3 
    - Backend   : .NET 8
    - Database  : Sql Server
    - MQ        : RabbitMQ (4.1.0) - Erlang(27.3.4)
    - Monitoring: Grafana (opsional) 
    - Lainnya   : npm (10.2.4)

2. 🏗️ Arsitektur Sistem
    [Client (Browser)]
        ↓
    [Frontend (Nuxt)]
        ↓
    [API Server (.NET 8)]
        ↓
    [Database (Sql Server)]

3. ⚙️ Setup dan Instalasi
    📌 Prasyarat:
    - Node.js v20.9.0
    - npm v10.2.4
    - SSMS
    - Visual Studio 2022 
    - Git
    - RabbitMQ
    - Grafana (opsional) 

    🛠️ Langkah Instalasi:
    1.  install Erlang & RabbitMQ 
        Bisa pake chocolately / manual
    2.  Clone Repository
        git clone https://github.com/mrizkyridwansyah/Kawai.git
    3.  Front End Web
        - cd Kawai.FE
        - npm install
        - npm run dev (untuk run debug)
    4.  Front End Andon
        - cd Kawai.Andon.FE
        - npm install
        - npm run dev (untuk run debug)
    5.  Back End 
        - open Kawai.sln di VS
        - build project Kawai.Api
        - run

4. 📁 Struktur Folder BackEnd
    /Kawai.Api      # API Utama. Controller, Middleware, SignalR, CronJobs, MQ ada disini
        /Controller     # Entry Point API
        /CronJobs       # Background Job Hangfire biar ga pake SqlJobs because SqlJobs is sucks
        /Hub            # Notification SignalR
        /Services       # Custom Folder aja, isi nya handler2 buat File Storage, MQ, Notif Service, Session Manager, Email, dll 
        /Shared         # Custom Folder juga 
    /Kawai.Data     # Layer untuk ke database. Eksekusi Query & SP ada disini
        /Repositories   # File untuk eksekusi query & sp sesuai action di Interface
        /Sql            # File query skema database agar bisa ditracking di git. (Tiap PERUBAHAN SKEMA DB, TOLONG TARO DISINI!!)
        /SqlConnection  # File untuk configurasi multi koneksi sql. (Jangan Sembarangan Diubah ya) 
    /Kawai.Domain  # Layer untuk DTO, Model & Interface dari Kawai.Data. 
        /DTOs           # Class untuk proses baca dan kembalian ke client. 
        /Models         # Class untuk proses tulis. untuk template payload yg mau client kirim.
        /Interfaces     # Interface untuk digunakan di Controller, sbg Dependency Injection.
        /Shared
            /Validator  # Anotasi2 yg digunakan sebagai validasi property di Models.
            DataResult.cs       # Standard Format Response List
            RawString.cs        # Handler untuk remove leading & trailing karakter spasi, table, whitespace dll (karna di db masih pake tipe data char)
            RequestParameter.cs # Standard Format Request untuk List / DataTable
        
5. 📡 API Documentation
    # Gunakan file: Kawai.postman_collection.json
    # Import ke Postman untuk mencoba semua endpoint yang tersedia.

6. 🗃️ Database Schema
    # Dapat dilihat langsung di folder: Kawai.Data/Sql
    # Semua perubahan skema WAJIB dicatat di sini untuk tracking via Git.

7. 🧩 Modul & Fitur
    - Auth      : Token-base Auth
    - Mobile    : Transaksi
    - Web       : Master Data, Report, Transaksi
    - Andon     : Ringkasan data penting 

8. 🚀 Deployment
    📍 Backend & Frontend
        Semua modul dideploy di IIS dengan port berikut:
        - API (Backend) : Port 9100
        - Web           : Port 9200
        - Andon         : Port 9300
        - APK (Mobile)  : Port 9400
    
    🔧 Langkah Deployment
        1.  Frontend (Web & Andon)
            - Pastikan baseUrl di app.config.json sudah mengarah ke URL API yang benar.
            - Jalankan build: npm run generate
            - Buat folder wwwroot pada direktori target (9200/9300).
            - Copy isi dari folder .output/public ke folder wwwroot

        2.  Backend (.NET)
            - Pastikan Frontend URL sudah ditambahkan ke AllowedOrigins di appsettings.json
            - Publish project Kawai.Api
            - Copy hasil publish ke direktori target.
            - Tambahkan konfigurasi berikut ke web.config
                <modules runAllManagedModulesForAllRequests="true">
                <remove name="WebDAVModule" /> <!-- Ini akan menonaktifkan WebDAV -->
                </modules>


⚠️ NOTE PENTING

-   Semua transaksi (web/mobile/dll) yang memengaruhi stok HARUS melalui MQ (RabbitMQ) untuk mencegah locking dan menjaga akurasi data stok.
-   Mekanisme MQ:
    -   Saat request dikirim ke API yang menggunakan MQ, client menerima HTTP 202 Accepted: "TRANSAKSI DITERIMA DAN ON PROCESS".
    -   Ketika transaksi selesai dieksekusi oleh MQ, client akan menerima notifikasi via SignalR apakah transaksi berhasil atau gagal.
-   Perubahan skema database:
    -   WAJIB diupdate di folder Kawai.Data/Sql
    -   Tujuannya agar skema live dan local bisa dibandingkan saat pengembangan fitur baru.

-   CronJobs (Hangfire):
    -   Gunakan Hangfire sebagai pengganti SQL Jobs (lebih fleksibel dan bisa diatur manual).
    -   Fitur Hangfire: requeue, view history, delay jika job sebelumnya belum selesai.
    -   Minimum interval job: 1 menit.