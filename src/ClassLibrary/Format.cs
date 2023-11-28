using System;
using System.Collections.Generic;
using System.Linq;
namespace ClassLibrary

// Clase responsabilidad de Agustín Toya.

{
    /// <summary>
    /// Clase usada para reutilizar el código de formatear entradas de texto. Quitando tildes, poniendo el texto en mayúsculas o minúsculas, 
    /// al par que quita los espacios en blanco del principio y el final de la palabra. 
    /// </summary>
    public class Format
    {
        /// <summary>
        /// Método que se encarga de formatear el texto para que quede todo en mayus y sacandole los tildes 
        /// que pueda llegar a tener
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string RemoveAcentMarkToUpper(string word)
        {
            string formatWord = $"";
            if (!string.IsNullOrEmpty(word))
            {

                word = word.ToUpper().Trim();
                //Primero le quita los tildes que vaya a tener
                // Diccionario para cambiar las letras con tilde, por otra sin tilde, así no hay dos
                // palabras sea iguales, pero que tengan tildes en lugares diferentes por un error de escritura
                Dictionary<char, char> tildes = new Dictionary<char, char>()
                {
                    {'Á', 'A'},
                    {'Ó','O'},
                    {'É','E'},
                    {'Í','I'},
                    {'Ú','U'}
                };
                // Lista de claves del diccionario
                List<char> keys = tildes.Keys.ToList();
                foreach (char c in word.Trim().ToUpper())
                {
                    if (keys.Contains(c))
                    {
                        formatWord += $"{tildes[c]}";
                    }
                    else
                    {
                        formatWord += $"{c}";
                    }
                }
            }
            return formatWord;
        }

        /// <summary>
        /// Método que se encarga de formatear el texto para que quede todo en mayus y sacandole los tildes 
        /// que pueda llegar a tener
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string RemoveAcentMarkToLower(string word)
        {
            string formatWord = $"";
            if (!string.IsNullOrEmpty(word))
            {

                word = word.ToLower().Trim();
                //Primero le quita los tildes que vaya a tener
                // Diccionario para cambiar las letras con tilde, por otra sin tilde, así no hay dos
                // palabras sea iguales, pero que tengan tildes en lugares diferentes por un error de escritura
                Dictionary<char, char> tildes = new Dictionary<char, char>()
                {
                    {'á', 'a'},
                    {'ó','o'},
                    {'é','e'},
                    {'í','i'},
                    {'ú','u'}
                };
                // Lista de claves del diccionario
                List<char> keys = tildes.Keys.ToList();
                foreach (char c in word.Trim().ToLower())
                {
                    if (keys.Contains(c))
                    {
                        formatWord += $"{tildes[c]}";
                    }
                    else
                    {
                        formatWord += $"{c}";
                    }
                }
            }
            return formatWord;
        }
    }
}