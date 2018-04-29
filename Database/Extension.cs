using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;

using LinqKit;

public enum FileSize
{
    KB = 1, MB = 2, GB = 3, TB = 4 // , PB = 5, EB = 6, ZB = 7, YB = 8
}

public static class BasicExtensions
{
    #region Guid

    public static string ToISFormatted(this Guid guid)
    {
        return guid.ToString("N").ToUpper();
    }

    public static string ToISFormatted(this Nullable<Guid> guid, string defaultValue = null)
    {
        if (guid.HasValue) return guid.Value.ToISFormatted();
        return defaultValue;
    }

    //public static string ToStringEx(this object obj)
    //{
    //    if (obj == null) return string.Empty;
    //    if (obj is Guid) return ToISFormatted((Guid)obj);
    //    return obj.ToString();
    //}

    #endregion

    #region String

    /// <summary>
    /// 进行多参数分拆字符串
    /// </summary>
    public static void Split(this string value, char separator,
        Action<int, List<string>> handle, bool sort = false, bool removeDuplicate = false)
    {
        var chars = new char[] { separator };
        if (separator == ',') chars = new char[] { separator, '，' };
        string[] parts = value.Split(chars, StringSplitOptions.RemoveEmptyEntries);
        if (sort) parts = parts.OrderBy(p => p).ToArray();
        if (removeDuplicate) parts = parts.Distinct().ToArray();
        handle(parts.Length, new List<string>(parts));
    }

    /// <summary>
    /// 扩展的 Split，生成 不重复的，非空的，以及可以排序的
    /// </summary>
    public static string[] SplitEx(this string value, char separator = '.', bool sort = false, bool removeDuplicate = false)
    {
        var chars = new char[] { separator };
        if (separator == ',') chars = new char[] { separator, '，' };
        string[] parts = value.Split(chars, StringSplitOptions.RemoveEmptyEntries);
        parts = parts.Where(p => p.Trim().Length > 0).ToArray();
        if (removeDuplicate) parts = parts.Distinct().ToArray();
        if (sort) parts = parts.OrderBy(p => p).ToArray();
        return parts;
    }

    /// <summary>
    /// 进行多参数分拆字符串
    /// </summary>
    public static void SplitEx(this string value,
        Action<string[], int> eachHandler, char separator = ',', bool sort = false, bool removeDuplicate = false)
    {
        var chars = new char[] { separator };
        if (separator == ',') chars = new char[] { separator, '，' };
        string[] parts = value.Split(chars, StringSplitOptions.RemoveEmptyEntries);
        if (sort) parts = parts.OrderBy(p => p).ToArray();
        if (removeDuplicate) parts = parts.Distinct().ToArray();
        for (int i = 0; i < parts.Length; i++) eachHandler(parts, i);
    }

    public static string Merge(this string value,
        string target, char separator = ',', bool sort = true, Action<string, List<string>> addHandle = null)
    {
        var chars = new char[] { separator };
        if (separator == ',') chars = new char[] { separator, '，' };
        List<string> result = new List<string>(
            value.Split(chars, StringSplitOptions.RemoveEmptyEntries));
        List<string> part2 = new List<string>(
            target.Split(chars, StringSplitOptions.RemoveEmptyEntries));
        part2.ForEach(p2 =>
        {
            if (!result.Contains(p2))
            {
                if (addHandle != null) addHandle(p2, result);
                result.Add(p2);
            }
        });
        if (sort) result = (from r in result orderby r select r).ToList();
        return result.ToFlat(new string(new char[] { separator }));
    }

