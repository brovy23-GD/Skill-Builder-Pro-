using SkillBuilderPro.WinForms.Models;
using System.Collections.Generic;
using System.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
                    Email = "michael.jordan@basketball.com",
                    Password = "",
                    Phone = "000-000-0023",
                    Sport = "Basketball",
                    TargetArea = "Shooting",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 35,
                    Height = 6.6,
                    Weight = 216,
                    Team = "Chicago Bulls",
                    Bio = "The G.O.A.T. Six rings, six Finals MVPs, zero Game 7s needed. The standard every competitor is measured against.",
                    JerseyNumber = 23,
                    Goal = "Maintain championship form"
                },

                new User
                {
                    FullName = "Kobe Bryant",
                    Email = "kobe.bryant@basketball.com",
                    Password = "",
                    Phone = "000-000-0024",
                    Sport = "Basketball",
                    TargetArea = "Footwork",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 32,
                    Height = 6.6,
                    Weight = 212,
                    Team = "Los Angeles Lakers",
                    Bio = "Mamba Mentality personified. 4 a.m. workouts, surgical footwork, and a killer instinct that never blinked.",
                    JerseyNumber = 24,
                    Goal = "Master complete offensive control"
                },

                new User
                {
                    FullName = "Babe Ruth",
                    Email = "babe.ruth@baseball.com",
                    Password = "",
                    Phone = "000-000-0003",
                    Sport = "Baseball",
                    TargetArea = "Hitting",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 39,
                    Height = 6.2,
                    Weight = 215,
                    Team = "New York Yankees",
                    Bio = "The Sultan of Swat. Called his shot and cashed it. Rewrote the record book and changed the game forever.",
                    JerseyNumber = 3,
                    Goal = "Drive the ball with power"
                },

                new User
                {
                    FullName = "Ken Griffey Jr.",
                    Email = "ken.griffeyjr@baseball.com",
                    Password = "",
                    Phone = "000-000-0024",
                    Sport = "Baseball",
                    TargetArea = "Hitting",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 35,
                    Height = 6.3,
                    Weight = 195,
                    Team = "Seattle Mariners",
                    Bio = "The Kid. The prettiest left-handed swing baseball has ever seen, 630 home runs, and a backwards hat full of Gold Gloves.",
                    JerseyNumber = 24,
                    Goal = "Drive the ball with power and confidence"
                },

                new User
                {
                    FullName = "Aubrey Rovy",
                    Email = "aubrey.rovy@softball.com",
                    Password = "",
                    Phone = "000-000-0000",
                    Sport = "Softball",
                    TargetArea = "Hitting",
                    ExperienceLevel = "Intermediate",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 13,
                    Height = 5.6,
                    Weight = 125,
                    Team = "Oak Lawn",
                    Bio = "Power at the plate, precision in her mechanics. When she squares one up, outfielders just turn and watch.",
                    JerseyNumber = 3,
                    Goal = "Improve hitting consistency"
                },

                new User
                {
                    FullName = "Allie Rovy",
                    Email = "allie.rovy@softball.com",
                    Password = "",
                    Phone = "000-000-0001",
                    Sport = "Softball",
                    TargetArea = "Fielding",
                    ExperienceLevel = "Intermediate",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 14,
                    Height = 5.6,
                    Weight = 125,
                    Team = "Oak Lawn Juniors",
                    Bio = "Sweet swing, never off the bases, and nothing drops in her outfield. Pure athlete with a Gold Glove ceiling.",
                    JerseyNumber = 2,
                    Goal = "Increase power"
                },

                new User
                {
                    FullName = "Tom Brady",
                    Email = "tom.brady@football.com",
                    Password = "",
                    Phone = "000-000-0012",
                    Sport = "Football",
                    TargetArea = "Passing",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 35,
                    Height = 6.4,
                    Weight = 225,
                    Team = "New England Patriots",
                    Bio = "Seven rings. Pick 199 turned greatest of all time through preparation, precision, and ice in the fourth quarter.",
                    JerseyNumber = 12,
                    Goal = "Increase passing accuracy"
                },

                new User
                {
                    FullName = "Walter Payton",
                    Email = "walter.payton@football.com",
                    GuardianEmail = "peter.griffin@quahog.com",
                    Password = "",
                    Phone = "000-000-0034",
                    Sport = "Football",
                    TargetArea = "Conditioning",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 31,
                    Height = 5.10,
                    Weight = 200,
                    Team = "Chicago Bears",
                    Bio = "Sweetness. Never ran out of bounds, never missed the hill workout, never gave a defender a clean shot.",
                    JerseyNumber = 34,
                    Goal = "Build competitive conditioning"
                },

                new User
                {
                    FullName = "Lionel Messi",
                    Email = "lionel.messi@soccer.com",
                    GuardianEmail = "marge.simpson@springfield.com",
                    Password = "",
                    Phone = "000-000-0010",
                    Sport = "Soccer",
                    TargetArea = "Dribbling",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 34,
                    Height = 5.7,
                    Weight = 148,
                    Team = "Argentina",
                    Bio = "Eight Ballon d'Ors and a World Cup. The ball obeys him like it's on a string — vision, touch, and finishing from another planet.",
                    JerseyNumber = 10,
                    Goal = "Create and finish more attacking chances"
                },

                new User
                {
                    FullName = "Neymar Jr.",
                    Email = "neymar.jr@soccer.com",
                    Password = "",
                    Phone = "000-000-0011",
                    Sport = "Soccer",
                    TargetArea = "Footwork",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 31,
                    Height = 5.9,
                    Weight = 150,
                    Team = "Brazil",
                    Bio = "Joga bonito in human form. Feet faster than the defender's thoughts and the audacity to try what nobody else would.",
                    JerseyNumber = 11,
                    Goal = "Improve quickness and creativity"
                },

                new User
                {
                    FullName = "Connor Bedard",
                    Email = "connor.bedard@hockey.com",
                    Password = "",
                    Phone = "000-000-0098",
                    Sport = "Hockey",
                    TargetArea = "Shooting",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 20,
                    Height = 5.10,
                    Weight = 190,
                    Team = "Chicago Blackhawks",
                    Bio = "The face of the franchise. A release so quick goalies are beat before they flinch — generational shooter, future of Chicago hockey.",
                    JerseyNumber = 98,
                    Goal = "Improve shot power and scoring touch"
                },

                new User
                {
                    FullName = "Wayne Gretzky",
                    Email = "wayne.gretzky@hockey.com",
                    Password = "",
                    Phone = "000-000-0099",
                    Sport = "Hockey",
                    TargetArea = "Passing",
                    ExperienceLevel = "Advanced",
                    Role = "Athlete",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 30,
                    Height = 6.0,
                    Weight = 185,
                    Team = "Edmonton Oilers",
                    Bio = "The Great One. Skated to where the puck was going. Owns more assists than anyone else has points.",
                    JerseyNumber = 99,
                    Goal = "Create more scoring opportunities"
                },


                    new User
                {
                    FullName = "Bobby Rovy",
                    Email = "bobby.rovy@skillbuilderpro.com",
                    Password = "",
                    Phone = "000-000-0001",
                    Sport = "Baskeball",
                    TargetArea = "Dribbling",
                    ExperienceLevel = "Advanced",
                    Role = "Admin",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 40,
                    Height = 6.0,
                    Weight = 190,
                    Team = "SkillBuilderPro",
                    Bio = "Built this thing. Full system access — users, drills, rosters, and everything under the hood.",
                    JerseyNumber = 1,
                    Goal = "Keep the platform running"
                },

                new User
                {
                    FullName = "Peter Griffin",
                    Email = "peter.griffin@quahog.com",
                    Password = "",
                    Phone = "000-000-0042",
                    Sport = "Football",
                    TargetArea = "Conditioning",
                    ExperienceLevel = "Beginner",
                    Role = "Parent",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 43,
                    Height = 5.9,
                    Weight = 270,
                    Team = "Quahog",
                    Bio = "Loudest guy in the bleachers. Questionable coaching advice, unquestionable attendance.",
                    JerseyNumber = 0,
                    Goal = "Follow my athlete's progress"
                },

                new User
                {
                    FullName = "Phil Jackson",
                    Email = "phil.jackson@basketball.com",
                    Password = "",
                    Phone = "000-000-0011",
                    Sport = "Basketball",
                    TargetArea = "Shooting",
                    ExperienceLevel = "Advanced",
                    Role = "Coach",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 55,
                    Height = 6.8,
                    Weight = 220,
                    Team = "Chicago Bulls",
                    Bio = "Eleven rings as a head coach. The Zen Master ran the triangle and managed egos nobody else could.",
                    JerseyNumber = 0,
                    Goal = "Develop the roster"
                },

                new User
                {
                    FullName = "Mike Ditka",
                    Email = "mike.ditka@football.com",
                    Password = "",
                    Phone = "000-000-0089",
                    Sport = "Football",
                    TargetArea = "Conditioning",
                    ExperienceLevel = "Advanced",
                    Role = "Coach",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 46,
                    Height = 6.3,
                    Weight = 230,
                    Team = "Chicago Bears",
                    Bio = "Da Coach. '85 Bears. Toughness wasn't a drill, it was the whole program.",
                    JerseyNumber = 0,
                    Goal = "Build a championship roster"
                },

                new User
                {
                    FullName = "Jennie Finch",
                    Email = "jennie.finch@softball.com",
                    Password = "",
                    Phone = "000-000-0027",
                    Sport = "Softball",
                    TargetArea = "Hitting",
                    ExperienceLevel = "Advanced",
                    Role = "Coach",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 38,
                    Height = 6.1,
                    Weight = 165,
                    Team = "Oak Lawn",
                    Bio = "Olympic gold medalist and one of the most dominant arms softball has produced. Runs the cage like a machine shop.",
                    JerseyNumber = 27,
                    Goal = "Develop hitters"
                },

                new User
                {
                    FullName = "Marge Simpson",
                    Email = "marge.simpson@springfield.com",
                    Password = "",
                    Phone = "000-000-0044",
                    Sport = "Soccer",
                    TargetArea = "Dribbling",
                    ExperienceLevel = "Beginner",
                    Role = "Parent",
                    IsActive = true,
                    PhotoPath = "",
                    Age = 38,
                    Height = 8.0,
                    Weight = 140,
                    Team = "Springfield",
                    Bio = "Actually reads the practice schedule. Runs the snack rotation.",
                    JerseyNumber = 0,
                    Goal = "Follow my athlete's progress"
                }

            };
        }
    }
}