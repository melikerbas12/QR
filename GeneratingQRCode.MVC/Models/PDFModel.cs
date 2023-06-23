using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GeneratingQRCode.Models
{
    public class PDFModel
    {
        public FileStreamResult FileStreamResult { get; set; }
    }
}