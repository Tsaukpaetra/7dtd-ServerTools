﻿

namespace ServerTools
{
    class BreakTime
    {
        public static bool IsEnabled = false;
        public static int Delay = 60;
        public static string Break_Message = "It has been {Time} minutes since the last break reminder. Stretch and get some water.";

        public static void Exec()
        {
            if (ConnectionManager.Instance.ClientCount() > 0)
            {
                Break_Message = Break_Message.Replace("{Time}", Delay.ToString());
                ChatHook.ChatMessage(null, Config.Chat_Response_Color + Break_Message + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
            }
        }
    }
}
