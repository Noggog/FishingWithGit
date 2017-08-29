using FishingWithGit.CustomSetup;
using System;
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
                    System.Console.WriteLine($"BAKE Took {sw.ElapsedMilliseconds}ms");
                    System.Console.WriteLine($"BAKE Took {sw.ElapsedMilliseconds}ms");
                    System.Console.WriteLine($"BAKE Took {sw.ElapsedMilliseconds}ms");
                    //var pth = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine);
                    //var item = new FishingInstallerClass()
                    //{
                    //    TargetDir = @"C:\Program Files (x86)\FishingWithGit\\"
                    //};
                    //item.AddToPath();
                    //item.RemoveFromPath();
                    BaseWrapper wrapper = new BaseWrapper(args);
                    System.Console.WriteLine($"CTOR Took {sw.ElapsedMilliseconds}ms");
                    wrapper.Logger.AlwaysLog = true;
                    var result = await wrapper.Wrap(sw);
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
