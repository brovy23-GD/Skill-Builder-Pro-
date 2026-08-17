# MAUI Login Design Specification

Use the unchanged approved `weight_room.png` environment, a restrained glass login surface, and role-specific CTA language. Authenticate through `POST /api/auth/login`, restore through `GET /api/auth/me`, store only JWT/expiry in SecureStorage, never persist passwords, and reject selected-role/JWT-role mismatches. Athlete/Parent public signup may use the existing register contract; Coach/Admin remain controlled provisioning. Implementation expansion is blocked by the API runtime gate.
# Phase 6A Finish Verification

Role-aware login rendering and semantic input descriptions were verified on Windows for Athlete. Centralized glass styling is applied without obscuring the approved visual.
