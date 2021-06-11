﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace ServerTools
{
    class ProtectedSpaces
    {
        public static bool IsEnabled = false, IsRunning = false;
        public static List<int[]> Protected = new List<int[]>();
        public static Dictionary<int, int[]> Vectors = new Dictionary<int, int[]>();
        private const string file = "ProtectedSpaces.xml";
        private static string filePath = string.Format("{0}/{1}", API.ConfigPath, file);
        private static FileSystemWatcher FileWatcher = new FileSystemWatcher(API.ConfigPath, file);

        public static void Load()
        {
            if (IsEnabled && !IsRunning)
            {
                LoadXml();
                InitFileWatcher();
            }
        }

        public static void Unload()
        {
            if (!IsEnabled && IsRunning)
            {
                FileWatcher.Dispose();
                IsRunning = false;
            }
        }

        public static void LoadXml()
        {
            try
            {
                if (!Utils.FileExists(filePath))
                {
                    UpdateXml();
                }
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(filePath);
                }
                catch (XmlException e)
                {
                    Log.Error(string.Format("[SERVERTOOLS] Failed loading {0}: {1}", file, e.Message));
                    return;
                }
                XmlNode _XmlNode = xmlDoc.DocumentElement;
                foreach (XmlNode childNode in _XmlNode.ChildNodes)
                {
                    if (childNode.Name == "Spaces")
                    {
                        Protected.Clear();
                        Vectors.Clear();
                        foreach (XmlNode subChild in childNode.ChildNodes)
                        {
                            if (subChild.NodeType == XmlNodeType.Comment)
                            {
                                continue;
                            }
                            if (subChild.NodeType != XmlNodeType.Element)
                            {
                                Log.Warning(string.Format("[SERVERTOOLS] Unexpected XML node found in 'Spaces' section: {0}", subChild.OuterXml));
                                continue;
                            }
                            XmlElement _line = (XmlElement)subChild;
                            if (!_line.HasAttribute("Corner1"))
                            {
                                Log.Warning(string.Format("[SERVERTOOLS] Ignoring protected space entry because of missing Corner1 attribute: {0}", subChild.OuterXml));
                                continue;
                            }
                            if (!_line.HasAttribute("Corner2"))
                            {
                                Log.Warning(string.Format("[SERVERTOOLS] Ignoring protected space entry because of missing Corner2 attribute: {0}", subChild.OuterXml));
                                continue;
                            }
                            string corner1 = _line.GetAttribute("Corner1");
                            string corner2 = _line.GetAttribute("Corner2");
                            if (!corner1.Contains(","))
                            {
                                Log.Warning(string.Format("[SERVERTOOLS] Ignoring protected space entry because of invalid form missing comma attribute: {0}", subChild.OuterXml));
                                continue;
                            }
                            if (!corner2.Contains(","))
                            {
                                Log.Warning(string.Format("[SERVERTOOLS] Ignoring protected space entry because of invalid form missing comma attribute: {0}", subChild.OuterXml));
                                continue;
                            }
                            if (!_line.HasAttribute("Active"))
                            {
                                Log.Warning(string.Format("[SERVERTOOLS] Ignoring protected space entry because of missing Active attribute: {0}", subChild.OuterXml));
                                continue;
                            }
                            if (!bool.TryParse(_line.GetAttribute("Active"), out bool _isActive))
                            {
                                Log.Warning(string.Format("[SERVERTOOLS] Ignoring protected space entry because improper True/False for Active attribute: {0}.", subChild.OuterXml));
                                continue;
                            }
                            string[] _corner1Split = corner1.Split(',');
                            string[] _corner2Split = corner2.Split(',');
                            int.TryParse(_corner1Split[0], out int _corner1_x);
                            int.TryParse(_corner1Split[1], out int _corner1_z);
                            int.TryParse(_corner2Split[0], out int _corner2_x);
                            int.TryParse(_corner2Split[1], out int _corner2_z);
                            int[] _vectors = new int[5];
                            if (_corner1_x < _corner2_x)
                            {
                                _vectors[0] = _corner1_x;
                                _vectors[2] = _corner2_x;
                            }
                            else
                            {
                                _vectors[0] = _corner2_x;
                                _vectors[2] = _corner1_x;
                            }
                            if (_corner1_z < _corner2_z)
                            {
                                _vectors[1] = _corner1_z;
                                _vectors[3] = _corner2_z;
                            }
                            else
                            {
                                _vectors[1] = _corner2_z;
                                _vectors[3] = _corner1_z;
                            }
                            if (_isActive)
                            {
                                _vectors[4] = 1;
                            }
                            else
                            {
                                _vectors[4] = 0;
                            }
                            if (!Protected.Contains(_vectors))
                            {
                                Protected.Add(_vectors);
                                if (_vectors[4] == 1)
                                {
                                    AddProtection(_vectors);
                                }
                                else
                                {
                                    RemoveProtection(_vectors);
                                }
                            }
                            else if (_vectors[4] == 1)
                            {
                                AddProtection(_vectors);
                            }
                            else
                            {
                                RemoveProtection(_vectors);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in ProtectedSpaces.LoadXml: {0}", e.Message));
            }
        }

        public static void UpdateXml()
        {
            FileWatcher.EnableRaisingEvents = false;
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<Protected>");
                sw.WriteLine("    <Spaces>");
                if (Protected.Count > 0)
                {
                    for (int i = 0; i < Protected.Count; i++)
                    {
                        if (Protected[i][4] == 1)
                        {
                            sw.WriteLine(string.Format("        <Protected Corner1=\"{0},{1}\" Corner2=\"{2},{3}\" Active=\"True\" />", Protected[i][0], Protected[i][1], Protected[i][2], Protected[i][3]));
                        }
                        else
                        {
                            sw.WriteLine(string.Format("        <Protected Corner1=\"{0},{1}\" Corner2=\"{2},{3}\" Active=\"False\" />", Protected[i][0], Protected[i][1], Protected[i][2], Protected[i][3]));
                        }
                    }
                }
                else
                {
                    sw.WriteLine("        <!-- <Protected Corner1=\"-30,-20\" Corner2=\"10,50\" Active=\"True\" /> -->");
                    sw.WriteLine("        <!-- <Protected Corner1=\"-800,75\" Corner2=\"-300,100\" Active=\"True\" /> -->");
                    sw.WriteLine("        <!-- <Protected Corner1=\"-50,-600\" Corner2=\"-5,-550\" Active=\"True\" /> -->");
                }
                sw.WriteLine("    </Spaces>");
                sw.WriteLine("</Protected>");
                sw.Flush();
                sw.Close();
            }
            FileWatcher.EnableRaisingEvents = true;
        }

        private static void InitFileWatcher()
        {
            FileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            FileWatcher.Created += new FileSystemEventHandler(OnFileChanged);
            FileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            FileWatcher.EnableRaisingEvents = true;
            IsRunning = true;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (!Utils.FileExists(filePath))
            {
                UpdateXml();
            }
            LoadXml();
        }

        public static void AddProtection(int[] _vectors)
        {
            try
            {
                if (_vectors != null && _vectors.Length == 5)
                {
                    List<int[]> _loadedLocations = new List<int[]>();
                    List<int[]> _unloadedLocations = new List<int[]>();
                    for (int i = _vectors[0]; i <= _vectors[2]; i++)
                    {
                        for (int j = _vectors[1]; j <= _vectors[3]; j++)
                        {
                            if (!GameManager.Instance.World.IsWithinTraderArea(new Vector3i(i, 1, j)))
                            {
                                if (GameManager.Instance.World.IsChunkAreaLoaded(i, 1, j))
                                {

                                    _loadedLocations.Add(new int[] { i, j });
                                }
                                else
                                {
                                    _unloadedLocations.Add(new int[] { i, j });
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    if (_loadedLocations.Count > 0)
                    {
                        List<Chunk> _chunkList = new List<Chunk>();
                        for (int i = 0; i < _loadedLocations.Count; i++)
                        {
                            Chunk _chunk = GameManager.Instance.World.GetChunkFromWorldPos(_loadedLocations[i][0], 1, _loadedLocations[i][1]) as Chunk;
                            if (!_chunkList.Contains(_chunk))
                            {
                                _chunkList.Add(_chunk);
                            }
                            Bounds bounds = _chunk.GetAABB();
                            int _x = _loadedLocations[i][0] - (int)bounds.min.x, _z = _loadedLocations[i][1] - (int)bounds.min.z;
                            _chunk.SetTraderArea(_x, _z, true);
                        }
                        if (_chunkList.Count > 0)
                        {
                            for (int i = 0; i < _chunkList.Count; i++)
                            {
                                Chunk _chunk = _chunkList[i];
                                List<ClientInfo> _clientList = PersistentOperations.ClientList();
                                if (_clientList != null && _clientList.Count > 0)
                                {
                                    for (int j = 0; j < _clientList.Count; j++)
                                    {
                                        ClientInfo _cInfo = _clientList[j];
                                        if (_cInfo != null)
                                        {
                                            _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChunk>().Setup(_chunk, true));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (_unloadedLocations.Count > 0)
                    {
                        for (int i = 0; i < _unloadedLocations.Count; i++)
                        {
                            if (GameManager.Instance.World.GetChunkSync(_unloadedLocations[i][0], _unloadedLocations[i][1]) is Chunk)
                            {
                                Chunk _chunk = GameManager.Instance.World.GetChunkSync(_unloadedLocations[i][0], 1, _unloadedLocations[i][1]) as Chunk;
                                Bounds bounds = _chunk.GetAABB();
                                int _x = _unloadedLocations[i][0] - (int)bounds.min.x, _z = _unloadedLocations[i][1] - (int)bounds.min.z;
                                _chunk.SetTraderArea(_x, _z, true);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in ProtectedSpaces.AddProtection: {0}", e.Message));
            }
        }

        public static void RemoveProtection(int[] _vectors)
        {
            try
            {
                if (_vectors != null && _vectors.Length == 5)
                {
                    List<int[]> _loadedLocations = new List<int[]>();
                    List<int[]> _unloadedLocations = new List<int[]>();
                    for (int i = _vectors[0]; i <= _vectors[2]; i++)
                    {
                        for (int j = _vectors[1]; j <= _vectors[3]; j++)
                        {
                            if (GameManager.Instance.World.IsChunkAreaLoaded(i, 1, j))
                            {
                                _loadedLocations.Add(new int[] { i, j });
                            }
                            else
                            {
                                _unloadedLocations.Add(new int[] { i, j });
                            }
                        }
                    }
                    if (_loadedLocations.Count > 0)
                    {
                        List<Chunk> _chunkList = new List<Chunk>();
                        for (int i = 0; i < _loadedLocations.Count; i++)
                        {
                            Chunk _chunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(_loadedLocations[i][0], 1, _loadedLocations[i][1]);
                            if (!_chunkList.Contains(_chunk))
                            {
                                _chunkList.Add(_chunk);
                            }
                            Bounds bounds = _chunk.GetAABB();
                            int _x = _loadedLocations[i][0] - (int)bounds.min.x, _z = _loadedLocations[i][1] - (int)bounds.min.z;
                            _chunk.SetTraderArea(_x, _z, false);
                        }
                        if (_chunkList.Count > 0)
                        {
                            for (int i = 0; i < _chunkList.Count; i++)
                            {
                                Chunk _chunk = _chunkList[i];
                                List<ClientInfo> _clientList = PersistentOperations.ClientList();
                                if (_clientList != null && _clientList.Count > 0)
                                {
                                    for (int j = 0; j < _clientList.Count; j++)
                                    {
                                        ClientInfo _cInfo = _clientList[j];
                                        if (_cInfo != null)
                                        {
                                            _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChunk>().Setup(_chunk, true));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (_unloadedLocations.Count > 0)
                    {
                        for (int i = 0; i < _unloadedLocations.Count; i++)
                        {
                            if (GameManager.Instance.World.GetChunkSync(_unloadedLocations[i][0], _unloadedLocations[i][1]) is Chunk)
                            {
                                Chunk _chunk = GameManager.Instance.World.GetChunkSync(_unloadedLocations[i][0], 1, _unloadedLocations[i][1]) as Chunk;
                                Bounds bounds = _chunk.GetAABB();
                                int _x = _unloadedLocations[i][0] - (int)bounds.min.x, _z = _unloadedLocations[i][1] - (int)bounds.min.z;
                                _chunk.SetTraderArea(_x, _z, false);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in ProtectedSpaces.RemoveProtection: {0}", e.Message));
            }
        }
    }
}
