# 🚀 Redis Cache Best Practices Guideline

Đây là tài liệu tổng hợp các best practice (hướng dẫn tốt nhất) khi sử dụng Redis làm cache trong hệ thống microservices, tài chính, ngân hàng lưu ký, hoặc các hệ thống lớn. Áp dụng cho ASP.NET Core, các nền tảng cloud (AWS, Azure, GCP), và các môi trường production.

---

## 🎯 Mục tiêu
- Đảm bảo cache được dùng **đúng chỗ**, tránh lạm dụng gây bug hoặc mất nhất quán dữ liệu.
- Giúp dev, BA, PM, QA có tiêu chuẩn thống nhất.
- Tối ưu performance & cost cho Redis Cluster.
- Đảm bảo bảo mật, vận hành ổn định, dễ mở rộng.

---

## ✅ Khi nào nên dùng Redis Cache

| Use case                    | Có nên cache? | Lý do / ví dụ cụ thể |
|------------------------------|---------------|-----------------------|
| 🔍 Lookup data (masterdata, branch, role) | ✅ Yes | Tỷ lệ đọc cao, ít thay đổi. VD: branch:123 |
| 🔥 Homepage / Dashboard list | ✅ Yes | Cho phép stale vài giây-phút |
| 🎯 Search suggest, tag cloud | ✅ Yes | Chỉ cần dữ liệu gần đúng |
| 📝 User profile, preference  | ✅ Yes | Không critical nếu trễ vài phút |
| 📈 Top N ranking (ZSET)       | ✅ Yes | Dùng ZSET score, update định kỳ |

---

## 🚫 Khi **không nên dùng cache**

| Use case                   | Không dùng cache | Lý do |
|-----------------------------|------------------|-------|
| 💰 Số dư tài khoản (balance) | ❌ No | Phải ACID, tuyệt đối nhất quán |
| 🏦 Ledger / Transaction      | ❌ No | Critical ledger, đọc DB trực tiếp |
| 🔄 Dữ liệu update liên tục   | ⚠️ Thận trọng | TTL quá thấp có thể gây cache storm |
| 📊 Dataset nhỏ (<=100 rows)  | ⚠️ Thận trọng | Index DB nhanh hơn, cache overhead |
| 📑 Audit logs, event stream  | ❌ No | Write-heavy, không cần cache |

---

# 🏆 100+ Redis Cache Best Practices

### 1. Thiết kế & Kiến trúc

1. Luôn đặt TTL (expire) cho mọi key.
2. Sử dụng namespace/prefix cho key (vd: user:profile:{id}).
3. Chọn đúng cấu trúc dữ liệu: STRING, HASH, SET, ZSET, LIST.
4. Tránh lưu giá trị lớn hơn 1MB.
5. Không dùng Redis làm primary database.
6. Chỉ cache dữ liệu có tỉ lệ đọc cao, ít thay đổi.
7. Không cache dữ liệu cần consistency tuyệt đối (balance, ledger).
8. Chỉ cache dữ liệu chấp nhận stale vài giây-phút.
9. Tránh cache dataset nhỏ (<100 rows).
10. Đặt key schema rõ ràng, có tài liệu hóa.
11. Sử dụng version cho key (vd: branch:v2:{id}).
12. Tránh cache dữ liệu update liên tục.
13. Chia nhỏ key nếu dữ liệu lớn (sharding).
14. Sử dụng pipelining/batch GET để giảm round-trip.
15. Đặt TTL ngắn cho dữ liệu thay đổi thường xuyên.
16. Đặt TTL dài cho dữ liệu ít thay đổi.
17. Sử dụng ZSET cho ranking, leaderboard.
18. Sử dụng HASH cho object nhiều field.
19. Sử dụng SET cho tập hợp không trùng lặp.
20. Sử dụng LIST cho queue đơn giản.
21. Tránh dùng KEYS * trong production.
22. Không dùng FLUSHALL/FLUSHDB trên môi trường thật.
23. Sử dụng cache aside pattern (read-through/write-through).
24. Invalidate cache khi dữ liệu gốc thay đổi.
25. Sử dụng distributed lock khi cần đồng bộ.
26. Tránh cache stampede bằng lock hoặc random TTL.
27. Sử dụng cache warming cho dữ liệu hot.
28. Sử dụng lazy loading cho cache miss.
29. Đặt limit cho số lượng key mỗi namespace.
30. Sử dụng eviction policy phù hợp (LRU, LFU, volatile-ttl...).
31. Monitor eviction, expired_keys, hit/miss.
32. Đặt alert khi hit rate giảm.
33. Sử dụng Prometheus/Grafana để giám sát.
34. Đặt security group, firewall chặn IP lạ.
35. Bật TLS nếu truy cập qua internet.
36. Sử dụng requirepass, ACL cho Redis.
37. Không hardcode password trong code.
38. Đặt password mạnh, đổi định kỳ.
39. Không expose Redis port ra internet.
40. Sử dụng VPC/private subnet cho Redis cluster.
41. Sử dụng Redis Sentinel/Cluster cho HA.
42. Đặt maxmemory phù hợp với workload.
43. Sử dụng RDB/AOF backup định kỳ.
44. Test restore backup thường xuyên.
45. Đặt client timeout hợp lý.
46. Sử dụng connection pool cho client.
47. Đặt limit cho số connection.
48. Sử dụng retry/backoff khi gặp lỗi connection.
49. Log lại lỗi cache miss/cache error.
50. Đặt circuit breaker cho cache layer.

