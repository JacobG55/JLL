using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace JLL.Components.Filters
{
    public class DateFilter : JFilter<DateTime>
    {
        [FormerlySerializedAs("filterOnStart")]
        public bool checkDateOnEnable = true;

        [Header("Date")]
        [Tooltip("When checked: All DateFilters must be true in order for the filter to succeed.\nWhen unchecked: Only one filter passing is required to succeed.")]
        public bool mustPassAllFilters = false;
        public DateEntryFilter[] DateFilters = new DateEntryFilter[1] { new DateEntryFilter() };
        public bool checkDateWithinRange = false;
        public DateRange dateRange = new DateRange();

        [Header("Time")]
        [Tooltip("Number between 0 - 23")]
        public IntFilter hour = new IntFilter() { value = 12 };
        [Tooltip("Number between 0 - 59")]
        public IntFilter minute = new IntFilter() { value = 30 };

        [Serializable]
        public class DateEntryFilter
        {
            public WeekDayFilter dayOfWeek = new WeekDayFilter() { value = DayOfWeek.Monday };
            public IntFilter day = new IntFilter() { value = 1 };
            public MonthFilter month = new MonthFilter() { value = Month.January };
            public IntFilter year = new IntFilter() { value = 2000 };
        }

        [Serializable]
        public class DateEntry
        {
            public int day = 1;
            public Month month = Month.January;

            public DateTime ToDateTime()
            {
                return new DateTime(DateTime.Now.Year, (int)month, day);
            }
        }

        [Serializable]
        public class DateRange
        {
            public DateEntry startDate = new DateEntry();
            public DateEntry endDate = new DateEntry() { month = Month.February };

            public bool IsBetweenDates(DateTime time)
            {
                return time.Ticks > startDate.ToDateTime().Ticks && time.Ticks < endDate.ToDateTime().Ticks;
            }
        }

        [Serializable]
        public class MonthFilter : NumFilter<Month>
        {
            public override bool CheckValue(Month val)
            {
                return CheckNum((int)val, (int)value);
            }
        }

        [Serializable]
        public class WeekDayFilter : NumFilter<DayOfWeek>
        {
            public override bool CheckValue(DayOfWeek val)
            {
                return CheckNum((int)val, (int)value);
            }
        }

        public enum Month
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12,
        }

        public static Month GetMonth(int num)
        {
            if (Enum.IsDefined(typeof(Month), num))
            {
                return (Month)num;
            }
            return Month.January;
        }

        public void OnEnable()
        {
            if (checkDateOnEnable)
            {
                FilterCurrentDate();
            }
        }

        public override void Filter(DateTime date)
        {
            if (DateFilters.Length > 0)
            {
                int filtered = 0;
                for (int i = 0; i < DateFilters.Length; i++)
                {
                    if (DateFilters[i].dayOfWeek.shouldCheck && !DateFilters[i].dayOfWeek.CheckValue(date.DayOfWeek))
                    {
                        continue;
                    }

                    if (DateFilters[i].day.shouldCheck && !DateFilters[i].day.CheckValue(date.Day))
                    {
                        continue;
                    }

                    if (DateFilters[i].month.shouldCheck && !DateFilters[i].month.CheckValue(GetMonth(date.Month)))
                    {
                        continue;
                    }

                    if (DateFilters[i].year.shouldCheck && !DateFilters[i].year.CheckValue(date.Year))
                    {
                        continue;
                    }

                    filtered++;
                }
                if (mustPassAllFilters ? filtered != DateFilters.Length : filtered == 0)
                {
                    goto Failed;
                }
            }

            if (checkDateWithinRange && !dateRange.IsBetweenDates(date))
            {
                goto Failed;
            }

            if (hour.shouldCheck && !hour.CheckValue(date.Hour))
            {
                goto Failed;
            }

            if (minute.shouldCheck && !minute.CheckValue(date.Minute))
            {
                goto Failed;
            }

            Result(date, true);
            return;

            Failed:
            Result(date);
        }

        public void FilterCurrentDate()
        {
            Filter(DateTime.Now);
        }

        public override void FilterDefault()
        {
            FilterCurrentDate();
        }
    }
}
