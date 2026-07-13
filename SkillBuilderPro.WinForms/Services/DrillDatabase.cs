using System;
using System.Collections.Generic;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Services
{
    public static class DrillDatabase
    {
        // ─── PUBLIC ENTRY POINT ───────────────────────────────────────────
        public static List<Drill> GetDrillsBySport(string sport)
        {
            if (string.IsNullOrWhiteSpace(sport))
                return new List<Drill>();

            switch (sport.Trim().ToLower())
            {
                case "basketball": return GetBasketballDrills();
                case "baseball": return GetBaseballDrills();
                case "softball": return GetSoftballDrills();
                case "football": return GetFootballDrills();
                case "soccer": return GetSoccerDrills();
                case "hockey": return GetHockeyDrills();
                default: return new List<Drill>();
            }
        }

        public static List<Drill> GetAllDrills()
        {
            var all = new List<Drill>();
            all.AddRange(GetBasketballDrills());
            all.AddRange(GetBaseballDrills());
            all.AddRange(GetSoftballDrills());
            all.AddRange(GetFootballDrills());
            all.AddRange(GetSoccerDrills());
            all.AddRange(GetHockeyDrills());
            return all;
        }
        /// <summary>
        /// Builds a TrainingSchedule from the athlete's selected drills.
        /// Called from MainForm.cs when the Generate button is clicked:
        ///     currentSchedule = DrillDatabase.GenerateTrainingSchedule(
        ///         CurrentUser.FullName, CurrentUser.Sport, selectedDrills);
        /// </summary>
        /// <param name="athleteName">Athlete's full name from CurrentUser.FullName</param>
        /// <param name="sport">Sport from CurrentUser.Sport</param>
        /// <param name="selectedDrills">Drills checked in the CheckedListBox</param>
        /// <returns>TrainingSchedule with Items ready to populate the schedule grid</returns>
        public static TrainingSchedule GenerateTrainingSchedule(
            string athleteName, string sport, List<Drill> selectedDrills)
        {
            // Create the schedule shell with athlete info
            var schedule = new TrainingSchedule
            {
                AthleteName = athleteName,
                Sport = sport,
                DateCreated = DateTime.Now,
                Items = new List<ScheduleItem>()
            };

            // Distribute drills across days of the week
            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            int dayIndex = 0;

            // Each selected drill becomes one row in the schedule grid
            foreach (var drill in selectedDrills)
            {
                schedule.Items.Add(new ScheduleItem
                {
                    Day = days[dayIndex % days.Length], // wraps if more than 7 drills
                    DrillName = drill.Name,
                    Category = drill.SkillCategory,
                    Duration = drill.Duration,
                    Notes = drill.Description
                });
                dayIndex++;
            }

            return schedule;
        }
        // ─── BASKETBALL ───────────────────────────────────────────────────
        private static List<Drill> GetBasketballDrills()
        {
            return new List<Drill>
            {
                // DRIBBLING
                new Drill { Id=Guid.NewGuid(), Name="Crossover Dribble", Description="Quick hand transfer between legs at game speed", SkillCategory="Dribbling", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Behind-the-Back Dribble", Description="Dribble ball behind body to change direction", SkillCategory="Dribbling", Sport="Basketball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Between-the-Legs Dribble", Description="Bounce ball between legs while moving", SkillCategory="Dribbling", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Hesitation Dribble", Description="Pause dribble to break defender's momentum", SkillCategory="Dribbling", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Speed Dribble", Description="Push ball forward in open court at maximum pace", SkillCategory="Dribbling", Sport="Basketball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Low Dribble", Description="Keep ball low to ground for tight ball control", SkillCategory="Dribbling", Sport="Basketball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Figure-8 Dribble", Description="Dribble in figure-8 pattern around cones", SkillCategory="Dribbling", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Retreat Dribble", Description="Dribble backward while facing defender", SkillCategory="Dribbling", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Directional Dribble", Description="Rapidly change dribble direction on command", SkillCategory="Dribbling", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="One-Hand Dribble Control", Description="Master dribbling with weak hand only", SkillCategory="Dribbling", Sport="Basketball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                // SHOOTING
                new Drill { Id=Guid.NewGuid(), Name="Spot-Up Three-Pointer", Description="Shoot from fixed position beyond arc", SkillCategory="Shooting", Sport="Basketball", Difficulty=2, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Mid-Range Pull-Up", Description="Stop and shoot from 15-18 feet", SkillCategory="Shooting", Sport="Basketball", Difficulty=2, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Catch-and-Shoot", Description="Receive pass and immediately shoot", SkillCategory="Shooting", Sport="Basketball", Difficulty=2, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Off-the-Dribble Jumper", Description="Dribble then quickly rise for shot", SkillCategory="Shooting", Sport="Basketball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Contested Shot", Description="Shoot with defender's hand in face", SkillCategory="Shooting", Sport="Basketball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="One-Foot Floater", Description="Shoot with one foot off ground in lane", SkillCategory="Shooting", Sport="Basketball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Step-Back Three", Description="Step back and shoot three-pointer", SkillCategory="Shooting", Sport="Basketball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Free Throw", Description="Unguarded shot from 15 feet (free throw line)", SkillCategory="Shooting", Sport="Basketball", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Fadeaway Shot", Description="Shoot while fading away from defender", SkillCategory="Shooting", Sport="Basketball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Bank Shot", Description="Shoot off backboard from 45-degree angle", SkillCategory="Shooting", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                // REBOUNDING
                new Drill { Id=Guid.NewGuid(), Name="Box-Out Technique", Description="Use body to create space for rebound", SkillCategory="Rebounding", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Vertical Jump Practice", Description="Maximum height leap for rebounds", SkillCategory="Rebounding", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Pursuit Rebound", Description="Chase loose ball after missed shot", SkillCategory="Rebounding", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Defensive Rebound", Description="Secure rebound on defensive end", SkillCategory="Rebounding", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Offensive Rebound", Description="Fight for rebound on opponent's basket", SkillCategory="Rebounding", Sport="Basketball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Outlet Pass", Description="Secure rebound and make quick outlet pass", SkillCategory="Rebounding", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Positioning Drill", Description="Get in correct spot before rebound falls", SkillCategory="Rebounding", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Two-Hand Grab", Description="Secure ball with both hands at apex", SkillCategory="Rebounding", Sport="Basketball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tip-Out", Description="Redirect rebound away from basket to teammate", SkillCategory="Rebounding", Sport="Basketball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Rebound & Finish", Description="Get rebound and score putback quickly", SkillCategory="Rebounding", Sport="Basketball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                // DEFENSE
                new Drill { Id=Guid.NewGuid(), Name="Stance & Footwork", Description="Proper defensive position and lateral movement", SkillCategory="Defense", Sport="Basketball", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Slide Defense", Description="Keep in front of opponent using quick slides", SkillCategory="Defense", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Hand Deflections", Description="Knock ball loose without fouling", SkillCategory="Defense", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Pressure Defense", Description="Aggressive full-court coverage", SkillCategory="Defense", Sport="Basketball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Help Defense", Description="Rotate to help teammate defending", SkillCategory="Defense", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Transition Defense", Description="Run back quickly on opponent's break", SkillCategory="Defense", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Perimeter Defense", Description="Guard player shooting from outside", SkillCategory="Defense", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Post Defense", Description="Guard player in low post", SkillCategory="Defense", Sport="Basketball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Pick-and-Roll Defense", Description="Navigate through screens", SkillCategory="Defense", Sport="Basketball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Steal Drills", Description="Time deflections to force turnovers", SkillCategory="Defense", Sport="Basketball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                // PASSING
                new Drill { Id=Guid.NewGuid(), Name="Chest Pass", Description="Quick pass from chest at game speed", SkillCategory="Passing", Sport="Basketball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Bounce Pass", Description="Pass that bounces to receiver", SkillCategory="Passing", Sport="Basketball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Overhead Pass", Description="Pass from above shoulders for distance", SkillCategory="Passing", Sport="Basketball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Baseball Pass", Description="One-hand long pass like baseball throw", SkillCategory="Passing", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="No-Look Pass", Description="Pass while looking opposite direction", SkillCategory="Passing", Sport="Basketball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Alley-Oop Pass", Description="Lob pass to teammate for dunk", SkillCategory="Passing", Sport="Basketball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Entry Pass", Description="Pass inside to post player", SkillCategory="Passing", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Skip Pass", Description="Pass across court to opposite side", SkillCategory="Passing", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Inbound Pass", Description="Quick pass from sideline after timeout", SkillCategory="Passing", Sport="Basketball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Kick-Out Pass", Description="Swing ball to open shooter", SkillCategory="Passing", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                // SPEED & AGILITY
                new Drill { Id=Guid.NewGuid(), Name="Lane Agility Drill", Description="Fast footwork in confined space", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Cone Weave", Description="Zigzag through cones at high speed", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lateral Shuffle", Description="Quick side-to-side movement", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backpedal Sprint", Description="Run backward at full speed", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Jump Rope", Description="Rope skipping for foot speed", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Ladder Drill", Description="Quick footwork through agility ladder", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Box Drill", Description="Sprint between four points in square", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Three-Quarter Court Sprint", Description="Full-speed running length", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Change of Direction Drill", Description="Quick cuts and transitions", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Reaction Drill", Description="React to coach's command with sprint", SkillCategory="Speed & Agility", Sport="Basketball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
            };
        }

        // ─── BASEBALL ────────────────────────────────────────────────────
        private static List<Drill> GetBaseballDrills()
        {
            return new List<Drill>
            {
                // HITTING
                new Drill { Id=Guid.NewGuid(), Name="Batting Practice", Description="Regular pitches at game speed", SkillCategory="Hitting", Sport="Baseball", Difficulty=2, Duration=30, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tee Work", Description="Hit from stationary tee", SkillCategory="Hitting", Sport="Baseball", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Soft Toss", Description="Ball tossed underhand from side", SkillCategory="Hitting", Sport="Baseball", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Live Pitching", Description="Face actual pitcher", SkillCategory="Hitting", Sport="Baseball", Difficulty=3, Duration=30, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Two-Strike Approach", Description="Adjust approach with 2 strikes", SkillCategory="Hitting", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Gap Hitting", Description="Hit ball to left-center/right-center", SkillCategory="Hitting", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Opposite Field Hitting", Description="Hit to opposite side of field", SkillCategory="Hitting", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Bunting Drill", Description="Square bunt technique practice", SkillCategory="Hitting", Sport="Baseball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Slap Hitting", Description="Quick contact hit to beat defense", SkillCategory="Hitting", Sport="Baseball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Home Run Swing", Description="Maximum power swing for distance", SkillCategory="Hitting", Sport="Baseball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                // PITCHING
                new Drill { Id=Guid.NewGuid(), Name="Fastball Accuracy", Description="Throw fastball to target", SkillCategory="Pitching", Sport="Baseball", Difficulty=2, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Curveball Control", Description="Breaking ball with proper spin", SkillCategory="Pitching", Sport="Baseball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Changeup Practice", Description="Off-speed pitch timing", SkillCategory="Pitching", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Slider Technique", Description="Lateral moving pitch", SkillCategory="Pitching", Sport="Baseball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Four-Seam Fastball", Description="Backspin fastball for rise", SkillCategory="Pitching", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Two-Seam Fastball", Description="Running fastball with movement", SkillCategory="Pitching", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Sinker Drill", Description="Downward moving pitch", SkillCategory="Pitching", Sport="Baseball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Cutter Pitch", Description="Late movement fastball", SkillCategory="Pitching", Sport="Baseball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Leg Drive", Description="Explosive power from lower body", SkillCategory="Pitching", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Arm Path Consistency", Description="Repeat exact arm angle", SkillCategory="Pitching", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                // FIELDING
                new Drill { Id=Guid.NewGuid(), Name="Ground Ball Fielding", Description="Field and throw to base", SkillCategory="Fielding", Sport="Baseball", Difficulty=2, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Fly Ball Tracking", Description="Track and catch fly balls", SkillCategory="Fielding", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Pop-Up Practice", Description="Catch short pop-ups", SkillCategory="Fielding", Sport="Baseball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Line Drive Reaction", Description="Quick reflex to line drives", SkillCategory="Fielding", Sport="Baseball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Throws to Base", Description="Accuracy to first, second, third", SkillCategory="Fielding", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Relay Throws", Description="Sequence of throws in relay", SkillCategory="Fielding", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backing Up Base", Description="Cover base when primary player commits", SkillCategory="Fielding", Sport="Baseball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Cutoff Position", Description="Proper angle for cutoff throws", SkillCategory="Fielding", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Double Play Turn", Description="Execute double play pivot", SkillCategory="Fielding", Sport="Baseball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tag at Base", Description="Proper technique for tag", SkillCategory="Fielding", Sport="Baseball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                // BASERUNNING
                new Drill { Id=Guid.NewGuid(), Name="Rounding First Base", Description="Proper wide turn technique", SkillCategory="Baserunning", Sport="Baseball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Rounding Second Base", Description="Full-speed turn reading field", SkillCategory="Baserunning", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Rounding Third Base", Description="Reading relay throw", SkillCategory="Baserunning", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Sliding Technique", Description="Safe sliding to base", SkillCategory="Baserunning", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Stealing Base", Description="Get good jump on pitcher", SkillCategory="Baserunning", Sport="Baseball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lead Off", Description="Maximum distance from base safely", SkillCategory="Baserunning", Sport="Baseball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tagging Up", Description="Position for scoring on fly ball", SkillCategory="Baserunning", Sport="Baseball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="First to Third", Description="Running hard and reading play", SkillCategory="Baserunning", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Home Plate Slide", Description="Proper slide into home", SkillCategory="Baserunning", Sport="Baseball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Reading Pitcher", Description="Identify wild pitch or passed ball", SkillCategory="Baserunning", Sport="Baseball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                // SPEED & AGILITY
                new Drill { Id=Guid.NewGuid(), Name="Cone Weave", Description="Navigate through cones quickly", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="60-Yard Dash", Description="Speed measurement test", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=2, Duration=10, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lateral Shuffle", Description="Side-to-side footwork", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backpedal Sprint", Description="Run backward at high speed", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Jump Rope", Description="Rope skipping for coordination", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Agility Ladder", Description="Rapid footwork through ladder", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Line Drill", Description="Sprint between baselines", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Box Drill", Description="Run around square perimeter", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Acceleration Burst", Description="Quick start from dead stop", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Change of Direction", Description="Plant and cut sharply", SkillCategory="Speed & Agility", Sport="Baseball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
            };
        }

        // ─── SOFTBALL ────────────────────────────────────────────────────
        private static List<Drill> GetSoftballDrills()
        {
            return new List<Drill>
            {
                // HITTING
                new Drill { Id=Guid.NewGuid(), Name="Tee Hitting", Description="Ball on stationary tee", SkillCategory="Hitting", Sport="Softball", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Soft Toss", Description="Ball tossed from side", SkillCategory="Hitting", Sport="Softball", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Live Pitching", Description="Face actual pitcher", SkillCategory="Hitting", Sport="Softball", Difficulty=3, Duration=30, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Weight Load", Description="Proper weight shift into back leg", SkillCategory="Hitting", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Hip Rotation", Description="Explosive hip drive through ball", SkillCategory="Hitting", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Bat Speed Drill", Description="Swing speed development", SkillCategory="Hitting", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Gap Hitting", Description="Hit to left-center/right-center", SkillCategory="Hitting", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Rise Ball Hitting", Description="Swing at rise pitch", SkillCategory="Hitting", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Drop Ball Hitting", Description="Swing at drop pitch", SkillCategory="Hitting", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Bunting Drill", Description="Sacrifice and speed bunts", SkillCategory="Hitting", Sport="Softball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                // PITCHING
                new Drill { Id=Guid.NewGuid(), Name="Windmill Mechanics", Description="Full arm circle technique", SkillCategory="Pitching", Sport="Softball", Difficulty=2, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Rising Fastball", Description="Pitch with upward movement", SkillCategory="Pitching", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Drop Ball", Description="Pitch with downward movement", SkillCategory="Pitching", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Curveball", Description="Breaking pitch with spin", SkillCategory="Pitching", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Changeup", Description="Off-speed pitch variation", SkillCategory="Pitching", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Riseball Accuracy", Description="Placement on rise pitches", SkillCategory="Pitching", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Dropball Accuracy", Description="Placement on drop pitches", SkillCategory="Pitching", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Pitch Sequence", Description="Vary pitches strategically", SkillCategory="Pitching", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Release Point", Description="Consistent arm angle", SkillCategory="Pitching", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Spin Rate Control", Description="Adjust spin for different pitches", SkillCategory="Pitching", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                // FIELDING
                new Drill { Id=Guid.NewGuid(), Name="Ground Ball Infield", Description="Field and throw from infield", SkillCategory="Fielding", Sport="Softball", Difficulty=2, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Fly Ball Outfield", Description="Track and catch fly balls", SkillCategory="Fielding", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Line Drive Reaction", Description="Quick reflex catches", SkillCategory="Fielding", Sport="Softball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Throwing Accuracy", Description="Accurate throws to bases", SkillCategory="Fielding", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backhand Plays", Description="Field ball off backhand", SkillCategory="Fielding", Sport="Softball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Double Play Pivot", Description="Execute double play", SkillCategory="Fielding", Sport="Softball", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tag Technique", Description="Proper tag at base", SkillCategory="Fielding", Sport="Softball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Cut-Off Throws", Description="Relay throw to proper base", SkillCategory="Fielding", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Charging Ball", Description="Move in on ground balls", SkillCategory="Fielding", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backing Up Base", Description="Cover position on play", SkillCategory="Fielding", Sport="Softball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                // BASERUNNING
                new Drill { Id=Guid.NewGuid(), Name="First Base Run", Description="Top speed to first", SkillCategory="Baserunning", Sport="Softball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Rounding Bases", Description="Proper wide turn on bases", SkillCategory="Baserunning", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Base Stealing", Description="Get jump on pitcher", SkillCategory="Baserunning", Sport="Softball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Sliding Technique", Description="Safe and proper slide", SkillCategory="Baserunning", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Reading Pitcher", Description="Identify wild pitches", SkillCategory="Baserunning", Sport="Softball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lead-Off Distance", Description="Maximum safe distance from base", SkillCategory="Baserunning", Sport="Softball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tagging Up", Description="Position for fly ball scoring", SkillCategory="Baserunning", Sport="Softball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Home Plate Slide", Description="Slide into home safely", SkillCategory="Baserunning", Sport="Softball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="First to Third", Description="Read play while running", SkillCategory="Baserunning", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Decision Making", Description="Know when to advance/return", SkillCategory="Baserunning", Sport="Softball", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                // SPEED & AGILITY
                new Drill { Id=Guid.NewGuid(), Name="Cone Weave", Description="Navigate through cones", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="60-Yard Dash", Description="Speed measurement", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=2, Duration=10, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lateral Shuffle", Description="Side-to-side movement", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backpedal Drill", Description="Run backward quickly", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Jump Rope", Description="Rope skipping for coordination", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Agility Ladder", Description="Rapid footwork", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Box Drill", Description="Run perimeter at speed", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Acceleration Burst", Description="Quick explosive start", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Direction Change", Description="Plant and cut sharply", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Change of Pace", Description="Alternate fast/slow movement", SkillCategory="Speed & Agility", Sport="Softball", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
            };
        }

        // ─── FOOTBALL ────────────────────────────────────────────────────
        private static List<Drill> GetFootballDrills()
        {
            return new List<Drill>
            {
                // RECEIVING
                new Drill { Id=Guid.NewGuid(), Name="Slant Route", Description="Quick 45-degree cut across field", SkillCategory="Receiving", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Post Route", Description="Vertical run then cut toward center", SkillCategory="Receiving", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Comeback Route", Description="Run upfield then cut back toward QB", SkillCategory="Receiving", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Corner Route", Description="Run vertical then cut to corner", SkillCategory="Receiving", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Hook Route", Description="Run upfield then turn to face QB", SkillCategory="Receiving", Sport="Football", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Wheel Route", Description="Run upfield then curve toward sideline", SkillCategory="Receiving", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="In Route", Description="Run upfield then cut inside", SkillCategory="Receiving", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Out Route", Description="Run upfield then cut toward sideline", SkillCategory="Receiving", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Go Route", Description="Full-speed vertical route downfield", SkillCategory="Receiving", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Dig Route", Description="Short upfield run with quick cut across", SkillCategory="Receiving", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                // PASSING
                new Drill { Id=Guid.NewGuid(), Name="Three-Step Drop", Description="Quick release from quick footwork", SkillCategory="Passing", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Five-Step Drop", Description="Mid-range throw preparation", SkillCategory="Passing", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Seven-Step Drop", Description="Deep throw setup", SkillCategory="Passing", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Play Action", Description="Fake handoff then throw", SkillCategory="Passing", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Screen Pass", Description="Short pass behind line of scrimmage", SkillCategory="Passing", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Slant Pass", Description="Throw to receiver on diagonal route", SkillCategory="Passing", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Bootleg Pass", Description="Roll out and throw from movement", SkillCategory="Passing", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Two-Minute Drill", Description="High-pressure quick throw situations", SkillCategory="Passing", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Red Zone Pass", Description="Throwing in scoring area", SkillCategory="Passing", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Third-Down Conversion", Description="Critical situation throw", SkillCategory="Passing", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                // BLOCKING
                new Drill { Id=Guid.NewGuid(), Name="Base Block", Description="Drive forward and block defender", SkillCategory="Blocking", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Trap Block", Description="Lead block on defensive target", SkillCategory="Blocking", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Pull Block", Description="Run behind line to block downfield", SkillCategory="Blocking", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Pass Protection", Description="Shield QB from rushing defender", SkillCategory="Blocking", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Double Team", Description="Two blockers on one defender", SkillCategory="Blocking", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Screen Block", Description="Block defender away from play", SkillCategory="Blocking", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Reach Block", Description="Block defender outside you", SkillCategory="Blocking", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Downfield Block", Description="Follow tackle downfield", SkillCategory="Blocking", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lead Block", Description="Block in front of ball carrier", SkillCategory="Blocking", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Chip Block", Description="Brief contact then release downfield", SkillCategory="Blocking", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                // DEFENSE
                new Drill { Id=Guid.NewGuid(), Name="Gap Control", Description="Stay assigned to your gap", SkillCategory="Defense", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Shed Block", Description="Separate from blocker quickly", SkillCategory="Defense", Sport="Football", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Pursuit Drill", Description="Chase ball carrier across field", SkillCategory="Defense", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tackling Form", Description="Proper technique to bring down runner", SkillCategory="Defense", Sport="Football", Difficulty=2, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Angle Tackling", Description="Take proper angle to target", SkillCategory="Defense", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Head-Up Tackling", Description="Keep head up while striking", SkillCategory="Defense", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Coverage", Description="Stay with assigned receiver", SkillCategory="Defense", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Blitz Timing", Description="Time gap penetration correctly", SkillCategory="Defense", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Read Keys", Description="React to QB's movement keys", SkillCategory="Defense", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Strip Ball", Description="Knock ball loose from ball carrier", SkillCategory="Defense", Sport="Football", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                // RUNNING
                new Drill { Id=Guid.NewGuid(), Name="Inside Zone", Description="Run between blocks upfield", SkillCategory="Running", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Outside Stretch", Description="Run to edge of field", SkillCategory="Running", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Power Run", Description="Drive through line with authority", SkillCategory="Running", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Speed Sweep", Description="Run the perimeter at high speed", SkillCategory="Running", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Counter Run", Description="Plant and cut opposite direction", SkillCategory="Running", Sport="Football", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Cutback", Description="Plant foot and cut back against grain", SkillCategory="Running", Sport="Football", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Jump Cut", Description="Jump over pile at line of scrimmage", SkillCategory="Running", Sport="Football", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Stiff Arm", Description="Hold off defender with straight arm", SkillCategory="Running", Sport="Football", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Breaking Tackle", Description="Shake off defender's grip", SkillCategory="Running", Sport="Football", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Vision Drill", Description="Read blocks and find running lane", SkillCategory="Running", Sport="Football", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                // SPEED & AGILITY
                new Drill { Id=Guid.NewGuid(), Name="Cone Weave", Description="Navigate through cones quickly", SkillCategory="Speed & Agility", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Three-Cone Drill", Description="Measure athletic ability and agility", SkillCategory="Speed & Agility", Sport="Football", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Shuttle Run", Description="Sprint 5-10-5 yard shuttle", SkillCategory="Speed & Agility", Sport="Football", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="40-Yard Dash", Description="Maximum speed test", SkillCategory="Speed & Agility", Sport="Football", Difficulty=2, Duration=10, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lateral Shuffle", Description="Quick side movement", SkillCategory="Speed & Agility", Sport="Football", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backpedal Drill", Description="Run backward at speed", SkillCategory="Speed & Agility", Sport="Football", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Jump Rope", Description="Rope skipping for coordination", SkillCategory="Speed & Agility", Sport="Football", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Ladder Drill", Description="Quick footwork through agility ladder", SkillCategory="Speed & Agility", Sport="Football", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="T-Drill", Description="Sprint forward, lateral, opposite lateral, back", SkillCategory="Speed & Agility", Sport="Football", Difficulty=3, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="5-10-5 Drill", Description="Change direction and acceleration", SkillCategory="Speed & Agility", Sport="Football", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
            };
        }

        // ─── SOCCER ──────────────────────────────────────────────────────
        private static List<Drill> GetSoccerDrills()
        {
            return new List<Drill>
            {
                // DRIBBLING
                new Drill { Id=Guid.NewGuid(), Name="Close Control Dribble", Description="Small touches maintaining tight possession", SkillCategory="Dribbling", Sport="Soccer", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Speed Dribble", Description="Push ball forward in open space", SkillCategory="Dribbling", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Directional Change", Description="Quick cut around cone or defender", SkillCategory="Dribbling", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Inside Foot Dribble", Description="Control ball using inside of foot", SkillCategory="Dribbling", Sport="Soccer", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Outside Foot Dribble", Description="Control using outside of foot only", SkillCategory="Dribbling", Sport="Soccer", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Sole of Foot Roll", Description="Dribble with bottom of shoe", SkillCategory="Dribbling", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Weaving Dribble", Description="Navigate through cones in figure-8", SkillCategory="Dribbling", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="One-Touch Dribble", Description="Move ball with single touch between runs", SkillCategory="Dribbling", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Acceleration Burst", Description="Quick dribble followed by speed burst", SkillCategory="Dribbling", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Deceleration Control", Description="Slow ball quickly to beat defender", SkillCategory="Dribbling", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                // PASSING
                new Drill { Id=Guid.NewGuid(), Name="Short Pass Accuracy", Description="5-10 yard passes to feet", SkillCategory="Passing", Sport="Soccer", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Long Ball Distribution", Description="30+ yard passes over distance", SkillCategory="Passing", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Ground Pass", Description="Roll pass along ground to teammate", SkillCategory="Passing", Sport="Soccer", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lofted Pass", Description="Arc pass over defenders' heads", SkillCategory="Passing", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Through Ball", Description="Pass between defenders to teammate", SkillCategory="Passing", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Square Pass", Description="Pass sideways to maintain formation", SkillCategory="Passing", Sport="Soccer", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Diagonal Pass", Description="Pass at 45-degree angle", SkillCategory="Passing", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="One-Touch Pass", Description="Receive and immediately pass", SkillCategory="Passing", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="First-Time Pass", Description="Quick pass without trapping ball", SkillCategory="Passing", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Weighted Pass", Description="Adjust pass speed to teammate's run", SkillCategory="Passing", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                // SHOOTING
                new Drill { Id=Guid.NewGuid(), Name="Inside-of-Foot Shot", Description="Accurate shooting with inside foot", SkillCategory="Shooting", Sport="Soccer", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Laces Shot", Description="Powerful shot hitting ball with shoelaces", SkillCategory="Shooting", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Outside-of-Foot Curl", Description="Shot that curves around goalkeeper", SkillCategory="Shooting", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Low Drive", Description="Shot along ground toward goal", SkillCategory="Shooting", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="High Shot", Description="Shot aimed high at upper corner", SkillCategory="Shooting", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Close-Range Finish", Description="Shot from 6-yard box", SkillCategory="Shooting", Sport="Soccer", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Long-Distance Shot", Description="Shot from 25+ yards out", SkillCategory="Shooting", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="First-Time Shot", Description="Shoot immediately upon receiving ball", SkillCategory="Shooting", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Volley Shot", Description="Shoot ball out of air before bouncing", SkillCategory="Shooting", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Chip Shot", Description="Lofted shot over goalkeeper's head", SkillCategory="Shooting", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                // DEFENSE
                new Drill { Id=Guid.NewGuid(), Name="Jockey Defending", Description="Stay in front of opponent while moving", SkillCategory="Defense", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tackle Timing", Description="Win ball with perfectly timed slide tackle", SkillCategory="Defense", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Clearance Header", Description="Head ball away from goal", SkillCategory="Defense", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Interception Positioning", Description="Read play and step in for ball", SkillCategory="Defense", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Pressing Drill", Description="Quickly close down opponent with ball", SkillCategory="Defense", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Covering", Description="Drop back to defend teammate's space", SkillCategory="Defense", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Offside Trap", Description="Maintain defensive line", SkillCategory="Defense", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Marking Assignment", Description="Stay with assigned opponent", SkillCategory="Defense", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Recover Speed", Description="Chase back quickly after losing ball", SkillCategory="Defense", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Goalkeeper Angle", Description="Adjust body position to block shots", SkillCategory="Defense", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                // FIRST TOUCH
                new Drill { Id=Guid.NewGuid(), Name="Sole of Foot Control", Description="Stop ball dead with shoe bottom", SkillCategory="First Touch", Sport="Soccer", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Inside Foot Trap", Description="Control ball with inside of foot", SkillCategory="First Touch", Sport="Soccer", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Outside Foot Control", Description="Receive on outside of foot", SkillCategory="First Touch", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Chest Control", Description="Control with chest and bring to feet", SkillCategory="First Touch", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Thigh Control", Description="Use thigh to receive and control", SkillCategory="First Touch", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Head Control", Description="Head ball down to feet", SkillCategory="First Touch", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="One-Touch Control", Description="Take one touch while moving", SkillCategory="First Touch", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Directional Touch", Description="First touch away from defenders", SkillCategory="First Touch", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Soft Reception", Description="Cushion ball for perfect control", SkillCategory="First Touch", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Ball Mastery", Description="Complete control in tight space", SkillCategory="First Touch", Sport="Soccer", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                // SPEED & AGILITY
                new Drill { Id=Guid.NewGuid(), Name="Cone Weaving", Description="Dribble through cones quickly", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Ladder Footwork", Description="Rapid foot placement through agility ladder", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Shuttle Run", Description="Sprint back and forth between lines", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lateral Shuffle", Description="Quick side-to-side movement", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Jump Rope", Description="Rope skipping for coordination", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Change of Pace", Description="Alternate fast and slow movement", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Acceleration Burst", Description="Quick explosive sprint", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Box Drill", Description="Run perimeter of box at speed", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Zigzag Sprint", Description="Run diagonal lines at high speed", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Reaction Sprint", Description="React to coach's signal with sprint", SkillCategory="Speed & Agility", Sport="Soccer", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
            };
        }                   // ─── HOCKEY ──────────────────────────────────────────────────────
        private static List<Drill> GetHockeyDrills()
        {
            return new List<Drill>
            {
                // SHOOTING
                new Drill { Id=Guid.NewGuid(), Name="Wrist Shot Reps", Description="Quick-release wrist shots to corners", SkillCategory="Shooting", Sport="Hockey", Difficulty=1, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Snap Shot", Description="Rapid snap release off the blade", SkillCategory="Shooting", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Slap Shot Power", Description="Full wind-up shot for maximum power", SkillCategory="Shooting", Sport="Hockey", Difficulty=2, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="One-Timer", Description="Shoot pass directly without stopping puck", SkillCategory="Shooting", Sport="Hockey", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backhand Shot", Description="Elevate puck off the backhand", SkillCategory="Shooting", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Shot in Stride", Description="Release shot without breaking skating stride", SkillCategory="Shooting", Sport="Hockey", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tip-In Deflections", Description="Redirect point shots in front of net", SkillCategory="Shooting", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Five-Hole Targeting", Description="Shoot between goalie's pads", SkillCategory="Shooting", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Top-Shelf Accuracy", Description="Pick upper corners over the glove", SkillCategory="Shooting", Sport="Hockey", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Rebound Finish", Description="Bury loose pucks around the crease", SkillCategory="Shooting", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                // STICK HANDLING
                new Drill { Id=Guid.NewGuid(), Name="Figure-8 Puck Control", Description="Handle puck in figure-8 around cones", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Toe Drag Series", Description="Pull puck across body with toe of blade", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=3, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Quick Hands Box", Description="Rapid side-to-side puck movement in tight box", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Puck Protection", Description="Shield puck from defender with body", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Head-Up Handling", Description="Stickhandle while scanning ice, not puck", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Obstacle Weave", Description="Handle puck through cone maze at speed", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backhand-Forehand Rolls", Description="Rapid blade rolls side to side", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="One-Hand Control", Description="Control puck with top hand only", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=3, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Deke Sequence", Description="Chain fakes to beat defender one-on-one", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Stationary Rapid Fire", Description="Max-speed stickhandling in place", SkillCategory="Stick Handling", Sport="Hockey", Difficulty=1, Duration=10, DateCreated=DateTime.Now },
                // SKATING
                new Drill { Id=Guid.NewGuid(), Name="Crossover Circuits", Description="Powerful crossovers around circles", SkillCategory="Skating", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Edge Work Ladder", Description="Inside and outside edge control drills", SkillCategory="Skating", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backward Skating", Description="Speed and control skating backward", SkillCategory="Skating", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Tight Turns", Description="Full-speed turns in small radius", SkillCategory="Skating", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Quick Starts", Description="Explosive first three strides", SkillCategory="Skating", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Hockey Stops", Description="Hard stops both directions", SkillCategory="Skating", Sport="Hockey", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Transition Pivots", Description="Forward-to-backward pivots at speed", SkillCategory="Skating", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Stride Lengthening", Description="Full extension power strides", SkillCategory="Skating", Sport="Hockey", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lateral Mohawks", Description="Open-hip lateral skating movement", SkillCategory="Skating", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Blue Line Sprints", Description="Repeated line-to-line speed bursts", SkillCategory="Skating", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                // PASSING
                new Drill { Id=Guid.NewGuid(), Name="Tape-to-Tape Pass", Description="Crisp flat passes to teammate's blade", SkillCategory="Passing", Sport="Hockey", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Saucer Pass Targets", Description="Elevated pass over sticks landing flat", SkillCategory="Passing", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Give-and-Go Reps", Description="Pass, skate to space, receive return", SkillCategory="Passing", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backhand Pass", Description="Accurate passes off the backhand", SkillCategory="Passing", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Breakout Pass", Description="First pass out of defensive zone", SkillCategory="Passing", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Stretch Pass", Description="Long pass to streaking forward", SkillCategory="Passing", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="One-Touch Passing", Description="Redirect pass immediately without settling", SkillCategory="Passing", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Moving Target Pass", Description="Lead a skating teammate accurately", SkillCategory="Passing", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Board Bank Pass", Description="Use boards to pass around defender", SkillCategory="Passing", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Cross-Ice Feed", Description="Hard pass across the slot for one-timer", SkillCategory="Passing", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                // DEFENSE
                new Drill { Id=Guid.NewGuid(), Name="Gap Control", Description="Maintain proper distance on rushing forward", SkillCategory="Defense", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Stick Lift", Description="Lift opponent's stick to steal puck", SkillCategory="Defense", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Poke Check Timing", Description="Jab puck away without lunging", SkillCategory="Defense", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Body Positioning", Description="Angle attacker to outside lane", SkillCategory="Defense", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Shot Blocking", Description="Safe technique to block shooting lanes", SkillCategory="Defense", Sport="Hockey", Difficulty=3, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Net-Front Battle", Description="Clear opponents from front of crease", SkillCategory="Defense", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Backcheck Sprint", Description="Sprint back to disrupt the rush", SkillCategory="Defense", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Corner Containment", Description="Pin puck carrier along the boards", SkillCategory="Defense", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Zone Coverage", Description="Rotate and cover in defensive zone", SkillCategory="Defense", Sport="Hockey", Difficulty=3, Duration=25, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Breakout Support", Description="Support puck retrieval under pressure", SkillCategory="Defense", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                // SPEED & AGILITY
                new Drill { Id=Guid.NewGuid(), Name="Line-to-Line Sprints", Description="Max-speed skating between lines", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Stop-and-Start Bursts", Description="Hard stop then explosive restart", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Cone Agility Weave", Description="Skate tight pattern through cones", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Ladder Footwork (Off-Ice)", Description="Rapid feet through agility ladder", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Jump Rope (Off-Ice)", Description="Rope skipping for foot speed", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=1, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Box Jumps (Off-Ice)", Description="Explosive lower-body power", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Reaction Starts", Description="Sprint on coach's whistle from stillness", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Circle Acceleration", Description="Build to top speed around face-off circles", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=2, Duration=20, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Lateral Bounds (Off-Ice)", Description="Side-to-side explosive jumps", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=2, Duration=15, DateCreated=DateTime.Now },
                new Drill { Id=Guid.NewGuid(), Name="Zigzag Transition", Description="Diagonal skating with pivots at each cone", SkillCategory="Speed & Agility", Sport="Hockey", Difficulty=3, Duration=20, DateCreated=DateTime.Now },
            };
        }

    };
}        
    

