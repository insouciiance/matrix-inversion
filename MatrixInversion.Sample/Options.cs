using CommandLine;

namespace MatrixInversion.Sample;

public class Options
{
    [Option("inverser", Required = true)]
    public string Inverser { get; set; } = null!;

    [Option("proc-count", Required = false, Default = 12)]
    public int ProcessorsCount { get; set; }

    [Option("runs-count", Required = false, Default = 5)]
    public int RunsCount { get; set; }


    [Option("size", Required = true)]
    public int Size { get; set; }

    [Option("strategy", Required = true)]
    public string GenerationStrategy { get; set; } = null!;

    [Option("min", Required = false, Default = -50)]
    public int Min { get; set; }

    [Option("max", Required = false, Default = 50)]
    public int Max { get; set; }


    [Option("dump-matrix", Default = false)]
    public bool DumpMatrix { get; set; }

    [Option("dump-result", Default = false)]
    public bool DumpResult { get; set; }
}
