using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace ConsoleApplication1
{
    [DataContract]
    public class SettingsMessage
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public byte[] RawBytes { get; set; }
        [DataMember]
        public ResultPoint[] ResultPoints { get; set; }
        [DataMember]
        public BarcodeFormat Format { get; set; }
    }
}
