using System.Drawing;
using GeneratingQRCode.Models;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QRCoder;

namespace GeneratingQRCode.MVC.Controllers;
public class QRCodeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        QRCodeModel model = new();
        return View(model);
    }

    [HttpPost]
    public IActionResult GenerateQRCode(QRCodeModel model)
    {
        // QR kodu oluştur
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(model.QRCodeData, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);
        Bitmap qrCodeImage = qrCode.GetGraphic(20);

        // QR kodunu Base64 stringine dönüştür
        using (MemoryStream ms = new MemoryStream())
        {
            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] qrCodeBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(qrCodeBytes);
            model.QRImageURL = "data:image/png;base64," + base64String;
        }

        return View("Index", model);
    }

    [HttpPost]
    public IActionResult GeneratePDF(QRCodeModel model)
    {
        // QR kodunu PdfSharpCore ile PDF'e dönüştür
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // QR kodunu görüntü olarak yükleyin
        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(model.QRImageURL.Substring(22))))
        {
            XImage qrCodeImage = XImage.FromStream(() => ms);

            // QR kodunu PDF sayfasına çizin
            gfx.DrawImage(qrCodeImage, 50, 50); // QR kodunun konumunu belirleyebilirsiniz

            // PDF dosyasını oluşturun ve indirme için kullanıcıya sunun
            using (MemoryStream output = new MemoryStream())
            {
                document.Save(output);
                output.Position = 0;
                return File(output.ToArray(), "application/pdf", "qrcode.pdf");
            }
        }
    }
}