﻿using System;
using System.IO;
using System.Text;

namespace ServerTools
{
    public class EntityDamage
    {
        public static bool IsEnabled = false;
        public static int Admin_Level = 0, Entity_Damage_Limit = 500, Player_Damage_Limit = 3000;
        private static readonly string file = string.Format("DamageLog_{0}.txt", DateTime.Today.ToString("M-d-yyyy"));
        private static readonly string filepath = string.Format("{0}/Logs/DamageLogs/{1}", API.ConfigPath, file);
        private static readonly string detectionFile = string.Format("DetectionLog_{0}.txt", DateTime.Today.ToString("M-d-yyyy"));
        private static readonly string detectionFilepath = string.Format("{0}/Logs/DetectionLogs/{1}", API.ConfigPath, detectionFile);

        public static bool ProcessEntityDamage(EntityAlive __instance, DamageResponse _dmResponse)
        {
            try
            {
                if (__instance != null && _dmResponse.Source != null && _dmResponse.Strength > 1)
                {
                    if (__instance is EntityPlayer player)
                    {
                        EntityPlayer _player1 = player;
                        int _sourceId = _dmResponse.Source.getEntityId();
                        if (_sourceId > 0 && _player1.entityId != _sourceId)
                        {
                            ClientInfo _cInfo2 = PersistentOperations.GetClientInfoFromEntityId(_sourceId);
                            if (_cInfo2 != null)
                            {
                                EntityPlayer _player2 = PersistentOperations.GetEntityPlayer(_cInfo2.playerId);
                                if (_player2 != null)
                                {
                                    if (IsEnabled)
                                    {
                                        ItemValue _itemValue = ItemClass.GetItem(_player2.inventory.holdingItem.Name, false);
                                        if (_itemValue != null)
                                        {
                                            int _distance = (int)_player2.GetDistance(__instance);
                                            using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                            {
                                                sw.WriteLine(string.Format("{0}: {1} \"{2}\" hit \"{3}\" with entity id {4} using {5} for {6} damage @ {7}. Distance: {8}", DateTime.Now, _cInfo2.playerId, _cInfo2.playerName, __instance.EntityName, __instance.entityId, _itemValue.ItemClass.GetLocalizedItemName() ?? _itemValue.ItemClass.GetItemName(), _dmResponse.Strength, __instance.position, _distance));
                                                sw.WriteLine();
                                                sw.Flush();
                                                sw.Close();
                                            }
                                            if (_dmResponse.Strength >= Player_Damage_Limit && GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo2) > Admin_Level)
                                            {
                                                Phrases.Dict.TryGetValue(952, out string _phrase952);
                                                SdtdConsole.Instance.ExecuteSync(string.Format("ban add {0} 5 years \"{1} {2}\"", _cInfo2.playerId, _phrase952, _dmResponse.Strength), null);
                                                using (StreamWriter sw = new StreamWriter(detectionFilepath, true, Encoding.UTF8))
                                                {
                                                    sw.WriteLine(string.Format("Detected \"{0}\" Steam Id {1}, exceeded damage limit @ {2}. Damage: {3}", _cInfo2.playerName, _cInfo2.playerId, __instance.position, _dmResponse.Strength));
                                                    sw.WriteLine();
                                                    sw.Flush();
                                                    sw.Close();
                                                }
                                                Phrases.Dict.TryGetValue(951, out string _phrase951);
                                                _phrase951 = _phrase951.Replace("{PlayerName}", _cInfo2.playerName);
                                                ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase951 + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
                                                return false;
                                            }
                                        }
                                    }
                                    if (Zones.IsEnabled && (Zones.ZonePvE.Contains(__instance.entityId) || Zones.ZonePvE.Contains(_cInfo2.entityId)))
                                    {
                                        Phrases.Dict.TryGetValue(323, out string _phrase323);
                                        ChatHook.ChatMessage(_cInfo2, Config.Chat_Response_Color + _phrase323 + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                                        if (!_player1.IsFriendsWith(_player2))
                                        {
                                            if (PersistentOperations.PvEViolations.ContainsKey(_cInfo2.entityId))
                                            {
                                                PersistentOperations.PvEViolations.TryGetValue(_cInfo2.entityId, out int _violations);
                                                _violations++;
                                                PersistentOperations.PvEViolations[_cInfo2.entityId] = _violations;
                                                if (PersistentOperations.Jail_Violation > 0 && _violations == PersistentOperations.Jail_Violation)
                                                {
                                                    Jail(_cInfo2, __instance);
                                                }
                                                if (PersistentOperations.Kill_Violation > 0 && _violations == PersistentOperations.Kill_Violation)
                                                {
                                                    Kill(_cInfo2);
                                                }
                                                if (PersistentOperations.Kick_Violation > 0 && _violations == PersistentOperations.Kick_Violation)
                                                {
                                                    Kick(_cInfo2);
                                                }
                                                if (PersistentOperations.Ban_Violation > 0 && _violations == PersistentOperations.Ban_Violation)
                                                {
                                                    Ban(_cInfo2);
                                                }
                                            }
                                            else
                                            {
                                                PersistentOperations.PvEViolations.Add(_cInfo2.entityId, 1);
                                            }
                                        }
                                        return false;
                                    }
                                    if ((Lobby.IsEnabled && Lobby.PvE && Lobby.LobbyPlayers.Contains(__instance.entityId)) || (Market.IsEnabled && Market.PvE && Market.MarketPlayers.Contains(__instance.entityId)))
                                    {
                                        Phrases.Dict.TryGetValue(260, out string _phrase260);
                                        ChatHook.ChatMessage(_cInfo2, Config.Chat_Response_Color + _phrase260 + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                                        if (!_player1.IsFriendsWith(_player2))
                                        {
                                            if (PersistentOperations.PvEViolations.ContainsKey(_cInfo2.entityId))
                                            {
                                                PersistentOperations.PvEViolations.TryGetValue(_cInfo2.entityId, out int _violations);
                                                _violations++;
                                                PersistentOperations.PvEViolations[_cInfo2.entityId] = _violations;
                                                if (PersistentOperations.Jail_Violation > 0 && _violations == PersistentOperations.Jail_Violation)
                                                {
                                                    Jail(_cInfo2, __instance);
                                                }
                                                if (PersistentOperations.Kill_Violation > 0 && _violations == PersistentOperations.Kill_Violation)
                                                {
                                                    Kill(_cInfo2);
                                                }
                                                if (PersistentOperations.Kick_Violation > 0 && _violations == PersistentOperations.Kick_Violation)
                                                {
                                                    Kick(_cInfo2);
                                                }
                                                else if (PersistentOperations.Ban_Violation > 0 && _violations == PersistentOperations.Ban_Violation)
                                                {
                                                    Ban(_cInfo2);
                                                }
                                            }
                                            else
                                            {
                                                PersistentOperations.PvEViolations.Add(_cInfo2.entityId, 1);
                                            }
                                        }
                                        return false;
                                    }
                                    if (KillNotice.IsEnabled && KillNotice.Show_Damage && KillNotice.PvP)
                                    {
                                        if (KillNotice.Damage.ContainsKey(_player1.entityId))
                                        {
                                            KillNotice.Damage[_player1.entityId] = _dmResponse.Strength;
                                        }
                                        else
                                        {
                                            KillNotice.Damage.Add(_player1.entityId, _dmResponse.Strength);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (IsEnabled && __instance is EntityZombie)
                    {
                        int _sourceId = _dmResponse.Source.getEntityId();
                        if (_sourceId > 0)
                        {
                            ClientInfo _cInfo = PersistentOperations.GetClientInfoFromEntityId(_sourceId);
                            if (_cInfo != null)
                            {
                                EntityPlayer _player = PersistentOperations.GetEntityPlayer(_cInfo.playerId);
                                if (_player != null)
                                {
                                    ItemValue _itemValue = ItemClass.GetItem(_player.inventory.holdingItem.Name, false);
                                    if (_itemValue != null)
                                    {
                                        int _distance = (int)_player.GetDistance(__instance);
                                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                        {
                                            sw.WriteLine(string.Format("{0}: {1} \"{2}\" hit \"{3}\" with entity id {4} using {5} for {6} damage @ {7}. Distance: {8}", DateTime.Now, _cInfo.playerId, _cInfo.playerName, __instance.EntityName, __instance.entityId, _itemValue.ItemClass.GetLocalizedItemName() ?? _itemValue.ItemClass.GetItemName(), _dmResponse.Strength, __instance.position, _distance));
                                            sw.WriteLine();
                                            sw.Flush();
                                            sw.Close();
                                        }
                                        if (_dmResponse.Strength >= Entity_Damage_Limit && GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo) > Admin_Level)
                                        {
                                            Phrases.Dict.TryGetValue(952, out string _phrase952);
                                            SdtdConsole.Instance.ExecuteSync(string.Format("ban add {0} 5 years \"{1} {2}\"", _cInfo.playerId, _phrase952, _dmResponse.Strength), null);
                                            using (StreamWriter sw = new StreamWriter(detectionFilepath, true, Encoding.UTF8))
                                            {
                                                sw.WriteLine(string.Format("Detected \"{0}\" Steam Id {1}, exceeded damage limit @ {2}. Damage: {3}", _cInfo.playerName, _cInfo.playerId, __instance.position, _dmResponse.Strength));
                                                sw.WriteLine();
                                                sw.Flush();
                                                sw.Close();
                                            }
                                            Phrases.Dict.TryGetValue(951, out string _phrase951);
                                            _phrase951 = _phrase951.Replace("{PlayerName}", _cInfo.playerName);
                                            ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase951 + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in ProcessDamage.ProcessEntityDamage: {0}", e.Message));
            }
            return true;
        }

        private static void Jail(ClientInfo _cInfoKiller, EntityAlive _cInfoVictim)
        {
            SdtdConsole.Instance.ExecuteSync(string.Format("st-Jail add {0} 120", _cInfoKiller.playerId), null);
            if (!Zones.Forgive.ContainsKey(_cInfoVictim.entityId))
            {
                Zones.Forgive.Add(_cInfoVictim.entityId, _cInfoKiller.entityId);
            }
            else
            {
                Zones.Forgive[_cInfoVictim.entityId] = _cInfoKiller.entityId;
            }
            Phrases.Dict.TryGetValue(204, out string _phrase204);
            _phrase204 = _phrase204.Replace("{PlayerName}", _cInfoKiller.playerName);
            ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase204 + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
        }

        private static void Kill(ClientInfo _cInfo)
        {
            SdtdConsole.Instance.ExecuteSync(string.Format("kill {0}", _cInfo.playerId), null);
            Phrases.Dict.TryGetValue(324, out string _phrase324);
            _phrase324 = _phrase324.Replace("{PlayerName}", _cInfo.playerName);
            ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase324 + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
        }

        private static void Kick(ClientInfo _cInfo)
        {
            Phrases.Dict.TryGetValue(326, out string _phrase326);
            SdtdConsole.Instance.ExecuteSync(string.Format("kick {0} \"{1}\"", _cInfo.playerId, _phrase326), null);
            Phrases.Dict.TryGetValue(325, out string _phrase325);
            _phrase325 = _phrase325.Replace("{PlayerName}", _cInfo.playerName);
            ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase325 + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
        }

        private static void Ban(ClientInfo _cInfo)
        {
            Phrases.Dict.TryGetValue(328, out string _phrase328);
            SdtdConsole.Instance.ExecuteSync(string.Format("ban add {0} 5 years \"{1}\"", _cInfo.playerId, _phrase328), null);
            Phrases.Dict.TryGetValue(327, out string _phrase327);
            _phrase327 = _phrase327.Replace("{PlayerName}", _cInfo.playerName);
            ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase327 + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
        }
    }
}
