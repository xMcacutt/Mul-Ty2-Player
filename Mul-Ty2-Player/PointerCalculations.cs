using MT2PClient;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MT2PClient
{
    internal static class PointerCalculations
    {
        public static int GetPointerAddress(int baseAddress, int[] offsets, int extraOffset = 0)
        {
            //READS MEMORY AT ADDRESS+OFFSETS[i] STORES IN ADDRESS AND REPEATS FOR ALL OFFSETS.
            //MEMORY AT ADDR+OFFSET[i] IS NOT READ.
            //ADDR+OFFSET[i] IS RETURNED
            int addr = baseAddress;
            for (int i = 0; i < offsets.Length; i++)
            {
                bool addBase = i == 0;
                ProcessHandler.TryRead(addr, out int nextAddress, addBase);
                addr = nextAddress + offsets[i];
            }
            if (extraOffset == 0)
            {
                return addr;
            }
            else
            {
                addr += extraOffset;
                return addr;
            }
        }
    }
}
