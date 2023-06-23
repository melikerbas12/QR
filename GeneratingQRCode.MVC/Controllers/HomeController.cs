

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GeneratingQRCode.Models;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QRCoder;
using static QRCoder.PayloadGenerator;

namespace GeneratingQRCode.MVC.Controllers;
public class HomeController : Controller
{

    public IActionResult CreateQRCode()
    {
        QRCodeModel model = new QRCodeModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult CreateQRCode(QRCodeModel model)
    {
        Payload payload = null;
        switch (model.QRCodeType)
        {
            case 1: // website url
                payload = new Url(model.WebsiteURL);
                break;
            case 2: // bookmark url
                payload = new Bookmark(model.BookmarkURL, model.BookmarkURL);
                break;
            case 3: // compose sms
                payload = new SMS(model.SMSPhoneNumber, model.SMSBody);
                break;
            case 4: // compose whatsapp message
                payload = new WhatsAppMessage(model.WhatsAppNumber, model.WhatsAppMessage);
                break;
            case 5: //compose email
                payload = new Mail(model.ReceiverEmailAddress, model.EmailSubject, model.EmailMessage);
                break;
            case 6: // wifi qr code
                payload = new WiFi(model.WIFIName, model.WIFIPassword, WiFi.Authentication.WPA);
                break;
        }
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload);
        QRCode qrCode = new QRCode(qrCodeData);
        var qrCodeAsBitmap = qrCode.GetGraphic(20);
        // use this when you want to show your logo in middle of QR Code and change color of qr code
        //Bitmap logoImage = new Bitmap(@"wwwroot/img/Virat-Kohli.jpg");
        //var qrCodeAsBitmap = qrCode.GetGraphic(20, Color.Black, Color.Red, logoImage);
        string base64String = Convert.ToBase64String(BitmapToByteArray(qrCodeAsBitmap));
        model.QRImageURL = "data:image/png;base64," + base64String;
        return View("CreateQRCode", model);
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult GenerateQRCodeAndConvertToPDF([FromBody] MyDataModel data)
    {
        try
        {
            // QR kod oluşturma
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data.JsonData, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            // QR kodunu PDF'e dönüştürme
            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XImage qrCodeXImage;

                using (MemoryStream qrCodeStream = new MemoryStream())
                {
                    qrCodeImage.Save(qrCodeStream, ImageFormat.Png);
                    qrCodeStream.Position = 0; // Stream'i başa al
                    qrCodeXImage = XImage.FromStream(() => qrCodeStream);
                }

                gfx.DrawImage(qrCodeXImage, 50, 50);

                document.Save(stream);
                document.Close();

                byte[] pdfBytes = stream.ToArray();

                // PDF'i indirme olarak dönme
                return File(pdfBytes, "application/pdf", "qrcode.pdf");
            }
        }
        catch (Exception ex)
        {
            // Hata durumunda uygun yanıtı dönme
            return BadRequest(ex.Message);
        }
    }

    private byte[] BitmapToByteArray(Bitmap bitmap)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
public class MyDataModel
{
    public string JsonData { get; set; }
}