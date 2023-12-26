using AElf;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public static class ServiceSearcher
{
    public static IEnumerable<ServiceDescriptor> GetFullService(this ServiceDescriptor service)
    {
        var allDependedServices = new List<ServiceDescriptor>();
        var seen = new SortedSet<ServiceDescriptor>(new ServiceDescriptorComparer());
        DepthFirstSearch(service, ref allDependedServices, ref seen);
        var services = allDependedServices.ToDictionary(dependedService => dependedService.File.Name);
        var result = new List<ServiceDescriptor>();
        var bases = new List<string>();
        var seenBases = new SortedSet<string>();

        DepthFirstSearchForBase(service, ref bases, ref seenBases, services);
        foreach (var baseItem in bases)
        {
            var lastIndex = result.Count;
            result.Insert(lastIndex, services[baseItem]); //push to back of list
        }

        return result;
    }

    private static void DepthFirstSearchForBase(ServiceDescriptor service, ref List<string> list,
        ref SortedSet<string> seen, IReadOnlyDictionary<string, ServiceDescriptor> allServices)
    {
        if (!seen.Add(service.File.Name)) return;

        var baseCount = GetServiceBaseCount(service);
        // const FileDescriptor* file = service->file();
        // Add all dependencies.
        for (var i = 0; i < baseCount; i++)
        {
            var baseName = GetServiceBase(service, i);
            if (!allServices.ContainsKey(baseName))
                //TODO Make this an exception instead?
                Console.WriteLine($"Can't find specified base {baseName}, did you forget to import it?");
            var baseService = allServices[baseName];
            DepthFirstSearchForBase(baseService, ref list, ref seen, allServices);
        }

        // Add this file.
        list.Add(service.File.Name);
    }

    private static void DepthFirstSearch(ServiceDescriptor service, ref List<ServiceDescriptor> list,
        ref SortedSet<ServiceDescriptor> seen)
    {
        if (!seen.Add(service)) return;

        var file = service.File;

        foreach (var dependancy in file.Dependencies)
        {
            switch (dependancy.Services.Count)
            {
                case 0:
                    continue;
                case > 1:
                    Console.WriteLine($"{dependancy.Name}: File contains more than one service.");
                    break;
            }

            DepthFirstSearch(dependancy.Services[0], ref list, ref seen);
        }

        // Add this file.
        list.Add(service);
    }

    private static string GetServiceBase(ServiceDescriptor service, int index)
    {
        return service.GetOptions().GetExtension(OptionsExtensions.Base)[index];
    }

    private static int GetServiceBaseCount(ServiceDescriptor service)
    {
        if (service.GetOptions() == null) return 0;
        if (service.GetOptions().GetExtension(OptionsExtensions.Base) == null) return 0;
        return service.GetOptions().GetExtension(OptionsExtensions.Base).Count == 0
            ? 0
            : service.GetOptions().GetExtension(OptionsExtensions.Base).Count;
    }

    private class ServiceDescriptorComparer : IComparer<ServiceDescriptor>
    {
        public int Compare(ServiceDescriptor? x, ServiceDescriptor? y)
        {
            return string.Compare(x?.FullName, y?.FullName, StringComparison.Ordinal);
        }
    }
}
