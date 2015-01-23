using BillyCMD.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.IO;

namespace BillyCMD
{
    static class Schemer
    {
        public static void PrintSchema(Options.SchemaSuboptions options)
        {
            Type type = null;

            switch (options.Type)
            {
                case "Contract":
                    type = typeof(Contract);
                    break;
                default:
                    Console.WriteLine("Unrecognized type " + options.Type);
                    Env.Exit();
                    break;
            }

            var jsonSchemaGenerator = new JsonSchemaGenerator();
            var schema = jsonSchemaGenerator.Generate(type);
            schema.Title = type.Name;

            var writer = new StringWriter();
            var jsonTextWriter = new JsonTextWriter(writer);
            schema.WriteTo(jsonTextWriter);

            dynamic parsedJson = JsonConvert.DeserializeObject(writer.ToString());
            var prettyString = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);

            Console.WriteLine(prettyString);
        }

    }
}