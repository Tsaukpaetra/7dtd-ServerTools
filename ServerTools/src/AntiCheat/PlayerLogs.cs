﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerTools
{
    class PlayerLogs
    {
        public static bool IsEnabled = false;
        public static int Delay = 60, Days_Before_Log_Delete = 5;
        private static readonly string file = string.Format("PlayerLog_{0}.txt", DateTime.Today.ToString("M-d-yyyy"));
        private static readonly string filepath = string.Format("{0}/Logs/PlayerLogs/{1}", API.ConfigPath, file);

        public static void Exec()
        {
            try
            {
                if (GameManager.Instance.World != null && GameManager.Instance.World.Players.Count > 0 && GameManager.Instance.World.Players.dict.Count > 0)
                {
                    List<EntityPlayer> EntityPlayerList = GameManager.Instance.World.Players.list;
                    for (int i = 0; i < EntityPlayerList.Count; i++)
                    {
                        PlayerDataFile playerDataFile = new PlayerDataFile();
                        EntityPlayer _player = EntityPlayerList[i] as EntityPlayer;
                        if (_player != null)
                        {
                            ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForEntityId(_player.entityId);
                            if (_cInfo != null)
                            {
                                if (_player.IsSpawned())
                                {
                                    var _x = (int)_player.position.x;
                                    var _y = (int)_player.position.y;
                                    var _z = (int)_player.position.z;
                                    double _regionX, _regionZ;
                                    if (_player.position.x < 0)
                                    {
                                        _regionX = Math.Truncate(_player.position.x / 512) - 1;
                                    }
                                    else
                                    {
                                        _regionX = Math.Truncate(_player.position.x / 512);
                                    }
                                    if (_player.position.z < 0)
                                    {
                                        _regionZ = Math.Truncate(_player.position.z / 512) - 1;
                                    }
                                    else
                                    {
                                        _regionZ = Math.Truncate(_player.position.z / 512);
                                    }
                                    string _ip = _cInfo.ip;
                                    using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                    {
                                        sw.WriteLine(string.Format("{0}: \"{1}\" SteamId {2}. IP Address {3} at Position: {4} X {5} Y {6} Z in RegionFile: r.{7}.{8}.7rg", DateTime.Now, _cInfo.playerName, _cInfo.playerId, _ip, _x, _y, _z, _regionX, _regionZ));
                                        sw.WriteLine();
                                        sw.Flush();
                                        sw.Close();
                                        sw.Dispose();
                                    }
                                    using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                    {
                                        sw.WriteLine(string.Format("Stats: Health={0} Stamina={1} ZombieKills={2} PlayerKills={3} PlayerLevel={4}", (int)_player.Stats.Health.Value, (int)_player.Stats.Stamina.Value, _player.KilledZombies, _player.KilledPlayers, _player.Progression.GetLevel()));
                                        sw.WriteLine();
                                        sw.Flush();
                                        sw.Close();
                                        sw.Dispose();
                                    }
                                    playerDataFile.Load(GameUtils.GetPlayerDataDir(), _cInfo.playerId.Trim());
                                    if (playerDataFile != null)
                                    {
                                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                        {
                                            sw.WriteLine(string.Format("Inventory of " + _cInfo.playerName + " steamId {0}", _cInfo.playerId));
                                            sw.WriteLine("Belt:");
                                            sw.Flush();
                                            sw.Close();
                                        }
                                        PrintInv(playerDataFile.inventory, _cInfo.entityId, "belt");
                                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                        {
                                            sw.WriteLine("Backpack:");
                                            sw.Flush();
                                            sw.Close();
                                            sw.Dispose();
                                        }
                                        PrintInv(playerDataFile.bag, _cInfo.entityId, "backpack");
                                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                        {
                                            sw.WriteLine("Equipment:");
                                            sw.Flush();
                                            sw.Close();
                                            sw.Dispose();
                                        }
                                        PrintEquipment(playerDataFile.equipment, _cInfo.entityId, "equipment");
                                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                        {
                                            sw.WriteLine("End of inventory");
                                            sw.WriteLine();
                                            sw.WriteLine("----------------");
                                            sw.WriteLine();
                                            sw.Flush();
                                            sw.Close();
                                            sw.Dispose();
                                        }
                                    }
                                }
                                else if (!_player.IsDead() && !_player.IsSpawned())
                                {
                                    using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                    {
                                        sw.WriteLine(string.Format("{0}: \"{1}\" SteamId {2}. Player has not spawned", DateTime.Now, _cInfo.playerName, _cInfo.playerId));
                                        sw.WriteLine();
                                        sw.WriteLine("----------------");
                                        sw.WriteLine();
                                        sw.Flush();
                                        sw.Close();
                                        sw.Dispose();
                                    }
                                }
                                else if (_player.IsDead())
                                {
                                    using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                    {
                                        sw.WriteLine(string.Format("{0}: \"{1}\" SteamId {2}. Player is currently dead", DateTime.Now, _cInfo.playerName, _cInfo.playerId));
                                        sw.WriteLine();
                                        sw.WriteLine("----------------");
                                        sw.WriteLine();
                                        sw.Flush();
                                        sw.Close();
                                        sw.Dispose();
                                    }
                                }
                            }
                        }
                    }
                    using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                    {
                        sw.WriteLine("***********************************************************");
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in PlayerLogs.Exec: {0}.", e.Message));
            }
        }

        private static void PrintInv(ItemStack[] _inv, int _entityId, string _location)
        {
            for (int i = 0; i < _inv.Length; i++)
            {
                if (!_inv[i].IsEmpty())
                {
                    if (_inv[i].itemValue.HasQuality && _inv[i].itemValue.Quality > 0)
                    {
                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                        {
                            sw.WriteLine(string.Format("    Slot {0}: {1:000} * {2} - quality: {3}", i, _inv[i].count, _inv[i].itemValue.ItemClass.GetItemName(), _inv[i].itemValue.Quality));
                            sw.Flush();
                            sw.Close();
                            sw.Dispose();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                        {
                            sw.WriteLine(string.Format("    Slot {0}: {1:000} * {2}", i, _inv[i].count, _inv[i].itemValue.ItemClass.GetItemName()));
                            sw.Flush();
                            sw.Close();
                            sw.Dispose();
                        }
                    }
                    if (_inv[i].itemValue.Modifications != null && _inv[i].itemValue.Modifications.Length > 0)
                    {
                        Mods(_inv[i].itemValue.Modifications, 1, null);
                    }
                    if (_inv[i].itemValue.CosmeticMods != null && _inv[i].itemValue.CosmeticMods.Length > 0)
                    {
                        CosmeticMods(_inv[i].itemValue.CosmeticMods, 1, null);
                    }
                }
            }
        }

        private static void PrintEquipment(Equipment _equipment, int _entityId, string _location)
        {
            for (int i = 0; i < _equipment.GetSlotCount(); i++)
            {
                ItemValue _item = _equipment.GetSlotItem(i);
                if (_item != null && !_item.IsEmpty())
                {
                    if (_item.HasQuality && _item.Quality > 0)
                    {
                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                        {
                            sw.WriteLine(string.Format("    Slot {0}: {1} - quality: {2}", _item.ItemClass.EquipSlot, _item.ItemClass.GetItemName(), _item.Quality));
                            sw.Flush();
                            sw.Close();
                            sw.Dispose();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                        {
                            sw.WriteLine(string.Format("    Slot {0}: {1}", _item.ItemClass.EquipSlot, _item.ItemClass.GetItemName()));
                            sw.Flush();
                            sw.Close();
                            sw.Dispose();
                        }
                    }
                    if (_item.Modifications != null && _item.Modifications.Length > 0)
                    {
                        Mods(_item.Modifications, 1, null);
                    }
                    if (_item.CosmeticMods != null && _item.CosmeticMods.Length > 0)
                    {
                        CosmeticMods(_item.CosmeticMods, 1, null);
                    }
                }
            }
        }

        private static string Mods(ItemValue[] _parts, int _indent, string _currentMessage)
        {
            if (_parts != null && _parts.Length > 0)
            {
                string indenter = new string(' ', _indent * 4);
                for (int i = 0; i < _parts.Length; i++)
                {
                    if (_parts[i] != null && !_parts[i].IsEmpty())
                    {
                        if (_currentMessage == null)
                        {
                            using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                            {
                                sw.WriteLine(string.Format("{0}         - {1}", indenter, _parts[i].ItemClass.GetItemName()));
                                sw.Flush();
                                sw.Close();
                                sw.Dispose();
                            }
                        }
                        else
                        {
                            if (_currentMessage.Length > 0)
                            {
                                _currentMessage += ",";
                            }
                            _currentMessage += _parts[i].ItemClass.GetItemName();
                            _currentMessage = Mods(_parts[i].Modifications, _indent + 1, _currentMessage);
                        }
                    }
                }
            }
            return _currentMessage;
        }

        private static string CosmeticMods(ItemValue[] _parts, int _indent, string _currentMessage)
        {
            if (_parts != null && _parts.Length > 0)
            {
                string indenter = new string(' ', _indent * 4);
                for (int i = 0; i < _parts.Length; i++)
                {
                    if (_parts[i] != null && !_parts[i].IsEmpty())
                    {
                        if (_currentMessage == null)
                        {
                            using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                            {
                                sw.WriteLine(string.Format("{0}         - {1}", indenter, _parts[i].ItemClass.GetItemName()));
                                sw.Flush();
                                sw.Close();
                                sw.Dispose();
                            }
                        }
                        else
                        {
                            if (_currentMessage.Length > 0)
                            {
                                _currentMessage += ",";
                            }
                            _currentMessage += _parts[i].ItemClass.GetItemName();
                            _currentMessage = CosmeticMods(_parts[i].Modifications, _indent + 1, _currentMessage);
                        }
                    }
                }
            }
            return _currentMessage;
        }
    }
}
