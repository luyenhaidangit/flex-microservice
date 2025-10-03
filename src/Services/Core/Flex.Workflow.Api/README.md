Flex.Workflow.Api

Overview
- Centralized approval workflow service implementing maker–checker with hash-chain audit and outbox.
- Stores workflow definitions, requests, actions and audit logs.

Key Entities
- `WorkflowDefinition`: Versioned definitions with steps/policy JSON.
- `ApprovalRequest`: Request header using shared RequestBase fields plus Domain/WorkflowCode.
- `ApprovalAction`: Each approve/reject step with actor/comment/evidence.
- `WorkflowAuditLog`: Append-only hash-chain audit per request.
- `OutboxEvent`: Event outbox for integration.

Endpoints
- `GET /api/definitions` – list definitions (optional `onlyActive=true`).
- `GET /api/definitions/{code}` – get active definition by code.
- `POST /api/definitions` – upsert a definition.
- `POST /api/requests` – create a new approval request.
- `GET /api/requests/pending` – list pending requests (paged filters).
- `GET /api/requests/{id}` – request detail (actions + audits).
- `POST /api/requests/{id}/approve` – approve pending request.
- `POST /api/requests/{id}/reject` – reject pending request.

Configuration
- Oracle wallet: `OracleWallet` section in appsettings (see System service for example).
- RabbitMQ: `RabbitMQ` section for MassTransit bus.

Notes
- MVP assumes single-step approval (finalizes on first approval).
- Audit uses SHA256(prev+material) to form a verifiable chain.
- Idempotency and policy evaluation hooks are ready for extension.

