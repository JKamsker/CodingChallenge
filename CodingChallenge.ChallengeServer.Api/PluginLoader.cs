using CodingChallenge.ChallengeServer.Abstractions;

using dnlib.DotNet;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

public class PluginLoader
{
    private readonly DirectoryInfo _pluginDirectory;

    public PluginLoader(DirectoryInfo pluginDirectory)
    {
        _pluginDirectory = pluginDirectory;
    }

    //public void LoadPlugins(IServiceCollection services)
    //{
    //    _pluginDirectory.Create();

    //    var ctx = new AssemblyLoadContext("PluginCtx", true);

    //    var dlls = _pluginDirectory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);
    //    foreach (var dll in dlls)
    //    {
    //        var assembly = ctx.LoadFromAssemblyPath(dll.FullName);
    //        var classes = assembly.GetTypes()
    //            .Where(x => x.IsAssignableTo(typeof(IChallengeService)))
    //            .ToList();

    //        if (!classes.Any())
    //        {
    //            ctx.Unload();
    //        }
    //    }
    //}

    public void LoadPlugins(IServiceCollection services)
    {
        _pluginDirectory.Create();
        //var ctx = new AssemblyLoadContext("PluginCtx", true);

        foreach (var dll in FindFilesContainingPluginFile())
        {
            var assembly = Assembly.LoadFile(dll.FullName);
            var types = GetTypesContainingInterface(assembly);
            foreach (var type in types)
            {
                services.AddTransient(typeof(IChallengeService), type);
            }
            //Debugger.Break();
        }
    }

    private IEnumerable<FileInfo> FindFilesContainingPluginFile()
    {
        var ctx = new AssemblyLoadContext("PluginCtx", true);
        var dlls = _pluginDirectory
            .EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly)
            .Where(x => x.Name != "CodingChallenge.ChallengeServer.Abstractions.dll")
            ;

        foreach (var dll in dlls)
        {
            Console.WriteLine($"Analyzing '{dll}'");
            var mod = ctx.LoadFromAssemblyPath(dll.FullName);

            var containsInterface = GetTypesContainingInterface(mod).Any();

            if (containsInterface)
            {
                Console.WriteLine($"'{dll}' contains the interface");

                yield return dll;
            }
        }

        ctx.Unload();

        //    private const string InterfaceToImplement = "CodingChallenge.ChallengeServer.Abstractions.IChallengeService";
        //var modCtx = ModuleDef.CreateModuleContext();
        //var dlls = _pluginDirectory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);
        //foreach (var dll in dlls)
        //{
        //    var mod = ModuleDefMD.Load(dll.FullName, modCtx);
        //    var containsInterface = mod.Types
        //        .Any(t => t.Interfaces.Any(m => m.Interface.FullName == InterfaceToImplement));

        //    if (containsInterface)
        //    {
        //        yield return dll;
        //    }
        //}
    }

    private static IEnumerable<Type> GetTypesContainingInterface(Assembly mod)
    {
        return mod.GetTypes()
                        .Where(x => x.IsAssignableTo(typeof(IChallengeService)));
    }
}