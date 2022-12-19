using System.ServiceModel;

namespace PumpService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PumpService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PumpService.svc or PumpService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class PumpService : IPumpService
    {
        private readonly IStatisticsService _statisticsService;
        private readonly ISettingsService _settingsService;
        private readonly IScriptService _scriptService;

        private IPumpServiceCallback Callback => OperationContext.Current?.GetCallbackChannel<IPumpServiceCallback>();

        public PumpService()
        {
            _statisticsService = new StatisticsService();
            _settingsService = new SettingsService();
            _scriptService = new ScriptService(_statisticsService, _settingsService, Callback);
        }

        public void RunScript()
        {
            _scriptService.Run(10);
        }

        public void UpdateAndCompileScript(string filename)
        {
            _settingsService.FileName = filename;
            _scriptService.Compile();
        }
    }
}