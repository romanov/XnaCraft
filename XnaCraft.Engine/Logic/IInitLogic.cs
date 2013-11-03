namespace XnaCraft.Engine.Logic
{
    public interface IInitLogic : ILogic
    {
        void OnInit();
        void OnShutdown();
    }
}