//���ߣ�������
//ʱ�䣺2010-12-01

using System;
using System.Globalization;

namespace ChineseCalendar
{

    #region �쳣����

    /// <summary>
    ///     �й������쳣����
    /// </summary>
    public class ChineseCalendarException : Exception
    {
        public ChineseCalendarException(string msg)
            : base(msg)
        {
        }
    }

    #endregion

    /// <summary>
    ///     �й�ũ���� ũ����Χ1901-1-1��2100-12-29 ������Χ1901-2-19��2101-1-28
    /// </summary>
    /// <remarks>
    ///     2010-2-22
    /// </remarks>
    public class ChineseCalendar
    {
        #region �ڲ��ṹ

        private struct LunarHolidayStruct
        {
            public int Day;
            public string HolidayName;
            public int Month;
            public int Recess;

            public LunarHolidayStruct(int month, int day, int recess, string name)
            {
                Month = month;
                Day = day;
                Recess = recess;
                HolidayName = name;
            }
        }

        private struct SolarHolidayStruct
        {
            public int Day;
            public string HolidayName;
            public int Month;
            public int Recess; //���ڳ���

            public SolarHolidayStruct(int month, int day, int recess, string name)
            {
                Month = month;
                Day = day;
                Recess = recess;
                HolidayName = name;
            }
        }

        private struct WeekHolidayStruct
        {
            public string HolidayName;
            public int Month;
            public int WeekAtMonth;
            public int WeekDay;

            public WeekHolidayStruct(int month, int weekAtMonth, int weekDay, string name)
            {
                Month = month;
                WeekAtMonth = weekAtMonth;
                WeekDay = weekDay;
                HolidayName = name;
            }
        }

        #endregion

        #region �ڲ�����

        private static ChineseLunisolarCalendar calendar = new ChineseLunisolarCalendar();

        private string _animal;
        private string _lunarDayString;
        private string _lunarMonthString;
        private string _lunarYearString;
        private string _sexagenary;
        private DateTime _solarDate;

        #endregion

        #region ��������

        #region ��������

        private const int GanZhiStartYear = 1864; //��֧������ʼ��
        private const string ChineseNumber = "��һ�����������߰˾�";
        private const int AnimalStartYear = 1900; //1900��Ϊ����
        private static DateTime GanZhiStartDay = new DateTime(1899, 12, 22); //��ʼ��
        private static DateTime ChineseConstellationReferDay = new DateTime(2007, 9, 13); //28���޲ο�ֵ,����Ϊ��
        public readonly int maxLunarYear = 2100;
        public readonly DateTime maxSolarDate = new DateTime(2101, 1, 28); //���������
        public readonly int minLunarYear = 1901;
        public readonly DateTime minSolarDate = new DateTime(1901, 2, 19); //��С��������

        #endregion

        #region �����͵���ʯ

        private static readonly string[] ConstellationName =
        {
            "������", "��ţ��", "˫����",
            "��з��", "ʨ����", "��Ů��",
            "�����", "��Ы��", "������",
            "Ħ����", "ˮƿ��", "˫����"
        };

        private static readonly string[] BirthStoneName =
        {
            "��ʯ", "����ʯ", "���", "����", "�챦ʯ", "���������",
            "����ʯ", "è��ʯ", "�Ʊ�ʯ", "��������", "��ˮ��", "�³�ʯ��Ѫʯ"
        };

        //��������    3��21��------4��20��   ����ʯ��   ��ʯ
        //��ţ����    4��21��------5��20��   ����ʯ��   ����ʯ
        //˫������    5��21��------6��21��   ����ʯ��   ���
        //��з����    6��22��------7��22��   ����ʯ��   ����
        //ʨ������    7��23��------8��22��   ����ʯ��   �챦ʯ
        //��Ů����    8��23��------9��22��   ����ʯ��   ���������
        //�������    9��23��-----10��23��   ����ʯ��   ����ʯ
        //��Ы����   10��24��-----11��21��   ����ʯ��   è��ʯ
        //��������   11��22��-----12��21��   ����ʯ��   �Ʊ�ʯ
        //Ħ������   12��22��------1��19��   ����ʯ��   ��������
        //ˮƿ����    1��20��------2��18��   ����ʯ��   ��ˮ��
        //˫������    2��19��------3��20��   ����ʯ��   �³�ʯ��Ѫʯ

        #endregion

        #region ��ʮ������

        private static readonly string[] ChineseConstellationName =
        {
            //��        ��      ��         ��        һ      ��      ��
            "��ľ��", "������", "Ů����", "������", "���º�", "β��", "��ˮ��",
            "��ľ�", "ţ��ţ", "ص����", "������", "Σ����", "�һ���", "��ˮ��",
            "��ľ��", "¦��", "θ����", "���ռ�", "������", "�����", "��ˮԳ",
            "��ľ��", "�����", "�����", "������", "����¹", "�����", "��ˮ�"
        };

        #endregion

        #region ��ʮ�Ľ���

        private static readonly string[] SolarTerms =
        {
            "С��", "��", "����", "��ˮ", "����", "����",
            "����", "����", "����", "С��", "â��", "����",
            "С��", "����", "����", "����", "��¶", "���",
            "��¶", "˪��", "����", "Сѩ", "��ѩ", "����"
        };

        private static readonly int[] sTermsInfo =
        {
            0, 21208, 42467, 63836, 85337, 107014,
            128867, 150921, 173149, 195551, 218072, 240693,
            263343, 285989, 308563, 331033, 353350, 375494,
            397447, 419210, 440795, 462224, 483532, 504758
        };

