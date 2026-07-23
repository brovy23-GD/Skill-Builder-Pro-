namespace SkillBuilderPro.WinForms.Services
{
    public static class ApiDrillService
    {
        public static List<(string Title, string VideoUrl)> GetAllDrills()
        {
            return new List<(string Title, string VideoUrl)>
            {
                ("API Sprint Warmup", "https://youtube.com/..."),
                ("API Agility Ladder", "https://youtube.com/..."),
            };
        }
    }
}
