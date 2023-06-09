/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Globalization;

namespace MnM.GWS
{
    /// <summary>
    /// Class DateHelper.
    /// </summary>
    public static class Dates
    {
        #region variables/consts
        const string dateformat = "{0}-{1}-{2}";

        const string timeformat = "{0}:{1}:{2}.{3}";

        /// <summary>
        /// The DTF
        /// </summary>
        private static DateTimeFormatInfo DTF = new DateTimeFormatInfo();
        #endregion

        #region Change
#if Dates
        /// <summary>
        /// Changes the specified change type.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="change">The change.</param>
        /// <param name="minDate">The minimum date.</param>
        /// <param name="maxDate">The maximum date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Change(this DateTime date, DateChangeType changeType, int change,
           DateTime? minDate = null, DateTime? maxDate = null)
        {
            if (change == 0) return date;

            DateTime dt = DateTime.MinValue;
            var mindt = (minDate == null) ? DateTime.MinValue : minDate.Value;
            var maxdt = (maxDate == null) ? DateTime.MaxValue : maxDate.Value;

            switch (changeType)
            {
                case DateChangeType.YearIncrement:
                    dt = date.AddYears(change);
                    break;
                case DateChangeType.MonthIncrement:
                    dt = date.AddMonths(change);
                    break;
                case DateChangeType.DayIncrement:
                    dt = date.AddDays(change);
                    break;
                case DateChangeType.DateReplacement:
                    dt = change.ToDate();
                    break;
                case DateChangeType.YearReplacement:
                    try
                    {
                        string Digits = change.ToString();
                        if (Digits.Length > 4) return date;

                        dt = DateSerial(ConcateLeftOfYear(date, Digits), date.Month, date.Day);
                        var max = DateSerial(ConcateLeftOfYear(maxdt, Digits), date.Month, date.Day);
                        var min = DateSerial(ConcateLeftOfYear(mindt, Digits), date.Month, date.Day);

                        if (Verify(dt, minDate, maxDate))
                            return dt;
                        if (Verify(max, minDate, maxDate))
                            return max;
                        if (Verify(min, minDate, maxDate))
                            return min;
                        return date;
                    }
                    catch { return date; }

                case DateChangeType.MonthReplacement:
                    try
                    {
                        dt = DateSerial(date.Year, change, date.Day);
                    }
                    catch { }
                    break;
                case DateChangeType.DayReplacement:
                    try
                    {
                        dt = DateSerial(date.Year, date.Month, change);
                    }
                    catch { }
                    break;
            }

            if (Verify(dt, mindt, maxdt) && dt != System.DateTime.MinValue)
                return dt;
            return date;
        }
       
        /// <summary>
        /// Changes the specified change type.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="change">The change.</param>
        /// <param name="minDate">The minimum date.</param>
        /// <param name="maxDate">The maximum date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Change(this object date, DateChangeType changeType, int change,
           object minDate = null, object maxDate = null)
        {
            var min = (minDate == null) ? DateTime.MinValue : minDate.ToDate();
            var max = (maxDate == null) ? DateTime.MaxValue : maxDate.ToDate();
            return date.ToDate().Change(changeType: changeType, change: change,
                minDate: min, maxDate: max);
        }
#endif
        /// <summary>
        /// Changes the specified change.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="change">The change.</param>
        /// <param name="minDate">The minimum date.</param>
        /// <param name="maxDate">The maximum date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Change(this DateTime date, DateTime change,
           DateTime? minDate = null, DateTime? maxDate = null)
        {
            var mindt = (minDate == null) ? DateTime.MinValue : minDate.Value;
            var maxdt = (maxDate == null) ? DateTime.MaxValue : maxDate.Value;
            if (Verify(change, mindt, maxdt) && change != System.DateTime.MinValue)
                return change;
            return date;
        }

        /// <summary>
        /// Changes the specified change.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="change">The change.</param>
        /// <param name="minDate">The minimum date.</param>
        /// <param name="maxDate">The maximum date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Change(object date, object change,
          object minDate = null, object maxDate = null)
        {
            DateTime dt, ch, min, max;
            if (!date.ToDate(out dt)) return DateTime.MinValue;
            if (!change.ToDate(out ch)) return DateTime.MinValue;

            min = (minDate == null) ? DateTime.MinValue : minDate.ToDate();
            max = (maxDate == null) ? DateTime.MaxValue : maxDate.ToDate();

            return dt.Change(change: ch, minDate: min, maxDate: max);
        }
        #endregion

        #region Compare
