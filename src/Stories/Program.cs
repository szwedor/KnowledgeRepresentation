using Stories.Parser;

namespace Stories
{
    class Program
    {
        static void Main(string[] args)
        {
            var yaleShootingProblem = @"
            initially not loaded
            initially alive

            load causes loaded
            shoot causes not loaded
            when bob shoot causes not alive if loaded";
            //when jim shoot typically causes not alive if loaded";

            var history = Parsing.GetHistory(yaleShootingProblem);

            var story = new Story(history);
        }
    }
}
