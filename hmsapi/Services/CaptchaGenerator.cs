using System;
using SkiaSharp;

namespace hmsapi.Services
{
    public class DaoCaptchaResult
    {
        public string CaptchaCode { get; set; } = null!;
        public byte[] CaptchaByteData { get; set; } = null!;
        public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteData);
        public DateTime Timestamp { get; set; }
    }

    public static class CaptchaGenerator
        {



            public static string GenerateCaptchaCode(int length = 6)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Random random = new Random();
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            public static DaoCaptchaResult GenerateCaptchaImage(int width, int height, string captchaCode)
            {
                using (SKBitmap bitmap = new SKBitmap(width, height))
                {
                    using (SKCanvas canvas = new SKCanvas(bitmap))
                    {
                        using (SKPaint paint = new SKPaint())
                        {
                            //grad
                            SKShader shader = SKShader.CreateLinearGradient(
                            new SKPoint(0, 0),  // Start point
                            new SKPoint(width, height),  // End point
                            new SKColor[] { SKColors.Red, SKColors.Yellow },  // Colors
                            null,  // Color positions (null for equally spaced colors)
                            SKShaderTileMode.Clamp);


                            Random random = new();
                            // Clear the canvas

                            canvas.Clear(SKColors.White);
                            paint.Shader = shader;
                            // Draw the code on the canvas
                            paint.TextSize = 40;
                            paint.Color = SKColors.Black;
                            canvas.DrawText(captchaCode, 10, 50, paint);

                            // Add some random noise for security (dots)
                            paint.Color = SKColors.Gray;
                            paint.StrokeWidth = 2;
                            for (int i = 0; i < 50; i++)
                            {
                                float x = bitmap.Width * (float)random.NextDouble();
                                float y = bitmap.Height * (float)random.NextDouble();
                                canvas.DrawPoint(x, y, paint);
                            }

                            // Add some random noise for security (lines)
                            paint.StrokeWidth = 1;
                            for (int i = 0; i < 5; i++)
                            {
                                float x1 = bitmap.Width * (float)random.NextDouble();
                                float y1 = bitmap.Height * (float)random.NextDouble();
                                float x2 = bitmap.Width * (float)random.NextDouble();
                                float y2 = bitmap.Height * (float)random.NextDouble();
                                canvas.DrawLine(x1, y1, x2, y2, paint);
                            }



                        };
                    };
                    using (SKImage image = SKImage.FromBitmap(bitmap))
                    {
                        using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                        {
                            return new DaoCaptchaResult()
                            {
                                CaptchaCode = captchaCode,
                                CaptchaByteData = data.ToArray(),
                                Timestamp = DateTime.Now,
                            };
                        };
                    };

                };

            }


        }
    
}

