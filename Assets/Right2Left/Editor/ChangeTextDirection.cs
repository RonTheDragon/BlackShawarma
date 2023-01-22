using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;


namespace Right2Left
{

    /// <summary>
    ///  created by https://github.com/yoddaa for changing text direction of writing.
    ///  add this script to your project to switch the direction of sentence inside the editor
    ///  under window choose: Change Text Direction RTL
    ///  dock the window next to the inspector
    ///  write your sentence then press the button to flip it. wait or press other window just for refresh.
    ///  copy the fliped text to wherever you need.
    ///  if it's not flipped after you wrote. change the size of the window to refresh.
    ///  if you want to change RTL via script during runtime then use the ChangeDuringRuntime script.
    ///  use the function from line 97 ->  SwitchRTL(string to flip)  it will return flipped string right2left.
    /// </summary>

#if UNITY_EDITOR
    public class ChangeTextDirection : EditorWindow
    {
        string myString = "";
        string resultString = "";
        string unwantedCharacters = "";


        public static bool isUsingFunctionSpliters = false;

        //the characters to remove when creating a camel case.
        //if you need underscore the replace it with other sign
        public static string functionSpliterString1 = "_";
        public static string functionSpliterString2 = "-";
        public static string toggleCondition = "removing - _ form camel case";

        
        //for display the window
        [MenuItem("Window/Change Text Direction RTL")]
        public static void ShowWindow()
        {
            GetWindow<ChangeTextDirection>("Change Text Direction RTL");
        }

        //window editor code
        private void OnGUI()
        {


            GUILayout.Label("Select your text and press.", EditorStyles.boldLabel);

            myString = EditorGUILayout.TextField("write here : ", myString);


            if (GUILayout.Button("Switch Direction Ltr <-> Rtl"))
            {

                resultString = (ReverseEachWord(myString));
                resultString = (Reverse(myString));

                EditorGUILayout.TextArea(resultString);
                myString = EditorGUILayout.TextField("write here", ReverseEachWord(myString));
                myString = EditorGUILayout.TextField("write here", resultString);

                
            }

            if (GUILayout.Button("All Upper Case"))
            {

                resultString = myString.ToUpper();

                EditorGUILayout.TextArea(resultString);
              
                myString = EditorGUILayout.TextField("write here", resultString);

                //myString.ToUpper();

            }
            if (GUILayout.Button("All Lower Case"))
            {

                resultString = myString.ToLower();

                EditorGUILayout.TextArea(resultString);
               
                myString = EditorGUILayout.TextField("write here", resultString);

                //myString.ToUpper();

            }

            if (GUILayout.Button("Reverse Each Word"))
            {

                resultString = CapitalizedTheFirstLetter(myString);

                EditorGUILayout.TextArea(resultString);

                myString = EditorGUILayout.TextField("write here", resultString);

                //myString.ToUpper();

            }

            if (GUILayout.Button("Break To Each Word"))
            {

                resultString = BreakToEachWord(myString);

                EditorGUILayout.TextArea(resultString);
                //myString = EditorGUILayout.TextField("write here", ReverseEachWord(myString));
                myString = EditorGUILayout.TextField("write here", resultString);

                //myString.ToUpper();

            }
            if (GUILayout.Button("ToCamelCase"))
            {

                resultString = ToCamelCase(myString);
                EditorGUILayout.TextArea(resultString);
                myString = EditorGUILayout.TextField("write here", BreakToEachWord2(resultString));
                // myString = EditorGUILayout.TextField("write here", resultString);
            }
            if (GUILayout.Button("Capitalized The First Letter"))
            {

                resultString = CapitalizedTheFirstLetter(myString);

                EditorGUILayout.TextArea(resultString);
                //myString = EditorGUILayout.TextField("write here", ReverseEachWord(myString));
                myString = EditorGUILayout.TextField("write here", resultString);

                //myString.ToUpper();

            }

            
            if (GUILayout.Button(toggleCondition))
            {
                if (isUsingFunctionSpliters)
                {
                    toggleCondition = "use - _  with camel case";
                }
                else
                {
                    toggleCondition = "removing - _ form camel case";
                }
               
                EditorGUILayout.TextArea(resultString);
                isUsingFunctionSpliters = EditorGUILayout.Toggle("isUsingFunctionSpliters", (isUsingFunctionSpliters == false));
              
            }


            if (GUILayout.Button("Clear"))
            {
                myString = null;
                resultString = null;
                unwantedCharacters = null;
                EditorGUILayout.TextArea(resultString);
                myString = EditorGUILayout.TextField("write here", resultString);
            }

            //-------------------------------------------------------------------

            //GUILayout.Label("Remove Characters", EditorStyles.boldLabel);
            unwantedCharacters = EditorGUILayout.TextField("Remove Characters", unwantedCharacters);

            if (GUILayout.Button("Remove Characters"))
            {
                resultString = RemoveCharacters(myString, unwantedCharacters);
                EditorGUILayout.TextArea(unwantedCharacters);
                myString = EditorGUILayout.TextField("simbolsToRemove", resultString);
                //resultString = RemoveSimbols(myString, simbolsToRemove);
            }

            //-------------------------------------------------------------------
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

        //---------------------------------------------------------------

        //use this function for changing text right to left.
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

            //if you need to cut the not just by space unmark and change

            //cutting the string by space or - or _ to an array of words
            if (input.Contains(functionSpliterString1) && (isUsingFunctionSpliters==true))
            {
                arr = input.Split(functionSpliterString1);
            }
            else if (input.Contains(functionSpliterString2))
            {
                arr = input.Split(functionSpliterString2);
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
#endif