#if Dates
        /// <summary>
        /// Compares the specified date2.
        /// </summary>
        /// <param name="date1">The date1.</param>
        /// <param name="date2">The date2.</param>
        /// <param name="toReturn">To return.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Compare(this DateTime date1, DateTime date2, ReturnDate toReturn)
        {
            return (date1 < date2) ? ((toReturn == ReturnDate.Smaller) ? date1 : date2) :
               ((toReturn == ReturnDate.Bigger) ? date1 : date2);
        }
        /// <summary>
        /// Compares the specified date2.
        /// </summary>
        /// <param name="date1">The date1.</param>
        /// <param name="date2">The date2.</param>
        /// <param name="DateToReturn">The date to return.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Compare(this object date1, object date2, ReturnDate DateToReturn)
        {
            DateTime dt1 = date1.ToDate();
            DateTime dt2 = date2.ToDate();
            return Compare(dt1, dt2, DateToReturn);
        }
        /// <summary>
        /// Compares the specified date2.
        /// </summary>
        /// <param name="date1">The date1.</param>
        /// <param name="date2">The date2.</param>
        /// <returns>ReturnDate.</returns>
        public static ReturnDate Compare(this DateTime date1, DateTime date2)
        {
            return (date1 < date2) ? ReturnDate.Smaller : ReturnDate.Bigger;
        }
        /// <summary>
        /// Compares the specified date2.
        /// </summary>
        /// <param name="date1">The date1.</param>
        /// <param name="date2">The date2.</param>
        /// <returns>ReturnDate.</returns>
        public static ReturnDate Compare(this object date1, object date2)
        {
            System.DateTime mDate1 = date1.ToDate();
            System.DateTime mDate2 = date2.ToDate();
            return Compare(mDate1, mDate2);
        }

        /// <summary>
        /// Compares the specified range1.
        /// </summary>
        /// <param name="compareDate">The compare date.</param>
        /// <param name="range1">The range1.</param>
        /// <param name="range2">The range2.</param>
        /// <param name="defaultReturn">The default return.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Compare
            (this DateTime compareDate, DateTime range1, DateTime range2, ReturnDate defaultReturn)
        {
            return (compareDate >= range1 & compareDate <= range2) ? compareDate :
                ((defaultReturn == ReturnDate.Bigger) ? range2 :
                ((defaultReturn == ReturnDate.Smaller) ? range1 :
                ((compareDate <= range1 && defaultReturn == ReturnDate.NearMatch) ? range1 :
                ((compareDate >= range2 & defaultReturn == ReturnDate.NearMatch) ? range2 :
                DateTime.MinValue))));
        }
        /// <summary>
        /// Compares the specified range1.
        /// </summary>
        /// <param name="compareDate">The compare date.</param>
        /// <param name="range1">The range1.</param>
        /// <param name="range2">The range2.</param>
        /// <param name="defaultReturn">The default return.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Compare(
            this object compareDate, object range1, object range2, ReturnDate defaultReturn)
        {
            DateTime cmp;
            if (!compareDate.ToDate(out cmp)) return DateTime.MinValue;
            var r1 = range1.ToDate();
            var r2 = range2.ToDate();
            return cmp.Compare(r1, r2, defaultReturn);
        }
