using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PumpService
{
    public class ScriptService : IScriptService
    {
        private CompilerResults _compilerResults = null;
        private readonly IStatisticsService _statisticsService;
        private readonly ISettingsService _settingsService;
        private readonly IPumpServiceCallback _pumpServiceCallback;

        public ScriptService(IStatisticsService statisticsService, ISettingsService settingsService, IPumpServiceCallback pumpServiceCallback)
        {
            _statisticsService = statisticsService;
            _settingsService = settingsService;
            _pumpServiceCallback = pumpServiceCallback;
        }

        public bool Compile()
        {
            try
            {
                var compilerParameters = new CompilerParameters();
                compilerParameters.GenerateExecutable = true;
                compilerParameters.ReferencedAssemblies.Add("System.dll");
                compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
                compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
                compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
                compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

                var fs = new FileStream(_settingsService.FileName, FileMode.Open);
                byte[] buffer = new byte[0];
                try
                {
                    var length = (int)fs.Length;
                    buffer = new byte[length];
                    int count, sum = 0;
                    while ((count = fs.Read(buffer, sum, length - sum)) > 0)
                        sum += count;
                }
                finally
                {
                    fs.Close();
                }

                var provider = new CSharpCodeProvider();

                _compilerResults = provider.CompileAssemblyFromSource(compilerParameters, Encoding.UTF8.GetString(buffer));
                if (_compilerResults.Errors?.HasErrors == true)
                {
                    string compileErrors = string.Empty;
                    for (int i = 0; i < _compilerResults.Errors.Count; i++)
                    {
                        if (compileErrors != string.Empty)
                            compileErrors += "\n";

                        compileErrors += _compilerResults.Errors[i];
                    }

                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Run(int count)
        {
            if (_compilerResults == null || _compilerResults.Errors?.HasErrors == true)
            {
                if (!Compile()) return;
            }

            var type = _compilerResults.CompiledAssembly.GetType("Sample.SampleScript");
            if (type == null) return;

            var entryPointMethod = type.GetMethod("EntryPoint");
            if (entryPointMethod == null) return;

            Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    if ((bool)entryPointMethod.Invoke(Activator.CreateInstance(type), null))
                        _statisticsService.SuccessTacts++;
                    else
                        _statisticsService.ErrorTacts++;

                    _statisticsService.AllTacts++;

                    _pumpServiceCallback?.UpdateStatistics((StatisticsService)_statisticsService);
                    Thread.Sleep(1000);
                }
            });
        }
    }
}