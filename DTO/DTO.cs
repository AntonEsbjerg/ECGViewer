using System;

namespace DTO
{
    public class DTO
    {
        public double ECGVoltage { get; set; }
        public double Msec { get; set; }

        public DTO(double ecgvoltage, double msec)
        {
            ECGVoltage = ecgvoltage;
            Msec = msec;
        }
    }
}
