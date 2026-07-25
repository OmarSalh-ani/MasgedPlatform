# Masged Unified Mobile API

**Single project:** `MasgedParentMobileAPI` — parent and teacher code in one host.

| Client | Routes | JWT |
|--------|--------|-----|
| Parent | `/api/*` | Default (`ApiSettings:Jwt`) |
| Teacher | `/api/teacher/*` | `TeacherJwt` |

Teacher implementation lives under [`Teacher/`](Teacher/) (`MasgedTeacherMobileAPI.*` namespaces).

## Run

```bash
cd MasgedParentMobileAPI
dotnet run
```

## Flutter

Use [ParentApp](../ParentApp/) only.
