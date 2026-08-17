# Goals & Progress Design Specification

Purpose: answer “What am I working toward?” with the approved slogan **SET IT. CHASE IT. ACHIEVE IT.** The protected approved graphic `goals_progress_approved.png` supplies the visible Chicago/action hero and blueprint; live MAUI controls supply all data.

The page order is hero, Your Focus, server progression metrics, Active Goals, summary-only Your Progress, premium Vision Board placeholder/derived targets, achievement/recent-win/rank opportunities, and Completed Goals. Current Phase 6A implements the hero, focus, metrics, active/completed goals, truthful progress-history limitation, and vision-board state. It consumes `GET /api/athlete/goals` and `GET /api/athlete/progression`; server CurrentValue, TargetValue, ProgressPercent, status and rank remain authoritative.

Phone uses a vertical scroll and responsive grids; Windows/tablet can expand card width without fixed positioning. Chicago identity comes from the approved visual, with charcoal/graphite surfaces and Performance Blue. The backend lacks goal time-series, personal mantra, dream championship, and season-goal data, so no chart or Athlete values are fabricated. Future work may add goal management/detail, richer achievements/recent wins, and width-triggered multi-column templates.

Every new graphic begins AWAITING USER APPROVAL. Codex cannot self-approve artwork.
# Phase 6A Finish Verification

Goals renders live focus and goal state with truthful empty/deferred states. The current asset is MARGINAL for a wide layout; `goals_background_wide.png` is proposed and awaits approval.

## Final Visual Refinement

Shared glass now uses transparent strokes and softer tint, retaining server authority and truthful unsupported history/Vision Board states. The portrait source remains the wide-screen limitation.

Final approved environment: `goals_background_approved.png`. Goal rows, percentages, progress bars, metrics, and completed state are live. Unsupported creation/history behavior remains disabled or explicitly deferred.
