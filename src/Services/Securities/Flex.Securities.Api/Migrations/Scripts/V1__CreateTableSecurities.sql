﻿CREATE TABLE SECURITIES (
    Id NUMBER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, -- IDENTITY tự tăng
    Symbol VARCHAR2(250 CHAR) NOT NULL,                     -- Ký hiệu chứng khoán
    TradePlace VARCHAR2(10 CHAR) NOT NULL,                  -- Nơi giao dịch
    Description CLOB,                                       -- Mô tả

    -- Trường audit
    CreatedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL, -- Ngày tạo
    LastModifiedDate TIMESTAMP                              -- Ngày chỉnh sửa cuối cùng
);