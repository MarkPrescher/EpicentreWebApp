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
        // DONE!!!
        public readonly static string[] WEEKDAY_TIME_SLOTS = { "08:00", "08:15", "08:30", "08:45",
                                                               "09:00", "09:15", "09:30", "09:45",
                                                               "10:00", "10:15", "10:30", "10:45",
                                                               "11:00", "11:15", "11:30", "11:45",
                                                               "12:00", "12:15", "12:30", "12:45",
                                                               "13:00", "13:15", "13:30", "13:45",
                                                               "14:00", "14:15", "14:30", "14:45",
                                                               "15:00", "15:15", "15:30", "15:45",
                                                               "16:00"};
        // Please finish time slots
        //DONE!!!
        public readonly static string[] WEEKEND_TIME_SLOTS = { "08:00", "08:15", "08:30", "08:45",
                                                               "09:00", "09:15", "09:30", "09:45",
                                                               "10:00", "10:15", "10:30", "10:45",
                                                               "11:00", "11:15", "11:30", "11:45",
                                                               "12:00", "12:15", "12:30", "12:45",
                                                               "13:00" };

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