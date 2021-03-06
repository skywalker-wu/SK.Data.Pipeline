﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.Data.Pipeline.Core.Common
{
    public static class StringUtil
    {
        public static void Info(this string str)
        {
            Logger.Instance.Info(str);
        }

        public static void Warning(this string str)
        {
            Logger.Instance.Warn(str);
        }

        public static void Error(this string str)
        {
            Logger.Instance.Error(str);
        }

        public static object AutoConvertToObject(this string valueStr)
        {
            if (valueStr == null) return null;

            int intObj = 0;
            if (int.TryParse(valueStr, out intObj))
            {
                return intObj;
            }

            double doubleObj = 0;
            if (double.TryParse(valueStr, out doubleObj))
            {
                return doubleObj;
            }

            return valueStr;
        }

        public static void Shuffle(string[] arrayString)
        {
		    var random = new Random(DateTime.Now.Millisecond);
            for (int i = arrayString.Length; i > 1; i--)
		    {
			    int j = random.Next(i); 
                string tmp = arrayString[j];
                arrayString[j] = arrayString[i - 1];
                arrayString[i - 1] = tmp;
		    }
        }
    }
}
