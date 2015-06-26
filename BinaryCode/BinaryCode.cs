namespace BinaryCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class BinaryCode
    {
        #region Methods

        public static string[] decode(string message)
        {
            int[] output1 = new int[message.Length];
            int[] output2 = new int[message.Length];

            output1[0] = 0;
            output2[0] = 1;

            bool isO1Gud = true, isO2Gud = true;

            if (message.Length > 1)
            {
                for (int i = 0; i < message.Length; i++)
                {
                    if (i == 0)
                    {
                        output1[i + 1] = Int32.Parse(message[i] + "") - output1[i];
                        if (isO1Gud && (output1[i + 1] < 0 || output1[i + 1] > 1)) isO1Gud = false;
                        output2[i + 1] = Int32.Parse(message[i] + "") - output2[i];
                        if (isO2Gud && (output2[i + 1] < 0 || output2[i + 1] > 1)) isO2Gud = false;
                    }
                    else if (i < message.Length - 1)
                    {
                        output1[i + 1] = Int32.Parse(message[i] + "") - output1[i] - output1[i - 1];
                        if (isO1Gud && (output1[i + 1] < 0 || output1[i + 1] > 1)) isO1Gud = false;
                        output2[i + 1] = Int32.Parse(message[i] + "") - output2[i] - output2[i - 1];
                        if (isO2Gud && (output2[i + 1] < 0 || output2[i + 1] > 1)) isO2Gud = false;
                    }

                }

                if (isO1Gud && Int32.Parse(message[message.Length - 1] + "") != (output1[message.Length - 2] + output1[message.Length - 1])) isO1Gud = false;

                if (isO2Gud && Int32.Parse(message[message.Length - 1] + "") != (output2[message.Length - 2] + output2[message.Length - 1])) isO2Gud = false;
            }
            else if (message.Length == 1)
            {
                if (message[0] == '0') { isO2Gud = false; }
                if (message[0] == '1') { isO1Gud = false; }
                else { isO1Gud = false; isO2Gud = false; }
            }
            else { isO1Gud = false; isO2Gud = false; }

            return new string[] { isO1Gud ? string.Join("", output1) : "NONE", isO2Gud ? string.Join("", output2) : "NONE" };
        }

        static void Main(string[] args)
        {
            Console.WriteLine(decode("123210120")[0] + "," + decode("123210120")[1]);
        }

        #endregion Methods
    }
}