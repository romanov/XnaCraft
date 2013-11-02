namespace XnaCraft.Engine
{
    public interface IInitLogic : ILogic
    {
        void OnInit();
        void OnShutdown();
    }
}