#endif

        /// <summary>
        /// Verifies the specified range1.
        /// </summary>
        /// <param name="compareDate">The compare date.</param>
        /// <param name="range1">The range1.</param>
        /// <param name="range2">The range2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Verify(this DateTime compareDate, DateTime range1, DateTime range2)
        {
            return compareDate >= range1 && compareDate <= range2;
        }

        /// <summary>
        /// Verifies the specified range1.
        /// </summary>
        /// <param name="compareDate">The compare date.</param>
        /// <param name="range1">The range1.</param>
        /// <param name="range2">The range2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Verify(this object compareDate, object range1, object range2)
        {
            DateTime cmp;
            if (!compareDate.ToDate(out cmp)) return false;
            var r1 = range1.ToDate();
            var r2 = range2.ToDate();
            return cmp.Verify(r1, r2);
        }
        #endregion

        #region Convert
#if Dates
        /// <summary>
        /// Converts the in hm.
        /// </summary>
        /// <param name="hm">The hm.</param>
        /// <param name="difference">The difference.</param>
        /// <returns>System.Double.</returns>
        public static double ConvertInHM(this double hm, HMDifference difference = HMDifference.AsItIs)
        {
            double result = (hm > 59) ?
                Math.Round((hm - hm % 60) / 60 + (hm % 60) / 100, 2) :
                Math.Round(hm, 2) / 100;

            switch (difference)
            {
                case HMDifference.AsItIs:
                    return result;
                case HMDifference.ZeroIFNegative:
                    return (result < 0) ? 0 : result;
                case HMDifference.Positive:
                    return Math.Abs(result);
                default:
                    return 0;
            }
        }
      
        /// <summary>
        /// Times the duration in hm.
        /// </summary>
        /// <param name="time1">The time1.</param>
        /// <param name="time2">The time2.</param>
        /// <param name="difference">The difference.</param>
        /// <returns>System.Double.</returns>
        public static double TimeDurationInHM(this string time1, string time2,
            HMDifference difference = HMDifference.AsItIs)
        {
            if (ToDate(time1, out DateTime t1, true) & ToDate(time2, out DateTime t2, true))
            {
                double duration = t1.GetDifference(t2);
                return duration.ConvertInHM(difference);
            }
            return 0;
        }
#endif

        /// <summary>
        /// Converts the in minutes.
        /// </summary>
        /// <param name="hm">The hm.</param>
        /// <returns>System.Double.</returns>
        public static double ConvertInMinutes(this double hm)
        {
            return Math.Round((hm - (hm - hm.Fix())) * 60 +
                (hm - hm.Fix()) * 100);
        }

        public static double ConvertInMonths(this double value)
        {
            return value.Fix() * 12 +
                ((value - value.Fix()) * 10);
        }

        /// <summary>
        /// Times the specified hours.
        /// </summary>
        /// <param name="Hours">The hours.</param>
        /// <param name="Minutes">The minutes.</param>
        /// <param name="Seconds">The seconds.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan Time(int Hours, int Minutes = 0, int Seconds = 0)
        {
            int unit;
            if (Seconds > 59)
            {
                unit = Seconds / 60;
                Seconds = Seconds % 60;
                Minutes += unit;
            }
            if (Minutes > 59)
            {
                unit = Minutes / 60;
                Minutes = Minutes / 60;
                Hours += unit;
            }
            if (Hours > 23)
            {
                Hours /= 24;
            }
            return new DateTime(1, 1, 1, Hours, Minutes, Seconds).TimeOfDay;
        }
        #endregion

        #region Correct DateRange