        #endregion

        #region ũ���������

        private const string CelestialStem = "���ұ����켺�����ɹ�";
        private const string TerrestrialBranch = "�ӳ���î������δ�����纥";
        private const string AnimalsStr = "��ţ������������Ｆ����";

        private static readonly string[] ChineseDayName =
        {
            "��һ", "����", "����", "����", "����", "����", "����", "����", "����", "��ʮ",
            "ʮһ", "ʮ��", "ʮ��", "ʮ��", "ʮ��", "ʮ��", "ʮ��", "ʮ��", "ʮ��", "��ʮ",
            "إһ", "إ��", "إ��", "إ��", "إ��", "إ��", "إ��", "إ��", "إ��", "��ʮ"
        };

        private static readonly string[] ChineseMonthName =
        {
            "��", "��", "��", "��", "��", "��", "��", "��", "��", "ʮ", "��", "��"
        };

        #endregion

        #region ����������Ľ���

        private static readonly SolarHolidayStruct[] solarHolidayInfo =
        {
            new SolarHolidayStruct(1, 1, 1, "Ԫ��"),
            new SolarHolidayStruct(2, 2, 0, "����ʪ����"),
            new SolarHolidayStruct(2, 10, 0, "���������"),
            new SolarHolidayStruct(2, 14, 0, "���˽�"),
            new SolarHolidayStruct(3, 1, 0, "���ʺ�����"),
            new SolarHolidayStruct(3, 5, 0, "ѧ�׷������"),
            new SolarHolidayStruct(3, 8, 0, "��Ů��"),
            new SolarHolidayStruct(3, 12, 0, "ֲ���� ����ɽ����������"),
            new SolarHolidayStruct(3, 14, 0, "���ʾ�����"),
            new SolarHolidayStruct(3, 15, 0, "������Ȩ����"),
            new SolarHolidayStruct(3, 17, 0, "�й���ҽ�� ���ʺ�����"),
            new SolarHolidayStruct(3, 21, 0, "����ɭ���� �����������ӹ����� ���������"),
            new SolarHolidayStruct(3, 22, 0, "����ˮ��"),
            new SolarHolidayStruct(3, 24, 0, "������ν�˲���"),
            new SolarHolidayStruct(4, 1, 0, "���˽�"),
            new SolarHolidayStruct(4, 7, 0, "����������"),
            new SolarHolidayStruct(4, 22, 0, "���������"),
            new SolarHolidayStruct(5, 1, 1, "�Ͷ���"),
            new SolarHolidayStruct(5, 2, 1, "�Ͷ��ڼ���"),
            new SolarHolidayStruct(5, 3, 1, "�Ͷ��ڼ���"),
            new SolarHolidayStruct(5, 4, 0, "�����"),
            new SolarHolidayStruct(5, 8, 0, "�����ʮ����"),
            new SolarHolidayStruct(5, 12, 0, "���ʻ�ʿ��"),
            new SolarHolidayStruct(5, 31, 0, "����������"),
            new SolarHolidayStruct(6, 1, 0, "���ʶ�ͯ��"),
            new SolarHolidayStruct(6, 5, 0, "���绷��������"),
            new SolarHolidayStruct(6, 26, 0, "���ʽ�����"),
            new SolarHolidayStruct(7, 1, 0, "������ ��ۻع���� ���罨����"),
            new SolarHolidayStruct(7, 11, 0, "�����˿���"),
            new SolarHolidayStruct(8, 1, 0, "������"),
            new SolarHolidayStruct(8, 8, 0, "�й����ӽ� ���׽�"),
            new SolarHolidayStruct(8, 15, 0, "����ս��ʤ������"),
            new SolarHolidayStruct(9, 9, 0, "ë����������"),
            new SolarHolidayStruct(9, 10, 0, "��ʦ��"),
            new SolarHolidayStruct(9, 18, 0, "�š�һ���±������"),
            new SolarHolidayStruct(9, 20, 0, "���ʰ�����"),
            new SolarHolidayStruct(9, 27, 0, "����������"),
            new SolarHolidayStruct(9, 28, 0, "���ӵ���"),
            new SolarHolidayStruct(10, 1, 1, "����� ����������"),
            new SolarHolidayStruct(10, 2, 1, "����ڼ���"),
            new SolarHolidayStruct(10, 3, 1, "����ڼ���"),
            new SolarHolidayStruct(10, 6, 0, "���˽�"),
            new SolarHolidayStruct(10, 24, 0, "���Ϲ���"),
            new SolarHolidayStruct(11, 10, 0, "���������"),
            new SolarHolidayStruct(11, 12, 0, "����ɽ��������"),
            new SolarHolidayStruct(12, 1, 0, "���簬�̲���"),
            new SolarHolidayStruct(12, 3, 0, "����м�����"),
            new SolarHolidayStruct(12, 20, 0, "���Żع����"),
            new SolarHolidayStruct(12, 24, 0, "ƽ��ҹ"),
            new SolarHolidayStruct(12, 25, 0, "ʥ����"),
            new SolarHolidayStruct(12, 26, 0, "ë�󶫵�������")
        };

        #endregion

        #region ��ũ������Ľ���

