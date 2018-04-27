using System;
using System.Collections.Generic;
using System.Text;
using OPMedia.UI.Configuration;
using OPMedia.Core;

using System.Windows.Forms;
using OPMedia.Core.TranslationSupport;
using OPMedia.Runtime.Addons.Configuration;
using OPMedia.UI.ProTONE.Configuration.MiscConfig;
using OPMedia.Runtime.ProTONE.Configuration;
using OPMedia.Core.Configuration;
using OPMedia.UI.ProTONE.Configuration.InternetConfig;

namespace OPMedia.UI.ProTONE.Configuration
{
    public class ProTONESettingsForm : SettingsForm
    {


        public new static DialogResult Show()
        {
            ProTONESettingsForm _instance = new ProTONESettingsForm();
            return _instance.ShowDialog();
        }

        public static DialogResult Show(string titleToOpen, string subTitleToOpen = "")
        {
            ProTONESettingsForm _instance = new ProTONESettingsForm(titleToOpen, subTitleToOpen);
            return _instance.ShowDialog();
        }

        protected ProTONESettingsForm(string titleToOpen, string subTitleToOpen) 
            : base(titleToOpen, subTitleToOpen)
        {
        }

        public ProTONESettingsForm() : base()
        {
        }
        
        public override void AddAditionalPanels()
        {
            if (ProTONEConfig.IsPlayer)
            {
                AddPanel(typeof(FileTypesPanel), AppConfig.CurrentUserIsAdministrator);
            }
            else if (ProTONEConfig.IsMediaLibrary)
            {
                AddPanel(typeof(AddonCfgPanel));
                AddPanel(typeof(AddonSettingsPanel));
            }

            AddPanel(typeof(SubtitleSettingsPanel));

            if (ProTONEConfig.IsPlayer)
            {
                if (!AppConfig.CurrentUserIsAdministrator)
                {
                    MessageDisplay.Show(Translator.Translate("TXT_PANELSHIDDEN_NOADMIN"),
                        Translator.Translate("TXT_CAUTION"), MessageBoxIcon.Exclamation);
                }

                AddPanel(typeof(MiscellaneousSettingsPanel));
            }

            AddPanel(typeof(ControlAppPanel));
        }

        public override bool RequiresInternetConfig()
        {
            return true;
        }

        public override List<BaseCfgPanel> GetControlSubPages()
        {
            if (!AppConfig.CurrentUserIsAdministrator || !ProTONEConfig.IsRCCServiceInstalled || !ProTONEConfig.IsPlayer)
                return null;

            return new List<BaseCfgPanel> 
            { 
                new RemoteControlPage() 
            };
        }

        public override List<BaseCfgPanel> GetTroubleshootingSubPages()
        {
            return new List<BaseCfgPanel> 
            { 
                new DiagnosticsPage() 
            };
        }

        public override List<BaseCfgPanel> GetInternetSubPages()
        {
            if (!ProTONEConfig.IsPlayer)
                return null;

            return new List<BaseCfgPanel> 
            { 
                new ShoutcastConfigPage(),
                new DeezerConfigPage(),
            };
        }
    }
}
