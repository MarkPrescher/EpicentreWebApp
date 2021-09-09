using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epicentre.Library
{
    public static class TimeSlots
    {
        public static int timeSlotCounter = 0;
        // Please finish time slots
        public readonly static string[] WEEKDAY_TIME_SLOTS = { "08:00", "08:15", "08:30", "08:45", "09:00" };
        // Please finish time slots
        public readonly static string[] WEEKEND_TIME_SLOTS = { "08:00", "08:15", "08:30", "08:45", "09:00" };

        public static bool CheckIfWeekDay(DateTime date)
        {
            DayOfWeek day = date.DayOfWeek;

            if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
            {
                return false;
            }
            return true;
        }
    }
}