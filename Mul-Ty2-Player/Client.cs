using MT2PClient;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace MT2PClient
{
    internal class Client
    {
        public static bool IsRunning;
        public static bool Relaunching = false;
        public static bool KoalaSelected = false;
        public static Riptide.Client _client;
        private static string _ip;
        private static string _pass;
        public static string Name;

        public static KoalaHandler HKoala;
        //public static CommandHandler HCommand;
        public static HeroHandler HHero;
        public static LevelHandler HLevel;
        //public static SyncHandler HSync;

        public static CancellationTokenSource cts;

        public static void StartClient(string ip, string name, string pass)
        {
            HLevel = new LevelHandler();
            //HSync = new SyncHandler();
            HHero = new HeroHandler();
            HKoala = new KoalaHandler();
            //HCommand = new CommandHandler();

            _ip = ip;
            _pass = pass;
            Name = name;

            _client = new Riptide.Client();
            _client.Connected += (s, e) => Connected();
            _client.Disconnected += (s, e) => Disconnected(s, e);
            _client.ConnectionFailed += (s, e) => ConnectionFailed();

            cts = new CancellationTokenSource();

            Message authentication = Message.Create();
            authentication.AddString(_pass);
            if (!_ip.Contains(':')) { _ip += ":8750" /*+ SettingsHandler.Settings.Port*/; }
            _client.Connect(_ip, 5, 0, authentication);

            Thread _loop = new(ClientLoop);
            _loop.Start();
        }

        private static void ClientLoop()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (IsRunning)
                {
                    try
                    {                        
                        HHero.SendCoordinates();
                        if (HLevel.InMainWorld)
                        {
                            //NEW LEVEL SETUP STUFF
                            if (!HLevel.NewLevelSetup)
                            {
                                HLevel.DoLevelSetup();
                            }

                            HHero.GetTyPosRot();
                            /*//OBSERVERS
                            if (SettingsHandler.DoOpalSyncing && HLevel.MainStages.Contains(HLevel.CurrentLevelId))
                            {
                                SyncHandler.HOpal.CheckObserverChanged();
                                SyncHandler.HCrate.CheckObserverChanged();
                            }
                            if (SettingsHandler.DoTESyncing) SyncHandler.HThEg.CheckObserverChanged();
                            if (SettingsHandler.DoCogSyncing) SyncHandler.HCog.CheckObserverChanged();
                            if (SettingsHandler.DoBilbySyncing) SyncHandler.HBilby.CheckObserverChanged();
                            if (SettingsHandler.DoRangSyncing) SyncHandler.HAttribute.CheckObserverChanged();
                            if (SettingsHandler.DoPortalSyncing) SyncHandler.HPortal.CheckObserverChanged();
                            if (SettingsHandler.DoCliffsSyncing) SyncHandler.HCliffs.CheckObserverChanged();
                            */
                        }
                    }
                    catch (TyClosedException ex)
                    {
                        Relaunching = true;
                        //Console.WriteLine(ex.Message);
                        //BasicIoC.SFXPlayer.PlaySound(SFX.MenuCancel);
                        while (!ProcessHandler.FindTyProcess())
                        {
                            _client.Update();
                            Thread.Sleep(10);
                        }
                        Console.WriteLine("Ty has been restarted. You're back in!");
                        //BasicIoC.SFXPlayer.PlaySound(SFX.MenuAccept);
                        Relaunching = false;
                        continue;
                    }
                }
                _client.Update();
                Thread.Sleep(10);
            }
        }

        private static void Connected()
        {
            //BasicIoC.LoginViewModel.SaveDetails();
            //BasicIoC.KoalaSelectViewModel.Setup();
            //BasicIoC.LoginViewModel.ConnectionAttemptSuccessful = true;
            //BasicIoC.LoginViewModel.ConnectionAttemptCompleted = true;
            Console.WriteLine("Connected Successfully!");
        }

        private static void Disconnected(object sender, Riptide.DisconnectedEventArgs e)
        {
            cts.Cancel();
            IsRunning = false;
            //BasicIoC.KoalaSelectViewModel.MakeAllAvailable();
            //BasicIoC.SFXPlayer.PlaySound(SFX.PlayerDisconnect);

            /*if (e.Reason == DisconnectReason.TimedOut && SettingsHandler.Settings.AttemptReconnect)
            {
                BasicIoC.LoggerInstance.Write("Initiating reconnection attempt.");
                cts = new CancellationTokenSource();
                _client.ConnectionFailed -= connectionFailedHandler;
                connectionFailedReconnectHandler = (s, e) => AutoReconnect.ConnectionFailed();
                _client.ConnectionFailed += connectionFailedReconnectHandler;
                Message authentication = Message.Create();
                authentication.AddString(_pass);
                if (!_ip.Contains(':')) { _ip += ":" + SettingsHandler.Settings.Port; }
                _client.Connect(_ip, 5, 0, authentication);
                Thread _loop = new(ClientLoop);
                _loop.Start();
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    WindowHandler.KoalaSelectWindow.Hide();
                    WindowHandler.ClientGUIWindow.Hide();
                    WindowHandler.SettingsWindow.Hide();
                    BasicIoC.LoggerInstance.Log.Clear();
                    BasicIoC.LoginViewModel.ConnectEnabled = true;
                    WindowHandler.LoginWindow.Show();
                }));*/
        }

        private static void ConnectionFailed()
        {
            SystemSounds.Hand.Play();
            Console.WriteLine("Connection failed!\nPlease check IPAddress & Password are correct and server is open.");
            cts.Cancel();
            return;
        }

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            Console.WriteLine(message.GetString());
        }

        [MessageHandler((ushort)MessageID.Disconnect)]
        public static void GetDisconnectedScrub(Message message)
        {
            _client.Disconnect();
        }
    }
}

