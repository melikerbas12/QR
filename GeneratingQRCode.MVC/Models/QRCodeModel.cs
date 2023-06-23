using System.ComponentModel.DataAnnotations;

namespace GeneratingQRCode.Models
{
    public class QRCodeModel
    {
        public string QRCodeData { get; set; }
        public string QRImageURL { get; set; }
    }
}