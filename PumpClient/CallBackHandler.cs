using PumpClient.PumpServiceReference;
using System;

namespace PumpClient
{
    public class CallBackHandler : IPumpServiceCallback
    {
        public void UpdateStatistics(StatisticsService statistics)
        {
            Console.Clear();
            Console.WriteLine("Обновление статистики выполнения скрипта");
            Console.WriteLine($"Всего тактов    : {statistics.AllTacts}");
            Console.WriteLine($"Успешных тактов : {statistics.SuccessTacts}");
            Console.WriteLine($"Ошибочных тактов: {statistics.ErrorTacts}");
        }
    }
}