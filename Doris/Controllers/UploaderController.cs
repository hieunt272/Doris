using Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Doris.Controllers
{
    [Authorize]
    public class UploaderController : Controller
    {
        private const int ThumbSize = 160;

        public ActionResult GetFile(string name, bool thumbnail = false)
        {
            var file = GetFileInfo(name);
            string mimeMapping = MimeMapping.GetMimeMapping(file.Name);
            if (!thumbnail)
                return File(file.FullName, mimeMapping);
            return Thumb(file, mimeMapping);
        }

        private ActionResult Thumb(FileInfo file, string contentType)
        {
            if (contentType.StartsWith("image"))
            {
                try
                {
                    using (var image = Image.FromFile(file.FullName))
                    {
                        var thumbHeight = image.Height * (ThumbSize / image.Width);
                        using (var thumbnailImage = image.GetThumbnailImage(ThumbSize, thumbHeight, () => false, IntPtr.Zero))
                        {
                            var memoryStream = new MemoryStream();
                            thumbnailImage.Save(memoryStream, image.RawFormat);
                            memoryStream.Position = 0L;
                            return File(memoryStream, contentType);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return Redirect("https://placeholdit.imgix.net/~text?txtsize=20&txt=160x120&w=160&h=120");
        }

        [HttpPost]
        public ActionResult DeleteFile(string name, string folder = "projects")
        {
            GetFileInfo(name, folder).Delete();
            return Json($"{name} was deleted");
        }

        [HttpGet]
        public JsonResult Upload(IEnumerable<string> names, string folder)
        {
            if (names == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var f = GetUploadFolder(folder).GetFiles();
            var listFile = (from fileInfo in f from name in names where name.Contains(fileInfo.Name) select fileInfo.Name).ToList();

            return Json(new
            {
                files = listFile.Select(
                    file => new
                    {
                        deleteType = "POST",
                        name = DateTime.Now.ToString("yyyy/MM/dd") + "/" + file,
                        size = file.Length,
                        url = Url.Action("GetFile", "Uploader", new
                        {
                            Name = file
                        }),
                        thumbnailUrl = Url.Action("GetFile", "Uploader", new
                        {
                            Name = file,
                            thumbnail = true
                        }),
                        deleteUrl = Url.Action("DeleteFile", "Uploader", new
                        {
                            Name = file
                        })
                    })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Upload(string folder = "projects")
        {
            var array = Request.Files.Cast<string>().Select(k => Request.Files[k]).ToArray();
            var stringList = new List<string>();
            foreach (var file in array)
            {
                var disk = SaveFileToDisk(file, folder);
                stringList.Add(disk);
            }
            return Upload(stringList, folder);
        }

        private string SaveFileToDisk(HttpPostedFileBase fileData, string folderPath)
        {
            var folderDate = DateTime.Now.ToString("yyyy/MM/dd");
            var folder = "~/images/" + folderPath + "/" + folderDate;
            var result = "";
            if (fileData.ContentLength <= 4000 * 1024 &&
                HtmlHelpers.CheckFileExt(Path.GetExtension(fileData.FileName), "jpg|png|gif|jpeg"))
            {
                var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(fileData.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(fileData.FileName);
                HtmlHelpers.CreateFolder(Server.MapPath(folder));

                var fileName = Server.MapPath(Path.Combine(folder, imgFileName));

                Resize(fileData, 1200, 900, Path.Combine(folder, fileName));
                result = folderDate + "/" + fileName;
            }
            return result;
        }

        private static DirectoryInfo GetUploadFolder(string folder)
        {
            var directoryInfo = new DirectoryInfo(HostingEnvironment.MapPath(Path.Combine("/images/" + folder + "/" + DateTime.Now.ToString("yyyy/MM/dd"))));
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            return directoryInfo;
        }

        private static FileInfo GetFileInfo(string name, string folder = "projects")
        {
            return GetUploadFolder(folder).GetFiles(name).Single();
        }

        public static void Resize(HttpPostedFileBase originalImage, int maxWidth, int maxHeight, string path, string font = null, int fontsize = 0)
        {
            var originalBmp = new Bitmap(originalImage.InputStream);
            //Check EXIF
            if (originalBmp.PropertyIdList.Contains(0x0112))
            {
                int rotationValue = originalBmp.GetPropertyItem(0x0112).Value[0];
                switch (rotationValue)
                {
                    case 1: // landscape, do nothing
                        break;
                    case 8: // rotated 90 right
                        // de-rotate:
                        originalBmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;

                    case 3: // bottoms up
                        originalBmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;

                    case 6: // rotated 90 left
                        originalBmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                }
            }

            float origWidth = originalBmp.Width;
            float origHeight = originalBmp.Height;
            float sngRatio;

            if (origWidth > maxWidth)
            {
                sngRatio = maxWidth / origWidth;
                origWidth = maxWidth;
                origHeight = origHeight * sngRatio;
            }

            if (origHeight > maxHeight)
            {
                sngRatio = maxHeight / origHeight;
                origHeight = maxHeight;
                origWidth = origWidth * sngRatio;
            }

            var newBmp = new Bitmap(originalBmp, (int)origWidth, (int)origHeight);
            var oGraphics = Graphics.FromImage(newBmp);

            oGraphics.SmoothingMode = SmoothingMode.HighQuality;
            oGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            oGraphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            oGraphics.CompositingQuality = CompositingQuality.HighQuality;

            if (font != null)
            {
                using (var font1 = new Font(font, fontsize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    var rect1 = new Rectangle(0, 0, (int)origWidth - 1, (int)origHeight - 1);
                    var rect2 = new Rectangle(0, 0, (int)origWidth, (int)origHeight);

                    var stringFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far };

                    oGraphics.DrawString("www.vico.vn", font1, new SolidBrush(Color.FromArgb(255, Color.Black)), rect1, stringFormat);
                    oGraphics.DrawString("www.vico.vn", font1, new SolidBrush(Color.FromArgb(255, Color.White)), rect2, stringFormat);
                    //oGraphics.DrawRectangle(Pens.DarkRed, rect1);
                }
            }

            oGraphics.DrawImage(newBmp, 0, 0, origWidth, origHeight);

            //var jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            var mimeType = HtmlHelpers.GetMimeType(originalImage.FileName);
            var jgpEncoder = HtmlHelpers.GetEncoderInfo(mimeType);

            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);

            var myEncoderParameter = new EncoderParameter(myEncoder, 90L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            newBmp.Save(path, jgpEncoder, myEncoderParameters);

            originalBmp.Dispose();
            newBmp.Dispose();
            oGraphics.Dispose();
        }

        public void ResizeWaterMark(HttpPostedFileBase originalImage, int maxWidth, int maxHeight, string path)
        {
            var originalBmp = new Bitmap(originalImage.InputStream);
            //Check EXIF
            if (originalBmp.PropertyIdList.Contains(0x0112))
            {
                int rotationValue = originalBmp.GetPropertyItem(0x0112).Value[0];
                switch (rotationValue)
                {
                    case 1: // landscape, do nothing
                        break;
                    case 8: // rotated 90 right
                        // de-rotate:
                        originalBmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;

                    case 3: // bottoms up
                        originalBmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;

                    case 6: // rotated 90 left
                        originalBmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                }
            }

            float origWidth = originalBmp.Width;
            float origHeight = originalBmp.Height;
            float sngRatio;

            if (origWidth > maxWidth)
            {
                sngRatio = maxWidth / origWidth;
                origWidth = maxWidth;
                origHeight = origHeight * sngRatio;
            }

            if (origHeight > maxHeight)
            {
                sngRatio = maxHeight / origHeight;
                origHeight = maxHeight;
                origWidth = origWidth * sngRatio;
            }

            var bmPhoto = new Bitmap((int)origWidth, (int)origHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);

            var grPhoto = Graphics.FromImage(bmPhoto);

            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            //grPhoto.DrawImage(originalBmp, new Rectangle(0, 0, (int)origWidth, (int)origHeight), 0, 0, origWidth, origHeight, GraphicsUnit.Pixel);

            grPhoto.DrawImage(originalBmp, 0, 0, origWidth, origHeight);

            if (origHeight >= 400 && origWidth >= 400)
            {
                var bmWatermark = new Bitmap(bmPhoto);
                bmWatermark.SetResolution(originalBmp.HorizontalResolution, originalBmp.VerticalResolution);

                var grWatermark = Graphics.FromImage(bmWatermark);

                //var imageAttributes = new ImageAttributes();
                //var colorMap = new ColorMap { OldColor = Color.FromArgb(255, 0, 255, 0), NewColor = Color.FromArgb(0, 0, 0, 0) };

                //ColorMap[] remapTable = { colorMap };

                //imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                //float[][] colorMatrixElements = { new[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f }, new[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f }, new[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f }, new[] { 0.0f, 0.0f, 0.0f, 0.3f, 0.0f }, new[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f } };

                //var wmColorMatrix = new ColorMatrix(colorMatrixElements);

                //imageAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                var imgWatermark = new Bitmap(Server.MapPath("/content/images/logo-watermark.png"));
                var wmWidth = imgWatermark.Width;
                var wmHeight = imgWatermark.Height;

                var xPosOfWm = 50; //(int)((origWidth - wmWidth) - 10);
                var yPosOfWm = 50; //(int)((origHeight - wmHeight) - 10);
                                   //const int yPosOfWm = 10;

                //grWatermark.DrawImage(imgWatermark, new Rectangle(xPosOfWm, yPosOfWm, wmWidth, wmHeight), 0, 0, wmWidth, wmHeight, GraphicsUnit.Pixel, imageAttributes);

                grWatermark.DrawImage(imgWatermark, new Rectangle(xPosOfWm, yPosOfWm, wmWidth, wmHeight), 0, 0, wmWidth, wmHeight, GraphicsUnit.Pixel);
                originalBmp = bmWatermark;
                grPhoto.Dispose();
                grWatermark.Dispose();
                imgWatermark.Dispose();
            }

            //var jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            var mimeType = HtmlHelpers.GetMimeType(originalImage.FileName);
            var jgpEncoder = HtmlHelpers.GetEncoderInfo(mimeType);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 90L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            originalBmp.Save(path, jgpEncoder, myEncoderParameters);
            //originalBmp.Save(path, ImageFormat.Jpeg);

            originalBmp.Dispose();
        }
    }
}