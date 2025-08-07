# Makefile cho flex-microservice
# Yêu cầu: bash/pwsh, docker, dotnet 8

SHELL := /bin/bash

# Cho phép override từ CLI: make build CONFIG=Release
CONFIG ?= Debug
SOLUTION ?= Flex.sln
# Tên service chính nếu cần chỉ định, ví dụ: IdentityService
SERVICE ?=
# Thư mục src
SRC_DIR := src

# Auto-discover tất cả csproj trong src (bỏ qua test ở run-all)
PROJECTS := $(shell find $(SRC_DIR) -name "*.csproj" -type f)
TEST_PROJECTS := $(shell find tests -name "*.csproj" -type f 2>/dev/null)

# Cross-platform pwsh fallback
PWSH := pwsh
ifeq (, $(shell which $(PWSH)))
  PWSH := pwsh.exe
endif

.PHONY: help
help:
	@echo "Targets:"
	@echo "  bootstrap   - Cài tool, restore, migrate DB, seed mẫu"
	@echo "  build       - Build toàn bộ solution ($(SOLUTION))"
	@echo "  test        - Chạy test + coverage"
	@echo "  run-all     - Chạy toàn bộ service (docker-compose hoặc dotnet run)"
	@echo "  migrate     - Apply migrations hoặc tạo migration mới (xem biến)"
	@echo "  lint        - Format & phân tích mã"
	@echo "Variables:"
	@echo "  CONFIG=Debug|Release, SERVICE=MyService, SOLUTION=Flex.sln"
	@echo "  MIGRATION_NAME=InitSchema (khi tạo migration mới)"
	@echo "  DB_CONTEXT=AppDbContext, PROJECT=src/IdentityService/IdentityService.csproj"

.PHONY: bootstrap
bootstrap:
	$(PWSH) -File ./scripts/bootstrap.ps1

.PHONY: build
build:
	dotnet restore $(SOLUTION)
	dotnet build $(SOLUTION) -c $(CONFIG) -warnaserror

.PHONY: test
test:
	@if [ -d "tests" ]; then \
	  dotnet test tests --no-build -c $(CONFIG) \
	    /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura ; \
	else \
	  echo "No tests/ directory found. Skipping."; \
	fi

.PHONY: run-all
run-all:
	$(PWSH) -File ./scripts/run-all.ps1 -Configuration $(CONFIG) -Service "$(SERVICE)"

.PHONY: migrate
migrate:
	$(PWSH) -File ./scripts/migrate.ps1 -Project "$(PROJECT)" -DbContext "$(DB_CONTEXT)" -MigrationName "$(MIGRATION_NAME)"

.PHONY: lint
lint:
	$(PWSH) -File ./scripts/lint.ps1
