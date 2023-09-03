using System;
using System.Collections.Generic;
using System.IO;
using MimeKit;
using MailKit.Net.Smtp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using QRCoder;
using EventOganizer.Entities;

namespace EventOganizer.EmailSender
{
    public class EmailSender
    {
        private readonly string _smtpServer = "smtp.abv.bg";
        private readonly int _smtpPort = 465;
        private readonly string _smtpUsername = "events111@abv.bg";
        private readonly string _smtpPassword = "Parket!01";

        public void SendTicketEmail(string recipientEmail, string recipientName, List<CartItem> cartItems)
        {
            try
            {
                PdfDocument document = new PdfDocument();

                

                foreach (var cartItem in cartItems)
                {
                    string qrData = $"Name: {recipientName}\nEmail: {recipientEmail}\nTicket ID: {cartItem.Id}\nQuantity: {cartItem.Quantity}";
                    QRCodeData qrCodeData = GenerateQRCodeData(qrData);
                    QRCode qrCode = new QRCode(qrCodeData);

                    PdfPage page = document.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    
                    int yOffset = 20;

                    XImage logoImage = XImage.FromFile("D://Uni//Дипломна Работа//Event-Organizer//EventOganizer//NewFolder//Logo.png");
                    gfx.DrawImage(logoImage, new XRect(50, yOffset, logoImage.PointWidth, logoImage.PointHeight));
                    yOffset += (int)logoImage.PointHeight + 20; 

                   
                    XFont font = new XFont("Arial", 12);
                    gfx.DrawString("Events Organizer", font, XBrushes.Black, new XPoint(50, yOffset));
                    yOffset += 20; 

                    string additionalInfo = $"ID на билет: {cartItem.Id}";
                    gfx.DrawString(additionalInfo, font, XBrushes.Black, new XPoint(50, yOffset));
                    yOffset += 20;
                    additionalInfo = $"\nБрой: { cartItem.Quantity}";
                        gfx.DrawString(additionalInfo, font, XBrushes.Black, new XPoint(50, yOffset));
                    yOffset += 20;
                    additionalInfo = $"\nЗакупил: { recipientName}";
                            gfx.DrawString(additionalInfo, font, XBrushes.Black, new XPoint(50, yOffset));
                    yOffset += 50;
                    gfx.DrawString("Вашият QR код е", font, XBrushes.Black, new XPoint(50, yOffset));
                    yOffset += 20;  

                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrCode.GetGraphic(10).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0;
                        double scalingFactor = 0.5;
                        XImage xImage = XImage.FromStream(ms);

                        gfx.DrawImage(xImage, new XRect(50, yOffset, xImage.PointWidth * scalingFactor, xImage.PointHeight * scalingFactor));
                    }

                    yOffset += 200;
                }





                MemoryStream pdfStream = new MemoryStream();
                document.Save(pdfStream);
                pdfStream.Position = 0;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Events Organiser", _smtpUsername));
                message.To.Add(new MailboxAddress(recipientName, recipientEmail));
                message.Subject = "Your QR Code Tickets";

                var pdfAttachment = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(new MemoryStream(pdfStream.ToArray())),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = "tickets.pdf"
                };

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $"<h1>Hello {recipientName},</h1><p>Вашите билети с QR кодове са в прикачения PDF файл.</p>";

                bodyBuilder.Attachments.Add(pdfAttachment);

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpServer, _smtpPort, true);
                    client.Authenticate(_smtpUsername, _smtpPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }

        private QRCodeData GenerateQRCodeData(string data)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            return qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        }
    }
}