    public static bool ToBooleanOrDefault(this string data, bool defaultValue = false, Action<string> exceptionHandle = null)
    {
        bool result = defaultValue;
        if (bool.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return defaultValue;
    }
    public static Nullable<bool> ToBooleanOrNull(this string data, Action<string> exceptionHandle = null)
    {
        if (string.IsNullOrEmpty(data)) return null;
        bool result = false;
        if (bool.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return null;
    }

    public static double ToDoubleOrDefault(this string data, double defaultValue = 0, Action<string> exceptionHandle = null)
    {
        double result = defaultValue;
        if (double.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return defaultValue;
    }
    public static Nullable<double> ToDoubleOrNull(this string data, Action<string> exceptionHandle = null)
    {
        if (string.IsNullOrEmpty(data)) return null;
        double result = 0;
        if (double.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return null;
    }

    public static decimal ToDecimalOrDefault(this string data, decimal defaultValue = 0, Action<string> exceptionHandle = null)
    {
        decimal result = defaultValue;
        if (decimal.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return defaultValue;
    }
    public static Nullable<decimal> ToDecimalOrNull(this string data, Action<string> exceptionHandle = null)
    {
        if (string.IsNullOrEmpty(data)) return null;
        decimal result = 0;
        if (decimal.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return null;
    }

    public static int ToIntOrDefault(this string data, int defaultValue = 0, Action<string> exceptionHandle = null)
    {
        int result = defaultValue;
        if (int.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return defaultValue;
    }
    public static Nullable<int> ToIntOrNull(this string data, Action<string> exceptionHandle = null)
    {
        if (string.IsNullOrEmpty(data)) return null;
        int result = 0;
        if (int.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return null;
    }

    public static Guid ToGuidOrEmpty(this string data, Action<string> exceptionHandle = null) { return ToGuidOrDefault(data, Guid.Empty, exceptionHandle); }
    public static Guid ToGuidOrDefault(this string data, Guid defaultValue, Action<string> exceptionHandle = null)
    {
        Guid result = Guid.Empty;
        if (Guid.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return defaultValue;
    }
    public static Nullable<Guid> ToGuidOrNull(this string data, Action<string> exceptionHandle = null)
    {
        if (string.IsNullOrEmpty(data)) return null;
        Guid result = Guid.Empty;
        if (Guid.TryParse(data, out result)) return result;
        if (exceptionHandle != null) exceptionHandle(data);
        return null;
    }

    /// <summary>
    /// 由字符串转换为日期
    /// </summary>
    /// <param name="isFormattedString">由符合ToISFormatted的格式反推时间</param>
    /// <returns></returns>
    public static DateTime ToDateTime(this string isFormattedString)
    {
        string[] parts = isFormattedString.SplitEx('-');
        Action<string> exHandle = s =>
        {
            throw new ArgumentException("isFormattedString: " + isFormattedString + " 不是合法的日期字串");
        };

        switch (parts.Length)
        {
            case 1:
                return new DateTime(
                    parts[0].ToIntOrDefault(exceptionHandle: exHandle), 1, 1);
            case 2:
                return new DateTime(
                    parts[0].ToIntOrDefault(exceptionHandle: exHandle),
                    parts[1].ToIntOrDefault(exceptionHandle: exHandle), 1);
            case 3:
                return new DateTime(
                    parts[0].ToIntOrDefault(exceptionHandle: exHandle),
                    parts[1].ToIntOrDefault(exceptionHandle: exHandle),
                    parts[2].ToIntOrDefault(exceptionHandle: exHandle));
            default:
                throw new ArgumentException("isFormattedString: " + isFormattedString + " 不是合法的日期字串");
        }
    }

    /// <summary>
    /// 对长字符串执行分段（按长度），并对每段执行操作
    /// </summary>
    /// <param name="data">需处理的字符串</param>
    /// <param name="computeNext">每次分流（int：当前批次序号, int：剩余字符未处理数，返回：需实际扣减的字符数）</param>
    /// <param name="produceNext">每次获得字串产品（int：当前批次号，int：当前批次，string：给出的字符串，string：返回的字符串）</param>
    /// <returns></returns>
    public static List<string> ToSections(this string data,
        Func<int, int, int> computeNext, Func<int, int, string, string> produceNext = null)
    {
        if (data.Length == 0) return new List<string>();
        int current = data.Length;
        int index = 0;
        List<string> result = new List<string>();
        while (current > 0)
        {
            int next = computeNext(index, current);
            if (next == 0) throw new ArgumentException("Zero length detected", "perPartition");
            int position = data.Length - current;
            int length = next;
            if (current < next) length = current;
            string section = data.Substring(position, length);
            if (produceNext != null) section = produceNext(index, current, section);
            result.Add(section);
            current -= next;
            index++;
        }
        return result;
    }

    /// <summary>
    /// 重复好几次
    /// </summary>
    /// <param name="data"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public static string Repeat(this string data, int times)
    {
        if (times <= 0) return string.Empty;
        var chars = data.ToCharArray();
        var result = new char[chars.Length * times];
        for (int i = 0; i < times; i++)
        {
            Buffer.BlockCopy(chars, 0, result, i * chars.Length * 2, chars.Length * 2);
        }
        return new string(result);
    }

    #endregion

    #region DateTime

    public static bool IsSQLValid(this DateTime dateTime)
    {
        return (
            dateTime > new DateTime(1900, 1, 1) &&
            dateTime < new DateTime(2100, 1, 1));
    }

    #region for dateTime

    /// <summary>
    /// 默认日期格式
    /// </summary>
    private const string DATE_FORMAT = "yyyy-MM-dd";
    /// <summary>
    /// 默认月份格式
    /// </summary>
    private const string MONTH_FORMAT = "yyyy-MM";
    /// <summary>
    /// 默认日期格式（带时间）
    /// </summary>
    private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
    /// <summary>
    /// 默认日期格式（带时间，不含秒）
    /// </summary>
    private const string DATE_TIME_FORMAT_WITHOUT_SEC = "yyyy-MM-dd HH:mm";
    /// <summary>
    /// ID 格式
    /// </summary>
    private const string SF_YEAR_FORMAT = "yyyy";
    private const string SF_MONTH_FORMAT = "yyyyMM";
    private const string SF_DATE_FORMAT = "yyyyMMdd";
    private const string SF_DATE_TIME_FORMAT = "yyyyMMddHHmmss";
    private const string SF_DATE_TIME_FORMAT_WITHOUT_SEC = "yyyyMMddHHmm";

    /// <summary>
    /// 输出 符合全局定义格式的日期型
    /// </summary>
    public static string ToISDate(this DateTime data) { return data.ToString(DATE_FORMAT); }
    public static string ToISDate(this Nullable<DateTime> data, string defaultText = null)
    {
        if (data.HasValue) return ToISDate(data.Value);
        return defaultText ?? string.Empty;
    }

    /// <summary>
    /// 输出 符合全局定义格式的时间类型
    /// </summary>
    /// <param name="data"></param>
    /// <param name="showSecond">是否显示秒（如果是 smalldatetime，无需显示）</param>
    /// <returns></returns>
    public static string ToISDateWithTime(this DateTime data, bool showSecond = true)
    {
        if (showSecond) return data.ToString(DATE_TIME_FORMAT);
        return data.ToString(DATE_TIME_FORMAT_WITHOUT_SEC);
    }
    public static string ToISDateWithTime(this Nullable<DateTime> data, bool showSecond = true)
    {
        if (data.HasValue) return ToISDateWithTime(data.Value, showSecond);
        return string.Empty;
    }

    /// <summary>
    /// 输出 符合全局定义格式的月份信息
    /// </summary>
    public static string ToISMonth(this DateTime data) { return data.ToString(MONTH_FORMAT); }
    public static string ToISMonth(this Nullable<DateTime> data)
    {
        if (data.HasValue) return ToISMonth(data.Value);
        return string.Empty;
    }

    // ID 格式
    public static string ToYearId(this DateTime data) { return data.ToString(SF_YEAR_FORMAT); }
    public static string ToYearId(this Nullable<DateTime> data, string defaultText = null)
    {
        if (data.HasValue) return ToYearId(data.Value);
        return defaultText ?? string.Empty;
    }

    public static string ToDateId(this DateTime data) { return data.ToString(SF_DATE_FORMAT); }
    public static string ToDateId(this Nullable<DateTime> data, string defaultText = null)
    {
        if (data.HasValue) return ToDateId(data.Value);
        return defaultText ?? string.Empty;
    }
    public static string ToMonthId(this DateTime data) { return data.ToString(SF_MONTH_FORMAT); }
    public static string ToMonthId(this Nullable<DateTime> data)
    {
        if (data.HasValue) return ToMonthId(data.Value);
        return string.Empty;
    }
    public static string ToDateTimeId(this DateTime data, bool showSecond = true)
    {
        if (showSecond) return data.ToString(SF_DATE_TIME_FORMAT);
        return data.ToString(SF_DATE_TIME_FORMAT_WITHOUT_SEC);
    }
    public static string ToDateTimeId(this Nullable<DateTime> data, bool showSecond = true)
    {
        if (data.HasValue) return ToDateTimeId(data.Value, showSecond);
        return string.Empty;
    }

    /// <summary>
    /// 获取本月第一天的日期
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static DateTime SpecificDayDate(this DateTime data,
        int ordinal = 1)
    { return new DateTime(data.Year, data.Month, ordinal); }

    /// <summary>
    /// 获得本月最后一天的日期
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static DateTime LastDayDate(this DateTime data)
    {
        return SpecificDayDate(data, DateTime.DaysInMonth(data.Year, data.Month));
    }

    /// <summary>
    /// 获取这个月包含的所有天的日期
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static DateTime[] ToDates(this DateTime data, bool wholeMonth = true)
    {
        var count = DateTime.DaysInMonth(data.Year, data.Month);
        var dates = new List<DateTime>();
        if (!wholeMonth) count = data.Day;
        for (int i = 0; i < count; i++) dates.Add(new DateTime(data.Year, data.Month, i + 1));
        return dates.ToArray();
    }

    /// <summary>
    /// 获取这个年份包含的月份
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string[] ToMonthIds(this DateTime data)
    {
        var ids = new List<string>();
        var start = new DateTime(data.Year, 1, 1);
        for (
            int i = 0; i < data.Month; i++)
            ids.Add(start.AddMonths(i).ToMonthId());
        return ids.ToArray();
    }

    #endregion

    #endregion

    #region Type

    public static Type LocateType(this Type type, string propertyName)
    {
        if (type.GetProperties().Any(p => p.Name == propertyName)) return type;
        if (type.IsInterface)
        {
            foreach (Type t in type.GetInterfaces())
                if (t.GetProperties().Any(p => p.Name == propertyName)) return t;
            return null;
        }
        return null;
    }

    public static MemberInfo LocateMember(this Type type, string memberName)
    {
        MemberInfo[] results = type.GetMember(memberName);
        if (results.Length > 0) return results[0];
        if (type.IsInterface)
        {
            foreach (Type t in type.GetInterfaces())
            {
                results = t.GetMember(memberName);
                if (results.Length > 0) return results[0];
            }
        }
        return null;
    }

    #endregion

    #region Object

    public static string ToXml(this object obj)
    {
        string xml = string.Empty;
        try
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(XmlWriter.Create(stream,
                new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = false }), obj);
            xml = Encoding.UTF8.GetString(stream.ToArray());
        }
        catch { }
        return xml;
    }

    public static void FlushTo(this object source, object target, string[] exclusion)
    {
        PropertyInfo[] s = source.GetType().GetProperties();
        FieldInfo[] df = target.GetType().GetFields();
        PropertyInfo[] dp = target.GetType().GetProperties();

        foreach (string f in exclusion)
        {
            if (!df.Any(ff => ff.Name == f) &&
                !dp.Any(p => p.Name == f)) throw new ArgumentException(f);
        }

        IEnumerable<PropertyInfo> commonP = s
            .Where(p => dp.Any(pp => pp.Name == p.Name && pp.PropertyType == p.PropertyType))
            .Where(p => !exclusion.Contains(p.Name));
        foreach (PropertyInfo pS in commonP)
        {
            PropertyInfo pT = dp.Single(p => p.Name == pS.Name);
            if (pS.CanRead && pT.CanWrite) pT.SetValue(target, pS.GetValue(source, null), null);
        }

        IEnumerable<PropertyInfo> commonF = s
            .Where(p => df.Any(f => f.Name == p.Name && f.FieldType == p.PropertyType))
            .Where(p => !exclusion.Contains(p.Name));
        foreach (PropertyInfo pS in commonF)
        {
            FieldInfo fT = df.Single(f => f.Name == pS.Name);
            if (pS.CanRead) fT.SetValue(target, pS.GetValue(source, null));
        }
    }

    public static void FlushTo<T>(this object source, T target, Action<T> afterUse = null)
    {
        FlushTo(source, target, new string[] { });
        if (afterUse != null) afterUse(target);
    }


    /// <summary>
    /// 根据类型匹配作为准入条件而进一步处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="handle"></param>
    public static void If<T>(this object obj, Action<T> handle, bool exceptionIfNull = false)
    {
        if (obj == null)
        {
            if (exceptionIfNull) throw new NullReferenceException();
            return;
        }
        if (obj is T) handle((T)obj);
        else if (exceptionIfNull) throw new InvalidCastException();
    }

    /// <summary>
    /// 扩展应对 object = null 的情况
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToStringEx(this object obj, string valueWhenNullOrEmpty = null)
    {
        if (obj == null) return valueWhenNullOrEmpty;
        if (obj.ToString().Trim().Length == 0) return valueWhenNullOrEmpty;
        return obj.ToString().Trim();
    }

    public static string ToStringEx(this object obj,
        Func<string, string> notNullOrEmptyHandle, string valueWhenNullOrEmpty = null)
    {
        if (obj == null) return valueWhenNullOrEmpty;
        if (obj.ToString().Trim().Length == 0) return valueWhenNullOrEmpty;
        return notNullOrEmptyHandle(obj.ToString().Trim());
    }

    /// <summary>
    /// 将类型当作某个进行处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T As<T>(this object obj, bool exceptionIfFail = false) where T : class
    {
        if (obj == null) return null;
        if (exceptionIfFail && !(obj is T)) throw new ArgumentException("type mismatched", "obj");
        return obj as T;
    }

    /// <summary>
    /// 如果对象不为 Null 则处理（If Not Null）Null，则可选处理
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="handle"></param>
    public static void IfNN<T>(this T obj,
        Action<T> handle, Action nullHandle = null)
    {
        if (obj != null) { handle(obj); return; }
        if (nullHandle != null) nullHandle();
    }

    #endregion

    #region Integer

    public static bool Equals<T>(
        this Nullable<int> data, T value, T nullValue)
        where T : struct, IComparable, IConvertible, IFormattable
    {
        if (data.HasValue) return Equals<T>(data.Value, value);
        return (nullValue.Equals(value));
    }

    public static bool Equals<T>(this int data, T value)
        where T : struct, IComparable, IConvertible, IFormattable
    { return data == Convert.ToInt32(value); }

    public static int[] ToArray(this int data, int start = 0)
    {
        var seed = new int[] { };
        var result = seed.AsEnumerable<int>();
        if (data >= start)
        {
            for (int i = start; i <= data; i++) result = result.Concat(new int[] { i }).ToArray();
        }
        else
        {
            for (int i = start; i >= data; i--) result = result.Concat(new int[] { i });
        }
        return result.ToArray();
    }

    public static List<int> ToList(this int data, int start = 0)
    {
        return ToArray(data, start).ToList();
    }

    public static string ToStringOrEmpty(this int data,
        bool comma = false, string emptyValue = null)
    {
        if (data == 0) return string.IsNullOrEmpty(emptyValue) ? string.Empty : emptyValue;
        if (comma)
        {
            return data.ToString("N0");
        }
        else
        {
            return data.ToString("F0");
        }
    }
    public static string ToStringOrEmpty(
        this Nullable<int> data, bool comma = false, string emptyValue = null)
    {
        if (data.HasValue) return ToStringOrEmpty(data.Value, comma, emptyValue);
        return string.Empty;
    }

    public static string ToStringOrEmpty(this long data,
        bool comma = false, string emptyValue = null)
    {
        if (data == 0) return string.IsNullOrEmpty(emptyValue) ? string.Empty : emptyValue;
        if (comma)
        {
            return data.ToString("N0");
        }
        else
        {
            return data.ToString("F0");
        }
    }
    public static string ToStringOrEmpty(
        this Nullable<long> data, bool comma = false, string emptyValue = null)
    {
        if (data.HasValue) return ToStringOrEmpty(data.Value, comma, emptyValue);
        return string.Empty;
    }

    public static int ToClassified(this Nullable<int> data, int nullDefault = 0)
    {
        if (data.HasValue) return data.Value;
        return nullDefault;
    }

    public static double ToFileSize(this long data, FileSize size)
    {
        int pow = (int)Math.Pow(1024, (double)(int)size);
        return (double)data / (double)pow;
    }

    public static double ToFileSize(this int data, FileSize size)
    {
        int pow = (int)Math.Pow(1024, (double)(int)size);
        return data / pow;
    }

    /// <summary>
    /// 将可空的整数作为枚举进行处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="handle"></param>
    /// <param name="defaultValue"></param>
    /// <param name="exceptionIfNotParsed"></param>
    public static void AsEnum<T>(this Nullable<int> data, Action<T> handle, T defaultValue = default(T), bool exceptionIfNotParsed = false)
        where T : struct, IComparable, IConvertible, IFormattable
    {
        if (!data.HasValue && exceptionIfNotParsed)
            throw new ArgumentNullException("data",
                string.Format("空值不能转换为类型为 '{0}' 的枚举值", typeof(T).Name));
        if (data.HasValue)
        {
            AsEnum<T>(data.Value, handle, defaultValue, exceptionIfNotParsed);
            return;
        }
        handle(defaultValue);
    }

    /// <summary>
    /// 将整数作为枚举进行处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="handle"></param>
    /// <param name="defaultValue"></param>
    /// <param name="exceptionIfNotParsed"></param>
    public static void AsEnum<T>(this int data, Action<T> handle, T defaultValue = default(T), bool exceptionIfNotParsed = false)
        where T : struct, IComparable, IConvertible, IFormattable
    {
        T result;
        bool trans = Enum.TryParse<T>(data.ToString(), out result);
        if (!trans && exceptionIfNotParsed)
            throw new ArgumentNullException("data",
                string.Format("'{0}' 不能转换为类型为 '{1}' 的枚举值", data.ToString(), typeof(T).Name));
        if (trans) { handle(result); return; }
        handle(defaultValue);
    }

    /// <summary>
    /// 转化为枚举值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="defaultValue"></param>
    /// <param name="exceptionIfNotParsed"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this int data, T defaultValue = default(T), bool exceptionIfNotParsed = false)
        where T : struct, IComparable, IConvertible, IFormattable
    {
        T result = defaultValue;
        AsEnum<T>(data, r => result = r, defaultValue, exceptionIfNotParsed);
        return result;
    }

    /// <summary>
    /// 转化为枚举值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="defaultValue"></param>
    /// <param name="exceptionIfNotParsed"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this Nullable<int> data, T defaultValue = default(T), bool exceptionIfNotParsed = false)
        where T : struct, IComparable, IConvertible, IFormattable
    {
        T result = defaultValue;
        AsEnum<T>(data, r => result = r, defaultValue, exceptionIfNotParsed);
        return result;
    }

    #endregion

    #region Double

    public static string ToStringOrEmpty(this double data,
        int decimalPlaces, bool comma = false, string emptyValue = null)
    {
        if (data == 0) return string.IsNullOrEmpty(emptyValue) ? string.Empty : emptyValue;
        if (comma)
        {
            return data.ToString("N" + decimalPlaces.ToString());
        }
        else
        {
            return data.ToString("F" + decimalPlaces.ToString());
        }
    }
    public static string ToStringOrEmpty(this Nullable<double> data,
        int decimalPlaces, bool comma = false, string emptyValue = null)
    {
        if (data.HasValue) return ToStringOrEmpty(data.Value, decimalPlaces, comma, emptyValue);
        return string.Empty;
    }

    public static string ToStringOrEmpty(this double data,
        bool hideDecimal = false, bool comma = false, string emptyValue = null)
    {
        if (data == 0) return string.IsNullOrEmpty(emptyValue) ? string.Empty : emptyValue;
        if (comma)
        {
            if (hideDecimal) return data.ToString("N0");
            return data.ToString("N");
        }
        else
        {
            if (hideDecimal) return data.ToString("F0");
            return data.ToString("F");
        }
    }
    public static string ToStringOrEmpty(this Nullable<double> data,
        bool hideDecimal = false, bool comma = false, string emptyValue = null)
    {
        if (data.HasValue) return ToStringOrEmpty(data.Value, hideDecimal, comma, emptyValue);
        return string.Empty;
    }
    public static double ToValueOrDefault(this Nullable<double> data,
        double defaultValue = 0)
    {
        if (data.HasValue) return data.Value;
        return defaultValue;
    }

    /// <summary>
    /// 非四舍五入，有余数进一
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static double ToRoundAbove(this double data)
    {
        if (data <= Math.Round(data, 2)) return Math.Round(data, 2);
        return Math.Round(data, 2) + 0.01;
    }

    /// <summary>
    /// 中国式四舍五入（默认保留两位）
    /// </summary>
    /// <param name="data"></param>
    /// <param name="digits"></param>
    /// <returns></returns>
    public static double ToCHNRounded(this double data,
        int digits = 2)
    { return Math.Round(data, digits, MidpointRounding.AwayFromZero); }
    public static double ToCHNRounded(this Nullable<double> data,
        int digits = 2)
    {
        if (!data.HasValue) return 0;
        return Math.Round(data.Value, digits, MidpointRounding.AwayFromZero);
    }

    #endregion

    #region Decimal

    public static string ToStringOrEmpty(this decimal data,
        int decimalPlaces, bool comma = false, string emptyValue = null)
    {
        if (data == 0) return string.IsNullOrEmpty(emptyValue) ? string.Empty : emptyValue;
        if (comma)
        {
            return data.ToString("N" + decimalPlaces.ToString());
        }
        else
        {
            return data.ToString("F" + decimalPlaces.ToString());
        }
    }
    public static string ToStringOrEmpty(Nullable<decimal> data,
        int decimalPlaces, bool comma = false, string emptyValue = null)
    {
        if (data.HasValue) return ToStringOrEmpty(data.Value, decimalPlaces, comma, emptyValue);
        return string.Empty;
    }

    public static string ToStringOrEmpty(this decimal data,
        bool hideDecimal = false, bool comma = false, string emptyValue = null)
    {
        if (data == 0) return string.IsNullOrEmpty(emptyValue) ? string.Empty : emptyValue;
        if (comma)
        {
            if (hideDecimal) return data.ToString("N0");
            return data.ToString("N");
        }
        else
        {
            if (hideDecimal) return data.ToString("F0");
            return data.ToString("F");
        }
    }
    public static string ToStringOrEmpty(this Nullable<decimal> data,
        bool hideDecimal = false, bool comma = false, string emptyValue = null)
    {
        if (data.HasValue) return ToStringOrEmpty(data.Value, hideDecimal, comma, emptyValue);
        return string.Empty;
    }

    /// <summary>
    /// 中国式四舍五入（默认保留两位）
    /// </summary>
    /// <param name="data"></param>
    /// <param name="digits"></param>
    /// <returns></returns>
    public static decimal ToCHNRounded(this decimal data,
        int digits = 2)
    { return Math.Round(data, digits, MidpointRounding.AwayFromZero); }
    public static decimal ToCHNRounded(this Nullable<decimal> data,
        int digits = 2)
    {
        if (!data.HasValue) return 0;
        return Math.Round(data.Value, digits, MidpointRounding.AwayFromZero);
    }

    #endregion

    #region List<T>

    /// <summary>
    /// 将一个数组扁平化
    /// </summary>
    public static string ToFlat<T>(this T[] data,
        string separator = ",", bool handleEmpty = false, string emptyHolder = "", bool canBeNull = true)
    {
        return ToFlat<T>(data.ToList(), separator, null, handleEmpty, emptyHolder, canBeNull);
    }

    /// <summary>
    /// 将一个列表数据扁平化
    /// </summary>
    public static string ToFlat<T>(this List<T> data,
        string separator = ",", Func<T, string> handlePart = null,
        bool handleEmpty = false, string emptyHolder = "", bool canBeNull = true)
    {
        if (data.Count == 0) return canBeNull ? null : string.Empty;
        string result = string.Empty;
        for (int i = 0; i < data.Count; i++)
        {
            string section = data[i].ToStringEx();
            if (handlePart != null) section = handlePart(data[i]);
            if (string.IsNullOrEmpty(section))
            {
                if (handleEmpty) result += (result.Length == 0) ? section : separator + emptyHolder;
            }
            else
            {
                result += (result.Length == 0) ? section : separator + section;
            }
        }
        if (string.IsNullOrEmpty(result)) return canBeNull ? null : string.Empty;
        return result;
    }

    #endregion

}
