namespace PumpService
{
    public interface IScriptService
    {
        bool Compile();
        void Run(int count);
    }
}