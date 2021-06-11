﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServerTools
{
    [Serializable]
    public class PersistentContainer
    {
        private static string filepath = string.Format("{0}/ServerTools.bin", API.ConfigPath);
        private static PersistentContainer instance;
        private static bool Saving = false;
        public static bool DataChange = false;

        private PersistentPlayers players;
        private Dictionary<int, int> auctionPrices;
        private Dictionary<int, List<int>> clientMuteList;
        private DateTime lastWeather;
        private string[] pollData;
        private Dictionary<string[], string> pollOld;
        private bool pollOpen;
        private Dictionary<string, bool> pollVote;
        private List<string> regionReset;
        private List<string[]> track;
        private Dictionary<string, string[]> webPanelAuthorizedIVKeyList;
        private Dictionary<string, DateTime> webPanelAuthorizedTimeList;
        private List<string> webPanelBanList;
        private Dictionary<string, DateTime> webPanelTimeoutList;
        

        public static PersistentContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PersistentContainer();
                }
                return instance;
            }
        }

        public PersistentPlayers Players
        {
            get
            {
                if (players == null)
                {
                    players = new PersistentPlayers();
                }
                return players;
            }
        }

        private PersistentContainer()
        {
        }

        public void Save()
        {
            try
            {
                if (DataChange)
                {
                    if (!Saving)
                    {
                        if (!StopServer.NoEntry)
                        {
                            Saving = true;
                            Stream stream = File.Open(filepath, FileMode.Create, FileAccess.ReadWrite);
                            BinaryFormatter bFormatter = new BinaryFormatter();
                            bFormatter.Serialize(stream, this);
                            stream.Close();
                            stream.Dispose();
                            Saving = false;
                        }
                        else
                        {
                            Log.Out(string.Format("[SERVERTOOLS] Unable to run data save so close to shutdown."));
                        }
                    }
                    DataChange = false;
                }
            }
            catch (Exception e)
            {
                Saving = false;
                Log.Out(string.Format("[SERVERTOOLS] Exception in PersistentContainer.Save: {0}", e.Message));
            }
        }

        public bool Load()
        {
            try
            {
                if (File.Exists(filepath))
                {
                    PersistentContainer obj;
                    Stream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    obj = (PersistentContainer)bFormatter.Deserialize(stream);
                    stream.Close();
                    stream.Dispose();
                    instance = obj;
                    return true;
                }
                else
                {
                    Stream stream = File.Open(filepath, FileMode.Create, FileAccess.ReadWrite);
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    bFormatter.Serialize(stream, this);
                    stream.Close();
                    stream.Dispose();
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Exception in PersistentContainer.Load: {0}", e.Message));
            }
            return false;
        }

        public Dictionary<int, int> AuctionPrices
        {
            get
            {
                return auctionPrices;
            }
            set
            {
                auctionPrices = value;
            }
        }

        public Dictionary<int, List<int>> ClientMuteList
        {
            get
            {
                return clientMuteList;
            }
            set
            {
                clientMuteList = value;
            }
        }

        public DateTime LastWeather
        {
            get
            {
                return lastWeather;
            }
            set
            {
                lastWeather = value;
            }
        }

        public string[] PollData
        {
            get
            {
                return pollData;
            }
            set
            {
                pollData = value;
            }
        }

        public Dictionary<string[], string> PollOld
        {
            get
            {
                return pollOld;
            }
            set
            {
                pollOld = value;
            }
        }

        public bool PollOpen
        {
            get
            {
                return pollOpen;
            }
            set
            {
                pollOpen = value;
            }
        }

        public Dictionary<string, bool> PollVote
        {
            get
            {
                return pollVote;
            }
            set
            {
                pollVote = value;
            }
        }

        public List<string> RegionReset
        {
            get
            {
                return regionReset;
            }
            set
            {
                regionReset = value;
            }
        }

        public List<string[]> Track
        {
            get
            {
                return track;
            }
            set
            {
                track = value;
            }
        }

        public Dictionary<string, string[]> WebPanelAuthorizedIVKeyList
        {
            get
            {
                return webPanelAuthorizedIVKeyList;
            }
            set
            {
                webPanelAuthorizedIVKeyList = value;
            }
        }

        public Dictionary<string, DateTime> WebPanelAuthorizedTimeList
        {
            get
            {
                return webPanelAuthorizedTimeList;
            }
            set
            {
                webPanelAuthorizedTimeList = value;
            }
        }

        public List<string> WebPanelBanList
        {
            get
            {
                return webPanelBanList;
            }
            set
            {
                webPanelBanList = value;
            }
        }

        public Dictionary<string, DateTime> WebPanelTimeoutList
        {
            get
            {
                return webPanelTimeoutList;
            }
            set
            {
                webPanelTimeoutList = value;
            }
        }
    }
}