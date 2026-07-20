USE [SkillBuilderProDb];

DELETE FROM Drills;
DBCC CHECKIDENT (Drills, RESEED, 0);

INSERT INTO Drills (Sport, Category, SubCategory, Name, Description, DifficultyLevel, Duration, VideoUrl, DateCreated) VALUES
('Basketball', 'Offense', 'Shooting', 'Shooting Form Fundamentals', 'Master the fundamentals of proper shooting technique', 1, 8, 'https://www.youtube.com/embed/UcnB9e5O5NY', '2026-07-20'),
('Basketball', 'Offense', 'Dribbling', 'Crossover Dribble Drill', 'Develop ball handling skills with crossover moves', 2, 7, 'https://www.youtube.com/embed/BnvGa0I8bMc', '2026-07-20'),
('Basketball', 'Offense', 'Passing', 'Passing Accuracy Drill', 'Improve passing precision and court vision', 2, 8, 'https://www.youtube.com/embed/OUskjh1r4Aw', '2026-07-20'),
('Basketball', 'Offense', 'Driving', 'Driving Layup Technique', 'Perfect your drive and finish at the rim', 2, 7, 'https://www.youtube.com/embed/XMFGTzxAi5I', '2026-07-20'),
('Basketball', 'Offense', 'Rebounding', 'Offensive Rebounding Positioning', 'Box out and secure offensive boards', 1, 6, 'https://www.youtube.com/embed/5_AszDpNxuc', '2026-07-20'),
('Basketball', 'Defense', 'Man-to-Man', 'Man-to-Man Defense Fundamentals', 'Master one-on-one defensive principles', 1, 8, 'https://www.youtube.com/embed/eP2nBwYdHek', '2026-07-20'),
('Basketball', 'Defense', 'Zone', 'Zone Defense Strategy', 'Learn area-based defensive coverage', 2, 9, 'https://www.youtube.com/embed/YVFqsTxJxQ0', '2026-07-20'),
('Basketball', 'Defense', 'Positioning', 'Defensive Positioning & Spacing', 'Maintain proper floor positioning', 2, 7, 'https://www.youtube.com/embed/VoRZPLfOLv8', '2026-07-20'),
('Basketball', 'Defense', 'Footwork', 'Lateral Footwork & Sliding', 'Improve lateral quickness and agility', 1, 6, 'https://www.youtube.com/embed/sIxe6cBk0qY', '2026-07-20'),
('Basketball', 'Defense', 'Rebounding', 'Defensive Rebounding Fundamentals', 'Secure defensive boards and outlet passes', 1, 7, 'https://www.youtube.com/embed/NiwfyUgomJE', '2026-07-20'),

('Football', 'Offense', 'Passing', 'QB Throwing Mechanics', 'Perfect the quarterback passing technique', 2, 10, 'https://www.youtube.com/embed/CjIiZcdto18', '2026-07-20'),
('Football', 'Offense', 'Footwork', 'Receiver Route Running Footwork', 'Execute precise route cuts and stems', 2, 8, 'https://www.youtube.com/embed/thxQUFhwwlo', '2026-07-20'),
('Football', 'Offense', 'Route Running', 'Route Running Techniques', 'Master receiver route trees and cuts', 2, 9, 'https://www.youtube.com/embed/b8Y-BrxoGQc', '2026-07-20'),
('Football', 'Offense', 'Ball Security', 'Carrying Ball Security Drills', 'Prevent fumbles and secure the football', 1, 6, 'https://www.youtube.com/embed/bnujHXSL8jg', '2026-07-20'),
('Football', 'Offense', 'Decision Making', 'QB Decision Making & Reading Defenses', 'Develop pre-snap and post-snap reads', 4, 12, 'https://www.youtube.com/embed/5xcK9H5GhyQ', '2026-07-20'),
('Football', 'Defense', 'Tackling', 'Proper Tackling Technique', 'Master safe and effective tackling form', 1, 8, 'https://www.youtube.com/embed/7wVRAsHDF_0', '2026-07-20'),
('Football', 'Defense', 'Coverage', 'Coverage Fundamentals & Assignments', 'Learn man and zone coverage concepts', 2, 10, 'https://www.youtube.com/embed/YI7b26LyPqQ', '2026-07-20'),
('Football', 'Defense', 'Gap Integrity', 'Gap Assignment & Control', 'Maintain defensive line gap assignments', 2, 9, 'https://www.youtube.com/embed/iore4RE2e4U', '2026-07-20'),
('Football', 'Defense', 'Film Study', 'Film Study & Opponent Recognition', 'Analyze game film and identify tendencies', 4, 15, 'https://www.youtube.com/embed/Fbch9ycvOJE', '2026-07-20'),
('Football', 'Defense', 'Footwork', 'Defensive Footwork & Agility', 'Improve lateral quickness and edge control', 2, 7, 'https://www.youtube.com/embed/kKSkNnUnWg0', '2026-07-20'),

