using Newtonsoft.Json;
using Questao2;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals =  GetTotalScoredGoals(teamName, year).GetAwaiter().GetResult(); ;

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = GetTotalScoredGoals(teamName, year).GetAwaiter().GetResult(); ;

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    //public static int getTotalScoredGoals(string team, int year)
    //{
        
    //    return 0;
    //}

    static async Task<int> GetTotalScoredGoals(string teamName, int year)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"https://jsonmock.hackerrank.com/api/football_matches?team1={teamName}&year={year}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            FootballMatchesData data = JsonConvert.DeserializeObject<FootballMatchesData>(responseBody);

            int totalGoals = 0;

            foreach (MatchData match in data.Data)
            {
                int team1Goals = int.Parse(match.Team1Goals);
                totalGoals += team1Goals;
            }

            return totalGoals;
        }
    }


}



