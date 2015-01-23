using CommandLine;
using System;

namespace BillyCMD
{
    class Program
    {
        static void Main(string[] args)
        {
            var invokedVerb = "";
            object invokedVerbInstance = null;

            if (!Parser.Default.ParseArgumentsStrict(args, new Options(), (v, o) =>
            {
                invokedVerb = v;
                invokedVerbInstance = o;
            }))
            {
                Console.WriteLine("Couldn't parse input arguments");
                Env.Exit();
            }

            switch (invokedVerb)
            {
                case "schema":
                    Schemer.PrintSchema((Options.SchemaSuboptions)invokedVerbInstance);
                    break;
                case "build":
                    Builder.BuildDocs((Options.BuildSubOptions)invokedVerbInstance);
                    break;
                default:
                    Console.WriteLine("Unrecognized command " + invokedVerb);
                    Env.Exit();
                    break;
            }
        }
    }
}
