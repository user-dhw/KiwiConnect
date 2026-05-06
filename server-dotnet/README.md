# server-dotnet

.NET 8 backend (ASP.NET Core Web API), replacing the original `server` (Node.js/Express) service.

## Migrated Endpoints (Batch 1 + Batch 2)

- Compatible routes:
    - `POST /webadmin/registered`
    - `POST /webadmin/login`
    - `POST /webadmin/getuser` (JWT)
    - `POST /webadmin/getusernumbering` (JWT)
    - `POST /webadmin/createhelp` (JWT)
    - `POST /webadmin/getwebhelplist` (JWT)
    - `POST /webadmin/gethelpdetails` (JWT)
    - `POST /webadmin/updateehelp` (JWT)
    - `POST /webadmin/deletehelp` (JWT)
    - `POST /webadmin/createactivity` (JWT)
    - `POST /webadmin/getwebactivitylist` (JWT)
    - `POST /webadmin/getactivitydetails` (JWT)
    - `POST /webadmin/updateactivity` (JWT)
    - `POST /webadmin/deleteactivity` (JWT)
    - `POST /webadmin/createoldstuff` (JWT)
    - `POST /webadmin/getweboldstufflist` (JWT)
    - `POST /webadmin/getoldstuffdetails` (JWT)
    - `POST /webadmin/updateoldstuff` (JWT)
    - `POST /webadmin/deleteoldstuff` (JWT)
    - `POST /webadmin/createarticle` (JWT)
    - `POST /webadmin/articlelist` (JWT)
    - `POST /webadmin/getarticledetails` (JWT)
    - `POST /webadmin/updatearticle` (JWT)
    - `POST /webadmin/deletejoin` (JWT)
    - `POST /webadmin/updatauser` (JWT)
    - `POST /webadmin/joinslist` (JWT)
    - `POST /webadmin/getwebjoinslist` (JWT)
    - `POST /webadmin/createfankui` (JWT)
    - `POST /webadmin/createjubao`
    - `POST /webadmin/createshensu`
    - `POST /webadmin/jubaocontent`
    - `POST /admin/login`
    - `POST /admin/getadmin` (JWT)
    - `POST /admin/numbering` (JWT)
    - `POST /admin/contentexamine` (JWT)
    - `POST /admin/changecontentstate` (JWT)
    - `POST /admin/getcomment` (JWT)
    - `POST /admin/getreply` (JWT)
    - `POST /admin/getuserlist` (JWT)
    - `POST /admin/getadminlist` (JWT)
    - `POST /admin/changeuserstate` (JWT)
    - `POST /admin/registered` (JWT)
    - `POST /admin/changepassword` (JWT)
    - `POST /admin/changeadminstate` (JWT)
    - `POST /admin/changeuseruserstate` (JWT)
    - `POST /admin/deleteuser` (JWT)
    - `POST /admin/admindelete` (JWT)
    - `POST /admin/lablelist` (JWT)
    - `POST /admin/changelable` (JWT)
    - `POST /admin/changecarousel` (JWT)
    - `POST /admin/carousellist` (JWT)
    - `POST /admin/deletecarouse` (JWT)
    - `POST /admin/kefullist` (JWT)
    - `POST /admin/changkefustate` (JWT)
    - `POST /admin/deletekefu` (JWT)
    - `POST /admin/changresult` (JWT)
    - `POST /admin/changeadminuser` (JWT)
    - `POST /admin/changeactivationdate` (JWT)
    - `POST /admin/announcementlist` (JWT)
    - `POST /admin/setannouncement` (JWT)
    - `POST /uplod` (single file upload)
    - `POST /web/webgetwebhelplist`
    - `POST /web/gethelpcontent`
    - `POST /web/setcomment` (JWT)
    - `POST /web/getcomment`
    - `POST /web/setreply` (JWT)
    - `POST /web/getreply`
    - `POST /web/webgetwebactivitylist`
    - `POST /web/webgetweboldstufflist`
    - `POST /web/getarticlelist`
    - `POST /web/getarticlecontent`
    - `POST /web/getoldstuffcontent`
    - `POST /web/getactivitycontent`
    - `POST /web/setjoin` (JWT)
    - `POST /web/search`
    - `POST /web/getnotice` (JWT)
    - `POST /web/changenotice` (JWT)
- Original response structure preserved: `{ state, data }`
- List endpoints preserve original structure: `{ state, data, count }`
- Original password rule preserved: `md5(password + salt)`
- Original config semantics preserved: MySQL + JWT + upload directory `uplodes`
- Unified exception handling added (global middleware)
- JWT auth failures return a compatible structure (with `code: 401`)
- Admin permission granularity checks added (`isyh/isgl/issh/isfk`)
- Basic parameter validation added (key login/register DTOs)

## Running Locally

```bash
cd server-dotnet

dotnet restore
dotnet run
```

Default port: `http://localhost:3000`

## Configuration

Config file: `appsettings.json`

- `Database`: MySQL connection parameters
- `Jwt`: secret key, expiry, issuer, audience
- `Security.PasswordSalt`: password salt matching the legacy system
- `FileStorage.UploadRoot`: upload directory (default `uplodes`)

## Directory Overview

- `Program.cs`: app startup, DI, CORS, JWT, static file mapping
- `Infrastructure/`: database connection factory, MD5, JWT services
- `Features/WebAdmin/`: user-facing account endpoints (register/login/get user)
- `Features/Admin/`: back-office admin endpoints (login/get admin)
- `Features/Web/`: public content endpoints (list/detail/comment/search/notice/join)
- `Features/Common/UploadController.cs`: file upload endpoint
- `Infrastructure/Web/GlobalExceptionMiddleware.cs`: global exception handling middleware

## Recommended Next Steps (by priority)

1. Add integration tests for key APIs
2. Run a full end-to-end regression between frontend and backend (login, review, customer service, announcements)
3. Optional: add finer-grained DTO validation rules (field length, enum values)

## Migration Strategy

Recommended parallel running approach:

- Keep the old `server` online;
- Route new endpoints to `.NET` first;
- Switch the gateway to `.NET` after completing module-level regression.
