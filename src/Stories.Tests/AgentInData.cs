using System;
using System.Collections.Generic;
using System.Text;

namespace Stories.Tests
{
    public static class AgentInData
    {
        public static readonly List<object[]> Data = new List<object[]>
        {

            new object[] { @"necessary John in cut",
                @"initially AdamAlive
                  when John cry causes cried
                  impossible Adam cut
                  cut causes not Tree if Tree", false },
            new object[] { @"necessary John in cut",
                @"initially AdamAlive
                  impossible Adam cut
                  cut causes not Tree if Tree", true },
            new object[] { @"necessary Adam in cut",
                @"initially AdamAlive
                  impossible Adam cut
                  cut causes not Tree if Tree", false },
            new object[] { @"necessary Adam in cut",
                @"initially AdamAlive
                  cut causes not Tree if Tree",true },
            new object[] { @"necessary Adam in cut",
                @"initially AdamAlive
                  when Adam cut causes not Tree if Tree",true },
            new object[] { @"necessary Adam in cut",//possible but no effect
                @"initially AdamAlive and not TreeUp 
                  when Adam cut causes not Tree if not Tree",true },
        };
        }
}