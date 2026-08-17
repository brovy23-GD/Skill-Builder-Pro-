# Trophy Room Design Specification

Purpose: answer “What have I earned?” The protected `trophy_room_approved.png` is the primary visible hero/environment with Chicago identity, Performance Blue lighting, Football, exactly ONE Soccer, Hockey, Men's Basketball, Women's Basketball, Baseball, and the original Skill Builder Pro Softball trophy.

Live sections use `GET /api/athlete/trophy-room` for Current Rank, Rank Journey, Skill Milestones, and Achievements, plus `GET /api/athlete/goals` for a small Goals preview. Earned state comes only from the API; no trophy collection unlock rules or fake earned trophies exist. The approved room remains visible above real controls rather than being covered by opaque panels.

Phone uses vertical scroll. Tablet/Windows can widen and later adopt multi-column milestone/achievement grids. Future interactive trophy close-ups, collection unlock logic, and custom crests require backend approval and separate user graphic approval.

Every new graphic begins AWAITING USER APPROVAL. Codex cannot self-approve artwork.
# Phase 6A Finish Verification

Trophy Room presents live rank journey, milestones, and achievements over the approved visual. Deferred unlock behavior is labeled rather than simulated. Asset classification: GOOD.

## Final Visual Refinement

Borderless glass reduces obstruction without changing the approved image or trophy collection. Framing avoids excessive crop and progression/achievement authority remains server-side.

Final approved environment: `trophy_room_background_approved.png`. CurrentProgression, RankHistory, SkillMilestones, Achievements, streak, and NextRank remain live overlays; no unlock/filter behavior is fabricated.