('Baseball', 'Hitting', 'Load', 'Batting Load Mechanics', 'Master the proper loading sequence', 1, 7, 'https://www.youtube.com/embed/f64FsinnQqc', '2026-07-20'),
('Baseball', 'Hitting', 'Timing', 'Pitch Timing & Recognition', 'Develop bat control and timing', 2, 8, 'https://www.youtube.com/embed/ZKYBIx0Nf2s', '2026-07-20'),
('Baseball', 'Hitting', 'Bat Path — Contact', 'Bat Path to Contact Point', 'Optimize swing plane and contact quality', 2, 8, 'https://www.youtube.com/embed/4Jnd8N9Lwv4', '2026-07-20'),
('Baseball', 'Hitting', 'Rotation — Power', 'Hip Rotation & Power Generation', 'Develop explosive hip turn for power', 2, 9, 'https://www.youtube.com/embed/lR6B5KrZVJQ', '2026-07-20'),
('Baseball', 'Hitting', 'Pitch Recognition', 'Pitch Recognition Training', 'Distinguish pitch types pre-contact', 4, 10, 'https://www.youtube.com/embed/2uEoyrtBJGo', '2026-07-20'),
('Baseball', 'Fielding', 'Ground Balls', 'Ground Ball Fielding Technique', 'Perfect the ready position and approach', 1, 7, 'https://www.youtube.com/embed/OTP9eUBsqmk', '2026-07-20'),
('Baseball', 'Fielding', 'Fly Balls', 'Fly Ball Tracking & Positioning', 'Read and track batted fly balls', 1, 8, 'https://www.youtube.com/embed/7rAdbVEWEik', '2026-07-20'),
('Baseball', 'Fielding', 'Throwing', 'Throwing Accuracy & Mechanics', 'Develop strong and accurate throws', 2, 8, 'https://www.youtube.com/embed/UHRU973uu2c', '2026-07-20'),
('Baseball', 'Fielding', 'Footwork', 'Infield Footwork & Positioning', 'Master proper footwork and angles', 2, 7, 'https://www.youtube.com/embed/o62E2M0lMGc', '2026-07-20'),
('Baseball', 'Fielding', 'Double Plays', 'Double Play Execution', 'Turn efficient double plays', 4, 9, 'https://www.youtube.com/embed/4aWrAxBqd7g', '2026-07-20'),

('Softball', 'Hitting', 'Load', 'Batting Load Mechanics', 'Master the proper loading sequence', 1, 7, 'https://www.youtube.com/embed/m4lGySDMia0', '2026-07-20'),
('Softball', 'Hitting', 'Timing', 'Pitch Timing & Recognition', 'Develop bat control and timing', 2, 8, 'https://www.youtube.com/embed/KJujwuKC2Rc', '2026-07-20'),
('Softball', 'Hitting', 'Bat Path — Contact', 'Bat Path to Contact Point', 'Optimize swing plane and contact quality', 2, 8, 'https://www.youtube.com/embed/8flrjFMypvw', '2026-07-20'),
('Softball', 'Hitting', 'Rotation — Power', 'Hip Rotation & Power Generation', 'Develop explosive hip turn for power', 2, 9, 'https://www.youtube.com/embed/nb6mpD15Mmk', '2026-07-20'),
('Softball', 'Hitting', 'Pitch Recognition', 'Pitch Recognition Training', 'Distinguish pitch types pre-contact', 4, 10, 'https://www.youtube.com/embed/qFhOW8ng60s', '2026-07-20'),
('Softball', 'Fielding', 'Ground Balls', 'Ground Ball Fielding Technique', 'Perfect the ready position and approach', 1, 7, 'https://www.youtube.com/embed/YGdpIL_nx6k', '2026-07-20'),
('Softball', 'Fielding', 'Fly Balls', 'Fly Ball Tracking & Positioning', 'Read and track batted fly balls', 1, 8, 'https://www.youtube.com/embed/N1r590RT3RA', '2026-07-20'),
('Softball', 'Fielding', 'Throwing', 'Throwing Accuracy & Mechanics', 'Develop strong and accurate throws', 2, 8, 'https://www.youtube.com/embed/VpGFe85xUWs', '2026-07-20'),
('Softball', 'Fielding', 'Footwork', 'Infield Footwork & Positioning', 'Master proper footwork and angles', 2, 7, 'https://www.youtube.com/embed/AJMacRtbx9s', '2026-07-20'),
('Softball', 'Fielding', 'Sliding', 'Baserunning Sliding Technique', 'Master safe and effective sliding', 2, 6, 'https://www.youtube.com/embed/J3oyOBgUbtQ', '2026-07-20'),

