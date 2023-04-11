namespace messages_backend.Services
{

    public interface ISteganographyService
    {
        void Encode(FileInfo carrier, string payload);
        string Decode(FileInfo file);
    }

    public class SteganographyService : ISteganographyService
    {
        public void Encode(FileInfo carrier, string payload)
        {
            int pos = LocatePixelArray(carrier);
            int readByte = 0;
            string stegoFilePath = carrier.FullName.Substring(0, carrier.FullName.Length - 4) + "_stego.bmp";
            FileInfo stegoFile = new FileInfo(stegoFilePath);
            try
            {
                File.Copy(carrier.FullName, stegoFile.FullName, true);
            }
            catch (IOException e1)
            {
                Console.WriteLine("IOException: " + e1.Message);
                return;
            }

            using (FileStream stream = new FileStream(stegoFile.FullName, FileMode.Open, FileAccess.ReadWrite))
            {
                stream.Seek(pos, SeekOrigin.Begin);
                for (int i = 0; i < 32; i++)
                {
                    readByte = stream.ReadByte();
                    stream.Seek(pos, SeekOrigin.Begin);
                    stream.WriteByte((byte)(readByte & 0b11111110));
                    pos++;
                }

                payload += (char)0;
                int payloadByte;
                int payloadBit;
                int newByte;
                foreach (char element in payload.ToCharArray())
                {
                    payloadByte = (int)element;
                    for (int i = 0; i < 8; i++)
                    {
                        readByte = stream.ReadByte();
                        payloadBit = (payloadByte >> i) & 1;
                        newByte = (readByte & 0b11111110) | payloadBit;
                        stream.Seek(pos, SeekOrigin.Begin);
                        stream.WriteByte((byte)newByte);
                        pos++;
                    }
                }
            }
        }

        public static int LocatePixelArray(FileInfo file)
        {
            using (FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(10, SeekOrigin.Begin);
                int location = 0;
                for (int i = 0; i < 4; i++)
                {
                    location |= stream.ReadByte() << (4 * i);
                }
                return location;
            }
        }

        public string Decode(FileInfo carrier)
        {
            int start = LocatePixelArray(carrier);
            using (FileStream stream = new FileStream(carrier.FullName, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(start, SeekOrigin.Begin);
                for (int i = 0; i < 32; i++)
                {
                    if ((stream.ReadByte() & 1) != 0)
                    {
                        return "Picture has not been encoded!!!";
                    }
                }

                string result = "";
                int character;
                while (true)
                {
                    character = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        character |= (stream.ReadByte() & 1) << i;
                    }
                    if (character == 0)
                        break;
                    result += (char)character;
                }
                return result;
            }
        }

        public static int CharactersAvailable(FileInfo carrier)
        {
            return (int)(carrier.Length - LocatePixelArray(carrier) + 32) / 8;
        }

    }
}
