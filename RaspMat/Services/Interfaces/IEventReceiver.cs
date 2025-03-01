namespace RaspMat.Services.Interfaces
{
    internal interface IEventReceiver<TEventType>
    {

        void Receive(TEventType message);

    }
}
