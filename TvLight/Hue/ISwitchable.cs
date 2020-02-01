using System.Diagnostics;

namespace TvLight.Hue
{
    public interface ISwitchable
    {
        public string Name { get; }

        bool TurnedOn { get; }
        void TurnOn();
        void TurnOff();
    }
}