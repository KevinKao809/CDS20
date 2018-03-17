using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSShareLib.Helper
{
    public class ImageHelper
    {
        MagickFormat imageFormat = MagickFormat.Jpg;
        string extentionName = ".jpg";
        string imageBgColor = "#ffffff";
        int JPGQuality = 90;
        int PNGCompression = 9;
        int PNGFilterType = 1;
        bool imageCrop = false;
        int imageWidth = 200, imageHeight = 200;

        public string PublishImage(string imageSourceFileName, BlobStorageHelper storageHelper, string uploadFilePath, string widthXheight, string bgColor, string imageFormat)
        {
            try
            {
                if (!string.IsNullOrEmpty(widthXheight))
                {
                    widthXheight = widthXheight.ToLower();
                    string[] value = widthXheight.Split('x');
                    imageWidth = int.Parse(value[0]);
                    imageHeight = int.Parse(value[1]);
                }

                if (!string.IsNullOrEmpty(bgColor))
                    this.imageBgColor = bgColor;

                switch (imageFormat.ToLower())
                {
                    case "jpg":
                    case "jpeg":
                        this.imageFormat = MagickFormat.Jpg;
                        extentionName = ".jpg";
                        break;
                    case "png":
                        this.imageFormat = MagickFormat.Png;
                        extentionName = ".png";
                        break;
                }

                string imageConvertFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + extentionName);
                GenerateImage(imageSourceFileName, imageConvertFileName);

                if (uploadFilePath.EndsWith("/"))
                    uploadFilePath = uploadFilePath.Substring(0, uploadFilePath.Length - 1);

                uploadFilePath = string.Format("{0}/{1}-{2}{3}", uploadFilePath, (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, widthXheight, extentionName);
                string fileAbsoluteUri = storageHelper.SaveFiletoStorage(imageConvertFileName, uploadFilePath);
                File.Delete(imageConvertFileName);

                return fileAbsoluteUri;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GenerateImage(string sourceImageFileName, string destImageFileName)
        {
            MagickImage bgImage = null;
            MagickImage ftImage = null;

            try
            {
                /* Load Source Image */
                ftImage = new MagickImage(sourceImageFileName);

                /* Setup Background Color on bgImage and ftImage  */
                if (this.imageFormat.Equals(MagickFormat.Png))
                {
                    if (imageBgColor.Equals("transparent"))
                    {   /* Background Color is 'transparent' */
                        ftImage.BackgroundColor = MagickColor.Transparent;
                        if (!this.imageCrop)
                        {
                            bgImage = new MagickImage(new MagickColor(MagickColor.Transparent), imageWidth, imageHeight);
                            bgImage.BackgroundColor = MagickColor.Transparent;
                        }
                    }
                    else
                    {   /* Background Color is Hex Color */
                        ftImage.BackgroundColor = new MagickColor(System.Drawing.ColorTranslator.FromHtml(imageBgColor));
                        if (!this.imageCrop)
                            bgImage = new MagickImage(new MagickColor(System.Drawing.ColorTranslator.FromHtml(imageBgColor)), imageWidth, imageHeight);

                    }
                }
                else  /* Format == Jpeg */
                {
                    /* Background Color is Hex Color */
                    ftImage.BackgroundColor = new MagickColor(System.Drawing.ColorTranslator.FromHtml(imageBgColor));
                    if (!this.imageCrop)
                        bgImage = new MagickImage(new MagickColor(System.Drawing.ColorTranslator.FromHtml(imageBgColor)), imageWidth, imageHeight);

                }

                /* Setup Output Format  */
                ftImage.Format = this.imageFormat;
                if (bgImage != null)
                    bgImage.Format = this.imageFormat;

                /* Resize, Crop, Merge, Setup Quality and Output  */
                if (this.imageCrop)
                {
                    /* Resize and Crop */
                    MagickGeometry geometry = new MagickGeometry(0, imageHeight);
                    ftImage.Resize(geometry);
                    ftImage.Crop(imageWidth, imageHeight, Gravity.Center);

                    /* Setup Quality */
                    if (this.imageFormat.Equals(MagickFormat.Png))
                    {
                        ftImage.SetDefine(MagickFormat.Png, "compression-strategy", PNGCompression.ToString());
                        ftImage.SetDefine(MagickFormat.Png, "compression-filter", PNGFilterType.ToString());
                    }
                    else
                        ftImage.Quality = JPGQuality;

                    /* Output */
                    ftImage.Write(destImageFileName);
                }
                else
                {
                    /* Only Resize */
                    MagickGeometry geometry = new MagickGeometry(imageWidth, imageHeight);
                    ftImage.Resize(geometry);

                    /* Merge bgImage and ftImage  */
                    if (this.imageFormat.Equals(MagickFormat.Png) && imageBgColor.Equals("transparent"))
                        bgImage.Composite(ftImage, Gravity.Center, CompositeOperator.Overlay);
                    else
                        bgImage.Composite(ftImage, Gravity.Center);

                    /* Setup Quality */
                    if (this.imageFormat.Equals(MagickFormat.Png))
                    {
                        bgImage.SetDefine(MagickFormat.Png, "compression-strategy", PNGCompression.ToString());
                        bgImage.SetDefine(MagickFormat.Png, "compression-filter", PNGFilterType.ToString());
                    }
                    else
                        ftImage.Quality = JPGQuality;

                    /* Output */
                    bgImage.Write(destImageFileName);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
