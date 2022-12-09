using DSStdInstallerCompiler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace iconChanger
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ICONDIR
    {
        public ushort idReserved;
        public ushort idType;
        public ushort idCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ICONDIRENTRY
    {
        public byte bWidth;
        public byte bHeight;
        public byte bColorCount;
        public byte bReserved;
        public ushort wPlanes;
        public ushort wBitCount;
        public uint dwBytesInRes;
        public uint dwImageOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct GRPICONDIRENTRY
    {
        public byte bWidth;
        public byte bHeight;
        public byte bColorCount;
        public byte bReserved;
        public ushort wPlanes;
        public ushort wBitCount;
        public uint dwBytesInRes;
        public ushort nID;
    }

    public class IconFile
    {
        ICONDIR _iconDir = new ICONDIR();
        ArrayList _iconEntry = new ArrayList();
        ArrayList _iconImage = new ArrayList();

        public IconFile()
        {
        }


        public int GetImageCount()
        {
            return _iconDir.idCount;
        }

        
        public byte[] GetImageData(int index)
        {
            Debug.Assert(0 <= index && index < GetImageCount());
            return (byte[])_iconImage[index];
        }


        public unsafe void Load(string fileName)
        {
            FileStream fs = null;
            BinaryReader br = null;
            byte[] buffer = null;

            try
            {
      
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);

              
                buffer = br.ReadBytes(sizeof(ICONDIR));
                fixed (ICONDIR* ptr = &_iconDir)
                {
                    Marshal.Copy(buffer, 0, (IntPtr)ptr, sizeof(ICONDIR));
                }

         
                Debug.Assert(_iconDir.idReserved == 0);
                Debug.Assert(_iconDir.idType == 1);
                Debug.Assert(_iconDir.idCount > 0);


      
                for (int i = 0; i < _iconDir.idCount; i++)
                {
                    ICONDIRENTRY entry = new ICONDIRENTRY();
                    buffer = br.ReadBytes(sizeof(ICONDIRENTRY));
                    ICONDIRENTRY* ptr = &entry;
                    {
                        Marshal.Copy(buffer, 0, (IntPtr)ptr, sizeof(ICONDIRENTRY));
                    }

                    _iconEntry.Add(entry);
                }

           
                for (int i = 0; i < _iconDir.idCount; i++)
                {
                    fs.Position = ((ICONDIRENTRY)_iconEntry[i]).dwImageOffset;
                    byte[] img = br.ReadBytes((int)((ICONDIRENTRY)_iconEntry[i]).dwBytesInRes);
                    _iconImage.Add(img);
                }

                byte[] b = (byte[])_iconImage[0];

            }
            catch (Exception ex)
            {
                Debug.Assert(false);
            }
            finally
            {
                if (br != null)
                {
                    br.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

       
        unsafe int SizeOfIconGroupData()
        {
            return sizeof(ICONDIR) + sizeof(GRPICONDIRENTRY) * GetImageCount();
        }


        public unsafe byte[] CreateIconGroupData(uint nBaseID)
        {

            byte[] data = new byte[SizeOfIconGroupData()];


            fixed (ICONDIR* ptr = &_iconDir)
            {
                Marshal.Copy((IntPtr)ptr, data, 0, sizeof(ICONDIR));
            }

            int offset = sizeof(ICONDIR);

            for (int i = 0; i < GetImageCount(); i++)
            {
                GRPICONDIRENTRY grpEntry = new GRPICONDIRENTRY();
                BITMAPINFOHEADER bitmapheader = new BITMAPINFOHEADER();


                BITMAPINFOHEADER* ptr = &bitmapheader;
                {
                    Marshal.Copy(GetImageData(i), 0, (IntPtr)ptr, sizeof(BITMAPINFOHEADER));
                }

                grpEntry.bWidth = ((ICONDIRENTRY)_iconEntry[i]).bWidth;
                grpEntry.bHeight = ((ICONDIRENTRY)_iconEntry[i]).bHeight;
                grpEntry.bColorCount = ((ICONDIRENTRY)_iconEntry[i]).bColorCount;
                grpEntry.bReserved = ((ICONDIRENTRY)_iconEntry[i]).bReserved;
                grpEntry.wPlanes = bitmapheader.biPlanes;
                grpEntry.wBitCount = bitmapheader.biBitCount;
                grpEntry.dwBytesInRes = ((ICONDIRENTRY)_iconEntry[i]).dwBytesInRes;
                grpEntry.nID = (ushort)(nBaseID + i);
                GRPICONDIRENTRY* ptr2 = &grpEntry;
                {
                    Marshal.Copy((IntPtr)ptr2, data, offset, Marshal.SizeOf(grpEntry));
                }

                offset += sizeof(GRPICONDIRENTRY);
            }

            return data;
        }

    }
}
