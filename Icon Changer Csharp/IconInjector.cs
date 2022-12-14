using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using iconChanger;
using System.Windows.Forms;

namespace DSStdInstallerCompiler
{
    class IconInjector
    {
       

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int UpdateResource(IntPtr hUpdate, uint lpType, uint lpName, ushort wLanguage, byte[] lpData, uint cbData);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr BeginUpdateResource(string pFileName,
            [MarshalAs(UnmanagedType.Bool)]bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        public static void InjectIcon(string execFileName, string iconFileName,ProgressBar Prog)
        {
            InjectIcon(execFileName, iconFileName, 1, 1,Prog);
        }
        static void InjectIcon(string execFileName, string iconFileName, uint iconGroupID, uint iconBaseID,ProgressBar Progressb)
        {
            const uint RT_ICON = 3;
            const uint RT_GROUP_ICON = 14;

            IconFile iconFile = new IconFile();
            iconFile.Load(iconFileName);


            IntPtr hUpdate = BeginUpdateResource(execFileName, false);
            Debug.Assert(hUpdate != IntPtr.Zero);


            byte[] data = iconFile.CreateIconGroupData(iconBaseID);
            UpdateResource(hUpdate, RT_GROUP_ICON, iconGroupID, 0, data, (uint)data.Length);
          
            Progressb.Maximum = iconFile.GetImageCount();
        
            for (int i = 0; i < iconFile.GetImageCount(); i++)
            {
                Progressb.Value = i+1;
                byte[] image = iconFile.GetImageData(i);
                UpdateResource(hUpdate, RT_ICON, (uint)(iconBaseID + i), 0, image, (uint)image.Length);
            }


            EndUpdateResource(hUpdate, false);

        }
    }


}