### 2. Tối ưu hiệu năng

51. Tránh N+1 query với cache.
52. Sử dụng batch operation khi có thể.
53. Tránh lưu trữ dữ liệu binary lớn (image, file).
54. Sử dụng compress (gzip/lz4) cho value lớn.
55. Đặt key ngắn gọn, dễ đọc.
56. Tránh key quá dài (>128 bytes).
57. Tránh value quá lớn (>1MB).
58. Sử dụng scan thay cho keys để duyệt key.
59. Đặt limit cho scan để tránh overload.
60. Sử dụng pub/sub cho event realtime.
61. Tránh abuse pub/sub cho message lớn.
62. Sử dụng sorted set cho top N, ranking.
63. Sử dụng expire randomization để tránh cache storm.
64. Đặt TTL random ±10% cho key cùng loại.
65. Sử dụng multi-key operation cẩn thận (mget, mset).
66. Tránh multi-key trên cluster nếu key khác slot.
67. Sử dụng hash tag để group key cùng slot.
68. Đặt limit cho số field trong hash.
69. Tránh hash quá lớn (>10k field).
70. Sử dụng unlink thay cho del để xóa key lớn.
71. Sử dụng lazy deletion để tránh block Redis.
72. Đặt maxmemory-policy phù hợp với use case.
73. Monitor memory fragmentation.
74. Sử dụng INFO command để kiểm tra health.
75. Đặt alert khi memory usage >80%.
76. Đặt alert khi connected clients >80% limit.
77. Đặt alert khi evicted_keys tăng đột biến.
78. Đặt alert khi rejected_connections tăng.
79. Đặt alert khi latency spike.
80. Sử dụng slowlog để debug command chậm.
81. Đặt limit cho slowlog-max-len.
82. Đặt loglevel phù hợp (warning/info).
83. Đặt protected-mode yes cho Redis standalone.
84. Đặt client-output-buffer-limit cho pub/sub.
85. Đặt tcp-keepalive cho connection lâu dài.
86. Đặt save config phù hợp cho RDB.
87. Đặt appendfsync phù hợp cho AOF.
88. Đặt auto-aof-rewrite-percentage.
89. Đặt auto-aof-rewrite-min-size.
90. Đặt stop-writes-on-bgsave-error no nếu cần HA.
91. Đặt lazyfree-lazy-eviction yes cho workload lớn.
92. Đặt lazyfree-lazy-expire yes cho workload lớn.
93. Đặt lazyfree-lazy-server-del yes cho workload lớn.
94. Đặt replica-read-only yes cho replica.
95. Đặt min-replicas-to-write cho HA.
96. Đặt min-replicas-max-lag cho HA.
97. Đặt repl-backlog-size phù hợp.
98. Đặt repl-timeout phù hợp.
99. Đặt client-query-buffer-limit.
100. Đặt maxclients phù hợp với server.

### 3. Checklist vận hành & bảo trì

101. Kiểm tra version Redis định kỳ, update bản vá bảo mật.
102. Test failover/switchover định kỳ.
103. Kiểm tra log Redis định kỳ.
104. Kiểm tra backup restore định kỳ.
105. Kiểm tra config drift giữa các node.
106. Kiểm tra latency từ app đến Redis.
107. Kiểm tra network partition risk.
108. Kiểm tra security scan định kỳ.
109. Kiểm tra access log, audit log.
110. Đào tạo dev/ops về Redis best practice.
111. Có runbook xử lý sự cố Redis.
112. Có checklist khi deploy code mới liên quan cache.
113. Có checklist khi scale up/down Redis.
114. Có checklist khi migrate data.
115. Có checklist khi rotate password.
116. Có checklist khi update config.
117. Có checklist khi update version.
118. Có checklist khi restore backup.
119. Có checklist khi failover.
120. Có checklist khi phát hiện security incident.

---

## 📝 Sample key convention

```plaintext
branch:v2:{branchId} -> HASH (Name, Code, IsActive, CreatedDate)
role:{roleName}      -> HASH (Description, Permissions)
session:{userId}     -> STRING JWT / encrypted session
top:stocks           -> ZSET (score=volume)
```

---

## 📚 Tham khảo thêm
- [Redis Official Documentation](https://redis.io/docs/)
- [Redis Security Checklist](https://redis.io/docs/management/security/)
- [Redis Memory Optimization](https://redis.io/docs/management/memory-optimization/)
- [AWS ElastiCache Best Practices](https://docs.aws.amazon.com/AmazonElastiCache/latest/red-ug/best-practices.html)

---

> **Lưu ý:** Tài liệu này là guideline tổng hợp, cần tùy chỉnh cho từng hệ thống cụ thể. Đội ngũ dev/ops nên review định kỳ để cập nhật các best practice mới nhất.