#if Dates
        /// <summary>
        /// Corrects the range.
        /// </summary>
        /// <param name="date1">The date1.</param>
        /// <param name="date2">The date2.</param>
        public static void CorrectRange(ref object date1, ref object date2)
        {
            DateTime dt1 = date1.ToDate();
            DateTime dt2 = date2.ToDate();

            if (dt1 == DateTime.MinValue)
                date1 = date2;
            else if (dt2 == DateTime.MinValue)
                date2 = date1;
            else
            {
                date1 = Compare(date1, date2, ReturnDate.Smaller);
                date2 = Compare(date1, date2, ReturnDate.Bigger);
            }
        }
        /// <summary>
        /// Corrects the range.
        /// </summary>
        /// <param name="date1">The date1.</param>
        /// <param name="time1">The time1.</param>
        /// <param name="date2">The date2.</param>
        /// <param name="time2">The time2.</param>
        public static void CorrectRange(ref string date1, string time1, ref string date2, string time2)
        {
            DateTime dt1 = (date1 + " " + time1).Trim().ToDate();
            DateTime dt2 = (date2 + " " + time2).Trim().ToDate();

            if (dt1 == DateTime.MinValue)
            {
                date1 = dt2.ToShortDateString();
                time1 = dt2.ToShortTimeString();
            }
            else if (dt2 == DateTime.MinValue)
            {
                date2 = dt1.ToShortDateString();
                time2 = dt1.ToShortTimeString();
            }
            else
            {
                date1 = Compare(dt1, dt2, ReturnDate.Smaller).ToShortDateString();
                date2 = Compare(date1, date2, ReturnDate.Bigger).ToShortDateString();
                time1 = dt1.ToShortTimeString();
                time2 = dt2.ToShortTimeString();
            }
        }
#endif
        #endregion

        #region Date Difference
#if Dates
        /// <summary>
        /// Gets the difference.
        /// </summary>
        /// <param name="date1">The date1.</param>
        /// <param name="date2">The date2.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>System.Int32.</returns>
        public static int GetDifference(this DateTime date1, DateTime date2, DateInterval interval = DateInterval.Minute)
        {
            TimeSpan Tspan;
            Tspan = date2.Subtract(date1);

            switch (interval)
            {
                case DateInterval.Year:
                    return date2.Year - date1.Year;

                case DateInterval.Month:
                    return (date2.Month - date1.Month) + (12 * (date2.Year - date1.Year));

                case DateInterval.Weekday:
                    return (int)(Tspan.TotalDays.Fix() / 7);

                case DateInterval.Day:
                    return (int)Tspan.TotalDays.Fix();

                case DateInterval.Hour:
                    return (int)Tspan.TotalHours.Fix();

                case DateInterval.Minute:
                    return (int)Tspan.TotalMinutes.Fix();

                default:
                    return (int)Tspan.TotalSeconds.Fix();
            }
        }
        /// <summary>
        /// Gets the difference.
        /// </summary>
        /// <param name="date1">The date1.</param>
        /// <param name="date2">The date2.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>System.Int32.</returns>
        public static int GetDifference(this object date1, object date2, DateInterval interval = DateInterval.Minute)
        {
            DateTime dt1, dt2;
            if (!date1.ToDate(out dt1)) return 0;
            if (!date2.ToDate(out dt2)) return 0;
            return dt1.GetDifference(dt2, interval);
        }
