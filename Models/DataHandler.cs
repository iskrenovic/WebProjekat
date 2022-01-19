using System;

namespace Models
{
    public static class DataHandler
    {
        public static bool StringOutOfRange(string str, int min, int max){
            return str.Length<min || str.Length>max;
        }

        public static bool ValueOutOfRange(double value, double min, double max){
            return value < min || value > max;
        }        

        public static bool ValueOutOfRange(int value, int min, int max){
            return value < min || value > max;
        }

        public static DateTime StringToDate(string date){
            try
            {
                string[] str = date.Split('-');
                int year = int.Parse(str[0]);
                int month = int.Parse(str[1]);
                int day = int.Parse(str[2]);
                return new DateTime(year, month, day);
            }
            catch{
                throw new Exception("Date format is wrong.");
            }
        }

        public static bool BetweenDates(DateTime a1, DateTime b1, DateTime a2, DateTime b2){
            return (a1 <= a2 && a2 < b1) || (a1<b2 && b2<=b1);
        }

        public static Object StringToJson(string mes){
            return new {message = mes};
        }       
    }
}