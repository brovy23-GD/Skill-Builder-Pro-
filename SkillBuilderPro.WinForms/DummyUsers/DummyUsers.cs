using SkillBuilderPro.WinForms.Models;
using System;
using System.Collections.Generic;

namespace SkillBuilderPro.WinForms
{
    public static class DummyUsers
    {
        public static List<User> GetAllDummyUsers()
        {
            return new List<User>
            {
                new User
                {
                    FullName = "Michael Jordan",
                    Email = "michael.jordan@nba.com",
                    Role = "Athlete",
                    Sport = "Basketball",
                    TargetArea = "Defensive Intensity & Clutch Shooting",
                    ExperienceLevel = "Advanced",
                    Age = 63,
                    Phone = "000-000-0000",
                    Height = 6.6,
                    Weight = 216,
                    Team = "Chicago Bulls",
                    Bio = "Basketball legend known for clutch performance."
                },

                new User
                {
                    FullName = "Kobe Bryant",
                    Email = "kobe.bryant@nba.com",
                    Role = "Athlete",
                    Sport = "Basketball",
                    TargetArea = "Clutch Shooting & Footwork",
                    ExperienceLevel = "Advanced",
                    Age = 41,
                    Phone = "000-000-0000",
                    Height = 6.6,
                    Weight = 212,
                    Team = "Los Angeles Lakers",
                    Bio = "One of the greatest competitors in NBA history."
                },

                new User
                {
                    FullName = "Cristiano Ronaldo",
                    Email = "cristiano.ronaldo@soccer.com",
                    Role = "Athlete",
                    Sport = "Soccer",
                    TargetArea = "Ball Control & Finishing",
                    ExperienceLevel = "Advanced",
                    Age = 39,
                    Phone = "000-000-0000",
                    Height = 6.2,
                    Weight = 183,
                    Team = "Portugal National Team",
                    Bio = "Soccer icon known for elite athleticism."
                },

                new User
                {
                    FullName = "Allie Rovy",
                    Email = "allie.rovy@softball.com",
                    Role = "Athlete",
                    Sport = "Softball",
                    TargetArea = "Outfield: Drop Step + Over-The-Shoulder Catch",
                    ExperienceLevel = "Intermediate",
                    Age = 14,
                    Phone = "000-000-0000",
                    Height = 5.4,
                    Weight = 110,
                    Team = "Oak Lawn Softball",
                    Bio = "Strong outfielder with great instincts."
                },

                new User
                {
                    FullName = "Aubrey Rovy",
                    Email = "aubrey.rovy@softball.com",
                    Role = "Athlete",
                    Sport = "Softball",
                    TargetArea = "Softball Hitting: Weight Load + Explosive Hip Rotation",
                    ExperienceLevel = "Intermediate",
                    Age = 16,
                    Phone = "000-000-0000",
                    Height = 5.5,
                    Weight = 115,
                    Team = "Oak Lawn Softball",
                    Bio = "Power hitter with excellent mechanics."
                },

                new User
                {
                    FullName = "Kaleigh Rovy",
                    Email = "kaleigh.rovy@softball.com",
                    Role = "Athlete",
                    Sport = "Softball",
                    TargetArea = "Softball Infield: Ground Ball Footwork + Quick Release",
                    ExperienceLevel = "Beginner",
                    Age = 12,
                    Phone = "000-000-0000",
                    Height = 5.0,
                    Weight = 95,
                    Team = "Oak Lawn Softball",
                    Bio = "Developing infielder with great potential."
                },

                new User
                {
                    FullName = "Kevin Hart",
                    Email = "kevin.hart@baseball.com",
                    Role = "Athlete",
                    Sport = "Baseball",
                    TargetArea = "Bunting: Pivot + Zone Control",
                    ExperienceLevel = "Beginner",
                    Age = 45,
                    Phone = "000-000-0000",
                    Height = 5.4,
                    Weight = 140,
                    Team = "Beginner Baseball",
                    Bio = "Comedian learning baseball fundamentals."
                },

                new User
                {
                    FullName = "Elon Musk",
                    Email = "elon.musk@football.com",
                    Role = "Athlete",
                    Sport = "Football",
                    TargetArea = "Conditioning: Bear Crawl + Fumble Recovery",
                    ExperienceLevel = "Beginner",
                    Age = 52,
                    Phone = "000-000-0000",
                    Height = 6.2,
                    Weight = 190,
                    Team = "Beginner Football",
                    Bio = "Tech entrepreneur exploring football training."
                },

                new User
                {
                    FullName = "Ariana Grande",
                    Email = "ariana.grande@soccer.com",
                    Role = "Athlete",
                    Sport = "Soccer",
                    TargetArea = "Agility: Ladder Drill + Burst Acceleration",
                    ExperienceLevel = "Beginner",
                    Age = 31,
                    Phone = "000-000-0000",
                    Height = 5.3,
                    Weight = 105,
                    Team = "Beginner Soccer",
                    Bio = "Singer learning soccer agility drills."
                },

                new User
                {
                    FullName = "Bill Gates",
                    Email = "bill.gates@baseball.com",
                    Role = "Athlete",
                    Sport = "Baseball",
                    TargetArea = "Agility: Ickey Shuffle + Reaction Start",
                    ExperienceLevel = "Beginner",
                    Age = 68,
                    Phone = "000-000-0000",
                    Height = 5.10,
                    Weight = 170,
                    Team = "Beginner Baseball",
                    Bio = "Tech billionaire learning baseball footwork."
                }
            };
        }

        public static User GetDummyUserByIndex(int index)
        {
            var users = GetAllDummyUsers();
            return users[index];
        }
    }
}

