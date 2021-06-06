using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace CMS.Common
{
    public class Utils
    {
        private static Dictionary<int, string> locationDictionary = new() { { -1, "Tất cả tỉnh thành" }, { 1, "An Giang" }, { 2, "Bà Rịa - Vũng Tàu" }, { 3, "Bắc Giang" }, { 4, "Bắc Kạn" }, { 5, "Bạc Liêu" }, { 6, "Bắc Ninh" }, { 7, "Bến Tre" }, { 8, "Bình Định" }, { 9, "Bình Dương" }, { 10, "Bình Phước" }, { 11, "Bình Thuận" }, { 12, "Cà Mau" }, { 13, "Cao Bằng" }, { 14, "Đắk Lắk" }, { 15, "Đắk Nông" }, { 16, "Điện Biên" }, { 17, "Đồng Nai" }, { 18, "Đồng Tháp" }, { 19, "Gia Lai" }, { 20, "Hà Giang" }, { 21, "Hà Nam" }, { 22, "Hà Tĩnh" }, { 23, "Hải Dương" }, { 24, "Hậu Giang" }, { 25, "Hòa Bình" }, { 26, "Hưng Yên" }, { 27, "Khánh Hòa" }, { 28, "Kiên Giang" }, { 29, "Kon Tum" }, { 30, "Lai Châu" }, { 31, "Lâm Đồng" }, { 32, "Lạng Sơn" }, { 33, "Lào Cai" }, { 34, "Long An" }, { 35, "Nam Định" }, { 36, "Nghệ An" }, { 37, "Ninh Bình" }, { 38, "Ninh Thuận" }, { 39, "Phú Thọ" }, { 40, "Quảng Bình" }, { 41, "Quảng Nam" }, { 42, "Quảng Ngãi" }, { 43, "Quảng Ninh" }, { 44, "Quảng Trị" }, { 45, "Sóc Trăng" }, { 46, "Sơn La" }, { 47, "Tây Ninh" }, { 48, "Thái Bình" }, { 49, "Thái Nguyên" }, { 50, "Thanh Hóa" }, { 51, "Thừa Thiên Huế" }, { 52, "Tiền Giang" }, { 53, "Trà Vinh" }, { 54, "Tuyên Quang" }, { 55, "Vĩnh Long" }, { 56, "Vĩnh Phúc" }, { 57, "Yên Bái" }, { 58, "Phú Yên" }, { 59, "Cần Thơ" }, { 60, "Đà Nẵng" }, { 61, "Hải Phòng" }, { 62, "Hà Nội" }, { 63, "TP HCM" } };
        private static Dictionary<string, string> metaSettings = new() { };
        private static readonly Random random = new();
        private static readonly Regex NonExplicitLines = new("\r|\n", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex DivEndings = new("</div>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex LineBreaks = new("</br\\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex Tags = new("<[^>]*>", RegexOptions.Compiled);

        

        public Utils()
        {
            
        }

        public static string GetResources(string ResourceName, string Key)
        {
            ResourceManager Resources = new("Resources." + ResourceName, System.Reflection.Assembly.Load("App_GlobalResources"));
            return Resources.GetString(Key);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string RandomNumber(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string GetRandomText(int len)
        {
            StringBuilder randomText = new();
            string alphabets = "2345679ABCEGHKMNPSXZabceghkmnpqstuvxz";
            Random r = new();
            for (int j = 1; j <= 6; j++)
            {
                randomText.Append(alphabets[r.Next(alphabets.Length)]);
            }
            return randomText.ToString();
        }

        public static string ShowNumber(object Number, int NumberDecimalDigits)
        {
            string NumberString = "0";
            if (Number != null && Number.ToString() != string.Empty)
            {
                NumberFormatInfo myNumberFormat = new CultureInfo("vi-VN").NumberFormat;
                myNumberFormat.NumberGroupSeparator = ".";
                myNumberFormat.NumberDecimalDigits = NumberDecimalDigits;

                NumberString = double.Parse(Number.ToString()).ToString("N", myNumberFormat);
            }
            return NumberString;
        }

        public static int CounterWebsite
        {
            get
            {
                return 0;
            }
        }

        public static Dictionary<int, string> LocationDictionary { get => locationDictionary; set => locationDictionary = value; }
        public static Dictionary<string, string> MetaSettings { get => metaSettings; set => metaSettings = value; }

        public static Random Random => random;

        public static Regex NonExplicitLines1 => NonExplicitLines;

        public static Regex DivEndings1 => DivEndings;

        public static Regex LineBreaks1 => LineBreaks;

        public static Regex Tags1 => Tags;

  

        public static void AddCounterWebsite()
        {
        }

        public static string CutText(string TextInput, int NumberCharacter)
        {
            if (string.IsNullOrEmpty(TextInput)) return TextInput;
            TextInput = StripHTML(TextInput);
            TextInput = Regex.Replace(TextInput, @"\r\n?|\n|\t", " ");
            TextInput = Regex.Replace(TextInput, @"\s+", " ");

            int Length = TextInput.Length;
            if (Length < NumberCharacter)
                return TextInput;

            // Process
            TextInput = TextInput.Substring(0, NumberCharacter);
            int index = TextInput.LastIndexOfAny(new char[] { ' ' });
            string Result = TextInput.Substring(0, index);
            return Result;
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static bool DeleteFile(string xPath, string xFileName)
        {
            if (File.Exists(xPath + xFileName))
            {
                File.Delete(xPath + xFileName);
                return true;
            }
            else
                return false;
        }

        public static string GetFileNameFromURL(string URL)
        {
            return URL.Substring(URL.LastIndexOf("/") + 1, URL.Length - URL.LastIndexOf("/") - 1);
        }

        public static string GetFileNameNotExFromUrl(string Url)
        {
            string tmp = GetFileNameFromURL(Url);
            return tmp[0..^4];
        }

        public static bool IsImageFile(string Extention)
        {
            foreach (string iEx in new string[5] { ".jpg", ".gif", ".png", ".bmp", ".jpeg" })
            {
                if (Extention.ToLower() == iEx)
                    return true;
            }
            return false;
        }

        public static void EditSize(string OrgFileName, string DesFileName, int WidthLimit, int HeightLimit)
        {
            if (System.IO.File.Exists(Path.Combine(DesFileName)))
            {
                System.IO.File.Delete(Path.Combine(DesFileName));
            }

            ImageFormat Format;
            System.Drawing.Image OrgImg = System.Drawing.Image.FromFile(OrgFileName);
            int DesWidth = OrgImg.Width;
            int DesHeight = OrgImg.Height;
            double ratio = (double)DesWidth / (double)DesHeight;
            Format = OrgImg.RawFormat;
            if (DesWidth <= WidthLimit && DesHeight <= HeightLimit)
            {
                File.Copy(OrgFileName, DesFileName);
                return;
            }

            if (DesWidth > WidthLimit)
            {
                DesWidth = WidthLimit;
                DesHeight = (int)Math.Round(DesWidth / ratio);
            }

            if (DesHeight > HeightLimit)
            {
                DesHeight = HeightLimit;
                DesWidth = (int)Math.Round(DesHeight * ratio);
            }

            Bitmap DesImg = new(DesWidth, DesHeight, PixelFormat.Format24bppRgb);
            DesImg.SetResolution(96, 96);

            Graphics GraphicImg = Graphics.FromImage(DesImg);
            GraphicImg.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicImg.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GraphicImg.PixelOffsetMode = PixelOffsetMode.HighQuality;
            System.Drawing.Rectangle oRectangle = new(0, 0, DesWidth, DesHeight);
            GraphicImg.DrawImage(OrgImg, oRectangle);
            OrgImg.Dispose();
            DesImg.Save(DesFileName, Format);
        }

        public static string GetFileSize(string FileName)
        {
            long Bytes = 0;
            if (File.Exists(FileName))
            {
                System.IO.FileInfo f = new System.IO.FileInfo(FileName);
                Bytes = f.Length;
            }

            if (Bytes >= 1073741824)
            {
                Decimal size = Decimal.Divide(Bytes, 1073741824);
                return String.Format("{0:##.##} GB", size);
            }
            else if (Bytes >= 1048576)
            {
                Decimal size = Decimal.Divide(Bytes, 1048576);
                return String.Format("{0:##.##} MB", size);
            }
            else if (Bytes >= 1024)
            {
                Decimal size = Decimal.Divide(Bytes, 1024);
                return String.Format("{0:##.##} KB", size);
            }
            else if (Bytes > 0 & Bytes < 1024)
            {
                Decimal size = Bytes;
                return String.Format("{0:##.##} Bytes", size);
            }
            else
            {
                return "0 Bytes";
            }
        }

        public static string FormatDateTimeToShortDate(DateTime TimeInput)
        {
            double DaySpan = (DateTime.Now - TimeInput).TotalDays;
            double HourSpan = (DateTime.Now - TimeInput).TotalHours;
            double MinuteSpan = (DateTime.Now - TimeInput).TotalMinutes;

            if (MinuteSpan < 1)
                return "Vừa xong";

            if (MinuteSpan < 60)
                return Math.Truncate(MinuteSpan) + " phút";

            if (MinuteSpan >= 60 && HourSpan < 24)
                return Math.Truncate(HourSpan) + " giờ";

            if (DaySpan < 2)
                return "Hôm qua";

            string Result = string.Format("{0:dd/MM/yyyy}", TimeInput);
            return Result;
        }

        public static int CheckTimeToRenew(DateTime dateTime)
        {
            int spaceMinutes = 0;
            try
            {
                Int32 unixTimestampInput = (Int32)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
                Int32 unixTimestampCurrent = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
                spaceMinutes = (unixTimestampCurrent - unixTimestampInput);

                return spaceMinutes;
            }
            catch
            {
                return spaceMinutes;
            }
        }

        public static string ConvertTimeSpaceToString(DateTime dateTimeFrom, DateTime dateTimeEnd)
        {
            string res = "";
            Int32 unixTimestampInput = (Int32)(dateTimeFrom.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
            Int32 unixTimestampCurrent = (Int32)(dateTimeEnd.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
            var spaceMinutes = (unixTimestampCurrent - unixTimestampInput);
            var hours = spaceMinutes / 60;
            var minutes = spaceMinutes - hours * 60;
            res = String.Format("{0}h {1} phút", hours, minutes);
            return res;
        }

        public static string RenderPrice(object Price)
        {
            string Result = "Liên hệ";
            if (Price != null)
            {
                double PriceNumber = Convert.ToDouble(Price);
                if (PriceNumber > 0)
                {
                    Result = ShowNumber(PriceNumber, 0);
                }
            }
            return Result;
        }

        public static string RenderPriceRecruitment(object PriceFrom, object PriceTo)
        {
            string Result = "Thỏa thuận";
            if (PriceFrom != null && PriceTo != null)
            {
                double PriceNumberFrom = Convert.ToDouble(PriceFrom);
                double PriceNumberTo = Convert.ToDouble(PriceTo);
                if (PriceNumberFrom > 0 && PriceNumberTo > 0)
                {
                    Result = ShowNumber(PriceNumberFrom, 0) + " ~ " + ShowNumber(PriceNumberTo, 0);
                }
            }
            else if (PriceFrom != null && PriceTo == null)
            {
                double PriceNumberFrom = Convert.ToDouble(PriceFrom);
                if (PriceNumberFrom > 0)
                {
                    Result = ShowNumber(PriceNumberFrom, 0) + " ~ " + Result;
                }
            }
            return Result;
        }

        public static string RenderNumber(object Number)
        {
            string Result = "Không xác định";
            if (Number != null)
            {
                double PriceNumber = Convert.ToDouble(Number);
                if (PriceNumber > 0)
                {
                    Result = ShowNumber(PriceNumber, 0);
                }
            }
            return Result;
        }

        public static string RenderNumberYear(object Number)
        {
            string Result = "Không xác định";
            if (Number != null)
            {
                double PriceNumber = Convert.ToDouble(Number);
                if (PriceNumber > 0)
                {
                    Result = PriceNumber.ToString();
                }
            }
            return Result;
        }

        public static string RenderString(object InputObject)
        {
            string Result = "Không xác định";
            if (InputObject != null)
            {
                string InputText = InputObject.ToString();
                if (!string.IsNullOrEmpty(InputText))
                {
                    Result = InputText;
                }
            }
            return Result;
        }

        public static bool IsEmail(string email)
        {
            try
            {
                string pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-\.]{1,}$";
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);

                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPhoneNumber(string number)
        {
            if (String.IsNullOrEmpty(number)) return false;
            string RegexE;
            List<string> lst = PhoneList();
            foreach (var p in lst)
            {
                RegexE = @"^" + p + @"([0-9]{1,7})$";
                if (Regex.Match(number, RegexE).Success)
                {
                    return true;
                }
            }

            return false;
        }

        public static List<string> PhoneList()
        {
            List<string> lst = new List<string>
            {
                //Viettel
                "032",
                "033",
                "034",
                "035",
                "036",
                "037",
                "038",
                "039",
                "086",
                "096",
                "097",
                "098",
                //MobilePhone
                "070",
                "079",
                "077",
                "076",
                "078",
                "089",
                "090",
                "093",
                //VinaPhone
                "083",
                "084",
                "085",
                "081",
                "082",
                "088",
                "091",
                "094",
                //VietNamMobile
                "092",
                "056",
                "058",
                //GPhone
                "099",
                "059"
            };
            return lst;
        }

        public static bool IsEmailOrPhone(string input)
        {
            if (IsEmail(input)) return true;
            if (IsPhoneNumber(input)) return true;
            return false;
        }

        public static string Decode(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;
            var decoded = html.Trim();
            if (!HasTags(decoded))
                return html;
            decoded = NonExplicitLines1.Replace(decoded, string.Empty);
            decoded = DivEndings1.Replace(decoded, Environment.NewLine);
            decoded = LineBreaks1.Replace(decoded, Environment.NewLine);
            decoded = Tags1.Replace(decoded, string.Empty).Trim();
            return WebUtility.HtmlDecode(decoded);
        }

        private static bool HasTags(string str)
        {
            return str.StartsWith("<") && str.EndsWith(">");
        }

        public static void MoveFile(string filePathSource, string filePathTarget)
        {
            try
            {
                if (System.IO.File.Exists(filePathTarget))
                    System.IO.File.Delete(filePathTarget);
                System.IO.File.Move(filePathSource, filePathTarget);
            }
            catch
            {
            }
        }

        public static Stream ResizeImageStream(Stream inputStream, int WidthLimit, int HeightLimit)
        {
            ImageFormat Format;
            System.Drawing.Image OrgImg = System.Drawing.Image.FromStream(inputStream);
            int DesWidth = OrgImg.Width;
            int DesHeight = OrgImg.Height;
            double ratio = (double)DesWidth / (double)DesHeight;
            Format = OrgImg.RawFormat;
            if (DesWidth <= WidthLimit && DesHeight <= HeightLimit)
            {
                return inputStream;
            }

            if (DesWidth > WidthLimit)
            {
                DesWidth = WidthLimit;
                DesHeight = (int)Math.Round(DesWidth / ratio);
            }

            if (DesHeight > HeightLimit)
            {
                DesHeight = HeightLimit;
                DesWidth = (int)Math.Round(DesHeight * ratio);
            }

            Bitmap DesImg = new Bitmap(DesWidth, DesHeight, PixelFormat.Format24bppRgb);
            DesImg.SetResolution(96, 96);

            Graphics GraphicImg = Graphics.FromImage(DesImg);
            GraphicImg.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicImg.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GraphicImg.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle oRectangle = new(0, 0, DesWidth, DesHeight);
            GraphicImg.DrawImage(OrgImg, oRectangle);
            OrgImg.Dispose();
            MemoryStream memoryStream = new();
            DesImg.Save(memoryStream, Format);
            return memoryStream;
        }

        public static Stream GetStream(Image img, ImageFormat format)
        {
            var ms = new MemoryStream();
            img.Save(ms, format);
            return ms;
        }

        public static string RenderTimeSince(DateTime datetime)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;
            var ts = new TimeSpan(DateTime.Now.Ticks - datetime.Ticks);
            if (ts.Seconds < 0 || ts.Minutes < 0 || ts.Hours < 0 || ts.Days < 0)
            {
                ts = -(ts);
            }
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "1 giây trước" : ts.Seconds + " giây trước";

            if (delta < 2 * MINUTE)
                return "1 phút trước";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " phút trước";

            if (delta < 90 * MINUTE)
                return "1 giờ trước";

            if (delta < 24 * HOUR)
                return ts.Hours + " giờ trước";

            if (delta < 48 * HOUR)
                return "hôm qua";

            if (delta < 30 * DAY)
                return ts.Days + " ngày trước";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "1 tháng trước" : months + " tháng trước";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "1 năm trước" : years + " năm trước";
            }
        }

        public static int CountWords(string text)
        {
            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }
            return wordCount;
        }

        public static string GetBase64Image(string base64File)
        {
            Regex regex = new(@"^[\w/\:.-]+;base64,");
            base64File = regex.Replace(base64File, string.Empty);
            return base64File;
        }

        public static string GetBase64ImageMime(string base64File)
        {
            string mimeType = "";

            Match match = Regex.Match(base64File, @"(?<=image\/)(.*?)(?=;)");
            if (match.Success)
            {
                mimeType = match.Value;
            }
            return mimeType;
        }

        public  static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}