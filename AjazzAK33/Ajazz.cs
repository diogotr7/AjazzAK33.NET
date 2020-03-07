using HidSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AjazzAK33
{
    public class Ajazz
    {
        private readonly HidDevice device;
        private readonly HidStream stream;

        public static bool TryGetKeyboard(out Ajazz keyboard)
        {
            var devs = DeviceList.Local.GetHidDevices().Where(d => d.VendorID == 0x0c45);
            if (!devs.Any())
            {
                keyboard = null;
                return false;
            }

            var device = devs.FirstOrDefault(d => d.GetMaxOutputReportLength() == 64);
            if(device == null)
            {
                keyboard = null;
                return false;
            }

            keyboard = new Ajazz(device);
            return true;
        }

        private Ajazz(HidDevice dev)
        {
            device = dev;

            if (!device.TryOpen(out stream))
            {
                throw new Exception();
            }

            stream.ReadTimeout = 4000;
            stream.WriteTimeout = 4000;
        }

        public bool SetMode(Mode mode)
        {
            WriteRead(Packets.Start);
            Packets.Mode[8] = (byte)mode;
            WriteRead(Packets.Mode);
            stream.Write(Packets.Finish);
            return true;
        }

        public bool SetColor(System.Drawing.Color clr)
        {
            WriteRead(Packets.Start);
            Packets.Mode[8] = (byte)Mode.Solid;
            WriteRead(Packets.Mode);
            WriteRead(Packets.SolidPrefix);
            Packets.SolidColorPacket[8 + 0] = clr.R;
            Packets.SolidColorPacket[8 + 1] = clr.G;
            Packets.SolidColorPacket[8 + 2] = clr.B;
            WriteRead(Packets.SolidColorPacket);
            stream.Write(Packets.Finish);

            return true;
        }

        public bool SetLevel(byte lvl)
        {
            WriteRead(Packets.Start);
            Packets.LevelPacket[8] = lvl;
            WriteRead(Packets.LevelPacket);
            stream.Write(Packets.Finish);
            return true;
        }

        public bool SetKey(IEnumerable<Tuple<Key, Color>> keys)
        {
            WriteRead(Packets.Start);
            Packets.Mode[8] = (byte)Mode.Custom;
            WriteRead(Packets.Mode);

            foreach(var key in keys)
            {
                if (!KeyMap.TryGetValue(key.Item1, out var coords))
                {
                    continue;
                }

                Packets.KeyPacket[5 + 0] = coords.x;
                Packets.KeyPacket[5 + 1] = coords.y;

                Packets.KeyPacket[8 + 0] = key.Item2.R;
                Packets.KeyPacket[8 + 1] = key.Item2.G;
                Packets.KeyPacket[8 + 2] = key.Item2.B;

                WriteRead(Packets.KeyPacket);
            }

            stream.Write(Packets.Finish);
            return true;
        }

        private void WriteRead(byte[] packet)
        {
            stream.Write(packet);
            stream.Read();
        }

        private readonly Dictionary<Key, (byte x, byte y)> KeyMap = new Dictionary<Key, (byte, byte)>()
        {            
            [Key.Esc] = (0x00, 0x00),
            [Key.F1] = (0x03, 0x00),
            [Key.F2] = (0x06, 0x00),
            [Key.F3] = (0x09, 0x00),
            [Key.F4] = (0x0C, 0x00),
            [Key.F5] = (0x0F, 0x00),
            [Key.F6] = (0x12, 0x00),
            [Key.F7] = (0x15, 0x00),
            [Key.F8] = (0x18, 0x00),
            [Key.F9] = (0x1B, 0x00),
            [Key.F10] = (0x1E, 0x00),
            [Key.F11] = (0x21, 0x00),
            [Key.F12] = (0x24, 0x00),
            //[Key.fn] = (0x2A , 0x00),    // # known not to work]= 27 2A 9B A2 A5
            [Key.Del] = (0xA8, 0x00),
            [Key.Tilde] = (0x3F, 0x00),
            [Key.N1] = (0x42, 0x00),
            [Key.N2] = (0x45, 0x00),
            [Key.N3] = (0x48, 0x00),
            [Key.N4] = (0x4B, 0x00),
            [Key.N5] = (0x4E, 0x00),
            [Key.N6] = (0x51, 0x00),
            [Key.N7] = (0x54, 0x00),
            [Key.N8] = (0x57, 0x00),
            [Key.N9] = (0x5A, 0x00),
            [Key.N0] = (0x5D, 0x00),
            [Key.Minus] = (0x60, 0x00),
            [Key.Equals] = (0x63, 0x00),
            [Key.Backspace] = (0x66, 0x00),
            [Key.Home] = (0x6C, 0x00),
            [Key.Tab] = (0x7E, 0x00),
            [Key.Q] = (0x81, 0x00),
            [Key.W] = (0x84, 0x00),
            [Key.E] = (0x87, 0x00),
            [Key.R] = (0x8A, 0x00),
            [Key.T] = (0x8D, 0x00),
            [Key.Y] = (0x90, 0x00),
            [Key.U] = (0x93, 0x00),
            [Key.I] = (0x96, 0x00),
            [Key.O] = (0x99, 0x00),
            [Key.P] = (0x9C, 0x00),
            [Key.OpenBracket] = (0x9F, 0x00),
            [Key.CloseBracket] = (0xA2, 0x00),
            [Key.Backslash] = (0xA5, 0x00),
            [Key.PgUp] = (0x6F, 0x00),
            [Key.CapsLock] = (0xBD, 0x00),
            [Key.A] = (0xC0, 0x00),
            [Key.S] = (0xC3, 0x00),
            [Key.D] = (0xC6, 0x00),
            [Key.F] = (0xC9, 0x00),
            [Key.G] = (0xCC, 0x00),
            [Key.H] = (0xCF, 0x00),
            [Key.J] = (0xD2, 0x00),
            [Key.K] = (0xD5, 0x00),
            [Key.L] = (0xD8, 0x00),
            [Key.Semicolon] = (0xDB, 0x00),
            [Key.Apostrophe] = (0xDE, 0x00),
            [Key.Enter] = (0xE4, 0x00),
            [Key.PgDn] = (0xAE, 0x00),
            [Key.LShift] = (0xFC, 0x00),
            [Key.Z] = (0x02, 0x01),
            [Key.X] = (0x05, 0x01),
            [Key.C] = (0x08, 0x01),
            [Key.V] = (0x0B, 0x01),
            [Key.B] = (0x0E, 0x01),
            [Key.N] = (0x11, 0x01),
            [Key.M] = (0x14, 0x01),
            [Key.Comma] = (0x17, 0x01),
            [Key.Period] = (0x1A, 0x01),
            [Key.Slash] = (0x1D, 0x01),
            [Key.RShift] = (0x23, 0x01),
            [Key.UArrow] = (0x29, 0x01),
            [Key.End] = (0xAB, 0x00),
            [Key.LCtrl] = (0x3B, 0x01),
            [Key.Super] = (0x3E, 0x01),
            [Key.LAlt] = (0x41, 0x01),
            [Key.Space] = (0x44, 0x01),
            [Key.RAlt] = (0x47, 0x01),
            [Key.RCtrl] = (0x53, 0x01),
            [Key.LArrow] = (0x65, 0x01),
            [Key.DArrow] = (0x68, 0x01),
            [Key.RArrow] = (0x6B, 0x01)
        };
    }
}
