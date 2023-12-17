using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCarter.Model
{
    public class Toolscript : SingletonBase<Toolscript>
    {
        public delegate void PopNotificationDelegate(string title, string msg);
        public static event PopNotificationDelegate OnRaisePopNotification;
        public Dictionary<string, List<ScriptItemtype>> GetdefaultToolScriptItem()
        {
            Dictionary<string, List<ScriptItemtype>> CategoryItems = new Dictionary<string, List<ScriptItemtype>>();
            List<ScriptItemtype> InformationItems = new List<ScriptItemtype>();
            List<ScriptItemtype> DiagnosticItems = new List<ScriptItemtype>();
            List<ScriptItemtype> BrightnessItems = new List<ScriptItemtype>();
            List<ScriptItemtype> PowermodesItems = new List<ScriptItemtype>();
            List<ScriptItemtype> PatternItems = new List<ScriptItemtype>();
            List<ScriptItemtype> CurrentItems = new List<ScriptItemtype>();
            List<ScriptItemtype> TouchItems = new List<ScriptItemtype>();
            List<ScriptItemtype> CustomerItems = new List<ScriptItemtype>();

            #region InformationItems
            InformationItems.Add(new ScriptItemtype()
            {
                Nodename = "HWversion",
                MSGname = "HWversion",
                Command = "FA 52 01 00 01",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            InformationItems.Add(new ScriptItemtype()
            {
                Nodename = "SWversion",
                MSGname = "SWversion",
                Command = "FA 52 01 00 02",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            CategoryItems.Add("Information", InformationItems);
            #endregion

            #region DiagnosticItems
            DiagnosticItems.Add(new ScriptItemtype()
            {
                Nodename = "ReadDiagnostic",
                MSGname = "ReadDiagnostic",
                Command = "FA 52 01 04 01",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            DiagnosticItems.Add(new ScriptItemtype()
            {
                Nodename = "ReadInputVoltage",
                MSGname = "ReadInputVoltage",
                Command = "FA 52 01 04 02",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            DiagnosticItems.Add(new ScriptItemtype()
            {
                Nodename = "Read_temp",
                MSGname = "Read_temp",
                Command = "FA 52 01 04 03",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            DiagnosticItems.Add(new ScriptItemtype()
            {
                Nodename = "T_chamber_temp",
                MSGname = "T_chamber_temp",
                Command = "FA 52 01 04 04",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            DiagnosticItems.Add(new ScriptItemtype()
            {
                Nodename = "display_brightness_ADC",
                MSGname = "display_brightness_ADC",
                Command = "FA 52 01 04 05",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            CategoryItems.Add("Diagnostic", DiagnosticItems);
            #endregion

            #region BrightnessItems
            BrightnessItems.Add(new ScriptItemtype()
            {
                Nodename = "WriteBrightness",
                MSGname = "WriteBrightness",
                Command = "FA 57 01 01 00",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            BrightnessItems.Add(new ScriptItemtype()
            {
                Nodename = "ReadBrightness",
                MSGname = "ReadBrightness",
                Command = "FA 52 01 01 01",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            CategoryItems.Add("Brightness", BrightnessItems);
            #endregion

            #region PowermodesItems
            PowermodesItems.Add(new ScriptItemtype()
            {
                Nodename = "Sleep",
                MSGname = "Sleep",
                Command = "FA 57 00 02 00",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            PowermodesItems.Add(new ScriptItemtype()
            {
                Nodename = "Wakeup",
                MSGname = "Wakeup",
                Command = "FA 57 00 02 01",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });

            PowermodesItems.Add(new ScriptItemtype()
            {
                Nodename = "FIDM_Poweroff",
                MSGname = "FIDM_Poweroff",
                Command = "FA 57 00 02 04",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            CategoryItems.Add("PowerModes", PowermodesItems);
            #endregion

            #region PatternItems
            PatternItems.Add(new ScriptItemtype()
            {
                Nodename = "Auto_scrolling",
                MSGname = "Pattern_Auto_scroll",
                Command = "FA 57 01 03 00",

                Delaytime = 200
            });
            PatternItems.Add(new ScriptItemtype()
            {
                Nodename = "White",
                MSGname = "Pattern_White",
                Command = "FA 57 01 03 01",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            PatternItems.Add(new ScriptItemtype()
            {
                Nodename = "Black",
                MSGname = "Pattern_Black",
                Command = "FA 57 01 03 02",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            PatternItems.Add(new ScriptItemtype()
            {
                Nodename = "Red",
                MSGname = "Pattern_Red",
                Command = "FA 57 01 03 03",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            PatternItems.Add(new ScriptItemtype()
            {
                Nodename = "Green",
                MSGname = "Pattern_Green",
                Command = "FA 57 01 03 04",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            PatternItems.Add(new ScriptItemtype()
            {
                Nodename = "Blue",
                MSGname = "Pattern_Blue",
                Command = "FA 57 01 03 05",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            PatternItems.Add(new ScriptItemtype()
            {
                Nodename = "Grey",
                MSGname = "Pattern_Grey",
                Command = "FA 57 01 03 06",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            PatternItems.Add(new ScriptItemtype()
            {
                Nodename = "checker",
                MSGname = "Pattern_checker",
                Command = "FA 57 01 03 07",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            CategoryItems.Add("Pattern", PatternItems);
            #endregion

            #region ReadCurrentItems
            CurrentItems.Add(new ScriptItemtype()
            {
                Nodename = "current",
                MSGname = "TXB1send_curr",
                Command = "FA 52 01 05 01",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            CategoryItems.Add("Current", CurrentItems);
            #endregion

            #region TouchItems
            TouchItems.Add(new ScriptItemtype()
            {
                Nodename = "StopsTriggeringTP",
                MSGname = "StopsTriggeringTP",
                Command = "FA 57 00 06 00",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            TouchItems.Add(new ScriptItemtype()
            {
                Nodename = "TXB1StartTriggerTP",
                MSGname = "TXB1StartTriggerTP",
                Command = "FA 57 00 06 01",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            TouchItems.Add(new ScriptItemtype()
            {
                Nodename = "ReadTouchinfo",
                MSGname = "ReadTouchinfo",
                Command = "FA 52 01 06 01",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            CategoryItems.Add("Touch", TouchItems);
            #endregion

            #region CustomerItems
            CustomerItems.Add(new ScriptItemtype()
            {
                Nodename = "default_hex",
                MSGname = "default_hex",
                Command = "",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            CustomerItems.Add(new ScriptItemtype()
            {
                Nodename = "default_tag",
                MSGname = "default_tag",
                Command = "",
                RecCommand = "",
                HashCodevalue = "",
                Delaytime = 200
            });
            CategoryItems.Add("CustomerItems", CustomerItems);
            #endregion

            return CategoryItems;
        }
    }
}