('Soccer', 'Attacking', 'Dribbling', 'Dribbling Control & Moves', 'Develop close ball control and turns', 1, 8, 'https://www.youtube.com/embed/LtaViR7mYKo', '2026-07-20'),
('Soccer', 'Attacking', 'Passing', 'Passing Accuracy & Vision', 'Master short, medium, and long passes', 1, 8, 'https://www.youtube.com/embed/-V88Iy1X-is', '2026-07-20'),
('Soccer', 'Attacking', 'Shooting', 'Shooting Technique & Accuracy', 'Develop powerful and accurate shots', 2, 9, 'https://www.youtube.com/embed/2w6MA9JtYIU', '2026-07-20'),
('Soccer', 'Attacking', 'First Touch', 'First Touch Control', 'Receive the ball cleanly under pressure', 1, 7, 'https://www.youtube.com/embed/EcIrs7UzDFg', '2026-07-20'),
('Soccer', 'Attacking', 'Positioning', 'Attacking Movement & Positioning', 'Create and recognize scoring opportunities', 2, 8, 'https://www.youtube.com/embed/dkoWJPzxylA', '2026-07-20'),
('Soccer', 'Defending', 'Marking', 'Marking & Man-to-Man Defense', 'Pressure and mark opponents effectively', 1, 7, 'https://www.youtube.com/embed/7aABi-CjciQ', '2026-07-20'),
('Soccer', 'Defending', 'Sliding/Tackling', 'Sliding Tackle & Dispossession', 'Master safe tackling and ball recovery', 2, 7, 'https://www.youtube.com/embed/U9OcS2kPECk', '2026-07-20'),
('Soccer', 'Defending', 'Positioning', 'Defensive Positioning & Spacing', 'Maintain shape and defensive organization', 2, 8, 'https://www.youtube.com/embed/1whM0VIZTN8', '2026-07-20'),
('Soccer', 'Defending', 'Recovery', 'Defensive Recovery & Transition', 'Get back quickly after losing possession', 2, 7, 'https://www.youtube.com/embed/_Jnc8SU9S4o', '2026-07-20'),
('Soccer', 'Defending', 'Pressing', 'Pressing & High Defensive Line', 'Apply pressure early and win the ball', 4, 9, 'https://www.youtube.com/embed/nlmVIHCCRQ4', '2026-07-20'),

('Hockey', 'Skating', 'Edges', 'Skating Edge Control', 'Master inside and outside edge work', 1, 8, 'https://www.youtube.com/embed/pp0Y3BDDp4A', '2026-07-20'),
('Hockey', 'Skating', 'Speed', 'Speed & Acceleration Drills', 'Build explosive speed on the ice', 2, 8, 'https://www.youtube.com/embed/p9_cuf4pt6g', '2026-07-20'),
('Hockey', 'Skating', 'Agility', 'Lateral Agility & Stopping', 'Develop quick direction changes', 2, 7, 'https://www.youtube.com/embed/kki3-72Tl9M', '2026-07-20'),
('Hockey', 'Skating', 'Transitions', 'Forward-Backward Transitions', 'Transition smoothly between skating directions', 2, 8, 'https://www.youtube.com/embed/PLDEmj3cX-I', '2026-07-20'),
('Hockey', 'Skating', 'Conditioning', 'Hockey Conditioning & Stamina', 'Build on-ice endurance and fitness', 1, 10, 'https://www.youtube.com/embed/7SwrfyehxSA', '2026-07-20'),
('Hockey', 'Shooting', 'Wrist Shot', 'Wrist Shot Technique', 'Develop quick and accurate wrist shots', 1, 7, 'https://www.youtube.com/embed/7OgjkSygbjI', '2026-07-20'),
('Hockey', 'Shooting', 'Snap Shot', 'Snap Shot Power & Accuracy', 'Master the quick snap shot', 2, 7, 'https://www.youtube.com/embed/glopfhTeNMY', '2026-07-20'),
('Hockey', 'Shooting', 'Slap Shot', 'Slap Shot Mechanics', 'Perfect the powerful slap shot', 2, 8, 'https://www.youtube.com/embed/csGpqnok6p8', '2026-07-20'),
('Hockey', 'Shooting', 'Accuracy', 'Shooting Accuracy & Target Practice', 'Improve shooting consistency and placement', 2, 7, 'https://www.youtube.com/embed/oNLonI4x1jc', '2026-07-20'),
('Hockey', 'Shooting', 'Release', 'Shot Release & Follow-Through', 'Develop quick release and follow-through', 1, 6, 'https://www.youtube.com/embed/pJb16NO_svg', '2026-07-20');

SELECT COUNT(*) FROM Drills WHERE VideoUrl IS NOT NULL;
