using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT2PClient
{
    internal class LevelHandler
    {
        public string PreviousLevel = "mainmenu";
        public string CurrentLevel;
        public bool InMainWorld;
        public bool NewLevelSetup;

        public void DoLevelSetup()
        {
            //HSync.SetMemAddrs();
            //HSync.RequestSync();
            //HSync.ProtectLeaderboard();
            KoalaHandler.SetCoordAddresses();
            NewLevelSetup = true;
        }

        public void GetCurrentLevel()
        {
            ProcessHandler.TryRead(0x4F3888, 8, out byte[]? buffer, true);
            if (buffer == null)
            {
                CurrentLevel = "mainmenu"; 
                return;
            }
            CurrentLevel = Encoding.ASCII.GetString(buffer).TrimEnd();
            InMainWorld = (!CurrentLevel.StartsWith("r", StringComparison.CurrentCultureIgnoreCase) && 
                !CurrentLevel.StartsWith("m", StringComparison.CurrentCultureIgnoreCase));
            if(CurrentLevel != PreviousLevel)
            {
                PreviousLevel = CurrentLevel;
                NewLevelSetup = false;
            }
        }
    }
}