        private static readonly LunarHolidayStruct[] lunarHolidayInfo =
        {
            new LunarHolidayStruct(1, 1, 1, "����"),
            new LunarHolidayStruct(1, 15, 0, "Ԫ����"),
            new LunarHolidayStruct(5, 5, 0, "�����"),
            new LunarHolidayStruct(7, 7, 0, "��Ϧ���˽�"),
            new LunarHolidayStruct(7, 15, 0, "��Ԫ�� �������"),
            new LunarHolidayStruct(8, 15, 0, "�����"),
            new LunarHolidayStruct(9, 9, 0, "������"),
            new LunarHolidayStruct(12, 8, 0, "���˽�"),
            new LunarHolidayStruct(12, 23, 0, "����С��(ɨ��)"),
            new LunarHolidayStruct(12, 24, 0, "�Ϸ�С��(����)")
            //new LunarHolidayStruct(12, 30, 0, "��Ϧ")  //ע���Ϧ��Ҫ�����������м���
            //��Ϧ������إ��Ҳ��������ʮ
        };

        #endregion

        #region ��ĳ�µڼ������ڼ�

        private static readonly WeekHolidayStruct[] weekHolidayInfo =
        {
            new WeekHolidayStruct(5, 2, 0, "ĸ�׽�"),
            new WeekHolidayStruct(5, 3, 0, "ȫ��������"),
            new WeekHolidayStruct(6, 3, 0, "���׽�"),
            new WeekHolidayStruct(9, 3, 2, "���ʺ�ƽ��"),
            new WeekHolidayStruct(9, 4, 0, "�������˽�"),
            new WeekHolidayStruct(10, 1, 1, "����ס����"),
            new WeekHolidayStruct(10, 1, 3, "���ʼ�����Ȼ�ֺ���"),
            new WeekHolidayStruct(11, 4, 4, "�ж���")
        };

        #endregion

        #endregion

        #region ���캯��

        #region ChineseCalendar <�������ڳ�ʼ��>

        /// <summary>
        ///     ��һ����׼�Ĺ�����������ʹ��
        /// </summary>
        /// <param name="dt">��������</param>
        public ChineseCalendar(DateTime dt)
        {
            //��������Ƿ������Ʒ�Χ��
            if (dt < minSolarDate || dt > maxSolarDate)
            {
                throw new ChineseCalendarException("��������ֻ֧����1901-2-19��2101-1-28��Χ��");
            }
            _solarDate = dt;
            LoadFromSolarDate();
        }

        #endregion

        #region ChineseCalendar <ũ�����ڳ�ʼ��>

        /// <summary>
        ///     ��ũ������������ʹ��
        /// </summary>
        /// <param name="year">ũ����</param>
        /// <param name="month">ũ����</param>
        /// <param name="day">ũ����</param>
        /// <param name="IsLeapMonth">�Ƿ�����</param>
        public ChineseCalendar(int year, int month, int day, bool IsLeapMonth)
        {
            _solarDate = GetDateFromLunarDate(year, month, day, IsLeapMonth);
            LoadFromSolarDate();
        }

        #endregion

        #endregion

        #region ˽�к���

        #region LoadFromSolarDate

        /// <summary>
        ///     ����ChineseLunisolarCalendar���ʼ��ũ������
        /// </summary>
        private void LoadFromSolarDate()
        {
            IsLunarLeapMonth = false;

            LunarYear = calendar.GetYear(_solarDate);
            LunarMonth = calendar.GetMonth(_solarDate);
            LunarDay = calendar.GetDayOfMonth(_solarDate);

            _sexagenary = null;
            _animal = null;
            _lunarYearString = null;
            _lunarMonthString = null;
            _lunarDayString = null;

            int leapMonth = calendar.GetLeapMonth(LunarYear);
            if (leapMonth != 0)
            {
                if (leapMonth == LunarMonth)
                {
                    IsLunarLeapMonth = true;
                    LunarMonth -= 1;
                }
                else if (leapMonth > 0 && leapMonth < LunarMonth)
                {
                    LunarMonth -= 1;
                }
            }
        }

        #endregion

        #region GetDateFromLunarDate

        /// <summary>
        ///     ����ת����
        /// </summary>
        /// <param name="year">������</param>
        /// <param name="month">������</param>
        /// <param name="day">������</param>
        /// <param name="isLeapMonth">�Ƿ�����</param>
        private DateTime GetDateFromLunarDate(int year, int month, int day, bool isLeapMonth)
        {
            if (year < 1901 || year > 2100)
                throw new ChineseCalendarException("ֻ֧��1901��2100�ڼ��ũ����");
            if (month < 1 || month > 12)
                throw new ChineseCalendarException("��ʾ�·ݵ����ֱ�����1��12֮��");

            int leapMonth = calendar.GetLeapMonth(year);

            if (isLeapMonth && leapMonth != month + 1)
            {
                throw new ChineseCalendarException("���²�������");
            }

            if (isLeapMonth || (leapMonth > 0 && leapMonth <= month))
            {
                if (day < 1 || day > calendar.GetDaysInMonth(year, month + 1))
                    throw new ChineseCalendarException("ũ��������������");
                return calendar.ToDateTime(year, month + 1, day, 0, 0, 0, 0);
            }
            if (day < 1 || day > calendar.GetDaysInMonth(year, month))
                throw new ChineseCalendarException("ũ��������������");
            return calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }

        #endregion

        #region GetSexagenaryHour

