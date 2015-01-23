using CommandLine;
using System;

namespace BillyCMD
{
    static class Env
    {
        public static void Exit()
        {
            Environment.Exit(Parser.DefaultExitCodeFail);
        }
    }
}