#endif
        #endregion

        #region Date Serial
        /// <summary>
        /// Dates the serial.
        /// </summary>
        /// <param name="Year">The year.</param>
        /// <param name="Month">The month.</param>
        /// <param name="Day">The day.</param>
        /// <returns>DateTime.</returns>
        public static DateTime DateSerial(int Year, int Month, int Day)
        {
            Day = Day.SingleCompare(1, CompareReturn.Big);
            Month = Month.SingleCompare(1, CompareReturn.Big);
            Year = Year.RangeCompare(1753, 9999, CompareReturn.Nearest);

            return new DateTime(Year, Month, Day);
        }
        /// <summary>
        /// Dates the serial.
        /// </summary>
        /// <param name="Year">The year.</param>
        /// <param name="Month">The month.</param>
        /// <param name="Day">The day.</param>
        /// <returns>DateTime.</returns>
        public static DateTime DateSerial(object Year, object Month, object Day)
        {
            if (Numbers.ToNumber(Year, out int y) &&
                Numbers.ToNumber(Month, out int m) &&
                Numbers.ToNumber(Day, out int d))
                return DateSerial(y, m, d);
            return DateTime.MinValue;
        }
        #endregion

        #region First & Last Date
        /// <summary>
        /// Firsts the date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime FirstDate(this object date)
        {
            var dt = date.ToDate();
            return new DateTime(dt.Year, dt.Month, 1);
        }
        /// <summary>
        /// Firsts the date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime FirstDate(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
        /// <summary>
        /// Lasts the date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime LastDate(this object date)
        {
            return (date.ToDate().AddMonths(1)).FirstDate().AddDays(-1);
        }
        /// <summary>
        /// Lasts the date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime LastDate(this DateTime date)
        {
            return (date.AddMonths(1)).FirstDate().AddDays(-1);
        }
        #endregion

        #region Get Month Name
        /// <summary>
        /// Monthes the name.
        /// </summary>
        /// <param name="Month">The month.</param>
        /// <param name="Abbreviate">if set to <c>true</c> [abbreviate].</param>
        /// <returns>System.String.</returns>
        public static string MonthName(this int Month, bool Abbreviate = false)
        {
            string mMonthName;
            mMonthName = DTF.GetMonthName(Month);
            if (Abbreviate)
            {
                return (mMonthName.Substring(0, 3));
            }
            else
            {
                return mMonthName;
            }

        }
        #endregion

        #region Last Day in Month
        /// <summary>
        /// Lasts the day in month.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.Int32.</returns>
        public static int LastDayInMonth(this DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month);
        }
        /// <summary>
        /// Lasts the day in month.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.Int32.</returns>
        public static int LastDayInMonth(this object date)
        {
            return date.ToDate().LastDayInMonth();
        }
        #endregion

        #region IS DATE
        public static bool IsDate(this object value)
        {
            if (value is DateTime)
                return true;
            else
            {
                var o = value?.ToString();

                if (string.IsNullOrEmpty(o))
                    return false;
                if (o.ToDate(out DateTime dt))
                    return true;
            }
            return false;
        }
        #endregion

        #region TO DATE
        public static bool ToDate(this object value, out DateTime result, bool includeTime = false)
        {
            result = DateTime.MinValue;
            if (value == null)
                return false;
            if (value is DateTime)
            {
                result = DateTime.Parse(value.ToString());
                return true;
            }
            var vtype = includeTime ? ValidateType.ddMMyyyyHHmm : ValidateType.ddMMyyyy;
            var dt = value.ToString().parseDateTime(vtype, out bool vdate,
                out bool vtime, out ValidateType vd, out ValidateType vt);
            if (dt == null)
                return false;
            result = dt.Value;

            return true;
        }
        public static DateTime ToDate(this object value, bool includeTime = false)
        {
            ToDate(value, out DateTime dt, includeTime);
            return dt;
        }
        public static bool ToDate(this object value, out DateTime result, DateTime min, DateTime max, bool includeTime = false)
        {
            if (!ToDate(value, out result, includeTime))
                return false;
            if (result >= min && result <= max)
                return true;
            return false;

        }
        public static DateTime ToDate(this object value, DateTime min, DateTime max, bool includeTime = false)
        {
            ToDate(value, out DateTime dt, includeTime);
            if (dt >= min && dt <= max)
                return dt;
            return DateTime.MinValue;
        }
        #endregion

        #region PARSE DATE TIME
        internal static DateTime? parseDateTime(this string word, ValidateType validation) =>
            parseDateTime(word, validation, out bool vdt, out bool vt, out ValidateType vtype, out ValidateType tyype);

        internal static DateTime? parseDateTime(this string word, ValidateType validation,
            out bool vdate, out bool vtime, out ValidateType vdateval, out ValidateType vtimeval)
        {
            DateTime? date = null;

            vdate = validation.Includes(out vdateval,
                   ValidateType.ddMMyyyy,
                   ValidateType.ddMMyy,
                   ValidateType.MMddyy,
                   ValidateType.MMddyyyy,
                   ValidateType.yyMMdd,
                   ValidateType.yyyyMMdd);

            vtime = validation.Includes(out vtimeval,
                ValidateType.HHmm,
                ValidateType.hhmm12,
                ValidateType.HHmmss,
                ValidateType.hhmmss12,
                ValidateType.HHmmssms,
                ValidateType.hhmmssms12);

            if (vdate || vtime)
            {
                if (vdate)
                    date = word.parseDate();

                if (vtime)
                {
                    if (vdate)
                    {
                        if (word.Contains("."))
                            word = word.Substring(word.IndexOf(".") + 1);
                    }
                    var dt = word.parseTime();

                    if (dt != null)
                        date += dt.Value.TimeOfDay;
                    else date = dt;
                }
            }
            return date;
        }

        static DateTime? parseDate(this string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            input = input.Trim();

            if (DateTime.TryParse(input, out DateTime dt)) return dt;

            if (input.IndexOf('.') != -1)
                input = input.Substring(0, input.IndexOf('.'));

            if (!int.TryParse(input, out int val)) return null;

            string[] result = new string[0];


            var d = input.ToCharArray();

            switch (d.Length)
            {
                case 3:
                    result = new string[1];

                    result[0] = (string.Format(dateformat,
                        d[0], d[1], d[2]));
                    break;
                case 4:
                    result = new string[3];

                    result[0] = (string.Format(dateformat,
                        d[0], d[1], "" + d[2] + d[3]));

                    result[1] = (string.Format(dateformat,
                        "" + d[0] + d[1], d[2], d[3]));

                    result[1] = (string.Format(dateformat,
                        d[0], "" + d[1] + d[2], d[3]));
                    break;
                case 5:
                    result = new string[3];

                    result[0] = (string.Format(dateformat,
                        "" + d[0] + d[1], d[2], "" + d[3] + d[4]));

                    result[1] = (string.Format(dateformat,
                        d[0], "" + d[1] + d[2], "" + d[3] + d[4]));

                    result[2] = (string.Format(dateformat,
                        "" + d[0] + d[1], "" + d[2] + d[3], d[4]));
                    break;
                case 6:
                    result = new string[2];

                    result[0] = (string.Format(dateformat,
                        "" + d[0] + d[1], "" + d[2] + d[3], "" + d[4] + d[5]));

                    result[1] = (string.Format(dateformat,
                        d[0], d[1], "" + d[2] + d[3] + d[4] + d[5]));
                    break;
                case 7:
                    result = new string[2];

                    result[0] = (string.Format(dateformat,
                        "" + d[0] + d[1], d[2], "" + d[3] + d[4] + d[5] + d[6]));

                    result[1] = (string.Format(dateformat,
                        d[0], "" + d[1] + d[2], "" + d[3] + d[4] + d[5] + d[6]));
                    break;
                case 8:
                    result = new string[1];

                    result[0] = (string.Format(dateformat,
                    "" + d[0] + d[1], "" + d[2] + d[3], "" + d[4] + d[5] + d[6] + d[7]));
                    break;

                default:
                    return null;
            }
            foreach (var item in result)
            {
                if (DateTime.TryParse(item, out DateTime date))
                    return date;
            }
            return null;
        }
        static DateTime? parseTime(this string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            input = input.Trim();

            if (DateTime.TryParse(input, out DateTime dt)) return dt;

            if (input.IndexOf('.') != -1)
                input = input.Substring(input.IndexOf('.') + 1);

            if (!int.TryParse(input, out int val)) return null;

            int num, j = 0;
            string result;
            string[] temp = new string[] { "00", "00", "00", "00", "00" };

            input = input.PadRight(10, '0');
            for (int i = 0; i < 6; i = i + 2)
            {
                num = Convert.ToInt32("" + input[i] + input[i + 1]);
                bool manipulate = (i < 2) ? num > 23 : num > 59;

                if (manipulate)
                {
                    temp[j] = "0" + input[i];
                    temp[j + 1] = "0" + input[i + 1];
                    j += 2;
                }
                else
                {
                    temp[j] = "" + input[i] + input[i + 1];
                    j++;
                }
            }
            temp[3] = input.Substring(6);
            result = string.Format(timeformat, temp[0], temp[1], temp[2], temp[3]);
            if (DateTime.TryParse(result, out DateTime retVal))
                return retVal;
            return null;
        }
        #endregion

        #region Week Day
        public static string WeekDayName(this DateTime date, bool abbreviate = true)
        {
            var day = date.DayOfWeek.ToString();
            if (abbreviate)
                return day.Substring(0, 3);
            return day;
        }
        public static string WeekDayName(this int day, DayOfWeek startDay = DayOfWeek.Monday, bool abbreviate = true)
        {
            var week = day.weekDay(startDay).ToString();
            if (abbreviate)
                return week.Substring(0, 3);
            return week;
        }

        public static int WeekDay(this DateTime DateValue, DayOfWeek weekStart = DayOfWeek.Monday)
        {
            ((int)DateValue.DayOfWeek).weekDay(out int week, weekStart);
            return week;
        }
        public static int WeekDay(this object date, DayOfWeek weekStart = DayOfWeek.Monday) =>
            date.ToDate().WeekDay(weekStart);

        #endregion

        #region Base Functions
        static DayOfWeek weekDay(this int day, DayOfWeek startDay = DayOfWeek.Monday)
        {
            day.weekDay(out int d, startDay);
            return (DayOfWeek)d;
        }
        static void weekDay(this int day, out int calculatedDay, DayOfWeek startDay = DayOfWeek.Monday)
        {
            if (day > 6)
                day = 6;
            if (startDay == DayOfWeek.Sunday)
                goto mks;

            var i = (int)startDay;
            day += i;
            if (day > 6)
                day = day % 6;
            day--;
        mks:
            calculatedDay = day;
        }
        static int ConcateLeftOfYear(this DateTime date, string ConcatePart)
        {
            if (ConcatePart.Length > 4)
                return date.Year;
            else
                return Convert.ToInt32(date.Year.ToString().Substring
                    (0, 4 - ConcatePart.Length) + ConcatePart);
        }
        static int ConcateRightOfYear(this DateTime date, string ConcatePart)
        {
            return (ConcatePart.Length > 4) ? date.Year :
                Convert.ToInt32(ConcatePart +
                    date.Year.ToString().Substring(ConcatePart.Length));
        }
        static int SingleCompare(this int Num1, int Num2, CompareReturn NumToReturn)
        {
            if (NumToReturn == CompareReturn.Small)
            {
                if (Num1 < Num2)
                {
                    return Num1;
                }
                else
                { return Num2; }
            }
            else if (NumToReturn == CompareReturn.Big)
            {
                if (Num1 > Num2)
                {
                    return Num1;
                }
                else
                { return Num2; }
            }
            else
            {
                return Num1;
            }
        }
        static int RangeCompare(this int CompareNumber, int Range1, int Range2,
            CompareReturn NumToReturnIfNotInRange)
        {

            if (CompareNumber >= Range1 & CompareNumber <= Range2)
                return CompareNumber;
            else if (NumToReturnIfNotInRange == CompareReturn.Big)
                return Range2;
            else if (NumToReturnIfNotInRange == CompareReturn.Small)
                return Range1;
            else if (CompareNumber <= Range1 & NumToReturnIfNotInRange == CompareReturn.Nearest)
                return Range1;
            else if (CompareNumber >= Range2 & NumToReturnIfNotInRange == CompareReturn.Nearest)
                return Range2;
            return 0;
        }
        #endregion
    }
}