        /// <summary>
        ///     ���ָ��ʱ���ʱ��
        /// </summary>
        /// <returns>ʱ��</returns>
        /// �ӣ�zi ������ 11 ʱ�����賿 1 ʱ���� �� ������ʱ�����Ծ�� ��һ˵Ϊ0��00~2��00���Դ����ƣ�
        /// ��chou ���賿 1 ʱ�����賿 3 ʱ���� ţ ţ����ʱ�����ݣ�׼�����
        /// ����yin ���賿 3 ʱ�������� 5 ʱ���� �� �ϻ��ڴ�ʱ���͡� 
        /// î��mao ������ 5 ʱ�������� 7 ʱ�� ���� �����ֳ����ã������ʱ�仹�����ϡ� 
        /// ���� chen ������ 7 ʱ�������� 9 ʱ���� �� �ഫ���ǡ�Ⱥ�����꡹��ʱ�� 
        /// �ȣ�si ������ 9 ʱ��������11ʱ���� �� ����ʱ�������ڲݴ��� 
        /// �磺wu ������11ʱ�������� 1 ʱ���� �� ��ʱ��̫�������ң��ഫ��ʱ�����ﵽ���ޣ�����������������������ද� 
        /// δ��wei �� ���� 1 ʱ�������� 3 ʱ�� �� �� �������ʱ��Բ� 
        /// �꣺shen �� ���� 3 ʱ�������� 5 ʱ���� �� ����ϲ������ʱ����� 
        /// �ϣ�you ������ 5 ʱ�������� 7 ʱ���� �� ��춰���ʼ�鳲 
        /// �磺xu ������ 7 ʱ�������� 9 ʱ�� �� �� ����ʼ���ſ� 
        /// ����hai ������ 9 ʱ�������� 11 ʱ���� �� ҹ��ʱ����������˯
        private string GetSexagenaryHour()
        {
            if (_solarDate.Hour == 23 || _solarDate.Hour == 0)
            {
                return TerrestrialBranch.Substring(0);
            }
            return TerrestrialBranch.Substring((_solarDate.Hour + 1)/2);
        }

        #endregion

        #region ConvertToChineseNum

        /// <summary>
        ///     ��0-9ת�ɺ�����ʽ
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private string ConvertToChineseNum(string n)
        {
            switch (n)
            {
                case "0":
                    return ChineseNumber.Substring(0, 1);
                case "1":
                    return ChineseNumber.Substring(1, 1);
                case "2":
                    return ChineseNumber.Substring(2, 1);
                case "3":
                    return ChineseNumber.Substring(3, 1);
                case "4":
                    return ChineseNumber.Substring(4, 1);
                case "5":
                    return ChineseNumber.Substring(5, 1);
                case "6":
                    return ChineseNumber.Substring(6, 1);
                case "7":
                    return ChineseNumber.Substring(7, 1);
                case "8":
                    return ChineseNumber.Substring(8, 1);
                case "9":
                    return ChineseNumber.Substring(9, 1);
                default:
                    return "";
            }
        }

        #endregion

        #region CompareWeekDayHoliday

