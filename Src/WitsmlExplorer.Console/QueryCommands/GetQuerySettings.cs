using System.ComponentModel;
using System.IO;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Cli;

namespace WitsmlExplorer.Console.QueryCommands
{
    public class GetQuerySettings : CommandSettings
    {
        private readonly string[] validReturnElements = {"requested", "all", "id-only", "header-only", "data-only", "station-location-only", "latest-change-only"};

        [CommandArgument(0, "<PATH_TO_QUERY_FILE>")]
        [Description("Path to query file in XML format")]
        public string QueryFile { get; init; }

        [CommandOption("--returnElements")]
        [Description("Indicates which elements and attributes are requested to be returned in addition to data-object selection items (requested(default)|all|id-only|header-only|data-only|station-location-only|latest-change-only)")]
        [DefaultValue("requested")]
        public string ReturnElements { get; init; }

        [CommandOption("--maxReturnNodes")]
        [Description("Max number of data nodes to return. Must be a whole number greater than zero, if provided")]
        [DefaultValue("")]
        public int? MaxReturnNodes { get; init; }

        public override ValidationResult Validate()
        {
            var queryPath = Path.Combine(Directory.GetCurrentDirectory(), QueryFile);
            if (!File.Exists(queryPath))
                return ValidationResult.Error($"Could not find query file: {queryPath}");

            if (!validReturnElements.Contains(ReturnElements))
                return ValidationResult.Error($"Invalid value for returnElements ({ReturnElements})");

            if (MaxReturnNodes is < 1)
                return ValidationResult.Error("MaxReturnNodes must be a whole number greater than zero");

            return ValidationResult.Success();
        }
    }
}
