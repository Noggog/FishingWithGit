﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    BaseWrapper wrapper = new BaseWrapper(args);
                    wrapper.AlwaysLog = true;
                    var result = await wrapper.Wrap();
                    sw.Stop();
                    System.Console.WriteLine($"DONE OVERALL   Took {sw.ElapsedMilliseconds}ms");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            });
            System.Console.ReadLine();
        }
    }
}