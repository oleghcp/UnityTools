#if INCLUDE_UNITY_UI
using System;
using System.Globalization;
using OlegHcp.CSharp.Collections;

namespace OlegHcp.GameConsole
{
    public static class CommandsHelper
    {
        public static bool ParseOnOff(string[] opt, Action<bool> func)
        {
            const string message = "options: [on|off]";

            if (opt.IsNullOrEmpty())
            {
                Terminal.I.WriteCmdError(message);
                return false;
            }

            switch (opt[0])
            {
                case "on":
                    func(true);
                    return true;

                case "off":
                    func(false);
                    return true;

                default:
                    Terminal.I.WriteCmdError(message);
                    return false;
            }
        }

        public static bool ParseInt(string[] opt, Action<int> func)
        {
            if (opt.HasAnyData() && int.TryParse(opt[0], out int parced))
            {
                func(parced);
                return true;
            }

            Terminal.I.WriteCmdError("options: [amount]");
            return false;
        }

        public static bool ParseFloat(string[] opt, Action<float> func)
        {
            if (opt.HasAnyData() && float.TryParse(opt[0], NumberStyles.Number, CultureInfo.InvariantCulture, out float parced))
            {
                func(parced);
                return true;
            }

            Terminal.I.WriteCmdError("options: [value]");
            return false;
        }

        public static bool ParseEnum<T>(string[] opt, Action<T> func) where T : struct, Enum, IConvertible
        {
            if (opt.HasAnyData() && Enum.TryParse(opt[0], true, out T enm))
            {
                func(enm);
                return true;
            }

            Terminal.I.WriteCmdError("options: [type]");
            return false;
        }
    }
}
#endif
