namespace TvLight.Hue
{
    public interface ISwitchable
    {
        bool TurnedOn { get; }
        void TurnOn();
        void TurnOff();
    }
}