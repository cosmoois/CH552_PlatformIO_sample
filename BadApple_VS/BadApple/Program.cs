//#define USE_BADAPPLE

using System;
using Rikka;

using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace BadApple
{
    class Program
    {
        const int VID = 0x413D, PID = 0x2107;

        static int frame_size;

        static System.Timers.Timer timer = new System.Timers.Timer();

        static byte[] binBuf;
        static int frame = 0;
        static USBHID dev;
        static void Main(string[] args)
        {
            Stream fileSteam;
            if (System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\frames"))
            {
                // Convert frame BMPs to a single bin file
                fileSteam = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\merge.bin", FileMode.OpenOrCreate);
#if USE_BADAPPLE
                frame_size = 2191;
                for (int i = 1; i < frame_size; i++)
                {
                    byte[] frameBuf = proceesBitmapFile(
                        AppDomain.CurrentDomain.BaseDirectory + "\\frames\\badapple " + (i).ToString() + ".bmp",
                        128, 64, 40);
#else
                // 先頭８フレーム程度は描画が安定しないためダミーフレームを設けたほうが良いか？
                string[] names = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\frames", "*");
                frame_size = names.Length;
                foreach (var name in names)
                {
                    byte[] frameBuf = proceesBitmapFile(
                        AppDomain.CurrentDomain.BaseDirectory + "\\frames\\" + Path.GetFileName(name),
                            128, 64, 40);
#endif
                    fileSteam.Write(frameBuf, 0, frameBuf.Length);
                }
                fileSteam.Flush();
                fileSteam.Close();
            }

            // Read the bin file into memory
            fileSteam = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\merge.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            binBuf = new byte[fileSteam.Length];
            fileSteam.Read(binBuf, 0, (int)fileSteam.Length);
            fileSteam.Close();

            USBHID.RefreshList();
            dev = USBHID.FindHIDDevice(VID, PID, 0);
            dev.Open();

            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 100;
            timer.Enabled = true;

            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;

            dev.Close();
            return;
        }


        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {

            int width = 128;
            int height = 64;

            byte[] reportBuf = new byte[64];
            byte[] recvDataBuf = new byte[64];

            for (int ychunk = 0; ychunk < height / 8; ychunk++)
            {
                for (int xchunk = 0; xchunk < width; xchunk += 32)
                {
                    int offset = xchunk + ychunk * width + frame * width * height / 8;

                    // Keyboardデバッグ用
                    reportBuf[0] = 0xEE;
                    reportBuf[1] = 0x00;
                    reportBuf[2] = 0x04;    // 'a'がキーボードとして帰ってくる想定だが。。。

                    reportBuf[3] = (byte)(xchunk / 8);
                    reportBuf[4] = (byte)ychunk;

                    reportBuf[5] = binBuf[offset];
                    reportBuf[6] = binBuf[offset + 1];
                    reportBuf[7] = binBuf[offset + 2];
                    reportBuf[8] = binBuf[offset + 3];
                    reportBuf[9] = binBuf[offset + 4];
                    reportBuf[10] = binBuf[offset + 5];
                    reportBuf[11] = binBuf[offset + 6];
                    reportBuf[12] = binBuf[offset + 7];

                    reportBuf[13] = binBuf[offset + 8];
                    reportBuf[14] = binBuf[offset + 9];
                    reportBuf[15] = binBuf[offset + 10];
                    reportBuf[16] = binBuf[offset + 11];
                    reportBuf[17] = binBuf[offset + 12];
                    reportBuf[18] = binBuf[offset + 13];
                    reportBuf[19] = binBuf[offset + 14];
                    reportBuf[20] = binBuf[offset + 15];

                    reportBuf[21] = binBuf[offset + 16];
                    reportBuf[22] = binBuf[offset + 17];
                    reportBuf[23] = binBuf[offset + 18];
                    reportBuf[24] = binBuf[offset + 19];
                    reportBuf[25] = binBuf[offset + 20];
                    reportBuf[26] = binBuf[offset + 21];
                    reportBuf[27] = binBuf[offset + 22];
                    reportBuf[28] = binBuf[offset + 23];

                    reportBuf[29] = binBuf[offset + 24];
                    reportBuf[30] = binBuf[offset + 25];
                    reportBuf[31] = binBuf[offset + 26];
                    reportBuf[32] = binBuf[offset + 27];
                    reportBuf[33] = binBuf[offset + 28];
                    reportBuf[34] = binBuf[offset + 29];
                    reportBuf[35] = binBuf[offset + 30];
                    reportBuf[36] = binBuf[offset + 31];

                    dev.Write(reportBuf);
                }
            }

            frame++;
            if (frame >= frame_size)
            {
                Console.WriteLine("Stopped.");
                timer.Enabled = false;
            }
        }

        private static byte[] masks = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
        public static byte[] proceesBitmapFile(String fileName, int width = 128, int height = 64, int threshold = 128)
        {
            Stream fileSteam = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Bitmap bitmap = new Bitmap(fileSteam);
            bitmap = new Bitmap(bitmap, width, height);

            BitmapData m_BitmapData;
            int g_RowSizeBytes;
            byte[] g_PixBytes;
            byte[] result = new byte[width * height / 8];

            // Lock the bitmap data.
            Rectangle bounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            m_BitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            g_RowSizeBytes = m_BitmapData.Stride;

            // Allocate room for the data.
            g_PixBytes = new byte[m_BitmapData.Stride * m_BitmapData.Height];
            // Copy the data into the g_PixBytes array.
            Marshal.Copy(m_BitmapData.Scan0, g_PixBytes, 0, g_PixBytes.Length);
            int numOfDummyBytesPerRow = m_BitmapData.Stride - m_BitmapData.Width * 3;

            int x = 0;
            int y = 0;
            int k = 0;
            while (k < g_PixBytes.Length)
            {
                int b = (255 - g_PixBytes[k++]);
                int g = (255 - g_PixBytes[k++]);
                int r = (255 - g_PixBytes[k++]);

                int offset = x + (y / 8) * width;
                int bit = y % 8;

                int gray = (r + b + g) / 3;

                if (gray > threshold)
                {
                    result[offset] |= masks[bit];
                }

                x++;
                if (x >= width)
                {
                    x = 0;
                    y++;
                    k += numOfDummyBytesPerRow;
                }
            }


            // Unlock the bitmap.
            bitmap.UnlockBits(m_BitmapData);

            // Release resources.
            g_PixBytes = null;
            m_BitmapData = null;
            fileSteam.Close();

            return result;
        }
    }
}
