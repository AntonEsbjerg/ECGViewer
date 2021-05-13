using System;

namespace DTO
{
    public class DTO_ECG
    {
        public double ECGVoltage { get; set; }

        public DTO_ECG(double ecgvoltage)
        {
            ECGVoltage = ecgvoltage;
        }
    }
}
