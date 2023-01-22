using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Right2Left
{



    /// <summary>
    ///  created by https://github.com/yoddaa for changing text direction of writing.
    ///  this script is for run time.
    /// </summary>


    public class ChangeDuringRuntime : MonoBehaviour
    {

        [SerializeField]
        InputField inputField;

        [SerializeField]
        Text legacyText;

        [SerializeField]
        TMP_InputField tMP_InputField;

        [SerializeField]
        TMP_Text tMP_Text;

        [SerializeField]
        private Button button;



        private void Start()
        {
            button = GetComponent<Button>();
        }

        public void OnclickButton()
        {

            legacyText.text = SwitchRTL(inputField.text);

            tMP_Text.text = SwitchRTL(tMP_InputField.text);
        }

        public void Clear()
        {
            legacyText.text = "";
            tMP_Text.text = "";
            inputField.text = null;
            tMP_InputField.text = null;
        }
        public void InputFieldsRTL()
        {

            inputField.text = SwitchRTL(inputField.text);
            tMP_InputField.text = SwitchRTL(tMP_InputField.text);
        }

        public void ToUpperCaseBtn()
        {
            inputField.text = inputField.text.ToUpper();
            tMP_InputField.text = tMP_InputField.text.ToUpper();
        }
        public void ToLowerCaseBtn()
        {
            inputField.text = inputField.text.ToLower();
            tMP_InputField.text = tMP_InputField.text.ToLower();
        }

        public void ToCamelCaseBtn()
        {
            inputField.text = ToCamelCase(inputField.text);
            tMP_InputField.text = ToCamelCase(tMP_InputField.text);
        }

        public void ToCapitalizeBtn()
        {
            inputField.text = CapitalizedTheFirstLetter(inputField.text);
            tMP_InputField.text = CapitalizedTheFirstLetter(tMP_InputField.text);
        }

        public void ReveseBtn()
        {
            inputField.text = Reverse(inputField.text);
            tMP_InputField.text = Reversed(tMP_InputField.text);
        }

        //use this function for changing text right to left.
        [SerializeField]
        public String SwitchRTL(String input)
        {
            //return the input back if it's null or empty
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = ReverseEachWord(input);


            input = Reverse(input);


            return ReverseEachWord(input);
        }

        //////////////////from here algorithms for manipulating text//////////////////////////////////////////////////



        static Boolean isUpperCase(string s)
        {
            return s.All(char.IsUpper);
        }
        static Boolean isLowerCase(string s)
        {
            return s.All(char.IsLower);
        }
        static Boolean isLetter(string s)
        {
            return s.All(char.IsLetter);
        }
        static Boolean isWhiteSpace(string s)
        {
            return s.All(char.IsWhiteSpace);
        }

        //reversing a string Manualy
        static String Reversed(String input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            //using System.Text with The StringBuilder
            StringBuilder reversed = new StringBuilder(input.Length);

            //going backwards on the input and planting in the reversed StringBuilder
            for (int i = input.Length - 1; i >= 0; i--)
            {
                //adding it to the reversd string
                reversed.Append(input[i]);
            }
            //send back the result
            return reversed.ToString();
        }

        //reversing a string with an Array
        static String Reversed2(String input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            //converting string into charcter array
            char[] arr = input.ToCharArray();
            //reversing it
            Array.Reverse(arr);//modifying the array not creating a new one
                               //returning a new string from the reversed array
            return new string(arr);
        }

        //Reversing a string array
        static String Reverse(String input)
        {
            //validating
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            //converting the input to a character array
            char[] arr = input.ToCharArray();
            //reversing the array
            Array.Reverse(arr);

            //return a new String reversed from the array
            return new String(arr);
        }

        //reversing each word in an array
        static String ReverseEachWord(String input)
        {
            //return the input back if it's null or empty
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            StringBuilder result = new StringBuilder();
            //cutting the string by space to an array of words
            String[] arr = input.Split(" ");
            //getting each word in the array
            for (int i = 0; i < arr.Length; i++)
            {
                //reversing the entery at [i] in the array and appenig it to the result
                result.Append(Reverse(arr[i]));
                //the spaces were removerd so we need to add them back in
                //and if we are not on the last word we can add space
                if (i != arr.Length - 1)
                {
                    result.Append(" ");
                }

            }
            //when the for loop is done we can send it back
            return result.ToString();

        }


        //if first word is Cap
        static String CapitalizedTheFirstLetter(String input)
        {
            //validating
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }


            //removeWhiteSpace
            input.Trim();

            //converting the input to a character array
            char[] arr = input.ToCharArray();

           
            //getting the first originalLetter To upperCase
            string firstletter = arr[0].ToString().ToUpper();


            //conecting the string starting From the second Letter
            for (int i = 1; i < arr.Length; i++)
            {
                firstletter += arr[i].ToString();
            }
            
            //sending back first letter capitalized word
            return firstletter;

        }


        //reversing each word in an array
        static String BreakToEachWord(String input)
        {
            //return the input back if it's null or empty
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            String[] arr;
            StringBuilder result = new StringBuilder();

            //if you unmark above remove this line as it is in the else statment already
            arr = input.Split(" ");

            //getting each word in the array
            for (int i = 0; i < arr.Length; i++)
            {
                if (i == 0)
                {
                    result.Append(arr[i].ToString());
                }
                else
                {
                    // the entery at [i] in the array and appenig it to the result
                    result.Append(CapitalizedTheFirstLetter(arr[i].ToString()));
                }

                //the spaces were removerd so we need to add them back in
                //and if we are not on the last word we can add space
                if (i != arr.Length - 1)
                {
                    result.Append(" ");
                }

            }
            //when the for loop is done we can send it back
            return result.ToString();
        }

        public string RemoveCharacters(string str1, string str2)
        {
            //check if str1 or str2 are not null or empty
            if (str1 == null || str2 == null)
                return "One or both of the input strings is null";

            if (str1.Length == 0 && str2.Length == 0)
                return "Both input strings are empty";

            // Create an empty result string
            string result = "";

            // Loop through each character in str1
            foreach (char c in str1)
            {
                // If the character is not present in str2, add it to the result string
                if (!str2.Contains(c))
                {
                    result += c;
                }
            }

            // Return the result string
            return result;
        }

        //reversing each word in an array
        static String BreakToEachWord2(String input)
        {
            //return the input back if it's null or empty
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            String[] arr;

            StringBuilder result = new StringBuilder();


            //cutting the string by space or - or _ to an array of words
            if (input.Contains("-"))
            {
                arr = input.Split("-");
            }
            else if (input.Contains("_"))
            {
                arr = input.Split("_");
            }
            else
            {
                arr = input.Split(" ");
            }


            //getting each word in the array
            for (int i = 0; i < arr.Length; i++)
            {
                if (i == 0)
                {
                    result.Append(arr[i].ToString());
                }
                else
                {
                    // the entery at [i] in the array and appenig it to the result
                    result.Append(CapitalizedTheFirstLetter(arr[i].ToString()));
                }




            }
            //when the for loop is done we can send it back
            return result.ToString();
        }

        static String ToCamelCase(String input)
        {
            //return the input back if it's null or empty
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }


            CapitalizedTheFirstLetter(input);
            



            return BreakToEachWord2(input);
        }
    }

}
