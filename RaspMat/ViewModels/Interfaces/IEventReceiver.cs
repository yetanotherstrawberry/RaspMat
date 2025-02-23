namespace RaspMat.ViewModels.Interfaces
{
    internal interface IEventReceiver<TEventType>
    {

        void Receive(TEventType message);

    }
}
