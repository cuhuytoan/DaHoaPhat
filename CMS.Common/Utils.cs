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
        public static Dictionary<int, string> LocationDictionary = new Dictionary<int, string> { { -1, "Tất cả tỉnh thành" }, { 1, "An Giang" }, { 2, "Bà Rịa - Vũng Tàu" }, { 3, "Bắc Giang" }, { 4, "Bắc Kạn" }, { 5, "Bạc Liêu" }, { 6, "Bắc Ninh" }, { 7, "Bến Tre" }, { 8, "Bình Định" }, { 9, "Bình Dương" }, { 10, "Bình Phước" }, { 11, "Bình Thuận" }, { 12, "Cà Mau" }, { 13, "Cao Bằng" }, { 14, "Đắk Lắk" }, { 15, "Đắk Nông" }, { 16, "Điện Biên" }, { 17, "Đồng Nai" }, { 18, "Đồng Tháp" }, { 19, "Gia Lai" }, { 20, "Hà Giang" }, { 21, "Hà Nam" }, { 22, "Hà Tĩnh" }, { 23, "Hải Dương" }, { 24, "Hậu Giang" }, { 25, "Hòa Bình" }, { 26, "Hưng Yên" }, { 27, "Khánh Hòa" }, { 28, "Kiên Giang" }, { 29, "Kon Tum" }, { 30, "Lai Châu" }, { 31, "Lâm Đồng" }, { 32, "Lạng Sơn" }, { 33, "Lào Cai" }, { 34, "Long An" }, { 35, "Nam Định" }, { 36, "Nghệ An" }, { 37, "Ninh Bình" }, { 38, "Ninh Thuận" }, { 39, "Phú Thọ" }, { 40, "Quảng Bình" }, { 41, "Quảng Nam" }, { 42, "Quảng Ngãi" }, { 43, "Quảng Ninh" }, { 44, "Quảng Trị" }, { 45, "Sóc Trăng" }, { 46, "Sơn La" }, { 47, "Tây Ninh" }, { 48, "Thái Bình" }, { 49, "Thái Nguyên" }, { 50, "Thanh Hóa" }, { 51, "Thừa Thiên Huế" }, { 52, "Tiền Giang" }, { 53, "Trà Vinh" }, { 54, "Tuyên Quang" }, { 55, "Vĩnh Long" }, { 56, "Vĩnh Phúc" }, { 57, "Yên Bái" }, { 58, "Phú Yên" }, { 59, "Cần Thơ" }, { 60, "Đà Nẵng" }, { 61, "Hải Phòng" }, { 62, "Hà Nội" }, { 63, "TP HCM" } };
        public static Dictionary<string, string> MetaSettings = new Dictionary<string, string>() { };
        private static Random random = new Random();
        private static readonly Regex NonExplicitLines = new Regex("\r|\n", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex DivEndings = new Regex("</div>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex LineBreaks = new Regex("</br\\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex Tags = new Regex("<[^>]*>", RegexOptions.Compiled);

        private readonly IHostEnvironment _env;
        public Utils(IHostEnvironment env)
        {
            _env = env;
        }

        public static string GetResources(string ResourceName, string Key)
        {
            ResourceManager Resources = new ResourceManager("Resources." + ResourceName, System.Reflection.Assembly.Load("App_GlobalResources"));
            return Resources.GetString(Key);
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomNumber(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetRandomText()
        {
            StringBuilder randomText = new StringBuilder();
            string alphabets = "2345679ABCEGHKMNPSXZabceghkmnpqstuvxz";
            Random r = new Random();
            for (int j = 0; j <= 2; j++)
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

        public static void AddCounterWebsite()
        {

        }

        public static string CutText(string TextInput, int NumberCharacter)
        {
            string Result = "";
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
            Result = TextInput.Substring(0, index);
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
            return tmp.Substring(0, tmp.Length - 4);
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

            Bitmap DesImg = new System.Drawing.Bitmap(DesWidth, DesHeight, PixelFormat.Format24bppRgb);
            DesImg.SetResolution(96, 96);

            Graphics GraphicImg = Graphics.FromImage(DesImg);
            GraphicImg.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicImg.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GraphicImg.PixelOffsetMode = PixelOffsetMode.HighQuality;
            System.Drawing.Rectangle oRectangle = new Rectangle(0, 0, DesWidth, DesHeight);
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
            string Result = "";
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

            Result = String.Format("{0:dd/MM/yyyy}", TimeInput);
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
            List<string> lst = new List<string>();
            //Viettel
            lst.Add("032");
            lst.Add("033");
            lst.Add("034");
            lst.Add("035");
            lst.Add("036");
            lst.Add("037");
            lst.Add("038");
            lst.Add("039");
            lst.Add("086");
            lst.Add("096");
            lst.Add("097");
            lst.Add("098");
            //MobilePhone
            lst.Add("070");
            lst.Add("079");
            lst.Add("077");
            lst.Add("076");
            lst.Add("078");
            lst.Add("089");
            lst.Add("090");
            lst.Add("093");
            //VinaPhone
            lst.Add("083");
            lst.Add("084");
            lst.Add("085");
            lst.Add("081");
            lst.Add("082");
            lst.Add("088");
            lst.Add("091");
            lst.Add("094");
            //VietNamMobile
            lst.Add("092");
            lst.Add("056");
            lst.Add("058");
            //GPhone
            lst.Add("099");
            lst.Add("059");
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
            decoded = NonExplicitLines.Replace(decoded, string.Empty);
            decoded = DivEndings.Replace(decoded, Environment.NewLine);
            decoded = LineBreaks.Replace(decoded, Environment.NewLine);
            decoded = Tags.Replace(decoded, string.Empty).Trim();
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
            catch (Exception ex)
            {
                var res = ex;
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

            Bitmap DesImg = new System.Drawing.Bitmap(DesWidth, DesHeight, PixelFormat.Format24bppRgb);
            DesImg.SetResolution(96, 96);

            Graphics GraphicImg = Graphics.FromImage(DesImg);
            GraphicImg.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicImg.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GraphicImg.PixelOffsetMode = PixelOffsetMode.HighQuality;
            System.Drawing.Rectangle oRectangle = new Rectangle(0, 0, DesWidth, DesHeight);
            GraphicImg.DrawImage(OrgImg, oRectangle);
            OrgImg.Dispose();
            MemoryStream memoryStream = new MemoryStream();
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
    }
}
