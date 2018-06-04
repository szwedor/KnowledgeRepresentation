using System;
using System.Collections.Generic;
using System.Text;

namespace Stories.Tests
{
    public static class AgentInData
    {
        public static string mechanicstory=@"initially JestPaliwo and not Zepsuty
when Rozwazny Kieruje causes not JestPaliwo if not Zepsuty
impossible Kieruje if Zepsuty
impossible Mechanik Kieruje
when Mechanik Naprawia typically causes not Zepsuty if Zepsuty
observable Zepsuty after Nierozwazny Kieruje
when Nierozwazny Kieruje causes not JestPaliwo if not Zepsuty
JestPaliwo after Rozwazny Tankuje
JestPaliwo after Nierozwazny Tankuje";
        public static readonly List<object[]> Data = new List<object[]>
        {
            new object[] { @"necessary John in cut",
                @"initially AdamAlive
                  when John cry causes cried
                  when Izaak moo causes mow
                  impossible Adam cut
                  cut causes not Tree if Tree", false },
            new object[] { @"necessary John in cut",
                @"initially AdamAlive
                  when John cry causes cried
                  impossible Adam cut
                  cut causes not Tree if Tree", true },
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
            new object[]{"possibly Rozwazny in Mechanik Naprawia",mechanicstory,false
            },
            new object[]{ "possibly Mechanik in Nierozwazny Kieruje",mechanicstory,false
            },
            new object[]{ "possibly Mechanik in Kieruje",mechanicstory,false
            },
            new object[]{"possibly Rozwazny in Rozwazny Kieruje",mechanicstory,true
            },
            new object[]{ "necessary Rozwazny in Rozwazny Kieruje",mechanicstory,true
            },
            new object[]{ "necessary Rozwazny in Nierozwazny Kieruje",mechanicstory,false
            },
            new object[]
            {
                @"necessary John in unload, load",
                @"unload causes not loaded
             load causes loaded
            impossible load if loaded",true
            },
            new object[]
            {
                @"necessary John in unload, load, load",
                @"unload causes not loaded
             load causes loaded
            impossible load if loaded",false
            },
            new object[]
            {
                @"necessary John in unload, load, load",
                @"unload causes not loaded
             load typically causes loaded
            impossible load if loaded",false
            },
            new object[]
            {
                @"possibly John in unload, load",
                @"unload causes not loaded
             load causes loaded
            impossible load if loaded",true
            },
            new object[]
            {
                @"possibly John in unload, load, load",
                @"unload causes not loaded
             load causes loaded
            impossible load if loaded",false
            },
            new object[]
            {
                @"possibly John in unload, load, load",
                @"unload causes not loaded
             load typically causes loaded
            impossible load if loaded",true
            },
            new object[]
            {
                @"possibly John in unload, load, load, load",
                @"unload causes not loaded
             load causes loaded
            impossible load if loaded",false
            },
            new object[]
            {
                @"possibly John in unload, load, load, load",
                @"unload causes not loaded
             load typically causes loaded
            impossible load if loaded",true
            },
            new object[]
            {
                @"necessary John in unload, load, load, load",
                @"unload causes not loaded
             load causes loaded
            impossible load if loaded",false
            },
            new object[]
            {
                @"necessary John in unload, load, load, load",
                @"unload causes not loaded
             load typically causes loaded
            impossible load if loaded",false
            },
        };
        }
}