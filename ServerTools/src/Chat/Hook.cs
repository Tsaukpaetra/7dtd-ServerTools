﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    public class ChatHook
    {
        public static bool ChatFlood = false;
        public static bool Admin_Name_Coloring = false;
        public static bool Donator_Name_Coloring = false;
        public static bool Special_Player_Name_Coloring = false;
        public static bool Normal_Player_Name_Coloring = false;
        public static bool Reserved_Check = false;
        public static string Admin_Color = "[FF0000]";
        public static string Mod_Color = "[008000]";
        public static string Don_Color1 = "[009000]";
        public static string Don_Color2 = "[FF66CC]";
        public static string Don_Color3 = "[E9C918]";
        public static string Special_Player_Color = "[ADAD85]";
        public static string Normal_Player_Color = "[00B3B3]";
        public static string Admin_Prefix = "(ADMIN)";
        public static string Mod_Prefix = "(MOD)";
        public static string Don_Prefix1 = "(DON)";
        public static string Don_Prefix2 = "(DON)";
        public static string Don_Prefix3 = "(DON)";
        public static string Special_Player_Prefix = "(SPECIAL)";
        public static string Normal_Player_Prefix = "(NOOB)";
        public static int Admin_Level = 0;
        public static int Mod_Level = 1;
        public static int Don_Level1 = 100;
        public static int Don_Level2 = 101;
        public static int Don_Level3 = 102;
        public static string Special_Players_List = "76561191234567891,76561191987654321";
        public static bool ChatCommandPrivateEnabled = false;
        public static string Command_Private = "/";
        public static bool ChatCommandPublicEnabled = false;
        public static string Command_Public = "!";
        private static string filepath = string.Format("{0}/ServerTools.bin", GameUtils.GetSaveGameDir());
        private static SortedDictionary<string, DateTime> Dict = new SortedDictionary<string, DateTime>();
        private static SortedDictionary<string, string> Dict1 = new SortedDictionary<string, string>();
        public static List<string> SpecialPlayers = new List<string>();
        private static List<string> SpecialPlayersColorOff = new List<string>();

        public static void SpecialIdCheck()
        {
            if (Special_Player_Name_Coloring)
            {
                var s_Id = Special_Players_List.Split(',');
                foreach (var specialId in s_Id)
                {
                    SpecialPlayers.Clear();
                    SpecialPlayers.Add(Convert.ToString(specialId));
                }
            }
        }

        public static bool Hook(ClientInfo _cInfo, string _message, string _playerName, string _secondaryName, bool _localizeSecondary)
        {
            if (!string.IsNullOrEmpty(_message) && _cInfo != null && _playerName != "Server" && _secondaryName != "ServerTools")
            {
                if (ChatFlood)
                {
                    if (_message.Length > 500)
                    {
                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Message too long.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                        return false;
                    }
                }
                if (ChatLog.IsEnabled)
                {
                    ChatLog.Log(_message, _playerName);
                }
                Player p = PersistentContainer.Instance.Players[_cInfo.playerId, false];
                if (p != null)
                {
                    if (p.IsMuted)
                    {
                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, "You are muted.", "Server", false, "ServerTools", false));
                        return false;
                    }
                }
                if (Admin_Name_Coloring && !_message.StartsWith(Command_Private) && !_message.StartsWith("@") && _secondaryName != "ServerTools1" && GameManager.Instance.adminTools.IsAdmin(_cInfo.playerId) & !AdminChatColor.AdminColorOff.Contains(_cInfo.playerId))
                {
                    AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfo.playerId);
                    if (Admin.PermissionLevel <= Admin_Level)
                    {
                        if (Admin_Prefix != "")
                        {
                            _playerName = string.Format("{0}{1} {2}[-]", Admin_Color, Admin_Prefix, _playerName);
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                            return false;
                        }
                        else
                        {
                            _playerName = string.Format("{0}{1}[-]", Admin_Color, _playerName);
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                            return false;
                        }
                    }
                    if (Admin.PermissionLevel > Admin_Level & Admin.PermissionLevel <= Mod_Level)
                    {
                        if (Mod_Prefix != "")
                        {
                            _playerName = string.Format("{0}{1} {2}[-]", Mod_Color, Mod_Prefix, _playerName);
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                            return false;
                        }
                        else
                        {
                            _playerName = string.Format("{0}{1}[-]", Mod_Color, _playerName);
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                            return false;
                        }
                    }
                }
                if (Donator_Name_Coloring && !_message.StartsWith(Command_Private) && !_message.StartsWith("@") && _secondaryName != "ServerTools1" && GameManager.Instance.adminTools.IsAdmin(_cInfo.playerId))
                {
                    AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfo.playerId);
                    if (Admin.PermissionLevel == Don_Level1)
                    {
                        DateTime _dt;
                        ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                        if (DateTime.Now < _dt)
                        {
                            if (Don_Prefix1 != "")
                            {
                                _playerName = string.Format("{0}{1} {2}[-]", Don_Color1, Don_Prefix1, _playerName);
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                                return false;
                            }
                            else
                            {
                                _playerName = string.Format("{0}{1}[-]", Don_Color1, _playerName);
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                                return false;
                            }
                        }
                    }
                    if (Admin.PermissionLevel == Don_Level2)
                    {
                        DateTime _dt;
                        ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                        if (DateTime.Now < _dt)
                        {
                            if (Don_Prefix2 != "")
                            {
                                _playerName = string.Format("{0}{1} {2}[-]", Don_Color2, Don_Prefix2, _playerName);
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                                return false;
                            }
                            else
                            {
                                _playerName = string.Format("{0}{1}[-]", Don_Color2, _playerName);
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                                return false;
                            }
                        }
                    }
                    if (Admin.PermissionLevel == Don_Level3)
                    {
                        DateTime _dt;
                        ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                        if (DateTime.Now < _dt)
                        {
                            if (Don_Prefix3 != "")
                            {
                                _playerName = string.Format("{0}{1} {2}[-]", Don_Color3, Don_Prefix3, _playerName);
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                                return false;
                            }
                            else
                            {
                                _playerName = string.Format("{0}{1}[-]", Don_Color3, _playerName);
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                                return false;
                            }
                        }
                    }
                }
                if (Special_Player_Name_Coloring && !_message.StartsWith(Command_Private) && !_message.StartsWith("@") && _secondaryName != "ServerTools1" && SpecialPlayers.Contains(_cInfo.playerId) && !SpecialPlayersColorOff.Contains(_cInfo.playerId))
                {
                    if (Special_Player_Prefix != "")
                    {
                        _playerName = string.Format("{0}{1} {2}[-]", Special_Player_Color, Special_Player_Prefix, _playerName);
                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                        return false;
                    }
                    else
                    {
                        _playerName = string.Format("{0}{1}[-]", Special_Player_Color, _playerName);
                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                        return false;
                    }
                }
                if (Normal_Player_Name_Coloring && !_message.StartsWith(Command_Private) && !_message.StartsWith("@") && _secondaryName != "ServerTools1" && !SpecialPlayers.Contains(_cInfo.playerId) && !GameManager.Instance.adminTools.IsAdmin(_cInfo.playerId))
                {
                    if (Normal_Player_Prefix != "")
                    {
                        _playerName = string.Format("{0}{1} {2}[-]", Normal_Player_Color, Normal_Player_Prefix, _playerName);
                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                        return false;
                    }
                    else
                    {
                        _playerName = string.Format("{0}{1}[-]", Normal_Player_Color, _playerName);
                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, _playerName, false, "ServerTools1", false);
                        return false;
                    }
                }
                if (Badwords.Invalid_Name)
                {
                    bool _hasBadName = false;
                    string _playerName1 = _playerName.ToLower();
                    foreach (string _word in Badwords.List)
                    {
                        if (_playerName1.Contains(_word))
                        {
                            _hasBadName = true;
                        }                       
                    }
                    if (_hasBadName)
                    {
                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message, "Invalid Name-No Commands", false, "ServerTools", false);
                        return false;
                    }
                }
                if (Badwords.IsEnabled)
                {
                    bool _hasBadWord = false;
                    string _message1 = _message.ToLower();
                    foreach (string _word in Badwords.List)
                    {
                        if (_message1.Contains(_word))
                        {
                            string _replace = "";
                            for (int i = 0; i < _word.Length; i++)
                            {
                                _replace = string.Format("{0}*", _replace);
                            }
                            _message1 = _message1.Replace(_word, _replace);
                            _hasBadWord = true;
                        }
                    }
                    if (_hasBadWord)
                    {
                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _message1, _playerName, false, "ServerTools", false);
                        return false;
                    }
                }
                if (_message.StartsWith("player"))
                {

                }
                if (_message.StartsWith(Command_Private) || _message.StartsWith(Command_Public))
                {
                    bool _announce = false;
                    if (_message.StartsWith(Command_Public))
                    {
                        _announce = true;
                        _message = _message.Replace(Command_Public, "");
                    }
                    if (_message.StartsWith(Command_Private))
                    {
                        _message = _message.Replace(Command_Private, "");
                    }
                    if (_message.StartsWith("w ") || _message.StartsWith("W ") || _message.StartsWith("pm ") || _message.StartsWith("PM "))
                    {
                        if (CustomCommands.IsEnabled)
                        {
                            Whisper.Send(_cInfo, _message);
                            return false;
                        }
                    }
                    if (_message.StartsWith("r ") || _message.StartsWith("R ") || _message.StartsWith("RE ") || _message.StartsWith("re "))
                    {
                        if (CustomCommands.IsEnabled)
                        {
                            Whisper.Reply(_cInfo, _message);
                            return false;
                        }
                    }
                    _message = _message.ToLower();
                    if (_message == "sethome")
                    {
                        if (TeleportHome.IsEnabled)
                        {
                            if (!ZoneProtection.Set_Home)
                            {
                                if (!ZoneProtection.PvEFlag.Contains(_cInfo.entityId))
                                {
                                    if (_announce)
                                    {
                                        TeleportHome.SetHome(_cInfo, _playerName, _announce);
                                    }
                                    else
                                    {
                                        TeleportHome.SetHome(_cInfo, _playerName, _announce);
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (_announce)
                                    {
                                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}You can not use sethome in a protected zone.[-]", _message), _playerName, false, "ServerTools", true);
                                    }
                                    else
                                    {
                                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You can not use sethome in a protected zone.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (_announce)
                                {
                                    TeleportHome.SetHome(_cInfo, _playerName, _announce);
                                }
                                else
                                {
                                    TeleportHome.SetHome(_cInfo, _playerName, _announce);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (_announce)
                            {
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}Sethome is not enabled.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                            }
                            else
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Sethome is not enabled.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            }
                        }
                    }
                    if (_message == "home")
                    {
                        if (TeleportHome.IsEnabled)
                        {
                            if (_announce)
                            {
                                TeleportHome.TeleHome(_cInfo, _playerName, _announce);
                            }
                            else
                            {
                                TeleportHome.TeleHome(_cInfo, _playerName, _announce);
                                return false;
                            }
                        }
                        else
                        {
                            if (_announce)
                            {
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}Home is not enabled.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                            }
                            else
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Home is not enabled.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            }
                        }
                    }
                    if (_message == "delhome")
                    {
                        if (TeleportHome.IsEnabled)
                        {
                            if (_announce)
                            {
                                TeleportHome.DelHome(_cInfo, _playerName, _announce);
                            }
                            else
                            {
                                TeleportHome.DelHome(_cInfo, _playerName, _announce);
                                return false;
                            }
                        }
                        else
                        {
                            if (_announce)
                            {
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}Delhome is not enabled.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                            }
                            else
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Delhome is not enabled.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            }
                        }
                    }
                    if (_message == "sethome2")
                    {
                        if (TeleportHome.Set_Home2_Enabled & TeleportHome.Set_Home2_Donor_Only & ReservedSlots.IsEnabled)
                        {
                            if (!ZoneProtection.Set_Home)
                            {
                                if (!ZoneProtection.PvEFlag.Contains(_cInfo.entityId))
                                {
                                    if (ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                                    {
                                        DateTime _dt;
                                        ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                        if (DateTime.Now < _dt)
                                        {
                                            if (_announce)
                                            {
                                                TeleportHome.SetHome2(_cInfo, _playerName, _announce);
                                            }
                                            else
                                            {
                                                TeleportHome.SetHome2(_cInfo, _playerName, _announce);
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (_announce)
                                            {
                                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}Your reserved status has expired. Command is unavailable.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                            }
                                            else
                                            {
                                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Your reserved status has expired Command is unavailable..[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                                return false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (_announce)
                                        {
                                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}You are not listed as a reserved player. Command is unavailable.[-]", _message), _playerName, false, "ServerTools", true);
                                        }
                                        else
                                        {
                                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You are not listed as a reserved player. Command is unavailable.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (_announce)
                                    {
                                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}You can not use sethome2 in a protected zone.[-]", _message), _playerName, false, "ServerTools", true);
                                    }
                                    else
                                    {
                                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You can not use sethome2 in a protected zone.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                                {
                                    DateTime _dt;
                                    ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                    if (DateTime.Now < _dt)
                                    {
                                        if (_announce)
                                        {
                                            TeleportHome.SetHome2(_cInfo, _playerName, _announce);
                                        }
                                        else
                                        {
                                            TeleportHome.SetHome2(_cInfo, _playerName, _announce);
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        if (_announce)
                                        {
                                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}Your reserved status has expired. Command is unavailable.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                        }
                                        else
                                        {
                                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Your reserved status has expired Command is unavailable..[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (_announce)
                                    {
                                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}You are not listed as a reserved player. Command is unavailable.[-]", _message), _playerName, false, "ServerTools", true);
                                    }
                                    else
                                    {
                                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You are not listed as a reserved player. Command is unavailable.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                        return false;
                                    }
                                }
                            }
                        }
                        else if (TeleportHome.Set_Home2_Enabled && !TeleportHome.Set_Home2_Donor_Only)
                        {
                            if (!ZoneProtection.Set_Home)
                            {
                                if (!ZoneProtection.PvEFlag.Contains(_cInfo.entityId))
                                {
                                    TeleportHome.SetHome2(_cInfo, _playerName, _announce);
                                    return false;
                                }
                                else
                                {
                                    if (_announce)
                                    {
                                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}You can not use sethome2 in a protected zone.[-]", _message), _playerName, false, "ServerTools", true);
                                    }
                                    else
                                    {
                                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You can not use sethome2 in a protected zone.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                TeleportHome.SetHome2(_cInfo, _playerName, _announce);
                                return false;
                            }
                        }
                    }
                    if (_message == "home2")
                    {
                        if (TeleportHome.Set_Home2_Enabled && TeleportHome.Set_Home2_Donor_Only && ReservedSlots.IsEnabled)
                        {
                            if (ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    TeleportHome.TeleHome2(_cInfo, _playerName, _announce);
                                    return false;
                                }
                                else
                                {
                                    if (_announce)
                                    {
                                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}Your reserved status has expired. Command is unavailable.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                    }
                                    else
                                    {
                                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Your reserved status has expired. Command is unavailable.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (_announce)
                                {
                                    GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}You are not on the reserved list, please donate or contact an admin.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You are not on the reserved list, please donate or contact an admin.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                        }
                        else if (TeleportHome.Set_Home2_Enabled && !TeleportHome.Set_Home2_Donor_Only)
                        {
                            TeleportHome.TeleHome2(_cInfo, _playerName, _announce);
                            return false;
                        }
                        else
                        {
                            if (_announce)
                            {
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}Home2 is not enabled.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                            }
                            else
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Home2 is not enabled.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            }
                        }
                    }
                    if (_message == "delhome2")
                    {
                        if (TeleportHome.Set_Home2_Enabled && TeleportHome.Set_Home2_Donor_Only && ReservedSlots.IsEnabled)
                        {
                            if (ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    if (_announce)
                                    {
                                        TeleportHome.DelHome2(_cInfo, _playerName, _announce);
                                    }
                                    else
                                    {
                                        TeleportHome.DelHome2(_cInfo, _playerName, _announce);
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (_announce)
                                    {
                                        GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}Your reserved status has expired. Command is unavailable.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                    }
                                    else
                                    {
                                        _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Your reserved status has expired. Command is unavailable.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                    }
                                }
                            }
                            else
                            {
                                if (_announce)
                                {
                                    GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}You are not on the reserved list, please donate or contact an admin.[-]", Config.Chat_Response_Color), _playerName, false, "ServerTools", true);
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You are not on the reserved list, please donate or contact an admin.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                            }
                        }
                        else if (TeleportHome.Set_Home2_Enabled && !TeleportHome.Set_Home2_Donor_Only)
                        {
                            TeleportHome.DelHome2(_cInfo, _playerName, _announce);
                            return false;
                        }
                    }
                    if (AdminChat.IsEnabled)
                    {
                        if (_message.StartsWith("mute ") || _message.StartsWith("unmute "))
                        {
                            if (_message.StartsWith("mute "))
                            {
                                MutePlayer.Add(_cInfo, _message);
                            }
                            if (_message.StartsWith("unmute "))
                            {
                                MutePlayer.Remove(_cInfo, _message);
                            }
                            return false;
                        }
                    }
                    if (_message == "commands")
                    {
                        string _commands1 = CustomCommands.GetChatCommands1(_cInfo);
                        string _commands2 = CustomCommands.GetChatCommands2(_cInfo);
                        string _commandsCustom = CustomCommands.GetChatCommandsCustom(_cInfo);
                        string _commandsAdmin = CustomCommands.GetChatCommandsAdmin(_cInfo);
                        if (_announce)
                        {
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _commands1, "Server", false, "ServerTools", false);
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _commands2, "Server", false, "ServerTools", false);
                            if (CustomCommands.IsEnabled)
                            {
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _commandsCustom, "Server", false, "ServerTools", false);
                            }
                            AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfo.playerId);
                            if (Admin.PermissionLevel <= Admin_Level)
                            {
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, _commandsAdmin, "Server", false, "ServerTools", false);
                            }
                        }
                        else
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, _commands1, "Server", false, "ServerTools", false));
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, _commands2, "Server", false, "ServerTools", false));
                            if (CustomCommands.IsEnabled)
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, _commandsCustom, "Server", false, "ServerTools", false));
                            }
                            AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfo.playerId);
                            if (Admin.PermissionLevel <= Admin_Level)
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, _commandsAdmin, "Server", false, "ServerTools", false));
                            }                           
                        }
                        return false;
                    }
                    if (_message == "day7" || _message == "day")
                    {
                        if (Day7.IsEnabled)
                        {
                            if (_announce)
                            {
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                            }
                            Day7.GetInfo(_cInfo, _announce);
                            return false;
                        }
                    }
                    if (_message == "bloodmoon" || _message == "bm")
                    {
                        if (Bloodmoon.IsEnabled)
                        {
                            if (_announce)
                            {
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                            }
                            Bloodmoon.GetBloodmoon(_cInfo, _announce);
                            return false;
                        }
                    }
                    if (_message == "killme" || _message == "wrist" || _message == "suicide")
                    {
                        if (_announce)
                        {
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                        }
                        if (KillMe.IsEnabled)
                        {
                            KillMe.CheckPlayer(_cInfo, _announce);
                        }
                        else
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Killme is not enabled.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                        }
                        return false;
                    }
                    if (_message == "gimme" || _message == "gimmie")
                    {
                        if (_announce)
                        {
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                        }
                        if (Gimme.Always_Show_Response && !_announce)
                        {
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                        }
                        if (Gimme.IsEnabled)
                        {
                            Gimme.Checkplayer(_cInfo, _announce, _playerName);
                        }
                        else
                        {
                            return true;
                        }
                        return false;
                    }
                    if (_message == "setjail" || _message.StartsWith("jail ") || _message.StartsWith("unjail "))
                    {
                        if (Jail.IsEnabled)
                        {
                            if (_message == "setjail")
                            {
                                if (_announce)
                                {
                                    GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                                }
                                Jail.SetJail(_cInfo);
                                return false;
                            }
                            if (_message.StartsWith("jail "))
                            {
                                if (_announce)
                                {
                                    GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                                }
                                Jail.PutInJail(_cInfo, _message);
                                return false;
                            }
                            if (_message.StartsWith("unjail "))
                            {
                                if (_announce)
                                {
                                    GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                                }
                                Jail.RemoveFromJail(_cInfo, _message);
                                return false;
                            }
                        }
                    }
                    if (_message == "setspawn")
                    {
                        if (NewSpawnTele.IsEnabled)
                        {
                            if (_announce)
                            {
                                GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                            }
                            NewSpawnTele.SetNewSpawnTele(_cInfo);
                            return false;
                        }
                    }
                    if (_message == "trackanimal" || _message == "track")
                    {
                        if (_announce)
                        {
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                        }
                        if (Animals.Always_Show_Response && !_announce)
                        {
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}{1}", Command_Public, _message), _playerName, false, "ServerTools", true);
                        }
                        if (Animals.IsEnabled)
                        {
                            Animals.Checkplayer(_cInfo, _announce, _playerName);
                            return false;
                        }
                        {
                            return true;
                        }
                    }
                    if (_message == "claim")
                    {
                        if (FirstClaimBlock.IsEnabled)
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Checking your claim block status.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            FirstClaimBlock.firstClaim(_cInfo);
                            return false;
                        }
                        return true;
                    }
                    if (_message.StartsWith("clanadd") || _message == "clandel" || _message.StartsWith("claninvite") || _message == "clanaccept" || _message == "clandecline" || _message.StartsWith("clanremove") || _message.StartsWith("clanpromote") || _message.StartsWith("clandemote") || _message.StartsWith("clan") || _message == "clancommands")
                    {
                        if (ClanManager.IsEnabled)
                        {
                            if (_message == "clancommands")
                            {
                                ClanManager.GetChatCommands(_cInfo);
                                return false;
                            }
                            if (_message.StartsWith("clanadd"))
                            {
                                if (_message == "clanadd")
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Usage: /clanadd clanName[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                                else
                                {
                                    _message = _message.Replace("clanadd ", "");
                                    ClanManager.AddClan(_cInfo, _message);
                                }
                                return false;
                            }
                            if (_message == "clandel")
                            {
                                ClanManager.RemoveClan(_cInfo);
                                return false;
                            }
                            if (_message.StartsWith("claninvite"))
                            {
                                if (_message == "claninvite")
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Usage: /claninvite playerName[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                                else
                                {
                                    _message = _message.Replace("claninvite ", "");
                                    ClanManager.InviteMember(_cInfo, _message);
                                }
                                return false;
                            }
                            if (_message == "clanaccept")
                            {
                                ClanManager.InviteAccept(_cInfo);
                                return false;
                            }
                            if (_message == "clandecline")
                            {
                                ClanManager.InviteDecline(_cInfo);
                                return false;
                            }
                            if (_message.StartsWith("clanremove"))
                            {
                                if (_message == "clanremove")
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Usage: /clanremove playerName[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                                else
                                {
                                    _message = _message.Replace("clanremove ", "");
                                    ClanManager.RemoveMember(_cInfo, _message);
                                }
                                return false;
                            }
                            if (_message.StartsWith("clanpromote"))
                            {
                                if (_message == "clanpromote")
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Usage: /clanpromote playerName[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                                else
                                {
                                    _message = _message.Replace("clanpromote ", "");
                                    ClanManager.PromoteMember(_cInfo, _message);
                                }
                                return false;
                            }
                            if (_message.StartsWith("clandemote"))
                            {
                                if (_message == "clandemote")
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Usage: /clandemote playerName[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                                else
                                {
                                    _message = _message.Replace("clandemote ", "");
                                    ClanManager.DemoteMember(_cInfo, _message);
                                }
                                return false;
                            }
                            if (_message == "clanleave")
                            {
                                ClanManager.LeaveClan(_cInfo);
                                return false;
                            }
                            if (_message.StartsWith("clan"))
                            {
                                if (_message == "clan")
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Usage: /clan message[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                                else
                                {
                                    _message = _message.Replace("clan ", "");
                                    ClanManager.Clan(_cInfo, _message);
                                }
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    if (_message == "doncolor")
                    {
                        if (Donator_Name_Coloring && ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                        {
                            GameManager.Instance.adminTools.IsAdmin(_cInfo.playerId);
                            AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfo.playerId);
                            if (Admin.PermissionLevel <= Mod_Level)
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Sorry {1}, your chat color can not be changed as a moderator or administrator.[-]", Config.Chat_Response_Color, _playerName), "Server", false, "ServerTools", false));
                                    return false;
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expired on {2}. Moderators and Admins can not change their chat color.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                            if (Admin.PermissionLevel == Don_Level1)
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    int dl2 = Don_Level2;
                                    SdtdConsole.Instance.ExecuteSync(string.Format("admin add {0} {1}", _cInfo.entityId, dl2), _cInfo);
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your chat color has been switched.[-]", Config.Chat_Response_Color, _playerName), "Server", false, "ServerTools", false));
                                    return false;
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expired on {2}. Command is unavailable.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                            if (Admin.PermissionLevel == Don_Level2)
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    int dl3 = Don_Level3;
                                    SdtdConsole.Instance.ExecuteSync(string.Format("admin add {0} {1}", _cInfo.entityId, dl3), _cInfo);
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your chat color has been switched.[-]", Config.Chat_Response_Color, _playerName), "Server", false, "ServerTools", false));
                                    return false;
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expired on {2}. Command is unavailable.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                            if (Admin.PermissionLevel == Don_Level3)
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    SdtdConsole.Instance.ExecuteSync(string.Format("admin remove {0}", _cInfo.entityId), _cInfo);
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your chat color has been turned off.[-]", Config.Chat_Response_Color, _playerName), "Server", false, "ServerTools", false));
                                    return false;
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expired on {2}. Command is unavailable.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                            else
                            {
                                int dl1 = Don_Level1;
                                SdtdConsole.Instance.ExecuteSync(string.Format("admin add {0} {1}", _cInfo.entityId, Don_Level1), _cInfo);
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your chat color has been turned on.[-]", Config.Chat_Response_Color, _playerName), "Server", false, "ServerTools", false));
                                return false;
                            }
                        }
                        if (!Donator_Name_Coloring & ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You have donated {1}, but the command is unavailable.[-]", Config.Chat_Response_Color, _playerName), "Server", false, "ServerTools", false));
                            return true;
                        }
                        if (Donator_Name_Coloring & !ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You have not donated {1}. Command is unavailable.[-]", Config.Chat_Response_Color, _playerName), "Server", false, "ServerTools", false));
                            return true;
                        }
                        return true;
                    }
                    if (_message == "reserved")
                    {
                        if (Reserved_Check && ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                        {
                            GameManager.Instance.adminTools.IsAdmin(_cInfo.playerId);
                            AdminToolsClientInfo Admin = GameManager.Instance.adminTools.GetAdminToolsClientInfo(_cInfo.playerId);
                            if (Admin.PermissionLevel <= Mod_Level)
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expires on {2}.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expired on {2}.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                            if (Admin.PermissionLevel == Don_Level1)
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expires on {2}.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expired on {2}[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                            if (Admin.PermissionLevel == Don_Level2)
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expires on {2}.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expired on {2}.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                            if (Admin.PermissionLevel == Don_Level3)
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expires on {2}.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expired on {2}.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                            else
                            {
                                DateTime _dt;
                                ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                                if (DateTime.Now < _dt)
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expires on {2}.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your reserved status expired on {2}.[-]", Config.Chat_Response_Color, _playerName, _dt), "Server", false, "ServerTools", false));
                                    return false;
                                }
                            }
                        }
                        if (Reserved_Check && !ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                        {
                            GameManager.Instance.GameMessageServer(_cInfo, EnumGameMessages.Chat, string.Format("{0}You have not donated {1}. Expiration date unavailable.[-]", Config.Chat_Response_Color, _playerName), _playerName, false, "ServerTools", true);
                            return true;
                        }
                        return true;
                    }
                    if (_message == "spcolor")
                    {
                        if (Special_Player_Name_Coloring && SpecialPlayers.Contains(_cInfo.playerId) && !SpecialPlayersColorOff.Contains(_cInfo.playerId))
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your chat color has been turned off.[-]", Config.Chat_Response_Color, _playerName), "Server", false, "ServerTools", false));
                            SpecialPlayersColorOff.Add(_cInfo.playerId);
                            return false;
                        }
                        if (Special_Player_Name_Coloring && SpecialPlayers.Contains(_cInfo.playerId) && SpecialPlayersColorOff.Contains(_cInfo.playerId))
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}, your chat color has been turned on.[-]", Config.Chat_Response_Color, _playerName), "Server", false, "ServerTools", false));
                            SpecialPlayersColorOff.Remove(_cInfo.playerId);
                            return false;
                        }
                        return true;
                    }
                    if (_message == "reward")
                    {
                        if (VoteReward.IsEnabled)
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Checking for your vote.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            VoteReward.CheckReward(_cInfo);
                            return false;
                        }
                        return true;
                    }
                    if (_message == "shutdown")
                    {
                        if (AutoShutdown.IsEnabled)
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Checking for the next shutdown time.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            AutoShutdown.CheckNextShutdown(_cInfo, _announce, _playerName);
                            return false;
                        }
                        return true;
                    }
                    if (_message == "admin")
                    {
                        if (AdminList.IsEnabled)
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Listing online administrators and moderators.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            AdminList.List(_cInfo, _announce, _playerName);
                            return false;
                        }
                        return true;
                    }
                    if (_message == "travel")
                    {
                        if (Travel.IsEnabled)
                        {
                            Travel.Check(_cInfo, _announce, _playerName);
                            return false;
                        }
                    }
                    if (_message == "return")
                    {
                        if (ZoneProtection.IsEnabled & ZoneProtection.Victim.ContainsKey(_cInfo.entityId))
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Sending you to your death point.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            ZoneProtection.ReturnToPosition(_cInfo);
                            return false;
                        }
                    }
                    if (_message == "forgive")
                    {
                        if (ZoneProtection.IsEnabled & Jail.IsEnabled & ZoneProtection.Forgive.ContainsKey(_cInfo.entityId))
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Your killer has been forgiven.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            Jail.Forgive(_cInfo);
                            return false;
                        }
                    }
                    if (_message == "weather")
                    {
                        if (WeatherVote.IsEnabled)
                        {
                            if (!WeatherVote.VoteClosed)
                            {
                                if (!WeatherVote.VoteOpen)
                                {
                                    WeatherVote.CallForVote1();
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}A weather vote has already begun.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                            }
                            else
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}A weather vote can only begin every {1} minutes.[-]", Config.Chat_Response_Color, WeatherVote.Vote_Delay), "Server", false, "ServerTools", false));
                            }
                            return false;
                        }  
                    }
                    if (_message == "clear")
                    {
                        if (WeatherVote.IsEnabled)
                        {
                            if (WeatherVote.VoteOpen)
                            {
                                if (!WeatherVote.clear.Contains(_cInfo.entityId))
                                {
                                    WeatherVote.clear.Add(_cInfo.entityId);
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Vote cast for {1}.[-]", Config.Chat_Response_Color, _message), "Server", false, "ServerTools", false));
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You have already voted.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                            }
                            else
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}There is no active weather vote. Type /weather in chat to open a new vote.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            }
                            return false;
                        }
                    }
                    if (_message == "rain")
                    {
                        if (WeatherVote.IsEnabled)
                        {
                            if (WeatherVote.VoteOpen)
                            {
                                if (!WeatherVote.rain.Contains(_cInfo.entityId))
                                {
                                    WeatherVote.rain.Add(_cInfo.entityId);
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Vote cast for {1}.[-]", Config.Chat_Response_Color, _message), "Server", false, "ServerTools", false));
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You have already voted.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                            }
                            else
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}There is no active weather vote. Type /weather in chat to open a new vote.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            }
                            return false;
                        }
                    }
                    if (_message == "snow")
                    {
                        if (WeatherVote.IsEnabled)
                        {
                            if (WeatherVote.VoteOpen)
                            {
                                if (!WeatherVote.snow.Contains(_cInfo.entityId))
                                {
                                    WeatherVote.snow.Add(_cInfo.entityId);
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Vote cast for {1}.[-]", Config.Chat_Response_Color, _message), "Server", false, "ServerTools", false));
                                }
                                else
                                {
                                    _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}You have already voted.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                }
                            }
                            else
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}There is no active weather vote. Type /weather in chat to open a new vote.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            }
                            return false;
                        }
                    }
                    if (_message == "wallet")
                    {
                        if (Shop.IsEnabled)
                        {
                            Wallet.WalletCheck(_cInfo, _playerName);
                            return false;
                        }
                    }
                    if (_message == "shop")
                    {
                        if (Shop.IsEnabled)
                        {
                            Shop.List(_cInfo, _playerName);
                            return false;
                        }
                    }
                    if (_message.StartsWith("buy"))
                    {
                        if (Shop.IsEnabled)
                        {
                            if (_message == "buy")
                            {
                                _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Usage: /buy #[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                            }
                            else
                            {
                                _message = _message.Replace("buy ", "");
                                Shop.Walletcheck(_cInfo, _message, _playerName);
                            }
                            return false;
                        }
                    }
                    if (_message.StartsWith("friend"))
                    {
                        if (FriendTeleport.IsEnabled)
                        {
                            if (_message == "friend")
                            {
                                FriendTeleport.ListFriends(_cInfo, _message);
                            }
                            else
                            {
                                _message = _message.Replace("friend ", "");
                                FriendTeleport.CheckDelay(_cInfo, _message, _announce);
                            }
                            return false;
                        }
                    }
                    if (_message == ("accept"))
                    {
                        if (FriendTeleport.IsEnabled)
                        {
                            if (FriendTeleport.Dict.ContainsKey(_cInfo.entityId))
                            {
                                int _dictValue;
                                if (FriendTeleport.Dict.TryGetValue(_cInfo.entityId, out _dictValue))
                                {
                                    DateTime _dict1Value;
                                    if (FriendTeleport.Dict1.TryGetValue(_cInfo.entityId, out _dict1Value))
                                    {
                                        TimeSpan varTime = DateTime.Now - _dict1Value;
                                        double fractionalSeconds = varTime.TotalSeconds;
                                        int _timepassed = (int)fractionalSeconds;
                                        if (_timepassed <= 30)
                                        {
                                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Your friends teleport request was accepted.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                            FriendTeleport.TeleFriend(_cInfo, _dictValue);
                                            FriendTeleport.Dict.Remove(_cInfo.entityId);
                                            FriendTeleport.Dict1.Remove(_cInfo.entityId);
                                        }
                                        else
                                        {
                                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}Your friends teleport request has expired.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                                            FriendTeleport.Dict.Remove(_cInfo.entityId);
                                            FriendTeleport.Dict1.Remove(_cInfo.entityId);
                                        }
                                    }
                                }
                                return false;
                            }
                        }
                    }
                    if (CustomCommands.IsEnabled && CustomCommands.Dict.ContainsKey(_message))
                    {
                        CustomCommands.CheckCustomDelay(_cInfo, _message, _playerName, _announce);
                        return false;
                    }
                }
                if (_message.StartsWith("@"))
                {
                    if (_message.StartsWith("@admins ") || _message.StartsWith("@ADMINS "))
                    {
                        if (!AdminChat.IsEnabled)
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}AdminChat is not enabled.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                        }
                        else
                        {
                            AdminChat.SendAdmins(_cInfo, _message);
                        }
                        return false;
                    }
                    if (_message.StartsWith("@all ") || _message.StartsWith("@ALL "))
                    {
                        if (!AdminChat.IsEnabled)
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}AdminChat is not enabled.[-]", Config.Chat_Response_Color), "Server", false, "ServerTools", false));
                        }
                        else
                        {
                            AdminChat.SendAll(_cInfo, _message);
                        }
                        return false;
                    }
                }
            }
            return true;
        }
    }
}