        /// <summary>
        ///     �ж�ĳ�������ǲ���ָ���ĵڼ����µڼ��ܵڼ��죬�������ǵ�0�졣
        /// </summary>
        /// <param name="date">ָ��������</param>
        /// <param name="month">�ڼ�����</param>
        /// <param name="week">�ڼ�������</param>
        /// <param name="day">�ڼ���</param>
        /// <returns>true��false</returns>
        private bool CompareWeekDayHoliday(DateTime date, int month, int week, int day)
        {
            if (date.Month == month) //�·���ͬ
            {
                if ((int) date.DayOfWeek == day) //���ڼ���ͬ
                {
                    var firstDay = new DateTime(date.Year, date.Month, 1); //���ɵ��µ�һ��
                    int firWeekDays = 7 - (int) firstDay.DayOfWeek; //�����һ���ж�����

                    if (day + firWeekDays + (week - 2)*7 + 1 == date.Day)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region  ����

        #region ��С�������

        /// <summary>
        ///     ��֧�ֵ���Сũ�����
        /// </summary>
        public static int MinLunarYear
        {
            get { return 1901; }
        }

        /// <summary>
        ///     ��֧�ֵ����ũ�����
        /// </summary>
        public static int MaxLunarYear
        {
            get { return 2100; }
        }

        /// <summary>
        ///     ��֧�ֵ���С��������
        /// </summary>
        public static DateTime MinSolarDate
        {
            get { return new DateTime(1901, 2, 19); }
        }

        /// <summary>
        ///     ��֧�ֵ����������
        /// </summary>
        public static DateTime MaxSolarDate
        {
            get { return new DateTime(2101, 1, 28); }
        }

        #endregion

        #region ����

        #region LunarDateHoliday

        /// <summary>
        ///     �й�ũ������
        /// </summary>
        public string LunarDateHoliday
        {
            get
            {
                string tempStr = "";
                if (IsLunarLeapMonth == false) //���²��������
                {
                    foreach (LunarHolidayStruct lh in lunarHolidayInfo)
                    {
                        if ((lh.Month == LunarMonth) && (lh.Day == LunarDay))
                        {
                            tempStr = lh.HolidayName;
                            break;
                        }
                    }

                    //�Գ�Ϧ�����ر���
                    if (LunarMonth == 12)
                    {
                        int leapMonth = calendar.GetLeapMonth(LunarYear);
                        int days;
                        //���㵱��ũ��12�µ�������
                        if (leapMonth > 0 && LunarMonth > leapMonth - 1)
                        {
                            days = calendar.GetDaysInMonth(LunarYear, 13);
                        }
                        else
                        {
                            days = calendar.GetDaysInMonth(LunarYear, 12);
                        }

                        if (LunarDay == days) //���Ϊ���һ��
                        {
                            tempStr = "��Ϧ";
                        }
                    }
                }
                return tempStr;
            }
        }

        #endregion

        #region WeekDayHoliday

        /// <summary>
        ///     ��ĳ�µڼ��ܵڼ��ռ���Ľ���
        /// </summary>
        public string WeekDayHoliday
        {
            get
            {
                string tempStr = "";
                foreach (WeekHolidayStruct wh in weekHolidayInfo)
                {
                    if (CompareWeekDayHoliday(_solarDate, wh.Month, wh.WeekAtMonth, wh.WeekDay))
                    {
                        tempStr = wh.HolidayName;
                        break;
                    }
                }
                return tempStr;
            }
        }

        #endregion

        #region SolarDateHoliday

        /// <summary>
        ///     �������ռ���Ľ���
        /// </summary>
        public string SolarDateHoliday
        {
            get
            {
                string tempStr = "";

                foreach (SolarHolidayStruct sh in solarHolidayInfo)
                {
                    if ((sh.Month == _solarDate.Month) && (sh.Day == _solarDate.Day))
                    {
                        tempStr = sh.HolidayName;
                        break;
                    }
                }
                return tempStr;
            }
        }

        #endregion

        #endregion

        #region ��������

        #region SolarDate

        /// <summary>
        ///     ȡ��Ӧ�Ĺ�������
        /// </summary>
        public DateTime SolarDate
        {
            get { return _solarDate; }
            set
            {
                if (_solarDate.Equals(value))
                {
                }
                else
                {
                    //��������Ƿ������Ʒ�Χ��
                    if (value < minSolarDate || value > maxSolarDate)
                    {
                        throw new ChineseCalendarException("��������ֻ֧����1901-2-19��2101-1-28��Χ��");
                    }
                    _solarDate = value;
                    LoadFromSolarDate();
                }
            }
        }

        #endregion

        #region WeekDay

        /// <summary>
        ///     ���ڼ���Ӣ�ı�ʾ
        /// </summary>
        public DayOfWeek WeekDay
        {
            get { return _solarDate.DayOfWeek; }
        }

        #endregion

        #region WeekDayString

        /// <summary>
        ///     ���ڼ������ı�ʾ
        /// </summary>
        public string WeekDayString
        {
            get
            {
                switch (_solarDate.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return "������";
                    case DayOfWeek.Monday:
                        return "����һ";
                    case DayOfWeek.Tuesday:
                        return "���ڶ�";
                    case DayOfWeek.Wednesday:
                        return "������";
                    case DayOfWeek.Thursday:
                        return "������";
                    case DayOfWeek.Friday:
                        return "������";
                    default:
                        return "������";
                }
            }
        }

        #endregion

        #region DateString

        /// <summary>
        ///     �����������ı�ʾ�� ��һ�ž���������һ��
        /// </summary>
        public string DateString
        {
            get
            {
                string str = "";
                int i;
                string num = _solarDate.Year.ToString();
                for (i = 0; i < num.Length; i++)
                {
                    str += ConvertToChineseNum(num.Substring(i, 1));
                }
                str += "��";
                num = _solarDate.Month.ToString();
                for (i = 0; i < num.Length; i++)
                {
                    str += ConvertToChineseNum(num.Substring(i, 1));
                }
                str += "��";
                num = _solarDate.Day.ToString();
                for (i = 0; i < num.Length; i++)
                {
                    str += ConvertToChineseNum(num.Substring(i, 1));
                }
                str += "��";
                return str;
            }
        }

        #endregion

        #region ChineseConstellation

        /// <summary>
        ///     28���޼���
        /// </summary>
        public string ChineseConstellation
        {
            get
            {
                TimeSpan ts = _solarDate - ChineseConstellationReferDay;
                int offset = ts.Days;
                int modStarDay = offset%28;
                return (modStarDay >= 0
                    ? ChineseConstellationName[modStarDay]
                    : ChineseConstellationName[27 + modStarDay]);
            }
        }

        #endregion

        #endregion

        #region ũ������

        #region IsLunarLeapMonth

        /// <summary>
        ///     �Ƿ�����
        /// </summary>
        public bool IsLunarLeapMonth { get; private set; }

        #endregion

        #region LunarDay

        /// <summary>
        ///     ũ����
        /// </summary>
        public int LunarDay { get; private set; }

        #endregion

        #region LunarDayString

        /// <summary>
        ///     ũ���յ����ı�ʾ
        /// </summary>
        public string LunarDayString
        {
            get
            {
                if (string.IsNullOrEmpty(_lunarDayString))
                    _lunarDayString = ChineseDayName[LunarDay - 1];
                return _lunarDayString;
            }
        }

        #endregion

        #region LunarMonth

        /// <summary>
        ///     ũ�����·�
        /// </summary>
        public int LunarMonth { get; private set; }

        #endregion

        #region LunarMonthString

        /// <summary>
        ///     ũ���·����ı�ʾ
        /// </summary>
        public string LunarMonthString
        {
            get
            {
                if (string.IsNullOrEmpty(_lunarMonthString))
                    _lunarMonthString = ChineseMonthName[LunarMonth - 1];
                return _lunarMonthString;
            }
        }

        #endregion

        #region LunarYear

        /// <summary>
        ///     ũ�����
        /// </summary>
        public int LunarYear { get; private set; }

        #endregion

        #region LunarYearString

        /// <summary>
        ///     ũ��������ı�ʾ
        /// </summary>
        public string LunarYearString
        {
            get
            {
                if (string.IsNullOrEmpty(_lunarYearString))
                {
                    _lunarYearString = "";
                    string num = LunarYear.ToString();
                    for (int i = 0; i < 4; i++)
                    {
                        _lunarYearString += ChineseNumber.Substring(Convert.ToInt32(num.Substring(i, 1)), 1);
                    }
                }
                return _lunarYearString;
            }
        }

        #endregion

        #endregion

        #region 24����

        /// <summary>
        ///     �����������ʮ�Ľ���
        /// </summary>
        /// <remarks>
        ///     �����Ķ��������֡��Ŵ��������õĳ�Ϊ"����"������ʱ���һ��ȷ�Ϊ24�ݣ�
        ///     ÿһ����ƽ����15�����࣬�����ֳ�"ƽ��"���ִ�ũ�����õĳ�Ϊ"����"����
        ///     �������ڹ���ϵ�λ��Ϊ��׼��һ��360�㣬������֮�����15�㡣���ڶ���ʱ��
        ///     ��λ�ڽ��յ㸽�����˶��ٶȽϿ죬���̫���ڻƵ����ƶ�15���ʱ�䲻��15�졣
        ///     ����ǰ�����������෴��̫���ڻƵ����ƶ�������һ��������16��֮�ࡣ����
        ///     ����ʱ���Ա�֤���������ֱ�Ȼ����ҹƽ�ֵ������졣
        ///     ���������ǿ�ʼ����˼,�������Ǵ����Ŀ�ʼ��
        ///     ��ˮ�����꿪ʼ������������
        ///     ���ݣ����ǲص���˼��������ָ����է�����������ݷ������ж��ߵĶ��
        ///     ���֣�����ƽ�ֵ���˼�����ֱ�ʾ��ҹƽ�֡�
        ///     �������������ʣ���ľ��ï��
        ///     ���꣺�����ٹȡ������������ʱ��������������׳�ɳ���
        ///     ���ģ��ļ��Ŀ�ʼ��
        ///     С�����������������������ʼ������
        ///     â�֣��������â������졣
        ///     ���������ȵ��������١�
        ///     С��������ȵ���˼��С���������ʼ���ȡ�
        ///     ����һ�������ȵ�ʱ��
        ///     ����＾�Ŀ�ʼ��
        ///     ���������ֹ����ص���˼�������Ǳ�ʾ���ȵ����������
        ///     ��¶������ת����¶�����ס�
        ///     ��֣���ҹƽ�֡�
        ///     ��¶��¶ˮ�Ժ�����Ҫ�����
        ///     ˪�����������䣬��ʼ��˪��
        ///     �����������Ŀ�ʼ��
        ///     Сѩ����ʼ��ѩ��
        ///     ��ѩ����ѩ�����࣬������ܻ�ѩ��
        ///     ����������Ķ������١�
        ///     С��������ʼ���䡣
        ///     �󺮣�һ���������ʱ��
        /// </remarks>
        public string TwentyFourSolarTerms
        {
            get
            {
                var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
                string tempStr = "";

                int y = _solarDate.Year;

                for (int i = 0; i < 24; i++)
                {
                    double num = 525948.76*(y - 1900) + sTermsInfo[i];

                    DateTime newDate = baseDateAndTime.AddMinutes(num);
                    if (newDate.DayOfYear == _solarDate.DayOfYear)
                    {
                        tempStr = SolarTerms[i];
                        break;
                    }
                }
                return tempStr;
            }
        }

        /// <summary>
        ///     ��ǰ����ǰһ���������������
        /// </summary>
        public DateTime PrevSolarTermsDate
        {
            get { return GetPrevSolarTermsDate(_solarDate); }
        }

        /// <summary>
        ///     ��ǰ���ں�һ���������������
        /// </summary>
        public DateTime NextSolarTremsDate
        {
            get { return GetNextSolarTremsDate(_solarDate); }
        }

        #endregion

        #region �����͵���ʯ

        #region Constellation

        /// <summary>
        ///     ����
        /// </summary>
        public string Constellation
        {
            get { return GetConstellation(_solarDate); }
        }

        #endregion

        #region BirthStone

        /// <summary>
        ///     ����ʯ
        /// </summary>
        public string BirthStone
        {
            get { return GetBirthStone(_solarDate); }
        }

        #endregion

        #endregion

        #region ����

        /// <summary>
        ///     ����
        /// </summary>
        public string AnimalString
        {
            get
            {
                if (string.IsNullOrEmpty(_animal))
                {
                    int y = calendar.GetSexagenaryYear(_solarDate);
                    _animal = AnimalsStr.Substring((y - 1)%12, 1);
                }

                return _animal;
            }
        }

        #endregion

        #region ��ɵ�֧

        #region SexagenaryYear

        /// <summary>
        ///     ũ����ĸ�֧��ʾ�����ҳ��ꡣ
        /// </summary>
        public string SexagenaryYear
        {
            get
            {
                if (string.IsNullOrEmpty(_sexagenary))
                {
                    int y = calendar.GetSexagenaryYear(_solarDate);
                    _sexagenary = CelestialStem.Substring((y - 1)%10, 1) +
                                  TerrestrialBranch.Substring((y - 1)%12, 1);
                }
                return _sexagenary;
            }
        }

        #endregion

        #region SexagenaryMonth

        /// <summary>
        ///     ��֧���±�ʾ��ע��ũ�������²��Ǹ�֧
        /// </summary>
        public string SexagenaryMonth
        {
            get
            {
                //ÿ���µĵ�֧���ǹ̶���,�������Ǵ����¿�ʼ
                int zhiIndex;
                if (LunarMonth > 10)
                {
                    zhiIndex = LunarMonth - 10;
                }
                else
                {
                    zhiIndex = LunarMonth + 2;
                }
                string zhi = TerrestrialBranch.Substring(zhiIndex - 1, 1);

                //���ݵ���ĸ�֧��ĸ��������¸ɵĵ�һ��
                int ganIndex = 1;
                int i = (LunarYear - GanZhiStartYear)%60; //�����֧
                switch (i%10)
                {
                        #region ...

                    case 0: //��
                        ganIndex = 3;
                        break;
                    case 1: //��
                        ganIndex = 5;
                        break;
                    case 2: //��
                        ganIndex = 7;
                        break;
                    case 3: //��
                        ganIndex = 9;
                        break;
                    case 4: //��
                        ganIndex = 1;
                        break;
                    case 5: //��
                        ganIndex = 3;
                        break;
                    case 6: //��
                        ganIndex = 5;
                        break;
                    case 7: //��
                        ganIndex = 7;
                        break;
                    case 8: //��
                        ganIndex = 9;
                        break;
                    case 9: //��
                        ganIndex = 1;
                        break;

                        #endregion
                }
                string gan = CelestialStem.Substring((ganIndex + LunarMonth - 2)%10, 1);

                return gan + zhi;
            }
        }

        #endregion

        #region SexagenaryDay

        /// <summary>
        ///     ȡ��֧�ձ�ʾ��
        /// </summary>
        public string SexagenaryDay
        {
            get
            {
                TimeSpan ts = _solarDate - GanZhiStartDay;
                int offset = ts.Days;
                int i = offset%60;
                return CelestialStem.Substring(i%10, 1) + TerrestrialBranch.Substring(i%12, 1);
            }
        }

        #endregion

        #region SexagenaryHour

        /// <summary>
        ///     ʱ��
        /// </summary>
        public string SexagenaryHour
        {
            get
            {
                _solarDate = new DateTime(_solarDate.Year, _solarDate.Month, _solarDate.Day,
                    DateTime.Today.Hour, DateTime.Today.Minute, DateTime.Today.Second);
                return GetSexagenaryHour();
            }
        }

        #endregion

        #region SexagenaryDateString

        /// <summary>
        ///     ȡ��ǰ���ڵĸ�֧��ʾ���� �������ҳ��±�����
        /// </summary>
        public string SexagenaryDateString
        {
            get { return SexagenaryYear + "��" + SexagenaryMonth + "��" + SexagenaryDay + "��"; }
        }

        #endregion

        #endregion

        #endregion

        #region ��������

        #region GetPrevSolarTermsDate

        /// <summary>
        ///     ��ȡָ������ǰһ���������������
        /// </summary>
        public DateTime GetPrevSolarTermsDate(DateTime dt)
        {
            var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
            var newDate = new DateTime();

            int y = dt.Year;

            for (int i = 24; i >= 1; i--)
            {
                double num = 525948.76*(y - 1900) + sTermsInfo[i - 1];

                newDate = baseDateAndTime.AddMinutes(num); //�����Ӽ���

                if (newDate.DayOfYear < dt.DayOfYear)
                {
                    break;
                }
            }
            return newDate;
        }

        #endregion

        #region GetNextSolarTremsDate

        /// <summary>
        ///     ��ȡָ�����ں�һ���������������
        /// </summary>
        public DateTime GetNextSolarTremsDate(DateTime dt)
        {
            var baseDateAndTime = new DateTime(1900, 1, 6, 2, 5, 0); //#1/6/1900 2:05:00 AM#
            var newDate = new DateTime();

            int y = dt.Year;

            for (int i = 1; i <= 24; i++)
            {
                double num = 525948.76*(y - 1900) + sTermsInfo[i - 1];

                newDate = baseDateAndTime.AddMinutes(num); //�����Ӽ���

                if (newDate.DayOfYear > dt.DayOfYear)
                {
                    break;
                }
            }
            return newDate;
        }

        #endregion

        #region GetConstellation

        /// <summary>
        ///     ��ȡָ���������ڵ�����
        /// </summary>
        /// <param name="dt">ָ��������</param>
        /// <returns>�������ַ���</returns>
        public string GetConstellation(DateTime dt)
        {
            int index;
            int m = dt.Month;
            int d = dt.Day;
            int y = m*100 + d;

            if (((y >= 321) && (y <= 420)))
            {
                index = 0;
            }
            else if ((y >= 421) && (y <= 520))
            {
                index = 1;
            }
            else if ((y >= 521) && (y <= 620))
            {
                index = 2;
            }
            else if ((y >= 621) && (y <= 722))
            {
                index = 3;
            }
            else if ((y >= 723) && (y <= 822))
            {
                index = 4;
            }
            else if ((y >= 823) && (y <= 922))
            {
                index = 5;
            }
            else if ((y >= 923) && (y <= 1022))
            {
                index = 6;
            }
            else if ((y >= 1023) && (y <= 1121))
            {
                index = 7;
            }
            else if ((y >= 1122) && (y <= 1221))
            {
                index = 8;
            }
            else if ((y >= 1222) || (y <= 119))
            {
                index = 9;
            }
            else if ((y >= 120) && (y <= 218))
            {
                index = 10;
            }
            else if ((y >= 219) && (y <= 320))
            {
                index = 11;
            }
            else
            {
                index = 0;
            }

            return ConstellationName[index];
        }

        #endregion

        #region GetBirthStone

        /// <summary>
        ///     ��ȡָ���������ڵĵ���ʯ
        /// </summary>
        /// <param name="dt">ָ��������</param>
        /// <returns>����ʯ�ַ���</returns>
        public string GetBirthStone(DateTime dt)
        {
            int index;
            int m = dt.Month;
            int d = dt.Day;
            int y = m*100 + d;

            if (((y >= 321) && (y <= 420)))
            {
                index = 0;
            }
            else if ((y >= 421) && (y <= 520))
            {
                index = 1;
            }
            else if ((y >= 521) && (y <= 620))
            {
                index = 2;
            }
            else if ((y >= 621) && (y <= 722))
            {
                index = 3;
            }
            else if ((y >= 723) && (y <= 822))
            {
                index = 4;
            }
            else if ((y >= 823) && (y <= 922))
            {
                index = 5;
            }
            else if ((y >= 923) && (y <= 1022))
            {
                index = 6;
            }
            else if ((y >= 1023) && (y <= 1121))
            {
                index = 7;
            }
            else if ((y >= 1122) && (y <= 1221))
            {
                index = 8;
            }
            else if ((y >= 1222) || (y <= 119))
            {
                index = 9;
            }
            else if ((y >= 120) && (y <= 218))
            {
                index = 10;
            }
            else if ((y >= 219) && (y <= 320))
            {
                index = 11;
            }
            else
            {
                index = 0;
            }

            return BirthStoneName[index];
        }

        #endregion

        #region ToString

        /// <summary>
        ///     ũ�����ڵ��ַ�����ʾ���磺����������ʮ����إ��
        /// </summary>
        /// <returns>ũ�����ڵ��ַ�����ʾ</returns>
        public override string ToString()
        {
            if (IsLunarLeapMonth == false)
            {
                return LunarYearString + "��" + LunarMonthString +
                       "��" + LunarDayString;
            }
            return LunarYearString + "����" + LunarMonthString +
                   "��" + LunarDayString;
        }

        #endregion

        #region ToSexagenaryString

        /// <summary>
        ///     ũ��������ɵ�֧�����Ф��ʾ�����磺����(ţ)������إ��
        /// </summary>
        /// <returns>ũ��������ɵ�֧�����Ф��ʾ��</returns>
        public string ToSexAnimalString()
        {
            if (IsLunarLeapMonth == false)
            {
                return SexagenaryYear + "(" + AnimalString + ")��" +
                       LunarMonthString + "��" + LunarDayString;
            }
            return SexagenaryYear + "(" + AnimalString + ")����" +
                   LunarMonthString + "��" + LunarDayString;
        }

        #endregion

        #region GetLeapMonth

        /// <summary>
        ///     ��ȡ���µ��·ݣ���0��ʾ�����£�6��ʾ��5�¡�
        /// </summary>
        /// <param name="year">ũ�����</param>
        /// <returns>�����ǵڼ�����</returns>
        public static int GetLeapMonth(int year)
        {
            if (year < 1901 || year > 2100)
                throw new ChineseCalendarException("ֻ֧��1901��2100�ڼ��ũ����");
            return calendar.GetLeapMonth(year);
        }

        #endregion

        #region GetDaysInMonth

        /// <summary>
        ///     �����ũ������·ݵ�����
        /// </summary>
        /// <param name="year">ũ����</param>
        /// <param name="month">ũ���£����û�������ǣ�1��12�����������ǣ�1��13��</param>
        /// <returns>����</returns>
        public static int GetDaysInMonth(int year, int month)
        {
            if (year < 1901 || year > 2100)
                throw new ChineseCalendarException("ֻ֧��1901��2100�ڼ��ũ����");
            return calendar.GetDaysInMonth(year, month);
        }

        #endregion

        #region AddDays

        /// <summary>
        ///     ��ָ���������ӵ���ũ��������
        /// </summary>
        /// <param name="n">����</param>
        /// <returns>ũ������</returns>
        public ChineseCalendar AddDays(int n)
        {
            return new ChineseCalendar(SolarDate.AddDays(n));
        }

        #endregion

        #region AddMonths

        /// <summary>
        ///     ��ָ�����·����ӵ���ũ��������
        /// </summary>
        /// <param name="n">�·���</param>
        /// <returns>ũ������</returns>
        public ChineseCalendar AddMonths(int n)
        {
            var temp = new ChineseCalendar(_solarDate);
            for (int i = 0; i < n; i++)
            {
                temp.SolarDate = temp._solarDate.AddDays(29);
                if (temp.LunarMonth == LunarMonth &&
                    (IsLunarLeapMonth || (IsLunarLeapMonth == false &&
                                      temp.IsLunarLeapMonth == false)))
                {
                    temp.LunarMonth++;
                    temp._lunarMonthString = null;
                }

                int days = GetDaysInMonth(temp.LunarYear, temp.LunarMonth);
                temp.LunarDay = days < LunarDay ? days : LunarDay;

                temp._lunarDayString = null;
            }

            return temp;
        }

        #endregion

        #region AddYears

        /// <summary>
        ///     ��ָ����������ӵ���ũ��������
        /// </summary>
        /// <param name="n">���</param>
        /// <returns>ũ������</returns>
        public ChineseCalendar AddYears(int n)
        {
            ChineseCalendar temp = this;
            temp.LunarYear += n;
            temp.IsLunarLeapMonth = false;
            temp._lunarYearString = null;
            temp._sexagenary = null;
            temp._animal = null;
            int days = GetDaysInMonth(temp.LunarYear, temp.LunarMonth);
            if (days < LunarDay)
            {
                temp.LunarDay = days;
                temp._lunarDayString = null;
            }

            return temp;
        }

        #endregion

        #endregion
    }
}