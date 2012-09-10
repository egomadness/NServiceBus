using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NServiceBus.Hosting.Helpers;

namespace NServiceBus.Hosting.Tests
{
    [TestFixture, Category("Performance"), Ignore]
    public class ReflectionPerfTests
    {
        [Test]
        public void SubSetReflection()
        {
            var allDlls = GetAllDlls();
            var stopwatch1 = Stopwatch.StartNew();
            var assemblies = allDlls
                .Select(Assembly.LoadFrom)
                .ToList();
            stopwatch1.Stop();
            Debug.WriteLine(string.Format("Load Assemblies: {0}ms", stopwatch1.ElapsedMilliseconds));
            var stopwatch = Stopwatch.StartNew();
            var types1 = assemblies
                .AllTypesAssignableTo<IWantCustomInitialization>()
                .WhereConcrete()
                .ToList();
            stopwatch.Stop();
            Debug.WriteLine("Find implementations 1: " + stopwatch.ElapsedMilliseconds + "ms");
            stopwatch = Stopwatch.StartNew();
            var types2 = assemblies
                .AllTypesAssignableTo<IWantCustomInitialization>()
                .WhereConcrete()
                .ToList();
            stopwatch.Stop();
            Debug.WriteLine("Find implementations 2: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        [Test]
        public void SubSetReflectionExcludeNSB()
        {
            var files = GetAllDlls()
                .Where(x => !Path.GetFileName(x).StartsWith("NServiceBus.Core")).ToList();
            var stopwatch1 = Stopwatch.StartNew();
            var allAssemblies = files
                .Select(Assembly.LoadFrom)
                .ToList();
            stopwatch1.Stop();
            Debug.WriteLine(string.Format("Load Assemblies: {0}ms", stopwatch1.ElapsedMilliseconds));
            var assemblies = allAssemblies;
            var stopwatch = Stopwatch.StartNew();
            var types1 = assemblies
                .AllTypesAssignableTo<IWantCustomInitialization>()
                .WhereConcrete()
                .ToList();
            stopwatch.Stop();
            Debug.WriteLine("Find implementations 1: " + stopwatch.ElapsedMilliseconds + "ms");
            stopwatch = Stopwatch.StartNew();
            var types2 = assemblies
                .AllTypesAssignableTo<IWantCustomInitialization>()
                .WhereConcrete()
                .ToList();
            stopwatch.Stop();
            Debug.WriteLine("Find implementations 2: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        private static List<string> GetAllDlls()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var directoryName = Path.GetDirectoryName(path);
            return Directory.EnumerateFiles(directoryName, "*.dll").ToList();

        }

        [Test]
        public void AllReflection()
        {
            var allDlls = GetAllDlls();
            var stopwatch1 = Stopwatch.StartNew();
            var assemblies = allDlls
                .Select(Assembly.LoadFrom)
                .ToList();
            stopwatch1.Stop();
            Debug.WriteLine(string.Format("Load Assemblies: {0}ms", stopwatch1.ElapsedMilliseconds));
            var stopwatch = Stopwatch.StartNew();
            var types1 = (from a in assemblies
                          from t in a.GetTypes()
                          where
                              typeof (IWantCustomInitialization).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract
                          select t).ToList();
            stopwatch.Stop();
            Debug.WriteLine("Find implementations 1: " + stopwatch.ElapsedMilliseconds + "ms");
            stopwatch = Stopwatch.StartNew();
            var types2 = (from a in assemblies
                          from t in a.GetTypes()
                          where
                              typeof (IWantCustomInitialization).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract
                          select t).ToList();
            stopwatch.Stop();
            Debug.WriteLine("Find implementations 2: " + stopwatch.ElapsedMilliseconds + "ms");

        }
    }
}